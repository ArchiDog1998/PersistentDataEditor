using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
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

        protected override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            if (obj == null) return;
            GH_Component com = (GH_Component)obj;
            if(com == null) return;

            if (com.Params.Input.Count < 2) return;

            if( com.Params.Input[0] is Param_Number)
            {
                Param_Number param = (Param_Number)com.Params.Input[0];
                param.PersistentData.Clear();
                param.PersistentData.Append(new GH_Number(OwnerGooData.Value.T0));
            }

            if (com.Params.Input[1] is Param_Number)
            {
                Param_Number param = (Param_Number)com.Params.Input[1];
                param.PersistentData.Clear();
                param.PersistentData.Append(new GH_Number(OwnerGooData.Value.T1));
            }
        }
    }
}
