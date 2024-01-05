using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PersistentDataEditor;

internal static class GraphicUtils
{
    public static RectangleF Inflate(this RectangleF bounds,
        float left = 0, float top = 0, float right = 0, float bottom = 0)
    {
        return new(bounds.X - left, bounds.Y - top,
            bounds.Width + left + right, bounds.Height + top + bottom);
    }

    public static GraphicsPath RoundCorner(this Rectangle bounds, float radius)
    {
        return new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height).RoundCorner(radius);
    }

    public static GraphicsPath RoundCorner(this RectangleF bounds, float radius)
    {
        float diameter = radius * 2;
        SizeF size = new(diameter, diameter);
        RectangleF arc = new(bounds.Location, size);
        GraphicsPath path = new();

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

    public static GraphicsPath Triangle(this RectangleF rect)
    {
        const float radius = 1.5f;

        const float width = 12;
        float height = width / 2 * (float)Math.Sqrt(3);
        PointF center = new PointF(rect.X + width / 2, rect.Y + rect.Height / 2);

        float up = height / 2;
        PointF pt1 = new PointF(center.X - width / 2, center.Y - up + radius / 2);
        PointF pt2 = new PointF(center.X + width / 2, center.Y - up + radius / 2);
        PointF pt3 = new PointF(center.X, center.Y + up + radius / 2);

        float horiThick = radius * (float)(Math.Sqrt(3) - 1);

        GraphicsPath path = new GraphicsPath();
        path.AddArc(new RectangleF(pt1.X + horiThick, pt1.Y, 2 * radius, 2 * radius), -90, -120);
        path.AddArc(new RectangleF(pt3.X - radius, pt3.Y - 3 * radius, 2 * radius, 2 * radius), -210, -120);
        path.AddArc(new RectangleF(pt2.X - horiThick - 2 * radius, pt2.Y, 2 * radius, 2 * radius), 30, -120);

        path.CloseFigure();
        return path;
    }

    public static SolidBrush GetBrush(this Color color)
    {
        return new SolidBrush(Color.FromArgb((byte)(GH_Canvas.ZoomFadeLow * (color.A / 255f)), color));
    }

    public static GH_ObjectResponse? Respond(this BaseControlItem control, Action<GH_Canvas, GH_CanvasMouseEvent> action, GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        if (control != null && control.Bounds.Contains(e.CanvasLocation)
            && sender.Viewport.Zoom >= 0.6)
        {
            action(sender, e);

            return GH_ObjectResponse.Release;
        }
        return null;
    }
}
