using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.GUI.Canvas;

namespace ComponentToolkit
{
    internal static class Datas
    {
        #region Colors
        public static readonly Color _controlForegroundColorDefault = Color.FromArgb(40, 40, 40);
        public static Color ControlForegroundColor
        {
            get
            {
                Color color = Grasshopper.Instances.Settings.GetValue(nameof(ControlForegroundColor), _controlForegroundColorDefault);
                return Color.FromArgb((int)(GH_Canvas.ZoomFadeLow / 255f * color.A), color);
            }
            set
            {
                Grasshopper.Instances.Settings.SetValue(nameof(ControlForegroundColor), value);
                Grasshopper.Instances.RedrawCanvas();
            }
        }

        public static readonly Color _controlTextgroundColorDefault = Color.FromArgb(40, 40, 40);
        public static Color ControlTextgroundColor
        {
            get
            {
                Color color = Grasshopper.Instances.Settings.GetValue(nameof(ControlTextgroundColor), _controlTextgroundColorDefault);
                return Color.FromArgb((int)(GH_Canvas.ZoomFadeLow / 255f * color.A), color);
            }
            set
            {
                Grasshopper.Instances.Settings.SetValue(nameof(ControlTextgroundColor), value);
                Grasshopper.Instances.RedrawCanvas();
            }
        }

        public static readonly Color _controlBackgroundColorDefault = Color.FromArgb(150, Color.WhiteSmoke);
        public static Color ControlBackgroundColor
        {
            get
            {
                Color color = Grasshopper.Instances.Settings.GetValue(nameof(ControlBackgroundColor), _controlBackgroundColorDefault);
                return Color.FromArgb((int)(GH_Canvas.ZoomFadeLow / 255f * color.A), color);
            }
            set
            {
                Grasshopper.Instances.Settings.SetValue(nameof(ControlBackgroundColor), value);
                Grasshopper.Instances.RedrawCanvas();
            }
        }

        public static readonly Color _controlBorderColorDefault = Color.FromArgb(40, 40, 40);
        public static Color ControlBorderColor
        {
            get
            {
                Color color = Grasshopper.Instances.Settings.GetValue(nameof(ControlBorderColor), _controlBorderColorDefault);
                return Color.FromArgb((int)(GH_Canvas.ZoomFadeLow / 255f * color.A), color);
            }
            set
            {
                Grasshopper.Instances.Settings.SetValue(nameof(ControlBorderColor), value);
                Grasshopper.Instances.RedrawCanvas();
            }
        }
        #endregion

        #region Settings For Layout
        public static readonly int MiniWidth = 6;

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

        public static bool ComponentUseControl
        {
            get => Grasshopper.Instances.Settings.GetValue(nameof(ComponentUseControl), true);
            set
            {
                Instances.Settings.SetValue(nameof(ComponentUseControl), value);
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
            get => Instances.Settings.GetValue(nameof(ControlAlignRightLayout), false);
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

        private static void RefreshLayout()
        {
            foreach (IGH_DocumentObject @object in Instances.ActiveCanvas.Document.Objects)
            {
                @object.Attributes.ExpireLayout();
            }
            Instances.RedrawCanvas();
        }
        #endregion

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
            get => Instances.Settings.GetValue(nameof(UseParamInterval2DControl), true);
            set
            {
                Instances.Settings.SetValue(nameof(UseParamInterval2DControl), value);
                RefreshLayout();
            }
        }

        public static bool UseParamLineControl
        {
            get => Instances.Settings.GetValue(nameof(UseParamLineControl), true);
            set
            {
                Instances.Settings.SetValue(nameof(UseParamLineControl), value);
                RefreshLayout();
            }
        }

        public static bool UseParamPlaneControl
        {
            get => Instances.Settings.GetValue(nameof(UseParamPlaneControl), true);
            set
            {
                Instances.Settings.SetValue(nameof(UseParamPlaneControl), value);
                RefreshLayout();
            }
        }

        public static bool UseParamCircleControl
        {
            get => Instances.Settings.GetValue(nameof(UseParamCircleControl), true);
            set
            {
                Instances.Settings.SetValue(nameof(UseParamCircleControl), value);
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
    }
}
