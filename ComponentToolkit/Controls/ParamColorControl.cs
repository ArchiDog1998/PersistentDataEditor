using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel.Special;
using System.Reflection;

namespace ComponentToolkit
{
    internal class ParamColorControl : ParamControlBase<GH_Colour>
    {
        internal static readonly FieldInfo SwatchColorInfo = typeof(GH_ColourSwatch).GetRuntimeFields().Where(m => m.Name.Contains("m_SwatchColour")).First();
        protected override bool Valid => base.Valid && Datas.UseParamColourControl;

        protected override Guid AddCompnentGuid => new Guid("9c53bac0-ba66-40bd-8154-ce9829b9db1a");
        internal ParamColorControl(GH_PersistentParam<GH_Colour> owner) : base(owner)
        {
        }

        protected override BaseControlItem[] SetControlItems(GH_PersistentParam<GH_Colour> owner)
        {
            return new BaseControlItem[]
            {
                new GooColorControl(() => OwnerGooData),
            };
        }

        protected override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            GH_ColourSwatch swatch = (GH_ColourSwatch)obj;
            if (swatch == null) return;
            if (OwnerGooData != null)
                SwatchColorInfo.SetValue(swatch, OwnerGooData.Value);
        }
    }
}
