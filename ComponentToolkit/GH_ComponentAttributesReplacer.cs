using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComponentToolkit
{
    public abstract class GH_ComponentAttributesReplacer: GH_ComponentAttributes
    {
        #region Settings
        public static readonly int MiniWidth = 6;

        public static readonly int _componentToEdgeDistanceDefault = 3;
        public static int ComponentToEdgeDistance
        {
            get => Grasshopper.Instances.Settings.GetValue(nameof(ComponentToEdgeDistance), _componentToEdgeDistanceDefault);
            set
            {
                Grasshopper.Instances.Settings.SetValue(nameof(ComponentToEdgeDistance), value);
                RefreshLayout();
            }
        }
        public static readonly int _componentToCoreDistanceDefault = 3;
        public static int ComponentToCoreDistance
        {
            get => Grasshopper.Instances.Settings.GetValue(nameof(ComponentToCoreDistance), _componentToCoreDistanceDefault);
            set
            {
                Grasshopper.Instances.Settings.SetValue(nameof(ComponentToCoreDistance), value);
                RefreshLayout();
            }
        }

        public static readonly int _componentControlNameDistanceDefault = 2;
        public static int ComponentControlNameDistance
        {
            get => Grasshopper.Instances.Settings.GetValue(nameof(ComponentControlNameDistance), _componentControlNameDistanceDefault);
            set
            {
                Grasshopper.Instances.Settings.SetValue(nameof(ComponentControlNameDistance), value);
                RefreshLayout();
            }

        }

        public static int AdditionWidth => ComponentToEdgeDistance + ComponentToCoreDistance;

        public static bool ComponentUseControl
        {
            get => Grasshopper.Instances.Settings.GetValue(nameof(ComponentUseControl), true);
            set
            {
                Grasshopper.Instances.Settings.SetValue(nameof(ComponentUseControl), value);
                RefreshLayout();
            }
        }

        public static bool ComponentInputEdgeLayout
        {
            get => Grasshopper.Instances.Settings.GetValue(nameof(ComponentInputEdgeLayout), true);
            set
            {
                Grasshopper.Instances.Settings.SetValue(nameof(ComponentInputEdgeLayout), value);
                RefreshLayout();
            }
        }

        public static bool ComponentOutputEdgeLayout
        {
            get => Grasshopper.Instances.Settings.GetValue(nameof(ComponentOutputEdgeLayout), false);
            set
            {
                Grasshopper.Instances.Settings.SetValue(nameof(ComponentOutputEdgeLayout), value);
                RefreshLayout();
            }
        }

        private static void RefreshLayout()
        {
            foreach (IGH_DocumentObject @object in Grasshopper.Instances.ActiveCanvas.Document.Objects)
            {
                @object.Attributes.ExpireLayout();
            }
            Grasshopper.Instances.RedrawCanvas();
        }
        #endregion

        private static readonly FieldInfo _tagsInfo = typeof(GH_LinkedParamAttributes).GetRuntimeFields().Where(m => m.Name.Contains("m_renderTags")).First();

        public GH_ComponentAttributesReplacer(IGH_Component component): base(component)
        {

        }

        public static void Init()
        {
            ExchangeMethod(
                typeof(GH_ComponentAttributes).GetRuntimeMethods().Where(m => m.Name.Contains(nameof(GH_ComponentAttributesReplacer.RenderComponentParameters))).First(),
                typeof(GH_ComponentAttributesReplacer).GetRuntimeMethods().Where(m => m.Name.Contains(nameof(GH_ComponentAttributesReplacer.RenderComponentParametersNew))).First()
            );

            ExchangeMethod(
                typeof(GH_ComponentAttributes).GetRuntimeMethods().Where(m => m.Name.Contains(nameof(GH_ComponentAttributesReplacer.LayoutInputParams))).First(),
                typeof(GH_ComponentAttributesReplacer).GetRuntimeMethods().Where(m => m.Name.Contains(nameof(GH_ComponentAttributesReplacer.LayoutInputParamsNew))).First()
            );

            ExchangeMethod(
                typeof(GH_ComponentAttributes).GetRuntimeMethods().Where(m => m.Name.Contains(nameof(GH_ComponentAttributesReplacer.LayoutOutputParams))).First(),
                typeof(GH_ComponentAttributesReplacer).GetRuntimeMethods().Where(m => m.Name.Contains(nameof(GH_ComponentAttributesReplacer.LayoutOutputParamsNew))).First()
            );
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




        private static void ParamsLayout(IGH_Component owner, RectangleF componentBox, bool isInput)
        {
            List<IGH_Param> gH_Params = isInput ? owner.Params.Input : owner.Params.Output;

            int count = gH_Params.Count;
            if (count == 0) return;

            //Get width and Init the Attributes.
            int singleParamBoxWidth = 0;
            foreach (IGH_Param param in gH_Params)
            {
                if (param.Attributes == null || !(param.Attributes is GH_AdvancedLinkParamAttr))
                {
                    param.Attributes = new GH_AdvancedLinkParamAttr(param, owner.Attributes);

                    //Refresh the attributes.
                    owner.OnPingDocument()?.DestroyAttributeCache();
                }
                GH_AdvancedLinkParamAttr attr = (GH_AdvancedLinkParamAttr)param.Attributes;
                singleParamBoxWidth = Math.Max(singleParamBoxWidth, attr.WholeWidth);
            }
            singleParamBoxWidth = Math.Max(singleParamBoxWidth + AdditionWidth, AdditionWidth + MiniWidth);

            //Layout every param.
            float singleParamBoxHeight = componentBox.Height / count;
            for (int i = 0; i < count; i++)
            {
                IGH_Param param = gH_Params[i];

                float rectX = isInput ? componentBox.X - singleParamBoxWidth : componentBox.Right + ComponentToCoreDistance;
                float rectY = componentBox.Y + i * singleParamBoxHeight;
                float width = singleParamBoxWidth - ComponentToCoreDistance;
                float height = singleParamBoxHeight;
                param.Attributes.Pivot = new PointF(rectX + 0.5f * singleParamBoxWidth, rectY + 0.5f * singleParamBoxHeight);
                param.Attributes.Bounds = GH_Convert.ToRectangle(new RectangleF(rectX, rectY, width, height));

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
                    attr.StringRect = ComponentInputEdgeLayout ? new RectangleF(attr.Bounds.X + additionforTag + ComponentToEdgeDistance, attr.Bounds.Y, stringwidth, attr.Bounds.Height) :
                        new RectangleF(attr.Bounds.Right - wholeWidth, attr.Bounds.Y, stringwidth, attr.Bounds.Height);

                    if(attr.Control != null)
                    {
                        attr.Control.Bounds = new RectangleF(attr.StringRect.Right + ComponentControlNameDistance, attr.StringRect.Top + (attr.StringRect.Height - attr.Control.Height)/2, attr.ControlWidth, attr.Control.Height);
                    }
                }
                else
                {
                    attr.StringRect = ComponentOutputEdgeLayout ? new RectangleF(attr.Bounds.Right - additionforTag - wholeWidth - ComponentToEdgeDistance, attr.Bounds.Y, stringwidth, attr.Bounds.Height) :
                         new RectangleF(attr.Bounds.X, attr.Bounds.Y, stringwidth, attr.Bounds.Height);

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

                    //Render Control
                    BaseControlItem control = attr.Control;
                    if(control != null && control.Bounds.Width >= 1f)
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
