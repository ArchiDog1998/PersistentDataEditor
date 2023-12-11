using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace PersistentDataEditor;

internal class ParamStringControl(GH_PersistentParam<GH_String> owner) : ParamControlBase<GH_String>(owner)
{
    protected override GooControlBase<GH_String> SetUpControl(IGH_Param param)
    {
        return new GooInputBoxStringControl<GH_String>(() => OwnerGooData, () => IsNull);
    }
}

internal class ParamBooleanControl(GH_PersistentParam<GH_Boolean> owner) : ParamControlBase<GH_Boolean>(owner)
{
    protected override GooControlBase<GH_Boolean> SetUpControl(IGH_Param param)
    {
        return new GooBooleanControl(() => OwnerGooData, () => IsNull);
    }
}

internal class ParamIntegerControl(GH_PersistentParam<GH_Integer> owner) : ParamControlBase<GH_Integer>(owner)
{
    private static readonly FieldInfo namedValueListInfo = typeof(Param_Integer).FindField("m_namedValues");
    private static FieldInfo nameInfo;
    private static FieldInfo valueInfo;

    protected override GooControlBase<GH_Integer> SetUpControl(IGH_Param param)
    {
        if (param is Param_Integer integer && integer.HasNamedValues)
        {

            IList list = (IList)namedValueListInfo.GetValue(param);

            SortedList<int, string> _keyValues = new SortedList<int, string>();
            foreach (var item in list)
            {
                nameInfo = nameInfo ?? item.GetType().FindField("Name");
                valueInfo = valueInfo ?? item.GetType().FindField("Value");

                _keyValues[(int)valueInfo.GetValue(item)] = (string)nameInfo.GetValue(item);
            }
            return new GooEnumControl(() => OwnerGooData, () => IsNull, _keyValues);
        }
        else
        {
            return new GooIntegerControl(() => OwnerGooData, () => IsNull, null);
        }
        
    }
}

internal class ParamNumberControl(GH_PersistentParam<GH_Number> owner) : ParamControlBase<GH_Number>(owner)
{
    protected override GooControlBase<GH_Number> SetUpControl(IGH_Param param)
    {
        return new GooNumberControl(() => OwnerGooData, () => IsNull, null);
    }
}


internal class ParamColourControl(GH_PersistentParam<GH_Colour> owner) : ParamControlBase<GH_Colour>(owner)
{
    protected override GooControlBase<GH_Colour> SetUpControl(IGH_Param param)
    {
        return new GooColorControl(() => OwnerGooData, () => IsNull);
    }
}

internal class ParamMaterialControl(GH_PersistentParam<GH_Material> owner) : ParamControlBase<GH_Material>(owner)
{
    protected override GooControlBase<GH_Material> SetUpControl(IGH_Param param)
    {
        return new GooMaterialControl(() => OwnerGooData, () => IsNull);
    }
}

internal class ParamIntervalControl(GH_PersistentParam<GH_Interval> owner) : ParamControlBase<GH_Interval>(owner)
{
    protected override GooControlBase<GH_Interval> SetUpControl(IGH_Param param)
    {
        return new GooIntervalControl(() => OwnerGooData, () => IsNull, null);
    }
}

internal class ParamPointControl(GH_PersistentParam<GH_Point> owner) : ParamControlBase<GH_Point>(owner)
{
    protected override GooControlBase<GH_Point> SetUpControl(IGH_Param param)
    {
        return new GooPointControl(() => OwnerGooData, () => IsNull, null);
    }
}

internal class ParamVectorControl(GH_PersistentParam<GH_Vector> owner) : ParamControlBase<GH_Vector>(owner)
{
    protected override GooControlBase<GH_Vector> SetUpControl(IGH_Param param)
    {
        return new GooVectorControl(() => OwnerGooData, () => IsNull, null);
    }
}

internal class ParamComplexControl(GH_PersistentParam<GH_ComplexNumber> owner) : ParamControlBase<GH_ComplexNumber>(owner)
{
    protected override GooControlBase<GH_ComplexNumber> SetUpControl(IGH_Param param)
    {
        return new GooComplexControl(() => OwnerGooData, () => IsNull, null);
    }
}

internal class ParamInterval2DControl(GH_PersistentParam<GH_Interval2D> owner) : ParamControlBase<GH_Interval2D>(owner)
{
    protected override GooControlBase<GH_Interval2D> SetUpControl(IGH_Param param)
    {
        return new GooInterval2DControl(() => OwnerGooData, () => IsNull, null);
    }
}

internal class ParamLineControl(GH_PersistentParam<GH_Line> owner) : ParamControlBase<GH_Line>(owner)
{
    protected override GooControlBase<GH_Line> SetUpControl(IGH_Param param)
    {
        return new GooLineControl(() => OwnerGooData, () => IsNull, null);
    }
}

internal class ParamPlaneControl(GH_PersistentParam<GH_Plane> owner) : ParamControlBase<GH_Plane>(owner)
{
    protected override GooControlBase<GH_Plane> SetUpControl(IGH_Param param)
    {
        return new GooPlaneControl(() => OwnerGooData, () => IsNull, null);
    }
}

internal class ParamCircleControl(GH_PersistentParam<GH_Circle> owner) : ParamControlBase<GH_Circle>(owner)
{
    protected override GooControlBase<GH_Circle> SetUpControl(IGH_Param param)
    {
        return new GooCircleControl(() => OwnerGooData, () => IsNull, null);
    }
}

internal class ParamRectangleControl(GH_PersistentParam<GH_Rectangle> owner) : ParamControlBase<GH_Rectangle>(owner)
{
    protected override GooControlBase<GH_Rectangle> SetUpControl(IGH_Param param)
    {
        return new GooRectangleControl(() => OwnerGooData, () => IsNull, null);
    }
}

internal class ParamBoxControl(GH_PersistentParam<GH_Box> owner) : ParamControlBase<GH_Box>(owner)
{
    protected override GooControlBase<GH_Box> SetUpControl(IGH_Param param)
    {
        return new GooBoxControl(() => OwnerGooData, () => IsNull, null);
    }
}

internal class ParamArcControl(GH_PersistentParam<GH_Arc> owner) : ParamControlBase<GH_Arc>(owner)
{
    protected override GooControlBase<GH_Arc> SetUpControl(IGH_Param param)
    {
        return new GooArcControl(() => OwnerGooData, () => IsNull, null);
    }
}

internal class ParamGeneralControl<T>(GH_PersistentParam<T> owner) : ParamControlBase<T>(owner) where T : class, IGH_Goo
{
    protected override GooControlBase<T> SetUpControl(IGH_Param param)
    {
        return new GooGeneralControl<T>(() => OwnerGooData, () => IsNull);
    }
}

internal class ParamVariableControl(Param_ScriptVariable owner) : ParamGeneralControl<IGH_Goo>(owner)
{
    protected override bool Valid => base.Valid && Owner.Access == GH_ParamAccess.item;

    protected override GooControlBase<IGH_Goo> SetUpControl(IGH_Param param)
    {
        return new GooVariableControl(() => OwnerGooData, () => IsNull, (Param_ScriptVariable)param);
    }
}
