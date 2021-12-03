using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
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

        protected override Guid AddCompnentGuid => new Guid("807B86E3-BE8D-4970-92B5-F8CDCB45B06B");

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

        protected override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            if (obj == null) return;
            GH_Component com = (GH_Component)obj;
            if (com == null) return;

            if (com.Params.Input.Count < 2) return;

            if (com.Params.Input[0] is Param_Plane)
            {
                Param_Plane param = (Param_Plane)com.Params.Input[0];
                param.PersistentData.Clear();
                param.PersistentData.Append(new GH_Plane(OwnerGooData.Value.Plane));
            }

            if (com.Params.Input[1] is Param_Number)
            {
                Param_Number param = (Param_Number)com.Params.Input[1];
                param.PersistentData.Clear();
                param.PersistentData.Append(new GH_Number(OwnerGooData.Value.Radius));
            }
        }
    }
}
