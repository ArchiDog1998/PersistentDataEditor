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
        protected override Guid AddCompnentGuid
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

        public GooBoxControl(Func<GH_Box> valueGetter, string name) : base(valueGetter, name)
        {

        }
        protected override BaseControlItem[] SetControlItems()
        {
            switch (type)
            {
                default:
                    return new BaseControlItem[]
                    {
                        new GooInputBoxStringControl<GH_Box>(()=> ShowValue, true),
                    };
                case Box_Control.Domain_Box:
                    return new BaseControlItem[]
                    {
                         new GooPlaneControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Plane(ShowValue.Value.Plane);
                         }, "P"),

                         new GooIntervalControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Interval(ShowValue.Value.X);
                         }, "X"),

                         new GooIntervalControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Interval(ShowValue.Value.Y);
                         }, "Y"),

                         new GooIntervalControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Interval(ShowValue.Value.Z);
                         }, "Z"),
                    };
                case Box_Control.Center_Box:
                    return new BaseControlItem[]
                    {
                         new GooPlaneControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Plane(ShowValue.Value.Plane);
                         }, "P"),

                         new GooNumberControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Number(ShowValue.Value.X.Length/2);
                         }, "X"),

                         new GooNumberControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Number(ShowValue.Value.Y.Length/2);
                         }, "Y"),

                         new GooNumberControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Number(ShowValue.Value.Z.Length/2);
                         }, "Z"),
                    };

                case Box_Control.Box_Rectangle:
                    return new BaseControlItem[]
                    {
                         new GooRectangleControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Rectangle(new Rhino.Geometry.Rectangle3d(
                                 ShowValue.Value.Plane, ShowValue.Value.X, ShowValue.Value.Y));
                         }, "S"),

                        new GooIntervalControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Interval(ShowValue.Value.Z);
                         }, "H"),
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

        protected override void DosomethingWhenCreate(IGH_DocumentObject obj)
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
                        param.PersistentData.Clear();
                        param.PersistentData.Append(new GH_Plane(ShowValue.Value.Plane));
                    }

                    if (com.Params.Input[1] is Param_Interval)
                    {
                        Param_Interval param = (Param_Interval)com.Params.Input[1];
                        param.PersistentData.Clear();
                        param.PersistentData.Append(new GH_Interval(ShowValue.Value.X));
                    }

                    if (com.Params.Input[2] is Param_Interval)
                    {
                        Param_Interval param = (Param_Interval)com.Params.Input[2];
                        param.PersistentData.Clear();
                        param.PersistentData.Append(new GH_Interval(ShowValue.Value.Y));
                    }

                    if (com.Params.Input[3] is Param_Interval)
                    {
                        Param_Interval param = (Param_Interval)com.Params.Input[3];
                        param.PersistentData.Clear();
                        param.PersistentData.Append(new GH_Interval(ShowValue.Value.Z));
                    }
                    break;
                case Box_Control.Center_Box:
                    if (com.Params.Input.Count < 4) return;

                    if (com.Params.Input[0] is Param_Plane)
                    {
                        Param_Plane param = (Param_Plane)com.Params.Input[0];
                        param.PersistentData.Clear();
                        param.PersistentData.Append(new GH_Plane(ShowValue.Value.Plane));
                    }

                    if (com.Params.Input[1] is Param_Number)
                    {
                        Param_Number param = (Param_Number)com.Params.Input[1];
                        param.PersistentData.Clear();
                        param.PersistentData.Append(new GH_Number(ShowValue.Value.X.Length/2));
                    }

                    if (com.Params.Input[2] is Param_Number)
                    {
                        Param_Number param = (Param_Number)com.Params.Input[2];
                        param.PersistentData.Clear();
                        param.PersistentData.Append(new GH_Number(ShowValue.Value.Y.Length / 2));
                    }

                    if (com.Params.Input[3] is Param_Number)
                    {
                        Param_Number param = (Param_Number)com.Params.Input[3];
                        param.PersistentData.Clear();
                        param.PersistentData.Append(new GH_Number(ShowValue.Value.Z.Length / 2));
                    }
                    break;

                case Box_Control.Box_Rectangle:
                    if (com.Params.Input.Count < 2) return;

                    if (com.Params.Input[0] is Param_Rectangle)
                    {
                        Param_Rectangle param = (Param_Rectangle)com.Params.Input[0];
                        param.PersistentData.Clear();
                        param.PersistentData.Append(new GH_Rectangle(new Rhino.Geometry.Rectangle3d(
                            ShowValue.Value.Plane, ShowValue.Value.X, ShowValue.Value.Y)));
                    }

                    if (com.Params.Input[1] is Param_Interval)
                    {
                        Param_Interval param = (Param_Interval)com.Params.Input[1];
                        param.PersistentData.Clear();
                        param.PersistentData.Append(new GH_Interval(ShowValue.Value.Z));
                    }

                    break;

            }

        }
    }
}
