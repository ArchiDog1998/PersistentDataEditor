using System.Drawing;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.GUI.Canvas;
using SimpleGrasshopper.Attributes;
using System.Linq;

namespace PersistentDataEditor;


internal static class Datas
{
    #region Colors
    public static bool IsCurrectObjectLock { private get; set; } = false;

    public static readonly Color _controlForegroundColorDefault = Color.FromArgb(40, 40, 40);
    public static Color ControlForegroundColor
    {
        get
        {
            Color color = Instances.Settings.GetValue(nameof(ControlForegroundColor), _controlForegroundColorDefault);
            return ChangeColourAlpha(color);
        }
        set
        {
            Instances.Settings.SetValue(nameof(ControlForegroundColor), value);
            Instances.RedrawCanvas();
        }
    }

    public static readonly Color _controlTextgroundColorDefault = Color.FromArgb(40, 40, 40);
    public static Color ControlTextgroundColor
    {
        get
        {
            Color color = Instances.Settings.GetValue(nameof(ControlTextgroundColor), _controlTextgroundColorDefault);
            return ChangeColourAlpha(color);
        }
        set
        {
            Instances.Settings.SetValue(nameof(ControlTextgroundColor), value);
            Instances.RedrawCanvas();
        }
    }

    public static readonly Color _controlBackgroundColorDefault = Color.FromArgb(150, Color.WhiteSmoke);
    public static Color ControlBackgroundColor
    {
        get
        {
            Color color = Instances.Settings.GetValue(nameof(ControlBackgroundColor), _controlBackgroundColorDefault);
            return ChangeColourAlpha(color);
        }
        set
        {
            Instances.Settings.SetValue(nameof(ControlBackgroundColor), value);
            Instances.RedrawCanvas();
        }
    }

    public static readonly Color _controlBorderColorDefault = Color.FromArgb(40, 40, 40);
    public static Color ControlBorderColor
    {
        get
        {
            Color color = Instances.Settings.GetValue(nameof(ControlBorderColor), _controlBorderColorDefault);
            return ChangeColourAlpha(color);
        }
        set
        {
            Instances.Settings.SetValue(nameof(ControlBorderColor), value);
            Instances.RedrawCanvas();
        }
    }
    private static Color ChangeColourAlpha(Color color)
    {
        return Color.FromArgb((int)(GH_Canvas.ZoomFadeLow / 255f * color.A * (IsCurrectObjectLock ? 0.5 : 1)), color);
    }
    #endregion

    #region AAA
    public static readonly int _componentParamIconSizeDefault = 16;
    public static int ComponentParamIconSize
    {
        get => Instances.Settings.GetValue(nameof(ComponentParamIconSize), _componentParamIconSizeDefault);
        set
        {
            Instances.Settings.SetValue(nameof(ComponentParamIconSize), value);
            GH_AdvancedLinkParamAttr.UpdataIcons();
            RefreshLayout();
        }
    }

    public static readonly double _componentIconOpacityDefault = 0.6;
    public static double ComponentIconOpacity
    {
        get => Instances.Settings.GetValue(nameof(ComponentIconOpacity), _componentIconOpacityDefault);
        set
        {
            Instances.Settings.SetValue(nameof(ComponentIconOpacity), value);
            GH_AdvancedLinkParamAttr.UpdataIcons();
            RefreshLayout();
        }
    }
    #endregion
    public static readonly int MiniWidth = 6;
    #region Settings For Layout
    public static readonly int _componentIconDistanceDefault = 2;
    public static int ComponentIconDistance
    {
        get => Instances.Settings.GetValue(nameof(ComponentIconDistance), _componentIconDistanceDefault);
        set
        {
            Instances.Settings.SetValue(nameof(ComponentIconDistance), value);
            RefreshLayout();
        }
    }

    public static readonly int _componentToEdgeDistanceDefault = 3;
    public static int ComponentToEdgeDistance
    {
        get => Instances.Settings.GetValue(nameof(ComponentToEdgeDistance), _componentToEdgeDistanceDefault);
        set
        {
            Instances.Settings.SetValue(nameof(ComponentToEdgeDistance), value);
            RefreshLayout();
        }
    }
    public static readonly int _componentToCoreDistanceDefault = 3;
    public static int ComponentToCoreDistance
    {
        get => Instances.Settings.GetValue(nameof(ComponentToCoreDistance), _componentToCoreDistanceDefault);
        set
        {
            Instances.Settings.SetValue(nameof(ComponentToCoreDistance), value);
            RefreshLayout();
        }
    }

