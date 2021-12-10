using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Parameters.Hints;
using Grasshopper.Kernel.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class ParamStringControl : ParamControlBase<GH_String>
    {
        public ParamStringControl(GH_PersistentParam<GH_String> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_String> SetUpControl(IGH_Param param)
        {
            return new GooInputBoxStringControl<GH_String>(() => OwnerGooData, () => IsNull);
        }
    }

    internal class ParamBooleanControl : ParamControlBase<GH_Boolean>
    {
        public ParamBooleanControl(GH_PersistentParam<GH_Boolean> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Boolean> SetUpControl(IGH_Param param)
        {
            return new GooBooleanControl(() => OwnerGooData, () => IsNull);
        }
    }

    internal class ParamIntegerControl : ParamControlBase<GH_Integer>
    {
        private static readonly FieldInfo namedValueListInfo = typeof(Param_Integer).GetRuntimeFields().Where(m => m.Name.Contains("m_namedValues")).First();
        private static FieldInfo nameInfo = null;
        private static FieldInfo valueInfo = null;

        public ParamIntegerControl(GH_PersistentParam<GH_Integer> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Integer> SetUpControl(IGH_Param param)
        {
            if (param is Param_Integer && ((Param_Integer)param).HasNamedValues)
            {

                IList list = (IList)namedValueListInfo.GetValue(param);

                SortedList<int, string> _keyValues = new SortedList<int, string>();
                foreach (var item in list)
                {
                    nameInfo = nameInfo ?? item.GetType().GetRuntimeFields().Where(m => m.Name.Contains("Name")).First();
                    valueInfo = valueInfo ?? item.GetType().GetRuntimeFields().Where(m => m.Name.Contains("Value")).First();

                    _keyValues[(int)valueInfo.GetValue(item)] = (string)nameInfo.GetValue(item);
                }
                return new GooEnumControl(() => OwnerGooData, () => IsNull, _keyValues);
            }
            else
            {
                return new GooIntegerControl(() => OwnerGooData, () => IsNull, null);
            }
            
        }
    }

    internal class ParamNumberControl : ParamControlBase<GH_Number>
    {
        public ParamNumberControl(GH_PersistentParam<GH_Number> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Number> SetUpControl(IGH_Param param)
        {
            return new GooNumberControl(() => OwnerGooData, () => IsNull, null);
        }
    }


    internal class ParamColourControl : ParamControlBase<GH_Colour>
    {
        public ParamColourControl(GH_PersistentParam<GH_Colour> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Colour> SetUpControl(IGH_Param param)
        {
            return new GooColorControl(() => OwnerGooData, () => IsNull);
        }
    }

    internal class ParamMaterialControl : ParamControlBase<GH_Material>
    {
        public ParamMaterialControl(GH_PersistentParam<GH_Material> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Material> SetUpControl(IGH_Param param)
        {
            return new GooMaterialControl(() => OwnerGooData, () => IsNull);
        }
    }

    internal class ParamIntervalControl : ParamControlBase<GH_Interval>
    {
        public ParamIntervalControl(GH_PersistentParam<GH_Interval> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Interval> SetUpControl(IGH_Param param)
        {
            return new GooIntervalControl(() => OwnerGooData, () => IsNull, null);
        }
    }

    internal class ParamPointControl : ParamControlBase<GH_Point>
    {
        public ParamPointControl(GH_PersistentParam<GH_Point> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Point> SetUpControl(IGH_Param param)
        {
            return new GooPointControl(() => OwnerGooData, () => IsNull, null);
        }
    }

    internal class ParamVectorControl : ParamControlBase<GH_Vector>
    {
        public ParamVectorControl(GH_PersistentParam<GH_Vector> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Vector> SetUpControl(IGH_Param param)
        {
            return new GooVectorControl(() => OwnerGooData, () => IsNull, null);
        }
    }

    internal class ParamComplexControl : ParamControlBase<GH_ComplexNumber>
    {
        public ParamComplexControl(GH_PersistentParam<GH_ComplexNumber> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_ComplexNumber> SetUpControl(IGH_Param param)
        {
            return new GooComplexControl(() => OwnerGooData, () => IsNull, null);
        }
    }

    internal class ParamInterval2DControl : ParamControlBase<GH_Interval2D>
    {
        public ParamInterval2DControl(GH_PersistentParam<GH_Interval2D> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Interval2D> SetUpControl(IGH_Param param)
        {
            return new GooInterval2DControl(() => OwnerGooData, () => IsNull, null);
        }
    }

    internal class ParamLineControl : ParamControlBase<GH_Line>
    {
        public ParamLineControl(GH_PersistentParam<GH_Line> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Line> SetUpControl(IGH_Param param)
        {
            return new GooLineControl(() => OwnerGooData, () => IsNull, null);
        }
    }

    internal class ParamPlaneControl : ParamControlBase<GH_Plane>
    {
        public ParamPlaneControl(GH_PersistentParam<GH_Plane> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Plane> SetUpControl(IGH_Param param)
        {
            return new GooPlaneControl(() => OwnerGooData, () => IsNull, null);
        }
    }

    internal class ParamCircleControl : ParamControlBase<GH_Circle>
    {
        public ParamCircleControl(GH_PersistentParam<GH_Circle> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Circle> SetUpControl(IGH_Param param)
        {
            return new GooCircleControl(() => OwnerGooData, () => IsNull, null);
        }
    }

    internal class ParamRectangleControl : ParamControlBase<GH_Rectangle>
    {
        public ParamRectangleControl(GH_PersistentParam<GH_Rectangle> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Rectangle> SetUpControl(IGH_Param param)
        {
            return new GooRectangleControl(() => OwnerGooData, () => IsNull, null);
        }
    }

    internal class ParamBoxControl : ParamControlBase<GH_Box>
    {
        public ParamBoxControl(GH_PersistentParam<GH_Box> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Box> SetUpControl(IGH_Param param)
        {
            return new GooBoxControl(() => OwnerGooData, () => IsNull, null);
        }
    }

    internal class ParamArcControl : ParamControlBase<GH_Arc>
    {
        public ParamArcControl(GH_PersistentParam<GH_Arc> owner) : base(owner)
        {

        }

        protected override GooControlBase<GH_Arc> SetUpControl(IGH_Param param)
        {
            return new GooArcControl(() => OwnerGooData, () => IsNull, null);
        }
    }

    internal class ParamGeneralControl<T> : ParamControlBase<T> where T : class, IGH_Goo
    {
        public ParamGeneralControl(GH_PersistentParam<T> owner) : base(owner)
        {

        }

        protected override GooControlBase<T> SetUpControl(IGH_Param param)
        {
            return new GooGeneralControl<T>(() => OwnerGooData, () => IsNull);
        }
    }

    internal class ParamVariableControl : ParamGeneralControl<IGH_Goo>
    {
        protected override bool Valid => base.Valid && Owner.Access == GH_ParamAccess.item;

        public ParamVariableControl(Param_ScriptVariable owner) : base(owner)
        {

        }

        protected override GooControlBase<IGH_Goo> SetUpControl(IGH_Param param)
        {
            return new GooVariableControl(() => OwnerGooData, () => IsNull, (Param_ScriptVariable)param);
        }
    }
}
