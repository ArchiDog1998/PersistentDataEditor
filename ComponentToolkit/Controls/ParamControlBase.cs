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
    internal abstract class ParamControlBase<T>:BaseControlItem where T : class, IGH_Goo
    {
        protected GH_PersistentParam<T> Owner { get; }
        internal sealed override int Width => Valid? ActualWidth : 0;
        protected int ActualWidth 
        {
            get
            {
                int all = 0;
                foreach (var control in ControlItems)
                {
                    all += control.Width;
                }
                return all;
            }
        }
        internal sealed override int Height 
        {
            get
            {
                int max = int.MinValue;
                foreach (var control in ControlItems)
                {
                    max = Math.Max(max, control.Height);
                }
                return max;
            }
        }
        private bool Valid => Owner.OnPingDocument() == Grasshopper.Instances.ActiveCanvas.Document && Owner.SourceCount == 0 && Owner.PersistentDataCount < 2;

        protected BaseControlItem[] ControlItems = new BaseControlItem[0];

        public ParamControlBase(GH_PersistentParam<T> owner)
        {
            Owner = owner;
        }

        protected void SetValue(T value, bool recordUndo = true)
        {
            if (recordUndo)
            {
                Owner.RecordUndoEvent("Set: " + value.ToString());
            }
            Owner.PersistentData.Clear();
            Owner.PersistentData.Append(value);
            Owner.ExpireSolution(true);
        }

        protected sealed override void LayoutObject(RectangleF bounds)
        {
            float x = bounds.X;
            foreach (BaseControlItem item in ControlItems)
            {
                item.Bounds = new RectangleF(x, bounds.Y + (bounds.Height - item.Height) /2, item.Width, item.Height);
                x += item.Width;
            }
            base.LayoutObject(bounds);
        }

        internal sealed override void RenderObject(GH_Canvas canvas, Graphics graphics, IGH_Component owner, GH_PaletteStyle style)
        {
            foreach (var control in ControlItems)
            {
                control.RenderObject(canvas, graphics, owner, style);
            }
        }
        internal sealed override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            foreach (var control in ControlItems)
            {
                if (control.Bounds.Contains(e.CanvasLocation))
                {
                    control.Clicked(sender, e);
                    return;
                }
            }
        }
    }
}
