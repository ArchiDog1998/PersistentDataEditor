using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;

namespace PersistentDataEditor
{
    internal class GooIntervalControl : GooHorizonalControlBase<GH_Interval>
    {
        public override Guid AddCompnentGuid => new Guid("d1a28e95-cf96-4936-bf34-8bf142d731bf");

        public GooIntervalControl(Func<GH_Interval> valueGetter, Func<bool> isNull, string name) : base(valueGetter, isNull, name)
        {
        }

        protected override BaseControlItem[] SetControlItems()
        {
            switch ((Domain_Control)Instances.Settings.GetValue(typeof(Domain_Control).FullName, 0))
            {
                default:
                    return new BaseControlItem[]
                    {
                        new GooInputBoxStringControl<GH_Interval>(()=> ShowValue, _isNull),
                    };
                case Domain_Control.T0_T1:
                    return new BaseControlItem[]
                    {
                        new GooInputBoxStringControl<GH_Number>(()=> ShowValue == null ? null : new GH_Number(ShowValue.Value.T0), _isNull),

                        new StringRender("To"),

                        new GooInputBoxStringControl<GH_Number>(()=> ShowValue == null ? null : new GH_Number(ShowValue.Value.T1), _isNull),
                    };
            }

        }

        private protected override GH_Interval CreateDefaultValue()
        {
            return new GH_Interval(new Interval(0, 1));
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

        public override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            if (obj == null) return;
            GH_Component com = (GH_Component)obj;

            if (com.Params.Input.Count < 2) return;

            if (com.Params.Input[0] is Param_Number para0 && _values[0].SaveValue is GH_Number Value0)
            {
                para0.PersistentData.Clear();
                para0.PersistentData.Append(Value0);
            }

            if (com.Params.Input[1] is Param_Number param1 && _values[1].SaveValue is GH_Number Value1)
            {
                param1.PersistentData.Clear();
                param1.PersistentData.Append(Value1);
            }
        }
    }
}
