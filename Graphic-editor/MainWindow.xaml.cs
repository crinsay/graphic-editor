using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;
using Ellipse = System.Windows.Shapes.Ellipse;
using Image = System.Windows.Controls.Image;
using Path = System.IO.Path;
using Point = System.Windows.Point;


namespace Graphic_editor;
enum DrawStyle
{
    Freestyle,
    Point,
    StraightLine,
    EditStraightLine,
    PolyLine,
    Circle,
    Triangle,
    Square,
    Pentagon,
    Hexagon,
    Star,
    Arrow,
    Eraser
}


public partial class MainWindow : Window
{
    #region Fields
    private DrawStyle _drawStyle = DrawStyle.Freestyle;
    private Color _selectedColor = Color.FromRgb(0, 0, 0);

    private Point _currentMouseLocation;
    private Point? _startMouseLocation = null;

    private ColorPickerWindow? _colorPickerWindow;
    private MatrixFilterWindow? _matrixFilterWindow;

    private readonly List<Line> _lines = [];
    private float[,] _customFilterMatrix = new float[3, 3];
    #endregion



    #region Events
    public delegate void ColorChangedHandler(Color newColor);
    public event ColorChangedHandler OnColorChanged = delegate { };

    public delegate void MatrixFilterChosenHandler();
    public event MatrixFilterChosenHandler MatrixOnColorChanged = delegate { };
    #endregion



    public MainWindow()
    {
        InitializeComponent();
        ButtonBrush.Background = new SolidColorBrush(Color.FromRgb(194, 210, 255));
    }



