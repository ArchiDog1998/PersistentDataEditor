using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace PersistentDataEditor.Controls;
internal class GooTimeControl(Func<GH_Time> valueGetter, Func<bool> isNull) : GooControlBase<GH_Time>(valueGetter, isNull)
{
    public override Guid AddComponentGuid => new("0c2f0932-5ddc-4ece-bd84-a3a059d3df7a");
    private GraphicsPath _roundRect;
    private string ShowString => ShowValue?.ToString();

    internal override float Height => 14;
    internal override float MinWidth => Math.Min(Math.Max(GH_FontServer.StringWidth(ShowString, GH_FontServer.StandardAdjusted), 15),
        Data.InputBoxControlMaxWidth);
    protected override void OnLayoutChanged(RectangleF bounds)
    {
        _roundRect = GH_Convert.ToRectangle(bounds).RoundCorner(2);
        base.OnLayoutChanged(bounds);
    }
    internal override void RenderObject(GH_Canvas canvas, Graphics graphics, GH_PaletteStyle style)
    {
        graphics.FillPath(Data.ControlBackgroundColor.GetBrush(), _roundRect);
        graphics.DrawPath(new Pen(Data.ControlBorderColor.GetBrush()), _roundRect);
        Color color = _isNull() ? Color.DarkRed : Data.ControlTextgroundColor;
        graphics.DrawString(ShowString, GH_FontServer.StandardAdjusted, color.GetBrush(), Bounds, GH_TextRenderingConstants.CenterCenter);
    }

    internal override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        if (e.Button == MouseButtons.Left)
        {
            var menu = new ToolStripDropDownMenu() 
            { 
                ShowImageMargin = false,
            };

            ShowValue ??= new(DateTime.Now);
            var time = ShowValue.Value;

            var ctrl = new DateTimePicker()
            {
                Value = time,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy/MM/dd hh:mm:ss",
            };

            ctrl.LostFocus += (sender, e) =>
            {
                menu.Close();
            };

            ctrl.ValueChanged += (sender, e) =>
            {
                if (sender is not DateTimePicker picker) return;
                ShowValue = new(picker.Value);
            };

            menu.Closing += (sender, e) =>
            {
                e.Cancel = e.CloseReason is ToolStripDropDownCloseReason.AppClicked;
            };

            GH_DocumentObject.Menu_AppendCustomItem(menu, ctrl);
            menu.Show(sender, e.ControlLocation);
        }
        base.Clicked(sender, e);
    }

    public override void DoSomethingWhenCreate(IGH_DocumentObject obj)
    {
        if (obj == null) return;
        GH_Component com = (GH_Component)obj;

        if (com.Params.Input.Count < 6) return;

        if (com.Params.Input[0] is Param_Integer param0)
        {
            param0.PersistentData.Clear();
            param0.PersistentData.Append(new(ShowValue.Value.Year));
        }

        if (com.Params.Input[1] is Param_Integer param1)
        {
            param1.PersistentData.Clear();
            param1.PersistentData.Append(new(ShowValue.Value.Month));
        }

        if (com.Params.Input[2] is Param_Integer param2)
        {
            param2.PersistentData.Clear();
            param2.PersistentData.Append(new(ShowValue.Value.Day));
        }

        if (com.Params.Input[3] is Param_Integer param3)
        {
            param3.PersistentData.Clear();
            param3.PersistentData.Append(new(ShowValue.Value.Hour));
        }

        if (com.Params.Input[4] is Param_Integer param4)
        {
            param4.PersistentData.Clear();
            param4.PersistentData.Append(new(ShowValue.Value.Minute));
        }

        if (com.Params.Input[5] is Param_Integer param5)
        {
            param5.PersistentData.Clear();
            param5.PersistentData.Append(new(ShowValue.Value.Second));
        }
    }
}
