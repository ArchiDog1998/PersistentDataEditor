using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PersistentDataEditor
{
    internal abstract class GooMultiControlBase<T> : GooControlBase<T> where T : class, IGH_Goo
    {
        protected BaseControlItem[] _controlItems;
        internal IGooValue[] _values;
        private string _name;
        protected bool _hasName;
        protected bool _RespondBase = true;

        protected GooMultiControlBase(Func<T> valueGetter, Func<bool> isNull, string name) : base(valueGetter, isNull)
        {
            _name = name;
            ChangeControlItems();
        }

        internal sealed override void ChangeControlItems()
        {
            BaseControlItem[] addcontrolItems = SetControlItems() ?? Array.Empty<BaseControlItem>();
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
                if (control is IGooValue value)
                {
                    value.ValueChange = SetValue;
                    gooValues.Add(value);
                }
            }
            _values = gooValues.ToArray();
        }

        private void SetValue()
        {
            IGH_Goo[] goos = new IGH_Goo[_values.Length];
            for (int i = 0; i < _values.Length; i++)
            {
                var gooValue = _values[i];
                var g = gooValue.SaveValue;

                if (!IsGooValid(g))
                {
                    //Add default value.
                    if (NewData.UseDefaultValueToControl)
                    {
                        g = gooValue.GetDefaultValue();
                    }


                    if (!IsGooValid(g))
                    {
                        ShowValue = null;
                        return;
                    }
                }

                goos[i] = g;
            }
            ShowValue = SetValue(goos);
        }

        private static bool IsGooValid(IGH_Goo g)
        {
            return g != null && g.IsValid;
        }

        protected abstract T SetValue(IGH_Goo[] values);
        protected abstract BaseControlItem[] SetControlItems();

        internal override void RenderObject(GH_Canvas canvas, Graphics graphics, GH_PaletteStyle style)
        {
            foreach (var control in _controlItems)
            {
                control.RenderObject(canvas, graphics, style);
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

                new InputBoxBalloon(Bounds, SaveString).ShowTextInputBox(sender, ShowValue?.ToString(), true, true, sender.Viewport.XFormMatrix(GH_Viewport.GH_DisplayMatrix.CanvasToControl));

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
