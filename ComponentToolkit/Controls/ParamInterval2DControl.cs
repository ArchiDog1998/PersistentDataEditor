﻿using Grasshopper.Kernel;
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
    internal class ParamInterval2DControl : ParamControlBase<GH_Interval2D>
    {
        protected override bool Valid => base.Valid && Datas.UseParamInterval2DControl;

        protected override Guid AddCompnentGuid => new Guid("8555a743-36c1-42b8-abcc-06d9cb94519f");

        public ParamInterval2DControl(GH_PersistentParam<GH_Interval2D> owner) : base(owner)
        {
        }

        protected override BaseControlItem[] SetControlItems(GH_PersistentParam<GH_Interval2D> owner)
        {
            return new BaseControlItem[]
            {
                new GooIntervalControl(()=>
                {
                    if(OwnerGooData == null) return null;
                    return new GH_Interval(OwnerGooData.Value.U);
                }, "U:"),

                new GooIntervalControl(()=>
                {
                    if(OwnerGooData == null) return null;
                    return new GH_Interval(OwnerGooData.Value.V);
                }, "V:"),
            };
        }

        protected override GH_Interval2D SetValue(IGH_Goo[] values)
        {
            return new GH_Interval2D(new UVInterval(((GH_Interval)values[0]).Value, ((GH_Interval)values[1]).Value));
        }

        protected override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            if (obj == null) return;
            GH_Component com = (GH_Component)obj;
            if (com == null) return;

            if (com.Params.Input.Count < 2) return;

            if (com.Params.Input[0] is Param_Interval)
            {
                Param_Interval param = (Param_Interval)com.Params.Input[0];
                param.PersistentData.Clear();
                param.PersistentData.Append(new GH_Interval(OwnerGooData.Value.U));
            }

            if (com.Params.Input[1] is Param_Interval)
            {
                Param_Interval param = (Param_Interval)com.Params.Input[1];
                param.PersistentData.Clear();
                param.PersistentData.Append(new GH_Interval(OwnerGooData.Value.V));
            }
        }
    }
}
