using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class ParamCircleControl : ParamControlBase<GH_Circle>
    {
        protected override bool Valid => base.Valid && Datas.UseParamCircleControl;

        protected override Guid AddCompnentGuid => new Guid("d114323a-e6ee-4164-946b-e4ca0ce15efa");

        public ParamCircleControl(GH_PersistentParam<GH_Circle> owner) : base(owner)
        {

        }

        protected override BaseControlItem[] SetControlItems(GH_PersistentParam<GH_Circle> owner)
        {
            return new BaseControlItem[]
            {
                new GooPointControl(()=>
                {
                    if(OwnerGooData == null) return null;
                    return new GH_Point(OwnerGooData.Value.Plane.Origin);
                }, "Center:"),

                new GooVectorControl(()=>
                {
                    if(OwnerGooData == null) return null;
                    return new GH_Vector(OwnerGooData.Value.Plane.Normal);
                }, "Normal:"),

                new GooNamedInputBoxControl<GH_Number>(()=>
                {
                    if(OwnerGooData == null) return null;
                    return new GH_Number(OwnerGooData.Value.Radius);
                }, "Radius:"),
            };
        }

        protected override GH_Circle SetValue(IGH_Goo[] values)
        {
            return new GH_Circle(new Rhino.Geometry.Circle(new Rhino.Geometry.Plane(((GH_Point)values[0]).Value, ((GH_Vector)values[1]).Value),
                ((GH_Number)values[2]).Value));
        }
    }
}
