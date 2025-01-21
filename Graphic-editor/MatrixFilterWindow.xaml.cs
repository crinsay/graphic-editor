using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using static System.Net.Mime.MediaTypeNames;

namespace Graphic_editor
{
    public partial class MatrixFilterWindow : Window
    {
        public MatrixFilterWindow()
        {
            InitializeComponent();
        }

        private void ButtonApplyClick(object sender, RoutedEventArgs e)
        {

        }

        private void TextBoxMatrix_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text.Length == 0)
                e.Handled = !(e.Text.All(char.IsDigit) || e.Text=="-");
            else
            e.Handled = !(e.Text.All(char.IsDigit));
        }
    }
}
