using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Linq;

namespace PersistentDataEditor;

internal class GooPlaneControl(Func<GH_Plane> valueGetter, Func<bool> isNull, string name)
    : GooVerticalControlBase<GH_Plane>(valueGetter, isNull, name)
{
    public override Guid AddComponentGuid => Data.PlaneType == Plane_Control.OZ
        ? new Guid("cfb6b17f-ca82-4f5d-b604-d4f69f569de3")
        : new Guid("bc3e379e-7206-4e7b-b63a-ff61f4b38a3e");

    private protected override GH_Plane CreateDefaultValue()
    {
        return new(Plane.WorldXY);
    }

    protected override BaseControlItem[] SetControlItems()
    {
        return Data.PlaneType switch
        {
            Plane_Control.OZ =>
            [
                new GooPointControl(() => ShowValue == null ? null : new GH_Point(ShowValue.Value.Origin), _isNull, "O"),
                new GooVectorControl(() => ShowValue == null ? null : new GH_Vector(ShowValue.Value.ZAxis), _isNull, "Z"),
            ],
            Plane_Control.OXY =>
            [
                new GooPointControl(() => ShowValue == null ? null : new GH_Point(ShowValue.Value.Origin), _isNull, "O"),
                new GooVectorControl(() => ShowValue == null ? null : new GH_Vector(ShowValue.Value.XAxis), _isNull, "X"),
                new GooVectorControl(() => ShowValue == null ? null : new GH_Vector(ShowValue.Value.YAxis), _isNull, "Y"),
            ],
            _ =>
            [
                new GooInputBoxStringControl<GH_Plane>(() => ShowValue, _isNull, true),
            ],
        };
    }

    protected override GH_Plane SetValue(IGH_Goo[] values)
    {
        return Data.PlaneType switch
        {
            Plane_Control.OZ => new GH_Plane(new Plane(((GH_Point)values[0]).Value, ((GH_Vector)values[1]).Value)),
            Plane_Control.OXY => new GH_Plane(new Plane(((GH_Point)values[0]).Value, ((GH_Vector)values[1]).Value, ((GH_Vector)values[2]).Value)),
            _ => (GH_Plane)values[0],
        };
    }

    public override void DoSomethingWhenCreate(IGH_DocumentObject obj)
    {
        if (obj == null) return;
        GH_Component com = (GH_Component)obj;

        if (Data.PlaneType == Plane_Control.OZ)
        {
            if (com.Params.Input.Count < 2) return;

            if (com.Params.Input[0] is Param_Point param0)
            {
                var value = _values[0].SaveValue;
                if (value != null)
                {
                    param0.PersistentData.Clear();
                    param0.PersistentData.Append(value);
                }
            }

            if (com.Params.Input[1] is Param_Vector param1)
            {
                IGH_Goo value = _values[0].SaveValue;
                if (value != null)
                {
                    param1.PersistentData.Clear();
                    param1.PersistentData.Append(value);
                }
            }
        }
        else
        {
            if (com.Params.Input.Count < 3) return;

            if (com.Params.Input[0] is Param_Point param0)
            {
                IGH_Goo value = _values[0].SaveValue;
                if (value != null)
                {
                    param0.PersistentData.Clear();
                    param0.PersistentData.Append(value);
                }
            }

            if (com.Params.Input[1] is Param_Vector param1)
            {
                var value = _values[0].SaveValue;
                if (value != null)
                {
                    param1.PersistentData.Clear();
                    param1.PersistentData.Append(value);
                }
            }

            if (com.Params.Input[2] is Param_Vector param2)
            {
                var value = _values[0].SaveValue;
                if (value != null)
                {
                    param2.PersistentData.Clear();
                    param2.PersistentData.Append(value);
                }
            }
        }
    }
}
