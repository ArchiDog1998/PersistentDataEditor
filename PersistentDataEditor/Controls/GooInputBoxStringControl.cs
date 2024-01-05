using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Base;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Input;
using Cursor = System.Windows.Forms.Cursor;

namespace PersistentDataEditor;

internal class GooInputBoxStringControl<T>(Func<T> valueGetter, Func<bool> isNull, bool readOnly = false)
    : GooControlBase<T>(valueGetter, isNull) where T : class, IGH_Goo
{
    public override Guid AddCompnentGuid => new("59e0b89a-e487-49f8-bab8-b5bab16be14c");

    private string ShowString => ShowValue?.ToString();
    internal override float Height => 14;
    internal override float MinWidth => Math.Min(Math.Max(GH_FontServer.StringWidth(ShowString, GH_FontServer.StandardAdjusted), 15),
        Data.InputBoxControlMaxWidth);

    private GraphicsPath _roundRect;
    protected virtual bool IsReadOnly { get; } = readOnly;

    PointF? clickedPt = null;
    PropertyInfo value = null;
    double oldValue = 0, changedValueDiff = 0;
    ModifierKeys _activeKey = ModifierKeys.None;

    internal override void MouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        if (e.Button == MouseButtons.Left && !IsReadOnly && double.TryParse(ShowString, out _))
        {
            clickedPt = e.CanvasLocation;
            value = ShowValue.GetType().GetRuntimeProperties().FirstOrDefault(p => p.Name == "Value");
            oldValue = Convert.ToDouble(value.GetValue(ShowValue));
            changedValueDiff = 0;
            _activeKey = Keyboard.Modifiers;

            sender.MouseMove += CanvasMouseMove;
            sender.MouseUp += Canvas_MouseUp;
        }
        _active = false;
        base.MouseDown(sender, e);
    }

    private void Canvas_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        var canvas = (GH_Canvas)sender;

        clickedPt = null;
        canvas.MouseUp -= Canvas_MouseUp;
        canvas.MouseMove -= CanvasMouseMove;

        Instances.CursorServer.ResetCursor(canvas);
    }

    bool _active = false;
    private void CanvasMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (!clickedPt.HasValue || value == null) return;

        var canvas = (GH_Canvas)sender;
        Instances.CursorServer.AttachCursor(canvas, "GH_NumericSlider");

        WrapCursor(canvas, ref clickedPt);

        var canvasPt = canvas.Viewport.UnprojectPoint(e.Location);

        double xDifference = double.Round(canvasPt.X - clickedPt.Value.X, 1);

        if (!_active && Math.Abs(xDifference) < 5) return;
        _active = true;

        xDifference *= Data.SliderSpeed;
        xDifference -= changedValueDiff;

        if (_activeKey == Keyboard.Modifiers)
        {
            ChangeValue(ref xDifference, _activeKey);
        }
        else
        {
            changedValueDiff += xDifference;
            ChangeValue(ref xDifference, _activeKey);
            oldValue += xDifference;
            xDifference = 0;
            _activeKey = Keyboard.Modifiers;
        }

        var newValue = oldValue + xDifference;

        var newGoo = ShowValue.Duplicate();
        value.SetValue(newGoo, Convert.ChangeType(newValue, value.PropertyType));
        ShowValue = (T)newGoo;

        static void ChangeValue(ref double value, ModifierKeys key)
        {
            value = key switch
            {
                ModifierKeys.Shift => value / 10,
                ModifierKeys.Control => value * 10,
                _ => value,
            };
        }
    }

    private static bool _preventUpdate = false;
    protected static void WrapCursor(GH_Canvas canvas, ref PointF? pt)
    {
        if (_preventUpdate || !pt.HasValue) return;
        _preventUpdate = true;

        Point point = canvas.PointToScreen(new Point(0, 0));
        Rectangle rectangle = new Rectangle(point.X, point.Y, canvas.Width, canvas.Height);
        Rectangle bounds = Screen.FromPoint(Cursor.Position).Bounds;
        rectangle = Rectangle.FromLTRB(Math.Max(rectangle.Left, bounds.Left), Math.Max(rectangle.Top, bounds.Top), Math.Min(rectangle.Right, bounds.Right), Math.Min(rectangle.Bottom, bounds.Bottom));
        checked
        {
            var xChange = (rectangle.Width - 5) * canvas.Viewport.ZoomInverse;
            var yChange = (rectangle.Height - 5) * canvas.Viewport.ZoomInverse;
            if (Cursor.Position.X <= rectangle.Left)
            {
                Cursor.Position = new Point(rectangle.Right - 5, Cursor.Position.Y);
                pt = new(pt.Value.X + xChange, pt.Value.Y);
            }
            if (Cursor.Position.X >= rectangle.Right - 1)
            {
                Cursor.Position = new Point(rectangle.Left + 5, Cursor.Position.Y);
                pt = new(pt.Value.X - xChange, pt.Value.Y);
            }
            if (Cursor.Position.Y <= rectangle.Top)
            {
                Cursor.Position = new Point(Cursor.Position.X, rectangle.Bottom - 5);
                pt = new(pt.Value.Y + yChange, pt.Value.Y);
            }
            if (Cursor.Position.Y >= rectangle.Bottom - 1)
            {
                Cursor.Position = new Point(Cursor.Position.X, rectangle.Top + 5);
                pt = new(pt.Value.Y - yChange, pt.Value.Y);
            }
            _preventUpdate = false;
        }
    }

    internal override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        if (e.Button == MouseButtons.Left && !IsReadOnly)
        {
            if (clickedPt == null || !_active && Math.Abs(e.CanvasX - clickedPt.Value.X) < 5)
            {
                new InputBoxBalloon(Bounds, SaveString).ShowTextInputBox(sender, ShowString, true, true, sender.Viewport.XFormMatrix(GH_Viewport.GH_DisplayMatrix.CanvasToControl));
            }

            clickedPt = null;
            sender.MouseMove -= CanvasMouseMove;
        }
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
