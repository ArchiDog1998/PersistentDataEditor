using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    public class ParamGeneralControl<T> : ParamInputBoxControl<T> where T : class, IGH_Goo
    {
        protected override bool Valid => base.Valid && Datas.UseParamGeneralControl;

        public ParamGeneralControl(GH_PersistentParam<T> owner) : base(owner)
        {
        }
    }
}
