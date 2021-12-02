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
    internal class ParamVector3dControl : ParamControlBase<GH_Vector>
    {
        protected override bool Valid => base.Valid && Datas.UseParamVectorControl;

        public ParamVector3dControl(GH_PersistentParam<GH_Vector> owner) : base(owner)
        {
        }

        protected override BaseControlItem[] SetControlItems(GH_PersistentParam<GH_Vector> owner)
        {
            return new BaseControlItem[]
            {
                new GooVectorControl(()=> OwnerGooData, null),
            };
        }
    }
}
