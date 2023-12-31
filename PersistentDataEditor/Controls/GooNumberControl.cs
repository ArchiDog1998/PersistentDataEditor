using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using System;

namespace PersistentDataEditor
{
    internal class GooNumberControl: GooHorizonalControlBase<GH_Number>
    {
        public override Guid AddCompnentGuid => new ("57da07bd-ecab-415d-9d86-af36d7073abc");

        protected override string AddCompnentInit => base.AddCompnentInit ?? "0..100.00";

        public GooNumberControl(Func<GH_Number> valueGetter, Func<bool> isNull, string name) : base(valueGetter, isNull, name)
        {

        }

        public override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            GH_NumberSlider slider = (GH_NumberSlider)obj;
            if (slider == null) return;

            if (slider.Slider.DecimalPlaces == 0)
            {
                slider.Slider.DecimalPlaces = 2;
            }
        }

        protected override GH_Number SetValue(IGH_Goo[] values)
        {
            return (GH_Number)values[0];
        }

        protected override BaseControlItem[] SetControlItems()
        {
            return new BaseControlItem[]
            {
                new GooInputBoxStringControl<GH_Number>(()=>
                {
                    if(ShowValue == null) return null;
                    return new GH_Number(ShowValue.Value);
                }, _isNull),
            };
        }
    }
}
