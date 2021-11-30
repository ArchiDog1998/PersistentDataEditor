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
    internal class ParamVector3dControl : ParamControlBase<GH_Vector>
    {
        private GH_Number X, Y, Z;

        public ParamVector3dControl(GH_PersistentParam<GH_Vector> owner) : base(owner)
        {
            ControlItems = new BaseControlItem[]
            {
                new StringRender("X:"),

                new GooNumberControl(()=> {
                    GH_Vector vector = Owner.PersistentData.get_FirstItem(true);
                    if(vector == null) return null;
                    X = new GH_Number(vector.Value.X);
                    return X;
                }, (number, record) =>
                {
                    X = number;
                    CreateValue(record);
                }),

                new StringRender("Y:"),

                new GooNumberControl(()=> {
                    GH_Vector vector = Owner.PersistentData.get_FirstItem(true);
                    if(vector == null) return null;
                    Y =  new GH_Number(vector.Value.Y);
                    return Y;
                }, (number, record) =>
                {
                    Y = number;
                    CreateValue(record);
                }),

                new StringRender("Z:"),

                new GooNumberControl(()=> {
                    GH_Vector vector = Owner.PersistentData.get_FirstItem(true);
                    if(vector == null) return null;
                    Z = new GH_Number(vector.Value.Z);
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

            GH_Vector value = new GH_Vector(new Vector3d(X.Value, Y.Value, Z.Value));

            SetValue(value, isRecord);
        }
    }
}
