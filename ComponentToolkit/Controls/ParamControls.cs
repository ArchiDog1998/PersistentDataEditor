using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class ParamStringControl : ParamControl<GH_String>
    {
        public ParamStringControl(GH_PersistentParam<GH_String> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_String> SetUpControl(IGH_Param param)
        {
            return new GooInputBoxStringControl<GH_String>(() => OwnerGooData);
        }
    }

    internal class ParamBooleanControl : ParamControl<GH_Boolean>
    {
        public ParamBooleanControl(GH_PersistentParam<GH_Boolean> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Boolean> SetUpControl(IGH_Param param)
        {
            return new GooBooleanControl(() => OwnerGooData);
        }
    }

    internal class ParamIntegerControl : ParamControl<GH_Integer>
    {
        public ParamIntegerControl(GH_PersistentParam<GH_Integer> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Integer> SetUpControl(IGH_Param param)
        {
            return new GooIntegerControl(() => OwnerGooData, null);
        }
    }

    internal class ParamNumberControl : ParamControl<GH_Number>
    {
        public ParamNumberControl(GH_PersistentParam<GH_Number> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Number> SetUpControl(IGH_Param param)
        {
            return new GooNumberControl(() => OwnerGooData, null);
        }
    }


    internal class ParamColourControl : ParamControl<GH_Colour>
    {
        public ParamColourControl(GH_PersistentParam<GH_Colour> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Colour> SetUpControl(IGH_Param param)
        {
            return new GooColorControl(() => OwnerGooData);
        }
    }

    internal class ParamMaterialControl : ParamControl<GH_Material>
    {
        public ParamMaterialControl(GH_PersistentParam<GH_Material> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Material> SetUpControl(IGH_Param param)
        {
            return new GooMaterialControl(() => OwnerGooData);
        }
    }

    internal class ParamIntervalControl : ParamControl<GH_Interval>
    {
        public ParamIntervalControl(GH_PersistentParam<GH_Interval> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Interval> SetUpControl(IGH_Param param)
        {
            return new GooIntervalControl(() => OwnerGooData, null);
        }
    }

    internal class ParamPointControl : ParamControl<GH_Point>
    {
        public ParamPointControl(GH_PersistentParam<GH_Point> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Point> SetUpControl(IGH_Param param)
        {
            return new GooPointControl(() => OwnerGooData, null);
        }
    }

    internal class ParamVectorControl : ParamControl<GH_Vector>
    {
        public ParamVectorControl(GH_PersistentParam<GH_Vector> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Vector> SetUpControl(IGH_Param param)
        {
            return new GooVectorControl(() => OwnerGooData, null);
        }
    }

    internal class ParamComplexControl : ParamControl<GH_ComplexNumber>
    {
        public ParamComplexControl(GH_PersistentParam<GH_ComplexNumber> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_ComplexNumber> SetUpControl(IGH_Param param)
        {
            return new GooComplexControl(() => OwnerGooData, null);
        }
    }

    internal class ParamInterval2DControl : ParamControl<GH_Interval2D>
    {
        public ParamInterval2DControl(GH_PersistentParam<GH_Interval2D> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Interval2D> SetUpControl(IGH_Param param)
        {
            return new GooInterval2DControl(() => OwnerGooData, null);
        }
    }

    internal class ParamLineControl : ParamControl<GH_Line>
    {
        public ParamLineControl(GH_PersistentParam<GH_Line> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Line> SetUpControl(IGH_Param param)
        {
            return new GooLineControl(() => OwnerGooData, null);
        }
    }

    internal class ParamPlaneControl : ParamControl<GH_Plane>
    {
        public ParamPlaneControl(GH_PersistentParam<GH_Plane> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Plane> SetUpControl(IGH_Param param)
        {
            return new GooPlaneControl(() => OwnerGooData, null);
        }
    }

    internal class ParamCircleControl : ParamControl<GH_Circle>
    {
        public ParamCircleControl(GH_PersistentParam<GH_Circle> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Circle> SetUpControl(IGH_Param param)
        {
            return new GooCircleControl(() => OwnerGooData, null);
        }
    }

    internal class ParamGeneralControl<T> : ParamControl<T> where T : class, IGH_Goo
    {
        protected override bool Valid
        {
            get
            {
                bool isActive = Owner.OnPingDocument() == Instances.ActiveCanvas.Document && Owner.SourceCount == 0 && Owner.PersistentDataCount < 2;
                bool isUse = Datas.UseParamControl && (Owner.Attributes.IsTopLevel ? Datas.ParamUseControl : Datas.ComponentUseControl);
                string saveBooleanKey = "UseParam" + typeof(IGH_Goo).Name;
                bool useParam = Instances.Settings.GetValue(saveBooleanKey, true);
                return isActive && isUse && useParam;
            }
        }

        public ParamGeneralControl(GH_PersistentParam<T> owner) : base(owner)
        {

        }

        protected override GooControlBase<T> SetUpControl(IGH_Param param)
        {
            return new GooGeneralControl<T>(() => OwnerGooData);
        }
    }
}
