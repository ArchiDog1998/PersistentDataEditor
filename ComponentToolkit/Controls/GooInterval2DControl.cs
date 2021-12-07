using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class GooInterval2DControl : GooVerticalControlBase<GH_Interval2D>
    {
        internal class GooInterval2DControlHor : GooHorizonalControlBase<GH_Interval2D>
        {
            public override Guid AddCompnentGuid => default(Guid);

            public GooInterval2DControlHor(Func<GH_Interval2D> valueGetter, Func<bool> isNull) : base(valueGetter, isNull, null)
            {
            }


            protected override BaseControlItem[] SetControlItems()
            {
                return new BaseControlItem[]
                {

                    new GooNumberControl(()=>
                    {
                        if(ShowValue == null) return null;
                        return new GH_Number(ShowValue.Value.U0);
                    }, _isNull, "U0"),

                    new GooNumberControl(()=>
                    {
                        if(ShowValue == null) return null;
                        return new GH_Number(ShowValue.Value.U1);
                    }, _isNull, "U1"),


                    new GooNumberControl(()=>
                    {
                        if(ShowValue == null) return null;
                        return new GH_Number(ShowValue.Value.V0);
                    }, _isNull, "V0"),

                    new GooNumberControl(()=>
                    {
                        if(ShowValue == null) return null;
                        return new GH_Number(ShowValue.Value.V1);
                    }, _isNull, "V1"),

                };
            }

            protected override GH_Interval2D SetValue(IGH_Goo[] values)
            {
                return new GH_Interval2D(new UVInterval(
                    new Rhino.Geometry.Interval( ((GH_Number)values[0]).Value, ((GH_Number)values[1]).Value),
                    new Rhino.Geometry.Interval(((GH_Number)values[2]).Value, ((GH_Number)values[3]).Value)));
            }
        }
        private Domain2D_Control type => (Domain2D_Control)Instances.Settings.GetValue(typeof(Domain2D_Control).FullName, 0);

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
                         new GooIntervalControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Interval(ShowValue.Value.U);
                         }, _isNull, "U"),
                    
                         new GooIntervalControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Interval(ShowValue.Value.V);
                         }, _isNull, "V"),
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
            if (com == null) return;

            if (type == Domain2D_Control.U0_U1_V0_V1)
            {
                if (com.Params.Input.Count < 4) return;

                var values = ((GooInterval2DControlHor)_values[0])._values;

                if (com.Params.Input[0] is Param_Number)
                {
                    Param_Number param = (Param_Number)com.Params.Input[0];
                    GH_Number number = ((GooInputBoxStringControl<GH_Number>)values[0])._savedValue;
                    if (number != null)
                    {
                        param.PersistentData.Clear();
                        param.PersistentData.Append(number);
                    }
                }

                if (com.Params.Input[1] is Param_Number)
                {
                    Param_Number param = (Param_Number)com.Params.Input[1];
                    GH_Number number = ((GooInputBoxStringControl<GH_Number>)values[1])._savedValue;
                    if (number != null)
                    {
                        param.PersistentData.Clear();
                        param.PersistentData.Append(number);
                    }
                }

                if (com.Params.Input[2] is Param_Number)
                {
                    Param_Number param = (Param_Number)com.Params.Input[2];
                    GH_Number number = ((GooInputBoxStringControl<GH_Number>)values[2])._savedValue;
                    if (number != null)
                    {
                        param.PersistentData.Clear();
                        param.PersistentData.Append(number);
                    }
                }

                if (com.Params.Input[3] is Param_Number)
                {
                    Param_Number param = (Param_Number)com.Params.Input[3];
                    GH_Number number = ((GooInputBoxStringControl<GH_Number>)values[3])._savedValue;
                    if (number != null)
                    {
                        param.PersistentData.Clear();
                        param.PersistentData.Append(number);
                    }
                }
            }
            else
            {
                if (com.Params.Input.Count < 2) return;

                if (com.Params.Input[0] is Param_Interval)
                {
                    Param_Interval param = (Param_Interval)com.Params.Input[0];
                    GH_Interval interval = ((GooIntervalControl)_values[0])._savedValue;
                    if (interval != null)
                    {
                        param.PersistentData.Clear();
                        param.PersistentData.Append(interval);
                    }
                }

                if (com.Params.Input[1] is Param_Interval)
                {
                    Param_Interval param = (Param_Interval)com.Params.Input[1];
                    GH_Interval interval = ((GooIntervalControl)_values[1])._savedValue;
                    if (interval != null)
                    {
                        param.PersistentData.Clear();
                        param.PersistentData.Append(interval);
                    }
                }
            }

        }
}
}
