using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows;

namespace ComponentToolkit
{
    public abstract class BaseControlItem
    {

        private RectangleF _bounds;
        internal RectangleF Bounds
        {
            get { return _bounds; }
            set
            {
                _bounds = value;
                LayoutObject(value);
            }
        }

        internal abstract int Width { get; }
        internal abstract int Height { get; }
        protected virtual void LayoutObject(RectangleF bounds) { }
        internal abstract void RenderObject(GH_Canvas canvas, Graphics graphics, IGH_Component owner, GH_PaletteStyle style);
        internal abstract void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e);

        protected static GraphicsPath RoundedRect(RectangleF bounds, float radius)
        {
            float diameter = radius * 2;
            SizeF size = new SizeF(diameter, diameter);
            RectangleF arc = new RectangleF(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}
