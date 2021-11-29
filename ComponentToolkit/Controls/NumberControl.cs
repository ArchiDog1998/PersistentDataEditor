using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    public class NumberControl : InputBoxControl<GH_Number>
    {
        public override string ShowString => Owner.PersistentData.get_FirstItem(false)?.Value.ToString();

        public NumberControl(GH_PersistentParam<GH_Number> owner) : base(owner)
        {

        }

        protected override void Save(string str)
        {
            if (double.TryParse(str, out double d))
            {
                Owner.RecordUndoEvent("Set: " + str);
                Owner.PersistentData.Clear();
                Owner.PersistentData.Append(new GH_Number(d));
            }

        }

    }
}
