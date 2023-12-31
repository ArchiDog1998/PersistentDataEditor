using Grasshopper;
using Grasshopper.Kernel;
using SimpleGrasshopper.Attributes;
using SimpleGrasshopper.Data;
using System.Drawing;
using System.Linq;

namespace PersistentDataEditor;

internal static partial class NewData
{
    public static readonly int MiniWidth = 6;
    public static bool IsCurrectObjectLock { private get; set; } = false;

    public static int AdditionWidth => ComponentToEdgeDistance + ComponentToCoreDistance;

    [ToolButton("ParamControlIcon_24.png")]
    [Setting, Config("Param's Control", icon: "ParamControlIcon_24.png")]
    private static readonly bool _useParamControl = true;

    [Setting, Config("Auto Apply InputBox Value", parent: "Param's Control")]
    private static readonly bool _textboxInputAutoApply = true;

    [Setting, Config("Auto Set Default Value", parent: "Param's Control")]
    private static readonly bool _useDefaultValueToControl = true;

    [Setting, Config("Only Item Access", parent: "Param's Control", section:1)]
    private static readonly bool _onlyItemAccessControl = true;

    [Setting, Config("Only Selected Object", parent: "Param's Control", section: 1)]
    private static readonly bool _onlyShowSelectedObjectControl = false;

    [Setting, Config("Use on components", icon: "ComponentIcon_24.png", parent: "Param's Control", section: 1)]
    private static readonly bool _componentUseControl = true;

    [Setting, Config("Use on parameters", icon: "ParametersIcon_24.png", parent: "Param's Control", section: 1)]
    private static readonly bool _paramUseControl = true;

    [Setting, Config("Independent Width", parent: "Param's Control", section: 2)]
    private static readonly bool _seperateCalculateWidthControl = true;

    [Setting, Config("Control Align Right", parent: "Param's Control", section: 2)]
    private static readonly bool _controlAlignRightLayout = true;

    [Setting, Config("Components' Params to Control", parent: "Param's Control", section: 3)]
    private static readonly int _componentControlNameDistance = 2;

    [Setting, Config("Params' Icon to Control", parent: "Param's Control", section: 3)]
    private static readonly int _paramsCoreDistance = 3;

    [Setting, Config("Max Text Box Width", parent: "Param's Control", section: 3)]
    private static readonly int _inputBoxControlMaxWidth = 100;


    [Setting, Config("Foreground Color", parent: "Param's Control", section: 4)]
    private static readonly Color _controlForegroundColor = Color.FromArgb(40, 40, 40);

    [Setting, Config("Textground Color", parent: "Param's Control", section: 4)]
    private static readonly Color _controlTextgroundColor = Color.FromArgb(40, 40, 40);

    [Setting, Config("Background Color", parent: "Param's Control", section: 4)]
    private static readonly Color _controlBackgroundColor = Color.FromArgb(150, Color.WhiteSmoke);

    [Setting, Config("Border Color", parent: "Param's Control", section: 4)]
    private static readonly Color _controlBorderColor = Color.FromArgb(40, 40, 40);

    [Config("Choose Controls", "Choose which control should be used.", parent: "Param's Control")]
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

    [Setting, Config("Material", icon: ParamGuids.OGLShader, parent: "Choose Controls")]
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

    [Setting, Config("Domain 2 Type", parent:"Domain 2")]
    private static readonly Domain2D_Control _domain2Type = Domain2D_Control.U_V;

    [Setting, Config("Line", icon: ParamGuids.Line, parent: "Choose Controls", section: 2)]
    private static readonly bool _useParamLineControl = true;

    [Setting, Config("Line Type", parent:"Line")]
    private static readonly Line_Control _lineType = Line_Control.From_To;

    [Setting, Config("Plane", icon: ParamGuids.Plane, parent: "Choose Controls", section: 2)]
    private static readonly bool _useParamPlaneControl = true;

    [Setting, Config("Plane Type", parent: "Plane")]
    private static readonly Plane_Control _planeType = Plane_Control.OZ;

    [Setting, Config("Circle", icon: ParamGuids.Circle, parent: "Choose Controls", section: 2)]
    private static readonly bool _useParamCircleControl = true;

    [Setting, Config("Circle Type", parent:"Circle")]
    private static readonly Circle_Control _circleType = Circle_Control.Plane_Radius;

    [Setting, Config("Rectangle", icon: ParamGuids.Rectangle, parent: "Choose Controls", section: 2)]
    private static readonly bool _useParamRectangleControl = true;

    [Setting, Config("Rectangle Type", parent:"Rectangle")]
    private static readonly Rectangle_Control _rectangleType = Rectangle_Control.Domain_Rectangle;

