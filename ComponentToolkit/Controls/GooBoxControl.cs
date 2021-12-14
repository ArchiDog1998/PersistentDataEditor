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
    internal class GooBoxControl : GooVerticalControlBase<GH_Box>
    {
        public override Guid AddCompnentGuid
        {
            get
            {
                switch (type)
                {
                    default:
                        return new Guid("79aa7f47-397c-4d3f-9761-aaf421bb7f5f");
                    case Box_Control.Center_Box:
                        return new Guid("28061aae-04fb-4cb5-ac45-16f3b66bc0a4");
                    case Box_Control.Box_Rectangle:
                        return new Guid("d0a56c9e-2483-45e7-ab98-a450b97f1bc0");

                }
            }
        }

        private Box_Control type => (Box_Control)Instances.Settings.GetValue(typeof(Box_Control).FullName, 0);

        public GooBoxControl(Func<GH_Box> valueGetter, Func<bool> isNull, string name) : base(valueGetter, isNull, name)
        {

        }
        protected override BaseControlItem[] SetControlItems()
        {
            switch (type)
            {
                default:
                    return new BaseControlItem[]
                    {
                        new GooInputBoxStringControl<GH_Box>(()=> ShowValue, _isNull, true),
                    };
                case Box_Control.Domain_Box:
                    return new BaseControlItem[]
                    {
                         new GooPlaneControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Plane(ShowValue.Value.Plane);
                         }, _isNull, "P"),

                         new GooIntervalControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Interval(ShowValue.Value.X);
                         }, _isNull, "X"),

                         new GooIntervalControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Interval(ShowValue.Value.Y);
                         }, _isNull, "Y"),

                         new GooIntervalControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Interval(ShowValue.Value.Z);
                         }, _isNull, "Z"),
                    };
                case Box_Control.Center_Box:
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
                             return new GH_Number(ShowValue.Value.X.Length/2);
                         }, _isNull, "X"),

                         new GooNumberControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Number(ShowValue.Value.Y.Length/2);
                         }, _isNull, "Y"),

                         new GooNumberControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Number(ShowValue.Value.Z.Length/2);
                         }, _isNull, "Z"),
                    };

                case Box_Control.Box_Rectangle:
                    return new BaseControlItem[]
                    {
                         new GooRectangleControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Rectangle(new Rhino.Geometry.Rectangle3d(
                                 ShowValue.Value.Plane, ShowValue.Value.X, ShowValue.Value.Y));
                         }, _isNull, "R"),

                        new GooIntervalControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Interval(ShowValue.Value.Z);
                         }, _isNull, "H"),
                    };
            }
        }
        protected override GH_Box SetValue(IGH_Goo[] values)
        {
            switch (type)
            {
                default:
                    return (GH_Box)values[0];
                case Box_Control.Domain_Box:
                    return new GH_Box(new Rhino.Geometry.Box(((GH_Plane)values[0]).Value, ((GH_Interval)values[1]).Value, ((GH_Interval)values[2]).Value, ((GH_Interval)values[3]).Value));
                case Box_Control.Center_Box:
                    double x = ((GH_Number)values[1]).Value;
                    double y = ((GH_Number)values[2]).Value;
                    double z = ((GH_Number)values[3]).Value;
                    return new GH_Box(new Rhino.Geometry.Box(((GH_Plane)values[0]).Value, new Rhino.Geometry.Interval(-x, x), new Rhino.Geometry.Interval(-y, y), new Rhino.Geometry.Interval(-z, z)));
                case Box_Control.Box_Rectangle:
                    Rhino.Geometry.Rectangle3d rect = ((GH_Rectangle)values[0]).Value;
                    Rhino.Geometry.Interval height = ((GH_Interval)values[1]).Value;
                    return new GH_Box(new Rhino.Geometry.Box(rect.Plane, rect.X, rect.Y, height));

            }
        }

        public override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            if (obj == null) return;
            GH_Component com = (GH_Component)obj;
            if (com == null) return;

            switch (type)
            {
                case Box_Control.Domain_Box:
                    if (com.Params.Input.Count < 4) return;

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

                    if (com.Params.Input[3] is Param_Interval)
                    {
                        Param_Interval param = (Param_Interval)com.Params.Input[3];
                        GH_Interval interval = ((GooIntervalControl)_values[3])._savedValue;
                        if (interval != null)
                        {
                            param.PersistentData.Clear();
                            param.PersistentData.Append(interval);
                        }
                    }
                    break;

                case Box_Control.Center_Box:
                    if (com.Params.Input.Count < 4) return;

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
                        GH_Number number = ((GooNumberControl)_values[1])._savedValue;
                        if (number != null)
                        {
                            param.PersistentData.Clear();
                            param.PersistentData.Append(number);
                        }
                    }

                    if (com.Params.Input[2] is Param_Number)
                    {
                        Param_Number param = (Param_Number)com.Params.Input[2];
                        GH_Number number = ((GooNumberControl)_values[2])._savedValue;
                        if (number != null)
                        {
                            param.PersistentData.Clear();
                            param.PersistentData.Append(number);
                        }
                    }

                    if (com.Params.Input[3] is Param_Number)
                    {
                        Param_Number param = (Param_Number)com.Params.Input[3];
                        GH_Number number = ((GooNumberControl)_values[3])._savedValue;
                        if (number != null)
                        {
                            param.PersistentData.Clear();
                            param.PersistentData.Append(number);
                        }
                    }
                    break;

                case Box_Control.Box_Rectangle:
                    if (com.Params.Input.Count < 2) return;

                    if (com.Params.Input[0] is Param_Rectangle)
                    {
                        Param_Rectangle param = (Param_Rectangle)com.Params.Input[0];
                        GH_Rectangle rect = ((GooRectangleControl)_values[0])._savedValue;
                        if (rect != null)
                        {
                            param.PersistentData.Clear();
                            param.PersistentData.Append(rect);
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

                    break;

            }

        }
    }
}
