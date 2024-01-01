using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

namespace PersistentDataEditor;

internal class GH_AdvancedComponentAttr(IGH_Component component) 
    : GH_ComponentAttributes(component)
{
    #region Layout
    private static readonly FieldInfo _tagsInfo = typeof(GH_LinkedParamAttributes).FindField("m_renderTags");
    protected override void Layout()
    {
        Pivot = GH_Convert.ToPoint(Pivot);
        m_innerBounds = LayoutComponentBoxNew(Owner);
        LayoutInputParamsNew(Owner, m_innerBounds);
        LayoutOutputParamsNew(Owner, m_innerBounds);
        Bounds = LayoutBounds(Owner, m_innerBounds);
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
        return GH_Convert.ToRectangle(new RectangleF(owner.Attributes.Pivot.X - 0.5f * num, owner.Attributes.Pivot.Y - 0.5f * val, num, val));

        static int GetParamsWholeHeight(List<IGH_Param> gH_Params, IGH_Component owner)
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
    }

    public static void LayoutInputParamsNew(IGH_Component owner, RectangleF componentBox)
    {
        ParamsLayout(owner, componentBox, true);
    }

    public static void LayoutOutputParamsNew(IGH_Component owner, RectangleF componentBox)
    {
        ParamsLayout(owner, componentBox, false);
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
        int iconSpaceWidth = Data.ShowLinkParamIcon ? Data.ComponentParamIconSize + Data.ComponentIconDistance : 0;
        foreach (IGH_Param param in gH_Params)
        {
            if (param.Attributes is not GH_AdvancedLinkParamAttr)
            {
                param.Attributes = new GH_AdvancedLinkParamAttr(param, owner.Attributes);

                //Refresh the attributes.
                owner.OnPingDocument()?.DestroyAttributeCache();
            }
            GH_AdvancedLinkParamAttr attr = (GH_AdvancedLinkParamAttr)param.Attributes;

            singleParamBoxMaxWidth = Math.Max(singleParamBoxMaxWidth, attr.WholeWidth);
            nameMaxWidth = Math.Max(nameMaxWidth, attr.StringWidth);
            controlMaxWidth = Math.Max(controlMaxWidth, attr.ControlMinWidth);
            heightCalculate += attr.ParamHeight;
        }

        if (Data.SeperateCalculateWidthControl)
        {
            int controlAddition = controlMaxWidth == 0 ? 0 : controlMaxWidth + Data.ComponentControlNameDistance;
            singleParamBoxMaxWidth = Math.Max(nameMaxWidth + controlAddition + Data.AdditionWidth + iconSpaceWidth, Data.AdditionWidth + Data.MiniWidth);
        }
        else
        {
            singleParamBoxMaxWidth = Math.Max(singleParamBoxMaxWidth + Data.AdditionWidth, Data.AdditionWidth + Data.MiniWidth);
        }


        //Layout every param.
        float heightFloat = componentBox.Height / heightCalculate;
        float movementY = 0;
        foreach (IGH_Param param in gH_Params)
        {
            float singleParamBoxHeight = heightFloat * ((GH_AdvancedLinkParamAttr)param.Attributes).ParamHeight;

            float rectX = isInput ? componentBox.X - singleParamBoxMaxWidth : componentBox.Right + Data.ComponentToCoreDistance;
            float rectY = componentBox.Y + movementY;
            float width = singleParamBoxMaxWidth - Data.ComponentToCoreDistance;
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

            if (isInput)
            {
                float startX = attr.Bounds.X + additionforTag + Data.ComponentToEdgeDistance;

                if (Data.ShowLinkParamIcon)
                {
                    float size = Data.ComponentParamIconSize;
                    attr.IconRect = new RectangleF(startX, attr.Bounds.Y + attr.Bounds.Height / 2 - size / 2, size, size);
                    startX += size + Data.ComponentIconDistance;
                }

                attr.Control.Bounds = new RectangleF(startX,
                    attr.Bounds.Y, attr.Bounds.Width - Data.ComponentControlNameDistance -
                    (Data.SeperateCalculateWidthControl ? nameMaxWidth : stringwidth), attr.Bounds.Height);
            }
            else
            {
                if (Data.ShowLinkParamIcon)
                {
                    float size = Data.ComponentParamIconSize;
                    attr.IconRect = new RectangleF(attr.Bounds.Right - Data.ComponentParamIconSize - additionforTag - Data.ComponentToEdgeDistance,
                        attr.Bounds.Y + attr.Bounds.Height / 2 - size / 2, size, size);
                }
            }
        }
    }

    #endregion
    protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
    {
        base.Render(canvas, graphics, channel);
        if (channel != GH_CanvasChannel.Objects) return;

        GH_Palette gH_Palette = GH_CapsuleRenderEngine.GetImpliedPalette(base.Owner);
        if (gH_Palette == GH_Palette.Normal && !base.Owner.IsPreviewCapable)
        {
            gH_Palette = GH_Palette.Hidden;
        }
        GH_Capsule gH_Capsule = GH_Capsule.CreateCapsule(Bounds, gH_Palette);
        bool left = base.Owner.Params.Input.Count == 0;
        bool right = base.Owner.Params.Output.Count == 0;
        gH_Capsule.SetJaggedEdges(left, right);
        GH_PaletteStyle impliedStyle = GH_CapsuleRenderEngine.GetImpliedStyle(gH_Palette, Selected, base.Owner.Locked, base.Owner.Hidden);
        RenderComponentParametersNew(canvas, graphics, Owner, impliedStyle);
    }

    private static void RenderComponentParametersNew(GH_Canvas canvas, Graphics graphics, IGH_Component owner, GH_PaletteStyle style)
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
                    //Render Icon;
                    if (Data.ShowLinkParamIcon)
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
                        (Data.ComponentInputEdgeLayout ? GH_TextRenderingConstants.NearCenter : GH_TextRenderingConstants.FarCenter) :
                        (Data.ComponentOutputEdgeLayout ? GH_TextRenderingConstants.FarCenter : GH_TextRenderingConstants.NearCenter);

                    graphics.DrawString(item.NickName, GH_FontServer.StandardAdjusted, solidBrush, bounds, format);
                    GH_LinkedParamAttributes gH_LinkedParamAttributes = (GH_LinkedParamAttributes)item.Attributes;

                    GH_StateTagList tags = (GH_StateTagList)_tagsInfo.GetValue(gH_LinkedParamAttributes);
                    tags?.RenderStateTags(graphics);
                }
            }
        }
        solidBrush.Dispose();
    }
}
