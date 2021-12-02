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

        public ParamComplexControl(GH_PersistentParam<GH_ComplexNumber> owner) : base(owner)
        {
        }

        protected override BaseControlItem[] SetControlItems(GH_PersistentParam<GH_ComplexNumber> owner)
        {
            return new BaseControlItem[]
            {
                new StringRender("R:"),

                new GooInputBoxControl<GH_Number>(()=>
                {
                    if(OwnerGooData == null) return null;
                    return new GH_Number(OwnerGooData.Value.Real);
                }),

                new StringRender("i:"),

                new GooInputBoxControl<GH_Number>(()=>
                {
                    if(OwnerGooData == null) return null;
                    return new GH_Number(OwnerGooData.Value.Imaginary);
                }),
            };
        }

        protected override GH_ComplexNumber SetValue(IGH_Goo[] values)
        {
            return new GH_ComplexNumber(new Complex(((GH_Number)values[0]).Value, ((GH_Number)values[1]).Value));
        }
    }
}
