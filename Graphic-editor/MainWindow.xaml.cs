using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Graphic_editor;

enum  DrawStyle
{
    Freestyle,
    Point
}
public partial class MainWindow : Window
{
    private Point _currentMouseLocation = new();
    private DrawStyle _drawStyle = DrawStyle.Freestyle;

    public MainWindow()
    {
        InitializeComponent();
    }

    // --- Changing drawing style ---
    private void ButtonBrushClick(object sender, RoutedEventArgs e)
    {
        _drawStyle = DrawStyle.Freestyle;
    }
    private void ButtonPointClick(object sender, RoutedEventArgs e)
    {
        _drawStyle = DrawStyle.Point;
    }


    // --- Handling mouse events ---
    private void paintingSurfaceMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && _drawStyle == DrawStyle.Freestyle)
        {
            Brush brushColor = new SolidColorBrush(Colors.Black);
            Line line = new Line();
            line.Stroke = brushColor;
            line.X1 = _currentMouseLocation.X;
            line.Y1 = _currentMouseLocation.Y;
            line.X2 = e.GetPosition(paintingSurface).X; 
            line.Y2 = e.GetPosition(paintingSurface).Y;

            _currentMouseLocation = e.GetPosition(paintingSurface);

            paintingSurface.Children.Add(line);
        }
        else
            _currentMouseLocation = e.GetPosition(paintingSurface);
    }

    private void paintingSurfaceMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _currentMouseLocation = e.GetPosition(paintingSurface);
        switch(_drawStyle)
        {
            case DrawStyle.Point:
                AddPoint();
                break;

        }
    }

    private void paintingSurfaceMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            _currentMouseLocation = e.GetPosition(paintingSurface);
            e.GetPosition(paintingSurface);
        }

    }


    // --- Drawing methods ---
    private void AddPoint()
    {
        Ellipse elipse = new Ellipse();
        elipse.Width = 6;
        elipse.Height = 6;

        Canvas.SetTop(elipse, _currentMouseLocation.Y - elipse.Height / 2);
        Canvas.SetLeft(elipse, _currentMouseLocation.X - elipse.Width / 2);

        Brush brushColor = new SolidColorBrush(Colors.Black);
        elipse.Fill = brushColor;

        paintingSurface.Children.Add(elipse);
    }


}