    [Setting, Config("Box", icon: ParamGuids.Box, parent: "Choose Controls", section: 2)]
    private static readonly bool _useParamBoxControl = true;

    [Setting, Config("Box Type", parent:"Box")]
    private static readonly Box_Control _boxType = Box_Control.Domain_Box;

    [Setting, Config("Arc", icon: ParamGuids.Arc, parent: "Choose Controls", section: 2)]
    private static readonly bool _useParamArcControl = true;

    [Setting, Config("Arc Type", parent: "Arc")]
    private static readonly Arc_Control _arcType = Arc_Control.Plane_Radius_Angle;

    [Setting, Config("General", icon: ParamGuids.GenericObject, parent: "Choose Controls", section: 3)]
    private static readonly bool _useParamGeneralControl = true;

    [Setting, Config("General Type", parent:"General")]
    private static readonly General_Control _generalType = General_Control.ReadOnly;

    [Setting, Config("Script", icon: ParamGuids.ScriptVariable, parent: "Choose Controls", section: 3)]
    private static readonly bool _useParamScriptControl = true;

    [Setting, Config("Number Slider", icon: "{57DA07BD-ECAB-415d-9D86-AF36D7073ABC}", parent: "Choose Controls", section: 3)]
    private static readonly bool _useParamNumberSliderControl = true;

    [Setting, Config("Component Input Align Edge", section: 1)]
    private static readonly bool _componentInputEdgeLayout = false;

    [Setting, Config("Component Output Align Edge", section: 1)]
    private static readonly bool _componentOutputEdgeLayout = false;

    [Setting, Config("Show Component's Param Icon", section: 1, icon: ParamGuids.GenericObject)]
    private static readonly bool _showLinkParamIcon = false;

    [Range(0, 20, 0)]
    [Setting, Config("Distance From Icon To String", parent: "Show Component's Param Icon")]
    private static readonly int _componentIconDistance = 2;

    [Range(0, 3, 3)]
    [Setting, Config("Icon's Opacity", parent: "Show Component's Param Icon")]
    private static readonly double _componentIconOpacity = 0.6;

    [Range(4, 20, 0)]
    [Setting, Config("Icon Size", parent: "Show Component's Param Icon")]
    private static readonly int _componentParamIconSize = 16;

    [Range(0, 20, 0)]
    [Setting, Config("Component's Params to Edge", section:2)]
    private static readonly int _componentToEdgeDistance = 3;

    [Range(0, 20, 0)]
    [Setting, Config("Component's Params to Icon", section: 2)]
    private static readonly int _componentToCoreDistance = 3;

    [Range(0, 20, 0)]
    [Setting, Config("Params' Icon to Edge", section: 2)]
    private static readonly int _paramsEdgeDistance = 5;

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

internal static partial class GumballData
{
    [Setting, Config("Geometry Gumball"), ToolButton("")]
    private static readonly bool _useGeoParamGumball = true;

    [Setting, Config("Use Rotate", icon: "b7798b74-037e-4f0c-8ac7-dc1043d093e0", parent: "Geometry Gumball")]
    private static readonly bool _geoParamGumballRotate = true;

    [Setting, Config("Use Scale", icon: "4d2a06bd-4b0f-4c65-9ee0-4220e4c01703", parent: "Geometry Gumball")]
    private static readonly bool _geoParamGumballScale = true;

    [Setting, Range(1, 100, 0)]
    [Config("Max Gumball Count", parent: "Geometry Gumball", section:1)]
    private static readonly int _gumballMaxShowCount = 10;

    [Setting, Range(1, 200, 0)]
    [Config("Gumball Size", parent: "Geometry Gumball", section: 1)]
    private static readonly int _paramGumballRadius = 50;

    [Setting, Range(1, 50, 0)]
    [Config("Gumball Preview Thickness", parent: "Geometry Gumball", section: 1)]
    private static readonly int _paramGumballWirePreviewThickness = 5;

    [Setting, Config("Wire Preview Color", parent: "Geometry Gumball", section: 2)]
    private static readonly Color _paramGumballPreviewWire = Color.DarkBlue;

    [Setting, Config("Mesh Preview Color", parent: "Geometry Gumball", section: 2)]
    private static readonly Color _paramGumballPreviewMesh = Color.DarkBlue;

    static GumballData()
    {
        OnPropertyChanged += (s, o) =>
        {
            ChangeGumball();
        };
    }

    internal static void ChangeGumball()
    {
        foreach (var doc in Instances.DocumentServer.Cast<GH_Document>())
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
}

