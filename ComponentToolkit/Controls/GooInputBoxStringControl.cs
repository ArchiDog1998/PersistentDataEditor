using Grasshopper.GUI;
using Grasshopper.GUI.Base;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComponentToolkit
{
    internal class GooInputBoxStringControl<T> : GooControlBase<T> where T : class, IGH_Goo
    {
        protected override Guid AddCompnentGuid => new Guid("59e0b89a-e487-49f8-bab8-b5bab16be14c");

        private string ShowString => ChangeStringShow(SavedValue?.ToString());
        internal override int Height => 14;
        internal override int Width => Math.Min(Math.Max(GH_FontServer.StringWidth(ShowString, GH_FontServer.StandardAdjusted), 15),
            Datas.InputBoxControlMaxWidth);

        private GraphicsPath path;
        protected virtual bool IsReadOnly { get; }
        public GooInputBoxStringControl(Func<T> valueGetter, bool readOnly = false) : base(valueGetter)
        {
            IsReadOnly = readOnly;
        }

        internal override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == MouseButtons.Left && !IsReadOnly)
                new InputBoxBalloon(Bounds, SaveString).ShowTextInputBox(sender, ShowString, true, true, sender.Viewport.XFormMatrix(GH_Viewport.GH_DisplayMatrix.CanvasToControl));
            base.Clicked(sender, e);
        }

        private string ChangeStringShow(string str)
        {
            if(string.IsNullOrEmpty(str)) return str;

            List<char> chars = new List<char>();
            foreach (char item in str)
            {
                if (item == '{' || item == '}') continue;
                chars.Add(item);
            }
            return new string(chars.ToArray());
        }

        private void SaveString(string str)
        {
            T value = (T)Activator.CreateInstance(typeof(T));
            if (value.CastFrom(str))
            {
                SavedValue = value;
            }
            else
            {
                MessageBox.Show($"Can't cast a {typeof(T).Name} from \"{str}\".");
            }
        }

        protected override void LayoutObject(RectangleF bounds)
        {
            path = RoundedRect(GH_Convert.ToRectangle(bounds), 2);
            base.LayoutObject(bounds);
        }

        internal override void RenderObject(GH_Canvas canvas, Graphics graphics, IGH_Component owner, GH_PaletteStyle style)
        {
            graphics.FillPath(new SolidBrush(Datas.ControlBackgroundColor), path);
            graphics.DrawPath(new Pen(new SolidBrush(Datas.ControlBorderColor)), path);
            Color color = IsNull ? Color.DarkRed : Datas.ControlTextgroundColor;
            graphics.DrawString(ShowString, GH_FontServer.StandardAdjusted, new SolidBrush(color), Bounds, GH_TextRenderingConstants.NearCenter);
        }

        internal class InputBoxBalloon : GH_TextBoxInputBase
        {
            private Action<string> _setValue;


            internal InputBoxBalloon(RectangleF bounds, Action<string> setValue)
            {

                this._setValue = setValue;

                float mul = 3.5f;
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
    }


}
