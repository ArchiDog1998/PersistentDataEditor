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
    public interface IControlItem
    {
        RectangleF Bounds { get; set; }
        bool Valid { get; }
        int Width { get; }
        int Height { get; }
        void RenderObject(GH_Canvas canvas, Graphics graphics, IGH_Component owner, GH_PaletteStyle style);

        void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e);

    }
}
