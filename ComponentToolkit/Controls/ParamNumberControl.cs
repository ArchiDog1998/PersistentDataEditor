﻿using Grasshopper.Kernel;
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
        protected override Guid AddCompnentGuid => new Guid("57da07bd-ecab-415d-9d86-af36d7073abc");
        protected override bool Valid => base.Valid && Datas.UseParamNumberControl;
        internal ParamNumberControl(GH_PersistentParam<GH_Number> owner) : base(owner)
        {
        }
    }
}
