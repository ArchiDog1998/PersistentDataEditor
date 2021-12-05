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
    internal class GooRectangleControl : GooVerticalControlBase<GH_Rectangle>
    {
        protected override Guid AddCompnentGuid => new Guid("d93100b6-d50b-40b2-831a-814659dc38e3");

        private Rectangle_Control type => (Rectangle_Control)Instances.Settings.GetValue(typeof(Rectangle_Control).FullName, 0);

        public GooRectangleControl(Func<GH_Rectangle> valueGetter, string name) : base(valueGetter, name)
        {
        }
        protected override BaseControlItem[] SetControlItems()
        {
            switch (type)
            {
                default:
                    return new BaseControlItem[]
                    {
                        new GooInputBoxStringControl<GH_Rectangle>(()=> ShowValue, true),
                    };
                case Rectangle_Control.Domain_Rectangle:
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
                    };
                case Rectangle_Control.Plane_Width_Height:
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
                             return new GH_Number(ShowValue.Value.X.Length);
                         }, "W"),

                         new GooNumberControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Number(ShowValue.Value.Y.Length);
                         }, "H"),
                    };

                case Rectangle_Control.Center_Rectangle:
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
                    };
            }

        }
        protected override GH_Rectangle SetValue(IGH_Goo[] values)
        {
            switch (type)
            {
                default:
                    return (GH_Rectangle)values[0];
                case Rectangle_Control.Domain_Rectangle:
                    return new GH_Rectangle(new Rhino.Geometry.Rectangle3d(((GH_Plane)values[0]).Value, ((GH_Interval)values[1]).Value, ((GH_Interval)values[2]).Value));
                case Rectangle_Control.Plane_Width_Height:
                    return new GH_Rectangle(new Rhino.Geometry.Rectangle3d(((GH_Plane)values[0]).Value, ((GH_Number)values[1]).Value, ((GH_Number)values[2]).Value));
                case Rectangle_Control.Center_Rectangle:
                    double x = ((GH_Number)values[1]).Value;
                    double y = ((GH_Number)values[2]).Value;
                    return new GH_Rectangle(new Rhino.Geometry.Rectangle3d(((GH_Plane)values[0]).Value, new Rhino.Geometry.Interval(-x, x), new Rhino.Geometry.Interval(-y, y)));

            }
        }
        protected override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            if (obj == null) return;
            GH_Component com = (GH_Component)obj;
            if (com == null) return;

            if (com.Params.Input.Count < 3) return;

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
        }

    }
}