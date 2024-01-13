using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Parameters.Hints;
using Grasshopper.Kernel.Types;
using System;

namespace PersistentDataEditor;

internal class GooVariableControl : GooHorizonalControlBase<IGH_Goo>
{
    public override Guid AddCompnentGuid => _values[0].AddCompnentGuid;

    private readonly IGH_Param _owner;
    private readonly Func<IGH_Param, Guid> _getId;
    public GooVariableControl(Func<IGH_Goo> valueGetter, Func<bool> isNull, IGH_Param owner, Func<IGH_Param, Guid> getId)
        : base(valueGetter, isNull, null)
    {
        owner.ObjectChanged += Owner_ObjectChanged;
        _owner = owner;
        _getId = getId;
        ChangeControlItems();
    }

    private void Owner_ObjectChanged(IGH_DocumentObject sender, GH_ObjectChangedEventArgs e)
    {
        if (e.Tag is IGH_TypeHint)
        {
            //For some case, the value type have changed. So the Value must be null for avoiding casting exception.
            ShowValue = null;

            ChangeControlItems();
        }
    }

    private BaseControlItem[] ChangeParamId(Guid hintid)
    {
        if (hintid == new GH_ArcHint().HintID && Data.UseParamArcControl)
            return [
                new GooArcControl (() => (GH_Arc)ShowValue, _isNull, null),
            ];
        else if ((hintid == new GH_BooleanHint_CS().HintID || hintid == new GH_BooleanHint_VB().HintID) && Data.UseParamBooleanControl)
            return [
                new GooBooleanControl(() => (GH_Boolean)ShowValue, _isNull),
            ];
        else if (hintid == new GH_BoxHint().HintID && Data.UseParamBoxControl)
            return [
                new GooBoxControl(() => (GH_Box)ShowValue, _isNull, null),
            ];
        else if (hintid == new GH_CircleHint().HintID && Data.UseParamCircleControl)
            return [
                new GooCircleControl(() => (GH_Circle)ShowValue, _isNull, null),
            ];
        else if (hintid == new GH_ColorHint().HintID && Data.UseParamColourControl)
            return [
                new GooColorControl(() => (GH_Colour)ShowValue, _isNull),
            ];
        else if (hintid == new GH_ComplexHint().HintID && Data.UseParamComplexControl)
            return [
                new GooComplexControl(() => (GH_ComplexNumber)ShowValue, _isNull, null),
            ];
        else if ((hintid == new GH_DoubleHint_CS().HintID || hintid == new GH_DoubleHint_VB().HintID) && Data.UseParamNumberControl)
            return [
                new GooNumberControl(() =>
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
                }, _isNull, null),
            ];
        else if ((hintid == new GH_IntegerHint_CS().HintID || hintid == new GH_IntegerHint_VB().HintID) && Data.UseParamIntegerControl)
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
                }, _isNull, null),
            ];
        else if (hintid == new GH_IntervalHint().HintID && Data.UseParamDomainControl)
            return [
                new GooIntervalControl(() => (GH_Interval)ShowValue, _isNull, null),
            ];
        else if (hintid == new GH_LineHint().HintID && Data.UseParamLineControl)
            return [
                new GooLineControl(() => (GH_Line)ShowValue, _isNull, null),
            ];
        else if (hintid == new GH_PlaneHint().HintID && Data.UseParamPlaneControl)
            return [
                new GooPlaneControl(() => (GH_Plane)ShowValue, _isNull, null),
            ];
        else if (hintid == new GH_Point3dHint().HintID && Data.UseParamPointControl)
            return [
                new GooPointControl(() => (GH_Point)ShowValue, _isNull, null),
            ];
        else if (hintid == new GH_Rectangle3dHint().HintID && Data.UseParamRectangleControl)
            return [
                new GooRectangleControl(() => (GH_Rectangle)ShowValue, _isNull, null),
            ];
        else if ((hintid == new GH_StringHint_CS().HintID || hintid == new GH_StringHint_VB().HintID) && Data.UseParamStringControl)
            return [
                new GooInputBoxStringControl<GH_String>(() => (GH_String)ShowValue, _isNull),
            ];
        else if (hintid == new GH_UVIntervalHint().HintID && Data.UseParamDomain2Control)
            return [
                new GooInterval2DControl(() => (GH_Interval2D)ShowValue, _isNull, null),
            ];
        else if (hintid == new GH_Vector3dHint().HintID && Data.UseParamVectorControl)
            return [
                new GooVectorControl(() => (GH_Vector)ShowValue, _isNull, null),
            ];
        else
            return [new GooGeneralControl<IGH_Goo>(() => ShowValue, _isNull)];
    }

    protected override BaseControlItem[] SetControlItems()
    {
        var id = _getId?.Invoke(_owner) ?? default;
        return ChangeParamId(id);
    }

    private static bool ShouldUse<T>() where T : class, IGH_Goo
    {
        string saveBooleanKey = "UseParam" + typeof(T).Name;
        return Instances.Settings.GetValue(saveBooleanKey, true);
    }

    protected override IGH_Goo SetValue(IGH_Goo[] values)
    {
        return values[0];
    }

    public override void DosomethingWhenCreate(IGH_DocumentObject obj)
    {
        _values[0].DosomethingWhenCreate(obj);
    }
}
