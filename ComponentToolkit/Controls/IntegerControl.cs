using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    public class IntegerControl : InputBoxControl<GH_Integer>
    {
        private static readonly FieldInfo namedValueListInfo = typeof(Param_Integer).GetRuntimeFields().Where(m => m.Name.Contains("m_namedValues")).First();

        public override string ShowString
        {
            get
            {
                int? value = Owner.PersistentData.get_FirstItem(false)?.Value;
                if (isNamed && value.HasValue)
                {
                    if (keyValues.ContainsKey(value.Value))
                    {
                        return keyValues[value.Value];
                    }
                    else
                    {
                        return value.ToString();
                    }
                }
                else
                {
                    return value.ToString();
                }
            }
        }

        public Dictionary<int, string> keyValues = new Dictionary<int, string>();

        public bool isNamed;

        public IntegerControl(GH_PersistentParam<GH_Integer> owner) : base(owner)
        {
            isNamed = owner is Param_Integer && ((Param_Integer)owner).HasNamedValues;

            if (isNamed)
            {
                IList list = (IList)namedValueListInfo.GetValue(owner);

                FieldInfo nameInfo = null;
                FieldInfo valueInfo = null;
                keyValues.Clear();
                foreach (var item in list)
                {
                    nameInfo = nameInfo ?? item.GetType().GetRuntimeFields().Where(m => m.Name.Contains("Name")).First();
                    valueInfo = valueInfo ?? item.GetType().GetRuntimeFields().Where(m => m.Name.Contains("Value")).First();
                    keyValues[(int)valueInfo.GetValue(item)] = (string)nameInfo.GetValue(item);
                }
            }
        }

        public override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (isNamed)
            {
                int? num = Owner.PersistentData.get_FirstItem(false)?.Value;
                ContextMenuStrip menu = new ContextMenuStrip() { ShowImageMargin = true };
                foreach (var namedValue in keyValues)
                {
                    GH_DocumentObject.Menu_AppendItem(menu, namedValue.Value, Menu_NamedValueClicked, true, namedValue.Key == num).Tag = namedValue.Key;
                }
                menu.Show(sender, e.ControlLocation);
            }
            else
            {
                base.Clicked(sender, e);
            }

        }
        private void Menu_NamedValueClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
            if (toolStripMenuItem != null && toolStripMenuItem.Tag != null && toolStripMenuItem.Tag is int)
            {
                int value = (int)toolStripMenuItem.Tag;
                Owner.RecordUndoEvent("Set: " + value);
                Owner.PersistentData.Clear();
                Owner.PersistentData.Append(new GH_Integer(value));
                Owner.ExpireSolution(true);
                Grasshopper.Instances.ActiveCanvas.Refresh();
            }
        }


        protected override void Save(string str)
        {
            if (int.TryParse(str, out int number))
            {
                Owner.RecordUndoEvent("Set: " + str);
                Owner.PersistentData.Clear();
                Owner.PersistentData.Append(new GH_Integer(number));
            }

        }
    }
}
