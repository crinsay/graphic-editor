using Emgu.CV.Reg;
using Emgu.CV;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Emgu.CV.Structure;
using Ellipse = System.Windows.Shapes.Ellipse;
using Color = System.Windows.Media.Color;
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
    private Point _currentMouseLocation;
    private Point? _startMouseLocation = null;
    private List<Line> _lines = new();
    private DrawStyle _drawStyle = DrawStyle.Freestyle;
    private Color _selectedColor = Color.FromRgb(0, 0, 0);
    private ColorPickerWindow? _colorPickerWindow;
    private MatrixFilterWindow? _matrixFilterWindow;

    public delegate void ColorChangedHandler(Color newColor);
    public event ColorChangedHandler OnColorChanged = delegate { };

    public delegate void MatrixFilterChosenHandler();
    public event MatrixFilterChosenHandler MatrixOnColorChanged = delegate { };


    public MainWindow()
    {
        InitializeComponent();
    }

    #region ButtonClicks
    // --- Handling Clicks ---
    private void ButtonBrushClick(object sender, RoutedEventArgs e)
    {
        RestartValues();
        _drawStyle = DrawStyle.Freestyle;
    }
    private void ButtonPointClick(object sender, RoutedEventArgs e)
    {
        RestartValues();
        _drawStyle = DrawStyle.Point;
    }
    private void ButtonStraightLineClick(object sender, RoutedEventArgs e)
    {
        RestartValues();
        _drawStyle = DrawStyle.StraightLine;
    }
    private void ButtonEditStraightLineClick(object sender, RoutedEventArgs e)
    {
        RestartValues();
        _drawStyle = DrawStyle.EditStraightLine;
        MakeStraightLinePurple();
    }
    private void ButtonPolyLineClick(object sender, RoutedEventArgs e)
    {
        RestartValues();
        _drawStyle = DrawStyle.PolyLine;
    }
    private void ButtonCircleClick(object sender, RoutedEventArgs e)
    {
        RestartValues();
        _drawStyle = DrawStyle.Circle;
    }
    private void ButtonTriangleClick(object sender, RoutedEventArgs e)
    {
        RestartValues();
        _drawStyle = DrawStyle.Triangle;
    }
    private void ButtonSquareClick(object sender, RoutedEventArgs e)
    {
        RestartValues();
        _drawStyle = DrawStyle.Square;
    }
    private void ButtonPentagonClick(object sender, RoutedEventArgs e)
    {
        RestartValues();
        _drawStyle = DrawStyle.Pentagon;
    }
    private void ButtonHexagonClick(object sender, RoutedEventArgs e)
    {
        RestartValues();
        _drawStyle = DrawStyle.Hexagon;
    }
    private void ButtonStarClick(object sender, RoutedEventArgs e)
    {
        RestartValues();
        _drawStyle = DrawStyle.Star;
    }
    private void ButtonArrowClick(object sender, RoutedEventArgs e)
    {
        RestartValues();
        _drawStyle = DrawStyle.Arrow;
    }
    private void ButtonColorPickerClick(object sender, RoutedEventArgs e)
    {
        if (_colorPickerWindow == null || !_colorPickerWindow.IsLoaded)
        {
            _colorPickerWindow = new ColorPickerWindow(_selectedColor);
            _colorPickerWindow.ColorUpdated += ColorPickerWindow_ColorUpdated; // Subskrypcja zdarzenia
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
            Uri newFileUri = new Uri(saveFileDialog.FileName);
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

    private void ButtonEaserClick(object sender, RoutedEventArgs e)
    {
        RestartValues();
        _drawStyle = DrawStyle.Eraser;
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
                X1 = _currentMouseLocation.X,
                Y1 = _currentMouseLocation.Y,
                X2 = e.GetPosition(PaintingSurface).X,
                Y2 = e.GetPosition(PaintingSurface).Y
            };

            _currentMouseLocation = e.GetPosition(PaintingSurface);

            PaintingSurface.Children.Add(line);
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
            Width = 6,
            Height = 6
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

    private void MakeStraightLinePurple()
    {
        foreach (Line line in _lines)
        {
            line.Stroke = new SolidColorBrush(Colors.Purple);
        }
    }
    private void MakeStraightLineBlack()
    {
        foreach (Line line in _lines)
        {
            line.Stroke = new SolidColorBrush(_selectedColor);
        }
    }

    private void EditStraightLine()
    {
        int tolerance = 5;
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
        PaintingSurface.Children.Add(arrow);
    }

    private void Erase(object sender, MouseButtonEventArgs e)
    {
        {
            var clickedElement = e.Source as FrameworkElement;
            if (clickedElement != null)
            {
                if (PaintingSurface.Children.Contains(clickedElement))
                {
                    PaintingSurface.Children.Remove(clickedElement);
                }
            }
        }
    }

    #endregion

    private void RestartValues()
    {
        _startMouseLocation = null;
        MakeStraightLineBlack();
    }

    private void ColorPickerWindow_ColorUpdated(Color newColor)
    {
        _selectedColor = newColor;
        BorderColorPicker.Background = new SolidColorBrush(newColor);
        ButtonColorPicker.Foreground = new SolidColorBrush(Color.FromRgb((byte)(255 - _selectedColor.R), (byte)(255 - _selectedColor.G), (byte)(255 - _selectedColor.B)));
    }


    // --- File operations ---
    private void ImportFile(Uri path)
    {
        if (path == null) return;

        Image image = new();
        BitmapImage bitmapImage = new();
        bitmapImage.BeginInit();
        bitmapImage.UriSource = path;
        bitmapImage.EndInit();
        image.Source = bitmapImage;
        PaintingSurface.Children.Add(image);
    }


    private void SaveToFile(Uri path, Canvas surface)
    {
        if (path == null) return;

        Transform transform = surface.LayoutTransform;
        Size size = new(surface.ActualWidth, surface.ActualHeight);

        surface.Measure(size);
        surface.Arrange(new Rect(size));

        RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
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


    // --- Filters ---
    private void AddSobelFilter()
    {
        const string temporaryFile = "SobelFilterImage.jpg";
        var image = new Image<Rgb, byte>(temporaryFile);
        var grayImage = image.Convert<Gray, byte>();
        var graySobelImage = grayImage.Sobel(0, 1, 3);
        graySobelImage.Save("SobelFilterImage.jpg");
    }

    private void ButtonMatrixClick(object sender, RoutedEventArgs e)
    {
        if (_matrixFilterWindow == null || !_matrixFilterWindow.IsLoaded)
        {
            _matrixFilterWindow = new MatrixFilterWindow();
            _matrixFilterWindow.Show();
        }
        else
        {
            _matrixFilterWindow.Focus();
        }
    }

}