using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class ParamNumberControl : ParamControlBase<GH_Number>
    {
        internal ParamNumberControl(GH_PersistentParam<GH_Number> owner) : base(owner)
        {
            ControlItems = new BaseControlItem[]
            {
                new GooNumberControl(()=> Owner.PersistentData.get_FirstItem(true), SetValue),
            };
        }

        protected override void LayoutObject(RectangleF bounds)
        {
            ControlItems[0].Bounds = bounds;
            base.LayoutObject(bounds);
        }
    }
}
