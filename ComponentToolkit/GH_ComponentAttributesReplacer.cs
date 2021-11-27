using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
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
        public GH_ComponentAttributesReplacer(IGH_Component component): base(component)
        {

        }

        public static void Init()
        {
            ExchangeMethod(
               typeof(GH_ComponentAttributes).GetRuntimeMethods().Where(m => m.Name.Contains("RenderComponentParameters")).First(),
               typeof(GH_ComponentAttributesReplacer).GetRuntimeMethods().Where(m => m.Name.Contains("RenderComponentParametersNew")).First()
               );

            ExchangeMethod(
                typeof(GH_ComponentAttributes).GetRuntimeMethods().Where(m => m.Name.Contains("LayoutInputParams")).First(),
                typeof(GH_ComponentAttributesReplacer).GetRuntimeMethods().Where(m => m.Name.Contains("LayoutInputParamsNew")).First()
            );

			ExchangeMethod(
				typeof(GH_ComponentAttributes).GetRuntimeMethods().Where(m => m.Name.Contains("LayoutOutputParams")).First(),
				typeof(GH_ComponentAttributesReplacer).GetRuntimeMethods().Where(m => m.Name.Contains("LayoutOutputParamsNew")).First()
			);
		}
		public static void LayoutInputParamsNew(IGH_Component owner, RectangleF componentBox)
		{
			int count = owner.Params.Input.Count;
			if (count == 0)
			{
				return;
			}
			int num = 0;
			foreach (IGH_Param item in owner.Params.Input)
			{
				num = Math.Max(num, GH_FontServer.StringWidth(item.NickName, GH_FontServer.StandardAdjusted));
			}
			num = Math.Max(num + 6, 12);
			float num2 = componentBox.Height / (float)count;
			int num3 = count - 1;
			for (int i = 0; i <= num3; i++)
			{
				IGH_Param iGH_Param = owner.Params.Input[i];
				if (iGH_Param.Attributes == null)
				{
					iGH_Param.Attributes = new GH_LinkedParamAttributes(iGH_Param, owner.Attributes);
				}
				float num4 = componentBox.X - (float)num;
				float num5 = componentBox.Y + (float)i * num2;
				float width = num - 3;
				float height = num2;
				iGH_Param.Attributes.Pivot = new PointF(num4 + 0.5f * (float)num, num5 + 0.5f * num2);
				iGH_Param.Attributes.Bounds = GH_Convert.ToRectangle(new RectangleF(num4, num5, width, height));
			}
			bool flag = false;
			int num6 = count - 1;
			for (int j = 0; j <= num6; j++)
			{
				IGH_Param iGH_Param2 = owner.Params.Input[j];
				GH_LinkedParamAttributes gH_LinkedParamAttributes = (GH_LinkedParamAttributes)iGH_Param2.Attributes;
				//gH_LinkedParamAttributes.m_renderTags = iGH_Param2.StateTags;
				//if (gH_LinkedParamAttributes.m_renderTags.Count == 0)
				//{
				//	gH_LinkedParamAttributes.m_renderTags = null;
				//}
				//if (gH_LinkedParamAttributes.m_renderTags != null)
				//{
				//	flag = true;
				//	Rectangle box = GH_Convert.ToRectangle(gH_LinkedParamAttributes.Bounds);
				//	gH_LinkedParamAttributes.m_renderTags.Layout(box, GH_StateTagLayoutDirection.Left);
				//	box = gH_LinkedParamAttributes.m_renderTags.BoundingBox;
				//	if (!box.IsEmpty)
				//	{
				//		gH_LinkedParamAttributes.Bounds = RectangleF.Union(gH_LinkedParamAttributes.Bounds, box);
				//	}
				//}
			}
			if (flag)
			{
				float num7 = float.MaxValue;
				int num8 = count - 1;
				for (int k = 0; k <= num8; k++)
				{
					IGH_Attributes attributes = owner.Params.Input[k].Attributes;
					num7 = Math.Min(num7, attributes.Bounds.X);
				}
				int num9 = count - 1;
				for (int l = 0; l <= num9; l++)
				{
					IGH_Attributes attributes2 = owner.Params.Input[l].Attributes;
					RectangleF bounds = attributes2.Bounds;
					bounds.Width = bounds.Right - num7;
					bounds.X = num7;
					attributes2.Bounds = bounds;
				}
			}
		}

		public static void LayoutOutputParamsNew(IGH_Component owner, RectangleF componentBox)
		{
			int count = owner.Params.Output.Count;
			if (count == 0)
			{
				return;
			}
			int num = 0;
			foreach (IGH_Param item in owner.Params.Output)
			{
				num = Math.Max(num, GH_FontServer.StringWidth(item.NickName, GH_FontServer.StandardAdjusted));
			}
			num = Math.Max(num + 6, 12);
			float num2 = componentBox.Height / (float)count;
			int num3 = count - 1;
			for (int i = 0; i <= num3; i++)
			{
				IGH_Param iGH_Param = owner.Params.Output[i];
				if (iGH_Param.Attributes == null)
				{
					iGH_Param.Attributes = new GH_LinkedParamAttributes(iGH_Param, owner.Attributes);
				}
				float num4 = componentBox.Right + 3f;
				float num5 = componentBox.Y + (float)i * num2;
				float width = num;
				float height = num2;
				iGH_Param.Attributes.Pivot = new PointF(num4 + 0.5f * (float)num, num5 + 0.5f * num2);
				iGH_Param.Attributes.Bounds = GH_Convert.ToRectangle(new RectangleF(num4, num5, width, height));
			}
			bool flag = false;
			int num6 = count - 1;
			for (int j = 0; j <= num6; j++)
			{
				IGH_Param iGH_Param2 = owner.Params.Output[j];
				GH_LinkedParamAttributes gH_LinkedParamAttributes = (GH_LinkedParamAttributes)iGH_Param2.Attributes;
				//gH_LinkedParamAttributes.m_renderTags = iGH_Param2.StateTags;
				//if (gH_LinkedParamAttributes.m_renderTags.Count == 0)
				//{
				//	gH_LinkedParamAttributes.m_renderTags = null;
				//}
				//if (gH_LinkedParamAttributes.m_renderTags != null)
				//{
				//	flag = true;
				//	Rectangle box = GH_Convert.ToRectangle(gH_LinkedParamAttributes.Bounds);
				//	gH_LinkedParamAttributes.m_renderTags.Layout(box, GH_StateTagLayoutDirection.Right);
				//	box = gH_LinkedParamAttributes.m_renderTags.BoundingBox;
				//	if (!box.IsEmpty)
				//	{
				//		gH_LinkedParamAttributes.Bounds = RectangleF.Union(gH_LinkedParamAttributes.Bounds, box);
				//	}
				//}
			}
			if (flag)
			{
				float num7 = float.MinValue;
				int num8 = count - 1;
				for (int k = 0; k <= num8; k++)
				{
					IGH_Attributes attributes = owner.Params.Output[k].Attributes;
					num7 = Math.Max(num7, attributes.Bounds.Right);
				}
				int num9 = count - 1;
				for (int l = 0; l <= num9; l++)
				{
					IGH_Attributes attributes2 = owner.Params.Output[l].Attributes;
					RectangleF bounds = attributes2.Bounds;
					bounds.Width = num7 - bounds.X;
					attributes2.Bounds = bounds;
				}
			}
		}

		public static void RenderComponentParametersNew(GH_Canvas canvas, Graphics graphics, IGH_Component owner, GH_PaletteStyle style)
        {
            int zoomFadeLow = GH_Canvas.ZoomFadeLow;
            if (zoomFadeLow < 5)
            {
                return;
            }
            StringFormat farCenter = GH_TextRenderingConstants.FarCenter;
            canvas.SetSmartTextRenderingHint();
            Color color = Color.FromArgb(zoomFadeLow, style.Text);
            SolidBrush solidBrush = new SolidBrush(color);
            foreach (IGH_Param item in owner.Params.Input)
            {
                RectangleF bounds = item.Attributes.Bounds;
                if (!(bounds.Width < 1f))
                {
                    graphics.DrawString(item.NickName, GH_FontServer.StandardAdjusted, solidBrush, bounds, farCenter);
                    GH_LinkedParamAttributes gH_LinkedParamAttributes = (GH_LinkedParamAttributes)item.Attributes;
                    //if (gH_LinkedParamAttributes.m_renderTags != null)
                    //{
                    //    gH_LinkedParamAttributes.m_renderTags.RenderStateTags(graphics);
                    //}
                    graphics.DrawRectangle(new Pen(solidBrush), GH_Convert.ToRectangle(bounds));
                }
            }
            farCenter = GH_TextRenderingConstants.NearCenter;
            foreach (IGH_Param item2 in owner.Params.Output)
            {
                RectangleF bounds2 = item2.Attributes.Bounds;
                if (!(bounds2.Width < 1f))
                {
                    graphics.DrawString(item2.NickName, GH_FontServer.StandardAdjusted, solidBrush, bounds2, farCenter);
                    GH_LinkedParamAttributes gH_LinkedParamAttributes2 = (GH_LinkedParamAttributes)item2.Attributes;
                    //if (gH_LinkedParamAttributes2.m_renderTags != null)
                    //{
                    //    gH_LinkedParamAttributes2.m_renderTags.RenderStateTags(graphics);
                    //}
                }
            }

            solidBrush.Dispose();
        }

        private static bool ExchangeMethod(MethodInfo targetMethod, MethodInfo injectMethod)
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
    }
}
