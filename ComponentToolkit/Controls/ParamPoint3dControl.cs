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
    internal class ParamPoint3dControl : ParamControlBase<GH_Point>
    {
        protected override bool Valid => base.Valid && Datas.UseParamPointControl;

        protected override Guid AddCompnentGuid => new Guid("3581f42a-9592-4549-bd6b-1c0fc39d067b");

        public ParamPoint3dControl(GH_PersistentParam<GH_Point> owner) : base(owner)
        {
        }


        protected override BaseControlItem[] SetControlItems(GH_PersistentParam<GH_Point> owner)
        {
            return new BaseControlItem[]
            {
                new GooPointControl(()=> OwnerGooData, null),
            };
        }
    }
}
