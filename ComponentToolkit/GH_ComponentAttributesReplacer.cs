using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace ComponentToolkit
{
    public abstract class GH_ComponentAttributesReplacer: GH_ComponentAttributes
    {


        private static readonly string _location = Path.Combine(Folders.SettingsFolder, "quickwires.json");

        internal static CreateObjectItems StaticCreateObjectItems;

        private static readonly FieldInfo _tagsInfo = typeof(GH_LinkedParamAttributes).GetRuntimeFields().Where(m => m.Name.Contains("m_renderTags")).First();

        public GH_ComponentAttributesReplacer(IGH_Component component): base(component)
        {

        }

        internal static void SaveToJson()
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            File.WriteAllText(_location, ser.Serialize(new CreateObjectItemsSave(StaticCreateObjectItems)));
        }

        internal static void RemoveAllQuickwireSettings()
        {
            if (File.Exists(_location))
                File.Delete(_location);
            StaticCreateObjectItems = new CreateObjectItems();
        }

        public static void Init()
        {
            //Read from json.
            try
            {
                string jsonStr;
                if (File.Exists(_location))
                {
                    jsonStr = File.ReadAllText(_location);
                }
                else
                {
                    jsonStr = Properties.Resources.quickwires;
                }
                JavaScriptSerializer ser = new JavaScriptSerializer();
                StaticCreateObjectItems = new CreateObjectItems(ser.Deserialize<CreateObjectItemsSave>(jsonStr));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Instances.ActiveCanvas.Document_ObjectsAdded += ActiveCanvas_Document_ObjectsAdded;
            Instances.ActiveCanvas.DocumentChanged += ActiveCanvas_DocumentChanged;

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
            if (e.NewDocument == null) return;
            foreach (var item in e.NewDocument.Objects)
            {
                ChangeFloatParam(item);
            }
            e.NewDocument?.DestroyAttributeCache();
        }

        private static void ActiveCanvas_Document_ObjectsAdded(GH_Document sender, GH_DocObjectEventArgs e)
        {
            foreach (var item in e.Objects)
            {
                ChangeFloatParam(item);
            }
            sender?.DestroyAttributeCache();
        }

        private static void ChangeFloatParam(IGH_DocumentObject item)
        {
            if (item is IGH_Param)
            {
                var param = (IGH_Param)item;
                if (param.Kind == GH_ParamKind.floating && param.Attributes is GH_FloatingParamAttributes && !(param.Attributes is GH_AdvancedFloatingParamAttr))
                {
                    PointF point = param.Attributes.Pivot;
                    param.Attributes = new GH_AdvancedFloatingParamAttr(param);
                    param.Attributes.Pivot = point;
                    param.Attributes.ExpireLayout();
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
            return GH_Convert.ToRectangle(new RectangleF(owner.Attributes.Pivot.X - 0.5f * (float)num, owner.Attributes.Pivot.Y - 0.5f * (float)val, num, val));
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
                        attr.IconPivot = new PointF(startX, attr.Bounds.Y + attr.Bounds.Height /2 - size/2);
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
                        attr.IconPivot = new PointF(attr.Bounds.Right - Datas.ComponentParamIconSize - additionforTag - Datas.ComponentToEdgeDistance, 
                            attr.Bounds.Y + attr.Bounds.Height / 2 - size / 2);
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
                        graphics.DrawImage(icon, attr.IconPivot);
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
            }
            solidBrush.Dispose();
        }

    }
}
