using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;

namespace PersistentDataEditor;

internal class GooLineControl(Func<GH_Line> valueGetter, Func<bool> isNull, string name)
    : GooVerticalControlBase<GH_Line>(valueGetter, isNull, name)
{
    public override Guid AddCompnentGuid => Data.LineType == Line_Control.From_To
        ? new("4c4e56eb-2f04-43f9-95a3-cc46a14f495a")
        : new("4c619bc9-39fd-4717-82a6-1e07ea237bbe");

    private protected override GH_Line CreateDefaultValue()
        => new(new Line(Point3d.Origin, new Point3d(1, 0, 0)));

    protected override BaseControlItem[] SetControlItems()
    {
        return Data.LineType switch
        {
            Line_Control.From_To =>
            [
                 new GooPointControl(() => ShowValue == null ? null : new GH_Point(ShowValue.Value.From), _isNull, "F"),
                new GooPointControl(() => ShowValue == null ? null : new GH_Point(ShowValue.Value.To), _isNull, "T"),
            ],
            Line_Control.Start_Direction =>
            [
                 new GooPointControl(() => ShowValue == null ? null : new GH_Point(ShowValue.Value.From), _isNull, "S"),
                new GooVectorControl(() => ShowValue == null ? null : new GH_Vector(ShowValue.Value.Direction), _isNull, "D"),
            ],
            Line_Control.SDL =>
            [
                 new GooPointControl(() => ShowValue == null ? null : new GH_Point(ShowValue.Value.From), _isNull, "S"),
                new GooVectorControl(() => ShowValue == null ? null : new GH_Vector(ShowValue.Value.UnitTangent), _isNull, "D"),
                new GooNumberControl(() => ShowValue == null ? null : new GH_Number(ShowValue.Value.Length), _isNull, "L"),
            ],
            _ =>
            [
                new GooInputBoxStringControl<GH_Line>(() => ShowValue, _isNull, true),
            ],
        };
    }

    protected override GH_Line SetValue(IGH_Goo[] values)
    {
        return Data.LineType switch
        {
            Line_Control.From_To => new GH_Line(new Line(((GH_Point)values[0]).Value, ((GH_Point)values[1]).Value)),
            Line_Control.Start_Direction => new GH_Line(new Line(((GH_Point)values[0]).Value, ((GH_Vector)values[1]).Value)),
            Line_Control.SDL => new GH_Line(new Line(((GH_Point)values[0]).Value, ((GH_Vector)values[1]).Value,
                                ((GH_Number)values[2]).Value)),
            _ => (GH_Line)values[0],
        };
    }

    public override void DosomethingWhenCreate(IGH_DocumentObject obj)
    {
        if (obj == null) return;
        GH_Component com = (GH_Component)obj;

        if (Data.LineType == Line_Control.From_To)
        {
            if (com.Params.Input.Count < 2) return;

            if (com.Params.Input[0] is Param_Point param0 && _values[0].SaveValue is GH_Point Value0)
            {
                param0.PersistentData.Clear();
                param0.PersistentData.Append(Value0);
            }

            if (com.Params.Input[1] is Param_Point param1 && _values[1].SaveValue is GH_Point Value1)
            {
                param1.PersistentData.Clear();
                param1.PersistentData.Append(Value1);
            }
        }
        else
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

            if (com.Params.Input[2] is Param_Number param2)
            {
                if (_values.Length > 2)
                {
                    if (_values[2].SaveValue is GH_Number number)
                    {
                        param2.PersistentData.Clear();
                        param2.PersistentData.Append(number);
                    }
                }
                else
                {
                    if (_values[1].SaveValue is GH_Vector vector)
                    {
                        param2.PersistentData.Clear();
                        param2.PersistentData.Append(new GH_Number(vector.Value.Length));
                    }
                }
            }
        }
    }
}
