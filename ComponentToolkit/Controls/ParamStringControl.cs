using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ComponentToolkit
{
    internal class ParamStringControl : ParamInputBoxControl<GH_String>
    {
        protected override bool Valid => base.Valid && GH_ComponentAttributesReplacer.UseParamStringControl;
        internal ParamStringControl(GH_PersistentParam<GH_String> owner):base(owner)
        {
        }
    }
}
