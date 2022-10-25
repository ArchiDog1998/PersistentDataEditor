using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel.Types;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PersistentDataEditor
{
    internal class GooBooleanControl : GooControlBase<GH_Boolean>
    {
        public override Guid AddCompnentGuid => new Guid("2e78987b-9dfb-42a2-8b76-3923ac8bd91a");

        internal override int Width => 10;

        internal override int Height => 10;

        public GooBooleanControl(Func<GH_Boolean> valueGetter, Func<bool> isNull) : base(valueGetter, isNull)
        {
        }

        internal override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == MouseButtons.Left)
            {
                bool lastBool = ShowValue?.Value ?? false;
                ShowValue = new GH_Boolean(!lastBool);
                return;
            }
            base.Clicked(sender, e);
        }

        internal override void RenderObject(GH_Canvas canvas, Graphics graphics, GH_PaletteStyle style)
        {
            graphics.FillEllipse(new SolidBrush(Datas.ControlBackgroundColor), Bounds);
            graphics.DrawEllipse(new Pen(new SolidBrush(Datas.ControlBorderColor), 1.5f), Bounds);

            if (ShowValue == null || !ShowValue.Value) return;
            RectangleF bound = Bounds;
            bound.Inflate(-2, -2);

            graphics.FillEllipse(new SolidBrush(Datas.ControlForegroundColor), bound);
        }
    }
}
