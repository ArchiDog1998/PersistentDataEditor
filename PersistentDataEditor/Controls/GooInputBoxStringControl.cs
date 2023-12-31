using Grasshopper.GUI;
using Grasshopper.GUI.Base;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PersistentDataEditor
{
    internal class GooInputBoxStringControl<T> : GooControlBase<T> where T : class, IGH_Goo
    {
        public override Guid AddCompnentGuid => new Guid("59e0b89a-e487-49f8-bab8-b5bab16be14c");

        private string ShowString => ShowValue?.ToString();
        internal override int Height => 14;
        internal override int Width => Math.Min(Math.Max(GH_FontServer.StringWidth(ShowString, GH_FontServer.StandardAdjusted), 15),
            NewData.InputBoxControlMaxWidth);

        private GraphicsPath _roundRect;
        protected virtual bool IsReadOnly { get; }

        public GooInputBoxStringControl(Func<T> valueGetter, Func<bool> isNull, bool readOnly = false) : base(valueGetter, isNull)
        {
            IsReadOnly = readOnly;
        }

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
                T value = (T)Activator.CreateInstance(typeof(T));
                if (value.CastFrom(str))
                {
                    ShowValue = value;
                    return;
                }
            }

            MessageBox.Show($"Can't cast a {typeof(T).Name} from \"{str}\".");
        }

        protected override void LayoutObject(RectangleF bounds)
        {
            _roundRect = RoundedRect(GH_Convert.ToRectangle(bounds), 2);
            base.LayoutObject(bounds);
        }

        internal override void RenderObject(GH_Canvas canvas, Graphics graphics, GH_PaletteStyle style)
        {
            graphics.FillPath(new SolidBrush(NewData.ControlBackgroundColor), _roundRect);
            graphics.DrawPath(new Pen(new SolidBrush(NewData.ControlBorderColor)), _roundRect);
            Color color = _isNull() ? Color.DarkRed : NewData.ControlTextgroundColor;
            graphics.DrawString(ShowString, GH_FontServer.StandardAdjusted, new SolidBrush(color), Bounds, GH_TextRenderingConstants.NearCenter);
        }
    }

    internal sealed class InputBoxBalloon : GH_TextBoxInputBase
    {
        private readonly Action<string> _setValue;

        //private static readonly MethodInfo _inputBoxInfo = typeof(InputBoxBalloon).FindMethod("TextOverrideLostFocus"));

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
        private void TextOverrideLostFocusNew(object sender, EventArgs e)
        {
            if(NewData.TextboxInputAutoApply) RespondToEnter();
            HideTextInputBox();
        }

        protected override void HandleTextInputAccepted(string text)
        {
            _setValue(text);
        }
    }
}
