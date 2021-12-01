using Grasshopper.GUI;
using Grasshopper.GUI.Base;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComponentToolkit
{
    public abstract class ParamControlBase<T>:BaseControlItem where T : class, IGH_Goo
    {
        protected GH_PersistentParam<T> Owner { get; }
        private bool _isSaveUndo = true;
        internal T OwnerGooData
        {
            get
            {
                return Owner.PersistentData.get_FirstItem(true);
            }
            private set
            {
                if (_isSaveUndo)
                {
                    Owner.RecordUndoEvent("Set: " + value.ToString());
                    _isSaveUndo = false;
                }
                Owner.PersistentData.Clear();
                Owner.PersistentData.Append(value);
                Owner.ExpireSolution(true);
            }
        }
        internal sealed override int Width => Valid? ActualWidth : 0;
        protected int ActualWidth 
        {
            get
            {
                int all = 0;
                foreach (var control in _controlItems)
                {
                    all += control.Width;
                }
                return all;
            }
        }
        internal sealed override int Height 
        {
            get
            {
                int max = int.MinValue;
                foreach (var control in _controlItems)
                {
                    max = Math.Max(max, control.Height);
                }
                return max;
            }
        }
        protected virtual bool Valid => Owner.OnPingDocument() == Grasshopper.Instances.ActiveCanvas.Document && Owner.SourceCount == 0 && Owner.PersistentDataCount < 2;

        private BaseControlItem[] _controlItems;
        private IGooValue[] _Values;
        public ParamControlBase(GH_PersistentParam<T> owner)
        {
            _controlItems = SetControlItems(owner) ?? new BaseControlItem[0];
            List<IGooValue> gooValues = new List<IGooValue>();
            foreach (var control in _controlItems)
            {
                if (control is IGooValue)
                {
                    ((IGooValue)control).ValueChange = SetValue;
                    gooValues.Add((IGooValue)control);
                }
            }
            _Values = gooValues.ToArray();
            Owner = owner;
        }

        private void SetValue()
        {
            IGH_Goo[] goos = new IGH_Goo[_Values.Length];
            for (int i = 0; i < _Values.Length; i++)
            {
                goos[i] = _Values[i].SaveGoo;
                if (goos[i] == null) return;
                if(! goos[i].IsValid) return;
            }
            OwnerGooData = SetValue(goos);
        }

        protected virtual T SetValue(IGH_Goo[] values) => (T)values[0];

        protected abstract BaseControlItem[] SetControlItems(GH_PersistentParam<T> owner);

        protected sealed override void LayoutObject(RectangleF bounds)
        {
            float x = bounds.X;
            foreach (BaseControlItem item in _controlItems)
            {
                item.Bounds = new RectangleF(x, bounds.Y + (bounds.Height - item.Height) /2, item.Width, item.Height);
                x += item.Width;
            }
            base.LayoutObject(bounds);
        }

        internal sealed override void RenderObject(GH_Canvas canvas, Graphics graphics, IGH_Component owner, GH_PaletteStyle style)
        {
            foreach (var control in _controlItems)
            {
                control.RenderObject(canvas, graphics, owner, style);
            }
        }
        internal sealed override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            _isSaveUndo = true;
            foreach (var control in _controlItems)
            {
                if (control is StringRender) continue;
                if (control.Bounds.Contains(e.CanvasLocation))
                {
                    control.Clicked(sender, e);
                    return;
                }
            }
            new InputBoxBalloon(Bounds, SaveString).ShowTextInputBox(sender, OwnerGooData.ToString(), true, true, sender.Viewport.XFormMatrix(GH_Viewport.GH_DisplayMatrix.CanvasToControl));

            void SaveString(string str)
            {
                T value = (T)Activator.CreateInstance(typeof(T));
                if (value.CastFrom(str))
                {
                    OwnerGooData = value;
                }
                else
                {
                    MessageBox.Show($"Can't cast a {typeof(T).Name} from \"{str}\".");
                }
            }
        }
    }
}
