using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class StringRender : BaseControlItem
    {
        private static Font _font;
        private static Font Font
        {
            get
            {
                if (_font == null)
                {
                    _font = new Font(GH_FontServer.StandardAdjusted.FontFamily, 6);
                }
                return _font;
            }
        }
        internal override int Width => GH_FontServer.StringWidth(_showString, Font);

        internal override int Height => 20;

        private string _showString;
        private Action<GH_Canvas, GH_CanvasMouseEvent> _click;

        public StringRender(string showString, Action<GH_Canvas, GH_CanvasMouseEvent> click = null)
        {
            _showString = showString;
            _click = click;
        }

        internal override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if(_click != null) _click(sender, e);
        }

        internal override void RenderObject(GH_Canvas canvas, Graphics graphics, IGH_Component owner, GH_PaletteStyle style)
        {
            SolidBrush solidBrush = new SolidBrush(Color.FromArgb(GH_Canvas.ZoomFadeLow, style.Text));
            graphics.DrawString(_showString, Font, solidBrush, Bounds, GH_TextRenderingConstants.CenterCenter);
        }
    }
}
