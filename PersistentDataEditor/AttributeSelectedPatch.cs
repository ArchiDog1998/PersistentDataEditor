using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using HarmonyLib;
using System.Drawing;
using System.Reflection;

namespace PersistentDataEditor;

[HarmonyPatch(typeof(GH_Canvas))]
internal class AttributeSelectedPatch
{
    private static readonly FieldInfo _innerBox = AccessTools.Field(typeof(GH_ComponentAttributes), "m_innerBounds");

    private static IGH_Attributes _activeAttribute = null;

    [HarmonyPatch("MouseDown_InactiveObject")]
    private static void Prefix(GH_Canvas __instance, GH_CanvasMouseEvent e)
    {
        if (!Data.OnlyShowSelectedObjectControl) return;

        _activeAttribute?.ExpireLayout();
        _activeAttribute = __instance.Document.FindAttribute(e.CanvasLocation, topLevelOnly: false);
        _activeAttribute?.ExpireLayout();
    }

    [HarmonyPatch("DoubleClick_InactiveObject")]
    static bool Prefix(GH_Canvas __instance, ref bool __result, GH_CanvasMouseEvent e)
    {
        if (!__instance.ModifiersEnabled)
        {
            return __result = false;
        }
        if (!__instance.IsDocument)
        {
            return __result = false;
        }
        IGH_Attributes iGH_Attributes = __instance.Document.FindAttribute(e.CanvasLocation, topLevelOnly: false);
        if (iGH_Attributes == null)
        {
            return __result = false;
        }

        if (iGH_Attributes is GH_ComponentAttributes attr
            && ((RectangleF)_innerBox.GetValue(attr)).Contains(e.CanvasLocation))
        {
            ComponentAttributePatch.SetSimplify(attr.Owner, !ComponentAttributePatch.IsSimplify(attr.Owner));
            attr.ExpireLayout();
            __instance.Refresh();

            __result = true;
            return false;
        }

        return true;
    }
}
