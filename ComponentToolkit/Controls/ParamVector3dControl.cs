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
    internal class ParamVector3dControl : ParamControlBase<GH_Vector>
    {
        protected override bool Valid => base.Valid && GH_ComponentAttributesReplacer.UseParamVectorControl;

        public ParamVector3dControl(GH_PersistentParam<GH_Vector> owner) : base(owner)
        {
        }

        protected override BaseControlItem[] SetControlItems(GH_PersistentParam<GH_Vector> owner)
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

        protected override GH_Vector SetValue(IGH_Goo[] values)
        {
            return new GH_Vector(new Vector3d(
                ((GH_Number)values[0]).Value,
                ((GH_Number)values[1]).Value,
                ((GH_Number)values[2]).Value));
        }
    }
}
