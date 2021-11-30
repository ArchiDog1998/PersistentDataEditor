﻿using Grasshopper.GUI;
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
    internal class GooEnumControl : GooControlBase<GH_Integer>
    {
        private string _showString
        {
            get
            {
                if(Value == null)
                {
                    return null;
                }
                else
                {
                    int index = Value.Value;
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
        internal override int Width => Math.Min(Math.Max(GH_FontServer.StringWidth(_showString, GH_FontServer.StandardAdjusted), 15), 100) + _triangleWidth;


        private SortedList<int, string> _namedValues;

        private GraphicsPath _roundBounds;
        private RectangleF _stringBounds;
        private GraphicsPath _triangle;

        public GooEnumControl(Func<GH_Integer> valueGetter, Action<GH_Integer, bool> valueChanged, SortedList<int, string> namedValues) : 
            base(valueGetter, valueChanged)
        {
            _namedValues = namedValues;
        }

        internal override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            ToolStripDropDownMenu menu = new ToolStripDropDownMenu();
            int? num = Value?.Value;
            foreach (var namedValue in _namedValues)
            {
                GH_DocumentObject.Menu_AppendItem(menu, namedValue.Value, Menu_NamedValueClicked, true, namedValue.Key == num).Tag = namedValue.Key;
            }
            menu.Show(sender, e.ControlLocation);
        }

        private void Menu_NamedValueClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
            if (toolStripMenuItem != null && toolStripMenuItem.Tag != null && toolStripMenuItem.Tag is int)
            {
                Value = new GH_Integer((int)toolStripMenuItem.Tag);
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

        internal override void RenderObject(GH_Canvas canvas, Graphics graphics, IGH_Component owner, GH_PaletteStyle style)
        {
            graphics.FillPath(new SolidBrush(ControlBackgroundColor), _roundBounds);
            graphics.DrawPath(new Pen(new SolidBrush(ControlBorderColor)), _roundBounds);
            graphics.DrawString(_showString, GH_FontServer.StandardAdjusted, new SolidBrush(ControlTextgroundColor), _stringBounds, GH_TextRenderingConstants.NearCenter);

            graphics.FillPath(new SolidBrush(ControlForegroundColor), _triangle);
        }
    }
}