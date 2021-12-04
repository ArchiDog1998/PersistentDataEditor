using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal abstract class GooVerticalControlBase<T> : GooMultiControlBase<T> where T : class, IGH_Goo
    {
        internal sealed override int Width
        {
            get
            {   
                int max = 0;
                if (_hasName)
                {
                    for (int i = 1; i < _controlItems.Length; i++)
                    {
                        max = Math.Max(max, _controlItems[i].Width);
                    }
                    max += _controlItems[0].Width;
                }
                else
                {
                    foreach (var control in _controlItems)
                    {
                        max = Math.Max(max, control.Width);
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
        public GooVerticalControlBase(Func<T> valueGetter, string name) : base(valueGetter, name)
        {
            _RespondBase = false;
        }

        protected sealed override void LayoutObject(RectangleF bounds)
        {
            if (bounds == RectangleF.Empty)
            {
                foreach (BaseControlItem item in _controlItems) item.Bounds = RectangleF.Empty;
            }
            else
            {
                if (_hasName)
                {
                    _controlItems[0].Bounds = new RectangleF(bounds.X,
                        bounds.Y + bounds.Height / 2 - _controlItems[0].Height / 2, _controlItems[0].Width, _controlItems[0].Height);

                    float y = bounds.Y;
                    for (int i = 1; i < _controlItems.Length; i++)
                    {
                        BaseControlItem item = _controlItems[i];
                        item.Bounds = new RectangleF((Datas.ControlAlignRightLayout ? bounds.Right - item.Width : bounds.X + _controlItems[0].Width),
                            y, item.Width, item.Height);
                        y += item.Height;
                    }
                }
                else
                {
                    float y = bounds.Y;
                    foreach (BaseControlItem item in _controlItems)
                    {
                        item.Bounds = new RectangleF(Datas.ControlAlignRightLayout ? bounds.Right - item.Width : bounds.X,
                            y, item.Width, item.Height);
                        y += item.Height;
                    }
                }
            }

            base.LayoutObject(bounds);
        }
    }
}
