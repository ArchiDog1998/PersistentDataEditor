using Grasshopper.Kernel.Types;
using System;
using System.Drawing;
using System.Linq;

namespace PersistentDataEditor.Controls;

internal abstract class GooHorizonalControlBase<T>(Func<T> valueGetter, Func<bool> isNull, string name)
    : GooMultiControlBase<T>(valueGetter, isNull, name) where T : class, IGH_Goo
{
    internal sealed override float MinWidth
        => _controlItems.Sum(control => control.MinWidth);

    internal sealed override float Height
        => _controlItems.Select(control => control.Height).Prepend(0).Max();

    protected sealed override void OnLayoutChanged(RectangleF bounds)
    {
        if (bounds == RectangleF.Empty)
        {
            foreach (BaseControlItem item in _controlItems) item.Bounds = RectangleF.Empty;
        }
        else
        {
            var stringRendererSum = _controlItems.Where(i => i is StringRender).Sum(ctrl => ctrl.MinWidth);

            var ratio = (bounds.Width - stringRendererSum) / (MinWidth - stringRendererSum);

            float x = bounds.X;
            foreach (BaseControlItem item in _controlItems)
            {
                var width = item.MinWidth;
                if (item is not StringRender)
                {
                    width *= ratio;
                }
                item.Bounds = new(x, bounds.Y + (bounds.Height - item.Height) / 2, width, item.Height);
                x += width;
            }
        }

        base.OnLayoutChanged(bounds);
    }
}
