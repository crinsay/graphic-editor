using System;
using System.Collections.Generic;
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

namespace Graphic_editor
{
    /// <summary>
    /// Interaction logic for ColorPickerWindow.xaml
    /// </summary>
    public partial class ColorPickerWindow : Window
    {
        public ColorPickerWindow()
        {
            InitializeComponent();
        }

        private void ButtonCancelClick(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonOKClick(object sender, RoutedEventArgs e)
        {

        }

        private void TextBoxRGB_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsDigit(e.Text);
        }
        private void TextBoxH_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsDigit(e.Text);
        }
        private void TextBoxSV_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsDigit(e.Text);
        }

        // Metoda sprawdzająca, czy tekst zawiera dozwolone znaki
        private static bool IsDigit(string text)
        {
            return text.All(char.IsDigit);
        }
    }
}
