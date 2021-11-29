using Grasshopper.GUI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel.Types;

namespace ComponentToolkit
{
    public class GH_AdvancedLinkParamAttr : GH_LinkedParamAttributes
    {
        public int StringWidth => GH_FontServer.StringWidth(Owner.NickName, GH_FontServer.StandardAdjusted);
        public int ControlWidth => Control == null ? 0 : Control.Width;
        public int WholeWidth => StringWidth + (ControlWidth == 0 ? 0 : ControlWidth + GH_ComponentAttributesReplacer.ComponentControlNameDistance);

        public IControlItem Control { get; private set; } = null;

        public RectangleF StringRect { get; internal set; }

        public GH_AdvancedLinkParamAttr(IGH_Param param, IGH_Attributes parent) : base(param, parent)
        {
            if (param.Kind != GH_ParamKind.input) return;
            if(IsPersistentParam(param.GetType(), out Type dataType))
            {
                if (typeof(GH_String).IsAssignableFrom(dataType))
                {
                    Control = new StringControl((GH_PersistentParam<GH_String>)param);
                }
                else if (typeof(GH_Integer).IsAssignableFrom(dataType))
                {
                    Control = new IntegerControl((GH_PersistentParam<GH_Integer>)param);
                }
                else if (typeof(GH_Number).IsAssignableFrom(dataType))
                {
                    Control = new NumberControl((GH_PersistentParam<GH_Number>)param);
                }
            }
        }

        public bool IsPersistentParam(Type type, out Type dataType)
        {
            dataType = default(Type);
            if (type == null)
            {
                return false;
            }
            else if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(GH_PersistentParam<>))
                {
                    dataType = type.GenericTypeArguments[0];
                    return true;
                }
                else if (type.GetGenericTypeDefinition() == typeof(GH_Param<>))
                    return false;
            }
            return IsPersistentParam(type.BaseType, out dataType);
        }

        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if(e.Button == MouseButtons.Left)
            {
                if(Control != null && Control.Bounds.Contains(e.CanvasLocation))
                {
                    Control.Clicked(sender, e);
                    return GH_ObjectResponse.Ignore;
                }
                else
                {
                    // Open a menu.
                }
            }
            return base.RespondToMouseDown(sender, e);
        }
    }
}
