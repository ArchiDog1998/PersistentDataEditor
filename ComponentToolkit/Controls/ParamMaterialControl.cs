using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class ParamMaterialControl : ParamControlBase<GH_Material>
    {
        protected override bool Valid => base.Valid && Datas.UseParamMaterialControl;

        protected override Guid AddCompnentGuid => new Guid("9c53bac0-ba66-40bd-8154-ce9829b9db1a");

        public ParamMaterialControl(GH_PersistentParam<GH_Material> owner) : base(owner)
        {

        }

        protected override BaseControlItem[] SetControlItems(GH_PersistentParam<GH_Material> owner)
        {
            return new BaseControlItem[]
            {
                new GooColorControl(()=>
                {
                    if(OwnerGooData == null) return null;
                    return new GH_Colour(OwnerGooData.Value.Diffuse);
                }),
            };
        }

        protected override GH_Material SetValue(IGH_Goo[] values)
        {
            return new GH_Material(((GH_Colour)values[0]).Value);
        }

        protected override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            GH_ColourSwatch swatch = (GH_ColourSwatch)obj;
            if (swatch == null) return;
            if (OwnerGooData != null)
                ParamColorControl.SwatchColorInfo.SetValue(swatch, OwnerGooData.Value);
        }
    }
}
