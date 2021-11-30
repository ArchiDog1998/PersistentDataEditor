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
    internal class ParamColorControl : ParamControlBase<GH_Colour>
    {
        internal ParamColorControl(GH_PersistentParam<GH_Colour> owner) : base(owner)
        {
            ControlItems = new BaseControlItem[]
            {
                new GooColorControl(()=> Owner.PersistentData.get_FirstItem(true), SetValue),
            };
        }
    }
}
