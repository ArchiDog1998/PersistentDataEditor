using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;

namespace PersistentDataEditor;

internal class GH_AdvancedLinkParamAttr : GH_LinkedParamAttributes, IControlAttr, IDisposable
{
    public int StringWidth => GH_FontServer.StringWidth(Owner.NickName, GH_FontServer.StandardAdjusted);
    public int ControlWidth => Control?.Width ?? 0;
    public int WholeWidth => StringWidth + (ControlWidth == 0 ? 0 : ControlWidth + NewData.ComponentControlNameDistance) +
        (NewData.ShowLinkParamIcon ? NewData.ComponentParamIconSize + NewData.ComponentIconDistance : 0);
    public int ParamHeight => Math.Max(20, (Control?.Height ?? 0) + 3);

    public BaseControlItem Control { get; private set; }

    public RectangleF StringRect { get; set; }

    public RectangleF IconRect { get; set; }

    public static SortedList<Guid, Bitmap> IconSet = new SortedList<Guid, Bitmap>();

    private IGumball _gumball;

    private MethodInfo _expressionInfo;

    public GH_AdvancedLinkParamAttr(IGH_Param param, IGH_Attributes parent) : base(param, parent)
    {
        _gumball = GH_AdvancedFloatingParamAttr.SetGumball(param);
        if (_gumball != null)
        {
            param.SolutionExpired += Param_SolutionExpired;
        }
        SetControl();
        SetParamIcon();

        if (IsExpressionParam(Owner.GetType(), out Type dataType))
        {
            Type type = typeof(GH_ExpressionParam<>).MakeGenericType(dataType);
            _expressionInfo = type.FindMethod("Menu_ExpressionEditorClick");
        }
    }

    public void ShowAllGumballs()
    {
        _gumball?.ShowAllGumballs();
    }

    public void Dispose()
    {
        _gumball?.Dispose();
    }

    private void Param_SolutionExpired(IGH_DocumentObject sender, GH_SolutionExpiredEventArgs e)
    {
        if (Owner == null)
        {
            Owner.SolutionExpired -= Param_SolutionExpired;
        }
        else if (Owner.OnPingDocument() == null)
        {
            Owner.SolutionExpired -= Param_SolutionExpired;
        }
    }

