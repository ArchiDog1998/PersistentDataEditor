using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class ParamComplexControl : ParamControlBase<GH_ComplexNumber>
    {
        protected override bool Valid => base.Valid && Datas.UseParamComplexControl;

        protected override Guid AddCompnentGuid => new Guid("63d12974-2915-4ccf-ac26-5d566c3bac92");

        public ParamComplexControl(GH_PersistentParam<GH_ComplexNumber> owner) : base(owner)
        {
        }

        protected override BaseControlItem[] SetControlItems(GH_PersistentParam<GH_ComplexNumber> owner)
        {
            return new BaseControlItem[]
            {
                new GooComplexControl(()=> OwnerGooData, null),
            };
        }

        protected override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            if (obj == null) return;
            GH_Component com = (GH_Component)obj;
            if (com == null) return;

            if (com.Params.Input.Count < 2) return;

            if (com.Params.Input[0] is Param_Number)
            {
                Param_Number param = (Param_Number)com.Params.Input[0];
                param.PersistentData.Clear();
                param.PersistentData.Append(new GH_Number(OwnerGooData.Value.Real));
            }

            if (com.Params.Input[1] is Param_Number)
            {
                Param_Number param = (Param_Number)com.Params.Input[1];
                param.PersistentData.Clear();
                param.PersistentData.Append(new GH_Number(OwnerGooData.Value.Imaginary));
            }
        }
    }
}
