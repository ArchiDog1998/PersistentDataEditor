using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace PersistentDataEditor;

internal class GH_AdvancedNumberSliderAttr : GH_NumberSliderAttributes, IControlAttr
{
    private static readonly FieldInfo _nameBoundInfo = typeof(GH_NumberSliderAttributes).FindField("m_boundsName");
    private static readonly FieldInfo _sliderBoundInfo = typeof(GH_NumberSliderAttributes).FindField("m_boundsSlider");

    private Rectangle _controlBounds;
    private int _width;

    private BaseControlItem[] _controlItems;
    public GH_AdvancedNumberSliderAttr(GH_NumberSlider nOwner) : base(nOwner)
    {
        SetControl();
    }

    public void SetControl()
    {
        if (NewData.UseParamNumberSliderControl && NewData.UseParamControl && NewData.ParamUseControl)
        {
            _controlItems =
            [
                new StringRender("m"),

                new GooInputBoxStringControl<GH_Number>
                    (()=> new GH_Number((double)Owner.Slider.Minimum), () => false) 
                    { ValueChange = SetValue},

                new StringRender("M"),

                new GooInputBoxStringControl<GH_Number>
                    (()=> new GH_Number((double)Owner.Slider.Maximum),  () => false)
                    { ValueChange = SetValue},

                new StringRender("D"),

                new GooInputBoxStringControl<GH_Integer>
                    (()=> new GH_Integer(Owner.Slider.DecimalPlaces),  () => false) 
                    { ValueChange = SetValue},
            ];
        }
        else
        {
            _controlItems = [];
        }
    }

    private void SetValue()
    {
        Owner.Slider.Minimum = (decimal)((GH_Number)((IGooValue)_controlItems[1]).SaveValue).Value;
        Owner.Slider.Maximum = (decimal)((GH_Number)((IGooValue)_controlItems[3]).SaveValue).Value;
        Owner.Slider.DecimalPlaces = ((GH_Integer)((IGooValue)_controlItems[5]).SaveValue).Value;

        Owner.ExpireSolution(true);
    }

    protected override void Layout()
    {
        int controlDis = NewData.ParamsCoreDistance;

        _width = 0;
        if(_controlItems != null && _controlItems.Length > 0 && 
            (!NewData.OnlyShowSelectedObjectControl || Owner.Attributes.Selected))
        {
            _width += controlDis * 2+ _controlItems.Sum(t=>t.Width);
        }

        Bounds = new Rectangle((int)Bounds.X, (int)Bounds.Y, (int)Bounds.Width - _width, (int)Bounds.Height);
        base.Layout();
        Rectangle nameRect = (Rectangle)_nameBoundInfo.GetValue(this);
        nameRect = new Rectangle(nameRect.X, nameRect.Y, nameRect.Width, nameRect.Height);
        _nameBoundInfo.SetValue(this, nameRect);

        Rectangle sliderRect = (Rectangle)_sliderBoundInfo.GetValue(this);
        sliderRect = new Rectangle(sliderRect.X + _width, sliderRect.Y, sliderRect.Width, sliderRect.Height);
        _sliderBoundInfo.SetValue(this, sliderRect);
        Owner.Slider.Bounds = sliderRect;

        if (_width > 0)
        {
            _controlBounds = Rectangle.FromLTRB(nameRect.Right, nameRect.Top, sliderRect.Left, sliderRect.Bottom);
            float x = _controlBounds.X + controlDis;
            foreach (BaseControlItem item in _controlItems)
            {
                item.Bounds = new RectangleF(x, _controlBounds.Y + (_controlBounds.Height - item.Height) / 2, item.Width, item.Height);
                x += item.Width;
            }
        }
        else
            _controlBounds = Rectangle.Empty;

        Bounds = new Rectangle((int)Bounds.X, (int)Bounds.Y, (int)Bounds.Width + _width, (int)Bounds.Height);
    }

    protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
    {
        base.Render(canvas, graphics, channel);
        if (channel != GH_CanvasChannel.Objects)
        {
            return;
        }

        if (_controlBounds == Rectangle.Empty) return;
        GH_Capsule gH_Capsule;
        switch (Owner.RuntimeMessageLevel)
        {
            case GH_RuntimeMessageLevel.Warning:
                gH_Capsule = GH_Capsule.CreateCapsule(_controlBounds, GH_Palette.Warning, 0, 5);
                break;
            case GH_RuntimeMessageLevel.Error:
                gH_Capsule = GH_Capsule.CreateCapsule(_controlBounds, GH_Palette.Error, 0, 5);
                break;
            default:
                gH_Capsule = GH_Capsule.CreateCapsule(_controlBounds, GH_Palette.Hidden, 0, 5);
                break;
        };
        gH_Capsule.Render(graphics, Selected, Owner.Locked, hidden: true);
        gH_Capsule.Dispose();

        GH_Palette gH_Palette = GH_CapsuleRenderEngine.GetImpliedPalette(Owner);
        GH_PaletteStyle impliedStyle = GH_CapsuleRenderEngine.GetImpliedStyle(gH_Palette, Selected, Owner.Locked, true);
        foreach (var item in _controlItems)
        {
            item?.RenderObject(canvas, graphics, impliedStyle);
        }
    }

    public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        if (e.Button == MouseButtons.Left && _controlItems != null)
        {
            foreach (var item in _controlItems)
            {
                if (item.Bounds.Contains(e.CanvasLocation))
                {
                    item.Clicked(sender, e);
                    return GH_ObjectResponse.Release;
                }
            }
        }
        return base.RespondToMouseUp(sender, e);
    }
}
