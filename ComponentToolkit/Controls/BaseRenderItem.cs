using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal abstract class BaseRenderItem
    {
        public static readonly Color _controlForegroundColorDefault = Color.FromArgb(40, 40, 40);
        public static Color ControlForegroundColor
        {
            get => Grasshopper.Instances.Settings.GetValue(nameof(ControlForegroundColor), _controlForegroundColorDefault);
            set
            {
                Grasshopper.Instances.Settings.SetValue(nameof(ControlForegroundColor), value);
                Grasshopper.Instances.RedrawCanvas();
            }
        }

        public static readonly Color _controlTextgroundColorDefault = Color.DimGray;
        public static Color ControlTextgroundColor
        {
            get => Grasshopper.Instances.Settings.GetValue(nameof(ControlTextgroundColor), _controlTextgroundColorDefault);
            set
            {
                Grasshopper.Instances.Settings.SetValue(nameof(ControlTextgroundColor), value);
                Grasshopper.Instances.RedrawCanvas();
            }
        }

        public static readonly Color _controlBackgroundColorDefault = Color.FromArgb(150, Color.WhiteSmoke);
        public static Color ControlBackgroundColor
        {
            get => Grasshopper.Instances.Settings.GetValue(nameof(ControlBackgroundColor), _controlBackgroundColorDefault);
            set
            {
                Grasshopper.Instances.Settings.SetValue(nameof(ControlBackgroundColor), value);
                Grasshopper.Instances.RedrawCanvas();
            }
        }

        public static readonly Color _controlBorderColorDefault = Color.Black;
        public static Color ControlBorderColor
        {
            get => Grasshopper.Instances.Settings.GetValue(nameof(ControlBorderColor), _controlBorderColorDefault);
            set
            {
                Grasshopper.Instances.Settings.SetValue(nameof(ControlBorderColor), value);
                Grasshopper.Instances.RedrawCanvas();
            }
        }

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
