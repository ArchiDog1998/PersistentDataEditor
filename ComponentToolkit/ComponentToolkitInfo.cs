using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace ComponentToolkit
{
    public class ComponentToolkitInfo : GH_AssemblyInfo
    {
        public override string Name => "ComponentToolkit";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => Properties.Resources.ComponentToolkitIcon_24;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "Extra information and control for grasshopper component.";

        public override Guid Id => new Guid("6CE6A8F4-539B-4F83-BF46-E02C605219AB");

        //Return a string identifying you or your company.
        public override string AuthorName => "秋水";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "1123993881@qq.com";

        public override string Version => "1.0.0";
    }

    public class ComponentToolkitAssemblyPriority : GH_AssemblyPriority
    {
        public override GH_LoadingInstruction PriorityLoad()
        {
            Grasshopper.Instances.CanvasCreated += Instances_CanvasCreated;
            return GH_LoadingInstruction.Proceed;
        }

        private void Instances_CanvasCreated(GH_Canvas canvas)
        {
            Grasshopper.Instances.CanvasCreated -= Instances_CanvasCreated;

            GH_DocumentEditor editor = Grasshopper.Instances.DocumentEditor;
            if (editor == null)
            {
                Grasshopper.Instances.ActiveCanvas.DocumentChanged += ActiveCanvas_DocumentChanged;
                return;
            }
            DoingSomethingFirst(editor);
        }

        private void ActiveCanvas_DocumentChanged(GH_Canvas sender, GH_CanvasDocumentChangedEventArgs e)
        {
            Grasshopper.Instances.ActiveCanvas.DocumentChanged -= ActiveCanvas_DocumentChanged;

            GH_DocumentEditor editor = Grasshopper.Instances.DocumentEditor;
            if (editor == null)
            {
                MessageBox.Show("ComponentToolkit can't find the menu!");
                return;
            }
            DoingSomethingFirst(editor);
        }

        private void DoingSomethingFirst(GH_DocumentEditor editor)
        {
            ToolStripMenuItem displayItem = (ToolStripMenuItem)editor.MainMenuStrip.Items[3];
            displayItem.DropDownItems.Insert(3, MenuCreator.CreateMajorMenu());
            GH_ComponentAttributesReplacer.Init();
        }
    }
}