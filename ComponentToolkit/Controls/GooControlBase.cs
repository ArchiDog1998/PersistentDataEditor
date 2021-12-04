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
    internal abstract class GooControlBase<T> : BaseControlItem, IGooValue where T : class, IGH_Goo
    {
        protected abstract Guid AddCompnentGuid { get; }
        protected virtual ushort AddCompnentIndex => 0;
        protected virtual string AddCompnentInit => SavedValue?.ToString();
        internal IGH_Param Owner { private get; set; } = null;
        public IGH_Goo SaveGoo => _savedValue;
        private T _savedValue;
        internal T SavedValue 
        {
            get
            {
                T value = _valueGetter();
                IsNull = value == null;
                if(!IsNull) _savedValue = value;
                return _savedValue;
            }
            private protected set
            {
                _savedValue = value;
                if (ValueChange != null)
                    ValueChange();
            }
        }
        protected bool IsNull { get; private set; }
        public Action ValueChange { protected get; set; }

        private Func<T> _valueGetter;

        internal GooControlBase(Func<T> valueGetter)
        {
            _valueGetter = valueGetter;
        }

        protected virtual void DosomethingWhenCreate(IGH_DocumentObject obj) { }

        internal override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == MouseButtons.Right && AddCompnentGuid != default(Guid) && Owner != null)
                new CreateObjectItem(AddCompnentGuid, AddCompnentIndex, AddCompnentInit, true).CreateObject(Owner, DosomethingWhenCreate);
        }
    }

    public interface IGooValue
    {
        IGH_Goo SaveGoo { get; }
        Action ValueChange { set; }
    }
}
