using Microsoft.Win32;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace deflate_lab
{
    public partial class MainWindow : Window
    {
        string filepath;

        public MainWindow()
        {
            InitializeComponent();
        }

        void ButtonChooseFile(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".txt",
                Filter = "(*.txt)|*.txt",
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                filepath = dialog.FileName;
                SelectedFileTextBox.Text = $"Выбран файл: {dialog.SafeFileName}";
                SelectedFileTextBox.Foreground = Brushes.Gray;
            }
            else
            {
                SelectedFileTextBox.Text = "Файл не выбран";
                SelectedFileTextBox.Foreground = Brushes.LightGray;
            }
        }

        string text;
        byte[] compressedHuffman;
        Huffman huffman = new Huffman();

        void Infalte_Button_Click(object s, RoutedEventArgs e)
        {
            string compressedFilePath = filepath.Replace(".txt", "_compressed.txt");

            byte[] compressedBytes = File.ReadAllBytes(compressedFilePath);

            string decompressedHuffman = huffman.Decompress(compressedBytes);
            string decompressedLZ77 = LZ77.Decompress(decompressedHuffman);


            string decompressedFilePath = filepath.Replace(".txt", "_decompressed.txt");
            using (StreamWriter decompressedFile = new StreamWriter(decompressedFilePath))
            {
                decompressedFile.Write(decompressedLZ77);
            }
        }
    

        async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                using (StreamReader file = new StreamReader(filepath))
                {
                    Dispatcher.Invoke(() => DeflateStartButton.IsEnabled = false);
                    Dispatcher.Invoke(() => ChooseFileButton.IsEnabled = false);
                    Dispatcher.Invoke(() => InflateStartButton.IsEnabled ^= false);
                    text = file.ReadToEnd();
                    Dispatcher.Invoke(() => ProgressBar.Value = 25);

                    string LZ77Encode_file = LZ77.Compress(10000, 5000, text);
                    Dispatcher.Invoke(() => ProgressBar.Value = 50);

                    compressedHuffman = huffman.Compress(LZ77Encode_file);
                    Dispatcher.Invoke(() => ProgressBar.Value = 85);

                }

                string compressed_filepath = filepath.Replace(".txt", "_compressed.txt");
                using (FileStream compressed_file = new FileStream(compressed_filepath, FileMode.Create))
                {
                    compressed_file.Write(compressedHuffman, 0, compressedHuffman.Length);
                }

                Dispatcher.Invoke(() =>
                {
                    ProgressBar.Value = 100;
                    SelectedFileTextBox.Text = "Готово";

                    Dispatcher.Invoke(() => DeflateStartButton.IsEnabled = true);
                    Dispatcher.Invoke(() => ChooseFileButton.IsEnabled = true);

                    FileInfo EncodedFile = new FileInfo(compressed_filepath);
                    long EncodedFileInBytes = EncodedFile.Length;

                    DeflateStartButton.Content = $"{EncodedFileInBytes / 1024}";
                    InflateStartButton.IsEnabled = true;
                });
            });
        }

    }
}
