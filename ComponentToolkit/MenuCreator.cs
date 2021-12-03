using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Base;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grasshopper.Kernel.Parameters;

namespace ComponentToolkit
{
    public static  class MenuCreator
    {

        public static ToolStripMenuItem CreateMajorMenu()
        {
            ToolStripMenuItem major = new ToolStripMenuItem("Component Toolkit", Properties.Resources.ComponentToolkitIcon_24) { ToolTipText = "Two tools and some component's layout params." };


            major.DropDownItems.Add(CreateQuickWireItem());

            major.DropDownItems.Add(CreateControlItem());

            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);

            major.DropDownItems.Add(CreateCheckBox("Component Input Align Edge", Datas.ComponentInputEdgeLayout, (boolean) => Datas.ComponentInputEdgeLayout = boolean));
            major.DropDownItems.Add(CreateCheckBox("Component Output Align Edge", Datas.ComponentOutputEdgeLayout, (boolean) => Datas.ComponentOutputEdgeLayout = boolean));
            major.DropDownItems.Add(CreateShowParamIconItem());

            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);
            CreateNumberBox(major, "Component's Params to Edge", Datas.ComponentToEdgeDistance, (v) => Datas.ComponentToEdgeDistance = (int)v, Datas._componentToEdgeDistanceDefault, 20, 0);
            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);
            CreateNumberBox(major, "Component's Params to Core", Datas.ComponentToCoreDistance, (v) => Datas.ComponentToCoreDistance = (int)v, Datas._componentToCoreDistanceDefault, 20, 0);
            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);
            CreateNumberBox(major, "Params to Edge", Datas.ParamsEdgeDistance, (v) => Datas.ParamsEdgeDistance = (int)v, Datas._paramsEdgeDistanceDefault, 20, 0);
            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);
            CreateNumberBox(major, "Params to Core", Datas.ParamsCoreDistance, (v) => Datas.ParamsCoreDistance = (int)v, Datas._paramsCoreDistanceDefault, 20, 0);


            return major;
        }

        private static ToolStripMenuItem CreateShowParamIconItem()
        {
            ToolStripMenuItem major = CreateCheckBox("Show Component's Param Icon", Datas.ShowLinkParamIcon,new Param_GenericObject().Icon_24x24, (boolean) => Datas.ShowLinkParamIcon = boolean);

            major.DropDown.Closing += DropDown_Closing;

            CreateNumberBox(major, "Distance From Icon To String", Datas.ComponentIconDistance, (v) => Datas.ComponentIconDistance = (int)v, Datas._componentIconDistanceDefault, 20, 0);
            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);
            CreateNumberBox(major, "Icon's Opacity", Datas.ComponentIconOpacity, (v) => Datas.ComponentIconOpacity = v, Datas._componentIconOpacityDefault, 1, 0, 3);
            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);
            CreateNumberBox(major, "Icon Size", Datas.ComponentParamIconSize, (v) => Datas.ComponentParamIconSize = (int)v, Datas._componentParamIconSizeDefault, 20, 4);

            return major;
        }

        private static ToolStripMenuItem CreateQuickWireItem()
        {
            ToolStripMenuItem major = CreateCheckBox("Use Quick Wire", Datas.UseQuickWire, Properties.Resources.QuickwireIcon_24, (boolean) => Datas.UseQuickWire = boolean);
            major.ToolTipText = "You can left click the component's param or double click floating param to choose which activeobjec you want to add.";

            ToolStripMenuItem click = new ToolStripMenuItem("Clear all quickwire settings");
            click.Click += (sender, e) =>
            {
               if( MessageBox.Show("Are you sure to remove all quickwire settings, and delete the quickwires.json file?", "Remove All?", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    GH_ComponentAttributesReplacer.RemoveAllQuickwireSettings();
                    MessageBox.Show("Succeed!");
                }
            };

            major.DropDownItems.Add(click);

            return major;
        }

        private static ToolStripMenuItem CreateControlItem()
        {
            ToolStripMenuItem major = CreateCheckBox("Param Control", Datas.UseParamControl, Properties.Resources.ParamControlIcon_24, 
                (boolean) => Datas.UseParamControl = boolean);
            major.ToolTipText = "It will show you the persistent param's value and you can change the value easily.";

            major.DropDownItems.Add(CreateUseControlItem());

            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);
            major.DropDownItems.Add(CreateCheckBox("Use on Components", Datas.ComponentUseControl, Properties.Resources.ComponentIcon_24, (boolean) => Datas.ComponentUseControl = boolean));
            major.DropDownItems.Add(CreateCheckBox("Use on Parameters", Datas.ParamUseControl, Properties.Resources.ParametersIcon_24, (boolean) => Datas.ParamUseControl = boolean));


            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);

            major.DropDownItems.Add(CreateCheckBox("Independent Width", Datas.SeperateCalculateWidthControl, (boolean) => Datas.SeperateCalculateWidthControl = boolean));
            major.DropDownItems.Add(CreateCheckBox("Control Align Right", Datas.ControlAlignRightLayout, (boolean) => Datas.ControlAlignRightLayout = boolean));
            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);
            CreateNumberBox(major, "Params to Control", Datas.ComponentControlNameDistance, (v) => Datas.ComponentControlNameDistance = (int)v, Datas._componentControlNameDistanceDefault, 20, 0);
            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);
            CreateNumberBox(major, "Max InputBox Width", Datas.InputBoxControlMaxWidth, (v) => Datas.InputBoxControlMaxWidth = (int)v, Datas._inputBoxControlMaxWidthDefault, 500, 20);

            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);

            major.DropDownItems.Add(CreateForeGroundColor());
            major.DropDownItems.Add(CreateBackGroundColor());
            return major;
        }

        private static ToolStripMenuItem CreateUseControlItem()
        {
            ToolStripMenuItem major = new ToolStripMenuItem("Choose Controls") { ToolTipText = "Choose which control should be used." };
            major.Font = new Font(major.Font, FontStyle.Bold);

            major.DropDown.Closing += DropDown_Closing;

            CreateTextLabel(major, "Single Control");

            major.DropDownItems.Add(CreateCheckBox("Boolean Control", Datas.UseParamBooleanControl, new Param_Boolean().Icon_24x24,
                (boolean) => Datas.UseParamBooleanControl = boolean));

            major.DropDownItems.Add(CreateCheckBox("String Control", Datas.UseParamStringControl, new Param_String().Icon_24x24,
                (boolean) => Datas.UseParamStringControl = boolean));

            major.DropDownItems.Add(CreateCheckBox("Integer Control", Datas.UseParamIntegerControl, new Param_Integer().Icon_24x24,
                (boolean) => Datas.UseParamIntegerControl = boolean));

            major.DropDownItems.Add(CreateCheckBox("Number Control", Datas.UseParamNumberControl, new Param_Number().Icon_24x24,
                (boolean) => Datas.UseParamNumberControl = boolean));

            major.DropDownItems.Add(CreateCheckBox("Colour Control", Datas.UseParamColourControl, new Param_Colour().Icon_24x24,
                (boolean) => Datas.UseParamColourControl = boolean));

            major.DropDownItems.Add(CreateCheckBox("Material Control", Datas.UseParamMaterialControl, new Param_OGLShader().Icon_24x24,
                (boolean) => Datas.UseParamMaterialControl = boolean));

            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);

            CreateTextLabel(major, "One Line Control");

            major.DropDownItems.Add(CreateCheckBox("Interval Control", Datas.UseParamIntervalControl, new Param_Interval().Icon_24x24,
                (boolean) => Datas.UseParamIntervalControl = boolean));

            major.DropDownItems.Add(CreateCheckBox("Point Control", Datas.UseParamPointControl, new Param_Point().Icon_24x24,
                (boolean) => Datas.UseParamPointControl = boolean));

            major.DropDownItems.Add(CreateCheckBox("Vector Control", Datas.UseParamVectorControl, new Param_Vector().Icon_24x24,
                (boolean) => Datas.UseParamVectorControl = boolean));

            major.DropDownItems.Add(CreateCheckBox("Complex Control", Datas.UseParamComplexControl, new Param_Complex().Icon_24x24,
                (boolean) => Datas.UseParamComplexControl = boolean));

            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);

            CreateTextLabel(major, "Multi-line Control");

            major.DropDownItems.Add(CreateCheckBox("Interval2d Control", Datas.UseParamInterval2DControl, new Param_Interval2D().Icon_24x24,
                (boolean) => Datas.UseParamInterval2DControl = boolean));

            major.DropDownItems.Add(CreateCheckBox("Line Control", Datas.UseParamLineControl, new Param_Line().Icon_24x24,
                (boolean) => Datas.UseParamLineControl = boolean));

            major.DropDownItems.Add(CreateCheckBox("Plane Control", Datas.UseParamPlaneControl, new Param_Plane().Icon_24x24,
                (boolean) => Datas.UseParamPlaneControl = boolean));

            major.DropDownItems.Add(CreateCheckBox("Circle Control", Datas.UseParamCircleControl, new Param_Circle().Icon_24x24,
                (boolean) => Datas.UseParamCircleControl = boolean));

            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);

            major.DropDownItems.Add(CreateCheckBox("General Control", Datas.UseParamGeneralControl, new Param_GenericObject().Icon_24x24,
                (boolean) => Datas.UseParamGeneralControl = boolean));

            return major;
        }

        private static ToolStripMenuItem CreateForeGroundColor()
        {
            ToolStripMenuItem major = new ToolStripMenuItem("Foreground Color") { ToolTipText = "Change controls' foreground color." };

            CreateColor(major, "Text Color", Datas.ControlTextgroundColor, Datas._controlTextgroundColorDefault, (color) => Datas.ControlTextgroundColor = color);
            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);
            CreateColor(major, "Foreground Color", Datas.ControlForegroundColor, Datas._controlForegroundColorDefault, (color) => Datas.ControlForegroundColor = color);


            return major;
        }

        private static ToolStripMenuItem CreateBackGroundColor()
        {
            ToolStripMenuItem major = new ToolStripMenuItem("Background Color") { ToolTipText = "Change controls' background color." };

            CreateColor(major, "Border Color", Datas.ControlBorderColor, Datas._controlBorderColorDefault, (color) => Datas.ControlBorderColor = color);
            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);
            CreateColor(major, "Background Color", Datas.ControlBackgroundColor, Datas._controlBackgroundColorDefault, (color) => Datas.ControlBackgroundColor = color);


            return major;
        }
        private static ToolStripMenuItem CreateCheckBox(string itemName, bool valueDefault, Bitmap icon, Action<bool> valueChange)
        {
            ToolStripMenuItem click = new ToolStripMenuItem(itemName, icon) { Checked = valueDefault };
            click.Font = new Font(click.Font, FontStyle.Regular);
            CreateCheckBox(ref click, valueChange);
            return click;
        }
        private static ToolStripMenuItem CreateCheckBox(string itemName, bool valueDefault, Action<bool> valueChange)
        {

            ToolStripMenuItem click = new ToolStripMenuItem(itemName) { Checked = valueDefault };
            CreateCheckBox(ref click, valueChange);
            return click;
        }

        private static void CreateCheckBox(ref ToolStripMenuItem click, Action<bool> valueChange)
        {
            click.Click += (sender, e) =>
            {
                ToolStripMenuItem item = (ToolStripMenuItem)sender;
                item.Checked = !item.Checked;
                valueChange.Invoke(item.Checked);
            };

        }

        private static void CreateTextLabel(ToolStripMenuItem item, string name, string tooltips = null)
        {
            ToolStripLabel textBox = new ToolStripLabel(name);
            textBox.Font = new Font(textBox.Font, FontStyle.Bold);
            if (!string.IsNullOrEmpty(tooltips))
                textBox.ToolTipText = tooltips;
            item.DropDownItems.Add(textBox);
        }

        private static void CreateNumberBox(ToolStripMenuItem item, string itemName, double originValue, Action<double> valueChange, double valueDefault, double Max, double Min, int decimalPlace = 0)
        {
            item.DropDown.Closing -= DropDown_Closing;
            item.DropDown.Closing += DropDown_Closing;

            CreateTextLabel(item, itemName, $"Value from {Min} to {Max}");

            GH_DigitScroller slider = new GH_DigitScroller
            {
                MinimumValue = (decimal)Min,
                MaximumValue = (decimal)Max,
                DecimalPlaces = decimalPlace,
                Value = (decimal)originValue,
                Size = new Size(150, 24),
            };
            slider.ValueChanged += Slider_ValueChanged;

            void Slider_ValueChanged(object sender, GH_DigitScrollerEventArgs e)
            {
                double result = (double)e.Value;
                result = result >= Min ? result : Min;
                result = result <= Max ? result : Max;
                slider.Value = (decimal)result;

                valueChange.Invoke(result);

            }

            GH_DocumentObject.Menu_AppendCustomItem(item.DropDown, slider);

            //Add a Reset Item.
            ToolStripMenuItem resetItem = new ToolStripMenuItem("Reset Value", Properties.Resources.ResetIcons_24);
            resetItem.Click += (sender, e) =>
            {
                slider.Value = (decimal)valueDefault;
                valueChange.Invoke(valueDefault);
            };
            item.DropDownItems.Add(resetItem);
        }

        private static void DropDown_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            e.Cancel = e.CloseReason == ToolStripDropDownCloseReason.ItemClicked;
        }

        private static GH_ColourPicker CreateColor(ToolStripMenuItem item, string itemName, Color rightColor, Color defaultColor, Action<Color> changeColor)
        {
            item.DropDown.Closing -= DropDown_Closing;
            item.DropDown.Closing += DropDown_Closing;

            ToolStripLabel textBox = new ToolStripLabel(itemName);
            textBox.Font = new Font(textBox.Font.FontFamily, textBox.Font.Size, FontStyle.Bold);
            item.DropDownItems.Add(textBox);

            GH_ColourPicker picker = GH_DocumentObject.Menu_AppendColourPicker(item.DropDown, rightColor, (sender, e) =>
            {
                changeColor.Invoke(e.Colour);
                Grasshopper.Instances.ActiveCanvas.Refresh();
            });

            //Add a Reset Item.
            ToolStripMenuItem resetItem = new ToolStripMenuItem("Reset Value", Properties.Resources.ResetIcons_24);
            resetItem.Click += (sender, e) =>
            {
                picker.Colour = defaultColor;
                changeColor.Invoke(defaultColor);
                Grasshopper.Instances.ActiveCanvas.Refresh();
            };
            item.DropDownItems.Add(resetItem);

            return picker;
        }
    }
}
