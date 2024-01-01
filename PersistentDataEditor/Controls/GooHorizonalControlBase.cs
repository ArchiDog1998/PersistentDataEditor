using Grasshopper.Kernel.Types;
using System;
using System.Drawing;
using System.Linq;

namespace PersistentDataEditor;

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
                if(item is not StringRender)
                {
                    item.Width = item.MinWidth * ratio;
                }
                item.Bounds = new (x, bounds.Y + (bounds.Height - item.Height) / 2, item.Width, item.Height);
                x += item.Width;
            }
        }

        base.OnLayoutChanged(bounds);
    }
}
