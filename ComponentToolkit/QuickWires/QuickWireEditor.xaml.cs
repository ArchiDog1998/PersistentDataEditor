using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ComponentToolkit
{
    /// <summary>
    /// Interaction logic for QuickWireEditor.xaml
    /// </summary>
    public partial class QuickWireEditor : Window
    {
        private GH_Canvas _canvas = Grasshopper.Instances.ActiveCanvas;
        private bool _isTheSame = false;
        private IGH_Param _targetParam;
        private IGH_Param TargetParam 
        {
            get => _targetParam;
            set
            {
                _targetParam = value;
                if (_targetParam == null) return;
                _isTheSame = _targetParam.ComponentGuid == _componentGuid;
            }
        }

        private bool _isInput;
        private Guid _componentGuid;
        private CreateObjectItem[] _preList;
        private bool _cancel = false;
        public QuickWireEditor(Guid componenguid, bool isInput, Bitmap icon, string name, ObservableCollection<CreateObjectItem> structureLists)
        {
            this._isInput = isInput;
            this.DataContext = structureLists;
            this._preList = structureLists.ToArray();

            MemoryStream ms = new MemoryStream();
            icon.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage ImageIcon = new BitmapImage();

            ImageIcon.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            ImageIcon.StreamSource = ms;
            ImageIcon.EndInit();
            Icon = ImageIcon;

            _componentGuid = componenguid;
            InitializeComponent();

            this.Title += "-" + name + (isInput ? "[In]" : "[Out]");
        }
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            int index = dataGrid.SelectedIndex;
            if (index == -1) return;
            ((ObservableCollection<CreateObjectItem>)DataContext).RemoveAt(index);
            dataGrid.SelectedIndex = Math.Min(index, ((ObservableCollection<CreateObjectItem>)DataContext).Count - 1);
        }  

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            int index = dataGrid.SelectedIndex;
            if (index < 1) return;
            int changeindex = index - 1;
            ChangeIndex(index, changeindex);
            dataGrid.SelectedIndex = changeindex;
        }
        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            int index = dataGrid.SelectedIndex;
            if (index < 0) return;
            if (index == ((ObservableCollection<CreateObjectItem>)DataContext).Count - 1) return;
            int changeindex = index + 1;
            ChangeIndex(index, changeindex);
            dataGrid.SelectedIndex = changeindex;
        }

        private void ChangeIndex(int a, int b)
        {
            ObservableCollection<CreateObjectItem> lt = (ObservableCollection<CreateObjectItem>)DataContext;
            CreateObjectItem relay = lt[a];
            lt[a] = lt[b];
            lt[b] = relay;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            Apply();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _cancel = true;
            this.Close();
        }

        private void Apply()
        {
            CreateObjectItem[] saveItems = ((ObservableCollection<CreateObjectItem>)DataContext).ToArray();
            if (_isInput)
                GH_ComponentAttributesReplacer.StaticCreateObjectItems.InputItems[_componentGuid] = saveItems;
            else
                GH_ComponentAttributesReplacer.StaticCreateObjectItems.OutputItems[_componentGuid] = saveItems;


        }

        protected override void OnClosed(EventArgs e)
        {
            if (_cancel)
            {
                if (_isInput)
                    GH_ComponentAttributesReplacer.StaticCreateObjectItems.InputItems[_componentGuid] = _preList;
                else
                    GH_ComponentAttributesReplacer.StaticCreateObjectItems.OutputItems[_componentGuid] = _preList;
            }
            else
            {
                Apply();
                GH_ComponentAttributesReplacer.SaveToJson();
            }
            base.OnClosed(e);
        }

        private void AddButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _canvas.MouseUp += _canvas_MouseUp;
            _canvas.MouseMove += _canvas_MouseMove;
            _canvas.MouseLeave += _canvas_MouseLeave;
            _canvas.CanvasPostPaintWidgets += CanvasPostPaintWidgets;
            BaseControlItem.ShouldRespond = false;
            _canvas.Refresh();
        }

        private void _canvas_MouseLeave(object sender, EventArgs e)
        {
            finish();
            TargetParam = null;
        }

        private void _canvas_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            finish();
            if (TargetParam != null) SaveOne(TargetParam);
            TargetParam = null;
        }

        private void finish()
        {
            _canvas.MouseUp -= _canvas_MouseUp;
            _canvas.MouseLeave -= _canvas_MouseLeave;
            _canvas.MouseMove -= _canvas_MouseMove;
            _canvas.CanvasPostPaintWidgets -= CanvasPostPaintWidgets;
            Instances.CursorServer.ResetCursor(_canvas);
            BaseControlItem.ShouldRespond = true;
            _canvas.Refresh();
        }

        private void _canvas_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Instances.CursorServer.AttachCursor(_canvas, "GH_Target");
            PointF pt = _canvas.Viewport.UnprojectPoint(e.Location);
            GH_RelevantObjectData gH_RelevantObjectData = _canvas.Document.RelevantObjectAtPoint(pt, GH_RelevantObjectFilter.Attributes);
            IGH_Param param = null;
            if (gH_RelevantObjectData != null)
            {
                if(gH_RelevantObjectData.Parameter != null)
                {
                    param = gH_RelevantObjectData.Parameter;
                    if (TargetParam == param) return;
                    if ((_isInput && param.Kind != GH_ParamKind.input) || (!_isInput && param.Kind != GH_ParamKind.output))
                    {
                        TargetParam = param;
                        _canvas.Refresh();
                    }
                }
                else
                {
                    IGH_DocumentObject obj = gH_RelevantObjectData.Object;
                    if (obj == null) return;
                    IGH_Component component = (IGH_Component)obj;
                    if(component == null) return;

                    List<IGH_Param> paramLt = null;
                    if (_isInput)
                    {
                        if(component.Params.Output.Count == 0) return;
                        paramLt = component.Params.Output;

                    }
                    else if (!_isInput)
                    {
                        if (component.Params.Input.Count == 0) return;
                        paramLt = component.Params.Input;
                    }

                    if (paramLt == null) return;
                    float minDis = float.MaxValue;
                    foreach (var item in paramLt)
                    {
                        PointF pt2 = item.Attributes.Pivot;
                        float distance = (float)Math.Sqrt(Math.Pow(pt.X - pt2.X, 2) + Math.Pow(pt.Y - pt2.Y, 2));
                        if(distance < minDis)
                        {
                            minDis = distance;
                            param = item;
                        }
                    }

                    if (param == TargetParam) return;
                    TargetParam = param;
                    _canvas.Refresh();

                }
            }
        }

        private void SaveOne(IGH_Param param)
        {
            CreateObjectItem item;
            if (param.Kind == GH_ParamKind.floating)
            {
                item = new CreateObjectItem(param.ComponentGuid, 0, null, _isInput);
            }
            else
            {
                IGH_DocumentObject obj = param.Attributes.GetTopLevel.DocObject;
                if (!(obj is IGH_Component)) return;
                IGH_Component com = (GH_Component)obj;

                int index = _isInput ? com.Params.Output.IndexOf(param) : com.Params.Input.IndexOf(param);
                item = new CreateObjectItem(com.ComponentGuid, (ushort)index, null, _isInput);

            }
            ((ObservableCollection<CreateObjectItem>)DataContext).Add(item);
            dataGrid.SelectedIndex = ((ObservableCollection<CreateObjectItem>)DataContext).Count - 1;
        }

        private void CanvasPostPaintWidgets(GH_Canvas canvas)
        {
            System.Drawing.Drawing2D.Matrix transform = canvas.Graphics.Transform;
            canvas.Graphics.ResetTransform();
            System.Drawing.Rectangle clientRectangle = canvas.ClientRectangle;
            clientRectangle.Inflate(5, 5);
            Region region = new Region(clientRectangle);
            System.Drawing.Rectangle rect = System.Drawing.Rectangle.Empty;
            if (TargetParam != null)
            {
                RectangleF bounds = TargetParam.Attributes.Bounds;
                rect = GH_Convert.ToRectangle(canvas.Viewport.ProjectRectangle(bounds));
                switch (TargetParam.Kind)
                {
                    case GH_ParamKind.input:
                        rect.Inflate(2, 2);
                        break;
                    case GH_ParamKind.output:
                        rect.Inflate(2, 2);
                        break;
                    case GH_ParamKind.floating:
                        rect.Inflate(0, 0);
                        break;
                }
                region.Exclude(rect);
            }
            SolidBrush solidBrush = new SolidBrush(System.Drawing.Color.FromArgb(180, System.Drawing.Color.White));
            canvas.Graphics.FillRegion(solidBrush, region);
            solidBrush.Dispose();
            region.Dispose();
            if (TargetParam != null)
            {
                System.Drawing.Color color = _isTheSame ? System.Drawing.Color.OliveDrab : System.Drawing.Color.Chocolate;

                canvas.Graphics.DrawRectangle(new System.Drawing.Pen(color), rect);
                System.Drawing.Pen pen = new System.Drawing.Pen(color, 3f);
                int num = 6;
                int num2 = rect.Left - 4;
                int num3 = rect.Right + 4;
                int num4 = rect.Top - 4;
                int num5 = rect.Bottom + 4;
                canvas.Graphics.DrawLines(pen, new System.Drawing.Point[3]
                {
                    new System.Drawing.Point(num2 + num, num4),
                    new System.Drawing.Point(num2, num4),
                    new System.Drawing.Point(num2, num4 + num)
                });
                canvas.Graphics.DrawLines(pen, new System.Drawing.Point[3]
                {
                    new System.Drawing.Point(num3 - num, num4),
                    new System.Drawing.Point(num3, num4),
                    new System.Drawing.Point(num3, num4 + num)
                });
                canvas.Graphics.DrawLines(pen, new System.Drawing.Point[3]
                {
                    new System.Drawing.Point(num2 + num, num5),
                    new System.Drawing.Point(num2, num5),
                    new System.Drawing.Point(num2, num5 - num)
                });
                canvas.Graphics.DrawLines(pen, new System.Drawing.Point[3]
                {
                    new System.Drawing.Point(num3 - num, num5),
                    new System.Drawing.Point(num3, num5),
                    new System.Drawing.Point(num3, num5 - num)
                });
                pen.Dispose();
            }
            canvas.Graphics.Transform = transform;
        }
    }

    [ValueConversion(typeof(int), typeof(bool))]
    public class GridSelectedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            int grid = (int)value;

            return grid != -1;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(System.Drawing.Bitmap), typeof(BitmapImage))]
    public class BitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            System.Drawing.Bitmap picture = (System.Drawing.Bitmap)value;

            MemoryStream ms = new MemoryStream();
            picture.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage image = new BitmapImage();

            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
