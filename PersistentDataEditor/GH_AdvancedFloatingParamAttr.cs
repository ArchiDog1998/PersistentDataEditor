using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using System;
using System.Drawing;
using System.Reflection;

namespace PersistentDataEditor;

internal class GH_AdvancedFloatingParamAttr : GH_FloatingParamAttributes, IControlAttr
{
    private static readonly FieldInfo _tagsinfo = typeof(GH_FloatingParamAttributes).FindField("m_stateTags");

    private readonly IGumball _gumball;
    private Rectangle _iconTextBound;

    private readonly MethodInfo _expressionInfo;

    public BaseControlItem Control { get; private set; }

    public override bool Selected 
    { 
        get => base.Selected;
        set
        {
            if (base.Selected == value) return;
            base.Selected = value;

            if (value)
            {
                //Open gumball.
                this.RedrawGumballs();
            }
            else
            {
                //Close gumball
                this.DisposeGumballs();
            }

            if (Data.OnlyShowSelectedObjectControl)
            {
                this.ExpireLayout();
            }
        }
    }

    public GH_AdvancedFloatingParamAttr(IGH_Param param) : base(param)
    {
        _gumball = SetGumball(param);
        SetControl();

        if (GH_AdvancedLinkParamAttr.IsExpressionParam(Owner.GetType(), out Type dataType))
        {
            Type type = typeof(GH_ExpressionParam<>).MakeGenericType(dataType);
            _expressionInfo = type.FindMethod("Menu_ExpressionEditorClick");
        }
    }

    internal static IGumball SetGumball(IGH_Param param)
    {
        IGumball gumball = null;
        if (GH_AdvancedLinkParamAttr.IsPersistentParam(param.GetType(), out Type dataType)
            && typeof(IGH_GeometricGoo).IsAssignableFrom(dataType))
        {
            Type controlType = typeof(GumballMouse<>).MakeGenericType(dataType);
            gumball = (IGumball)Activator.CreateInstance(controlType, param);
        }
        return gumball;
    }

    public void SetControl()
    {
        if (Data.UseParamControl && Data.ParamUseControl)
        {
            Control = GH_AdvancedLinkParamAttr.GetControl(Owner);
        }
        else
        {
            Control = null;
        }
    }

