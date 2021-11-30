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
        internal override int Width => 10;

        internal override int Height => 10;

        public GooBooleanControl(Func<GH_Boolean> valueGetter, Action<GH_Boolean, bool> valueChanged) : base(valueGetter, valueChanged)
        {
        }

        internal override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            bool lastBool = Value?.Value ?? false;
            Value = new GH_Boolean(!lastBool);
        }

        internal override void RenderObject(GH_Canvas canvas, Graphics graphics, IGH_Component owner, GH_PaletteStyle style)
        {
            graphics.FillEllipse(new SolidBrush(ControlBackgroundColor), Bounds);
            graphics.DrawEllipse(new Pen(new SolidBrush(ControlBorderColor), 1.5f), Bounds);

            if (Value != null && Value.Value)
            {
                RectangleF bound = this.Bounds;
                bound.Inflate(-2, -2);

                graphics.FillEllipse(new SolidBrush(ControlForegroundColor), bound);
            }
        }
    }
}
