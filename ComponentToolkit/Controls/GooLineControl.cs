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
    internal class GooLineControl : GooVerticalControlBase<GH_Line>
    {
        protected override Guid AddCompnentGuid => type == Line_Control.From_To ? new Guid("4c4e56eb-2f04-43f9-95a3-cc46a14f495a") :
            new Guid("4c619bc9-39fd-4717-82a6-1e07ea237bbe");

        private Line_Control type => (Line_Control)Instances.Settings.GetValue(typeof(Line_Control).FullName, 0);

        public GooLineControl(Func<GH_Line> valueGetter, string name) : base(valueGetter, name)
        {
        }

        protected override BaseControlItem[] SetControlItems()
        {
            switch (type)
            {
                default:
                    return new BaseControlItem[]
                    {
                        new GooInputBoxStringControl<GH_Line>(()=> SavedValue, true),
                    };
                case Line_Control.From_To:
                    return new BaseControlItem[]
                    {
                         new GooPointControl(()=>
                         {
                             if(SavedValue == null) return null;
                             return new GH_Point(SavedValue.Value.From);
                         }, "F"),

                         new GooPointControl(()=>
                         {
                             if(SavedValue == null) return null;
                             return new GH_Point(SavedValue.Value.To);
                         }, "T"),
                    };
                case Line_Control.Start_Direction:
                    return new BaseControlItem[]
                    {
                         new GooPointControl(()=>
                         {
                             if(SavedValue == null) return null;
                             return new GH_Point(SavedValue.Value.From);
                         }, "S"),

                         new GooVectorControl(()=>
                         {
                             if(SavedValue == null) return null;
                             return new GH_Vector(SavedValue.Value.Direction);
                         }, "D"),
                    };

                case Line_Control.SDL:
                    return new BaseControlItem[]
                    {
                         new GooPointControl(()=>
                         {
                             if(SavedValue == null) return null;
                             return new GH_Point(SavedValue.Value.From);
                         }, "S"),

                         new GooVectorControl(()=>
                         {
                             if(SavedValue == null) return null;
                             return new GH_Vector(SavedValue.Value.UnitTangent);
                         }, "D"),

                         new GooNumberControl(() =>
                         {
                             if(SavedValue == null) return null;
                             return new GH_Number(SavedValue.Value.Length);
                         }, "L"),
                    };
            }
        }

        protected override GH_Line SetValue(IGH_Goo[] values)
        {
            switch (type)
            {
                default:
                    return (GH_Line)values[0];
                case Line_Control.From_To:
                    return new GH_Line(new Rhino.Geometry.Line(((GH_Point)values[0]).Value, ((GH_Point)values[1]).Value));
                case Line_Control.Start_Direction:
                    return new GH_Line(new Rhino.Geometry.Line(((GH_Point)values[0]).Value, ((GH_Vector)values[1]).Value));
                case Line_Control.SDL:
                    return new GH_Line(new Rhino.Geometry.Line(((GH_Point)values[0]).Value, ((GH_Vector)values[1]).Value, 
                        ((GH_Number)values[2]).Value));
            }
        }

        protected override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            if (obj == null) return;
            GH_Component com = (GH_Component)obj;
            if (com == null) return;

            if(type == Line_Control.From_To)
            {
                if (com.Params.Input.Count < 2) return;

                if (com.Params.Input[0] is Param_Point)
                {
                    Param_Point param = (Param_Point)com.Params.Input[0];
                    param.PersistentData.Clear();
                    param.PersistentData.Append(new GH_Point(SavedValue.Value.From));
                }

                if (com.Params.Input[1] is Param_Point)
                {
                    Param_Point param = (Param_Point)com.Params.Input[1];
                    param.PersistentData.Clear();
                    param.PersistentData.Append(new GH_Point(SavedValue.Value.To));
                }
            }
            else
            {
                if (com.Params.Input.Count < 3) return;

                if (com.Params.Input[0] is Param_Point)
                {
                    Param_Point param = (Param_Point)com.Params.Input[0];
                    param.PersistentData.Clear();
                    param.PersistentData.Append(new GH_Point(SavedValue.Value.From));
                }

                if (com.Params.Input[1] is Param_Vector)
                {
                    Param_Vector param = (Param_Vector)com.Params.Input[1];
                    param.PersistentData.Clear();
                    param.PersistentData.Append(new GH_Vector(SavedValue.Value.Direction));
                }

                if (com.Params.Input[2] is Param_Number)
                {
                    Param_Number param = (Param_Number)com.Params.Input[2];
                    param.PersistentData.Clear();
                    param.PersistentData.Append(new GH_Number(SavedValue.Value.Length));
                }
            }

        }
    }
}
