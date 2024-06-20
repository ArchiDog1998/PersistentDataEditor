using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using System;

namespace PersistentDataEditor.Controls;

internal class GooMaterialControl(Func<GH_Material> valueGetter, Func<bool> isNull)
    : GooHorizonalControlBase<GH_Material>(valueGetter, isNull, null)
{
    public override Guid AddComponentGuid => new("9c53bac0-ba66-40bd-8154-ce9829b9db1a");

    protected override BaseControlItem[] SetControlItems()
    {
        return
        [
            new GooColorControl(() =>
            {
                if (ShowValue == null) return null;
                return new GH_Colour(ShowValue.Value.Diffuse);
            }, _isNull),
        ];
    }

    protected override GH_Material SetValue(IGH_Goo[] values)
    {
        return new(((GH_Colour)values[0]).Value);
    }

    public override void DoSomethingWhenCreate(IGH_DocumentObject obj)
    {
        GH_ColourSwatch swatch = (GH_ColourSwatch)obj;
        if (swatch == null) return;

        GH_Colour colour = ((GooColorControl)_values[0])._savedValue;
        if (colour != null)
            GooColorControl.SwatchColorInfo.SetValue(swatch, colour.Value);
    }
}
