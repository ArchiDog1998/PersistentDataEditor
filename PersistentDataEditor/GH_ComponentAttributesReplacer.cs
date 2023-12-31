using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Base;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Special;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PersistentDataEditor;

public abstract class GH_ComponentAttributesReplacer : GH_ComponentAttributes
{
    private static readonly MethodInfo _renderInfo = typeof(GH_FloatingParamAttributes).FindMethod("Render");

    private static readonly FieldInfo _tagsInfo = typeof(GH_LinkedParamAttributes).FindField("m_renderTags");
    private static readonly MethodInfo _pathMapperCreate = typeof(GH_PathMapper).FindMethod("GetInputMapping");

    protected GH_ComponentAttributesReplacer(IGH_Component component) : base(component)
    {

    }

    public static void Init()
    {
        Instances.ActiveCanvas.DocumentChanged += ActiveCanvas_DocumentChanged;
        Instances.ActiveCanvas.MouseDown += ActiveCanvas_MouseDown;

        new MoveShowConduit().Enabled = true;

        ExchangeMethod(
            typeof(GH_ComponentAttributes).FindMethod(nameof(RenderComponentParameters)),
            typeof(GH_ComponentAttributesReplacer).FindMethod(nameof(RenderComponentParametersNew))
        );

        ExchangeMethod(
            typeof(GH_ComponentAttributes).FindMethod(nameof(LayoutComponentBox)),
            typeof(GH_ComponentAttributesReplacer).FindMethod(nameof(LayoutComponentBoxNew))
        );

        ExchangeMethod(
            typeof(GH_ComponentAttributes).FindMethod(nameof(LayoutInputParams)),
            typeof(GH_ComponentAttributesReplacer).FindMethod(nameof(LayoutInputParamsNew))
        );

        ExchangeMethod(
            typeof(GH_ComponentAttributes).FindMethod(nameof(LayoutOutputParams)),
            typeof(GH_ComponentAttributesReplacer).FindMethod(nameof(LayoutOutputParamsNew))
        );

        ExchangeMethod(
            typeof(GH_TextBoxInputBase).FindMethod("TextOverrideLostFocus"),
            typeof(InputBoxBalloon).FindMethod("TextOverrideLostFocusNew")
        );
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
            ChangeFloatParam(item);
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
            ChangeFloatParam(item);
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

    private static void ChangeFloatParam(IGH_DocumentObject item)
    {
        if (item is IGH_Param o)
        {
            if (o.Kind == GH_ParamKind.floating && o.Attributes is GH_FloatingParamAttributes && o.Attributes is not GH_AdvancedFloatingParamAttr)
            {

                MethodInfo renderplus = o.Attributes.GetType().FindMethod("Render");

                RuntimeHelpers.PrepareMethod(_renderInfo.MethodHandle);
                RuntimeHelpers.PrepareMethod(renderplus.MethodHandle);
                unsafe
                {
                    if (_renderInfo.MethodHandle.Value.ToPointer() != renderplus.MethodHandle.Value.ToPointer()) return;
                }

                PointF point = o.Attributes.Pivot;
                bool isSelected = o.Attributes.Selected;
                o.Attributes = new GH_AdvancedFloatingParamAttr(o)
                {
                    Pivot = point,
                    Selected = isSelected
                };
                o.Attributes.ExpireLayout();
            }
            else if (o is GH_NumberSlider slider && o.Attributes is GH_NumberSliderAttributes && o.Attributes is not GH_AdvancedNumberSliderAttr)
            {
                PointF point = o.Attributes.Pivot;
                bool isSelected = o.Attributes.Selected;
                o.Attributes = new GH_AdvancedNumberSliderAttr(slider)
                {
                    Pivot = point,
                    Selected = isSelected
                };
                o.Attributes.ExpireLayout();
            }
        }
    }

    internal static bool ExchangeMethod(MethodInfo targetMethod, MethodInfo injectMethod)
    {
        if (targetMethod == null || injectMethod == null)
        {
            return false;
        }
        RuntimeHelpers.PrepareMethod(targetMethod.MethodHandle);
        RuntimeHelpers.PrepareMethod(injectMethod.MethodHandle);
        unsafe
        {
            if (IntPtr.Size == 4)
            {
                int* tar = (int*)targetMethod.MethodHandle.Value.ToPointer() + 2;
                int* inj = (int*)injectMethod.MethodHandle.Value.ToPointer() + 2;
                var relay = *tar;
                *tar = *inj;
                *inj = relay;
            }
            else
            {
                long* tar = (long*)targetMethod.MethodHandle.Value.ToPointer() + 1;
                long* inj = (long*)injectMethod.MethodHandle.Value.ToPointer() + 1;
                var relay = *tar;
                *tar = *inj;
                *inj = relay;
            }
        }
        return true;
    }

    #region Layout
    public static RectangleF LayoutComponentBoxNew(IGH_Component owner)
    {
        int inputHeight = GetParamsWholeHeight(owner.Params.Input, owner);
        int outputHeight = GetParamsWholeHeight(owner.Params.Output, owner);

        int val = Math.Max(inputHeight, outputHeight);
        val = Math.Max(val, 24);
        int num = 24;
        if (!IsIconMode(owner.IconDisplayMode))
        {
            val = Math.Max(val, GH_Convert.ToSize(GH_FontServer.MeasureString(owner.NickName, GH_FontServer.LargeAdjusted)).Width + 6);
        }
        return GH_Convert.ToRectangle(new RectangleF(owner.Attributes.Pivot.X - 0.5f * num, owner.Attributes.Pivot.Y - 0.5f * val, num, val));
    }

    private static int GetParamsWholeHeight(List<IGH_Param> gH_Params, IGH_Component owner)
    {
        int wholeHeight = 0;
        foreach (IGH_Param param in gH_Params)
        {
            if (param.Attributes == null || param.Attributes is not GH_AdvancedLinkParamAttr)
            {
                param.Attributes = new GH_AdvancedLinkParamAttr(param, owner.Attributes);

                //Refresh the attributes.
                owner.OnPingDocument()?.DestroyAttributeCache();
            }
            GH_AdvancedLinkParamAttr attr = (GH_AdvancedLinkParamAttr)param.Attributes;
            wholeHeight += attr.ParamHeight;
        }
        return wholeHeight;
    }

    private static void ParamsLayout(IGH_Component owner, RectangleF componentBox, bool isInput)
    {
        List<IGH_Param> gH_Params = isInput ? owner.Params.Input : owner.Params.Output;

        int count = gH_Params.Count;
        if (count == 0) return;

        //Get width and Init the Attributes.
        int singleParamBoxMaxWidth = 0;
        int nameMaxWidth = 0;
        int controlMaxWidth = 0;
        int heightCalculate = 0;
        int iconSpaceWidth = NewData.ShowLinkParamIcon ? NewData.ComponentParamIconSize + NewData.ComponentIconDistance : 0;
        foreach (IGH_Param param in gH_Params)
        {
            if (param.Attributes == null || param.Attributes is not GH_AdvancedLinkParamAttr)
            {
                param.Attributes = new GH_AdvancedLinkParamAttr(param, owner.Attributes);

                //Refresh the attributes.
                owner.OnPingDocument()?.DestroyAttributeCache();
            }
            GH_AdvancedLinkParamAttr attr = (GH_AdvancedLinkParamAttr)param.Attributes;

            singleParamBoxMaxWidth = Math.Max(singleParamBoxMaxWidth, attr.WholeWidth);
            nameMaxWidth = Math.Max(nameMaxWidth, attr.StringWidth);
            controlMaxWidth = Math.Max(controlMaxWidth, attr.ControlWidth);
            heightCalculate += attr.ParamHeight;
        }

        if (NewData.SeperateCalculateWidthControl)
        {
            int controlAddition = controlMaxWidth == 0 ? 0 : controlMaxWidth + NewData.ComponentControlNameDistance;
            singleParamBoxMaxWidth = Math.Max(nameMaxWidth + controlAddition + NewData.AdditionWidth + iconSpaceWidth, NewData.AdditionWidth + NewData.MiniWidth);
        }
        else singleParamBoxMaxWidth = Math.Max(singleParamBoxMaxWidth + NewData.AdditionWidth, NewData.AdditionWidth + NewData.MiniWidth);


        //Layout every param.
        float heightFloat = componentBox.Height / heightCalculate;
        float movementY = 0;
        foreach (IGH_Param param in gH_Params)
        {
            float singleParamBoxHeight = heightFloat * ((GH_AdvancedLinkParamAttr)param.Attributes).ParamHeight;

            float rectX = isInput ? componentBox.X - singleParamBoxMaxWidth : componentBox.Right + NewData.ComponentToCoreDistance;
            float rectY = componentBox.Y + movementY;
            float width = singleParamBoxMaxWidth - NewData.ComponentToCoreDistance;
            float height = singleParamBoxHeight;
            param.Attributes.Pivot = new PointF(rectX + 0.5f * singleParamBoxMaxWidth, rectY + 0.5f * singleParamBoxHeight);
            param.Attributes.Bounds = GH_Convert.ToRectangle(new RectangleF(rectX, rectY, width, height));

            movementY += singleParamBoxHeight;
        }

        //Layout tags.
        bool flag = false;
        int tagsCount = 0;
        foreach (IGH_Param param in gH_Params)
        {
            GH_LinkedParamAttributes paramAttr = (GH_LinkedParamAttributes)param.Attributes;

            GH_StateTagList tags = param.StateTags;
            if (tags.Count == 0) tags = null;
            if (tags != null) tagsCount = Math.Max(tagsCount, tags.Count);
            _tagsInfo.SetValue(paramAttr, tags);


            if (tags != null)
            {
                flag = true;
                Rectangle box = GH_Convert.ToRectangle(paramAttr.Bounds);
                tags.Layout(box, isInput ? GH_StateTagLayoutDirection.Left : GH_StateTagLayoutDirection.Right);
                box = tags.BoundingBox;
                if (!box.IsEmpty)
                {
                    paramAttr.Bounds = RectangleF.Union(paramAttr.Bounds, box);
                }
            }
        }

        if (flag)
        {
            if (isInput)
            {
                //Find minimum param box width.
                float minParamBoxX = float.MaxValue;
                foreach (IGH_Param param in gH_Params)
                {
                    minParamBoxX = Math.Min(minParamBoxX, param.Attributes.Bounds.X);
                }

                //Align all param box.
                foreach (IGH_Param param in gH_Params)
                {
                    IGH_Attributes attributes2 = param.Attributes;

                    RectangleF bounds = attributes2.Bounds;
                    bounds.Width = bounds.Right - minParamBoxX;
                    bounds.X = minParamBoxX;
                    attributes2.Bounds = bounds;
                }
            }
            else
            {
                float maxParamBoxRight = float.MinValue;
                foreach (IGH_Param param in gH_Params)
                {
                    maxParamBoxRight = Math.Max(maxParamBoxRight, param.Attributes.Bounds.Right);
                }
                foreach (IGH_Param param in gH_Params)
                {
                    IGH_Attributes attributes2 = param.Attributes;

                    RectangleF bounds = attributes2.Bounds;
                    bounds.Width = maxParamBoxRight - bounds.X;
                    attributes2.Bounds = bounds;
                }
            }

        }

        int additionforTag = tagsCount == 0 ? 0 : tagsCount * 20 - 4;

        //LayoutForRender
        foreach (IGH_Param param in gH_Params)
        {
            GH_AdvancedLinkParamAttr attr = (GH_AdvancedLinkParamAttr)param.Attributes;

            int stringwidth = attr.StringWidth;
            int wholeWidth = attr.WholeWidth;

            if (isInput)
            {
                float startX = attr.Bounds.X + additionforTag + NewData.ComponentToEdgeDistance;

                if (NewData.ShowLinkParamIcon)
                {
                    float size = NewData.ComponentParamIconSize;
                    attr.IconRect = new RectangleF(startX, attr.Bounds.Y + attr.Bounds.Height / 2 - size / 2, size, size);
                    startX += size + NewData.ComponentIconDistance;
                }

                if (NewData.SeperateCalculateWidthControl)
                {
                    float maxStringRight = startX + nameMaxWidth;
                    attr.StringRect = NewData.ComponentInputEdgeLayout ? new RectangleF(startX, attr.Bounds.Y, stringwidth, attr.Bounds.Height) :
                        new RectangleF(maxStringRight - stringwidth, attr.Bounds.Y, stringwidth, attr.Bounds.Height);


                    if (attr.Control != null)
                    {
                        attr.Control.Bounds = new RectangleF(NewData.ControlAlignRightLayout ? attr.Bounds.Right - attr.ControlWidth : maxStringRight + NewData.ComponentControlNameDistance,
                            attr.StringRect.Top + (attr.StringRect.Height - attr.Control.Height) / 2, attr.ControlWidth, attr.Control.Height);
                    }
                }
                else
                {
                    attr.StringRect = NewData.ComponentInputEdgeLayout ? new RectangleF(startX, attr.Bounds.Y, stringwidth, attr.Bounds.Height) :
                        new RectangleF(attr.Bounds.Right - wholeWidth, attr.Bounds.Y, stringwidth, attr.Bounds.Height);
                    if (attr.Control != null)
                    {
                        attr.Control.Bounds = new RectangleF(NewData.ControlAlignRightLayout ? attr.Bounds.Right - attr.ControlWidth : attr.StringRect.Right + NewData.ComponentControlNameDistance,
                            attr.StringRect.Top + (attr.StringRect.Height - attr.Control.Height) / 2, attr.ControlWidth, attr.Control.Height);
                    }

                }
            }
            else
            {
                attr.StringRect = NewData.ComponentOutputEdgeLayout ? new RectangleF(attr.Bounds.Right - additionforTag - wholeWidth - NewData.ComponentToEdgeDistance, attr.Bounds.Y, stringwidth, attr.Bounds.Height) :
                     new RectangleF(attr.Bounds.X, attr.Bounds.Y, stringwidth, attr.Bounds.Height);

                if (NewData.ShowLinkParamIcon)
                {
                    float size = NewData.ComponentParamIconSize;
                    attr.IconRect = new RectangleF(attr.Bounds.Right - NewData.ComponentParamIconSize - additionforTag - NewData.ComponentToEdgeDistance,
                        attr.Bounds.Y + attr.Bounds.Height / 2 - size / 2, size, size);
                }
            }
        }
    }

    public static void LayoutInputParamsNew(IGH_Component owner, RectangleF componentBox)
    {
        ParamsLayout(owner, componentBox, true);
    }

    public static void LayoutOutputParamsNew(IGH_Component owner, RectangleF componentBox)
    {
        ParamsLayout(owner, componentBox, false);
    }
    #endregion

    #region Render

    public static void RenderComponentParametersNew(GH_Canvas canvas, Graphics graphics, IGH_Component owner, GH_PaletteStyle style)
    {
        RenderComponentParametersPr(canvas, graphics, owner, style);
    }

    private static void RenderComponentParametersPr(GH_Canvas canvas, Graphics graphics, IGH_Component owner, GH_PaletteStyle style)
    {

        int zoomFadeLow = GH_Canvas.ZoomFadeLow;
        if (zoomFadeLow < 5)
        {
            return;
        }
        canvas.SetSmartTextRenderingHint();
        Color color = Color.FromArgb(zoomFadeLow, style.Text);
        SolidBrush solidBrush = new SolidBrush(color);
        foreach (IGH_Param item in owner.Params)
        {
            RectangleF bounds = item.Attributes.Bounds;
            if (!(bounds.Width < 1f))
            {
                if (item.Attributes is GH_AdvancedLinkParamAttr attr)
                {
                    //Render names.
                    graphics.DrawString(item.NickName, GH_FontServer.StandardAdjusted, solidBrush, attr.StringRect, GH_TextRenderingConstants.CenterCenter);

                    //Render Icon;
                    if (NewData.ShowLinkParamIcon)
                    {
                        if (!GH_AdvancedLinkParamAttr.IconSet.TryGetValue(item.ComponentGuid, out Bitmap icon))
                        {
                            icon = attr.SetParamIcon();
                        }
                        graphics.DrawImage(icon, attr.IconRect, new RectangleF(0, 0, icon.Width, icon.Height), GraphicsUnit.Pixel);
                    }

                    //Render Control
                    BaseControlItem control = attr.Control;
                    if (control != null && control.Bounds.Width >= 1f)
                    {
                        attr.Control.RenderObject(canvas, graphics, style);
                    }

                    //Render tags.
                    GH_StateTagList tags = (GH_StateTagList)_tagsInfo.GetValue(item.Attributes);
                    tags?.RenderStateTags(graphics);
                }
                else
                {
                    StringFormat format = item.Kind == GH_ParamKind.input ?
                        (NewData.ComponentInputEdgeLayout ? GH_TextRenderingConstants.NearCenter : GH_TextRenderingConstants.FarCenter) :
                        (NewData.ComponentOutputEdgeLayout ? GH_TextRenderingConstants.FarCenter : GH_TextRenderingConstants.NearCenter);

                    graphics.DrawString(item.NickName, GH_FontServer.StandardAdjusted, solidBrush, bounds, format);
                    GH_LinkedParamAttributes gH_LinkedParamAttributes = (GH_LinkedParamAttributes)item.Attributes;

                    GH_StateTagList tags = (GH_StateTagList)_tagsInfo.GetValue(gH_LinkedParamAttributes);
                    tags?.RenderStateTags(graphics);
                }
            }
        }
        solidBrush.Dispose();
    }
    #endregion
}
