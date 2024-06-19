using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;

namespace PersistentDataEditor;

internal class GooBoxControl(Func<GH_Box> valueGetter, Func<bool> isNull, string name) : GooVerticalControlBase<GH_Box>(valueGetter, isNull, name)
{
    public override Guid AddComponentGuid => Data.BoxType switch
    {
        Box_Control.Center_Box => new Guid("28061aae-04fb-4cb5-ac45-16f3b66bc0a4"),
        Box_Control.Box_Rectangle => new Guid("d0a56c9e-2483-45e7-ab98-a450b97f1bc0"),
        _ => new Guid("79aa7f47-397c-4d3f-9761-aaf421bb7f5f"),
    };

    private protected override GH_Box CreateDefaultValue()
    {
        return new GH_Box(new Box(Plane.WorldXY, new Interval(0, 1), new Interval(0, 1), new Interval(0, 1)));
    }

    protected override BaseControlItem[] SetControlItems()
    {
        return Data.BoxType switch
        {
            Box_Control.Domain_Box =>
            [
                 new GooPlaneControl(() => ShowValue == null ? null : new GH_Plane(ShowValue.Value.Plane), _isNull, "P"),
                new GooIntervalControl(() => ShowValue == null ? null : new GH_Interval(ShowValue.Value.X), _isNull, "X"),
                new GooIntervalControl(() => ShowValue == null ? null : new GH_Interval(ShowValue.Value.Y), _isNull, "Y"),
                new GooIntervalControl(() => ShowValue == null ? null : new GH_Interval(ShowValue.Value.Z), _isNull, "Z"),
            ],
            Box_Control.Center_Box =>
            [
                 new GooPlaneControl(() => ShowValue == null ? null : new GH_Plane(ShowValue.Value.Plane), _isNull, "P"),
                new GooNumberControl(() => ShowValue == null ? null : new GH_Number(ShowValue.Value.X.Length / 2), _isNull, "X"),
                new GooNumberControl(() => ShowValue == null ? null : new GH_Number(ShowValue.Value.Y.Length / 2), _isNull, "Y"),
                new GooNumberControl(() => ShowValue == null ? null : new GH_Number(ShowValue.Value.Z.Length / 2), _isNull, "Z"),
            ],
            Box_Control.Box_Rectangle =>
            [
                 new GooRectangleControl(() => ShowValue == null ? null
                     : new GH_Rectangle(new Rectangle3d(
                         ShowValue.Value.Plane, ShowValue.Value.X, ShowValue.Value.Y)), _isNull, "R"),
                new GooIntervalControl(() => ShowValue == null ? null : new GH_Interval(ShowValue.Value.Z), _isNull, "H"),
            ],
            _ =>
            [
                new GooInputBoxStringControl<GH_Box>(() => ShowValue, _isNull, true),
            ],
        };
    }
    protected override GH_Box SetValue(IGH_Goo[] values)
    {
        switch (Data.BoxType)
        {
            default:
                return (GH_Box)values[0];
            case Box_Control.Domain_Box:
                return new GH_Box(new Box(((GH_Plane)values[0]).Value, ((GH_Interval)values[1]).Value, ((GH_Interval)values[2]).Value, ((GH_Interval)values[3]).Value));
            case Box_Control.Center_Box:
                double x = ((GH_Number)values[1]).Value;
                double y = ((GH_Number)values[2]).Value;
                double z = ((GH_Number)values[3]).Value;
                return new GH_Box(new Box(((GH_Plane)values[0]).Value, new Interval(-x, x), new Interval(-y, y), new Interval(-z, z)));
            case Box_Control.Box_Rectangle:
                Rectangle3d rect = ((GH_Rectangle)values[0]).Value;
                Interval height = ((GH_Interval)values[1]).Value;
                return new GH_Box(new Box(rect.Plane, rect.X, rect.Y, height));

        }
    }

    public override void DoSomethingWhenCreate(IGH_DocumentObject obj)
    {
        if (obj == null) return;
        GH_Component com = (GH_Component)obj;

        switch (Data.BoxType)
        {
            case Box_Control.Domain_Box:
                if (com.Params.Input.Count < 4) return;

                if (com.Params.Input[0] is Param_Plane param0 && _values[0].SaveValue is GH_Plane Value0)
                {
                    param0.PersistentData.Clear();
                    param0.PersistentData.Append(Value0);
                }

                if (com.Params.Input[1] is Param_Interval param1 && _values[1].SaveValue is GH_Interval Value1)
                {
                    param1.PersistentData.Clear();
                    param1.PersistentData.Append(Value1);
                }

                if (com.Params.Input[2] is Param_Interval param2 && _values[2].SaveValue is GH_Interval Value2)
                {
                    param2.PersistentData.Clear();
                    param2.PersistentData.Append(Value2);
                }

                if (com.Params.Input[3] is Param_Interval param3 && _values[3].SaveValue is GH_Interval Value3)
                {
                    param3.PersistentData.Clear();
                    param3.PersistentData.Append(Value3);
                }
                break;

            case Box_Control.Center_Box:
                if (com.Params.Input.Count < 4) return;

                if (com.Params.Input[0] is Param_Plane param00 && _values[0].SaveValue is GH_Plane Value00)
                {
                    param00.PersistentData.Clear();
                    param00.PersistentData.Append(Value00);
                }

                if (com.Params.Input[1] is Param_Number param01 && _values[1].SaveValue is GH_Number Value01)
                {
                    param01.PersistentData.Clear();
                    param01.PersistentData.Append(Value01);
                }

                if (com.Params.Input[2] is Param_Number param02 && _values[2].SaveValue is GH_Number Value02)
                {
                    param02.PersistentData.Clear();
                    param02.PersistentData.Append(Value02);
                }

                if (com.Params.Input[3] is Param_Number param03 && _values[3].SaveValue is GH_Number Value03)
                {
                    param03.PersistentData.Clear();
                    param03.PersistentData.Append(Value03);
                }
                break;

            case Box_Control.Box_Rectangle:
                if (com.Params.Input.Count < 2) return;

                if (com.Params.Input[0] is Param_Rectangle param10 && _values[0].SaveValue is GH_Rectangle Value10)
                {
                    param10.PersistentData.Clear();
                    param10.PersistentData.Append(Value10);
                }

                if (com.Params.Input[1] is Param_Interval param11 && _values[1].SaveValue is GH_Interval Value11)
                {
                    param11.PersistentData.Clear();
                    param11.PersistentData.Append(Value11);
                }

                break;

        }
    }
}
