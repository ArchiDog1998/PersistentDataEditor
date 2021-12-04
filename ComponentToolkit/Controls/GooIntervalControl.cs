using Grasshopper;
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
    internal class GooIntervalControl : GooHorizonalControlBase<GH_Interval>
    {
        protected override Guid AddCompnentGuid => new Guid("d1a28e95-cf96-4936-bf34-8bf142d731bf");

        public GooIntervalControl(Func<GH_Interval> valueGetter, string name) : base(valueGetter, name)
        {
        }

        protected override BaseControlItem[] SetControlItems()
        {
            switch ((Domain_Control)Instances.Settings.GetValue(typeof(Domain_Control).FullName, 0))
            {
                default:
                    return new BaseControlItem[]
                    {
                        new GooInputBoxStringControl<GH_Interval>(()=> ShowValue),
                    };
                case Domain_Control.T0_T1:
                    return new BaseControlItem[]
                    {
                        new GooInputBoxStringControl<GH_Number>(()=>
                        {
                            if(ShowValue == null) return null;
                            return new GH_Number(ShowValue.Value.T0);
                        }),

                        new StringRender("To"),

                        new GooInputBoxStringControl<GH_Number>(()=>
                        {
                            if(ShowValue == null) return null;
                            return new GH_Number(ShowValue.Value.T1);
                        }),
                    };
            }

        }

        protected override GH_Interval SetValue(IGH_Goo[] values)
        {
            switch ((Domain_Control)Instances.Settings.GetValue(typeof(Domain_Control).FullName, 0))
            {
                default:
                    return (GH_Interval)values[0];
                case Domain_Control.T0_T1:
                    return new GH_Interval(new Interval(((GH_Number)values[0]).Value, ((GH_Number)values[1]).Value));
            }
        }

        protected override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            if (obj == null) return;
            GH_Component com = (GH_Component)obj;
            if (com == null) return;

            if (com.Params.Input.Count < 2) return;

            if (com.Params.Input[0] is Param_Number)
            {
                Param_Number param = (Param_Number)com.Params.Input[0];
                param.PersistentData.Clear();
                param.PersistentData.Append(new GH_Number(ShowValue.Value.T0));
            }

            if (com.Params.Input[1] is Param_Number)
            {
                Param_Number param = (Param_Number)com.Params.Input[1];
                param.PersistentData.Clear();
                param.PersistentData.Append(new GH_Number(ShowValue.Value.T1));
            }
        }
    }
}
