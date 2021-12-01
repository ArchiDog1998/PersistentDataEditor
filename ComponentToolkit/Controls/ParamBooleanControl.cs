using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class ParamBooleanControl : ParamControlBase<GH_Boolean>
    {
        protected override bool Valid => base.Valid && GH_ComponentAttributesReplacer.UseParamBooleanControl;

        internal ParamBooleanControl(GH_PersistentParam<GH_Boolean> owner) : base(owner)
        {
        }

        protected override BaseControlItem[] SetControlItems(GH_PersistentParam<GH_Boolean> owner)
        {
            return new BaseControlItem[]
            {
                new GooBooleanControl(()=> OwnerGooData),
            };
        }
    }
}
