using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class GooStringControl : GooInputBoxControl<GH_String>
    {
        internal GooStringControl(Func<GH_String> valueGetter, Action<GH_String, bool> valueChanged):
            base(valueGetter, valueChanged)
        {

        }

        protected override void SaveString(string str)
        {
            Value = new GH_String(str);
        }
    }
}
