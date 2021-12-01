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
    internal class GooColorControl : GooControlBase<GH_Colour>
    {
        internal override int Width => 12;

        internal override int Height => 12;

        private Brush _background = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.White, Color.LightGray);
        private GraphicsPath _path;


        public GooColorControl(Func<GH_Colour> valueGetter):base(valueGetter)
        {
        }


        internal override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            ToolStripDropDownMenu menu = new ToolStripDropDownMenu() { ShowImageMargin = false };

            Color color = ShowValue?.Value ?? Color.Transparent;
            GH_DocumentObject.Menu_AppendColourPicker(menu, color, ColourChanged);
            menu.Show(sender, e.ControlLocation);


            void ColourChanged(GH_ColourPicker sender1, GH_ColourPickerEventArgs e1)
            {
                ShowValue = new GH_Colour(e1.Colour);
            }
        }

        protected override void LayoutObject(RectangleF bounds)
        {
            _path = RoundedRect(GH_Convert.ToRectangle(Bounds), 2);
            base.LayoutObject(bounds);
        }

        internal override void RenderObject(GH_Canvas canvas, Graphics graphics, IGH_Component owner, GH_PaletteStyle style)
        {

            graphics.FillPath(_background, _path);

            if (ShowValue != null)
                graphics.FillPath(new SolidBrush(ShowValue.Value), _path);

            graphics.DrawPath(new Pen(new SolidBrush(ControlBorderColor)), _path);
        }
    }
}
