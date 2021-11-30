using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class GooIntegerControl : GooInputBoxControl<GH_Integer>
    {
        internal GooIntegerControl(Func<GH_Integer> valueGetter, Action<GH_Integer, bool> valueChanged) :
                base(valueGetter, valueChanged)
        {

        }

        protected override void SaveString(string str)
        {
            if (GH_Convert.ToInt32(str, out int number, GH_Conversion.Both))
            {
                Value = new GH_Integer(number);
            }
        }
    }
}