    public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        return Control?.Respond(Control.MouseMove, sender, e) ?? base.RespondToMouseMove(sender, e);
    }

    public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        return Control?.Respond(Control.MouseDown, sender, e) ?? base.RespondToMouseDown(sender, e);
    }

    public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        return Control?.Respond(Control.Clicked, sender, e) ?? base.RespondToMouseUp(sender, e);
    }

    public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        if (_expressionInfo != null)
        {
            _expressionInfo.Invoke(Owner, [Instances.DocumentEditor, EventArgs.Empty]);
            return GH_ObjectResponse.Release;
        }
        return base.RespondToMouseDoubleClick(sender, e);
    }

    protected override void Layout()
    {
        int minWidth = 50;
        int edgeDistance = Data.ParamsEdgeDistance;
        int controlDis = Data.ParamsCoreDistance;

        //Get Icon/Text Bound.
        if (IsIconMode(Owner.IconDisplayMode))
        {
            _iconTextBound = new Rectangle((int)Pivot.X - 12, (int)Pivot.Y - 12, 24, 24);
        }
        else
        {
            float stringWidth = GH_FontServer.MeasureString(Owner.NickName, GH_FontServer.StandardAdjusted).Width + 1;
            _iconTextBound = new Rectangle((int)(Pivot.X - stringWidth / 2), (int)Pivot.Y - 10, (int)stringWidth, 20);
        }
        Bounds = _iconTextBound;

        GH_StateTagList tags = Owner.StateTags;
        if (tags.Count == 0) tags = null;

        //Get Control Bound.
        if (Control != null && Control.MinWidth > 0)
        {
            float controlWidth = Control.MinWidth;
            float controlHeight = Control.Height;
            Control.Bounds = new RectangleF(_iconTextBound.X - controlWidth - controlDis, _iconTextBound.Y + _iconTextBound.Height / 2 - controlHeight / 2,
                controlWidth, controlHeight);

            controlHeight += 3;
            Bounds = GH_Convert.ToRectangle(RectangleF.Union(Bounds, new RectangleF(_iconTextBound.X - controlWidth - controlDis, _iconTextBound.Y + _iconTextBound.Height / 2 - controlHeight / 2,
                controlWidth, controlHeight)));
        }
        else if (tags != null)
        {
            Bounds = new Rectangle((int)Bounds.X - controlDis, (int)Bounds.Y, (int)Bounds.Width + controlDis, (int)Bounds.Height);
        }

        //Set tags layout.

        if (tags != null)
        {
            tags.Layout(GH_Convert.ToRectangle(Bounds), GH_StateTagLayoutDirection.Left);
            Rectangle boundingBox = tags.BoundingBox;
            if (!boundingBox.IsEmpty)
            {
                Bounds = GH_Convert.ToRectangle(RectangleF.Union(Bounds, boundingBox));
            }
        }
        _tagsinfo.SetValue(this, tags);

        if (Bounds.Width + 2 * edgeDistance < minWidth) edgeDistance = (minWidth - (int)Bounds.Width) / 2;
        Bounds = new Rectangle((int)Bounds.X - edgeDistance, (int)Bounds.Y, (int)Bounds.Width + edgeDistance * 2, (int)Bounds.Height);

    }

    protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
    {
        switch (channel)
        {
            case GH_CanvasChannel.Wires:
                if (Owner.SourceCount > 0)
                {
                    RenderIncomingWires(canvas.Painter, Owner.Sources, Owner.WireDisplay);
                }
                break;

            case GH_CanvasChannel.Objects:
                {
                    //Check should render.
                    GH_Viewport viewport = canvas.Viewport;
                    RectangleF rec = Bounds;
                    if (!viewport.IsVisible(ref rec, 10f)) break;
                    Bounds = rec;

                    bool hidden = true;
                    if (Owner is IGH_PreviewObject previewObject)
                    {
                        hidden = previewObject.Hidden;
                    }
                    GH_Capsule gH_Capsule = !IsIconMode(Owner.IconDisplayMode) ? GH_Capsule.CreateTextCapsule(Bounds, _iconTextBound, GH_CapsuleRenderEngine.GetImpliedPalette(Owner), Owner.NickName) :
                        GH_Capsule.CreateCapsule(Bounds, GH_CapsuleRenderEngine.GetImpliedPalette(Owner));
                    if (HasInputGrip)
                    {
                        gH_Capsule.AddInputGrip(InputGrip.Y);
                    }
                    if (HasOutputGrip)
                    {
                        gH_Capsule.AddOutputGrip(OutputGrip.Y);
                    }
                    if (IsIconMode(Owner.IconDisplayMode))
                    {
                        if (Owner.Locked)
                        {
                            gH_Capsule.Render(graphics, Selected, locked: true, hidden);
                            gH_Capsule.RenderEngine.RenderIcon(graphics, Owner.Icon_24x24_Locked, _iconTextBound, 0, 1);
                        }
                        else
                        {
                            gH_Capsule.Render(graphics, Selected, locked: false, hidden);
                            gH_Capsule.RenderEngine.RenderIcon(graphics, Owner.Icon_24x24, _iconTextBound, 0, 1);
                        }
                        if (Owner.Obsolete && CentralSettings.CanvasObsoleteTags)
                        {
                            GH_GraphicsUtil.RenderObjectOverlay(graphics, Owner, _iconTextBound);
                        }
                    }
                    else
                    {
                        gH_Capsule.Render(graphics, Selected, Owner.Locked, hidden);
                    }
                    gH_Capsule.Dispose();

                    GH_Palette gH_Palette = GH_CapsuleRenderEngine.GetImpliedPalette(Owner);
                    GH_PaletteStyle impliedStyle = GH_CapsuleRenderEngine.GetImpliedStyle(gH_Palette, Selected, Owner.Locked, hidden);
                    Control?.RenderObject(canvas, graphics, impliedStyle);

                    GH_StateTagList tags = (GH_StateTagList)_tagsinfo.GetValue(this);
                    tags?.RenderStateTags(graphics);
                    break;
                }
        }
    }

    public void DisposeGumballs()
    {
        _gumball?.Dispose();
    }

    public void RedrawGumballs()
    {
        _gumball?.ShowAllGumballs();
    }
}
