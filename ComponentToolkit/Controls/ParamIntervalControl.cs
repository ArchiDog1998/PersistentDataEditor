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
    internal class ParamIntervalControl : ParamControlBase<GH_Interval>
    {
        protected override bool Valid => base.Valid && Datas.UseParamIntervalControl;

        protected override Guid AddCompnentGuid => new Guid("d1a28e95-cf96-4936-bf34-8bf142d731bf");

        public ParamIntervalControl(GH_PersistentParam<GH_Interval> owner) : base(owner)
        {
        }

        protected override BaseControlItem[] SetControlItems(GH_PersistentParam<GH_Interval> owner)
        {
            return new BaseControlItem[]
            {
                new GooIntervalControl(()=> OwnerGooData, null),
            };
        }
    }
}
