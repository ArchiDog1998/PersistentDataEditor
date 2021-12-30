using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Special;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;

namespace PersistentDataEditor
{
    public abstract class GH_ComponentAttributesReplacer: GH_ComponentAttributes
    {
        private static readonly MethodInfo _renderInfo = typeof(GH_FloatingParamAttributes).GetRuntimeMethods().Where(m => m.Name.Contains("Render")).First();


        private static readonly FieldInfo _tagsInfo = typeof(GH_LinkedParamAttributes).GetRuntimeFields().Where(m => m.Name.Contains("m_renderTags")).First();
        private static readonly MethodInfo _pathMapperCreate = typeof(GH_PathMapper).GetRuntimeMethods().Where(m => m.Name.Contains("GetInputMapping")).First();

        public GH_ComponentAttributesReplacer(IGH_Component component): base(component)
        {

        }

        public static void Init()
        {
            Instances.ActiveCanvas.DocumentChanged += ActiveCanvas_DocumentChanged;
            Instances.ActiveCanvas.MouseDown += ActiveCanvas_MouseDown;

            new MoveShowConduit().Enabled = true;

            ExchangeMethod(
                typeof(GH_ComponentAttributes).GetRuntimeMethods().Where(m => m.Name.Contains(nameof(GH_ComponentAttributes.RenderComponentParameters))).First(),
                typeof(GH_ComponentAttributesReplacer).GetRuntimeMethods().Where(m => m.Name.Contains(nameof(GH_ComponentAttributesReplacer.RenderComponentParametersNew))).First()
            );

            ExchangeMethod(
                typeof(GH_ComponentAttributes).GetRuntimeMethods().Where(m => m.Name.Contains(nameof(GH_ComponentAttributes.LayoutComponentBox))).First(),
                typeof(GH_ComponentAttributesReplacer).GetRuntimeMethods().Where(m => m.Name.Contains(nameof(GH_ComponentAttributesReplacer.LayoutComponentBoxNew))).First()
            );

            ExchangeMethod(
                typeof(GH_ComponentAttributes).GetRuntimeMethods().Where(m => m.Name.Contains(nameof(GH_ComponentAttributes.LayoutInputParams))).First(),
                typeof(GH_ComponentAttributesReplacer).GetRuntimeMethods().Where(m => m.Name.Contains(nameof(GH_ComponentAttributesReplacer.LayoutInputParamsNew))).First()
            );

            ExchangeMethod(
                typeof(GH_ComponentAttributes).GetRuntimeMethods().Where(m => m.Name.Contains(nameof(GH_ComponentAttributes.LayoutOutputParams))).First(),
                typeof(GH_ComponentAttributesReplacer).GetRuntimeMethods().Where(m => m.Name.Contains(nameof(GH_ComponentAttributesReplacer.LayoutOutputParamsNew))).First()
            );
        }


