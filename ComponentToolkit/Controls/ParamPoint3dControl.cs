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
    internal class ParamPoint3dControl : ParamControlBase<GH_Point>
    {
        protected override bool Valid => base.Valid && GH_ComponentAttributesReplacer.UseParamPointControl;

        private GH_Number X, Y, Z;

        public ParamPoint3dControl(GH_PersistentParam<GH_Point> owner) : base(owner)
        {
            ControlItems = new BaseControlItem[]
            {
                new StringRender("X:"),

                new GooNumberControl(()=> {
                    GH_Point point = Owner.PersistentData.get_FirstItem(true);
                    if(point == null) return null;
                    X = new GH_Number(point.Value.X);
                    return X;
                }, (number, record) =>
                {
                    X = number;
                    CreateValue(record);
                }),

                new StringRender("Y:"),

                new GooNumberControl(()=> {
                    GH_Point point = Owner.PersistentData.get_FirstItem(true);
                    if(point == null) return null;
                    Y = new GH_Number(point.Value.Y);
                    return Y;
                }, (number, record) =>
                {
                    Y = number;
                    CreateValue(record);
                }),

                new StringRender("Z:"),

                new GooNumberControl(()=> {
                    GH_Point point = Owner.PersistentData.get_FirstItem(true);
                    if(point == null) return null;
                    Z = new GH_Number(point.Value.Z);
                    return Z;
                }, (number, record) =>
                {
                    Z = number;
                    CreateValue(record);
                }),
            };
        }

        private void CreateValue(bool isRecord)
        {
            Owner.Attributes.ExpireLayout();

            if (Z == null || Y == null || Z == null) return;

            GH_Point value = new GH_Point(new Point3d(X.Value, Y.Value, Z.Value));

            SetValue(value, isRecord);
        }
    }
}
