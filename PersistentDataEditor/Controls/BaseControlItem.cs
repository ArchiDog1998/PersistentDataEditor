using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using System;
using System.Drawing;
using System.Windows;

namespace PersistentDataEditor;

public abstract class BaseControlItem
{
    internal static bool ShouldRespond = true;
    private RectangleF _bounds;
    internal RectangleF Bounds
    {
        get => _bounds;
        set
        {
            if (!Valid)
            {
                _bounds = RectangleF.Empty;
                return;
            }

            var shrinkHeight = -(value.Height - Height) / 2;
            value = value.Inflate(0, shrinkHeight, 0, shrinkHeight);

            var shrinkWidth = -(value.Width - MinWidth) / 2;
            switch (Data.ControlAlignment)
            {
                case HorizontalAlignment.Left:
                    value = value.Inflate(0, 0, 2 * shrinkWidth, 0);
                    break;
                case HorizontalAlignment.Right:
                    value = value.Inflate(2 * shrinkWidth, 0, 0, 0);
                    break;
                case HorizontalAlignment.Center:
                    value = value.Inflate(shrinkWidth, 0, shrinkWidth, 0);
                    break;
                default:
                    break;
            }

            _bounds = value;
            OnLayoutChanged(_bounds);
        }
    }
    protected virtual bool Valid => true;
    internal abstract float MinWidth { get; }
    internal abstract float Height { get; }
    protected virtual void OnLayoutChanged(RectangleF bounds) { }
    internal virtual void ChangeControlItems() { }
    internal abstract void RenderObject(GH_Canvas canvas, Graphics graphics, GH_PaletteStyle style);
    internal abstract void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e);
    internal virtual void MouseDown(GH_Canvas sender, GH_CanvasMouseEvent e) { }
    internal virtual void MouseMove(GH_Canvas sender, GH_CanvasMouseEvent e) { }
}
