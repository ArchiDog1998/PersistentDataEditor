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
            protected override Guid AddCompnentGuid => default(Guid);

            public GooInterval2DControlHor(Func<GH_Interval2D> valueGetter) : base(valueGetter, null)
            {
            }


            protected override BaseControlItem[] SetControlItems()
            {
                return new BaseControlItem[]
                {

                    new GooNumberControl(()=>
                    {
                        if(SavedValue == null) return null;
                        return new GH_Number(SavedValue.Value.U0);
                    },"U0"),

                    new GooNumberControl(()=>
                    {
                        if(SavedValue == null) return null;
                        return new GH_Number(SavedValue.Value.U1);
                    }, "U1"),


                    new GooNumberControl(()=>
                    {
                        if(SavedValue == null) return null;
                        return new GH_Number(SavedValue.Value.V0);
                    }, "V0"),

                    new GooNumberControl(()=>
                    {
                        if(SavedValue == null) return null;
                        return new GH_Number(SavedValue.Value.V1);
                    }, "V1"),

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

        protected override Guid AddCompnentGuid => type == Domain2D_Control.U0_U1_V0_V1 ? 
            new Guid("9083b87f-a98c-4e41-9591-077ae4220b19"): new Guid("8555a743-36c1-42b8-abcc-06d9cb94519f");
        public GooInterval2DControl(Func<GH_Interval2D> valueGetter, string name) : base(valueGetter, name)
        {
        }

        protected override BaseControlItem[] SetControlItems()
        {
            switch (type)
            {
                default:
                    return new BaseControlItem[]
                    {
                        new GooInputBoxStringControl<GH_Interval2D>(()=> SavedValue, true),
                    };
                case Domain2D_Control.U_V:
                    return new BaseControlItem[]
                    {
                         new GooIntervalControl(()=>
                         {
                             if(SavedValue == null) return null;
                             return new GH_Interval(SavedValue.Value.U);
                         }, "U"),
                    
                         new GooIntervalControl(()=>
                         {
                             if(SavedValue == null) return null;
                             return new GH_Interval(SavedValue.Value.V);
                         }, "V"),
                    };
                case Domain2D_Control.U0_U1_V0_V1:
                    return new BaseControlItem[]
                    {
                         new GooInterval2DControlHor(() => SavedValue),
                    };
            }
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

            if (type == Domain2D_Control.U0_U1_V0_V1)
            {
                if (com.Params.Input.Count < 4) return;

                if (com.Params.Input[0] is Param_Number)
                {
                    Param_Number param = (Param_Number)com.Params.Input[0];
                    param.PersistentData.Clear();
                    param.PersistentData.Append(new GH_Number(SavedValue.Value.U0));
                }

                if (com.Params.Input[1] is Param_Number)
                {
                    Param_Number param = (Param_Number)com.Params.Input[1];
                    param.PersistentData.Clear();
                    param.PersistentData.Append(new GH_Number(SavedValue.Value.U1));
                }

                if (com.Params.Input[2] is Param_Number)
                {
                    Param_Number param = (Param_Number)com.Params.Input[2];
                    param.PersistentData.Clear();
                    param.PersistentData.Append(new GH_Number(SavedValue.Value.V0));
                }

                if (com.Params.Input[3] is Param_Number)
                {
                    Param_Number param = (Param_Number)com.Params.Input[3];
                    param.PersistentData.Clear();
                    param.PersistentData.Append(new GH_Number(SavedValue.Value.V1));
                }
            }
            else
            {
                if (com.Params.Input.Count < 2) return;

                if (com.Params.Input[0] is Param_Interval)
                {
                    Param_Interval param = (Param_Interval)com.Params.Input[0];
                    param.PersistentData.Clear();
                    param.PersistentData.Append(new GH_Interval(SavedValue.Value.U));
                }

                if (com.Params.Input[1] is Param_Interval)
                {
                    Param_Interval param = (Param_Interval)com.Params.Input[1];
                    param.PersistentData.Clear();
                    param.PersistentData.Append(new GH_Interval(SavedValue.Value.V));
                }
            }

        }
}
}
