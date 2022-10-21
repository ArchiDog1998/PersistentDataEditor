using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;

namespace PersistentDataEditor
{
    internal class GooCircleControl : GooVerticalControlBase<GH_Circle>
    {
        private static Circle_Control type => (Circle_Control)Instances.Settings.GetValue(typeof(Circle_Control).FullName, 0);

        public override Guid AddCompnentGuid => type == Circle_Control.CNR ? new Guid("d114323a-e6ee-4164-946b-e4ca0ce15efa") :
            new Guid("807b86e3-be8d-4970-92b5-f8cdcb45b06b");

        public GooCircleControl(Func<GH_Circle> valueGetter, Func<bool> isNull, string name) : base(valueGetter, isNull, name)
        {

        }

        private protected override GH_Circle CreateDefaultValue() 
            => new GH_Circle(new Circle(1));

        protected override GH_Circle SetValue(IGH_Goo[] values)
        {
            switch (type)
            {
                default:
                    return (GH_Circle)values[0];
                case Circle_Control.Plane_Radius:
                    return new GH_Circle(new Circle(((GH_Plane)values[0]).Value, ((GH_Number)values[1]).Value));
                case Circle_Control.CNR:
                    return new GH_Circle(new Circle(new Plane(((GH_Point)values[0]).Value, ((GH_Vector)values[1]).Value), 
                        ((GH_Number)values[2]).Value));
            }
        }

        protected override BaseControlItem[] SetControlItems()
        {
            switch (type)
            {
                default:
                    return new BaseControlItem[]
                    {
                        new GooInputBoxStringControl<GH_Circle>(()=> ShowValue, _isNull, true),
                    };
                case Circle_Control.Plane_Radius:
                    return new BaseControlItem[]
                    {
                         new GooPlaneControl(()=> ShowValue == null ? null : new GH_Plane(ShowValue.Value.Plane), _isNull, "P"),

                         new GooNumberControl(()=> ShowValue == null ? null : new GH_Number(ShowValue.Value.Radius), _isNull, "R"),
                    };
                case Circle_Control.CNR:
                    return new BaseControlItem[]
                    {
                         new GooPointControl(()=> ShowValue == null ? null : new GH_Point(ShowValue.Value.Center), _isNull, "C"),

                         new GooVectorControl(()=> ShowValue == null ? null : new GH_Vector(ShowValue.Value.Normal), _isNull, "N"),

                         new GooNumberControl(()=> ShowValue == null ? null : new GH_Number(ShowValue.Value.Radius), _isNull, "R"),
                    };
            }
        }

        public override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            if (obj == null) return;
            GH_Component com = (GH_Component)obj;

            if (type == Circle_Control.CNR)
            {                
                if (com.Params.Input.Count < 3) return;

                if (com.Params.Input[0] is Param_Point param0 && _values[0].SaveValue is GH_Point Value0)
                {
                    param0.PersistentData.Clear();
                    param0.PersistentData.Append(Value0);
                }

                if (com.Params.Input[1] is Param_Vector param1 && _values[1].SaveValue is GH_Vector Value1)
                {
                    param1.PersistentData.Clear();
                    param1.PersistentData.Append(Value1);
                }

                if (com.Params.Input[2] is Param_Number param2 && _values[2].SaveValue is GH_Number Value2)
                {
                    param2.PersistentData.Clear();
                    param2.PersistentData.Append(Value2);
                }
            }
            else
            {
                if (com.Params.Input.Count < 2) return;

                if (com.Params.Input[0] is Param_Plane param0 && _values[0].SaveValue is GH_Plane Value0)
                {
                    param0.PersistentData.Clear();
                    param0.PersistentData.Append(Value0);
                }

                if (com.Params.Input[1] is Param_Number param1 && _values[1].SaveValue is GH_Number Value1)
                {
                    param1.PersistentData.Clear();
                    param1.PersistentData.Append(Value1);
                }
            }

        }
    }
}
