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
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                string inputText = textBox.Text + e.Text;
                e.Handled = IsCorrect(inputText);
            }
        }
        private static bool IsCorrect(string text) => !(text.All(char.IsDigit) && int.Parse(text) <= 255);
    }
}
