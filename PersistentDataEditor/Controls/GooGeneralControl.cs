using Grasshopper.Kernel.Types;
using System;

namespace PersistentDataEditor;

internal class GooGeneralControl<T>(Func<T> valueGetter, Func<bool> isNull)
    : GooInputBoxStringControl<T>(valueGetter, isNull) where T : class, IGH_Goo
{
    public override Guid AddCompnentGuid => default;

    protected override bool IsReadOnly => NewData.GeneralType == General_Control.ReadOnly;
}
