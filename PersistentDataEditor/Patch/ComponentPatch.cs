using Grasshopper.Kernel;
using HarmonyLib;
using Grasshopper;
using System.Windows.Forms;
using Grasshopper.My.Resources;

namespace PersistentDataEditor.Patch;

[HarmonyPatch(typeof(GH_Component))]

internal class ComponentPatch
{
    [HarmonyPatch(nameof(GH_Component.AppendAdditionalMenuItems))]
    private static void Prefix(GH_Component __instance, ToolStripDropDown menu)
    {
        GH_DocumentObject.Menu_AppendSeparator(menu);

        GH_DocumentObject.Menu_AppendItem(menu, "Simplify Component", (s, e) =>
        {
            ComponentAttributePatch.SetSimplify(__instance, !ComponentAttributePatch.IsSimplify(__instance));
            __instance.Attributes.ExpireLayout();
            Instances.ActiveCanvas.Refresh();
        }, Res_ContextMenu.Modifier_Simplify_16x16, true, ComponentAttributePatch.IsSimplify(__instance));
    }
}
