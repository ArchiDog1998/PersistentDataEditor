using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using HarmonyLib;
using System;
using System.Drawing;
using Complex = System.Numerics.Complex;

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
        if (type == typeof(int) && Data.UseParamIntegerControl)
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
        if (type == typeof(string) && Data.UseParamStringControl)
        {
            return [new GooInputBoxStringControl<GH_String>(() => (GH_String)ShowValue, _isNull)];
        }
        if (type == typeof(double) && Data.UseParamNumberControl)
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
        if (type == typeof(Complex) && Data.UseParamComplexControl)
        {
            return [new GooComplexControl(() => (GH_ComplexNumber)ShowValue, _isNull, null)];
        }
        if (type == typeof(Color) && Data.UseParamColourControl)
        {
            return [new GooColorControl(() => (GH_Colour)ShowValue, _isNull)];
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
