using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel.Types;
using System;
using System.Drawing;
using System.Linq;

namespace PersistentDataEditor;

internal abstract class GooVerticalControlBase<T> : GooMultiControlBase<T> where T : class, IGH_Goo
{
    PointF _start, _end;

    internal sealed override int MinWidth
    {
        get
        {
            int max = 0;
            if (_hasName)
            {
                for (int i = 1; i < _controlItems.Length; i++)
                {
                    max = Math.Max(max, _controlItems[i].MinWidth);
                }
                max += _controlItems[0].MinWidth;
            }
            else
            {
                foreach (var control in _controlItems)
                {
                    max = Math.Max(max, control.MinWidth);
                }
            }

            return max;
        }
    }

    internal sealed override int Height
    {
        get
        {
            int all = 0;
            for (int i = _hasName ? 1 : 0; i < _controlItems.Length; i++)
            {
                all += _controlItems[i].Height;
            }
            return all;
        }
    }

    protected GooVerticalControlBase(Func<T> valueGetter, Func<bool> isNull, string name) : base(valueGetter, isNull, name)
    {
        _RespondBase = false;
    }

    internal override void RenderObject(GH_Canvas canvas, Graphics graphics, GH_PaletteStyle style)
    {
        if (canvas.Viewport.Zoom > 0.9f)
        {
            graphics.DrawLine(new Pen(Data.ControlForegroundColor, 0.5f), _start, _end);
        }
        base.RenderObject(canvas, graphics, style);
    }

    protected sealed override void OnLayoutChanged(RectangleF bounds)
    {
        if (bounds == RectangleF.Empty)
        {
            foreach (BaseControlItem item in _controlItems) item.Bounds = RectangleF.Empty;
        }
        else
        {
            float margin = 3;

            if (_hasName)
            {
                _controlItems[0].Bounds = new RectangleF(bounds.X,
                    bounds.Y + bounds.Height / 2 - _controlItems[0].Height / 2, _controlItems[0].MinWidth, _controlItems[0].Height);

                _start = new PointF(_controlItems[0].Bounds.Right, bounds.Top + margin);
                _end = new PointF(_controlItems[0].Bounds.Right, bounds.Bottom - margin);

                float y = bounds.Y;
                for (int i = 1; i < _controlItems.Length; i++)
                {
                    BaseControlItem item = _controlItems[i];
                    item.Width = (int)bounds.Width;
                    item.Bounds = new RectangleF(bounds.X, y, bounds.Width, item.Height);
                    y += item.Height;
                }
            }
            else
            {
                _start = new PointF(bounds.X, bounds.Top + margin);
                _end = new PointF(bounds.X, bounds.Bottom - margin);

                float y = bounds.Y;
                foreach (BaseControlItem item in _controlItems)
                {
                    item.Width = (int)bounds.Width;
                    item.Bounds = new RectangleF(bounds.X, y, bounds.Width, item.Height);
                    y += item.Height;
                }
            }
        }

        base.OnLayoutChanged(bounds);
    }
}
