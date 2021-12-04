using Grasshopper.GUI;
using Grasshopper.GUI.Base;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComponentToolkit
{
    internal class GooColorControl : GooControlBase<GH_Colour>
    {
        internal static readonly FieldInfo SwatchColorInfo = typeof(GH_ColourSwatch).GetRuntimeFields().Where(m => m.Name.Contains("m_SwatchColour")).First();

        protected override Guid AddCompnentGuid => new Guid("9c53bac0-ba66-40bd-8154-ce9829b9db1a");

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

            Color color = SavedValue?.Value ?? Color.Transparent;
            GH_DocumentObject.Menu_AppendColourPicker(menu, color, ColourChanged);
            menu.Show(sender, e.ControlLocation);


            void ColourChanged(GH_ColourPicker sender1, GH_ColourPickerEventArgs e1)
            {
                SavedValue = new GH_Colour(e1.Colour);
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

            if (SavedValue != null)
                graphics.FillPath(new SolidBrush(SavedValue.Value), _path);

            graphics.DrawPath(new Pen(new SolidBrush(Datas.ControlBorderColor)), _path);
        }

        protected override void DosomethingWhenCreate(IGH_DocumentObject obj)
        {
            GH_ColourSwatch swatch = (GH_ColourSwatch)obj;
            if (swatch == null) return;
            if (SavedValue != null)
                SwatchColorInfo.SetValue(swatch, SavedValue.Value);
        }
    }
}
