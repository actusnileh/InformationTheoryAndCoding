using System.Text;
using System.Text.RegularExpressions;

namespace ConvulationCode
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        bool codeswitcher_result = false;
        string pattern = "^[01]+$";
        void ValueSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            int sliderValue = (int)e.NewValue;
            ValueSliderLabel.Content = (sliderValue).ToString();

            string oldSequence = CodeTextBox.Text;
            int sequenceLength = oldSequence.Length;

            int bitsToModify = (int)Math.Ceiling(sequenceLength * sliderValue / 100.0);

            Random random = new();

            StringBuilder modifiedSequence = new(oldSequence);

            for (int i = 0; i < bitsToModify; i++)
            {
                int index = random.Next(sequenceLength);

                modifiedSequence[index] = (modifiedSequence[index] == '0') ? '1' : '0';
            }
            CodeTextBox.Text = modifiedSequence.ToString();
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
            if (CodeTextBox.Text.Length == 0)
            {
                return;
            }
            else
            {
                if (!codeswitcher_result && Regex.IsMatch(CodeTextBox.Text, pattern))
                {
                    CodeSwitcher.IsChecked = false;
                    ResultTextBox.Text = Convulation.Encode(CodeTextBox.Text);
                }
                else
                {
                    CodeSwitcher.IsChecked = true;
                    string bitSequence = string.Join("", CodeTextBox.Text.Select(c => Convert.ToString(c, 2).PadLeft(8, '0')));
                    ResultTextBox.Text = Convulation.Encode(bitSequence);
                }
            }
        }

        void DecodeButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (CodeTextBox.Text.Length == 0)
            {
                return;
            }
            else
            {
                if (!Regex.IsMatch(CodeTextBox.Text, pattern))
                    return;
                if (!codeswitcher_result)
                {
                    CodeSwitcher.IsChecked = false;
                    ResultTextBox.Text = Convulation.Decode(CodeTextBox.Text);
                }
                else
                {
                    CodeSwitcher.IsChecked = true;
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
}