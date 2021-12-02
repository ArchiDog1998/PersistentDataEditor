using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class ParamPlaneControl : ParamControlBase<GH_Plane>
    {
        protected override bool Valid => base.Valid && Datas.UseParamPlaneControl;

        public ParamPlaneControl(GH_PersistentParam<GH_Plane> owner) : base(owner)
        {

        }

        protected override BaseControlItem[] SetControlItems(GH_PersistentParam<GH_Plane> owner)
        {
            return new BaseControlItem[]
            {
                new GooPointControl(()=>
                {
                    if(OwnerGooData == null) return null;
                    return new GH_Point(OwnerGooData.Value.Origin);
                }, "Origin:"),

                new GooVectorControl(()=>
                {
                    if(OwnerGooData == null) return null;
                    return new GH_Vector(OwnerGooData.Value.Normal);
                }, "Normal:"),
            };
        }

        protected override GH_Plane SetValue(IGH_Goo[] values)
        {
            return new GH_Plane(new Rhino.Geometry.Plane(((GH_Point)values[0]).Value, ((GH_Vector)values[1]).Value));
        }
    }
}
