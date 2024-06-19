using Grasshopper.Kernel.Types;
using System;

namespace PersistentDataEditor;

internal class GooGeneralControl<T>(Func<T> valueGetter, Func<bool> isNull)
    : GooInputBoxStringControl<T>(valueGetter, isNull) where T : class, IGH_Goo
{
    public override Guid AddComponentGuid => default;

    protected override bool IsReadOnly => Data.GeneralType == General_Control.ReadOnly;
}
