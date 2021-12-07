using Grasshopper;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class GooGeneralControl<T> : GooInputBoxStringControl<T> where T : class, IGH_Goo
    {
        private General_Control type => (General_Control)Instances.Settings.GetValue(typeof(General_Control).FullName, 0);


        public override Guid AddCompnentGuid => default(Guid);

        protected override bool IsReadOnly => type == General_Control.ReadOnly;

        public GooGeneralControl(Func<T> valueGetter, Func<bool> isNull) : base(valueGetter, isNull)
        {

        }
    }
}
