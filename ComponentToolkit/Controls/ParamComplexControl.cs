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
    }
}
