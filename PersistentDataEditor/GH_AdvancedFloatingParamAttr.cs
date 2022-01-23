using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Grasshopper;
using Rhino;
using Grasshopper.Kernel.Types;

namespace PersistentDataEditor
{
    internal class GH_AdvancedFloatingParamAttr : GH_FloatingParamAttributes, IControlAttr, IDisposable
    {
        private static FieldInfo _tagsinfo = typeof(GH_FloatingParamAttributes).GetRuntimeFields().Where(m => m.Name.Contains("m_stateTags")).First();

        private IGumball _gumball;
        private Rectangle _iconTextBound;

        public BaseControlItem Control { get; private set; } = null;
        public GH_AdvancedFloatingParamAttr(IGH_Param param): base(param)
        {
            _gumball = SetGumball(param);
            if (_gumball != null)
            {
                param.SolutionExpired += Param_SolutionExpired;
            }
            SetControl();
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

        private static bool IsGeometricGoo(Type type)
        {
            if (type == null)
            {
                return false;
            }
            else if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(GH_GeometricGoo<>))
                {
                    return true;
                }
                else if (type.GetGenericTypeDefinition() == typeof(GH_Goo<>))
                    return false;
            }
            return IsGeometricGoo(type.BaseType);
        }

        private void Param_SolutionExpired(IGH_DocumentObject sender, GH_SolutionExpiredEventArgs e)
        {
            if (base.Owner == null)
            {
                Owner.SolutionExpired -= Param_SolutionExpired;
            }
            else if (base.Owner.OnPingDocument() == null)
            {
                Owner.SolutionExpired -= Param_SolutionExpired;
            }
            if (_gumball != null && !_gumball.IsMouseUp)
                _gumball.ShowAllGumballs();
        }

        public void SetControl()
        {
            if (Datas.UseParamControl && Datas.ParamUseControl)
            {
                Control = GH_AdvancedLinkParamAttr.GetControl(Owner);
            }
            else
            {
                Control = null;
            }
        }

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {

            if (Control != null && Control.Bounds.Contains(e.CanvasLocation) && sender.Viewport.Zoom >= 0.6)
            {
                Control.Clicked(sender, e);

                return GH_ObjectResponse.Release;
            }
            return base.RespondToMouseUp(sender, e);
        }

        protected override void Layout()
        {
            int minWidth = 50;
            int edgeDistance = Datas.ParamsEdgeDistance;
            int controlDis = Datas.ParamsCoreDistance;

            //Get Icon/Text Bound.
            if (IsIconMode(base.Owner.IconDisplayMode))
            {
                _iconTextBound = new Rectangle((int)Pivot.X - 12, (int)Pivot.Y - 12, 24, 24);
            }
            else
            {
                float stringWidth = GH_FontServer.MeasureString(base.Owner.NickName, GH_FontServer.StandardAdjusted).Width + 1;
                _iconTextBound = new Rectangle((int)(Pivot.X - stringWidth/2), (int)Pivot.Y - 10, (int)stringWidth, 20);
            }
            Bounds = _iconTextBound;

            GH_StateTagList tags = Owner.StateTags;
            if (tags.Count == 0) tags = null;

            //Get Control Bound.
            if (Control != null && Control.Width > 0)
            {
                float controlWidth = Control.Width;
                float controlHeight = Control.Height;
                Control.Bounds = new RectangleF(_iconTextBound.X - controlWidth - controlDis, _iconTextBound.Y + _iconTextBound.Height / 2 - controlHeight / 2,
                    controlWidth, controlHeight);

                controlHeight += 3;
                Bounds = GH_Convert.ToRectangle(RectangleF.Union(Bounds, new RectangleF(_iconTextBound.X - controlWidth - controlDis, _iconTextBound.Y + _iconTextBound.Height / 2 - controlHeight / 2,
                    controlWidth, controlHeight)));
            }
            else if(tags != null)
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
                    if (base.Owner.SourceCount > 0)
                    {
                        RenderIncomingWires(canvas.Painter, base.Owner.Sources, base.Owner.WireDisplay);
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
                        if (base.Owner is IGH_PreviewObject)
                        {
                            hidden = ((IGH_PreviewObject)base.Owner).Hidden;
                        }
                        GH_Capsule gH_Capsule = null;
                        gH_Capsule = ((!IsIconMode(base.Owner.IconDisplayMode)) ? GH_Capsule.CreateTextCapsule(Bounds, _iconTextBound, GH_CapsuleRenderEngine.GetImpliedPalette(base.Owner), base.Owner.NickName) : 
                            GH_Capsule.CreateCapsule(Bounds, GH_CapsuleRenderEngine.GetImpliedPalette(base.Owner)));
                        if (HasInputGrip)
                        {
                            gH_Capsule.AddInputGrip(InputGrip.Y);
                        }
                        if (HasOutputGrip)
                        {
                            gH_Capsule.AddOutputGrip(OutputGrip.Y);
                        }
                        if (IsIconMode(base.Owner.IconDisplayMode))
                        {
                            if (base.Owner.Locked)
                            {
                                gH_Capsule.Render(graphics, Selected, locked: true, hidden);
                                gH_Capsule.RenderEngine.RenderIcon(graphics, base.Owner.Icon_24x24_Locked, _iconTextBound, 0, 1);
                            }
                            else
                            {
                                gH_Capsule.Render(graphics, Selected, locked: false, hidden);
                                gH_Capsule.RenderEngine.RenderIcon(graphics, base.Owner.Icon_24x24, _iconTextBound, 0, 1);
                            }
                            if (base.Owner.Obsolete && CentralSettings.CanvasObsoleteTags)
                            {
                                GH_GraphicsUtil.RenderObjectOverlay(graphics, base.Owner, _iconTextBound);
                            }
                        }
                        else
                        {
                            gH_Capsule.Render(graphics, Selected, base.Owner.Locked, hidden);
                        }
                        gH_Capsule.Dispose();

                        GH_Palette gH_Palette = GH_CapsuleRenderEngine.GetImpliedPalette(base.Owner);
                        GH_PaletteStyle impliedStyle = GH_CapsuleRenderEngine.GetImpliedStyle(gH_Palette, Selected, base.Owner.Locked, hidden);
                        Control?.RenderObject(canvas, graphics, null, impliedStyle);

                        GH_StateTagList tags = (GH_StateTagList)_tagsinfo.GetValue(this);
                        if (tags != null)
                        {
                            tags.RenderStateTags(graphics);
                        }
                        break;
                    }
            }
        }

        public void Dispose()
        {
            if(_gumball != null)
            {
                _gumball.Dispose();
            }
        }

        public void RedrawGumballs()
        {
            if (_gumball != null)
            {
                _gumball.ShowAllGumballs();
            }
        }
    }
}
