using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ComponentToolkit
{
    public class StringControl : InputBoxControl<GH_String>
    {
        public override string ShowString => Owner.PersistentData.get_FirstItem(false)?.Value.ToString();

        public StringControl(GH_PersistentParam<GH_String> owner):base(owner)
        {
        }

        protected override void Save(string str)
        {
            Owner.RecordUndoEvent("Set: " + str);
            Owner.PersistentData.Clear();
            Owner.PersistentData.Append(new GH_String(str));
        }
    }
}
