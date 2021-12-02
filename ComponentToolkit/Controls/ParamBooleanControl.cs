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
        protected override bool Valid => base.Valid && Datas.UseParamBooleanControl;

        protected override Guid AddCompnentGuid => new Guid("2e78987b-9dfb-42a2-8b76-3923ac8bd91a");

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
