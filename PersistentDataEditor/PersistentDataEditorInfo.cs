using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace PersistentDataEditor;

public class PersistentDataEditorInfo : GH_AssemblyInfo
{
    public override string Name => "Persistent Data Editor";

    //Return a 24x24 pixel bitmap to represent this GHA library.
    public override Bitmap Icon => Properties.Resources.ComponentToolkitIcon_24;

    //Return a short string describing the purpose of this GHA library.
    public override string Description => "Different way to change the persistent data in persistent parameters.";

    public override Guid Id => new("6CE6A8F4-539B-4F83-BF46-E02C605219AB");

    //Return a string identifying you or your company.
    public override string AuthorName => "秋水";

    //Return a string representing your preferred contact details.
    public override string AuthorContact => "1123993881@qq.com";

    public override string Version => "1.1.4";
}

partial class SimpleAssemblyPriority
{
    protected override void DoWithEditor(GH_DocumentEditor editor)
    {
        ToolStripMenuItem displayItem = (ToolStripMenuItem)editor.MainMenuStrip.Items[3];
        ToolStripItem[] gumballs = displayItem.DropDownItems.Find("mnuGumballs", false);

        ToolStripMenuItem gumball = null;
        if (gumballs.Length == 0)
        {
            MessageBox.Show("ComponentToolkit can't find the Gumballs Item!");
        }
        else
        {
            gumball = (ToolStripMenuItem)gumballs[0];
            displayItem.DropDownItems.Remove(gumball);
        }

        GH_ComponentAttributesReplacer.Init();

        CentralSettings.PreviewGumballs = false;
        base.DoWithEditor(editor);
    }
}

public static class PD
{
    public static FieldInfo FindField(this Type type, string name)
        => type.GetRuntimeFields().First(m => m.Name.Contains(name));
    public static MethodInfo FindMethod(this Type type, string name)
        => type.GetRuntimeMethods().First(m => m.Name.Contains(name));
}