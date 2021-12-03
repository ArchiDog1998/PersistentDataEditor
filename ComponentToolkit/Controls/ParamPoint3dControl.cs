using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
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

        protected override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            if (obj == null) return;
            GH_Component com = (GH_Component)obj;
            if (com == null) return;

            if (com.Params.Input.Count < 3) return;

            if (com.Params.Input[0] is Param_Number)
            {
                Param_Number param = (Param_Number)com.Params.Input[0];
                param.PersistentData.Clear();
                param.PersistentData.Append(new GH_Number(OwnerGooData.Value.X));
            }

            if (com.Params.Input[1] is Param_Number)
            {
                Param_Number param = (Param_Number)com.Params.Input[1];
                param.PersistentData.Clear();
                param.PersistentData.Append(new GH_Number(OwnerGooData.Value.Y));
            }

            if (com.Params.Input[2] is Param_Number)
            {
                Param_Number param = (Param_Number)com.Params.Input[2];
                param.PersistentData.Clear();
                param.PersistentData.Append(new GH_Number(OwnerGooData.Value.Z));
            }
        }
    }
}
