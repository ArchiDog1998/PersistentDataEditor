using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace PersistentDataEditor;

internal abstract class GooControlBase<T> : BaseControlItem, IGooValue where T : class, IGH_Goo
{
    private static readonly MethodInfo _function = typeof(GH_Canvas).GetRuntimeMethods().First(m => m.Name.Contains("InstantiateNewObject") && !m.IsPublic);

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
            if (value != null) _savedValue = value;
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

    private readonly Func<T> _valueGetter;

    internal GooControlBase(Func<T> valueGetter, Func<bool> isNull)
    {
        _valueGetter = valueGetter;
        _isNull = isNull;
    }

    public virtual void DosomethingWhenCreate(IGH_DocumentObject obj) { }

    internal override void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        if (e.Button == MouseButtons.Right && AddCompnentGuid != default && Owner != null)
            CreateObject(Owner, AddCompnentGuid, AddCompnentIndex, AddCompnentInit, true, DosomethingWhenCreate);
    }

    private static IGH_DocumentObject CreateObject(IGH_Param param, Guid componentGuid, ushort index, string init, bool isinput, Action<IGH_DocumentObject> action = null)
    {
        const int width = 100;

        IGH_DocumentObject obj = Grasshopper.Instances.ComponentServer.EmitObject(componentGuid);
        if (obj == null) return null;

        action?.Invoke(obj);

        //Get Aimed Point.  
        RectangleF outBound = param.Attributes.GetTopLevel.Bounds;
        RectangleF thisBound = param.Attributes.Bounds;

        PointF objCenter = new PointF(outBound.Left + (isinput ? -width : (width + outBound.Width)),
               thisBound.Top + thisBound.Height / 2);

        AddAnObjectToCanvas(obj, objCenter, init);
        AddSources();

        Grasshopper.Instances.ActiveCanvas.Document.NewSolution(false);

        return obj;

        void AddSources()
        {
            if (obj is IGH_Component com)
            {
                if (isinput)
                {
                    param.AddSource(com.Params.Output[index]);
                }
                else
                {
                    com.Params.Input[index].AddSource(param);
                }
            }
            else if (obj is IGH_Param par)
            {
                if (isinput)
                {
                    param.AddSource(par);
                }
                else
                {
                    par.AddSource(param);
                }
            }
        }

        static void AddAnObjectToCanvas(IGH_DocumentObject obj, PointF pivot, string init, bool update = false)
        {
            _function.Invoke(Grasshopper.Instances.ActiveCanvas, new object[] { obj, init, pivot, update });
        }
    }

    private protected virtual T CreateDefaultValue()
    {
        return Activator.CreateInstance<T>();
    }

    public IGH_Goo GetDefaultValue()
    {
        _savedValue = CreateDefaultValue();
        return _savedValue;
    }
}

public interface IGooValue
{
    IGH_Goo SaveValue { get; }
    Action ValueChange { set; }
    Guid AddCompnentGuid { get; }
    void DosomethingWhenCreate(IGH_DocumentObject obj);
    IGH_Goo GetDefaultValue();
}
