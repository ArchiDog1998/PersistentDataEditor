using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Components;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Parameters.Hints;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ComponentToolkit
{
    internal class GH_AdvancedLinkParamAttr : GH_LinkedParamAttributes, IControlAttr
    {
        public int StringWidth => GH_FontServer.StringWidth(Owner.NickName, GH_FontServer.StandardAdjusted);
        public int ControlWidth => Control?.Width ?? 0;
        public int WholeWidth => StringWidth + (ControlWidth == 0 ? 0 : ControlWidth + Datas.ComponentControlNameDistance) + 
            (Datas.ShowLinkParamIcon ? Datas.ComponentParamIconSize + Datas.ComponentIconDistance : 0);
        public int ParamHeight => Math.Max(20, (Control?.Height ?? 0) + 3);

        public BaseControlItem Control { get; private set; } = null;

        public RectangleF StringRect { get; set; }

        public RectangleF IconRect { get; set; }

        public static SortedList<Guid, Bitmap> IconSet = new SortedList<Guid, Bitmap>();

        public GH_AdvancedLinkParamAttr(IGH_Param param, IGH_Attributes parent) : base(param, parent)
        {
            SetControl();
            SetParamIcon();
        }

        public void SetControl()
        {
            if(Datas.UseParamControl && Owner.Kind == GH_ParamKind.input && Datas.ComponentUseControl)
            {
                Control = GetControl(Owner);
            }
            else
            {
                Control = null;
            }
        }

        public static void UpdataIcons()
        {
            SortedList<Guid, Bitmap> iconset = new SortedList<Guid, Bitmap>();
            foreach (var item in IconSet)
            {
                IGH_ObjectProxy proxy = Grasshopper.Instances.ComponentServer.EmitObjectProxy(item.Key);
                iconset[item.Key] = BitmapConvert(proxy.Icon);
            }
            IconSet = iconset;
        }

        public Bitmap SetParamIcon()
        {
            if (Owner == null) return null;

            Bitmap outBit = BitmapConvert(Owner.Icon_24x24);
            IconSet[Owner.ComponentGuid] = outBit;
            return outBit;
        }

        private static Bitmap BitmapConvert(Bitmap bitmap, bool useOpacity = true)
        {
            float opacity = useOpacity ? (float)Datas.ComponentIconOpacity : 1;
            float[][] nArray =
            {
                new float[]{1,0,0,0,0},
                new float[]{0,1,0,0,0},
                new float[]{0,0,1,0,0},
                new float[]{0,0,0,opacity,0},
                new float[]{0,0,0,0,1},
            };

            ImageAttributes attr = new ImageAttributes();
            attr.SetColorMatrix(new ColorMatrix(nArray), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            Bitmap outBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            Graphics g = Graphics.FromImage(outBitmap);
            g.DrawImage(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, attr);
            g.Dispose();

            return outBitmap;
        }

        internal static BaseControlItem GetControl(IGH_Param param)
        {
            if(param is Param_ScriptVariable)
            {
                return GetUse("Script") ? new ParamVariableControl((Param_ScriptVariable)param) : null;
            }
            if(Datas.OnlyItemAccessControl && param.Access != GH_ParamAccess.item) return null;

            if (param is Param_Geometry) return null;

            if (IsPersistentParam(param.GetType(), out Type storeType))
            {
                if (storeType == typeof(GH_Curve) || storeType == typeof(GH_Brep) || storeType == typeof(GH_Surface) || storeType == typeof(GH_Mesh) || storeType.FullName == "Grasshopper.Kernel.Types.GH_SubD"
                    || storeType == typeof(GH_Field) || storeType == typeof(GH_Transform) || storeType == typeof(GH_Matrix))
                {
                    return null;
                }
                else if (storeType == typeof(GH_String))
                {
                    return GetUse(nameof(GH_String)) ? new ParamStringControl((GH_PersistentParam<GH_String>)param) : null;
                }
                else if (storeType == typeof(GH_Integer))
                {
                    return GetUse(nameof(GH_Integer)) ? new ParamIntegerControl((GH_PersistentParam<GH_Integer>)param) : null;
                }
                else if (storeType == typeof(GH_Number))
                {
                    return GetUse(nameof(GH_Number)) ? new ParamNumberControl((GH_PersistentParam<GH_Number>)param) : null;
                }
                else if (storeType == typeof(GH_Colour))
                {
                    return GetUse(nameof(GH_Colour)) ? new ParamColourControl((GH_PersistentParam<GH_Colour>)param) : null;
                }
                else if (storeType == typeof(GH_Boolean))
                {
                    return GetUse(nameof(GH_Boolean)) ? new ParamBooleanControl((GH_PersistentParam<GH_Boolean>)param) : null;
                }
                else if (storeType == typeof(GH_Material))
                {
                    return GetUse(nameof(GH_Material)) ? new ParamMaterialControl((GH_PersistentParam<GH_Material>)param) : null;
                }
                else if (storeType == typeof(GH_Interval))
                {
                    return GetUse(nameof(GH_Interval)) ? new ParamIntervalControl((GH_PersistentParam<GH_Interval>)param) : null;
                }
                else if (storeType == typeof(GH_Point))
                {
                    return GetUse(nameof(GH_Point)) ? new ParamPointControl((GH_PersistentParam<GH_Point>)param) : null;
                }
                else if (storeType == typeof(GH_Vector))
                {
                    return GetUse(nameof(GH_Vector)) ? new ParamVectorControl((GH_PersistentParam<GH_Vector>)param) : null;
                }
                else if (storeType == typeof(GH_ComplexNumber))
                {
                    return GetUse(nameof(GH_ComplexNumber)) ? new ParamComplexControl((GH_PersistentParam<GH_ComplexNumber>)param) : null;
                }
                else if (storeType == typeof(GH_Interval2D))
                {
                    return GetUse(nameof(GH_Interval2D)) ? new ParamInterval2DControl((GH_PersistentParam<GH_Interval2D>)param) : null;
                }
                else if (storeType == typeof(GH_Line))
                {
                    return GetUse(nameof(GH_Line)) ? new ParamLineControl((GH_PersistentParam<GH_Line>)param) : null;
                }
                else if (storeType == typeof(GH_Plane))
                {
                    return GetUse(nameof(GH_Plane)) ? new ParamPlaneControl((GH_PersistentParam<GH_Plane>)param) : null;
                }
                else if (storeType == typeof(GH_Circle))
                {
                    return GetUse(nameof(GH_Circle)) ? new ParamCircleControl((GH_PersistentParam<GH_Circle>)param) : null;
                }
                else if (storeType == typeof(GH_Rectangle))
                {
                    return GetUse(nameof(GH_Rectangle)) ? new ParamRectangleControl((GH_PersistentParam<GH_Rectangle>)param) : null;
                }
                else if (storeType == typeof(GH_Box))
                {
                    return GetUse(nameof(GH_Box)) ? new ParamBoxControl((GH_PersistentParam<GH_Box>)param) : null;
                }
                else if (storeType == typeof(GH_Arc))
                {
                    return GetUse(nameof(GH_Arc)) ? new ParamArcControl((GH_PersistentParam<GH_Arc>)param) : null;
                }

                else if (GetUse(nameof(IGH_Goo)))
                {
                    Type controlType = typeof(ParamGeneralControl<>).MakeGenericType(storeType);
                    return (BaseControlItem)Activator.CreateInstance(controlType, param);
                }
            }
            return null;
        }

        internal static bool GetUse(string name)
        {
            return Instances.Settings.GetValue("UseParam" + name, true);
        }

        internal static bool IsPersistentParam(Type type, out Type dataType)
        {
            dataType = null;
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

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {

            if (Control != null && Control.Bounds.Contains(e.CanvasLocation))
            {
                Control.Clicked(sender, e);

                return GH_ObjectResponse.Release;
            }

            if (Datas.UseQuickWire && BaseControlItem.ShouldRespond && e.Button == MouseButtons.Left)
            {
                RespondToQuickWire(Owner, Owner.ComponentGuid, Owner.Kind == GH_ParamKind.input).Show(sender, e.ControlLocation);
                return GH_ObjectResponse.Release;
            } 
            return base.RespondToMouseUp(sender, e);
        }

        internal static ToolStripDropDownMenu RespondToQuickWire(IGH_Param param, Guid guid, bool isInput, bool isFirst = true)
        {
            SortedList<Guid, CreateObjectItem[]> dict = new SortedList<Guid, CreateObjectItem[]>();
            if (isInput)
            {
                dict = GH_ComponentAttributesReplacer.StaticCreateObjectItems.InputItems;
            }
            else
            {
                dict = GH_ComponentAttributesReplacer.StaticCreateObjectItems.OutputItems;
            }

            //Change Guid.
            if(param is Param_ScriptVariable && guid == new Param_ScriptVariable().ComponentGuid)
            {
                Param_ScriptVariable script = (Param_ScriptVariable)param;

                if(script.TypeHint != null)
                {
                    if (script.TypeHint is GH_ArcHint)
                        guid = new Param_Arc().ComponentGuid;
                    else if (script.TypeHint is GH_BooleanHint_CS || script.TypeHint is GH_BooleanHint_VB)
                        guid = new Param_Boolean().ComponentGuid;
                    else if (script.TypeHint is GH_BoxHint)
                        guid = new Param_Box().ComponentGuid;
                    else if (script.TypeHint is GH_BrepHint)
                        guid = new Param_Brep().ComponentGuid;
                    else if (script.TypeHint is GH_CircleHint)
                        guid = new Param_Circle().ComponentGuid;
                    else if (script.TypeHint is GH_ColorHint)
                        guid = new Param_Colour().ComponentGuid;
                    else if (script.TypeHint is GH_ComplexHint)
                        guid = new Param_Complex().ComponentGuid;
                    else if (script.TypeHint is GH_CurveHint)
                        guid = new Param_Curve().ComponentGuid;
                    else if (script.TypeHint is GH_DateTimeHint)
                        guid = new Param_Time().ComponentGuid;
                    else if (script.TypeHint is GH_DoubleHint_CS || script.TypeHint is GH_DoubleHint_VB)
                        guid = new Param_Number().ComponentGuid;
                    else if (script.TypeHint is GH_GeometryBaseHint)
                        guid = new Param_Geometry().ComponentGuid;
                    else if (script.TypeHint is GH_GuidHint)
                        guid = new Param_Guid().ComponentGuid;
                    else if (script.TypeHint is GH_IntegerHint_CS || script.TypeHint is GH_IntegerHint_VB)
                        guid = new Param_Integer().ComponentGuid;
                    else if (script.TypeHint is GH_IntervalHint)
                        guid = new Param_Interval().ComponentGuid;
                    else if (script.TypeHint is GH_LineHint)
                        guid = new Param_Line().ComponentGuid;
                    else if (script.TypeHint is GH_MeshHint)
                        guid = new Param_Mesh().ComponentGuid;
                    else if (script.TypeHint is GH_NullHint)
                        guid = new Param_GenericObject().ComponentGuid;
                    else if (script.TypeHint is GH_PlaneHint)
                        guid = new Param_Plane().ComponentGuid;
                    else if (script.TypeHint is GH_Point3dHint)
                        guid = new Param_Point().ComponentGuid;
                    else if (script.TypeHint is GH_PolylineHint)
                        guid = new Param_Curve().ComponentGuid;
                    else if (script.TypeHint is GH_Rectangle3dHint)
                        guid = new Param_Rectangle().ComponentGuid;
                    else if (script.TypeHint is GH_StringHint_CS || script.TypeHint is GH_StringHint_VB)
                        guid = new Param_String().ComponentGuid;
                    else if (script.TypeHint?.TypeName == "SubD")
                        guid = new Guid("{89CD1A12-0007-4581-99BA-66578665E610}");
                    else if (script.TypeHint is GH_SurfaceHint)
                        guid = new Param_Surface().ComponentGuid;
                    else if (script.TypeHint is GH_TransformHint)
                        guid = new Param_Transform().ComponentGuid;
                    else if (script.TypeHint is GH_UVIntervalHint)
                        guid = new Param_Interval2D().ComponentGuid;
                    else if (script.TypeHint is GH_Vector3dHint)
                        guid = new Param_Vector().ComponentGuid;
                }

            }

            CreateObjectItem[] items = new CreateObjectItem[0];
            if (dict.ContainsKey(guid))
            {
                items = dict[guid];
            }

            ToolStripDropDownMenu menu = new ToolStripDropDownMenu() { MaximumSize = new Size(500, 700) };

            if (isFirst)
            {

                if (param.VolatileDataCount > 1 && !isInput)
                {
                    IGH_ObjectProxy proxy = Instances.ComponentServer.EmitObjectProxy(new Guid("59daf374-bc21-4a5e-8282-5504fb7ae9ae"));

                    ToolStripMenuItem listItem = GH_DocumentObject.Menu_AppendItem(menu, "List");
                    listItem.Image = proxy.Icon;
                    CreateQuickWireMenu(listItem.DropDown, GH_ComponentAttributesReplacer.StaticCreateObjectItems.ListItems, param, (sender, e) =>
                    {
                        ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;

                        if (toolStripMenuItem != null && toolStripMenuItem.Tag != null && toolStripMenuItem.Tag is CreateObjectItem[])
                        {
                            ObservableCollection<CreateObjectItem> structureLists = new ObservableCollection<CreateObjectItem>((CreateObjectItem[])toolStripMenuItem.Tag);
                            new QuickWireEditor(isInput, proxy.Icon, "List", structureLists, (par) => par.Access == GH_ParamAccess.list && par is Param_GenericObject,
                                (arr, isIn) =>
                                {
                                    GH_ComponentAttributesReplacer.StaticCreateObjectItems.ListItems = arr;

                                }).Show();
                        }
                    });

                    if(param.VolatileData.PathCount > 1)
                    {
                        GH_GraftTreeComponent tree = new GH_GraftTreeComponent();
                        ToolStripMenuItem treeItem = GH_DocumentObject.Menu_AppendItem(menu, "Tree");
                        treeItem.Image = tree.Icon_24x24;
                        CreateQuickWireMenu(treeItem.DropDown, GH_ComponentAttributesReplacer.StaticCreateObjectItems.TreeItems, param, (sender, e) =>
                        {
                            ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;

                            if (toolStripMenuItem != null && toolStripMenuItem.Tag != null && toolStripMenuItem.Tag is CreateObjectItem[])
                            {
                                ObservableCollection<CreateObjectItem> structureLists = new ObservableCollection<CreateObjectItem>((CreateObjectItem[])toolStripMenuItem.Tag);
                                new QuickWireEditor(isInput, tree.Icon_24x24, "Tree", structureLists, (par) => par.Access == GH_ParamAccess.tree && par is Param_GenericObject,
                                    (arr, isIn) =>
                                    {
                                        GH_ComponentAttributesReplacer.StaticCreateObjectItems.TreeItems = arr;

                                    }).Show();
                            }
                        });
                    }
                }


                //Curve
                if (guid == new Param_Rectangle().ComponentGuid || guid == new Param_Circle().ComponentGuid || guid == new Param_Arc().ComponentGuid
                    || guid == new Param_Line().ComponentGuid)
                {
                    Param_Curve curve = new Param_Curve();
                    ToolStripMenuItem item = GH_DocumentObject.Menu_AppendItem(menu, curve.Name);
                    item.Image = curve.Icon_24x24;
                    item.DropDown = RespondToQuickWire(param, curve.ComponentGuid, isInput, false);
                }

                //Brep
                if (guid == new Param_Surface().ComponentGuid || guid == new Guid("{89CD1A12-0007-4581-99BA-66578665E610}"))
                {
                    Param_Brep brep = new Param_Brep();
                    ToolStripMenuItem item = GH_DocumentObject.Menu_AppendItem(menu, brep.Name);
                    item.Image = brep.Icon_24x24;
                    item.DropDown = RespondToQuickWire(param, brep.ComponentGuid, isInput, false);
                }

                //Geometry
                if (guid == new Param_Rectangle().ComponentGuid || guid == new Param_Circle().ComponentGuid || guid == new Param_Arc().ComponentGuid || guid == new Param_Line().ComponentGuid 
                    || guid == new Param_Point().ComponentGuid ||guid == new Param_Plane().ComponentGuid || guid == new Param_Vector().ComponentGuid
                    || guid == new Param_Curve().ComponentGuid || guid == new Param_Surface().ComponentGuid || guid == new Param_Brep().ComponentGuid || guid == new Param_Group().ComponentGuid
                    || guid == new Param_Mesh().ComponentGuid || guid == new Guid("{89CD1A12-0007-4581-99BA-66578665E610}") || guid == new Param_Box().ComponentGuid)
                {
                    Param_Geometry geo = new Param_Geometry();
                    ToolStripMenuItem item = GH_DocumentObject.Menu_AppendItem(menu, geo.Name);
                    item.Image = geo.Icon_24x24;
                    item.DropDown = RespondToQuickWire(param, geo.ComponentGuid, isInput, false);
                }

                //General
                if(guid != new Param_GenericObject().ComponentGuid)
                {
                    Param_GenericObject general = new Param_GenericObject();
                    ToolStripMenuItem item = GH_DocumentObject.Menu_AppendItem(menu, general.Name);
                    item.Image = general.Icon_24x24;
                    item.DropDown = RespondToQuickWire(param, general.ComponentGuid, isInput, false);
                }
                GH_DocumentObject.Menu_AppendSeparator(menu);
            }

            CreateQuickWireMenu(menu, items, param, (sender, e) => Menu_EditItemClicked(sender, guid, param, isInput));

            return menu;
        }

        private static void CreateQuickWireMenu(ToolStrip menu, CreateObjectItem[] items, IGH_Param param, EventHandler click) 
        {
            foreach (CreateObjectItem createItem in items)
            {
                ToolStripMenuItem item = GH_DocumentObject.Menu_AppendItem(menu, createItem.ShowName, (sender, e) => Menu_CreateItemClicked(sender, param), createItem.Icon);
                item.Tag = createItem;
                if (!string.IsNullOrEmpty(createItem.InitString))
                {
                    item.ToolTipText = $"Init String:\n{createItem.InitString}";
                }
                else
                {
                    item.ToolTipText = "No Init String.";
                }
            }
            ToolStripMenuItem editItem = GH_DocumentObject.Menu_AppendItem(menu, "Edit", click);
            editItem.Image = Properties.Resources.EditIcon_24;
            editItem.Tag = items;
            editItem.ForeColor = Color.DimGray;
        }

        private static void Menu_CreateItemClicked(object sender, IGH_Param param)
        {
            ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
            if (toolStripMenuItem != null && toolStripMenuItem.Tag != null && toolStripMenuItem.Tag is CreateObjectItem)
            {
                CreateObjectItem createItem = (CreateObjectItem)toolStripMenuItem.Tag;
                createItem.CreateObject(param);
                return;
            }
            MessageBox.Show("Something wrong with create object.");
        }

        private static void Menu_EditItemClicked(object sender, Guid guid, IGH_Param param, bool isInput)
        {
            ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;

            string name;
            Bitmap icon;
            IGH_ObjectProxy proxy = Instances.ComponentServer.EmitObjectProxy(guid);
            if (proxy == null) 
            {
                name = param.Name;
                icon = param.Icon_24x24;
            }
            else
            {
                name = proxy.Desc.Name;
                icon = proxy.Icon;
            }

            if (toolStripMenuItem != null && toolStripMenuItem.Tag != null && toolStripMenuItem.Tag is CreateObjectItem[])
            {
                ObservableCollection<CreateObjectItem> structureLists = new ObservableCollection<CreateObjectItem>((CreateObjectItem[])toolStripMenuItem.Tag);
                new QuickWireEditor(isInput, icon, name, structureLists, (par)=> par.ComponentGuid == guid, 
                    (arr, isIn) =>
                    {
                        if (isIn)
                            GH_ComponentAttributesReplacer.StaticCreateObjectItems.InputItems[guid] = arr;
                        else
                            GH_ComponentAttributesReplacer.StaticCreateObjectItems.OutputItems[guid] = arr;

                    }).Show();
            }
        }
    }
}
