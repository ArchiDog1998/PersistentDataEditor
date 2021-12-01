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
    internal class ParamInterval2DControl : ParamControlBase<GH_Interval2D>
    {
        protected override bool Valid => base.Valid && GH_ComponentAttributesReplacer.UseParamInterval2DControl;

        public ParamInterval2DControl(GH_PersistentParam<GH_Interval2D> owner) : base(owner)
        {
        }

        protected override BaseControlItem[] SetControlItems(GH_PersistentParam<GH_Interval2D> owner)
        {
            return new BaseControlItem[]
            {
                new StringRender("U0:"),

                new GooInputBoxControl<GH_Number>(()=>
                {
                    if(OwnerGooData == null) return null;
                    return new GH_Number(OwnerGooData.Value.U0);
                }),

                new StringRender("U1:"),

                new GooInputBoxControl<GH_Number>(()=>
                {
                    if(OwnerGooData == null) return null;
                    return new GH_Number(OwnerGooData.Value.U1);
                }),

                new StringRender("V0:"),

                new GooInputBoxControl<GH_Number>(()=>
                {
                    if(OwnerGooData == null) return null;
                    return new GH_Number(OwnerGooData.Value.V0);
                }),

                new StringRender("V1:"),

                new GooInputBoxControl<GH_Number>(()=>
                {
                    if(OwnerGooData == null) return null;
                    return new GH_Number(OwnerGooData.Value.V1);
                }),
            };
        }

        protected override GH_Interval2D SetValue(IGH_Goo[] values)
        {
            return new GH_Interval2D(new UVInterval(new Interval( ((GH_Number)values[0]).Value, ((GH_Number)values[1]).Value),
                new Interval(((GH_Number)values[2]).Value, ((GH_Number)values[3]).Value)));

        }
    }
}
