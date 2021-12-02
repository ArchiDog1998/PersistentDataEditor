using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class GooIntervalControl : GooMultiControlBase<GH_Interval>
    {
        public GooIntervalControl(Func<GH_Interval> valueGetter, string name) : base(valueGetter, name)
        {
        }

        protected override BaseControlItem[] SetControlItems()
        {
            return new BaseControlItem[]
            {
                new GooInputBoxControl<GH_Number>(()=>
                {
                    if(ShowValue == null) return null;
                    return new GH_Number(ShowValue.Value.T0);
                }),

                new StringRender("To"),

                new GooInputBoxControl<GH_Number>(()=>
                {
                    if(ShowValue == null) return null;
                    return new GH_Number(ShowValue.Value.T1);
                }),
            };
        }

        protected override GH_Interval SetValue(IGH_Goo[] values)
        {
            return new GH_Interval(new Interval(((GH_Number)values[0]).Value, ((GH_Number)values[1]).Value));
        }
    }
}
