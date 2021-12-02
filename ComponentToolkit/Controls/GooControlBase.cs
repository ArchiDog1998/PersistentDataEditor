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
    internal abstract class GooControlBase<T> : BaseControlItem, IGooValue where T : class, IGH_Goo
    {
        public IGH_Goo SaveGoo => _savedValue;

        private T _savedValue;
        protected T ShowValue 
        {
            get
            {
                T value = _valueGetter();
                IsNull = value == null;
                if(!IsNull) _savedValue = value;
                return _savedValue;
            }
            set
            {
                _savedValue = value;
                if (ValueChange != null)
                    ValueChange();
            }
        }
        public bool IsNull { get; private set; }
        public Action ValueChange { protected get; set; }
        private Func<T> _valueGetter;

        internal GooControlBase(Func<T> valueGetter)
        {
            _valueGetter = valueGetter;
        }
    }

    internal interface IGooValue
    {
        IGH_Goo SaveGoo { get; }
        Action ValueChange { set; }
    }
}
