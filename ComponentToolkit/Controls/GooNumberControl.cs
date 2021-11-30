using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class GooNumberControl : GooInputBoxControl<GH_Number>
    {
        internal GooNumberControl(Func<GH_Number> valueGetter, Action<GH_Number, bool> valueChanged) :
        base(valueGetter, valueChanged)
        {

        }

        protected override void SaveString(string str)
        {
            if (GH_Convert.ToDouble(str, out double number, GH_Conversion.Both))
            {
                Value = new GH_Number(number);
            }
        }
    }
}
