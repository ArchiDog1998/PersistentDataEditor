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

namespace ComponentToolkit
{
    internal class GooBooleanControl : GooControlBase<GH_Boolean>
    {
        protected override Guid AddCompnentGuid => new Guid("2e78987b-9dfb-42a2-8b76-3923ac8bd91a");

        internal override int Width => 10;

        internal override int Height => 10;

        public GooBooleanControl(Func<GH_Boolean> valueGetter) : base(valueGetter)
        {
        }

        internal override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            bool lastBool = ShowValue?.Value ?? false;
            ShowValue = new GH_Boolean(!lastBool);
        }

        internal override void RenderObject(GH_Canvas canvas, Graphics graphics, IGH_Component owner, GH_PaletteStyle style)
        {
            graphics.FillEllipse(new SolidBrush(Datas.ControlBackgroundColor), Bounds);
            graphics.DrawEllipse(new Pen(new SolidBrush(Datas.ControlBorderColor), 1.5f), Bounds);

            if (ShowValue != null && ShowValue.Value)
            {
                RectangleF bound = this.Bounds;
                bound.Inflate(-2, -2);

                graphics.FillEllipse(new SolidBrush(Datas.ControlForegroundColor), bound);
            }
        }
    }
}
