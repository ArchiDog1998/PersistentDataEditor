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
    internal class GH_AdvancedLinkParamAttr : GH_LinkedParamAttributes
    {
        public int StringWidth => GH_FontServer.StringWidth(Owner.NickName, GH_FontServer.StandardAdjusted);
        public int ControlWidth => Control != null && GH_ComponentAttributesReplacer.ComponentUseControl ? Control.Width : 0;
        public int WholeWidth => StringWidth + (ControlWidth == 0 ? 0 : ControlWidth + GH_ComponentAttributesReplacer.ComponentControlNameDistance);

        public BaseControlItem Control { get; private set; } = null;

        public RectangleF StringRect { get; internal set; }

        public GH_AdvancedLinkParamAttr(IGH_Param param, IGH_Attributes parent) : base(param, parent)
        {
            if (param.Kind != GH_ParamKind.input) return;
            if(IsPersistentParam(param.GetType(), out Type dataType))
            {
                if (typeof(GH_String).IsAssignableFrom(dataType))
                {
                    Control = new ParamStringControl((GH_PersistentParam<GH_String>)param);
                }
                else if (typeof(GH_Integer).IsAssignableFrom(dataType))
                {
                    Control = new ParamIntegerControl((GH_PersistentParam<GH_Integer>)param);
                }
                else if (typeof(GH_Number).IsAssignableFrom(dataType))
                {
                    Control = new ParamNumberControl((GH_PersistentParam<GH_Number>)param);
                }
                else if (typeof(GH_Colour).IsAssignableFrom(dataType))
                {
                    Control = new ParamColorControl((GH_PersistentParam<GH_Colour>)param);
                }
                else if (typeof(GH_Boolean).IsAssignableFrom(dataType))
                {
                    Control = new ParamBooleanControl((GH_PersistentParam<GH_Boolean>)param);
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