    public static readonly int _componentControlNameDistanceDefault = 2;
    public static int ComponentControlNameDistance
    {
        get => Instances.Settings.GetValue(nameof(ComponentControlNameDistance), _componentControlNameDistanceDefault);
        set
        {
            Instances.Settings.SetValue(nameof(ComponentControlNameDistance), value);
            RefreshLayout();
        }
    }

    public static readonly int _paramsEdgeDistanceDefault = 5;
    public static int ParamsEdgeDistance
    {
        get => Instances.Settings.GetValue(nameof(ParamsEdgeDistance), _paramsEdgeDistanceDefault);
        set
        {
            Instances.Settings.SetValue(nameof(ParamsEdgeDistance), value);
            RefreshLayout();
        }
    }

    public static readonly int _paramsCoreDistanceDefault = 3;
    public static int ParamsCoreDistance
    {
        get => Instances.Settings.GetValue(nameof(ParamsCoreDistance), _paramsCoreDistanceDefault);
        set
        {
            Instances.Settings.SetValue(nameof(ParamsCoreDistance), value);
            RefreshLayout();
        }
    }

    public static readonly int _inputBoxControlMaxWidthDefault = 100;
    public static int InputBoxControlMaxWidth
    {
        get => Instances.Settings.GetValue(nameof(InputBoxControlMaxWidth), _inputBoxControlMaxWidthDefault);
        set
        {
            Instances.Settings.SetValue(nameof(InputBoxControlMaxWidth), value);
            RefreshLayout();
        }
    }

    public static int AdditionWidth => ComponentToEdgeDistance + ComponentToCoreDistance;

    public static bool UseParamControl
    {
        get => Instances.Settings.GetValue(nameof(UseParamControl), true);
        set
        {
            Instances.Settings.SetValue(nameof(UseParamControl), value);
            ChangeControl();
        }
    }

    public static bool TextboxInputAutoApply
    {
        get => Instances.Settings.GetValue(nameof(TextboxInputAutoApply), true);
        set => Instances.Settings.SetValue(nameof(TextboxInputAutoApply), value);
    }

    public static bool UseDefaultValueToControl
    {
        get => Instances.Settings.GetValue(nameof(UseDefaultValueToControl), true);
        set => Instances.Settings.SetValue(nameof(UseDefaultValueToControl), value);
    }

    public static bool OnlyItemAccessControl
    {
        get => Instances.Settings.GetValue(nameof(OnlyItemAccessControl), true);
        set
        {
            Instances.Settings.SetValue(nameof(OnlyItemAccessControl), value);
            ChangeControl();
        }
    }

    public static bool ComponentUseControl
    {
        get => Instances.Settings.GetValue(nameof(ComponentUseControl), true);
        set
        {
            Instances.Settings.SetValue(nameof(ComponentUseControl), value);
            ChangeControl();
        }
    }

    public static bool ParamUseControl
    {
        get => Instances.Settings.GetValue(nameof(ParamUseControl), true);
        set
        {
            Instances.Settings.SetValue(nameof(ParamUseControl), value);
            ChangeControl();
        }
    }


    public static bool OnlyShowSelectedObjectControl
    {
        get => Instances.Settings.GetValue(nameof(OnlyShowSelectedObjectControl), false);
        set
        {
            Instances.Settings.SetValue(nameof(OnlyShowSelectedObjectControl), value);
            RefreshLayout();
        }
    }

    public static bool ComponentInputEdgeLayout
    {
        get => Instances.Settings.GetValue(nameof(ComponentInputEdgeLayout), false);
        set
        {
            Instances.Settings.SetValue(nameof(ComponentInputEdgeLayout), value);
            RefreshLayout();
        }
    }

    public static bool ComponentOutputEdgeLayout
    {
        get => Instances.Settings.GetValue(nameof(ComponentOutputEdgeLayout), false);
        set
        {
            Instances.Settings.SetValue(nameof(ComponentOutputEdgeLayout), value);
            RefreshLayout();
        }
    }

    public static bool ControlAlignRightLayout
    {
        get => Instances.Settings.GetValue(nameof(ControlAlignRightLayout), true);
        set
        {
            Instances.Settings.SetValue(nameof(ControlAlignRightLayout), value);
            RefreshLayout();
        }
    }

