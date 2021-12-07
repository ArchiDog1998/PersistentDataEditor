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
        public abstract Guid AddCompnentGuid { get; }
        protected virtual ushort AddCompnentIndex => 0;
        protected virtual string AddCompnentInit => ShowValue?.ToString();
        internal IGH_Param Owner { private get; set; } = null;
        public IGH_Goo SaveValue => _savedValue;
        internal T _savedValue;
        internal T ShowValue 
        {
            get
            {
                T value = _valueGetter();
                if(value != null) _savedValue = value;
                return _savedValue;
            }
            private protected set
            {
                _savedValue = value;
                ValueChange();
            }
        }

        protected Func<bool> _isNull;

        public Action ValueChange { protected get; set; }

        private Func<T> _valueGetter;

        internal GooControlBase(Func<T> valueGetter, Func<bool> isNull)
        {
            _valueGetter = valueGetter;
            _isNull = isNull;
        }

        public virtual void DosomethingWhenCreate(IGH_DocumentObject obj) { }

        internal override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == MouseButtons.Right && AddCompnentGuid != default(Guid) && Owner != null)
                new CreateObjectItem(AddCompnentGuid, AddCompnentIndex, AddCompnentInit, true).CreateObject(Owner, DosomethingWhenCreate);
        }
    }

    public interface IGooValue
    {
        IGH_Goo SaveValue { get; }
        Action ValueChange { set; }
        Guid AddCompnentGuid { get; }
        void DosomethingWhenCreate(IGH_DocumentObject obj);
    }
}
