using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PersistentDataEditor.Controls;

internal class GooEnumControl(Func<GH_Integer> valueGetter, Func<bool> isNull, SortedList<int, string> namedValues) : GooControlBase<GH_Integer>(valueGetter, isNull)
{
    public override Guid AddComponentGuid => new("00027467-0D24-4fa7-B178-8DC0AC5F42EC");

    private string ShowString
    {
        get
        {
            if (ShowValue == null)
            {
                return null;
            }

            int index = ShowValue.Value;
            return _namedValues.TryGetValue(index, out var value) ? value : index.ToString();
        }
    }
    internal override float Height => 14;

    private const int _triangleWidth = 14;
    internal override float MinWidth => Math.Min(Math.Max(GH_FontServer.StringWidth(ShowString, GH_FontServer.StandardAdjusted), 15), Data.InputBoxControlMaxWidth) + _triangleWidth;

    private readonly SortedList<int, string> _namedValues = namedValues;

    private GraphicsPath _roundBounds;
    private RectangleF _stringBounds;
    private GraphicsPath _triangle;

    internal override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        if (e.Button == MouseButtons.Left)
        {
            ToolStripDropDownMenu menu = new ToolStripDropDownMenu();
            int? num = ShowValue?.Value;
            foreach (var namedValue in _namedValues)
            {
                GH_DocumentObject.Menu_AppendItem(menu, namedValue.Value, Menu_NamedValueClicked, true, namedValue.Key == num).Tag = namedValue.Key;
            }
            menu.Show(sender, e.ControlLocation);
        }
        base.Clicked(sender, e);
    }

    private void Menu_NamedValueClicked(object sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem toolStripMenuItem && toolStripMenuItem.Tag is int tag)
        {
            ShowValue = new GH_Integer(tag);
        }
    }

    protected override void OnLayoutChanged(RectangleF bounds)
    {
        _roundBounds = GH_Convert.ToRectangle(bounds).RoundCorner(2);
        _stringBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width - _triangleWidth, bounds.Height);

        RectangleF triBounds = new RectangleF(bounds.Right - _triangleWidth, bounds.Y, _triangleWidth, bounds.Height);
        _triangle = triBounds.Triangle();

        base.OnLayoutChanged(bounds);
    }

    internal override void RenderObject(GH_Canvas canvas, Graphics graphics, GH_PaletteStyle style)
    {
        graphics.FillPath(Data.ControlBackgroundColor.GetBrush(), _roundBounds);
        graphics.DrawPath(new Pen(Data.ControlBorderColor.GetBrush()), _roundBounds);
        graphics.DrawString(ShowString, GH_FontServer.StandardAdjusted, Data.ControlTextgroundColor.GetBrush(), _stringBounds, GH_TextRenderingConstants.NearCenter);

        graphics.FillPath(Data.ControlForegroundColor.GetBrush(), _triangle);
    }

    public override void DoSomethingWhenCreate(IGH_DocumentObject obj)
    {
        if (_namedValues == null) return;

        GH_ValueList valuelist = (GH_ValueList)obj;
        if (valuelist == null) return;

        valuelist.ListItems.Clear();
        foreach (var keyvalue in _namedValues)
        {
            valuelist.ListItems.Add(new GH_ValueListItem(keyvalue.Value, keyvalue.Key.ToString()));
        }
    }
}