    public static bool SeperateCalculateWidthControl
    {
        get => Instances.Settings.GetValue(nameof(SeperateCalculateWidthControl), true);
        set
        {
            Instances.Settings.SetValue(nameof(SeperateCalculateWidthControl), value);
            RefreshLayout();
        }
    }

    #endregion

    internal static void ChangeControl()
    {
        foreach (GH_Document doc in Instances.DocumentServer)
        {
            foreach (IGH_Attributes attr in doc.Attributes)
            {
                if (attr is IControlAttr controlAttr)
                {
                    controlAttr.SetControl();
                }
                attr.ExpireLayout();
            }
        }
        Instances.RedrawCanvas();
    }

    internal static void ChangeGumball()
    {
        foreach (GH_Document doc in Instances.DocumentServer)
        {
            foreach (IGH_Attributes attr in doc.Attributes)
            {
                if (attr is GH_AdvancedFloatingParamAttr paramAttr)
                {
                    paramAttr.RedrawGumballs();
                }
            }
        }
    }

    internal static void RefreshLayout()
    {
        foreach (GH_Document doc in Instances.DocumentServer)
        {
            foreach (IGH_Attributes attr in doc.Attributes)
            {
                //Refresh Variable Control.
                if (attr is GH_AdvancedLinkParamAttr att)
                {
                    if (att.Control is ParamVariableControl)
                        att.Control?.ChangeControlItems();
                }
                attr.ExpireLayout();
            }
        }
        Instances.RedrawCanvas();
    }

    
    public static bool ShowLinkParamIcon
    {
        get => Instances.Settings.GetValue(nameof(ShowLinkParamIcon), false);
        set
        {
            Instances.Settings.SetValue(nameof(ShowLinkParamIcon), value);
            RefreshLayout();
        }
    }

    public static readonly int _gumballMaxShowCountDefault = 10;
    public static int GumballMaxShowCount
    {
        get => Instances.Settings.GetValue(nameof(GumballMaxShowCount), _gumballMaxShowCountDefault);
        set
        {
            Instances.Settings.SetValue(nameof(GumballMaxShowCount), value);
            ChangeGumball();
        }
    }

    public static readonly int _paramGumballRadiusDefault = 50;
    public static int ParamGumballRadius
    {
        get => Instances.Settings.GetValue(nameof(ParamGumballRadius), _paramGumballRadiusDefault);
        set
        {
            Instances.Settings.SetValue(nameof(ParamGumballRadius), value);
            ChangeGumball();
        }
    }

    public static bool UseGeoParamGumball
    {
        get => Instances.Settings.GetValue(nameof(UseGeoParamGumball), true);
        set
        {
            Instances.Settings.SetValue(nameof(UseGeoParamGumball), value);
            ChangeGumball();
        }
    }

    public static bool GeoParamGumballRotate
    {
        get => Instances.Settings.GetValue(nameof(GeoParamGumballRotate), true);
        set
        {
            Instances.Settings.SetValue(nameof(GeoParamGumballRotate), value);
            ChangeGumball();
        }
    }

    public static bool GeoParamGumballScale
    {
        get => Instances.Settings.GetValue(nameof(GeoParamGumballScale), true);
        set
        {
            Instances.Settings.SetValue(nameof(GeoParamGumballScale), value);
            ChangeGumball();
        }
    }

    public static readonly int _paramGumballWirePreviewThicknessDefault = 5;
    public static int ParamGumballWirePreviewThickness
    {
        get => Instances.Settings.GetValue(nameof(ParamGumballWirePreviewThickness), _paramGumballWirePreviewThicknessDefault);
        set => Instances.Settings.SetValue(nameof(ParamGumballWirePreviewThickness), value);
    }

    public static readonly Color _paramGumballPreviewWireColorDefault = Color.DarkBlue;
    public static Color ParamGumballPreviewWireColor
    {
        get => Instances.Settings.GetValue(nameof(ParamGumballPreviewWireColor), _paramGumballPreviewWireColorDefault);
        set => Instances.Settings.SetValue(nameof(ParamGumballPreviewWireColor), value);
    }

    public static readonly Color _paramGumballPreviewMeshColorDefault = Color.DarkBlue;
    public static Color ParamGumballPreviewMeshColor
    {
        get => Instances.Settings.GetValue(nameof(ParamGumballPreviewMeshColor), _paramGumballPreviewMeshColorDefault);
        set => Instances.Settings.SetValue(nameof(ParamGumballPreviewMeshColor), value);
    }
}