    public void SetControl()
    {
        if (NewData.UseParamControl && Owner.Kind == GH_ParamKind.input && NewData.ComponentUseControl)
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
            IGH_ObjectProxy proxy = Instances.ComponentServer.EmitObjectProxy(item.Key);
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
        float opacity = useOpacity ? (float)NewData.ComponentIconOpacity : 1;
        float[][] nArray =
        [
            [1, 0, 0, 0, 0],
            [0, 1, 0, 0, 0],
            [0, 0, 1, 0, 0],
            [0, 0, 0, opacity, 0],
            [0, 0, 0, 0, 1],
        ];

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
        if (param is Param_ScriptVariable variable)
        {
            return NewData.UseParamScriptControl ? new ParamVariableControl(variable) : null;
        }
        if (NewData.OnlyItemAccessControl && param.Access != GH_ParamAccess.item) return null;


        if (IsPersistentParam(param.GetType(), out Type storeType))
        {
            if (storeType == typeof(GH_Curve) || storeType == typeof(GH_Brep) || storeType == typeof(GH_Surface) || storeType == typeof(GH_Mesh) || storeType.FullName == "Grasshopper.Kernel.Types.GH_SubD" || storeType == typeof(IGH_GeometricGoo)
                || storeType == typeof(GH_Field) || storeType == typeof(GH_Transform) || storeType == typeof(GH_Matrix))
            {
                return null;
            }
            else if (storeType == typeof(GH_String))
            {
                return NewData.UseParamStringControl ? new ParamStringControl((GH_PersistentParam<GH_String>)param) : null;
            }
            else if (storeType == typeof(GH_Integer))
            {
                return NewData.UseParamIntegerControl ? new ParamIntegerControl((GH_PersistentParam<GH_Integer>)param) : null;
            }
            else if (storeType == typeof(GH_Number))
            {
                return NewData.UseParamNumberControl ? new ParamNumberControl((GH_PersistentParam<GH_Number>)param) : null;
            }
            else if (storeType == typeof(GH_Colour))
            {
                return NewData.UseParamColourControl ? new ParamColourControl((GH_PersistentParam<GH_Colour>)param) : null;
            }
            else if (storeType == typeof(GH_Boolean))
            {
                return NewData.UseParamBooleanControl ? new ParamBooleanControl((GH_PersistentParam<GH_Boolean>)param) : null;
            }
            else if (storeType == typeof(GH_Material))
            {
                return NewData.UseParamMaterialControl ? new ParamMaterialControl((GH_PersistentParam<GH_Material>)param) : null;
            }
            else if (storeType == typeof(GH_Interval))
            {
                return NewData.UseParamDomainControl ? new ParamIntervalControl((GH_PersistentParam<GH_Interval>)param) : null;
            }
            else if (storeType == typeof(GH_Point))
            {
                return NewData.UseParamPointControl ? new ParamPointControl((GH_PersistentParam<GH_Point>)param) : null;
            }
            else if (storeType == typeof(GH_Vector))
            {
                return NewData.UseParamVectorControl ? new ParamVectorControl((GH_PersistentParam<GH_Vector>)param) : null;
            }
            else if (storeType == typeof(GH_ComplexNumber))
            {
                return NewData.UseParamComplexControl ? new ParamComplexControl((GH_PersistentParam<GH_ComplexNumber>)param) : null;
            }
            else if (storeType == typeof(GH_Interval2D))
            {
                return NewData.UseParamDomain2Control ? new ParamInterval2DControl((GH_PersistentParam<GH_Interval2D>)param) : null;
            }
            else if (storeType == typeof(GH_Line))
            {
                return NewData.UseParamLineControl ? new ParamLineControl((GH_PersistentParam<GH_Line>)param) : null;
            }
            else if (storeType == typeof(GH_Plane))
            {
                return NewData.UseParamPlaneControl ? new ParamPlaneControl((GH_PersistentParam<GH_Plane>)param) : null;
            }
            else if (storeType == typeof(GH_Circle))
            {
                return NewData.UseParamCircleControl ? new ParamCircleControl((GH_PersistentParam<GH_Circle>)param) : null;
            }
            else if (storeType == typeof(GH_Rectangle))
            {
                return NewData.UseParamRectangleControl ? new ParamRectangleControl((GH_PersistentParam<GH_Rectangle>)param) : null;
            }
            else if (storeType == typeof(GH_Box))
            {
                return NewData.UseParamBoxControl ? new ParamBoxControl((GH_PersistentParam<GH_Box>)param) : null;
            }
            else if (storeType == typeof(GH_Arc))
            {
                return NewData.UseParamArcControl ? new ParamArcControl((GH_PersistentParam<GH_Arc>)param) : null;
            }

            else if (NewData.UseParamGeneralControl)
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

    internal static bool IsExpressionParam(Type type, out Type dataType)
    {
        dataType = null;
        if (type == null)
        {
            return false;
        }
        else if (type.IsGenericType)
        {
            if (type.GetGenericTypeDefinition() == typeof(GH_ExpressionParam<>))
            {
                dataType = type.GenericTypeArguments[0];
                return true;
            }
            else if (type.GetGenericTypeDefinition() == typeof(GH_PersistentParam<>))
                return false;
        }
        return IsExpressionParam(type.BaseType, out dataType);
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

    public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        if (_expressionInfo != null)
        {
            _expressionInfo.Invoke(Owner, new object[] { Instances.DocumentEditor, new EventArgs() });
            return GH_ObjectResponse.Release;
        }
        return base.RespondToMouseDoubleClick(sender, e);
    }
}
