using Grasshopper.Kernel.Types;
using System;
using System.Drawing;
using System.Linq;

namespace PersistentDataEditor;

internal abstract class GooHorizonalControlBase<T>(Func<T> valueGetter, Func<bool> isNull, string name)
    : GooMultiControlBase<T>(valueGetter, isNull, name) where T : class, IGH_Goo
{
    internal sealed override int MinWidth
        => _controlItems.Sum(control => control.MinWidth);

    internal sealed override int Height
        => _controlItems.Select(control => control.Height).Prepend(0).Max();

    protected sealed override void OnLayoutChanged(RectangleF bounds)
    {
        if (bounds == RectangleF.Empty)
        {
            foreach (BaseControlItem item in _controlItems) item.Bounds = RectangleF.Empty;
        }
        else
        {
            float x = bounds.X;
            foreach (BaseControlItem item in _controlItems)
            {
                item.Bounds = new RectangleF(x, bounds.Y + (bounds.Height - item.Height) / 2, item.MinWidth, item.Height);
                x += item.MinWidth;
            }
        }

        base.OnLayoutChanged(bounds);
    }
}
