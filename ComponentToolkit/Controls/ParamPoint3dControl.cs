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
    internal class ParamPoint3dControl : ParamControlBase<GH_Point>
    {
        protected override bool Valid => base.Valid && Datas.UseParamPointControl;

        public ParamPoint3dControl(GH_PersistentParam<GH_Point> owner) : base(owner)
        {
        }


        protected override BaseControlItem[] SetControlItems(GH_PersistentParam<GH_Point> owner)
        {
            return new BaseControlItem[]
            {
                new StringRender("X:"),

                new GooInputBoxControl<GH_Number>(()=>
                {
                    if(OwnerGooData == null) return null;
                    return new GH_Number(OwnerGooData.Value.X);
                }),

                new StringRender("Y:"),

                new GooInputBoxControl<GH_Number>(()=>
                {
                    if(OwnerGooData == null) return null;
                    return new GH_Number(OwnerGooData.Value.Y);
                }),

                new StringRender("Z:"),

                new GooInputBoxControl<GH_Number>(()=>
                {
                    if(OwnerGooData == null) return null;
                    return new GH_Number(OwnerGooData.Value.Z);
                }),
            };
        }

        protected override GH_Point SetValue(IGH_Goo[] values)
        {
            return new GH_Point(new Point3d(
                ((GH_Number)values[0]).Value, 
                ((GH_Number)values[1]).Value,
                ((GH_Number)values[2]).Value));
        }
    }
}
