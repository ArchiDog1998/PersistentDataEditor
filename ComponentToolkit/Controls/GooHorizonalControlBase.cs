using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ComponentToolkit
{
    internal abstract class GooHorizonalControlBase<T> : GooMultiControlBase<T> where T : class, IGH_Goo
    {
        internal sealed override int Width
        {
            get
            {
                int all = 0;
                foreach (var control in _controlItems)
                {
                    all += control.Width;
                }
                return all;
            }
        }
        internal sealed override int Height
        {
            get
            {
                int max = 0;
                foreach (var control in _controlItems)
                {
                    max = Math.Max(max, control.Height);
                }
                return max;
            }
        }
        public GooHorizonalControlBase(Func<T> valueGetter, string name) : base(valueGetter, name)
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
