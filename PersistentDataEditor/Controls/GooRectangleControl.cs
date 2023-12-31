using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;

namespace PersistentDataEditor;

internal class GooRectangleControl(Func<GH_Rectangle> valueGetter, Func<bool> isNull, string name)
    : GooVerticalControlBase<GH_Rectangle>(valueGetter, isNull, name)
{
    public override Guid AddCompnentGuid => new("d93100b6-d50b-40b2-831a-814659dc38e3");

    private protected override GH_Rectangle CreateDefaultValue()
    {
        return new GH_Rectangle(new Rectangle3d(Plane.WorldXY, 1, 1));
    }

    protected override BaseControlItem[] SetControlItems()
    {
        return NewData.RectangleType switch
        {
            Rectangle_Control.Domain_Rectangle =>
            [
                new GooPlaneControl(() => ShowValue == null ? null : new GH_Plane(ShowValue.Value.Plane), _isNull, "P"),
                new GooIntervalControl(() => ShowValue == null ? null : new GH_Interval(ShowValue.Value.X), _isNull, "X"),
                new GooIntervalControl(() => ShowValue == null ? null : new GH_Interval(ShowValue.Value.Y), _isNull, "Y"),
            ],
            Rectangle_Control.Plane_Width_Height =>
            [
                 new GooPlaneControl(() => ShowValue == null ? null : new GH_Plane(ShowValue.Value.Plane), _isNull, "P"),
                new GooNumberControl(() => ShowValue == null ? null : new GH_Number(ShowValue.Value.X.Length), _isNull, "W"),
                new GooNumberControl(() => ShowValue == null ? null : new GH_Number(ShowValue.Value.Y.Length), _isNull, "H"),
            ],
            Rectangle_Control.Center_Rectangle =>
            [
                 new GooPlaneControl(() => ShowValue == null ? null : new GH_Plane(ShowValue.Value.Plane), _isNull, "P"),
                new GooNumberControl(() => ShowValue == null ? null : new GH_Number(ShowValue.Value.X.Length / 2), _isNull, "X"),
                new GooNumberControl(() => ShowValue == null ? null : new GH_Number(ShowValue.Value.Y.Length / 2), _isNull, "Y"),
            ],
            _ =>
            [
                new GooInputBoxStringControl<GH_Rectangle>(() => ShowValue, _isNull, true),
            ],
        };
    }
    protected override GH_Rectangle SetValue(IGH_Goo[] values)
    {
        switch (NewData.RectangleType)
        {
            default:
                return (GH_Rectangle)values[0];
            case Rectangle_Control.Domain_Rectangle:
                return new GH_Rectangle(new Rectangle3d(((GH_Plane)values[0]).Value, ((GH_Interval)values[1]).Value, ((GH_Interval)values[2]).Value));
            case Rectangle_Control.Plane_Width_Height:
                return new GH_Rectangle(new Rectangle3d(((GH_Plane)values[0]).Value, ((GH_Number)values[1]).Value, ((GH_Number)values[2]).Value));
            case Rectangle_Control.Center_Rectangle:
                double x = ((GH_Number)values[1]).Value;
                double y = ((GH_Number)values[2]).Value;
                return new GH_Rectangle(new Rectangle3d(((GH_Plane)values[0]).Value, new Interval(-x, x), new Interval(-y, y)));
        }
    }
    public override void DosomethingWhenCreate(IGH_DocumentObject obj)
    {
        if (obj == null) return;
        GH_Component com = (GH_Component)obj;

        if (com.Params.Input.Count < 3) return;

        if (com.Params.Input[0] is Param_Plane param0 && _values[0].SaveValue is GH_Plane Value0)
        {
            param0.PersistentData.Clear();
            param0.PersistentData.Append(Value0);
        }

        if (com.Params.Input[1] is Param_Interval param1)
        {
            switch (NewData.RectangleType)
            {
                case Rectangle_Control.Domain_Rectangle:
                    if (_values[0].SaveValue is GH_Interval Value11)
                    {
                        param1.PersistentData.Clear();
                        param1.PersistentData.Append(Value11);
                    }
                    break;

                case Rectangle_Control.Plane_Width_Height:
                    if (_values[0].SaveValue is GH_Number Value12)
                    {
                        param1.PersistentData.Clear();
                        param1.PersistentData.Append(new GH_Interval(new Interval(0, Value12.Value)));
                    }
                    break;

                case Rectangle_Control.Center_Rectangle:
                    if (_values[0].SaveValue is GH_Number Value13)
                    {
                        param1.PersistentData.Clear();
                        param1.PersistentData.Append(new GH_Interval(new Interval(-Value13.Value, Value13.Value)));
                    }
                    break;
            }
        }

        if (com.Params.Input[2] is Param_Interval param2)
        {
            switch (NewData.RectangleType)
            {
                case Rectangle_Control.Domain_Rectangle:
                    if (_values[2].SaveValue is GH_Interval Value20)
                    {
                        param2.PersistentData.Clear();
                        param2.PersistentData.Append(Value20);
                    }
                    break;

                case Rectangle_Control.Plane_Width_Height:
                    if (_values[2].SaveValue is GH_Number Value21)
                    {
                        param2.PersistentData.Clear();
                        param2.PersistentData.Append(new GH_Interval(new Interval(0, Value21.Value)));
                    }
                    break;

                case Rectangle_Control.Center_Rectangle:
                    if (_values[2].SaveValue is GH_Number Value22)
                    {
                        param2.PersistentData.Clear();
                        param2.PersistentData.Append(new GH_Interval(new Interval(-Value22.Value, Value22.Value)));
                    }
                    break;
            }
        }
    }
}