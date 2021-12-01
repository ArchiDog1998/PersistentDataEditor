using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    public class ParamInputBoxControl<T> : ParamControlBase<T> where T: class, IGH_Goo
    {
        internal ParamInputBoxControl(GH_PersistentParam<T> owner) : base(owner)
        {
        }

        protected sealed override BaseControlItem[] SetControlItems(GH_PersistentParam<T> owner)
        {
            return new BaseControlItem[]
            {
                new GooInputBoxControl<T>(()=> OwnerGooData),
            };
        }
    }
}
