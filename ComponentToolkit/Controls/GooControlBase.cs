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

namespace ComponentToolkit
{
    internal abstract class GooControlBase<T> : BaseControlItem where T : class, IGH_Goo
    {

        private T _value;
        internal T Value 
        {
            get
            {
                T value = _valueGetter();
                IsNull = value == null;
                if(!IsNull) _value = value;
                return _value;
            }
            set
            {
                _value = value;
                _valueChanged(value, SaveUndo);
            }
        }
        public bool IsNull { get; private set; }
        private Action<T, bool> _valueChanged;
        private Func<T> _valueGetter;

        internal bool SaveUndo { private get; set; } = true;

        internal GooControlBase(Func<T> valueGetter, Action<T, bool> valueChanged)
        {
            _valueGetter = valueGetter;
            _valueChanged = valueChanged;
        }
    }
}
