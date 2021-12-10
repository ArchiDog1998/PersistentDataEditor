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
    internal class GooArcControl : GooVerticalControlBase<GH_Arc>
    {
        public override Guid AddCompnentGuid => type == Arc_Control.SED ? new Guid("9d2583dd-6cf5-497c-8c40-c9a290598396") :
            new Guid("bb59bffc-f54c-4682-9778-f6c3fe74fce3");

        private Arc_Control type => (Arc_Control)Instances.Settings.GetValue(typeof(Arc_Control).FullName, 0);

        public GooArcControl(Func<GH_Arc> valueGetter, Func<bool> isNull, string name) : base(valueGetter, isNull, name)
        {

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
                         new GooPlaneControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Plane(ShowValue.Value.Plane);
                         }, _isNull, "P"),

                         new GooNumberControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Number(ShowValue.Value.Radius);
                         }, _isNull, "R"),

                         new GooIntervalControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Interval(ShowValue.Value.AngleDomain);
                         }, _isNull, "D"),
                    };
                case Arc_Control.SED:
                    return new BaseControlItem[]
                    {
                         new GooPointControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Point(ShowValue.Value.StartPoint);
                         }, _isNull, "S"),

                         new GooPointControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Point(ShowValue.Value.EndPoint);
                         }, _isNull, "E"),

                         new GooVectorControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Vector(ShowValue.Value.TangentAt(0));
                         }, _isNull, "D"),
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
                    return new GH_Arc(new Rhino.Geometry.Arc(
                        new Rhino.Geometry.Circle(((GH_Plane)values[0]).Value, ((GH_Number)values[1]).Value), ((GH_Interval)values[2]).Value));
                case Arc_Control.SED:
                    return new GH_Arc(new Rhino.Geometry.Arc(((GH_Point)values[0]).Value, ((GH_Vector)values[2]).Value, ((GH_Point)values[1]).Value));

            }
        }

        public override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            if (obj == null) return;
            GH_Component com = (GH_Component)obj;
            if (com == null) return;

            switch (type)
            {
                case Arc_Control.Plane_Radius_Angle:
                    if (com.Params.Input.Count < 3) return;

                    if (com.Params.Input[0] is Param_Plane)
                    {
                        Param_Plane param = (Param_Plane)com.Params.Input[0];
                        GH_Plane plane = ((GooPlaneControl)_values[0])._savedValue;
                        if (plane != null)
                        {
                            param.PersistentData.Clear();
                            param.PersistentData.Append(plane);
                        }
                    }

                    if (com.Params.Input[1] is Param_Number)
                    {
                        Param_Number param = (Param_Number)com.Params.Input[1];
                        GH_Number interval = ((GooNumberControl)_values[1])._savedValue;
                        if (interval != null)
                        {
                            param.PersistentData.Clear();
                            param.PersistentData.Append(interval);
                        }
                    }

                    if (com.Params.Input[2] is Param_Interval)
                    {
                        Param_Interval param = (Param_Interval)com.Params.Input[2];
                        GH_Interval interval = ((GooIntervalControl)_values[2])._savedValue;
                        if (interval != null)
                        {
                            param.PersistentData.Clear();
                            param.PersistentData.Append(interval);
                        }
                    }

                    break;

                case Arc_Control.SED:
                    if (com.Params.Input.Count < 4) return;

                    if (com.Params.Input[0] is Param_Point)
                    {
                        Param_Point param = (Param_Point)com.Params.Input[0];
                        GH_Point plane = ((GooPointControl)_values[0])._savedValue;
                        if (plane != null)
                        {
                            param.PersistentData.Clear();
                            param.PersistentData.Append(plane);
                        }
                    }

                    if (com.Params.Input[1] is Param_Point)
                    {
                        Param_Point param = (Param_Point)com.Params.Input[1];
                        GH_Point number = ((GooPointControl)_values[1])._savedValue;
                        if (number != null)
                        {
                            param.PersistentData.Clear();
                            param.PersistentData.Append(number);
                        }
                    }

                    if (com.Params.Input[2] is Param_Vector)
                    {
                        Param_Vector param = (Param_Vector)com.Params.Input[2];
                        GH_Vector number = ((GooVectorControl)_values[2])._savedValue;
                        if (number != null)
                        {
                            param.PersistentData.Clear();
                            param.PersistentData.Append(number);
                        }
                    }
                    break;

            }
        }
    }
}
