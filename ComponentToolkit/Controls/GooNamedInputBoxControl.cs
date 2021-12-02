using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class GooNamedInputBoxControl<T> : GooMultiControlBase<T> where T : class, IGH_Goo
    {
        public GooNamedInputBoxControl(Func<T> valueGetter, string name) : base(valueGetter, name)
        {

        }

        protected override BaseControlItem[] SetControlItems()
        {
            return new BaseControlItem[]
            {
                new GooInputBoxControl<T>(()=>
                {
                    if(ShowValue == null) return null;
                    return ShowValue;
                }),
            };
        }

        protected override T SetValue(IGH_Goo[] values) => (T)values[0];
    }
}
