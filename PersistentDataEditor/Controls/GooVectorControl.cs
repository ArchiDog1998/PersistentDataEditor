using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;

namespace PersistentDataEditor
{
    internal class GooVectorControl : GooHorizonalControlBase<GH_Vector>
    {
        public override Guid AddCompnentGuid => new Guid("56b92eab-d121-43f7-94d3-6cd8f0ddead8");

        public GooVectorControl(Func<GH_Vector> valueGetter, Func<bool> isNull, string name) : base(valueGetter, isNull, name)
        {

        }

        protected override BaseControlItem[] SetControlItems()
        {
            switch ((Vector_Control)Instances.Settings.GetValue(typeof(Vector_Control).FullName, 0))
            {
                default:
                    return new BaseControlItem[]
                    {
                        new GooInputBoxStringControl<GH_Vector>(()=> ShowValue, _isNull),
                    };
                case Vector_Control.XYZ:
                    return new BaseControlItem[]
                    {
                        new StringRender("X"),

                        new GooInputBoxStringControl<GH_Number>(()=> ShowValue == null ? null : new GH_Number(ShowValue.Value.X), _isNull),

                        new StringRender("Y"),

                        new GooInputBoxStringControl<GH_Number>(()=> ShowValue == null ? null : new GH_Number(ShowValue.Value.Y), _isNull),

                        new StringRender("Z"),

                        new GooInputBoxStringControl<GH_Number>(()=> ShowValue == null ? null : new GH_Number(ShowValue.Value.Z), _isNull),
                    };
            }
        }

        private protected override GH_Vector CreateDefaultValue() 
            => new GH_Vector(Vector3d.ZAxis);

        protected override GH_Vector SetValue(IGH_Goo[] values)
        {
            switch ((Vector_Control)Instances.Settings.GetValue(typeof(Vector_Control).FullName, 0))
            {
                default:
                    return (GH_Vector)values[0];

                case Vector_Control.XYZ:
                    return new GH_Vector(new Vector3d(
                        ((GH_Number)values[0]).Value,
                        ((GH_Number)values[1]).Value,
                        ((GH_Number)values[2]).Value));
            }
        }


        public override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            if (obj == null) return;
            GH_Component com = (GH_Component)obj;

            if (com.Params.Input.Count < 3) return;

            if (com.Params.Input[0] is Param_Number param0 && _values[0].SaveValue is GH_Number Value0)
            {
                param0.PersistentData.Clear();
                param0.PersistentData.Append(Value0);
            }

            if (com.Params.Input[1] is Param_Number param1 && _values[1].SaveValue is GH_Number Value1)
            {
                param1.PersistentData.Clear();
                param1.PersistentData.Append(Value1);
            }

            if (com.Params.Input[2] is Param_Number param2 && _values[2].SaveValue is GH_Number Value2)
            {
                param2.PersistentData.Clear();
                param2.PersistentData.Append(Value2);
            }
        }
    }
}
