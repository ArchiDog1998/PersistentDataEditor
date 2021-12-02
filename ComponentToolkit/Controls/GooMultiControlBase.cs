using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ComponentToolkit
{
    internal abstract class GooMultiControlBase<T> : GooControlBase<T> where T : class, IGH_Goo
    {
        internal sealed override int Width
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
                int max = 0;
                foreach (var control in _controlItems)
                {
                    max = Math.Max(max, control.Height);
                }
                return max;
            }
        }
        private BaseControlItem[] _controlItems;
        private IGooValue[] _Values;
        public GooMultiControlBase(Func<T> valueGetter, string name) : base(valueGetter)
        {
            BaseControlItem[] addcontrolItems = SetControlItems() ?? new BaseControlItem[0];
            if (string.IsNullOrEmpty(name))
            {
                _controlItems = addcontrolItems;
            }
            else
            {
                List<BaseControlItem> items = new List<BaseControlItem> { new StringRender(name) };
                items.AddRange(addcontrolItems);
                _controlItems = items.ToArray();
            }

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
        }
        private void SetValue()
        {
            IGH_Goo[] goos = new IGH_Goo[_Values.Length];
            for (int i = 0; i < _Values.Length; i++)
            {
                goos[i] = _Values[i].SaveGoo;
                if (goos[i] == null || !goos[i].IsValid)
                {
                    base.ValueChange();
                    return;
                }
            }
            ShowValue = SetValue(goos);
        }

        protected abstract T SetValue(IGH_Goo[] values);
        protected abstract BaseControlItem[] SetControlItems();

        protected sealed override void LayoutObject(RectangleF bounds)
        {
            if (bounds == RectangleF.Empty)
            {
                foreach (BaseControlItem item in _controlItems) item.Bounds = RectangleF.Empty;
            }
            else
            {
                float x = bounds.X;
                foreach (BaseControlItem item in _controlItems)
                {
                    item.Bounds = new RectangleF(x, bounds.Y + (bounds.Height - item.Height) / 2, item.Width, item.Height);
                    x += item.Width;
                }
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
            foreach (var control in _controlItems)
            {
                if (control is StringRender) continue;
                if (control.Bounds.Contains(e.CanvasLocation))
                {
                    control.Clicked(sender, e);
                    return;
                }
            }
            new InputBoxBalloon(Bounds, SaveString).ShowTextInputBox(sender, ShowValue?.ToString(), true, true, sender.Viewport.XFormMatrix(GH_Viewport.GH_DisplayMatrix.CanvasToControl));

            void SaveString(string str)
            {
                T value = (T)Activator.CreateInstance(typeof(T));
                if (value.CastFrom(str))
                {
                    ShowValue = value;
                }
                else
                {
                    MessageBox.Show($"Can't cast a {typeof(T).Name} from \"{str}\".");
                }
            }
        }
    }
}