        private static void ActiveCanvas_DocumentChanged(GH_Canvas sender, GH_CanvasDocumentChangedEventArgs e)
        {
            if(e.OldDocument != null)
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
                if (component is IGH_Component)
                {
                    foreach (var item in ((IGH_Component)component).Params.Input)
                    {
                        if (item.Attributes is GH_AdvancedLinkParamAttr)
                        {
                            ((GH_AdvancedLinkParamAttr)item.Attributes).Dispose();
                        }
                    }
                }
                if (component is IGH_Param)
                {
                    IGH_Param param = (IGH_Param)component;
                    if (param.Attributes is GH_AdvancedFloatingParamAttr)
                    {
                        GH_AdvancedFloatingParamAttr floating = (GH_AdvancedFloatingParamAttr)(param.Attributes);
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

            GH_RelevantObjectData obj = Instances.ActiveCanvas.Document.RelevantObjectAtPoint(canvasLocation, GH_RelevantObjectFilter.Attributes);
            if(obj != null)
            {
                if (OnGumballComponent != obj.TopLevelObject)
                {
                    CloseGumball();
                }

                if (obj.TopLevelObject is IGH_Component)
                {
                    IGH_Component gH_Component = (IGH_Component)obj.TopLevelObject;
                    new Thread(() =>
                    {
                        Thread.Sleep(5);

                        if (!gH_Component.Attributes.Selected) return;

                        OnGumballComponent = gH_Component;
                        foreach (var item in gH_Component.Params.Input)
                        {
                            if (item.Attributes is GH_AdvancedLinkParamAttr)
                            {
                                ((GH_AdvancedLinkParamAttr)item.Attributes).ShowAllGumballs();
                            }
                        }

                    }).Start();
                }
                else if (obj.TopLevelObject is IGH_Param)
                {
                    IGH_Param gH_Param = (IGH_Param)obj.TopLevelObject;
                    new Thread(() =>
                    {
                        Thread.Sleep(5);

                        if (!gH_Param.Attributes.Selected) return;

                        if (!(gH_Param.Attributes is GH_AdvancedFloatingParamAttr)) return;

                        OnGumballComponent = gH_Param;
                        GH_AdvancedFloatingParamAttr attr = (GH_AdvancedFloatingParamAttr)gH_Param.Attributes;
                        attr.RedrawGumballs();

                    }).Start();
                }
            }
            else
            {
                CloseGumball();
            }
        }

        private static void CloseGumball()
        {
            if (OnGumballComponent == null) return;

            if (OnGumballComponent is IGH_Component)
            {
                foreach (var item in ((IGH_Component)OnGumballComponent).Params.Input)
                {
                    if (item.Attributes is GH_AdvancedLinkParamAttr)
                    {
                        ((GH_AdvancedLinkParamAttr)item.Attributes).Dispose();
                    }
                }
            }
            else
            {
                ((GH_AdvancedFloatingParamAttr)((IGH_Param)OnGumballComponent).Attributes).Dispose();
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
                if (item is GH_PathMapper)
                {
                    GH_PathMapper pathMapper = (GH_PathMapper)item;

                    Instances.ActiveCanvas.Document.ScheduleSolution(50, (doc) =>
                    {
                        pathMapper.Lexers.Clear();
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
                else if (item is GH_NumberSlider)
                {
                    GH_NumberSlider slider = (GH_NumberSlider)item;
                    if (slider.Slider.Type == Grasshopper.GUI.Base.GH_SliderAccuracy.Integer)
                    {
                        slider.Slider.DecimalPlaces = 0;
                        slider.Slider.Type = Grasshopper.GUI.Base.GH_SliderAccuracy.Float;
                    }
                }
            }
            e.Document?.DestroyAttributeCache();
        }

        private static void ChangeFloatParam(IGH_DocumentObject item)
        {
            if (item is IGH_Param)
            {
                var param = (IGH_Param)item;
                if (param.Kind == GH_ParamKind.floating && param.Attributes is GH_FloatingParamAttributes && !(param.Attributes is GH_AdvancedFloatingParamAttr))
                {

                    MethodInfo renderplus = param.Attributes.GetType().GetRuntimeMethods().Where(m => m.Name.Contains("Render")).First();

                    RuntimeHelpers.PrepareMethod(_renderInfo.MethodHandle);
                    RuntimeHelpers.PrepareMethod(renderplus.MethodHandle);
                    unsafe
                    {
                        if (_renderInfo.MethodHandle.Value.ToPointer() != renderplus.MethodHandle.Value.ToPointer()) return;
                    }

                    PointF point = param.Attributes.Pivot;
                    bool isSelected = param.Attributes.Selected;
                    param.Attributes = new GH_AdvancedFloatingParamAttr(param);
                    param.Attributes.Pivot = point;
                    param.Attributes.Selected = isSelected;
                    param.Attributes.ExpireLayout();
                }
                else if (item is GH_NumberSlider && item.Attributes is GH_NumberSliderAttributes && !(item.Attributes is GH_AdvancedNumberSliderAttr))
                {
                    GH_NumberSlider slider = (GH_NumberSlider)item;

                    PointF point = item.Attributes.Pivot;
                    bool isSelected = item.Attributes.Selected;
                    item.Attributes = new GH_AdvancedNumberSliderAttr((GH_NumberSlider)item);
                    item.Attributes.Pivot = point;
                    item.Attributes.Selected = isSelected;
                    item.Attributes.ExpireLayout();
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
                if (param.Attributes == null || !(param.Attributes is GH_AdvancedLinkParamAttr))
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
            int iconSpaceWidth = Datas.ShowLinkParamIcon ? Datas.ComponentParamIconSize + Datas.ComponentIconDistance : 0;
            foreach (IGH_Param param in gH_Params)
            {
                if (param.Attributes == null || !(param.Attributes is GH_AdvancedLinkParamAttr))
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

            if (Datas.SeperateCalculateWidthControl)
            {
                int controlAddition = controlMaxWidth == 0? 0 : controlMaxWidth + Datas.ComponentControlNameDistance;
                singleParamBoxMaxWidth = Math.Max(nameMaxWidth + controlAddition + Datas.AdditionWidth + iconSpaceWidth, Datas.AdditionWidth + Datas.MiniWidth);
            }
            else singleParamBoxMaxWidth = Math.Max(singleParamBoxMaxWidth + Datas.AdditionWidth, Datas.AdditionWidth + Datas.MiniWidth);


            //Layout every param.
            float heightFloat = componentBox.Height / heightCalculate;
            float movementY = 0;
            foreach (IGH_Param param in gH_Params)
            {
                float singleParamBoxHeight = heightFloat * ((GH_AdvancedLinkParamAttr)param.Attributes).ParamHeight;

                float rectX = isInput ? componentBox.X - singleParamBoxMaxWidth : componentBox.Right + Datas.ComponentToCoreDistance;
                float rectY = componentBox.Y + movementY;
                float width = singleParamBoxMaxWidth - Datas.ComponentToCoreDistance;
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
                if(tags != null) tagsCount = Math.Max(tagsCount, tags.Count);
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
                    float startX = attr.Bounds.X + additionforTag + Datas.ComponentToEdgeDistance;

                    if (Datas.ShowLinkParamIcon)
                    {
                        float size = Datas.ComponentParamIconSize;
                        attr.IconRect = new RectangleF(startX, attr.Bounds.Y + attr.Bounds.Height /2 - size/2, size, size);
                        startX += size + Datas.ComponentIconDistance;
                    }

                    if (Datas.SeperateCalculateWidthControl)
                    {
                        float maxStringRight = startX + nameMaxWidth;
                        attr.StringRect = Datas.ComponentInputEdgeLayout ? new RectangleF(startX , attr.Bounds.Y, stringwidth, attr.Bounds.Height) :
                            new RectangleF(maxStringRight - stringwidth, attr.Bounds.Y, stringwidth, attr.Bounds.Height);


                        if (attr.Control != null)
                        {
                            attr.Control.Bounds = new RectangleF(Datas.ControlAlignRightLayout ? attr.Bounds.Right - attr.ControlWidth : maxStringRight + Datas.ComponentControlNameDistance,
                                attr.StringRect.Top + (attr.StringRect.Height - attr.Control.Height) / 2, attr.ControlWidth, attr.Control.Height);
                        }
                    }
                    else
                    {
                        attr.StringRect = Datas.ComponentInputEdgeLayout ? new RectangleF(startX, attr.Bounds.Y, stringwidth, attr.Bounds.Height) :
                            new RectangleF(attr.Bounds.Right - wholeWidth, attr.Bounds.Y, stringwidth, attr.Bounds.Height);
                        if (attr.Control != null)
                        {
                            attr.Control.Bounds = new RectangleF(Datas.ControlAlignRightLayout ? attr.Bounds.Right - attr.ControlWidth : attr.StringRect.Right + Datas.ComponentControlNameDistance,
                                attr.StringRect.Top + (attr.StringRect.Height - attr.Control.Height) / 2, attr.ControlWidth, attr.Control.Height);
                        }

                    }
                }
                else
                {
                    attr.StringRect = Datas.ComponentOutputEdgeLayout ? new RectangleF(attr.Bounds.Right - additionforTag - wholeWidth - Datas.ComponentToEdgeDistance, attr.Bounds.Y, stringwidth, attr.Bounds.Height) :
                         new RectangleF(attr.Bounds.X, attr.Bounds.Y, stringwidth, attr.Bounds.Height);

                    if (Datas.ShowLinkParamIcon)
                    {
                        float size = Datas.ComponentParamIconSize;
                        attr.IconRect = new RectangleF(attr.Bounds.Right - Datas.ComponentParamIconSize - additionforTag - Datas.ComponentToEdgeDistance, 
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
                    if(item.Attributes is GH_AdvancedLinkParamAttr)
                    {
                        GH_AdvancedLinkParamAttr attr = (GH_AdvancedLinkParamAttr)item.Attributes;

                        //Render names.
                        graphics.DrawString(item.NickName, GH_FontServer.StandardAdjusted, solidBrush, attr.StringRect, GH_TextRenderingConstants.CenterCenter);

                        //Render Icon;
                        if (Datas.ShowLinkParamIcon)
                        {
                            Bitmap icon;
                            if (!GH_AdvancedLinkParamAttr.IconSet.TryGetValue(item.ComponentGuid, out icon))
                            {
                                icon = attr.SetParamIcon();
                            }
                            graphics.DrawImage(icon, attr.IconRect, new RectangleF(0, 0, icon.Width, icon.Height), GraphicsUnit.Pixel);
                        }

                        //Render Control
                        BaseControlItem control = attr.Control;
                        if (control != null && control.Bounds.Width >= 1f)
                        {
                            attr.Control.RenderObject(canvas, graphics, owner, style);
                        }

                        //Render tags.
                        GH_StateTagList tags = (GH_StateTagList)_tagsInfo.GetValue(item.Attributes);
                        if (tags != null) tags.RenderStateTags(graphics);
                    }
                    else
                    {
                        StringFormat format = item.Kind == GH_ParamKind.input ?
                            ( Datas.ComponentInputEdgeLayout ? GH_TextRenderingConstants.NearCenter : GH_TextRenderingConstants.FarCenter) :
                            ( Datas.ComponentOutputEdgeLayout ? GH_TextRenderingConstants.FarCenter : GH_TextRenderingConstants.NearCenter);

                        graphics.DrawString(item.NickName, GH_FontServer.StandardAdjusted, solidBrush, bounds, format);
                        GH_LinkedParamAttributes gH_LinkedParamAttributes = (GH_LinkedParamAttributes)item.Attributes;

                        GH_StateTagList tags = (GH_StateTagList)_tagsInfo.GetValue(gH_LinkedParamAttributes);
                        if (tags != null)
                        {
                            tags.RenderStateTags(graphics);
                        }
                    }
                }
            }
            solidBrush.Dispose();
        }
        #endregion
    }
}
