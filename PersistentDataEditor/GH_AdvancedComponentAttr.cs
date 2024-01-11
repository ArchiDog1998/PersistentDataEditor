using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace PersistentDataEditor;

internal class GH_AdvancedComponentAttr : GH_ComponentAttributes
{
    private bool _isSimplify = false;
    private const float SimplifyHeight = 10;

    public override bool Selected 
    { 
        get => base.Selected;
        set
        {
            if (base.Selected == value) return;
            base.Selected = value;

            foreach (var item in Owner.Params.Input)
            {
                if (item.Attributes is GH_AdvancedLinkParamAttr attr)
                {
                    if (!Data.ShowedGumball && value 
                        && item.SourceCount == 0 && !Owner.Hidden)
                    {
                        Data.ShowedGumball = true;
                        //Open gumball.
                        attr.ShowAllGumballs();
                    }
                    else
                    {
                        Data.ShowedGumball = false;
                        //Close gumball
                        attr.DisposeGumball();
                    }
                }
            }

            if (Data.OnlyShowSelectedObjectControl)
            {
                this.ExpireLayout();
            }
        }
    }

    public GH_AdvancedComponentAttr(IGH_Component component)
        : base(component)
    {
        _isSimplify = Owner.OnPingDocument().ValueTable.GetValue(nameof(_isSimplify) + Owner.InstanceGuid, false);
    }

    #region Layout
    private static readonly FieldInfo _tagsInfo = typeof(GH_LinkedParamAttributes).FindField("m_renderTags");
    protected override void Layout()
    {
        Pivot = GH_Convert.ToPoint(Pivot);
        m_innerBounds = LayoutComponentBoxNew(Owner);
        ParamsLayoutNew(Owner, m_innerBounds, true);
        ParamsLayoutNew(Owner, m_innerBounds, false);
        Bounds = LayoutBounds(Owner, m_innerBounds);

        var bound = Bounds;
        bound.Inflate(Data.ComponentToEdgeDistance - 2, 0);
        Bounds = bound;

        RectangleF LayoutComponentBoxNew(IGH_Component owner)
        {
            float inputHeight = GetParamsWholeHeight(owner.Params.Input, owner);
            float outputHeight = GetParamsWholeHeight(owner.Params.Output, owner);

            float val = Math.Max(inputHeight, outputHeight);
            val = Math.Max(val, 24);
            int num = 24;
            if (!IsIconMode(owner.IconDisplayMode))
            {
                val = Math.Max(val, GH_Convert.ToSize(GH_FontServer.MeasureString(owner.NickName, GH_FontServer.LargeAdjusted)).Width + 6);
            }
            return GH_Convert.ToRectangle(new RectangleF(owner.Attributes.Pivot.X - 0.5f * num, owner.Attributes.Pivot.Y - 0.5f * val, num, val));

            float GetParamsWholeHeight(List<IGH_Param> gH_Params, IGH_Component owner)
            {
                float wholeHeight = 0;
                foreach (IGH_Param param in gH_Params)
                {
                    if (param.Attributes == null || param.Attributes is not GH_AdvancedLinkParamAttr)
                    {
                        param.Attributes = new GH_AdvancedLinkParamAttr(param, owner.Attributes);

                        //Refresh the attributes.
                        owner.OnPingDocument()?.DestroyAttributeCache();
                    }
                    GH_AdvancedLinkParamAttr attr = (GH_AdvancedLinkParamAttr)param.Attributes;
                    wholeHeight += _isSimplify ? SimplifyHeight : attr.ParamHeight;
                }
                return wholeHeight;
            }
        }
    }

    private void ParamsLayoutNew(IGH_Component owner, RectangleF componentBox, bool isInput)
    {
        List<IGH_Param> gH_Params = isInput ? owner.Params.Input : owner.Params.Output;

        int count = gH_Params.Count;
        if (count == 0) return;

        var attributes = GetOrCreateAdvancedLinkParamAttr(owner, gH_Params);
        LayoutTextBounds(componentBox, attributes, isInput, _isSimplify);

        if (_isSimplify) return;

        if (isInput)  // Control Bounds.
        {
            LayoutControl(attributes);
        }

        //Layout Tags
        LayoutTags(gH_Params, attributes, isInput);

        if (Data.ShowLinkParamIcon)//Set Icon Rect.
        {
            LayoutParamIcon(attributes, isInput);
        }
    }

    private static void LayoutParamIcon(IEnumerable<GH_AdvancedLinkParamAttr> attributes, bool isInput)
    {
        var width = Data.ComponentParamIconSize;
        var distance = Data.ComponentIconDistance;

        foreach (var attribute in attributes)
        {
            attribute.IconRect = new RectangleF(new PointF(
                isInput ? attribute.Bounds.Left - width - distance : attribute.Bounds.Right + distance,
                attribute.Bounds.Location.Y),
                new SizeF(width, attribute.ParamHeight));

            attribute.Bounds = RectangleF.Union(attribute.Bounds, attribute.IconRect);
        }
    }

