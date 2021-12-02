using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComponentToolkit
{
    internal class GH_AdvancedFloatingParamAttr : GH_FloatingParamAttributes
    {
        public GH_AdvancedFloatingParamAttr(IGH_Param param): base(param)
        {
        }

        public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (MenuCreator.UseQuickWire && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ToolStripDropDownMenu menu = new ToolStripDropDownMenu();
                GH_DocumentObject.Menu_AppendItem(menu, "Input", (sender1, e1) =>
                {
                    GH_AdvancedLinkParamAttr.RespondToQuickWire(Owner, true).Show(sender, e.ControlLocation);
                });
                GH_DocumentObject.Menu_AppendItem(menu, "Output", (sender1, e1) =>
                {
                    GH_AdvancedLinkParamAttr.RespondToQuickWire(Owner, false).Show(sender, e.ControlLocation);
                });
                menu.Show(sender, e.ControlLocation);

                return GH_ObjectResponse.Release;
            }
            return base.RespondToMouseDoubleClick(sender, e);
        }
    }
}
