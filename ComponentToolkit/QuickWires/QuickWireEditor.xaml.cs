using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
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

        private IGH_Param _param;

        private bool _isInput;
        private Guid _componentGuid;
        public QuickWireEditor(Guid componenguid, bool isInput, Bitmap icon)
        {
            this._isInput = isInput;

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
        }
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _canvas.MouseUp += _canvas_MouseUp;
            _canvas.MouseMove += _canvas_MouseMove;
            _canvas.MouseLeave += _canvas_MouseLeave;
            _canvas.CanvasPostPaintWidgets += CanvasPostPaintWidgets;
            _canvas.Refresh();
        }

        private void _canvas_MouseLeave(object sender, EventArgs e)
        {
            _canvas.MouseUp -= _canvas_MouseUp;
            _canvas.MouseLeave -= _canvas_MouseLeave;
            _canvas.MouseMove -= _canvas_MouseMove;
            _canvas.CanvasPostPaintWidgets -= CanvasPostPaintWidgets;
            Instances.CursorServer.ResetCursor(_canvas);
            _canvas.Refresh();
            _param = null;
        }

        private void _canvas_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _canvas.MouseUp -= _canvas_MouseUp;
            _canvas.MouseLeave -= _canvas_MouseLeave;
            _canvas.MouseMove -= _canvas_MouseMove;
            _canvas.CanvasPostPaintWidgets -= CanvasPostPaintWidgets;
            Instances.CursorServer.ResetCursor(_canvas);
            _canvas.Refresh();

            if (_param != null) SaveOne(_param);
            _param = null;
        }

        private void _canvas_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Instances.CursorServer.AttachCursor(_canvas, "GH_Target");
            System.Drawing.PointF pt = _canvas.Viewport.UnprojectPoint(e.Location);
            GH_RelevantObjectData gH_RelevantObjectData = _canvas.Document.RelevantObjectAtPoint(pt, GH_RelevantObjectFilter.Attributes);
            IGH_Param param = null;
            if (gH_RelevantObjectData != null && gH_RelevantObjectData.Parameter != null)
            {
                param = gH_RelevantObjectData.Parameter;
                if (_param != param)
                {
                    if (_isInput && param.Kind != GH_ParamKind.input)
                    {
                        _param = param;
                        _canvas.Refresh();
                    }
                    else if (!_isInput && param.Kind != GH_ParamKind.output)
                    {
                        _param = param;
                        _canvas.Refresh();
                    }
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

                int index = _isInput ? com.Params.Input.IndexOf(param) : com.Params.Output.IndexOf(param);

                item = new CreateObjectItem(com.ComponentGuid, (ushort)index, null, _isInput);

            }
            ((List<CreateObjectItem>)DataContext).Add(item);
        }

        private void CanvasPostPaintWidgets(GH_Canvas canvas)
        {
            System.Drawing.Drawing2D.Matrix transform = canvas.Graphics.Transform;
            canvas.Graphics.ResetTransform();
            System.Drawing.Rectangle clientRectangle = canvas.ClientRectangle;
            clientRectangle.Inflate(5, 5);
            Region region = new Region(clientRectangle);
            System.Drawing.Rectangle rect = System.Drawing.Rectangle.Empty;
            if (_param != null)
            {
                RectangleF bounds = _param.Attributes.Bounds;
                rect = GH_Convert.ToRectangle(canvas.Viewport.ProjectRectangle(bounds));
                switch (_param.Kind)
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
            if (_param != null)
            {
                canvas.Graphics.DrawRectangle(Pens.Black, rect);
                System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Black, 3f);
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

    [ValueConversion(typeof(List< CreateObjectItem>), typeof(ObservableCollection<CreateObjectItem>))]
    public class CreateItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            List<CreateObjectItem> structure = (List<CreateObjectItem>)value;
            if (structure == null) return null;

            ObservableCollection<CreateObjectItem> structureLists = new ObservableCollection<CreateObjectItem>(structure);

            return structureLists;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            ObservableCollection<CreateObjectItem> structure = (ObservableCollection<CreateObjectItem>)value;
            if (structure == null) return null;
            List<CreateObjectItem> structureLists = new List<CreateObjectItem>(structure);
            return structureLists;
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
