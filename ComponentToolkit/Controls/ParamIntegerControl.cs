using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class ParamIntegerControl : ParamControlBase<GH_Integer>
    {
        protected override Guid AddCompnentGuid => new Guid("57da07bd-ecab-415d-9d86-af36d7073abc");

        protected override bool Valid => base.Valid && Datas.UseParamIntegerControl;

        private static readonly FieldInfo namedValueListInfo = typeof(Param_Integer).GetRuntimeFields().Where(m => m.Name.Contains("m_namedValues")).First();
        private static FieldInfo nameInfo = null;
        private static FieldInfo valueInfo = null;
        internal ParamIntegerControl(GH_PersistentParam<GH_Integer> owner) : base(owner)
        {
        }

        protected override BaseControlItem[] SetControlItems(GH_PersistentParam<GH_Integer> owner)
        {
            if (owner is Param_Integer && ((Param_Integer)owner).HasNamedValues)
            {
                IList list = (IList)namedValueListInfo.GetValue(owner);

                SortedList<int, string> keyValues = new SortedList<int, string>();
                foreach (var item in list)
                {
                    nameInfo = nameInfo ?? item.GetType().GetRuntimeFields().Where(m => m.Name.Contains("Name")).First();
                    valueInfo = valueInfo ?? item.GetType().GetRuntimeFields().Where(m => m.Name.Contains("Value")).First();

                    keyValues[(int)valueInfo.GetValue(item)] = (string)nameInfo.GetValue(item);
                }

                return new BaseControlItem[]
                {
                    new GooEnumControl(()=> OwnerGooData, keyValues),
                };
            }
            else
            {
                return  new BaseControlItem[]
                {
                    new GooInputBoxControl<GH_Integer>(()=> OwnerGooData),
                };
            }
        }
    }
}
