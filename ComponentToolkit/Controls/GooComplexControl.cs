using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class GooComplexControl : GooHorizonalControlBase<GH_ComplexNumber>
    {
        public override Guid AddCompnentGuid => new Guid("63d12974-2915-4ccf-ac26-5d566c3bac92");

        public GooComplexControl(Func<GH_ComplexNumber> valueGetter, Func<bool> isNull, string name) : base(valueGetter, isNull, name)
        {

        }

        protected override BaseControlItem[] SetControlItems()
        {
            switch ((Complex_Control)Instances.Settings.GetValue(typeof(Complex_Control).FullName, 0))
            {
                default:
                    return new BaseControlItem[]
                    {
                        new GooInputBoxStringControl<GH_ComplexNumber>(()=> ShowValue, _isNull),
                    };
                case Complex_Control.Real_Imaginary:
                    return new BaseControlItem[]
                    {
                        new StringRender("R"),

                        new GooInputBoxStringControl<GH_Number>(()=>
                        {
                            if(ShowValue == null) return null;
                            return new GH_Number(ShowValue.Value.Real);
                        }, _isNull),

                        new StringRender("i"),

                        new GooInputBoxStringControl<GH_Number>(()=>
                        {
                            if(ShowValue == null) return null;
                            return new GH_Number(ShowValue.Value.Imaginary);
                        }, _isNull),
                    };
            }
        }

        protected override GH_ComplexNumber SetValue(IGH_Goo[] values)
        {
            switch ((Complex_Control)Instances.Settings.GetValue(typeof(Complex_Control).FullName, 0))
            {
                default:
                    return (GH_ComplexNumber)values[0];
                case Complex_Control.Real_Imaginary:
                    return new GH_ComplexNumber(new Complex(((GH_Number)values[0]).Value, ((GH_Number)values[1]).Value));
            }
        }

        public override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            if (obj == null) return;
            GH_Component com = (GH_Component)obj;
            if (com == null) return;

            if (com.Params.Input.Count < 2) return;

            if (com.Params.Input[0] is Param_Number)
            {
                Param_Number param = (Param_Number)com.Params.Input[0];
                GH_Number number = ((GooInputBoxStringControl<GH_Number>)_values[0])._savedValue;
                if (number != null) 
                {
                    param.PersistentData.Clear();
                    param.PersistentData.Append(number);
                } 
            }

            if (com.Params.Input[1] is Param_Number)
            {
                Param_Number param = (Param_Number)com.Params.Input[1];
                GH_Number number = ((GooInputBoxStringControl<GH_Number>)_values[1])._savedValue;
                if (number != null)
                {
                    param.PersistentData.Clear();
                    param.PersistentData.Append(number);
                }
            }
        }
    }
}
