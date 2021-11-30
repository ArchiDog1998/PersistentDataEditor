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
        internal ParamBooleanControl(GH_PersistentParam<GH_Boolean> owner) : base(owner)
        {
            ControlItems = new BaseControlItem[]
            {
                new GooBooleanControl(()=> Owner.PersistentData.get_FirstItem(true), SetValue),
            };
        }
    }
}
