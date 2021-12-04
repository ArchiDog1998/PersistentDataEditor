using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComponentToolkit
{
    internal abstract class GooMultiControlBase<T> : GooControlBase<T> where T : class, IGH_Goo
    {
        protected BaseControlItem[] _controlItems;
        private IGooValue[] _values;
        private string _name;
        protected bool _hasName = false;
        protected bool _RespondBase = true;
        public GooMultiControlBase(Func<T> valueGetter, string name) : base(valueGetter)
        {
            _name = name;
            ChangeControlItems();
        }

        internal sealed override void ChangeControlItems()
        {
            BaseControlItem[] addcontrolItems = SetControlItems() ?? new BaseControlItem[0];
            if (string.IsNullOrEmpty(_name))
            {
                _controlItems = addcontrolItems;
            }
            else
            {
                _hasName = true;
                List<BaseControlItem> items = new List<BaseControlItem> { new StringRender(_name) };
                items.AddRange(addcontrolItems);
                _controlItems = items.ToArray();
            }

            List<IGooValue> gooValues = new List<IGooValue>();
            foreach (var control in _controlItems)
            {
                control.ChangeControlItems();
                if (control is IGooValue)
                {
                    ((IGooValue)control).ValueChange = SetValue;
                    gooValues.Add((IGooValue)control);
                }
            }
            _values = gooValues.ToArray(); ;
        }

        private void SetValue()
        {
            IGH_Goo[] goos = new IGH_Goo[_values.Length];
            for (int i = 0; i < _values.Length; i++)
            {
                goos[i] = _values[i].SaveValue;
                if (goos[i] == null || !goos[i].IsValid)
                {
                    base.ValueChange();
                    return;
                }
            }
            ShowValue = SetValue(goos);
        }

        protected abstract T SetValue(IGH_Goo[] values);
        protected abstract BaseControlItem[] SetControlItems();

        internal sealed override void RenderObject(GH_Canvas canvas, Graphics graphics, IGH_Component owner, GH_PaletteStyle style)
        {
            foreach (var control in _controlItems)
            {
                control.RenderObject(canvas, graphics, owner, style);
            }
        }
        internal sealed override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == MouseButtons.Left)
            {
                foreach (var control in _controlItems)
                {
                    if (control is StringRender) continue;
                    if (control.Bounds.Contains(e.CanvasLocation))
                    {
                        control.Clicked(sender, e);
                        return;
                    }
                }

                if (!_RespondBase) return;

                new GooInputBoxStringControl<IGH_Goo>.InputBoxBalloon(Bounds, SaveString).ShowTextInputBox(sender, ShowValue?.ToString(), true, true, sender.Viewport.XFormMatrix(GH_Viewport.GH_DisplayMatrix.CanvasToControl));

                void SaveString(string str)
                {
                    T value = (T)Activator.CreateInstance(typeof(T));
                    if (value.CastFrom(str))
                    {
                        ShowValue = value;
                    }
                    else
                    {
                        MessageBox.Show($"Can't cast a {typeof(T).Name} from \"{str}\".");
                    }
                }
                return;
            }
            base.Clicked(sender, e);
        }
    }
}
