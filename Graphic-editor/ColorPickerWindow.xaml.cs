using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace Graphic_editor;

public partial class ColorPickerWindow : Window, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action<Color>? ColorUpdated;

    private Color _currentColor;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // --- Bindings ---
    private string _r = string.Empty;
    public string R
    {
        get => _r;
        set
        {
            if (_r != value)
            {
                _r = value;
                if (byte.TryParse(_r, out byte rValue))
                {
                    _currentColor.R = rValue;
                    ConvertRgbToHsv();
                    UpdateBorderColor();
                }
                OnPropertyChanged();
            }
        }
    }
    private string _g = string.Empty;
    public string G
    {
        get => _g;
        set
        {
            if (_g != value)
            {
                _g = value;
                if (byte.TryParse(_g, out byte gValue))
                {
                    _currentColor.G = gValue;
                    ConvertRgbToHsv();
                    UpdateBorderColor();
                }
                OnPropertyChanged();
            }
        }
    }
    private string _b = string.Empty;
    public string B
    {
        get => _b;
        set
        {
            if (_b != value)
            {
                _b = value;
                if (byte.TryParse(_b, out byte bValue))
                {
                    _currentColor.B = bValue;
                    ConvertRgbToHsv();
                    UpdateBorderColor();
                }
                OnPropertyChanged();
            }
        }
    }

    // --- Validation ---
    private void TextBoxRGB_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
        var textBox = sender as TextBox;
        if (textBox != null)
        {
            string inputText = textBox.Text + e.Text;
            e.Handled = IsCorrect(inputText);
        }
    }
    private static bool IsCorrect(string text) => !(text.All(char.IsDigit) && int.Parse(text) <= 255);



    // --- Constructor ---
    public ColorPickerWindow(Color initialColor)
    {
        InitializeComponent();
        DataContext = this;
        _currentColor = initialColor;

        R = _currentColor.R.ToString();
        G = _currentColor.G.ToString();
        B = _currentColor.B.ToString();

        UpdateBorderColor();
    }

    // --- Event handlers ---
    private void ButtonCancelClick(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void ButtonOKClick(object sender, RoutedEventArgs e)
    {
        ColorUpdated?.Invoke(_currentColor);
        this.Close();
    }

    // --- Methods ---
    private void UpdateBorderColor()
    {
        BorderPickedColor.Background = new SolidColorBrush(_currentColor);
    }

    private void ConvertRgbToHsv()
    {
        double r = _currentColor.R / 255.0;
        double g = _currentColor.G / 255.0;
        double b = _currentColor.B / 255.0;

        double max = Math.Max(r, Math.Max(g, b));
        double min = Math.Min(r, Math.Min(g, b));
        double d = max - min;

        double h = 0;
        double s = 0;

        double v = Math.Round(max * 100, 2);
        if (max != 0)
            s = Math.Round((double)(d / max) * 100, 2);

        if (d == 0)
        {
            h = 0;
        }
        else if (max == r)
        {
            h = 60 * (((g - b) / d) % 6);
        }
        else if (max == g)
        {
            h = 60 * (((b - r) / d) + 2);
        }
        else if (max == b)
        {
            h = 60 * (((r - g) / d) + 4);
        }
    

        TextBoxH.Text = h.ToString();
        TextBoxS.Text = s.ToString();
        TextBoxV.Text = v.ToString();
    }
}
