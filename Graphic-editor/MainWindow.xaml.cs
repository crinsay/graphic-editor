using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Graphic_editor;

enum DrawStyle
{
    Freestyle,
    Point,
    StraightLine
}
public partial class MainWindow : Window
{
    private Point _currentMouseLocation;
    private Point? _startMouseLocation = null;
    private DrawStyle _drawStyle = DrawStyle.Freestyle;

    public MainWindow()
    {
        InitializeComponent();
    }

    // --- Changing drawing style ---
    private void ButtonBrushClick(object sender, RoutedEventArgs e)
    {
        _drawStyle = DrawStyle.Freestyle;
        _startMouseLocation = null;
    }
    private void ButtonPointClick(object sender, RoutedEventArgs e)
    {
        _drawStyle = DrawStyle.Point;
        _startMouseLocation = null;
    }
    private void ButtonStraightLineClick(object sender, RoutedEventArgs e)
    {
        _drawStyle = DrawStyle.StraightLine;
    }


    // --- Handling mouse events ---
    private void PaintingSurfaceMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && _drawStyle == DrawStyle.Freestyle)
        {
            Brush brushColor = new SolidColorBrush(Colors.Black);
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

        Brush brushColor = new SolidColorBrush(Colors.Black);
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
            Brush brushColor = new SolidColorBrush(Colors.Black);
            Line line = new()
            {
                Stroke = brushColor,
                X1 = _startMouseLocation.Value.X,
                Y1 = _startMouseLocation.Value.Y,
                X2 = _currentMouseLocation.X,
                Y2 = _currentMouseLocation.Y
            };

            PaintingSurface.Children.Add(line);

            _startMouseLocation = null;
        }
    }
}
