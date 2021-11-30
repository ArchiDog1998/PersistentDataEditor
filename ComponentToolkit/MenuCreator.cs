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

namespace ComponentToolkit
{
    public static  class MenuCreator
    {
        public static ToolStripMenuItem CreateMajorMenu()
        {
            ToolStripMenuItem major = new ToolStripMenuItem("Component Layout") { ToolTipText = "Set some component's layout params." };
            major.DropDownItems.Add(CreateControlItem());

            ToolStripMenuItem inputClick = new ToolStripMenuItem("Component Input Align Edge") { Checked = GH_ComponentAttributesReplacer.ComponentInputEdgeLayout };
            inputClick.Click += (sender, e) =>
            {
                inputClick.Checked = !inputClick.Checked;
                GH_ComponentAttributesReplacer.ComponentInputEdgeLayout = inputClick.Checked;
            };
            major.DropDownItems.Add(inputClick);

            ToolStripMenuItem outputClick = new ToolStripMenuItem("Component Output Align Edge") { Checked = GH_ComponentAttributesReplacer.ComponentOutputEdgeLayout };
            outputClick.Click += (sender, e) =>
            {
                outputClick.Checked = !outputClick.Checked;
                GH_ComponentAttributesReplacer.ComponentOutputEdgeLayout = outputClick.Checked;
            };
            major.DropDownItems.Add(outputClick);

            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);
            CreateNumberBox(major, "Params to Edge", GH_ComponentAttributesReplacer.ComponentToEdgeDistance, (v) => GH_ComponentAttributesReplacer.ComponentToEdgeDistance = (int)v, GH_ComponentAttributesReplacer._componentToEdgeDistanceDefault, 20, 0);
            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);
            CreateNumberBox(major, "Params to Core", GH_ComponentAttributesReplacer.ComponentToCoreDistance, (v) => GH_ComponentAttributesReplacer.ComponentToCoreDistance = (int)v, GH_ComponentAttributesReplacer._componentToCoreDistanceDefault, 20, 0);
            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);
            CreateNumberBox(major, "Params to Control", GH_ComponentAttributesReplacer.ComponentControlNameDistance, (v) => GH_ComponentAttributesReplacer.ComponentControlNameDistance = (int)v, GH_ComponentAttributesReplacer._componentControlNameDistanceDefault, 20, 0);



            return major;
        }

        private static ToolStripMenuItem CreateControlItem()
        {
            ToolStripMenuItem major = CreateCheckBox("Use Control", GH_ComponentAttributesReplacer.ComponentUseControl, (boolean) => GH_ComponentAttributesReplacer.ComponentUseControl = boolean);
            major.DropDownItems.Add(CreateForeGroundColor());
            major.DropDownItems.Add(CreateBackGroundColor());
            return major;
        }
        private static ToolStripMenuItem CreateForeGroundColor()
        {
            ToolStripMenuItem major = new ToolStripMenuItem("Foreground Color") { ToolTipText = "Change controls' foreground color." };

            CreateColor(major, "Text Color", BaseRenderItem.ControlTextgroundColor, BaseRenderItem._controlTextgroundColorDefault, (color) => BaseRenderItem.ControlTextgroundColor = color);
            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);
            CreateColor(major, "Foreground Color", BaseRenderItem.ControlForegroundColor, BaseRenderItem._controlForegroundColorDefault, (color) => BaseRenderItem.ControlForegroundColor = color);


            return major;
        }

        private static ToolStripMenuItem CreateBackGroundColor()
        {
            ToolStripMenuItem major = new ToolStripMenuItem("Background Color") { ToolTipText = "Change controls' background color." };

            CreateColor(major, "Border Color", BaseRenderItem.ControlBorderColor, BaseRenderItem._controlBorderColorDefault, (color) => BaseRenderItem.ControlBorderColor = color);
            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);
            CreateColor(major, "Background Color", BaseRenderItem.ControlBackgroundColor, BaseRenderItem._controlBackgroundColorDefault, (color) => BaseRenderItem.ControlBackgroundColor = color);


            return major;
        }

        private static ToolStripMenuItem CreateCheckBox(string itemName, bool valueDefault, Action<bool> valueChange)
        {

            ToolStripMenuItem click = new ToolStripMenuItem(itemName) { Checked = valueDefault };
            click.Click += (sender, e) =>
            {
                click.Checked = !click.Checked;
                valueChange.Invoke(click.Checked);
            };

            return click;
        }

            private static void CreateNumberBox(ToolStripMenuItem item, string itemName, double originValue, Action<double> valueChange, double valueDefault, double Max, double Min)
        {
            item.DropDown.Closing -= DropDown_Closing;
            item.DropDown.Closing += DropDown_Closing;

            ToolStripLabel textBox = new ToolStripLabel(itemName);
            textBox.Font = new Font(textBox.Font.FontFamily, textBox.Font.Size, FontStyle.Bold);
            textBox.ToolTipText = $"Value from {Min} to {Max}";
            item.DropDownItems.Add(textBox);

            int decimalPlace = 3;

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
