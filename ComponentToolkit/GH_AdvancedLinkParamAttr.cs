using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
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
    internal class GH_AdvancedLinkParamAttr : GH_LinkedParamAttributes
    {
        public int StringWidth => GH_FontServer.StringWidth(Owner.NickName, GH_FontServer.StandardAdjusted);
        public int ControlWidth => Control?.Width ?? 0;
        public int WholeWidth => StringWidth + (ControlWidth == 0 ? 0 : ControlWidth + Datas.ComponentControlNameDistance) + 
            (Datas.ShowLinkParamIcon ? Datas.ComponentParamIconSize + Datas.ComponentIconDistance : 0);
        public int ParamHeight => Math.Max(20, (Control?.Height ?? 0) + 3);

        public BaseControlItem Control { get; private set; } = null;

        public RectangleF StringRect { get; set; }

        public PointF IconPivot { get; set; }

        public static SortedList<Guid, Bitmap> IconSet = new SortedList<Guid, Bitmap>();

        public GH_AdvancedLinkParamAttr(IGH_Param param, IGH_Attributes parent) : base(param, parent)
        {
            if (param.Kind != GH_ParamKind.input) return;
            Control = SetControl(param);
            SetParamIcon();
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

            int iconSize = Datas.ComponentParamIconSize;
            Bitmap outBitmap = new Bitmap(iconSize, iconSize);
            Graphics g = Graphics.FromImage(outBitmap);
            g.DrawImage(bitmap, new Rectangle(0, 0, iconSize, iconSize), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, attr);
            g.Dispose();

            return outBitmap;
        }

        internal static BaseControlItem SetControl(IGH_Param param)
        {
            if(param is Param_ScriptVariable)
            {
                return new ParamVariableControl((Param_ScriptVariable)param);
            }

            if (IsPersistentParam(param.GetType(), out Type storeType))
            {
                if (storeType == typeof(GH_String))
                {
                    return new ParamStringControl((GH_PersistentParam<GH_String>)param);
                }
                else if (storeType == typeof(GH_Integer))
                {
                    return new ParamIntegerControl((GH_PersistentParam<GH_Integer>)param);
                }
                else if (storeType == typeof(GH_Number))
                {
                    return new ParamNumberControl((GH_PersistentParam<GH_Number>)param);
                }
                else if (storeType == typeof(GH_Colour))
                {
                    return new ParamColourControl((GH_PersistentParam<GH_Colour>)param);
                }
                else if (storeType == typeof(GH_Boolean))
                {
                    return new ParamBooleanControl((GH_PersistentParam<GH_Boolean>)param);
                }
                else if (storeType == typeof(GH_Material))
                {
                    return new ParamMaterialControl((GH_PersistentParam<GH_Material>)param);
                }

                else if (storeType == typeof(GH_Interval))
                {
                    return new ParamIntervalControl((GH_PersistentParam<GH_Interval>)param);
                }
                else if (storeType == typeof(GH_Point))
                {
                    return new ParamPointControl((GH_PersistentParam<GH_Point>)param);
                }
                else if (storeType == typeof(GH_Vector))
                {
                    return new ParamVectorControl((GH_PersistentParam<GH_Vector>)param);
                }
                else if (storeType == typeof(GH_ComplexNumber))
                {
                    return new ParamComplexControl((GH_PersistentParam<GH_ComplexNumber>)param);
                }
                else if (storeType == typeof(GH_Interval2D))
                {
                    return new ParamInterval2DControl((GH_PersistentParam<GH_Interval2D>)param);
                }
                else if (storeType == typeof(GH_Line))
                {
                    return new ParamLineControl((GH_PersistentParam<GH_Line>)param);
                }
                else if (storeType == typeof(GH_Plane))
                {
                    return new ParamPlaneControl((GH_PersistentParam<GH_Plane>)param);
                }
                else if (storeType == typeof(GH_Circle))
                {
                    return new ParamCircleControl((GH_PersistentParam<GH_Circle>)param);
                }
                else if (storeType == typeof(GH_Rectangle))
                {
                    return new ParamRectangleControl((GH_PersistentParam<GH_Rectangle>)param);
                }
                else if (storeType == typeof(GH_Box))
                {
                    return new ParamBoxControl((GH_PersistentParam<GH_Box>)param);
                }

                else
                {
                    Type controlType = typeof(ParamGeneralControl<>).MakeGenericType(storeType);
                    return (BaseControlItem)Activator.CreateInstance(controlType, param);
                }
            }
            return null;

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
                if (StringRect.Contains(e.CanvasLocation) ||
                    (Datas.ShowLinkParamIcon && new RectangleF(IconPivot, new Size(Datas.ComponentParamIconSize, Datas.ComponentParamIconSize)).Contains(e.CanvasLocation)))
                {
                    RespondToQuickWire(Owner, Owner.Kind == GH_ParamKind.input).Show(sender, e.ControlLocation);
                    return GH_ObjectResponse.Release;
                }
            } 
            return base.RespondToMouseUp(sender, e);
        }

        internal static ToolStripDropDownMenu RespondToQuickWire(IGH_Param param, bool isInput)
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

            Guid guid = param.ComponentGuid;
            if(param is Param_ScriptVariable)
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
            ToolStripMenuItem editItem = GH_DocumentObject.Menu_AppendItem(menu, "Edit", (sender, e) => Menu_EditItemClicked(sender, guid, isInput));
            editItem.Image = Properties.Resources.EditIcon_24;
            editItem.Tag = items;
            editItem.ForeColor = Color.DimGray;

            return menu;
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

        private static void Menu_EditItemClicked(object sender, Guid guid, bool isInput)
        {
            ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;

            IGH_ObjectProxy proxy = Grasshopper.Instances.ComponentServer.EmitObjectProxy(guid);
            if (proxy == null) return;

            if (toolStripMenuItem != null && toolStripMenuItem.Tag != null && toolStripMenuItem.Tag is CreateObjectItem[])
            {
                ObservableCollection<CreateObjectItem> structureLists = new ObservableCollection<CreateObjectItem>((CreateObjectItem[])toolStripMenuItem.Tag);
                new QuickWireEditor(guid, isInput, proxy.Icon, proxy.Desc.Name, structureLists).Show();
            }
        }
    }
}
