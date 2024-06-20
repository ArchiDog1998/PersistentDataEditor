using Grasshopper.GUI;
using Grasshopper.GUI.Base;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using HarmonyLib;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

namespace PersistentDataEditor.Controls;

internal class GooColorControl(Func<GH_Colour> valueGetter, Func<bool> isNull) : GooControlBase<GH_Colour>(valueGetter, isNull)
{
    internal static readonly FieldInfo SwatchColorInfo = AccessTools.Field(typeof(GH_ColourSwatch), "m_SwatchColour");
    private readonly Brush _background = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.White, Color.LightGray);
    private GraphicsPath _path;

    public override Guid AddComponentGuid => new("9c53bac0-ba66-40bd-8154-ce9829b9db1a");

    internal override float MinWidth => 12;

    internal override float Height => 12;

    internal override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        if (e.Button == MouseButtons.Left)
        {
            var menu = new ToolStripDropDownMenu() { ShowImageMargin = false };

            Color color = ShowValue?.Value ?? Color.Transparent;
            GH_DocumentObject.Menu_AppendColourPicker(menu, color, ColourChanged);
            menu.Show(sender, e.ControlLocation);
        }
        base.Clicked(sender, e);

        void ColourChanged(GH_ColourPicker sender1, GH_ColourPickerEventArgs e1)
        {
            ShowValue = new GH_Colour(e1.Colour);
        }
    }

    protected override void OnLayoutChanged(RectangleF bounds)
    {
        _path = GH_Convert.ToRectangle(Bounds).RoundCorner(2);
        base.OnLayoutChanged(bounds);
    }

    internal override void RenderObject(GH_Canvas canvas, Graphics graphics, GH_PaletteStyle style)
    {
        graphics.FillPath(_background, _path);

        if (ShowValue != null)
            graphics.FillPath(new SolidBrush(ShowValue.Value), _path);

        graphics.DrawPath(new Pen(new SolidBrush(Data.ControlBorderColor)), _path);
    }

    public override void DoSomethingWhenCreate(IGH_DocumentObject obj)
    {
        GH_ColourSwatch swatch = (GH_ColourSwatch)obj;
        if (swatch == null) return;
        if (ShowValue != null)
            SwatchColorInfo.SetValue(swatch, ShowValue.Value);
    }
}
