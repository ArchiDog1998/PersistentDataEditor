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
    public abstract class InputBoxControl<T> : ControlItem<T> where T : class, IGH_Goo
    {
        public virtual string ShowString { get; }
        public override int Height => 14;
        public override int Width => Valid ? Math.Max(GH_FontServer.StringWidth(ShowString, GH_FontServer.StandardAdjusted), 15) : 0;

        public InputBoxControl(GH_PersistentParam<T> owner) : base(owner)
        {
        }
        public override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            new InputBoxBalloon(Bounds, Write).ShowTextInputBox(sender, ShowString, selectContent: true, limitToBoundary: true, sender.Viewport.XFormMatrix(GH_Viewport.GH_DisplayMatrix.CanvasToControl));
            void Write(string str)
            {
                Save(str);
                Owner.ExpireSolution(true);
                Grasshopper.Instances.ActiveCanvas.Refresh();
            }
        }

        protected abstract void Save(string str);

        public sealed override void RenderObject(GH_Canvas canvas, Graphics graphics, IGH_Component owner, GH_PaletteStyle style)
        {
            GraphicsPath path = RoundedRect(GH_Convert.ToRectangle(Bounds), 2);
            graphics.FillPath(new SolidBrush(Color.FromArgb(150, Color.WhiteSmoke)), path);
            graphics.DrawPath(new Pen(new SolidBrush(Color.Black)), path);
            graphics.DrawString(ShowString, GH_FontServer.StandardAdjusted, new SolidBrush(Color.Black), Bounds, GH_TextRenderingConstants.CenterCenter);

        }
    }

    internal class InputBoxBalloon : GH_TextBoxInputBase
    {

        public Action<string> SetValue { get; }


        public InputBoxBalloon(RectangleF bounds, Action<string> setValue)
        {

            this.SetValue = setValue;

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
            SetValue(text);
        }
    }
}
