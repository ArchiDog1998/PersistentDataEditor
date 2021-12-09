using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System.Drawing;

namespace ComponentToolkit
{
    internal abstract class ParamControlBase<T> : BaseControlItem where T : class, IGH_Goo
    {
        protected GH_PersistentParam<T> Owner { get; }
        private bool _isSaveUndo = true;
        internal T OwnerGooData
        {
            get
            {
                T value = Owner.PersistentData.get_FirstItem(true);
                IsNull = value == null;
                return value;
            }
            private set
            {
                if (_isSaveUndo)
                {
                    Owner.RecordUndoEvent("Set: " + value?.ToString());
                    _isSaveUndo = false;
                }
                Owner.PersistentData.Clear();
                if (value != null)
                    Owner.PersistentData.Append(value);
                Owner.ExpireSolution(true);
            }
        }

        protected bool IsNull { get; private set; }

        private GooControlBase<T> _gooControl;

        internal sealed override int Width
        {
            get
            {
                if (Valid)
                {
                    return _gooControl.Width;
                }
                else
                {
                    _gooControl.Bounds = RectangleF.Empty;
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
                    return _gooControl.Height;
                }
                else
                {
                    _gooControl.Bounds = RectangleF.Empty;
                    return 0;
                }

            }
        }
        protected override bool Valid => Owner.OnPingDocument() == Instances.ActiveCanvas.Document && Owner.SourceCount == 0 && Owner.PersistentDataCount < 2;

        public ParamControlBase(GH_PersistentParam<T> owner)
        {
            Owner = owner;
            _gooControl = SetUpControl(owner);
            _gooControl.Owner = owner;
            _gooControl.ValueChange = SetValue;
        }

        private void SetValue()
        {
            OwnerGooData = (T)(object)_gooControl.SaveValue;
        }
        protected abstract GooControlBase<T> SetUpControl(IGH_Param param);


        protected sealed override void LayoutObject(RectangleF bounds)
        {
            this._gooControl.Bounds = bounds;
            base.LayoutObject(bounds);
        }
        internal sealed override void RenderObject(GH_Canvas canvas, Graphics graphics, IGH_Component owner, GH_PaletteStyle style)
        {
            if (!Valid) return;
            _gooControl.RenderObject(canvas, graphics, owner, style);
        }
        internal override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (!Valid || !ShouldRespond) return;
            if(e.Button == System.Windows.Forms.MouseButtons.Left) _isSaveUndo = true;
            _gooControl.Clicked(sender, e);
        }

        internal override void ChangeControlItems()
        {
            this._gooControl.ChangeControlItems();
        }

    }
}
