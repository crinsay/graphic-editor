using System.Windows;
using System.Windows.Controls;

namespace Graphic_editor
{
    public partial class MatrixFilterWindow : Window
    {
        public event Action<float[,]>? FilterMatrixConfirmed;

        private float[,] _filterMatrix;

        public MatrixFilterWindow()
        {
            InitializeComponent();
            _filterMatrix = new float[3, 3];
        }

        private void ButtonApplyClick(object sender, RoutedEventArgs e)
        {
            _filterMatrix[0, 0] = GetMatrixValue(Matrix11);
            _filterMatrix[0, 1] = GetMatrixValue(Matrix12);
            _filterMatrix[0, 2] = GetMatrixValue(Matrix13);
            _filterMatrix[1, 0] = GetMatrixValue(Matrix21);
            _filterMatrix[1, 1] = GetMatrixValue(Matrix22);
            _filterMatrix[1, 2] = GetMatrixValue(Matrix23);
            _filterMatrix[2, 0] = GetMatrixValue(Matrix31);
            _filterMatrix[2, 1] = GetMatrixValue(Matrix32);
            _filterMatrix[2, 2] = GetMatrixValue(Matrix33);

            FilterMatrixConfirmed?.Invoke(_filterMatrix);

            Close();
        }

        private static float GetMatrixValue(TextBox textBox)
        {
            if (float.TryParse(textBox.Text, out float value))
            {
                return value;
            }
            return 0;
        }

        private void TextBoxMatrix_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox != null && textBox.Text.Length == 0)
                e.Handled = !(e.Text.All(char.IsDigit) || e.Text == "-");
            else
                e.Handled = !(e.Text.All(char.IsDigit));
        }
    }
}
