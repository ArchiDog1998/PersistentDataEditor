using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class GooComplexControl : GooMultiControlBase<GH_ComplexNumber>
    {
        public GooComplexControl(Func<GH_ComplexNumber> valueGetter, string name) : base(valueGetter, name)
        {

        }

        protected override BaseControlItem[] SetControlItems()
        {
            return new BaseControlItem[]
            {
                new StringRender("R:"),

                new GooInputBoxControl<GH_Number>(()=>
                {
                    if(ShowValue == null) return null;
                    return new GH_Number(ShowValue.Value.Real);
                }),

                new StringRender("i:"),

                new GooInputBoxControl<GH_Number>(()=>
                {
                    if(ShowValue == null) return null;
                    return new GH_Number(ShowValue.Value.Imaginary);
                }),
            };
        }

        protected override GH_ComplexNumber SetValue(IGH_Goo[] values)
        {
            return new GH_ComplexNumber(new Complex(((GH_Number)values[0]).Value, ((GH_Number)values[1]).Value));
        }
    }
}
