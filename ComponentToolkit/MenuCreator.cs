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
            CreateNumberBox(major, "Component's Params to Icon", Datas.ComponentToCoreDistance, (v) => Datas.ComponentToCoreDistance = (int)v, Datas._componentToCoreDistanceDefault, 20, 0);
            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);
            CreateNumberBox(major, "Params' Icon to Edge", Datas.ParamsEdgeDistance, (v) => Datas.ParamsEdgeDistance = (int)v, Datas._paramsEdgeDistanceDefault, 20, 0);


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
            CreateNumberBox(major, "Components' Params to Control", Datas.ComponentControlNameDistance, (v) => Datas.ComponentControlNameDistance = (int)v, Datas._componentControlNameDistanceDefault, 20, 0);
            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);
            CreateNumberBox(major, "Params' Icon to Control", Datas.ParamsCoreDistance, (v) => Datas.ParamsCoreDistance = (int)v, Datas._paramsCoreDistanceDefault, 20, 0);
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

            major.DropDownItems.Add(CreateControlStateCheckBox<Boolean_Control, GH_Boolean>(new Param_Boolean().Icon_24x24));

            major.DropDownItems.Add(CreateControlStateCheckBox<String_Control, GH_String>(new Param_String().Icon_24x24));

            major.DropDownItems.Add(CreateControlStateCheckBox<Integer_Control, GH_Integer>(new Param_Integer().Icon_24x24));

            major.DropDownItems.Add(CreateControlStateCheckBox<Number_Control, GH_Number>(new Param_Number().Icon_24x24));

            major.DropDownItems.Add(CreateControlStateCheckBox<Colour_Control, GH_Colour>(new Param_Colour().Icon_24x24));

            major.DropDownItems.Add(CreateControlStateCheckBox<Material_Control, GH_Material>(new Param_OGLShader().Icon_24x24));

            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);

            CreateTextLabel(major, "One Line Control");

            major.DropDownItems.Add(CreateControlStateCheckBox<Domain_Control, GH_Interval>(new Param_Interval().Icon_24x24));

            major.DropDownItems.Add(CreateControlStateCheckBox<Point_Control, GH_Point>(new Param_Point().Icon_24x24));

            major.DropDownItems.Add(CreateControlStateCheckBox<Vector_Control, GH_Vector>(new Param_Vector().Icon_24x24));

            major.DropDownItems.Add(CreateControlStateCheckBox<Complex_Control, GH_ComplexNumber>(new Param_Complex().Icon_24x24));

            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);

            CreateTextLabel(major, "Multi-line Control");

            major.DropDownItems.Add(CreateControlStateCheckBox<Domain2D_Control, GH_Interval2D>(new Param_Interval2D().Icon_24x24));

            major.DropDownItems.Add(CreateControlStateCheckBox<Line_Control, GH_Line>(new Param_Line().Icon_24x24));

            major.DropDownItems.Add(CreateControlStateCheckBox<Plane_Control, GH_Plane>(new Param_Plane().Icon_24x24));

            major.DropDownItems.Add(CreateControlStateCheckBox<Circle_Control, GH_Circle>(new Param_Circle().Icon_24x24));

            GH_DocumentObject.Menu_AppendSeparator(major.DropDown);

            major.DropDownItems.Add(CreateControlStateCheckBox<General_Control,IGH_Goo>(new Param_GenericObject().Icon_24x24));

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

        private static ToolStripMenuItem CreateControlStateCheckBox<T, Tg>(Bitmap icon) where T : Enum where Tg : class, IGH_Goo
        {
            string saveBooleanKey = "UseParam" + typeof(Tg).Name;
            ToolStripMenuItem major = CreateCheckBox(typeof(T).Name.Replace('_', ' '), Instances.Settings.GetValue(saveBooleanKey, true)
                , icon, (boolean) =>
                {
                    Instances.Settings.SetValue(saveBooleanKey, boolean);
                    Datas.RefreshLayout();
                });
            major.DropDown.Closing += DropDown_Closing;




            string saveKey = typeof(T).FullName;
            int current = Instances.Settings.GetValue(saveKey, 0);

            var enums = Enum.GetNames(typeof(T)).GetEnumerator();
            foreach (int i in Enum.GetValues(typeof(T)))
            {
                enums.MoveNext();
                string name = enums.Current.ToString().Replace('_', ' ');
                ToolStripMenuItem item = new ToolStripMenuItem(name) { Tag = i, Checked = i == current };
                item.Click += Item_Click;
                major.DropDownItems.Add(item);
            }

            void Item_Click(object sender, EventArgs e)
            {
                foreach (ToolStripMenuItem dropIt in major.DropDownItems)
                {
                    dropIt.Checked = false;
                }

                ToolStripMenuItem it = (ToolStripMenuItem)sender;
                it.Checked = true;
                Instances.Settings.SetValue(saveKey, (int)it.Tag);

                //Refresh Control.
                foreach (GH_Document doc in Instances.DocumentServer)
                {
                    foreach (IGH_DocumentObject @object in doc.Objects)
                    {
                        if(@object is IGH_Param && @object.Attributes is GH_AdvancedFloatingParamAttr)
                        {
                            GH_AdvancedFloatingParamAttr att = (GH_AdvancedFloatingParamAttr)@object.Attributes;
                            att.Control?.ChangeControlItems();
                        }
                        if (@object is IGH_Component)
                        {
                            IGH_Component com = (IGH_Component)@object;
                            foreach (var param in com.Params)
                            {
                                if(param.Attributes is GH_AdvancedLinkParamAttr)
                                {
                                    GH_AdvancedLinkParamAttr att = (GH_AdvancedLinkParamAttr)param.Attributes;
                                    att.Control?.ChangeControlItems();
                                }
                            }
                        }
                        @object.Attributes.ExpireLayout();
                    }
                }

                Instances.RedrawCanvas();
            }

            return major;
        }
    }
}
