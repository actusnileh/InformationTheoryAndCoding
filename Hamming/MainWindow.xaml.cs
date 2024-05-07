using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Hamming
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        int[,] matrix;
        int type_matrix = 0;

        void SnackBarNotification_Send(string text, int time)
        {
            SnackBarNotification.MessageQueue?.Enqueue(
            text,
            null,
            null,
            null,
            false,
            true,
            TimeSpan.FromSeconds(time)
            );
        }

        void CodeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (c != '0' && c != '1')
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        void EncodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (CodeTextBox.Text.Length > 0)
            {
                (var result, matrix) = HammingAlgorithm.Encode(CodeTextBox.Text);
                type_matrix = 1;
                ResultTextBlock.Text = result;
                ErrorTextBlock.Text = "";
            }
            else
            {
                SnackBarNotification_Send("Введите код!", 2);
            }
        }

        void DecodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (CodeTextBox.Text.Length > 0)
            {
                (var result, int error, matrix) = HammingAlgorithm.Decode(CodeTextBox.Text);
                ResultTextBlock.Text = result;
                type_matrix = 2;
                ErrorTextBlock.Text = error == 0 ? "" : error.ToString();
            }
            else
            {
                SnackBarNotification_Send("Введите код!", 2);
            }
        }

        void MatrixButton_Click(object sender, RoutedEventArgs e)
        {
            if (matrix != null && type_matrix != 0)
            {
                if (type_matrix == 1)
                {
                    StringBuilder sb = new();
                    for (int i = 0; i < matrix.GetLength(0); i++)
                    {
                        for (int j = 0; j < matrix.GetLength(1); j++)
                        {
                            sb.Append(matrix[i, j]);
                            sb.Append("");
                        }
                        sb.Append($" [r{i}]");
                        sb.AppendLine();
                    }
                    MessageBox.Show(sb.ToString(), "Матрица");
                }
                else if (type_matrix == 2)
                {
                    StringBuilder sb = new();
                    for (int i = 0; i < matrix.GetLength(0); i++)
                    {
                        for (int j = 0; j < matrix.GetLength(1); j++)
                        {
                            sb.Append(matrix[i, j]);
                            sb.Append("");
                        }
                        sb.Append($" [s{i}]");
                        sb.AppendLine();
                    }
                    MessageBox.Show(sb.ToString(), "Матрица");
                }
            }
            else
            {
                SnackBarNotification_Send("Введите код!", 2);
            }
        }
    }
}