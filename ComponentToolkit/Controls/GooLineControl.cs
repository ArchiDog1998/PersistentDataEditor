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
        public override Guid AddCompnentGuid => type == Line_Control.From_To ? new Guid("4c4e56eb-2f04-43f9-95a3-cc46a14f495a") :
            new Guid("4c619bc9-39fd-4717-82a6-1e07ea237bbe");

        private Line_Control type => (Line_Control)Instances.Settings.GetValue(typeof(Line_Control).FullName, 0);

        public GooLineControl(Func<GH_Line> valueGetter, Func<bool> isNull, string name) : base(valueGetter, isNull, name)
        {
        }

        protected override BaseControlItem[] SetControlItems()
        {
            switch (type)
            {
                default:
                    return new BaseControlItem[]
                    {
                        new GooInputBoxStringControl<GH_Line>(()=> ShowValue, _isNull, true),
                    };
                case Line_Control.From_To:
                    return new BaseControlItem[]
                    {
                         new GooPointControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Point(ShowValue.Value.From);
                         }, _isNull, "F"),

                         new GooPointControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Point(ShowValue.Value.To);
                         }, _isNull, "T"),
                    };
                case Line_Control.Start_Direction:
                    return new BaseControlItem[]
                    {
                         new GooPointControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Point(ShowValue.Value.From);
                         }, _isNull, "S"),

                         new GooVectorControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Vector(ShowValue.Value.Direction);
                         }, _isNull, "D"),
                    };

                case Line_Control.SDL:
                    return new BaseControlItem[]
                    {
                         new GooPointControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Point(ShowValue.Value.From);
                         }, _isNull, "S"),

                         new GooVectorControl(()=>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Vector(ShowValue.Value.UnitTangent);
                         }, _isNull, "D"),

                         new GooNumberControl(() =>
                         {
                             if(ShowValue == null) return null;
                             return new GH_Number(ShowValue.Value.Length);
                         }, _isNull, "L"),
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

        public override void DosomethingWhenCreate(IGH_DocumentObject obj)
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
                    GH_Point point = ((GooPointControl)_values[0])._savedValue;
                    if (point != null)
                    {
                        param.PersistentData.Clear();
                        param.PersistentData.Append(point);
                    }
                }

                if (com.Params.Input[1] is Param_Point)
                {
                    Param_Point param = (Param_Point)com.Params.Input[1];
                    GH_Point point = ((GooPointControl)_values[1])._savedValue;
                    if (point != null)
                    {
                        param.PersistentData.Clear();
                        param.PersistentData.Append(point);
                    }
                }
            }
            else
            {
                if (com.Params.Input.Count < 3) return;

                if (com.Params.Input[0] is Param_Point)
                {
                    Param_Point param = (Param_Point)com.Params.Input[0];
                    GH_Point point = ((GooPointControl)_values[0])._savedValue;
                    if (point != null)
                    {
                        param.PersistentData.Clear();
                        param.PersistentData.Append(point);
                    }
                }

                if (com.Params.Input[1] is Param_Vector)
                {
                    Param_Vector param = (Param_Vector)com.Params.Input[1];
                    GH_Vector vector = ((GooVectorControl)_values[1])._savedValue;
                    if (vector != null)
                    {
                        param.PersistentData.Clear();
                        param.PersistentData.Append(vector);
                    }
                }

                if (com.Params.Input[2] is Param_Number)
                {
                    Param_Number param = (Param_Number)com.Params.Input[2];

                    if(_values.Length > 2)
                    {
                        GH_Number number = ((GooInputBoxStringControl<GH_Number>)_values[2])._savedValue;
                        if (number != null)
                        {
                            param.PersistentData.Clear();
                            param.PersistentData.Append(number);
                        }
                    }
                    else
                    {
                        GH_Vector vector = ((GooVectorControl)_values[1])._savedValue;
                        if (vector != null)
                        {
                            param.PersistentData.Clear();
                            param.PersistentData.Append(new GH_Number(vector.Value.Length));
                        }
                    }
                }
            }
        }
    }
}
