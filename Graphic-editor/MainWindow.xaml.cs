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
    EditStraightLine,
    PolyLine,
    Circle,
    Triangle,
    Square,
    Pentagon,
    Hexagon,
    Star,
    Arrow
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
        foreach (Line line in _lines)
        {
            line.Stroke = new SolidColorBrush(Colors.Purple);
        }
    }
    private void MakeStraightLineBlack()
    {
        foreach (Line line in _lines)
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

    private void AddPolyLine()
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
        Brush brushColor = new SolidColorBrush(Colors.Black);
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
        Brush brushColor = new SolidColorBrush(Colors.Black);
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
        Brush brushColor = new SolidColorBrush(Colors.Black);
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
        Brush brushColor = new SolidColorBrush(Colors.Black);
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
        Brush brushColor = new SolidColorBrush(Colors.Black);
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
        Brush brushColor = new SolidColorBrush(Colors.Black);
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
        Brush brushColor = new SolidColorBrush(Colors.Black);
        arrow.Stroke = brushColor;
        PaintingSurface.Children.Add(arrow);
    }

    #endregion

    private void RestartValues()
    {
        _startMouseLocation = null;
        MakeStraightLineBlack();
    }


}