    #region ButtonClicks
    // --- Handling Clicks ---
    private void ButtonBrushClick(object sender, RoutedEventArgs e)
    {
        RestartValues(ButtonBrush);
        _drawStyle = DrawStyle.Freestyle;
    }
    private void ButtonPointClick(object sender, RoutedEventArgs e)
    {
        RestartValues(ButtonPoint);
        _drawStyle = DrawStyle.Point;
    }
    private void ButtonStraightLineClick(object sender, RoutedEventArgs e)
    {
        RestartValues(ButtonStraightLine);
        _drawStyle = DrawStyle.StraightLine;
    }
    private void ButtonEditStraightLineClick(object sender, RoutedEventArgs e)
    {
        RestartValues(ButtonEditStraightLine);
        _drawStyle = DrawStyle.EditStraightLine;
        MakeLinesThicker();
    }
    private void ButtonPolyLineClick(object sender, RoutedEventArgs e)
    {
        RestartValues(ButtonPolyLine);
        _drawStyle = DrawStyle.PolyLine;
    }
    private void ButtonCircleClick(object sender, RoutedEventArgs e)
    {
        RestartValues(ButtonCircle);
        _drawStyle = DrawStyle.Circle;
    }
    private void ButtonTriangleClick(object sender, RoutedEventArgs e)
    {
        RestartValues(ButtonTriangle);
        _drawStyle = DrawStyle.Triangle;
    }
    private void ButtonSquareClick(object sender, RoutedEventArgs e)
    {
        RestartValues(ButtonSquare);
        _drawStyle = DrawStyle.Square;
    }
    private void ButtonPentagonClick(object sender, RoutedEventArgs e)
    {
        RestartValues(ButtonPentagon);
        _drawStyle = DrawStyle.Pentagon;
    }
    private void ButtonHexagonClick(object sender, RoutedEventArgs e)
    {
        RestartValues(ButtonHexagon);
        _drawStyle = DrawStyle.Hexagon;
    }
    private void ButtonStarClick(object sender, RoutedEventArgs e)
    {
        RestartValues(ButtonStar);
        _drawStyle = DrawStyle.Star;
    }
    private void ButtonArrowClick(object sender, RoutedEventArgs e)
    {
        RestartValues(ButtonArrow);
        _drawStyle = DrawStyle.Arrow;

    }
    private void ButtonEraserClick(object sender, RoutedEventArgs e)
    {
        RestartValues(ButtonEraser);
        _drawStyle = DrawStyle.Eraser;
    }
    private void ButtonColorPickerClick(object sender, RoutedEventArgs e)
    {
        if (_colorPickerWindow == null || !_colorPickerWindow.IsLoaded)
        {
            _colorPickerWindow = new ColorPickerWindow(_selectedColor);
            _colorPickerWindow.ColorUpdated += ColorPickerWindow_ColorUpdated;
            _colorPickerWindow.Show();
        }
        else
        {
            _colorPickerWindow.Focus();
        }
    }
    private void ButtonSaveAsClick(object sender, RoutedEventArgs e)
    {
        SaveFileDialog saveFileDialog = new()
        {
            Filter = "JPEG Image|*.jpg|PNG Image|*.png",
            Title = "Save an Image File"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            Uri newFileUri = new(saveFileDialog.FileName);
            SaveToFile(newFileUri, PaintingSurface);
        }
    }
    private void ButtonImportFileClick(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = "JPEG Image|*.jpg|PNG Image|*.png",
            Title = "Import an Image File"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            Uri fileUri = new(openFileDialog.FileName);
            ImportFile(fileUri);
        }
    }
    private void ButtonSobelClick(object sender, RoutedEventArgs e)
    {

        var tempFileFullPath = Path.Combine(Directory.GetCurrentDirectory(), "SobelFilterImage.jpg");
        var tempFileUri = new Uri(tempFileFullPath);
        SaveToFile(tempFileUri, PaintingSurface);

        AddSobelFilter();

        ImportFile(tempFileUri);
    }

    private void ButtonMatrixClick(object sender, RoutedEventArgs e)
    {
        if (_matrixFilterWindow == null || !_matrixFilterWindow.IsLoaded)
        {
            _matrixFilterWindow = new MatrixFilterWindow();
            _matrixFilterWindow.FilterMatrixConfirmed += OnFilterMatrixConfirmed;
            _matrixFilterWindow.Show();
        }
        else
        {
            _matrixFilterWindow.Focus();
        }
    }
    #endregion



    #region MouseEvents
    // --- Handling mouse events ---
    private void PaintingSurfaceMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && _drawStyle == DrawStyle.Freestyle)
        {
            Brush brushColor = new SolidColorBrush(_selectedColor);
            Line line = new()
            {
                Stroke = brushColor,
                StrokeThickness = 2,
                X1 = _currentMouseLocation.X,
                Y1 = _currentMouseLocation.Y,
                X2 = e.GetPosition(PaintingSurface).X,
                Y2 = e.GetPosition(PaintingSurface).Y
            };

            _currentMouseLocation = e.GetPosition(PaintingSurface);

            PaintingSurface.Children.Add(line);
        }
        else if (e.LeftButton == MouseButtonState.Pressed && _drawStyle == DrawStyle.Eraser)
        {
                    if (e.Source is FrameworkElement clickedElement)
        {
            if (PaintingSurface.Children.Contains(clickedElement))
            {
                PaintingSurface.Children.Remove(clickedElement);
            }
        }
        }
        else
            _currentMouseLocation = e.GetPosition(PaintingSurface);
    }

    private void PaintingSurfaceMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _currentMouseLocation = e.GetPosition(PaintingSurface);
        switch (_drawStyle)
        {
            case DrawStyle.Point:
                AddPoint();
                break;
            case DrawStyle.StraightLine:
                AddStraightLine();
                break;
            case DrawStyle.EditStraightLine:
                EditStraightLine();
                break;
            case DrawStyle.PolyLine:
                AddPolyLine();
                break;
            case DrawStyle.Circle:
                AddCircle();
                break;
            case DrawStyle.Triangle:
                AddTriangle();
                break;
            case DrawStyle.Square:
                AddSquare();
                break;
            case DrawStyle.Pentagon:
                AddPentagon();
                break;
            case DrawStyle.Hexagon:
                AddHexagon();
                break;
            case DrawStyle.Star:
                AddStar();
                break;
            case DrawStyle.Arrow:
                AddArrow();
                break;
            case DrawStyle.Eraser:
                Erase(sender, e);
                break;
        }
    }

    private void PaintingSurfaceMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            _currentMouseLocation = e.GetPosition(PaintingSurface);
            e.GetPosition(PaintingSurface);
        }

    }
    #endregion



    #region Drawing
    // --- Drawing methods ---
    private void AddPoint()
    {
        Ellipse elipse = new()
        {
            Width = 4,
            Height = 4
        };

        Canvas.SetTop(elipse, _currentMouseLocation.Y - elipse.Height / 2);
        Canvas.SetLeft(elipse, _currentMouseLocation.X - elipse.Width / 2);

        Brush brushColor = new SolidColorBrush(_selectedColor);
        elipse.Fill = brushColor;

        PaintingSurface.Children.Add(elipse);
    }

    private void AddStraightLine()
    {
        if (_startMouseLocation == null)
        {
            _startMouseLocation = _currentMouseLocation;
        }
        else
        {
            Brush brushColor = new SolidColorBrush(_selectedColor);
            Line line = new()
            {
                Stroke = brushColor,
                StrokeThickness = 2,
                X1 = _startMouseLocation.Value.X,
                Y1 = _startMouseLocation.Value.Y,
                X2 = _currentMouseLocation.X,
                Y2 = _currentMouseLocation.Y
            };

            _lines.Add(line);
            PaintingSurface.Children.Add(line);

            _startMouseLocation = null;
        }
    }



    private void EditStraightLine()
    {
        int tolerance = 7;
        if (_startMouseLocation == null)
        {
            foreach (Line line in _lines)
            {
                if (line.X1 > _currentMouseLocation.X - tolerance && line.X1 < _currentMouseLocation.X + tolerance
                    && line.Y1 > _currentMouseLocation.Y - tolerance && line.Y1 < _currentMouseLocation.Y + tolerance)
                {
                    _startMouseLocation = new Point(line.X1, line.Y1);
                }
                else if (line.X2 > _currentMouseLocation.X - tolerance && line.X2 < _currentMouseLocation.X + tolerance
                    && line.Y2 > _currentMouseLocation.Y - tolerance && line.Y2 < _currentMouseLocation.Y + tolerance)
                {
                    _startMouseLocation = new Point(line.X2, line.Y2);
                }
            }
        }
        else
        {
            foreach (Line line in _lines)
            {
                if (line.X1 == _startMouseLocation.Value.X && line.Y1 == _startMouseLocation.Value.Y)
                {
                    line.X1 = _currentMouseLocation.X;
                    line.Y1 = _currentMouseLocation.Y;
                }
                else if (line.X2 == _startMouseLocation.Value.X && line.Y2 == _startMouseLocation.Value.Y)
                {
                    line.X2 = _currentMouseLocation.X;
                    line.Y2 = _currentMouseLocation.Y;
                }
            }
            _startMouseLocation = null;
        }
    }

    private void AddPolyLine()
    {
        if (_startMouseLocation == null)
        {
            _startMouseLocation = _currentMouseLocation;
        }
        else
        {
            Brush brushColor = new SolidColorBrush(_selectedColor);
            Line line = new()
            {
                Stroke = brushColor,
                StrokeThickness = 2,
                X1 = _startMouseLocation.Value.X,
                Y1 = _startMouseLocation.Value.Y,
                X2 = _currentMouseLocation.X,
                Y2 = _currentMouseLocation.Y
            };
            _lines.Add(line);
            PaintingSurface.Children.Add(line);
            _startMouseLocation = _currentMouseLocation;
        }
    }

    private void AddCircle()
    {
        Ellipse elipse = new()
        {
            Width = 100,
            StrokeThickness = 2,
            Height = 100
        };
        Canvas.SetTop(elipse, _currentMouseLocation.Y - elipse.Height / 2);
        Canvas.SetLeft(elipse, _currentMouseLocation.X - elipse.Width / 2);
        Brush brushColor = new SolidColorBrush(_selectedColor);
        elipse.Stroke = brushColor;
        PaintingSurface.Children.Add(elipse);
    }
    private void AddTriangle()
    {
        Polygon triangle = new()
        {
            Points =
            [
                new Point(_currentMouseLocation.X, _currentMouseLocation.Y - 50),
                new Point(_currentMouseLocation.X - 50, _currentMouseLocation.Y + 50),
                new Point(_currentMouseLocation.X + 50, _currentMouseLocation.Y + 50)
            ]
        };
        Brush brushColor = new SolidColorBrush(_selectedColor);
        triangle.Stroke = brushColor;
        triangle.StrokeThickness = 2;
        PaintingSurface.Children.Add(triangle);
    }
    private void AddSquare()
    {
        Rectangle square = new()
        {
            Width = 100,
            Height = 100
        };
        Canvas.SetTop(square, _currentMouseLocation.Y - square.Height / 2);
        Canvas.SetLeft(square, _currentMouseLocation.X - square.Width / 2);
        Brush brushColor = new SolidColorBrush(_selectedColor);
        square.Stroke = brushColor;
        square.StrokeThickness = 2;
        PaintingSurface.Children.Add(square);
    }

    private void AddPentagon()
    {
        Polygon pentagon = new()
        {
            Points =
            [
                new Point(_currentMouseLocation.X, _currentMouseLocation.Y - 50),
                new Point(_currentMouseLocation.X - 48, _currentMouseLocation.Y - 15),
                new Point(_currentMouseLocation.X - 29, _currentMouseLocation.Y + 40),
                new Point(_currentMouseLocation.X + 29, _currentMouseLocation.Y + 40),
                new Point(_currentMouseLocation.X + 48, _currentMouseLocation.Y - 15)
            ]
        };
        Brush brushColor = new SolidColorBrush(_selectedColor);
        pentagon.Stroke = brushColor;
        pentagon.StrokeThickness = 2;
        PaintingSurface.Children.Add(pentagon);
    }

    private void AddHexagon()
    {
        Polygon hexagon = new()
        {
            Points =
            [
                new Point(_currentMouseLocation.X, _currentMouseLocation.Y - 50),
                new Point(_currentMouseLocation.X + 43, _currentMouseLocation.Y - 25),
                new Point(_currentMouseLocation.X + 43, _currentMouseLocation.Y + 25),
                new Point(_currentMouseLocation.X, _currentMouseLocation.Y + 50),
                new Point(_currentMouseLocation.X - 43, _currentMouseLocation.Y + 25),
                new Point(_currentMouseLocation.X - 43, _currentMouseLocation.Y - 25)
            ]
        };
        Brush brushColor = new SolidColorBrush(_selectedColor);
        hexagon.Stroke = brushColor;
        hexagon.StrokeThickness = 2;
        PaintingSurface.Children.Add(hexagon);
    }

    private void AddStar()
    {
        Polygon star = new()
        {
            Points =
            [
                new Point(_currentMouseLocation.X, _currentMouseLocation.Y - 50),
                new Point(_currentMouseLocation.X + 15, _currentMouseLocation.Y - 15),
                new Point(_currentMouseLocation.X + 50, _currentMouseLocation.Y - 15),
                new Point(_currentMouseLocation.X + 20, _currentMouseLocation.Y + 10),
                new Point(_currentMouseLocation.X + 35, _currentMouseLocation.Y + 50),
                new Point(_currentMouseLocation.X, _currentMouseLocation.Y + 25),
                new Point(_currentMouseLocation.X - 35, _currentMouseLocation.Y + 50),
                new Point(_currentMouseLocation.X - 20, _currentMouseLocation.Y + 10),
                new Point(_currentMouseLocation.X - 50, _currentMouseLocation.Y - 15),
                new Point(_currentMouseLocation.X - 15, _currentMouseLocation.Y - 15)
            ]
        };
        Brush brushColor = new SolidColorBrush(_selectedColor);
        star.Stroke = brushColor;
        star.StrokeThickness = 2;
        PaintingSurface.Children.Add(star);
    }

    private void AddArrow()
    {
        Polygon arrow = new()
        {
            Points =
            [
                new Point(_currentMouseLocation.X - 50, _currentMouseLocation.Y +25),
                new Point(_currentMouseLocation.X - 50, _currentMouseLocation.Y - 25),
                new Point(_currentMouseLocation.X + 50, _currentMouseLocation.Y - 25),
                new Point(_currentMouseLocation.X + 50, _currentMouseLocation.Y - 50),
                new Point(_currentMouseLocation.X + 100, _currentMouseLocation.Y),
                new Point(_currentMouseLocation.X + 50, _currentMouseLocation.Y + 50),
                new Point(_currentMouseLocation.X + 50, _currentMouseLocation.Y + 25)
            ]
        };
        Brush brushColor = new SolidColorBrush(_selectedColor);
        arrow.Stroke = brushColor;
        arrow.StrokeThickness = 2;
        PaintingSurface.Children.Add(arrow);
    }

    private void Erase(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is FrameworkElement clickedElement)
        {
            if (PaintingSurface.Children.Contains(clickedElement))
            {
                PaintingSurface.Children.Remove(clickedElement);
            }
        }
    }

    #endregion



    #region EventHandlers
    // --- Event handlers ---
    private void ColorPickerWindow_ColorUpdated(Color newColor)
    {
        _selectedColor = newColor;
        BorderColorPicker.Background = new SolidColorBrush(newColor);
        ButtonColorPicker.Foreground = new SolidColorBrush(Color.FromRgb((byte)(255 - _selectedColor.R), (byte)(255 - _selectedColor.G), (byte)(255 - _selectedColor.B)));
    }
    private void OnFilterMatrixConfirmed(float[,] filterMatrix)
    {
        _customFilterMatrix = filterMatrix;
        var tempFileFullPath = Path.Combine(Directory.GetCurrentDirectory(), "MatrixFilterImage.jpg");
        var tempFileUri = new Uri(tempFileFullPath);
        SaveToFile(tempFileUri, PaintingSurface);

        AddMatrixFilter(_customFilterMatrix);
        ImportFile(tempFileUri);
    }
    #endregion



    #region FileOperations
    // --- File operations ---
    private void ImportFile(Uri path)
    {
        if (path == null) return;

        var bitmapImage = new BitmapImage();
        using (var fileStream = new FileStream(path.LocalPath, FileMode.Open, FileAccess.Read))
        {
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = fileStream;
            bitmapImage.EndInit();
        }

        var image = new Image
        {
            Source = bitmapImage
        };

        PaintingSurface.Children.Add(image);
        ResizeWindow();
    }


    private static void SaveToFile(Uri path, Canvas surface)
    {
        if (path == null) return;

        Transform transform = surface.LayoutTransform;
        Size size = new(surface.ActualWidth, surface.ActualHeight);

        surface.Measure(size);
        surface.Arrange(new Rect(size));

        RenderTargetBitmap renderTargetBitmap = new(
            (int)size.Width,
            (int)size.Height,
            96d,
            96d,
            PixelFormats.Pbgra32);

        renderTargetBitmap.Render(surface);

        using (FileStream outStream = new(path.LocalPath, FileMode.Create))
        {
            PngBitmapEncoder encoder = new();
            encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            encoder.Save(outStream);
        }

        surface.LayoutTransform = transform;
    }
    #endregion



    #region Filters
    // --- Filters ---
    private static void AddSobelFilter()
    {
        const string temporaryFile = "SobelFilterImage.jpg";
        var image = new Image<Rgb, byte>(temporaryFile);
        var grayImage = image.Convert<Gray, byte>();
        var graySobelImage = grayImage.Sobel(0, 1, 3);
        graySobelImage.Save("SobelFilterImage.jpg");
    }

    private void AddMatrixFilter(float[,] matrix)
    {
        const string temporaryFile = "MatrixFilterImage.jpg";
        var tempFileFullPath = Path.Combine(Directory.GetCurrentDirectory(), temporaryFile);
        var tempFileUri = new Uri(tempFileFullPath);
        SaveToFile(tempFileUri, PaintingSurface);

        var image = new Image<Rgb, byte>(temporaryFile);
        var kernel = new ConvolutionKernelF(matrix);
        var dst = new Mat(image.Size, DepthType.Cv8U, 3);
        var anchor = new System.Drawing.Point(-1, -1);
        CvInvoke.Filter2D(image,
                          dst,
                          kernel,
                          anchor);
        dst.Save(temporaryFile);

    }

    #endregion



    #region HelperMethods
    // --- Helper methods ---
    private void RestartValues(Button button)
    {
        // --- Restarting buttons backgrounds ---
        ButtonBrush.Background = new SolidColorBrush(Color.FromRgb(214, 225, 255));
        ButtonPoint.Background = new SolidColorBrush(Color.FromRgb(214, 225, 255));
        ButtonStraightLine.Background = new SolidColorBrush(Color.FromRgb(214, 225, 255));
        ButtonEditStraightLine.Background = new SolidColorBrush(Color.FromRgb(214, 225, 255));
        ButtonPolyLine.Background = new SolidColorBrush(Color.FromRgb(214, 225, 255));
        ButtonCircle.Background = new SolidColorBrush(Color.FromRgb(214, 225, 255));
        ButtonTriangle.Background = new SolidColorBrush(Color.FromRgb(214, 225, 255));
        ButtonSquare.Background = new SolidColorBrush(Color.FromRgb(214, 225, 255));
        ButtonPentagon.Background = new SolidColorBrush(Color.FromRgb(214, 225, 255));
        ButtonHexagon.Background = new SolidColorBrush(Color.FromRgb(214, 225, 255));
        ButtonStar.Background = new SolidColorBrush(Color.FromRgb(214, 225, 255));
        ButtonArrow.Background = new SolidColorBrush(Color.FromRgb(214, 225, 255));
        ButtonEraser.Background = new SolidColorBrush(Color.FromRgb(214, 225, 255));

        _startMouseLocation = null;
        MakeLinesNormal();

        button.Background = new SolidColorBrush(Color.FromRgb(194, 210, 255));
    }

    private void MakeLinesThicker()
    {
        foreach (Line line in _lines)
        {
            line.StrokeThickness = 5;
        }
    }
    private void MakeLinesNormal()
    {
        foreach (Line line in _lines)
        {
            line.StrokeThickness = 2;
        }
    }

    private void ResizeWindow()
    {
        var currentWidth = this.Width;
        var currentHeight = this.Height;
        this.Width = currentWidth + 1;
        this.Height = currentHeight + 1;
        this.Width = currentWidth;
        this.Height = currentHeight;
    }
    #endregion
}