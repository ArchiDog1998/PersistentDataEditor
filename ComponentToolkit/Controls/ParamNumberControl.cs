using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class ParamNumberControl : ParamInputBoxControl<GH_Number>
    {
        protected override bool Valid => base.Valid && Datas.UseParamNumberControl;
        internal ParamNumberControl(GH_PersistentParam<GH_Number> owner) : base(owner)
        {
        }
    }
}
