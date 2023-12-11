using Grasshopper;
using SimpleGrasshopper.Attributes;
using SimpleGrasshopper.Data;
using System.Drawing;
using System.Linq;

namespace PersistentDataEditor;

internal static partial class NewData
{
    [Setting]
    private static readonly Color _controlForegroundColor = Color.FromArgb(40, 40, 40);

    [Setting]
    private static readonly Color _controlTextgroundColor = Color.FromArgb(40, 40, 40);

    [Setting]
    private static readonly Color _controlBackgroundColor = Color.FromArgb(150, Color.WhiteSmoke);

    [Setting]
    private static readonly Color _controlBorderColor = Color.FromArgb(40, 40, 40);

    [Setting]
    private static readonly int _componentParamIconSize = 16;

    [Setting]
    private static readonly double _componentIconOpacity = 0.6;

    [Setting]
    private static readonly int _componentIconDistance = 2;

    [Setting]
    private static readonly int _componentToEdgeDistance = 3;

    [Setting]
    private static readonly int _componentToCoreDistance = 3;

    [Setting]
    private static readonly int _componentControlNameDistance = 2;

    [Setting]
    private static readonly int _paramsEdgeDistance = 5;

    [Setting]
    private static readonly int _paramsCoreDistance = 3;

    [Setting]
    private static readonly int _inputBoxControlMaxWidtht = 100;

    public static int AdditionWidth => ComponentToEdgeDistance + ComponentToCoreDistance;

    [Setting]
    private static readonly bool _useParamControl = true;

    [Setting]
    private static readonly bool _textboxInputAutoApply = true;

    [Setting]
    private static readonly bool _useDefaultValueToControl = true;

    [Setting]
    private static readonly bool _onlyItemAccessControl = true;

    [Setting]
    private static readonly bool _componentUseControl = true;

    [Setting]
    private static readonly bool _paramUseControl = true;

    [Setting]
    private static readonly bool _onlyShowSelectedObjectControl = false;

    [Setting]
    private static readonly bool _componentInputEdgeLayout = false;

    [Setting]
    private static readonly bool _componentOutputEdgeLayout = false;

    [Setting]
    private static readonly bool _controlAlignRightLayout = true;

    [Setting]
    private static readonly bool _seperateCalculateWidthControl = true;

    [Config("Choose Controls", "Choose which control should be used.")]
    public static object ChooseControls { get; set; }

    [Setting, Config("Boolean", icon: ParamGuids.Boolean, parent: "Choose Controls")]
    private static readonly bool _useParamBooleanControl = true;

    [Setting, Config("String", icon: ParamGuids.String, parent: "Choose Controls")]
    private static readonly bool _useParamStringControl = true;

    [Setting, Config("Integer", icon: ParamGuids.Integer, parent: "Choose Controls")]
    private static readonly bool _useParamIntegerControl = true;

    [Setting, Config("Number", icon: ParamGuids.Number, parent: "Choose Controls")]
    private static readonly bool _useParamNumberControl = true;

    [Setting, Config("Colour", icon: ParamGuids.Colour, parent: "Choose Controls")]
    private static readonly bool _useParamColourControl = true;

    [Setting, Config("Material", icon: ParamGuids.ModelRenderMaterial, parent: "Choose Controls")]
    private static readonly bool _useParamMaterialControl = true;

    [Setting, Config("Domain", icon: ParamGuids.Interval, parent: "Choose Controls", section: 1)]
    private static readonly bool _useParamDomainControl = true;

    [Setting, Config("Point", icon: ParamGuids.Point, parent: "Choose Controls", section: 1)]
    private static readonly bool _useParamPointControl = true;

    [Setting, Config("Vector", icon: ParamGuids.Vector, parent: "Choose Controls", section: 1)]
    private static readonly bool _useParamVectorControl = true;

    [Setting, Config("Complex", icon: ParamGuids.Complex, parent: "Choose Controls", section: 1)]
    private static readonly bool _useParamComplexControl = true;

    [Setting, Config("Domain 2", icon: ParamGuids.Interval2D, parent: "Choose Controls", section: 2)]
    private static readonly bool _useParamDomain2Control = true;

    [Setting, Config("Line", icon: ParamGuids.Line, parent: "Choose Controls", section: 2)]
    private static readonly bool _useParamLineControl = true;

    [Setting, Config("Plane", icon: ParamGuids.Plane, parent: "Choose Controls", section: 2)]
    private static readonly bool _useParamPlaneControl = true;

    [Setting, Config("Circle", icon: ParamGuids.Circle, parent: "Choose Controls", section: 2)]
    private static readonly bool _useParamCircleControl = true;

    [Setting, Config("Rectangle", icon: ParamGuids.Rectangle, parent: "Choose Controls", section: 2)]
    private static readonly bool _useParamRectangleControl = true;

    [Setting, Config("Box", icon: ParamGuids.Box, parent: "Choose Controls", section: 2)]
    private static readonly bool _useParamBoxControl = true;

    [Setting, Config("Arc", icon: ParamGuids.Arc, parent: "Choose Controls", section: 2)]
    private static readonly bool _useParamArcControl = true;

    [Setting, Config("General", icon: ParamGuids.GenericObject, parent: "Choose Controls", section: 3)]
    private static readonly bool _useParamGeneralControl = true;

    [Setting, Config("Script", icon: ParamGuids.ScriptVariable, parent: "Choose Controls", section: 3)]
    private static readonly bool _useParamScriptControl = true;

    [Setting, Config("Number Slider", icon: "{57DA07BD-ECAB-415d-9D86-AF36D7073ABC}", parent: "Choose Controls", section: 3)]
    private static readonly bool _useParamNumberSliderControl = true;
    static NewData()
    {
        OnPropertyChanged += (name, value) =>
        {
            foreach (var attr in Instances.DocumentServer.SelectMany(doc => doc.Attributes))
            {
                if (attr is GH_AdvancedLinkParamAttr att)
                {
                    if (att.Control is ParamVariableControl)
                        att.Control?.ChangeControlItems();
                }
                if (attr is IControlAttr controlAttr)
                {
                    controlAttr.SetControl();
                }
                attr.ExpireLayout();
            }

            Instances.RedrawCanvas();
        };

        OnComponentParamIconSizeChanged += value =>
        {
            GH_AdvancedLinkParamAttr.UpdataIcons();
        };

        OnComponentIconOpacityChanged += value =>
        {
            GH_AdvancedLinkParamAttr.UpdataIcons();
        };
    }
}

