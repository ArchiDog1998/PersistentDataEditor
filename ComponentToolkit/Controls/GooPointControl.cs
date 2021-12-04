using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class GooPointControl : GooHorizonalControlBase<GH_Point>
    {
        protected override Guid AddCompnentGuid => new Guid("3581f42a-9592-4549-bd6b-1c0fc39d067b");

        public GooPointControl(Func<GH_Point> valueGetter, string name) : base(valueGetter, name)
        {

        }

        protected override BaseControlItem[] SetControlItems()
        {
            switch ((Point_Control)Instances.Settings.GetValue(typeof(Point_Control).FullName, 0))
            {
                default:
                    return new BaseControlItem[]
                    {
                        new GooInputBoxStringControl<GH_Point>(()=> SavedValue),
                    };
                case Point_Control.XYZ:
                    return new BaseControlItem[]
                    {
                        new StringRender("X"),

                        new GooInputBoxStringControl<GH_Number>(()=>
                        {
                            if(SavedValue == null) return null;
                            return new GH_Number(SavedValue.Value.X);
                        }),

                        new StringRender("Y"),

                        new GooInputBoxStringControl<GH_Number>(()=>
                        {
                            if(SavedValue == null) return null;
                            return new GH_Number(SavedValue.Value.Y);
                        }),

                        new StringRender("Z"),

                        new GooInputBoxStringControl<GH_Number>(()=>
                        {
                            if(SavedValue == null) return null;
                            return new GH_Number(SavedValue.Value.Z);
                        }),
                    };
            }


        }

        protected override GH_Point SetValue(IGH_Goo[] values)
        {
            switch ((Point_Control)Instances.Settings.GetValue(typeof(Point_Control).FullName, 0))
            {
                default:
                    return (GH_Point)values[0];

                case Point_Control.XYZ:
                    return new GH_Point(new Point3d(
                ((GH_Number)values[0]).Value,
                ((GH_Number)values[1]).Value,
                ((GH_Number)values[2]).Value));
            }
        }

        protected override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            if (obj == null) return;
            GH_Component com = (GH_Component)obj;
            if (com == null) return;

            if (com.Params.Input.Count < 3) return;

            if (com.Params.Input[0] is Param_Number)
            {
                Param_Number param = (Param_Number)com.Params.Input[0];
                param.PersistentData.Clear();
                param.PersistentData.Append(new GH_Number(SavedValue.Value.X));
            }

            if (com.Params.Input[1] is Param_Number)
            {
                Param_Number param = (Param_Number)com.Params.Input[1];
                param.PersistentData.Clear();
                param.PersistentData.Append(new GH_Number(SavedValue.Value.Y));
            }

            if (com.Params.Input[2] is Param_Number)
            {
                Param_Number param = (Param_Number)com.Params.Input[2];
                param.PersistentData.Clear();
                param.PersistentData.Append(new GH_Number(SavedValue.Value.Z));
            }
        }
    }
}
