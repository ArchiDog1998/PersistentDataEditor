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
    internal class ParamPlaneControl : ParamControlBase<GH_Plane>
    {
        protected override bool Valid => base.Valid && Datas.UseParamPlaneControl;

        protected override Guid AddCompnentGuid => new Guid("cfb6b17f-ca82-4f5d-b604-d4f69f569de3");

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

        protected override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            if (obj == null) return;
            GH_Component com = (GH_Component)obj;
            if (com == null) return;

            if (com.Params.Input.Count < 2) return;

            if (com.Params.Input[0] is Param_Point)
            {
                Param_Point param = (Param_Point)com.Params.Input[0];
                param.PersistentData.Clear();
                param.PersistentData.Append(new GH_Point(OwnerGooData.Value.Origin));
            }

            if (com.Params.Input[1] is Param_Vector)
            {
                Param_Vector param = (Param_Vector)com.Params.Input[1];
                param.PersistentData.Clear();
                param.PersistentData.Append(new GH_Vector(OwnerGooData.Value.ZAxis));
            }
        }
    }
}
