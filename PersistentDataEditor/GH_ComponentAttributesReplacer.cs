using Grasshopper;
using Grasshopper.GUI.Base;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Special;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Threading.Tasks;

namespace PersistentDataEditor;

public abstract class GH_ComponentAttributesReplacer : GH_ComponentAttributes
{
    //private static readonly MethodInfo _renderInfo = typeof(GH_FloatingParamAttributes).FindMethod("Render");

    private static readonly MethodInfo _pathMapperCreate = typeof(GH_PathMapper).FindMethod("GetInputMapping");

    protected GH_ComponentAttributesReplacer(IGH_Component component) : base(component)
    {

    }

    public static void Init()
    {
        Instances.ActiveCanvas.DocumentChanged += ActiveCanvas_DocumentChanged;
        Instances.ActiveCanvas.MouseDown += ActiveCanvas_MouseDown;

        new MoveShowConduit().Enabled = true;
    }

    private static void ActiveCanvas_DocumentChanged(GH_Canvas sender, GH_CanvasDocumentChangedEventArgs e)
    {
        if (e.OldDocument != null)
        {
            e.OldDocument.ObjectsAdded -= Document_ObjectsAdded;
            e.OldDocument.ObjectsDeleted -= Document_ObjectsDeleted;
        }

        if (e.NewDocument == null) return;

        e.NewDocument.ObjectsAdded += Document_ObjectsAdded;
        e.NewDocument.ObjectsDeleted += Document_ObjectsDeleted;
        foreach (var item in e.NewDocument.Objects)
        {
            ChangeDocumentObject(item);
        }
        e.NewDocument.DestroyAttributeCache();
    }


    #region Gumball
    private static void Document_ObjectsDeleted(object sender, GH_DocObjectEventArgs e)
    {
        foreach (var component in e.Objects)
        {
            if (component is IGH_Component ghComponent)
            {
                foreach (var item in ghComponent.Params.Input)
                {
                    if (item.Attributes is GH_AdvancedLinkParamAttr attr)
                    {
                        attr.Dispose();
                    }
                }
            }
            if (component is IGH_Param param)
            {
                if (param.Attributes is GH_AdvancedFloatingParamAttr attr)
                {
                    GH_AdvancedFloatingParamAttr floating = attr;
                    floating.Dispose();
                }
            }
        }
    }
    private static IGH_DocumentObject OnGumballComponent;
    private static void ActiveCanvas_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        PointF canvasLocation = Instances.ActiveCanvas.Viewport.UnprojectPoint(e.Location);

        if (!Instances.ActiveCanvas.IsDocument) return;
        OnGumballComponent?.Attributes.ExpireLayout();

        GH_RelevantObjectData obj = Instances.ActiveCanvas.Document.RelevantObjectAtPoint(canvasLocation, GH_RelevantObjectFilter.Attributes);
        if (obj != null)
        {
            if (OnGumballComponent != obj.TopLevelObject)
            {
                CloseGumball();
            }

            if (obj.TopLevelObject is IGH_Component gH_Component)
            {
                Task.Run(() =>
                {
                    Task.Delay(5);

                    if (!gH_Component.Attributes.Selected) return;

                    OnGumballComponent = gH_Component;
                    OnGumballComponent.Attributes.ExpireLayout();

                    foreach (var item in gH_Component.Params.Input)
                    {
                        if (item.Attributes is GH_AdvancedLinkParamAttr attr && item.SourceCount == 0)
                        {
                            attr.ShowAllGumballs();
                        }
                    }

                });

                return;
            }
            else if (obj.TopLevelObject is IGH_Param gH_Param)
            {
                if (gH_Param.SourceCount == 0)
                {
                    Task.Run(() =>
                    {
                        Task.Delay(5);

                        if (!gH_Param.Attributes.Selected) return;

                        OnGumballComponent = gH_Param;
                        OnGumballComponent.Attributes.ExpireLayout();

                        if (gH_Param.Attributes is not GH_AdvancedFloatingParamAttr attr) return;

                        attr.RedrawGumballs();

                    });
                    return;
                }
            }
        }

        CloseGumball();
    }

    private static void CloseGumball()
    {
        if (OnGumballComponent == null) return;

        if (OnGumballComponent is IGH_Component component)
        {
            foreach (var item in component.Params.Input)
            {
                if (item.Attributes is GH_AdvancedLinkParamAttr attr)
                {
                    attr.Dispose();
                }
            }
        }
        else if (((IGH_Param)OnGumballComponent).Attributes is GH_AdvancedFloatingParamAttr floatParam)
        {
            floatParam.Dispose();
        }

        OnGumballComponent = null;
    }
    #endregion

    private static void Document_ObjectsAdded(object sender, GH_DocObjectEventArgs e)
    {
        foreach (var item in e.Objects)
        {
            if (item is IGH_Component)
            {
                item.Attributes.ExpireLayout();
            }
            ChangeDocumentObject(item);
            if (item is GH_PathMapper pathMapper)
            {
                Instances.ActiveCanvas.Document.ScheduleSolution(50, (doc) =>
                {
                    if (pathMapper.Lexers.Count > 0) return;

                    List<string> inputMapping = (List<string>)_pathMapperCreate.Invoke(pathMapper, new object[] { true });
                    if (inputMapping.Count != 0)
                    {
                        foreach (string str in inputMapping)
                        {
                            pathMapper.Lexers.Add(new GH_LexerCombo(str, "{path_index}"));
                        }
                    }
                });
            }
            else if (item is GH_NumberSlider slider)
            {
                if (slider.Slider.Type == GH_SliderAccuracy.Integer)
                {
                    slider.Slider.DecimalPlaces = 0;
                    slider.Slider.Type = GH_SliderAccuracy.Float;
                }
            }
        }
        e.Document?.DestroyAttributeCache();
    }

    private static void ChangeDocumentObject(IGH_DocumentObject item)
    {
        if (item is IGH_Param o)
        {
            if (o.Kind == GH_ParamKind.floating && o.Attributes.GetType() == typeof(GH_FloatingParamAttributes))
            {
                PointF point = o.Attributes.Pivot;
                bool isSelected = o.Attributes.Selected;
                o.Attributes = new GH_AdvancedFloatingParamAttr(o)
                {
                    Pivot = point,
                    Selected = isSelected
                };
                o.Attributes.ExpireLayout();
            }
            else if (o is GH_NumberSlider slider && o.Attributes.GetType() == typeof(GH_NumberSliderAttributes))
            {
                PointF point = o.Attributes.Pivot;
                bool isSelected = o.Attributes.Selected;
                o.Attributes = new GH_AdvancedNumberSliderAttr(slider)
                {
                    Pivot = point,
                    Selected = isSelected,
                };
                o.Attributes.ExpireLayout();
            }
        }
        else if (item is IGH_Component component && component.Attributes.GetType() == typeof(GH_ComponentAttributes))
        {
            PointF point = component.Attributes.Pivot;
            bool isSelected = component.Attributes.Selected;
            component.Attributes = new GH_AdvancedComponentAttr(component)
            {
                Pivot = point,
                Selected = isSelected,
            };
            component.Attributes.ExpireLayout();
        }
    }
}
