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

namespace ComponentToolkit
{
    internal abstract class GooInputBoxControl<T> : GooControlBase<T> where T : class, IGH_Goo
    {
        protected virtual string ShowString => Value?.ToString();
        internal override int Height => 14;
        internal override int Width => Math.Min(Math.Max(GH_FontServer.StringWidth(ShowString, GH_FontServer.StandardAdjusted), 15), 100);

        private GraphicsPath path;

        public GooInputBoxControl(Func<T> valueGetter, Action<T, bool> valueChanged) : base(valueGetter, valueChanged)
        {
        }

        internal override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            new InputBoxBalloon(Bounds, SaveString).ShowTextInputBox(sender, ShowString, true, true, sender.Viewport.XFormMatrix(GH_Viewport.GH_DisplayMatrix.CanvasToControl));
        }

        protected abstract void SaveString(string str);

        protected override void LayoutObject(RectangleF bounds)
        {
            path = RoundedRect(GH_Convert.ToRectangle(bounds), 2);
            base.LayoutObject(bounds);
        }

        internal override void RenderObject(GH_Canvas canvas, Graphics graphics, IGH_Component owner, GH_PaletteStyle style)
        {
            graphics.FillPath(new SolidBrush(ControlBackgroundColor), path);
            graphics.DrawPath(new Pen(new SolidBrush(ControlBorderColor)), path);
            Color color = IsNull ? Color.DarkRed : ControlTextgroundColor;
            graphics.DrawString(ShowString, GH_FontServer.StandardAdjusted, new SolidBrush(color), Bounds, GH_TextRenderingConstants.NearCenter);
        }
    }

    internal class InputBoxBalloon : GH_TextBoxInputBase
    {
        private Action<string> _setValue;


        internal InputBoxBalloon(RectangleF bounds, Action<string> setValue)
        {

            this._setValue = setValue;

            float mul = 3.5f;
            if (bounds.Height * mul > bounds.Width)
            {
                bounds = new RectangleF(bounds.Location, new SizeF(bounds.Height * mul, bounds.Height));
            }

            Bounds = GH_Convert.ToRectangle(bounds);
            Font = GH_FontServer.ConsoleAdjusted;
        }

        protected override void HandleTextInputAccepted(string text)
        {
            _setValue(text);
        }
    }
}
