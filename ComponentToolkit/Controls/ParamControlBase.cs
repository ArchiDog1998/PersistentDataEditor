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
        
        protected abstract Guid AddCompnentGuid { get; }
        protected virtual ushort AddCompnentIndex => 0;
        protected virtual string AddCompnentInit => OwnerGooData?.ToString();
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
        internal sealed override int Width
        {
            get
            {
                if (Valid)
                {
                    int max = int.MinValue;
                    foreach (var control in _controlItems)
                    {
                        max = Math.Max(max, control.Width);
                    }
                    return max;
                }
                else
                {
                    ClearLayout();
                    return 0;
                }

            }
        }
        internal sealed override int Height 
        {
            get
            {
                if (Valid)
                {
                    int all = 0;
                    foreach (var control in _controlItems)
                    {
                        all += control.Height;
                    }
                    return all;
                }
                else
                {
                    ClearLayout();
                    return 0;
                }

            }
        }
        protected override bool Valid
        {
            get
            {
                bool isActive = Owner.OnPingDocument() == Grasshopper.Instances.ActiveCanvas.Document && Owner.SourceCount == 0 && Owner.PersistentDataCount < 2;
                bool isUse = Datas.UseParamControl && ( Owner.Attributes.IsTopLevel ? Datas.ParamUseControl : Datas.ComponentUseControl);
                return isActive && isUse;
            }
        }

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
                if (goos[i] == null || !goos[i].IsValid)
                {
                    Owner.Attributes.GetTopLevel.ExpireLayout();
                    Grasshopper.Instances.ActiveCanvas.Refresh();
                    return;
                }
            }
            OwnerGooData = SetValue(goos);
        }

        protected virtual T SetValue(IGH_Goo[] values) => (T)values[0];

        protected abstract BaseControlItem[] SetControlItems(GH_PersistentParam<T> owner);

        protected sealed override void LayoutObject(RectangleF bounds)
        {
            float y = bounds.Y;
            foreach (BaseControlItem item in _controlItems)
            {
                item.Bounds = new RectangleF(Datas.ControlAlignRightLayout ? bounds.Right - item.Width : bounds.X,
                    y, item.Width, item.Height);
                y += item.Height;
            }
            base.LayoutObject(bounds);
        }

        private void ClearLayout()
        {
            foreach (var control in _controlItems)
            {
                control.Bounds = RectangleF.Empty;
            }
        }


        internal sealed override void RenderObject(GH_Canvas canvas, Graphics graphics, IGH_Component owner, GH_PaletteStyle style)
        {
            if (!Valid) return;
            foreach (var control in _controlItems)
            {
                control.RenderObject(canvas, graphics, owner, style);
            }
        }

        protected virtual void DosomethingWhenCreate(IGH_DocumentObject obj) { }

        internal sealed override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (!Valid) return;

            if (e.Button == MouseButtons.Right && AddCompnentGuid != default(Guid))
            {
                new CreateObjectItem(AddCompnentGuid, AddCompnentIndex, AddCompnentInit, true).CreateObject(Owner, DosomethingWhenCreate);
            }
            else if (e.Button == MouseButtons.Left)
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
}
