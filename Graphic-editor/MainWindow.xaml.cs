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
    StraightLine,
    EditStraightLine
}
public partial class MainWindow : Window
{
    private Point _currentMouseLocation;
    private Point? _startMouseLocation = null;
    private List<Line> _lines = new();
    private DrawStyle _drawStyle = DrawStyle.Freestyle;

    public MainWindow()
    {
        InitializeComponent();
    }

    #region ButtonClicks
    // --- Changing drawing style ---
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
    #endregion


    #region MouseEvents
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
            case DrawStyle.EditStraightLine:
                EditStraightLine();
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

            _lines.Add(line);
            PaintingSurface.Children.Add(line);

            _startMouseLocation = null;
        }
    }

    private void MakeStraightLinePurple()
    {
        foreach(Line line in _lines)
        {
            line.Stroke = new SolidColorBrush(Colors.Purple);
        }
    }
    private void MakeStraightLineBlack()
    {
        foreach(Line line in _lines)
        {
            line.Stroke = new SolidColorBrush(Colors.Black);
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
    #endregion

    private void RestartValues()
    {
        _startMouseLocation = null;
        MakeStraightLineBlack();
    }
}
