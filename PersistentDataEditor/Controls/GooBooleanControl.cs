using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel.Types;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PersistentDataEditor;

internal class GooBooleanControl(Func<GH_Boolean> valueGetter, Func<bool> isNull)
    : GooControlBase<GH_Boolean>(valueGetter, isNull)
{
    public override Guid AddComponentGuid => new("2e78987b-9dfb-42a2-8b76-3923ac8bd91a");

    internal override float MinWidth => 10;

    internal override float Height => 10;

    internal override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        if (e.Button == MouseButtons.Left)
        {
            bool lastBool = ShowValue?.Value ?? false;
            ShowValue = new GH_Boolean(!lastBool);
            return;
        }
        base.Clicked(sender, e);
    }

    protected override void OnLayoutChanged(RectangleF bounds)
    {
        base.OnLayoutChanged(bounds);
        var width = (bounds.Height - bounds.Width) / 2;
        if (width != 0)
        {
            bounds.Inflate(width, 0);
            Bounds = bounds;
        }
    }

    internal override void RenderObject(GH_Canvas canvas, Graphics graphics, GH_PaletteStyle style)
    {
        graphics.FillEllipse(Data.ControlBackgroundColor.GetBrush(), Bounds);
        graphics.DrawEllipse(new Pen(Data.ControlBorderColor.GetBrush(), 1.5f), Bounds);

        if (ShowValue == null || !ShowValue.Value) return;
        RectangleF bound = Bounds;
        bound.Inflate(-2, -2);

        graphics.FillEllipse(Data.ControlForegroundColor.GetBrush(), bound);
    }
}
