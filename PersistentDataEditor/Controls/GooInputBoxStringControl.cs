using Grasshopper.GUI;
using Grasshopper.GUI.Base;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PersistentDataEditor;

internal class GooInputBoxStringControl<T>(Func<T> valueGetter, Func<bool> isNull, bool readOnly = false)
    : GooControlBase<T>(valueGetter, isNull) where T : class, IGH_Goo
{
    public override Guid AddCompnentGuid => new("59e0b89a-e487-49f8-bab8-b5bab16be14c");

    private string ShowString => ShowValue?.ToString();
    internal override int Height => 14;
    internal override int MinWidth => Math.Min(Math.Max(GH_FontServer.StringWidth(ShowString, GH_FontServer.StandardAdjusted), 15),
        Data.InputBoxControlMaxWidth);

    private GraphicsPath _roundRect;
    protected virtual bool IsReadOnly { get; } = readOnly;

    internal override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        if (e.Button == MouseButtons.Left && !IsReadOnly)
            new InputBoxBalloon(Bounds, SaveString).ShowTextInputBox(sender, ShowString, true, true, sender.Viewport.XFormMatrix(GH_Viewport.GH_DisplayMatrix.CanvasToControl));
        base.Clicked(sender, e);
    }

    private void SaveString(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            ShowValue = null;
            return;
        }

        if (typeof(T).IsInterface)
        {
            ShowValue = (T)(object)new GH_String(str);
            return;
        }
        else
        {
            T value = Activator.CreateInstance<T>();
            if (value.CastFrom(str))
            {
                ShowValue = value;
                return;
            }
        }

        MessageBox.Show($"Can't cast a {typeof(T).Name} from \"{str}\".");
    }

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
}

internal sealed class InputBoxBalloon : GH_TextBoxInputBase
{
    private readonly Action<string> _setValue;

    internal InputBoxBalloon(RectangleF bounds, Action<string> setValue)
    {
        _setValue = setValue;

        const float mul = 3.5f;
        if (bounds.Height * mul > bounds.Width)
        {
            bounds = new RectangleF(bounds.Location, new SizeF(bounds.Height * mul, bounds.Height));
        }

        Bounds = GH_Convert.ToRectangle(bounds);
        Font = GH_FontServer.ConsoleAdjusted;
    }

    protected override void HandleTextInputAccepted(string text)
    {
        _setValue(text);
    }
}
