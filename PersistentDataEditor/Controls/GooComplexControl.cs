using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using System;

namespace PersistentDataEditor;

internal class GooComplexControl(Func<GH_ComplexNumber> valueGetter, Func<bool> isNull, string name)
    : GooHorizonalControlBase<GH_ComplexNumber>(valueGetter, isNull, name)
{
    public override Guid AddCompnentGuid => new("63d12974-2915-4ccf-ac26-5d566c3bac92");

    protected override BaseControlItem[] SetControlItems()
    {
        return (Complex_Control)Instances.Settings.GetValue(typeof(Complex_Control).FullName, 0) switch
        {
            Complex_Control.Real_Imaginary =>
            [
                new StringRender("R"),
                new GooInputBoxStringControl<GH_Number>(() => ShowValue == null ? null : new GH_Number(ShowValue.Value.Real), _isNull),
                new StringRender("i"),
                new GooInputBoxStringControl<GH_Number>(() => ShowValue == null ? null : new GH_Number(ShowValue.Value.Imaginary), _isNull),
            ],
            _ =>
            [
                new GooInputBoxStringControl<GH_ComplexNumber>(() => ShowValue, _isNull),
            ],
        };
    }

    protected override GH_ComplexNumber SetValue(IGH_Goo[] values)
    {
        return Data.ComplexType switch
        {
            Complex_Control.Real_Imaginary => new GH_ComplexNumber(new Complex(((GH_Number)values[0]).Value, ((GH_Number)values[1]).Value)),
            _ => (GH_ComplexNumber)values[0],
        };
    }

    public override void DosomethingWhenCreate(IGH_DocumentObject obj)
    {
        if (obj == null) return;
        GH_Component com = (GH_Component)obj;

        if (com.Params.Input.Count < 2) return;

        if (com.Params.Input[0] is Param_Number param0 && _values[0].SaveValue is GH_Number Value0)
        {
            param0.PersistentData.Clear();
            param0.PersistentData.Append(Value0);
        }

        if (com.Params.Input[1] is Param_Number param1 && _values[1].SaveValue is GH_Number Value1)
        {
            param1.PersistentData.Clear();
            param1.PersistentData.Append(Value1);
        }
    }
}
