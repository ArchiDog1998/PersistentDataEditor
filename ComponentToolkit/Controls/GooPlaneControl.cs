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
    internal class GooPlaneControl : GooVerticalControlBase<GH_Plane>
    {
        protected override Guid AddCompnentGuid => type == Plane_Control.OZ ? new Guid("cfb6b17f-ca82-4f5d-b604-d4f69f569de3") :
            new Guid("bc3e379e-7206-4e7b-b63a-ff61f4b38a3e");

        private Plane_Control type => (Plane_Control)Instances.Settings.GetValue(typeof(Plane_Control).FullName, 0);
        public GooPlaneControl(Func<GH_Plane> valueGetter, string name) : base(valueGetter, name)
        {

        }

        protected override BaseControlItem[] SetControlItems()
        {
            switch (type)
            {
                default:
                    return new BaseControlItem[]
                    {
                        new GooInputBoxStringControl<GH_Plane>(()=> ShowValue, true),
                    };
                case Plane_Control.OZ:
                    return new BaseControlItem[]
                    {
                         new GooPointControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Point(ShowValue.Value.Origin);
                         }, "O"),

                         new GooVectorControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Vector(ShowValue.Value.ZAxis);
                         }, "Z"),
                    };
                case Plane_Control.OXY:
                    return new BaseControlItem[]
                    {
                         new GooPointControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Point(ShowValue.Value.Origin);
                         }, "O"),

                         new GooVectorControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Vector(ShowValue.Value.XAxis);
                         }, "X"),

                         new GooVectorControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Vector(ShowValue.Value.YAxis);
                         }, "Y"),
                    };
            }
        }

        protected override GH_Plane SetValue(IGH_Goo[] values)
        {
            switch (type)
            {
                default:
                    return (GH_Plane)values[0];
                case Plane_Control.OZ:
                    return new GH_Plane(new Rhino.Geometry.Plane(((GH_Point)values[0]).Value, ((GH_Vector)values[1]).Value));
                case Plane_Control.OXY:
                    return new GH_Plane(new Rhino.Geometry.Plane(((GH_Point)values[0]).Value, ((GH_Vector)values[1]).Value, ((GH_Vector)values[2]).Value));
            }
        }

        protected override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            if (obj == null) return;
            GH_Component com = (GH_Component)obj;
            if (com == null) return;

            if (type == Plane_Control.OZ)
            {
                if (com.Params.Input.Count < 2) return;

                if (com.Params.Input[0] is Param_Point)
                {
                    Param_Point param = (Param_Point)com.Params.Input[0];
                    param.PersistentData.Clear();
                    param.PersistentData.Append(new GH_Point(ShowValue.Value.Origin));
                }

                if (com.Params.Input[1] is Param_Vector)
                {
                    Param_Vector param = (Param_Vector)com.Params.Input[1];
                    param.PersistentData.Clear();
                    param.PersistentData.Append(new GH_Vector(ShowValue.Value.ZAxis));
                }
            }
            else
            {
                if (com.Params.Input.Count < 3) return;

                if (com.Params.Input[0] is Param_Point)
                {
                    Param_Point param = (Param_Point)com.Params.Input[0];
                    param.PersistentData.Clear();
                    param.PersistentData.Append(new GH_Point(ShowValue.Value.Origin));
                }

                if (com.Params.Input[1] is Param_Vector)
                {
                    Param_Vector param = (Param_Vector)com.Params.Input[1];
                    param.PersistentData.Clear();
                    param.PersistentData.Append(new GH_Vector(ShowValue.Value.XAxis));
                }

                if (com.Params.Input[2] is Param_Vector)
                {
                    Param_Vector param = (Param_Vector)com.Params.Input[2];
                    param.PersistentData.Clear();
                    param.PersistentData.Append(new GH_Vector(ShowValue.Value.YAxis));
                }
            }

        }
    }
}
