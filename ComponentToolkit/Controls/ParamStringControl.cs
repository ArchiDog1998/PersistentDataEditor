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
        protected override Guid AddCompnentGuid => new Guid("59e0b89a-e487-49f8-bab8-b5bab16be14c");
        protected override bool Valid => base.Valid && Datas.UseParamStringControl;
        internal ParamStringControl(GH_PersistentParam<GH_String> owner):base(owner)
        {
        }

        protected override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            obj.Name = obj.NickName = Owner.Name;
        }
    }
}
