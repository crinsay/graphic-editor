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
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private Color _currentColor;

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
                    UpdateBorderColor();
                }
                OnPropertyChanged();
            }
        }
    }



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

    private void ButtonCancelClick(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void ButtonOKClick(object sender, RoutedEventArgs e)
    {
        ColorUpdated?.Invoke(_currentColor);
        this.Close();
    }

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

    private void UpdateBorderColor()
    {
        BorderPickedColor.Background = new SolidColorBrush(_currentColor);
    }


}
