using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System.Drawing;
using System.Linq;

namespace PersistentDataEditor;

internal abstract class ParamControlBase<T> : BaseControlItem where T : class, IGH_Goo
{
    protected GH_PersistentParam<T> Owner { get; }
    private bool _isSaveUndo = true;
    internal T OwnerGooData
    {
        get
        {
            T value = Owner.PersistentData.get_FirstItem(true);
            IsNull = value == null;
            return value;
        }
        private set
        {
            if (_isSaveUndo)
            {
                Owner.RecordUndoEvent("Set: " + value?.ToString());
                _isSaveUndo = false;
            }
            Owner.PersistentData.Clear();
            if (value != null)
                Owner.PersistentData.Append(value);
            Owner.ExpireSolution(true);
        }
    }

    protected bool IsNull { get; private set; }

    private readonly GooControlBase<T> _gooControl;

    internal sealed override int Width
    {
        get
        {
            if (Valid)
            {
                return _gooControl.Width;
            }

            _gooControl.Bounds = RectangleF.Empty;
            return 0;

        }
    }
    internal sealed override int Height
    {
        get
        {
            if (Valid)
            {
                return _gooControl.Height;
            }

            _gooControl.Bounds = RectangleF.Empty;
            return 0;

        }
    }
    protected override bool Valid => Owner.OnPingDocument() == Instances.ActiveCanvas.Document
        && Owner.SourceCount == 0 && Owner.PersistentDataCount < 2
        && (!NewData.OnlyShowSelectedObjectControl || Owner.Attributes.Selected)
        && !Owner.PersistentData.Any(d => d is IGH_GeometricGoo g && g.IsReferencedGeometry);

    protected ParamControlBase(GH_PersistentParam<T> owner)
    {
        Owner = owner;
        _gooControl = SetUpControl(owner);
        _gooControl.Owner = owner;
        _gooControl.ValueChange = SetValue;
    }

    private void SetValue()
    {
        OwnerGooData = (T)_gooControl.SaveValue;
    }
    protected abstract GooControlBase<T> SetUpControl(IGH_Param param);

    protected sealed override void LayoutObject(RectangleF bounds)
    {
        _gooControl.Bounds = bounds;
        base.LayoutObject(bounds);
    }

    internal sealed override void RenderObject(GH_Canvas canvas, Graphics graphics, GH_PaletteStyle style)
    {
        if (!Valid) return;
        NewData.IsCurrectObjectLock = Owner.Locked;
        _gooControl.RenderObject(canvas, graphics, style);
    }

    internal sealed override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        if (!Valid || !ShouldRespond || Owner.Locked || sender.Viewport.Zoom < 0.6) return;
        if (e.Button == System.Windows.Forms.MouseButtons.Left) _isSaveUndo = true;
        _gooControl.Clicked(sender, e);
    }

    internal sealed override void ChangeControlItems()
    {
        _gooControl.ChangeControlItems();
    }
}
