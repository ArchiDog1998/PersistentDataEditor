using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PersistentDataEditor
{
    internal class GooEnumControl : GooControlBase<GH_Integer>
    {
        public override Guid AddCompnentGuid => new Guid("00027467-0D24-4fa7-B178-8DC0AC5F42EC");

        private string _showString
        {
            get
            {
                if(ShowValue == null)
                {
                    return null;
                }
                else
                {
                    int index = ShowValue.Value;
                    if (_namedValues.ContainsKey(index))
                    {
                        return _namedValues[index];
                    }
                    else
                    {
                        return index.ToString();
                    }
                }
            }
        }
        internal override int Height => 14;

        private int _triangleWidth = 14;
        internal override int Width => Math.Min(Math.Max(GH_FontServer.StringWidth(_showString, GH_FontServer.StandardAdjusted), 15), Datas.InputBoxControlMaxWidth) + _triangleWidth;

        private SortedList<int, string> _namedValues;

        private GraphicsPath _roundBounds;
        private RectangleF _stringBounds;
        private GraphicsPath _triangle;

        public GooEnumControl(Func<GH_Integer> valueGetter, Func<bool> isNull, SortedList<int, string> namedValues) : base(valueGetter, isNull)
        {
            _namedValues = namedValues;
        }

        internal override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if(e.Button == MouseButtons.Left)
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
            ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
            if (toolStripMenuItem != null && toolStripMenuItem.Tag != null && toolStripMenuItem.Tag is int)
            {
                ShowValue = new GH_Integer((int)toolStripMenuItem.Tag);
            }
        }

        protected override void LayoutObject(RectangleF bounds)
        {
            this._roundBounds = RoundedRect(GH_Convert.ToRectangle(bounds), 2);
            this._stringBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width - _triangleWidth, bounds.Height);

            RectangleF triBounds = new RectangleF(bounds.Right - _triangleWidth, bounds.Y, _triangleWidth, bounds.Height);
            this._triangle = Triangle(triBounds);

            base.LayoutObject(bounds);
        }

        private static GraphicsPath Triangle(RectangleF rect)
        {
            float radius = 1.5f;

            float width = 12;
            float height = width / 2 * (float)Math.Sqrt(3);
            PointF center = new PointF(rect.X + width / 2, rect.Y + rect.Height / 2);

            float up = height/ 2;
            PointF pt1 = new PointF(center.X - width / 2, center.Y - up + radius / 2);
            PointF pt2 = new PointF(center.X + width / 2, center.Y - up + radius / 2);
            PointF pt3 = new PointF(center.X, center.Y + up + radius / 2);

            float horiThick = radius * (float)(Math.Sqrt(3)  - 1);

            GraphicsPath path = new GraphicsPath();
            path.AddArc(new RectangleF(pt1.X + horiThick, pt1.Y, 2 * radius, 2 * radius), -90, -120);
            path.AddArc(new RectangleF(pt3.X - radius, pt3.Y - 3 * radius, 2 * radius, 2 * radius), -210, -120);
            path.AddArc(new RectangleF(pt2.X - horiThick - 2 * radius, pt2.Y, 2 * radius, 2 * radius), 30, -120);

            path.CloseFigure();
            return path;
        }

        internal override void RenderObject(GH_Canvas canvas, Graphics graphics, GH_PaletteStyle style)
        {
            graphics.FillPath(new SolidBrush(Datas.ControlBackgroundColor), _roundBounds);
            graphics.DrawPath(new Pen(new SolidBrush(Datas.ControlBorderColor)), _roundBounds);
            graphics.DrawString(_showString, GH_FontServer.StandardAdjusted, new SolidBrush(Datas.ControlTextgroundColor), _stringBounds, GH_TextRenderingConstants.NearCenter);

            graphics.FillPath(new SolidBrush(Datas.ControlForegroundColor), _triangle);
        }

        public override void DosomethingWhenCreate(IGH_DocumentObject obj)
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
}
