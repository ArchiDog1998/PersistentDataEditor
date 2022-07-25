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

namespace PersistentDataEditor
{
    public class PersistentDataEditorInfo : GH_AssemblyInfo
    {
        public override string Name => "Persistent Data Editor";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => Properties.Resources.ComponentToolkitIcon_24;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "Different way to change the persistent data in persistent parameters.";

        public override Guid Id => new Guid("6CE6A8F4-539B-4F83-BF46-E02C605219AB");

        //Return a string identifying you or your company.
        public override string AuthorName => "秋水";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "1123993881@qq.com";

        public override string Version => "1.1.1";
    }

    public class PersistentDataEditorAssemblyPriority : GH_AssemblyPriority
    {
        public override GH_LoadingInstruction PriorityLoad()
        {
            Instances.CanvasCreated += Instances_CanvasCreated;
            return GH_LoadingInstruction.Proceed;
        }

        private void Instances_CanvasCreated(GH_Canvas canvas)
        {
            Instances.CanvasCreated -= Instances_CanvasCreated;

            GH_DocumentEditor editor = Instances.DocumentEditor;
            if (editor == null)
            {
                Instances.ActiveCanvas.DocumentChanged += ActiveCanvas_DocumentChanged;
                return;
            }
            DoingSomethingFirst(editor);
        }

        private void ActiveCanvas_DocumentChanged(GH_Canvas sender, GH_CanvasDocumentChangedEventArgs e)
        {
            Instances.ActiveCanvas.DocumentChanged -= ActiveCanvas_DocumentChanged;

            GH_DocumentEditor editor = Instances.DocumentEditor;
            if (editor == null)
            {
                MessageBox.Show("Persistent Data Editor can't find the menu!");
                return;
            }
            DoingSomethingFirst(editor);
        }

        private void DoingSomethingFirst(GH_DocumentEditor editor)
        {
            ToolStripMenuItem displayItem = (ToolStripMenuItem)editor.MainMenuStrip.Items[3];
            ToolStripItem[] gumballs = displayItem.DropDownItems.Find("mnuGumballs", false);

            ToolStripMenuItem gumball = null;
            if(gumballs == null || gumballs.Length == 0)
            {
                MessageBox.Show("ComponentToolkit can't find the Gumballs Item!");
            }
            else
            {
                gumball = (ToolStripMenuItem)gumballs[0];
                displayItem.DropDownItems.Remove(gumball);
            }
            displayItem.DropDownItems.Insert(3, MenuCreator.CreateMajorMenu(gumball?.Image));
            GH_ComponentAttributesReplacer.Init();

            CentralSettings.PreviewGumballs = false;
        }
    }
}