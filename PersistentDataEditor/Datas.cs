using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.GUI.Canvas;

namespace PersistentDataEditor
{
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

        #region Settings For Layout
        public static readonly int MiniWidth = 6;

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
            get => Grasshopper.Instances.Settings.GetValue(nameof(UseParamControl), true);
            set
            {
                Instances.Settings.SetValue(nameof(UseParamControl), value);
                ChangeControl();
            }
        }

        public static bool OnlyItemAccessControl
        {
            get => Grasshopper.Instances.Settings.GetValue(nameof(OnlyItemAccessControl), true);
            set
            {
                Instances.Settings.SetValue(nameof(OnlyItemAccessControl), value);
                ChangeControl();
            }
        }

        public static bool ComponentUseControl
        {
            get => Grasshopper.Instances.Settings.GetValue(nameof(ComponentUseControl), true);
            set
            {
                Instances.Settings.SetValue(nameof(ComponentUseControl), value);
                ChangeControl();
            }
        }

        public static bool ParamUseControl
        {
            get => Grasshopper.Instances.Settings.GetValue(nameof(ParamUseControl), true);
            set
            {
                Instances.Settings.SetValue(nameof(ParamUseControl), value);
                ChangeControl();
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
                    if (attr is IControlAttr)
                    {
                        ((IControlAttr)attr).SetControl();
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
                    if (attr is GH_AdvancedFloatingParamAttr)
                    {
                        ((GH_AdvancedFloatingParamAttr)attr).RedrawGumballs();
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
                    if (attr is GH_AdvancedLinkParamAttr)
                    {
                        GH_AdvancedLinkParamAttr att = (GH_AdvancedLinkParamAttr)attr;
                        if (att.Control is ParamVariableControl)
                            att.Control?.ChangeControlItems();
                    }
                    attr.ExpireLayout();
                }
            }
            Instances.RedrawCanvas();
        }

        #region Settings for Using Control Item
        public static bool UseParamBooleanControl
        {
            get => Instances.Settings.GetValue(nameof(UseParamBooleanControl), true);
            set
            {
                Instances.Settings.SetValue(nameof(UseParamBooleanControl), value);
                RefreshLayout();
            }
        }

        public static bool UseParamColourControl
        {
            get => Instances.Settings.GetValue(nameof(UseParamColourControl), true);
            set
            {
                Instances.Settings.SetValue(nameof(UseParamColourControl), value);
                RefreshLayout();
            }
        }

        public static bool UseParamIntegerControl
        {
            get => Instances.Settings.GetValue(nameof(UseParamIntegerControl), true);
            set
            {
                Instances.Settings.SetValue(nameof(UseParamIntegerControl), value);
                RefreshLayout();
            }
        }

        public static bool UseParamIntervalControl
        {
            get => Instances.Settings.GetValue(nameof(UseParamIntervalControl), true);
            set
            {
                Instances.Settings.SetValue(nameof(UseParamIntervalControl), value);
                RefreshLayout();
            }
        }

        public static bool UseParamNumberControl
        {
            get => Instances.Settings.GetValue(nameof(UseParamNumberControl), true);
            set
            {
                Instances.Settings.SetValue(nameof(UseParamNumberControl), value);
                RefreshLayout();
            }
        }

        public static bool UseParamPointControl
        {
            get => Instances.Settings.GetValue(nameof(UseParamPointControl), true);
            set
            {
                Instances.Settings.SetValue(nameof(UseParamPointControl), value);
                RefreshLayout();
            }
        }

        public static bool UseParamStringControl
        {
            get => Instances.Settings.GetValue(nameof(UseParamStringControl), true);
            set
            {
                Instances.Settings.SetValue(nameof(UseParamStringControl), value);
                RefreshLayout();
            }
        }

        public static bool UseParamVectorControl
        {
            get => Instances.Settings.GetValue(nameof(UseParamVectorControl), true);
            set
            {
                Instances.Settings.SetValue(nameof(UseParamVectorControl), value);
                RefreshLayout();
            }
        }

        public static bool UseParamComplexControl
        {
            get => Instances.Settings.GetValue(nameof(UseParamComplexControl), true);
            set
            {
                Instances.Settings.SetValue(nameof(UseParamComplexControl), value);
                RefreshLayout();
            }
        }

        public static bool UseParamInterval2DControl
        {
            get => Instances.Settings.GetValue(nameof(UseParamInterval2DControl), false);
            set
            {
                Instances.Settings.SetValue(nameof(UseParamInterval2DControl), value);
                RefreshLayout();
            }
        }

        public static bool UseParamLineControl
        {
            get => Instances.Settings.GetValue(nameof(UseParamLineControl), false);
            set
            {
                Instances.Settings.SetValue(nameof(UseParamLineControl), value);
                RefreshLayout();
            }
        }

        public static bool UseParamPlaneControl
        {
            get => Instances.Settings.GetValue(nameof(UseParamPlaneControl), false);
            set
            {
                Instances.Settings.SetValue(nameof(UseParamPlaneControl), value);
                RefreshLayout();
            }
        }

        public static bool UseParamCircleControl
        {
            get => Instances.Settings.GetValue(nameof(UseParamCircleControl), false);
            set
            {
                Instances.Settings.SetValue(nameof(UseParamCircleControl), value);
                RefreshLayout();
            }
        }

        public static bool UseParamMaterialControl
        {
            get => Instances.Settings.GetValue(nameof(UseParamMaterialControl), true);
            set
            {
                Instances.Settings.SetValue(nameof(UseParamMaterialControl), value);
                RefreshLayout();
            }
        }

        public static bool UseParamGeneralControl
        {
            get => Instances.Settings.GetValue(nameof(UseParamGeneralControl), false);
            set
            {
                Instances.Settings.SetValue(nameof(UseParamGeneralControl), value);
                RefreshLayout();
            }
        }
        #endregion

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
            set
            {
                Instances.Settings.SetValue(nameof(ParamGumballWirePreviewThickness), value);
            }
        }

        public static readonly Color _paramGumballPreviewWireColorDefault = Color.DarkBlue;
        public static Color ParamGumballPreviewWireColor
        {
            get
            {
                return Instances.Settings.GetValue(nameof(ParamGumballPreviewWireColor), _paramGumballPreviewWireColorDefault);
            }
            set
            {
                Grasshopper.Instances.Settings.SetValue(nameof(ParamGumballPreviewWireColor), value);
            }
        }

        public static readonly Color _paramGumballPreviewMeshColorDefault = Color.DarkBlue;
        public static Color ParamGumballPreviewMeshColor
        {
            get
            {
                return Instances.Settings.GetValue(nameof(ParamGumballPreviewMeshColor), _paramGumballPreviewMeshColorDefault);
            }
            set
            {
                Grasshopper.Instances.Settings.SetValue(nameof(ParamGumballPreviewMeshColor), value);
            }
        }
    }
}
