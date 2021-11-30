using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

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
                else if (typeof(GH_Interval).IsAssignableFrom(dataType))
                {
                    Control = new ParamIntervalControl((GH_PersistentParam<GH_Interval>)param);
                }
                else if (typeof(GH_Point).IsAssignableFrom(dataType))
                {
                    Control = new ParamPoint3dControl((GH_PersistentParam<GH_Point>)param);
                }
                else if (typeof(GH_Vector).IsAssignableFrom(dataType))
                {
                    Control = new ParamVector3dControl((GH_PersistentParam<GH_Vector>)param);
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

        public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Control != null && Control.Bounds.Contains(e.CanvasLocation))
                {
                    Control.Clicked(sender, e);

                    return GH_ObjectResponse.Release;
                }
                if (StringRect.Contains(e.CanvasLocation))
                {
                    SortedList<Guid, CreateObjectItem[]> dict = new SortedList<Guid, CreateObjectItem[]>();
                    if (Owner.Kind == GH_ParamKind.input)
                    {
                        dict = GH_ComponentAttributesReplacer.createObjectItems.InputItems;
                    }
                    else if (Owner.Kind == GH_ParamKind.output)
                    {
                        dict = GH_ComponentAttributesReplacer.createObjectItems.OutputItems;
                    }

                    CreateObjectItem[] items = new CreateObjectItem[0];
                    if (dict.ContainsKey(Owner.ComponentGuid))
                    {
                        items = dict[Owner.ComponentGuid];
                    }

                    ToolStripDropDownMenu menu = new ToolStripDropDownMenu();
                    foreach (CreateObjectItem createItem in items)
                    {
                        GH_DocumentObject.Menu_AppendItem(menu, createItem.ShowName, Menu_CreateItemClicked, createItem.Icon).Tag = createItem;
                    }
                    GH_DocumentObject.Menu_AppendItem(menu, "Edit", Menu_EditItemClicked).Tag = items;

                    menu.Show(sender, e.ControlLocation);

                    return GH_ObjectResponse.Release;
                }
            }
            return base.RespondToMouseDoubleClick(sender, e);
        }

        private void Menu_CreateItemClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
            if (toolStripMenuItem != null && toolStripMenuItem.Tag != null && toolStripMenuItem.Tag is CreateObjectItem)
            {
                CreateObjectItem createItem = (CreateObjectItem)toolStripMenuItem.Tag;
                createItem.CreateObject(Owner);
                return;
            }
            MessageBox.Show("Something wrong with create object.");
        }

        private void Menu_EditItemClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
            if (toolStripMenuItem != null && toolStripMenuItem.Tag != null && toolStripMenuItem.Tag is CreateObjectItem[])
            {
                bool isInput = Owner.Kind == GH_ParamKind.input;
                new QuickWireEditor(Owner.ComponentGuid, isInput, Owner.Icon_24x24) { DataContext = new List<CreateObjectItem>((CreateObjectItem[]) toolStripMenuItem.Tag) }.Show();
            }
        }
    }
}
