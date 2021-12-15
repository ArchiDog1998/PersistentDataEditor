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

        private IGumball _gumball;


        public GH_AdvancedLinkParamAttr(IGH_Param param, IGH_Attributes parent) : base(param, parent)
        {
            _gumball = GH_AdvancedFloatingParamAttr.SetGumball(param);
            if (_gumball != null)
            {
                param.SolutionExpired += Param_SolutionExpired;
            }
            SetControl();
            SetParamIcon();
        }

        public void ShowAllGumballs()
        {
            if (_gumball != null)
            {
                _gumball.ShowAllGumballs();
            }
        }

        public void CloseAllGumballs()
        {
            if (_gumball != null)
            {
                _gumball.Dispose();
            }
        }

        private void Param_SolutionExpired(IGH_DocumentObject sender, GH_SolutionExpiredEventArgs e)
        {
            if (base.Owner == null)
            {
                Owner.SolutionExpired -= Param_SolutionExpired;
            }
            else if (base.Owner.OnPingDocument() == null)
            {
                Owner.SolutionExpired -= Param_SolutionExpired;
            }
            if (_gumball != null && !_gumball.IsMouseUp)
                _gumball.ShowAllGumballs();
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


            if (IsPersistentParam(param.GetType(), out Type storeType))
            {
                if (storeType == typeof(GH_Curve) || storeType == typeof(GH_Brep) || storeType == typeof(GH_Surface) || storeType == typeof(GH_Mesh) || storeType.FullName == "Grasshopper.Kernel.Types.GH_SubD" || storeType == typeof(IGH_GeometricGoo)
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
            return base.RespondToMouseUp(sender, e);
        }

    }
}
