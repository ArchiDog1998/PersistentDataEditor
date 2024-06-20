using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using HarmonyLib;
using System;
using System.Drawing;
using Complex = System.Numerics.Complex;
using Rhino.Geometry;

namespace PersistentDataEditor.Controls;
internal class GooVariableControl3 : GooHorizonalControlBase<IGH_Goo>
{
    public override Guid AddComponentGuid => _values[0].AddComponentGuid;

    private readonly IGH_Param _owner;
    public GooVariableControl3(Func<IGH_Goo> valueGetter, Func<bool> isNull, IGH_Param owner)
        : base(valueGetter, isNull, null)
    {
        owner.ObjectChanged += Owner_ObjectChanged;
        _owner = owner;
        ChangeControlItems();
    }
    private void Owner_ObjectChanged(IGH_DocumentObject sender, GH_ObjectChangedEventArgs e)
    {
        if (e.CustomType == "hint_changed")
        {
            //For some case, the value type have changed. So the Value must be null for avoiding casting exception.
            ShowValue = null;

            ChangeControlItems();
        }
    }
    protected override BaseControlItem[] SetControlItems()
    {
        if (_owner == null) return [];
        var paramType = AccessTools.Method(_owner.GetType(), "GetParamType").Invoke(_owner, []);
        var type = (Type)AccessTools.Property(paramType.GetType(), "Type").GetValue(paramType);

        return GetChangeParam(type);
    }

    private BaseControlItem[] GetChangeParam(Type type)
    {
        if (type == typeof(bool) && Data.UseParamBooleanControl)
        {
            return [new GooBooleanControl(() => (GH_Boolean)ShowValue, _isNull)];
        }
        else if (type == typeof(int) && Data.UseParamIntegerControl)
        {
            return [
                new GooIntegerControl(() =>
                {
                    if (ShowValue is GH_Number number)
                    {
                        return new GH_Integer((int)number.Value);
                    }
                    else if (ShowValue is GH_String str)
                    {
                        if (int.TryParse(str.Value, out var value))
                        {
                            return new GH_Integer(value);
                        }
                        return null;
                    }
                    else
                    {
                        return (GH_Integer)ShowValue;
                    }
                }, _isNull, null)];
        }
        else if (type == typeof(string) && Data.UseParamStringControl)
        {
            return [new GooInputBoxStringControl<GH_String>(() => (GH_String)ShowValue, _isNull)];
        }
        else if (type == typeof(double) && Data.UseParamNumberControl)
        {
            return [new GooNumberControl(() =>
                {
                    if (ShowValue is GH_Integer integer)
                    {
                        return new GH_Number(integer.Value);
                    }
                    else if (ShowValue is GH_String str)
                    {
                        if (double.TryParse(str.Value, out double value))
                        {
                            return new GH_Number(value);
                        }
                        return null;
                    }
                    else
                    {
                        return (GH_Number)ShowValue;
                    }
                }, _isNull, null)];
        }
        else if (type == typeof(Complex) && Data.UseParamComplexControl)
        {
            return [new GooComplexControl(() => (GH_ComplexNumber)ShowValue, _isNull, null)];
        }
        else if (type == typeof(Color) && Data.UseParamColourControl)
        {
            return [new GooColorControl(() => (GH_Colour)ShowValue, _isNull)];
        }
        else if (type == typeof(Point3d) && Data.UseParamPointControl)
        {
            return [new GooPointControl(() => (GH_Point)ShowValue, _isNull, null)];
        }
        else if (type == typeof(Vector3d) && Data.UseParamVectorControl)
        {
            return [new GooVectorControl(() => (GH_Vector)ShowValue, _isNull, null)];
        }
        else if (type == typeof(Plane) && Data.UseParamPlaneControl)
        {
            return [new GooPlaneControl(() => (GH_Plane)ShowValue, _isNull, null)];
        }
        else if (type == typeof(Interval) && Data.UseParamDomainControl)
        {
            return [new GooIntervalControl(() => (GH_Interval)ShowValue, _isNull, null)];
        }
        else if (type == typeof(UVInterval) && Data.UseParamDomain2Control)
        {
            return [new GooInterval2DControl(() => (GH_Interval2D)ShowValue, _isNull, null)];
        }
        else if (type == typeof(Box) && Data.UseParamBoxControl)
        {
            return [new GooBoxControl(() => (GH_Box)ShowValue, _isNull, null)];
        }
        else if (type == typeof(Line) && Data.UseParamLineControl)
        {
            return [new GooLineControl(() => (GH_Line)ShowValue, _isNull, null)];
        }
        else if (type == typeof(Circle) && Data.UseParamCircleControl)
        {
            return [new GooCircleControl(() => (GH_Circle)ShowValue, _isNull, null)];
        }
        else if (type == typeof(Arc) && Data.UseParamArcControl)
        {
            return [new GooArcControl(() => (GH_Arc)ShowValue, _isNull, null)];
        }
        else if (type == typeof(Rectangle3d) && Data.UseParamRectangleControl)
        {
            return [new GooRectangleControl(() => (GH_Rectangle)ShowValue, _isNull, null)];
        }
        else if (type == typeof(DateTime) && Data.UseParamTimeControl)
        {
            return [new GooTimeControl(() => (GH_Time)ShowValue, _isNull)];
        }
        return [new GooGeneralControl<IGH_Goo>(() => ShowValue, _isNull)];
    }

    protected override IGH_Goo SetValue(IGH_Goo[] values)
    {
        return values[0];
    }

    public override void DoSomethingWhenCreate(IGH_DocumentObject obj)
    {
        _values[0].DoSomethingWhenCreate(obj);
    }
}
