using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System.Drawing;

namespace PersistentDataEditor.Controls;

internal class StringRender(string showString) : BaseControlItem
{
    private static Font _font;
    private static Font Font => _font ??= new Font(GH_FontServer.StandardAdjusted.FontFamily, 6);
    internal override float MinWidth => GH_FontServer.StringWidth(showString, Font);

    internal override float Height => 17;

    internal override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
    }

    internal override void RenderObject(GH_Canvas canvas, Graphics graphics, GH_PaletteStyle style)
    {
        graphics.DrawString(showString, Font, style.Text.GetBrush(), Bounds, GH_TextRenderingConstants.CenterCenter);
    }
}
