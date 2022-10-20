using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;

namespace PersistentDataEditor
{
    internal class GooInterval2DControl : GooVerticalControlBase<GH_Interval2D>
    {
        private protected override GH_Interval2D CreateDefaultValue()
        {
            return new GH_Interval2D(new UVInterval(new Interval(0, 1), new Interval(0, 1)));
        }

        internal class GooInterval2DControlHor : GooHorizonalControlBase<GH_Interval2D>
        {
            public override Guid AddCompnentGuid => default;

            public GooInterval2DControlHor(Func<GH_Interval2D> valueGetter, Func<bool> isNull) : base(valueGetter, isNull, null)
            {
            }

            private protected override GH_Interval2D CreateDefaultValue()
            {
                return new GH_Interval2D(new UVInterval(new Interval(0, 1), new Interval(0, 1)));
            }

            protected override BaseControlItem[] SetControlItems()
            {
                return new BaseControlItem[]
                {

                    new GooNumberControl(()=> ShowValue == null ? null : new GH_Number(ShowValue.Value.U0), _isNull, "U0"),

                    new GooNumberControl(()=> ShowValue == null ? null : new GH_Number(ShowValue.Value.U1), _isNull, "U1"),


                    new GooNumberControl(()=> ShowValue == null ? null : new GH_Number(ShowValue.Value.V0), _isNull, "V0"),

                    new GooNumberControl(()=> ShowValue == null ? null : new GH_Number(ShowValue.Value.V1), _isNull, "V1"),

                };
            }

            protected override GH_Interval2D SetValue(IGH_Goo[] values)
            {
                return new GH_Interval2D(new UVInterval(
                    new Interval( ((GH_Number)values[0]).Value, ((GH_Number)values[1]).Value),
                    new Interval(((GH_Number)values[2]).Value, ((GH_Number)values[3]).Value)));
            }
        }
        private static Domain2D_Control type => (Domain2D_Control)Instances.Settings.GetValue(typeof(Domain2D_Control).FullName, 0);

        public override Guid AddCompnentGuid => type == Domain2D_Control.U0_U1_V0_V1 ? 
            new Guid("9083b87f-a98c-4e41-9591-077ae4220b19"): new Guid("8555a743-36c1-42b8-abcc-06d9cb94519f");

        public GooInterval2DControl(Func<GH_Interval2D> valueGetter, Func<bool> isNull, string name) : base(valueGetter, isNull, name)
        {
        }

        protected override BaseControlItem[] SetControlItems()
        {
            switch (type)
            {
                default:
                    return new BaseControlItem[]
                    {
                        new GooInputBoxStringControl<GH_Interval2D>(()=> ShowValue, _isNull, true),
                    };
                case Domain2D_Control.U_V:
                    return new BaseControlItem[]
                    {
                         new GooIntervalControl(()=> ShowValue == null ? null : new GH_Interval(ShowValue.Value.U), _isNull, "U"),
                    
                         new GooIntervalControl(()=> ShowValue == null ? null : new GH_Interval(ShowValue.Value.V), _isNull, "V"),
                    };
                case Domain2D_Control.U0_U1_V0_V1:
                    return new BaseControlItem[]
                    {
                         new GooInterval2DControlHor(() => ShowValue, _isNull),
                    };
            }
        }

        protected override GH_Interval2D SetValue(IGH_Goo[] values)
        {
            return new GH_Interval2D(new UVInterval(((GH_Interval)values[0]).Value, ((GH_Interval)values[1]).Value));
        }

        public override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            if (obj == null) return;
            GH_Component com = (GH_Component)obj;

            if (type == Domain2D_Control.U0_U1_V0_V1)
            {
                if (com.Params.Input.Count < 4) return;

                if (com.Params.Input[0] is Param_Number param0 && _values[0].SaveValue is GH_Number value0)
                {
                    param0.PersistentData.Clear();
                    param0.PersistentData.Append(value0);
                }
                if (com.Params.Input[1] is Param_Number param1 && _values[1].SaveValue is GH_Number value1)
                {
                    param1.PersistentData.Clear();
                    param1.PersistentData.Append(value1);
                }
                if (com.Params.Input[2] is Param_Number param2 && _values[2].SaveValue is GH_Number value2)
                {
                    param2.PersistentData.Clear();
                    param2.PersistentData.Append(value2);
                }
                if (com.Params.Input[3] is Param_Number param3 && _values[3].SaveValue is GH_Number value3)
                {
                    param3.PersistentData.Clear();
                    param3.PersistentData.Append(value3);
                }
            }
            else
            {
                if (com.Params.Input.Count < 2) return;
                if (com.Params.Input[0] is Param_Interval param0 && _values[0].SaveValue is GH_Interval value0)
                {
                    param0.PersistentData.Clear();
                    param0.PersistentData.Append(value0);
                }
                if (com.Params.Input[1] is Param_Interval param1 && _values[1].SaveValue is GH_Interval value1)
                {
                    param1.PersistentData.Clear();
                    param1.PersistentData.Append(value1);
                }
            }
        }
}
}
