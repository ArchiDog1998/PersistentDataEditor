using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
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
        protected override Guid AddCompnentGuid => isValuelist? new Guid("00027467-0D24-4fa7-B178-8DC0AC5F42EC") : new Guid("57da07bd-ecab-415d-9d86-af36d7073abc");

        protected override bool Valid => base.Valid && Datas.UseParamIntegerControl;
        protected override string AddCompnentInit => base.AddCompnentInit ?? "0..100";

        private static readonly FieldInfo namedValueListInfo = typeof(Param_Integer).GetRuntimeFields().Where(m => m.Name.Contains("m_namedValues")).First();
        private static FieldInfo nameInfo = null;
        private static FieldInfo valueInfo = null;

        private bool isValuelist = false;
        private SortedList<int, string> _keyValues;

        internal ParamIntegerControl(GH_PersistentParam<GH_Integer> owner) : base(owner)
        {
        }

        protected override BaseControlItem[] SetControlItems(GH_PersistentParam<GH_Integer> owner)
        {
            if (owner is Param_Integer && ((Param_Integer)owner).HasNamedValues)
            {
                isValuelist = true;

                IList list = (IList)namedValueListInfo.GetValue(owner);

                _keyValues = new SortedList<int, string>();
                foreach (var item in list)
                {
                    nameInfo = nameInfo ?? item.GetType().GetRuntimeFields().Where(m => m.Name.Contains("Name")).First();
                    valueInfo = valueInfo ?? item.GetType().GetRuntimeFields().Where(m => m.Name.Contains("Value")).First();

                    _keyValues[(int)valueInfo.GetValue(item)] = (string)nameInfo.GetValue(item);
                }

                return new BaseControlItem[]
                {
                    new GooEnumControl(()=> OwnerGooData, _keyValues),
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

        protected override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            if (!isValuelist || _keyValues == null) return;

            GH_ValueList valuelist = (GH_ValueList)obj;
            if (valuelist == null) return;

            valuelist.ListItems.Clear();
            foreach (var keyvalue in _keyValues)
            {
                valuelist.ListItems.Add(new GH_ValueListItem(keyvalue.Value, keyvalue.Key.ToString()));
            }
        }
    }
}
