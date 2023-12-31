using Grasshopper.Kernel.Types;
using System;

namespace PersistentDataEditor;

internal class GooIntegerControl(Func<GH_Integer> valueGetter, Func<bool> isNull, string name) : GooHorizonalControlBase<GH_Integer>(valueGetter, isNull, name)
{
    public override Guid AddCompnentGuid => new("57da07bd-ecab-415d-9d86-af36d7073abc");

    protected override string AddCompnentInit => base.AddCompnentInit ?? "0..100";

    protected override GH_Integer SetValue(IGH_Goo[] values)
    {
        return (GH_Integer)values[0];
    }

    protected override BaseControlItem[] SetControlItems()
    {
        return
        [
            new GooInputBoxStringControl<GH_Integer>(() =>
            {
                if (ShowValue == null) return null;
                return new GH_Integer(ShowValue.Value);
            }, _isNull),
        ];
    }
}
