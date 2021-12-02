using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class ParamLineControl : ParamControlBase<GH_Line>
    {
        protected override bool Valid => base.Valid && Datas.UseParamLineControl;

        protected override Guid AddCompnentGuid => new Guid("4c4e56eb-2f04-43f9-95a3-cc46a14f495a");

        public ParamLineControl(GH_PersistentParam<GH_Line> owner) : base(owner)
        {

        }

        protected override BaseControlItem[] SetControlItems(GH_PersistentParam<GH_Line> owner)
        {
            return new BaseControlItem[]
            {
                new GooPointControl(()=>
                {
                    if(OwnerGooData == null) return null;
                    return new GH_Point(OwnerGooData.Value.From);
                }, "From:"),

                new GooPointControl(()=>
                {
                    if(OwnerGooData == null) return null;
                    return new GH_Point(OwnerGooData.Value.To);
                }, "To:"),
            };
        }

        protected override GH_Line SetValue(IGH_Goo[] values)
        {
            return new GH_Line(new  Rhino.Geometry.Line(((GH_Point)values[0]).Value, ((GH_Point)values[1]).Value));
        }
    }
}
