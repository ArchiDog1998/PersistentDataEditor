using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;

namespace ComponentToolkit
{
    internal abstract class BaseControlItem : BaseRenderItem
    {
       internal abstract void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e);
    }
}
