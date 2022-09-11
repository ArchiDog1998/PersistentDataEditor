using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistentDataEditor
{ 
    internal class GooIntegerControl : GooHorizonalControlBase<GH_Integer>
    {
        public override Guid AddCompnentGuid => new Guid("57da07bd-ecab-415d-9d86-af36d7073abc");

        protected override string AddCompnentInit => base.AddCompnentInit ?? "0..100";

        public GooIntegerControl(Func<GH_Integer> valueGetter, Func<bool> isNull, string name) : base(valueGetter, isNull, name)
        {

        }

        protected override GH_Integer SetValue(IGH_Goo[] values)
        {
            return (GH_Integer)values[0];
        }

        protected override BaseControlItem[] SetControlItems()
        {
            return new BaseControlItem[]
            {
                new GooInputBoxStringControl<GH_Integer>(()=>
                {
                    if(ShowValue == null) return null;
                    return new GH_Integer(ShowValue.Value);
                }, _isNull),
            };
        }
    }
}
