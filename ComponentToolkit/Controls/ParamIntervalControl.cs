using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    internal class ParamIntervalControl : ParamControlBase<GH_Interval>
    {
        protected override bool Valid => base.Valid && GH_ComponentAttributesReplacer.UseParamIntervalControl;

        private GH_Number T0, T1;
        public ParamIntervalControl(GH_PersistentParam<GH_Interval> owner) : base(owner)
        {
            ControlItems = new BaseControlItem[]
            {
                new GooNumberControl(()=> {
                    GH_Interval interval = Owner.PersistentData.get_FirstItem(true);
                    if(interval == null) return null;
                    T0 = new GH_Number(interval.Value.T0);
                    return T0;
                }, (number, record) =>
                {
                    T0 = number;
                    CreateValue(record);
                }),

                new StringRender("To"),

                new GooNumberControl(()=> {
                    GH_Interval interval = Owner.PersistentData.get_FirstItem(true);
                    if(interval == null) return null;
                    T1 = new GH_Number(interval.Value.T1);
                    return T1;
                }, (number, record) =>
                {
                    T1 = number;
                    CreateValue(record);
                }),
            };
        }

        private void CreateValue(bool isRecord)
        {
            Owner.Attributes.ExpireLayout();

            if (T0 == null || T1 == null) return;

            GH_Interval value = new GH_Interval(new Interval(T0.Value, T1.Value));
            
            SetValue(value, isRecord);
        }
    }
}
