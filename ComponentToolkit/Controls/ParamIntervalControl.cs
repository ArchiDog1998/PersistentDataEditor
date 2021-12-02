using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class ParamIntervalControl : ParamControlBase<GH_Interval>
    {
        protected override bool Valid => base.Valid && Datas.UseParamIntervalControl;

        public ParamIntervalControl(GH_PersistentParam<GH_Interval> owner) : base(owner)
        {
        }

        protected override BaseControlItem[] SetControlItems(GH_PersistentParam<GH_Interval> owner)
        {
            return new BaseControlItem[]
            {
                new GooInputBoxControl<GH_Number>(()=>
                {
                    if(OwnerGooData == null) return null;
                    return new GH_Number(OwnerGooData.Value.T0);
                }),

                new StringRender("To"),

                new GooInputBoxControl<GH_Number>(()=>
                {
                    if(OwnerGooData == null) return null;
                    return new GH_Number(OwnerGooData.Value.T1);
                }),
            };
        }

        protected override GH_Interval SetValue(IGH_Goo[] values)
        {
            return new GH_Interval(new Interval (((GH_Number)values[0]).Value, ((GH_Number)values[1]).Value));
        }
    }
}