    private static void LayoutTags(List<IGH_Param> gH_Params, IEnumerable<GH_AdvancedLinkParamAttr> attributes, bool isInput)
    {
        CreateLayoutTags();
        AlignBounds();

        void CreateLayoutTags()
        {
            foreach (IGH_Param param in gH_Params) // From GH code source.
            {
                GH_LinkedParamAttributes paramAttr = (GH_LinkedParamAttributes)param.Attributes;

                GH_StateTagList tags = param.StateTags;
                if (tags.Count == 0) tags = null;
                _tagsInfo.SetValue(paramAttr, tags);

                if (tags == null) continue;

                Rectangle box = GH_Convert.ToRectangle(paramAttr.Bounds);
                tags.Layout(box, isInput ? GH_StateTagLayoutDirection.Left : GH_StateTagLayoutDirection.Right);
                box = tags.BoundingBox;
                if (!box.IsEmpty)
                {
                    paramAttr.Bounds = RectangleF.Union(paramAttr.Bounds, box);
                }
            }
        }

        void AlignBounds()
        {
            float minX = float.MaxValue, maxX = float.MinValue;
            foreach (var attribute in attributes)
            {
                minX = Math.Min(minX, attribute.Bounds.Left);
                maxX = Math.Max(maxX, attribute.Bounds.Right);
            }
            foreach (var attribute in attributes)
            {
                attribute.Bounds = RectangleF.FromLTRB(minX, attribute.Bounds.Top, maxX, attribute.Bounds.Bottom);
            }
        }
    }

    private static void LayoutControl(IEnumerable<GH_AdvancedLinkParamAttr> attributes)
    {
        CreateControlBounds();
        if (Data.SeperateCalculateWidthControl)
        {
            MoveControlRight();
        }
        AlignControlLeft();
        UnionLayoutBounds();

        void CreateControlBounds()
        {
            foreach (var attribute in attributes)
            {
                if (attribute.Control is null) continue;
                attribute.Control.Bounds = new RectangleF(new PointF(attribute.Bounds.X - attribute.ControlMinWidth - Data.ComponentControlNameDistance, attribute.Bounds.Y), new SizeF(attribute.ControlMinWidth, attribute.ParamHeight));
            }
        }

        void MoveControlRight()
        {
            var minX = float.MaxValue;
            foreach (var attribute in attributes)
            {
                if (attribute.Control is null || attribute.Control.Bounds.Width < 1f) continue;
                minX = Math.Min(minX, attribute.Control.Bounds.Right);
            }
            foreach (var attribute in attributes)
            {
                if (attribute.Control is null) continue;

                var moveX = minX - attribute.Control.Bounds.Right;

                attribute.Control.Bounds = new RectangleF(
                    new PointF(attribute.Control.Bounds.Location.X + moveX,
                    attribute.Control.Bounds.Location.Y),
                    attribute.Control.Bounds.Size);
            }
        }

        void AlignControlLeft()
        {
            float minX = float.MaxValue;
            foreach (var attribute in attributes)
            {
                if (attribute.Control is null || attribute.Control.Bounds.Width < 1f) continue;
                minX = Math.Min(minX, attribute.Control.Bounds.Left);
            }
            foreach (var attribute in attributes)
            {
                if (attribute.Control is null) continue;
                attribute.Control.Bounds = RectangleF.FromLTRB(minX, attribute.Control.Bounds.Top,
                    attribute.Control.Bounds.Right, attribute.Control.Bounds.Bottom);
            }
        }

        void UnionLayoutBounds()
        {
            foreach (var attribute in attributes)
            {
                if (attribute.Control is null || attribute.Control.Bounds.Width < 1f) continue;
                attribute.Bounds = RectangleF.Union(attribute.Bounds, attribute.Control.Bounds);
            }
        }
    }

    private static void LayoutTextBounds(RectangleF componentBox, IEnumerable<GH_AdvancedLinkParamAttr> attributes, bool isInput, bool isSimplify)
    {
        var heightRatio = componentBox.Height / attributes.Sum(a => isSimplify ? 10 : a.ParamHeight);

        var startX = componentBox.Location.X;
        var startY = componentBox.Location.Y;
        if (!isInput)
        {
            startX += componentBox.Width;
        }
        foreach (var attribute in attributes)
        {
            var height = heightRatio * (isSimplify ? 10 : attribute.ParamHeight);

            attribute.Bounds = new RectangleF(new PointF(isInput ? startX - (isSimplify ? 0 : attribute.StringWidth): startX, startY),
                new SizeF(isSimplify ? 0 : attribute.StringWidth, height));

            startY += height;
        }
    }

    private static IEnumerable<GH_AdvancedLinkParamAttr> GetOrCreateAdvancedLinkParamAttr(IGH_Component owner, IEnumerable<IGH_Param> parameters)
    {
        return parameters.Select(param =>
        {
            if (param.Attributes is not GH_AdvancedLinkParamAttr)
            {
                param.Attributes = new GH_AdvancedLinkParamAttr(param, owner.Attributes);

                //Refresh the attributes.
                owner.OnPingDocument()?.DestroyAttributeCache();
            }
            return (GH_AdvancedLinkParamAttr)param.Attributes;
        });
    }
    #endregion

    #region Render
    protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
    {
        base.Render(canvas, graphics, channel);
        if (channel != GH_CanvasChannel.Objects || _isSimplify) return;

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
            if (bounds.Width >= 1f)
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
                        GH_TextRenderingConstants.FarCenter :
                        GH_TextRenderingConstants.NearCenter;

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

    public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        if (m_innerBounds.Contains(e.CanvasLocation))
        {
            _isSimplify = !_isSimplify;
            this.ExpireLayout();
            sender.Refresh();
            Owner.OnPingDocument().ValueTable.SetValue(nameof(_isSimplify) + Owner.InstanceGuid, _isSimplify);
            return GH_ObjectResponse.Release;
        }
        return base.RespondToMouseDoubleClick(sender, e);
    }
}
