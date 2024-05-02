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
                var result = HammingAlgorithm.Encode(CodeTextBox.Text);
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
                (var result, int error) = HammingAlgorithm.Decode(CodeTextBox.Text);
                ResultTextBlock.Text = result;
                ErrorTextBlock.Text = error == 0 ? "" : error.ToString();
            }
            else
            {
                SnackBarNotification_Send("Введите код!", 2);
            }
        }
    }
}