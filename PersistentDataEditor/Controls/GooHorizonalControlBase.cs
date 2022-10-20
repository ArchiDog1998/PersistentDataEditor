using Grasshopper.Kernel.Types;
using System;
using System.Drawing;
using System.Linq;

namespace PersistentDataEditor
{
    internal abstract class GooHorizonalControlBase<T> : GooMultiControlBase<T> where T : class, IGH_Goo
    {
        internal sealed override int Width 
            => _controlItems.Sum(control => control.Width);

        internal sealed override int Height 
            => _controlItems.Select(control => control.Height).Prepend(0).Max();

        protected GooHorizonalControlBase(Func<T> valueGetter, Func<bool> isNull, string name) : base(valueGetter, isNull, name)
        {
        }

        protected sealed override void LayoutObject(RectangleF bounds)
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
                    item.Bounds = new RectangleF(x, bounds.Y + (bounds.Height - item.Height) / 2, item.Width, item.Height);
                    x += item.Width;
                }
            }

            base.LayoutObject(bounds);
        }
    }
}
