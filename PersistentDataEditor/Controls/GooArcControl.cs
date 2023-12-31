using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;

namespace PersistentDataEditor
{
    internal class GooArcControl : GooVerticalControlBase<GH_Arc>
    {
        public override Guid AddCompnentGuid => type == Arc_Control.SED ? new Guid("9d2583dd-6cf5-497c-8c40-c9a290598396") :
            new Guid("bb59bffc-f54c-4682-9778-f6c3fe74fce3");

        private Arc_Control type => NewData.ArcType;

        public GooArcControl(Func<GH_Arc> valueGetter, Func<bool> isNull, string name) : base(valueGetter, isNull, name)
        {

        }

        private protected override GH_Arc CreateDefaultValue()
        {
            return new GH_Arc(new Arc(Plane.WorldXY, 1, Math.PI / 2));
        }

        protected override BaseControlItem[] SetControlItems()
        {
            switch (type)
            {
                default:
                    return new BaseControlItem[]
                    {
                        new GooInputBoxStringControl<GH_Arc>(()=> ShowValue, _isNull, true),
                    };
                case Arc_Control.Plane_Radius_Angle:
                    return new BaseControlItem[]
                    {
                         new GooPlaneControl(()=> ShowValue == null ? null : new GH_Plane(ShowValue.Value.Plane), _isNull, "P"),

                         new GooNumberControl(()=> ShowValue == null ? null : new GH_Number(ShowValue.Value.Radius), _isNull, "R"),

                         new GooIntervalControl(()=> ShowValue == null ? null : new GH_Interval(ShowValue.Value.AngleDomain), _isNull, "D"),
                    };
                case Arc_Control.SED:
                    return new BaseControlItem[]
                    {
                         new GooPointControl(()=> ShowValue == null ? null : new GH_Point(ShowValue.Value.StartPoint), _isNull, "S"),

                         new GooPointControl(()=> ShowValue == null ? null : new GH_Point(ShowValue.Value.EndPoint), _isNull, "E"),

                         new GooVectorControl(()=> ShowValue == null ? null : new GH_Vector(ShowValue.Value.TangentAt(0)), _isNull, "D"),
                    };
            }
        }
        protected override GH_Arc SetValue(IGH_Goo[] values)
        {
            switch (type)
            {
                default:
                    return (GH_Arc)values[0];
                case Arc_Control.Plane_Radius_Angle:
                    return new GH_Arc(new Arc(
                        new Circle(((GH_Plane)values[0]).Value, ((GH_Number)values[1]).Value), ((GH_Interval)values[2]).Value));
                case Arc_Control.SED:
                    return new GH_Arc(new Arc(((GH_Point)values[0]).Value, ((GH_Vector)values[2]).Value, ((GH_Point)values[1]).Value));

            }
        }

        public override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            if (obj == null) return;
            GH_Component com = (GH_Component)obj;

            switch (type)
            {
                case Arc_Control.Plane_Radius_Angle:
                    if (com.Params.Input.Count < 3) return;

                    if (com.Params.Input[0] is Param_Plane param0 && _values[0].SaveValue is GH_Plane value0)
                    {
                            param0.PersistentData.Clear();
                            param0.PersistentData.Append(value0);
                    }

                    if (com.Params.Input[1] is Param_Number param1 && _values[1].SaveValue is GH_Number value1)
                    {
                        param1.PersistentData.Clear();
                        param1.PersistentData.Append(value1);
                    }
                    if (com.Params.Input[2] is Param_Interval param2 && _values[2].SaveValue is GH_Interval value2)
                    {
                        param2.PersistentData.Clear();
                        param2.PersistentData.Append(value2);
                    }

                    break;

                case Arc_Control.SED:
                    if (com.Params.Input.Count < 4) return;
                    if (com.Params.Input[0] is Param_Point param00 && _values[0].SaveValue is GH_Point value00)
                    {
                        param00.PersistentData.Clear();
                        param00.PersistentData.Append(value00);
                    }

                    if (com.Params.Input[1] is Param_Point param01 && _values[1].SaveValue is GH_Point value01)
                    {
                        param01.PersistentData.Clear();
                        param01.PersistentData.Append(value01);
                    }
                    if (com.Params.Input[2] is Param_Vector param02 && _values[2].SaveValue is GH_Vector value02)
                    {
                        param02.PersistentData.Clear();
                        param02.PersistentData.Append(value02);
                    }
                    
                    break;

            }
        }
    }
}
