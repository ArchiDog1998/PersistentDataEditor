using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class GooVectorControl : GooMultiControlBase<GH_Vector>
    {
        public GooVectorControl(Func<GH_Vector> valueGetter, string name) : base(valueGetter, name)
        {

        }

        protected override BaseControlItem[] SetControlItems()
        {
            return new BaseControlItem[]
            {
                new StringRender("X:"),

                new GooInputBoxControl<GH_Number>(()=>
                {
                    if(ShowValue == null) return null;
                    return new GH_Number(ShowValue.Value.X);
                }),

                new StringRender("Y:"),

                new GooInputBoxControl<GH_Number>(()=>
                {
                    if(ShowValue == null) return null;
                    return new GH_Number(ShowValue.Value.Y);
                }),

                new StringRender("Z:"),

                new GooInputBoxControl<GH_Number>(()=>
                {
                    if(ShowValue == null) return null;
                    return new GH_Number(ShowValue.Value.Z);
                }),
            };
        }

        protected override GH_Vector SetValue(IGH_Goo[] values)
        {
            return new GH_Vector(new Vector3d(
                ((GH_Number)values[0]).Value,
                ((GH_Number)values[1]).Value,
                ((GH_Number)values[2]).Value));
        }
    }
}
