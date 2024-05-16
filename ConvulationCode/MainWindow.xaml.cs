using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace ConvulationCode
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        int slider_value = 0;
        bool codeswitcher_result = false;

        void ValueSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            slider_value = (int)e.NewValue;
            ValueSliderLabel.Content = slider_value.ToString();
        }

        void CodeSwitcher_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            codeswitcher_result = CodeSwitcher.IsChecked ?? false;
        }

        void CodeSwitcher_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            codeswitcher_result = CodeSwitcher.IsChecked ?? false;
        }

        void EncodeButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!codeswitcher_result)
            {
                ResultTextBox.Text = Convulation.Encode(CodeTextBox.Text);
            }
            else
            {
                string bitSequence = string.Join("", CodeTextBox.Text.Select(c => Convert.ToString(c, 2).PadLeft(8, '0')));
                ResultTextBox.Text = Convulation.Encode(bitSequence);
            }
        }

        void DecodeButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!codeswitcher_result)
                ResultTextBox.Text = Convulation.Decode(CodeTextBox.Text);
            else
            {
                string bitSequence = Convulation.Decode(CodeTextBox.Text);

                List<string> bytes = [];

                for (int i = 0; i < bitSequence.Length; i += 8)
                {
                    bytes.Add(bitSequence.Substring(i, 8));
                }

                StringBuilder text = new StringBuilder();
                foreach (string byteStr in bytes)
                {
                    int charCode = Convert.ToInt32(byteStr, 2);
                    text.Append((char)charCode);
                }

                ResultTextBox.Text = text.ToString();
            }
        }
    }
}