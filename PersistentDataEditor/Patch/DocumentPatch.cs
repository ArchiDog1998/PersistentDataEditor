using Grasshopper.Kernel;
using HarmonyLib;
using System.Drawing;
using System.Reflection;

namespace PersistentDataEditor.Patch;

[HarmonyPatch(typeof(GH_Document))]
internal class DocumentPatch
{
    private static readonly FieldInfo _objectType = AccessTools.Field(typeof(GH_RelevantObjectData), "m_type");

    [HarmonyPatch(nameof(GH_Document.RelevantObjectAtPoint), typeof(PointF), typeof(GH_RelevantObjectFilter))]
    private static void Postfix(ref GH_RelevantObjectData __result)
    {
        if (__result == null) return;

        var obj = __result.Object.Attributes.GetTopLevel.DocObject;

        if (obj is IGH_Component component
            && ComponentAttributePatch.IsSimplify(component))
        {
            if (__result.ObjectType is GH_ObjectSpecies.grip_out or GH_ObjectSpecies.grip or GH_ObjectSpecies.grip_in)
            {
                _objectType.SetValue(__result, GH_ObjectSpecies.doc_object);
            }
        }
    }

    [HarmonyPatch(nameof(GH_Document.FindAttributeByGrip), typeof(PointF), typeof(bool), typeof(bool), typeof(bool), typeof(int))]
    private static void Postfix(ref IGH_Attributes __result)
    {
        if (__result == null) return;
        var obj = __result.GetTopLevel.DocObject;
        if (obj is IGH_Component component
            && ComponentAttributePatch.IsSimplify(component))
        {
            __result = null;
        }
    }
}
