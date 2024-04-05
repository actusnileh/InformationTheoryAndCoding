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

        async void Button_Click(object sender, RoutedEventArgs e)
        {
            string text, decompressedHuffman, decompressedLZ77;
            byte[] compressedHuffman;
            await Task.Run(() =>
            {
                using (StreamReader file = new StreamReader(filepath))
                {
                    Dispatcher.Invoke(() => DeflateStartButton.IsEnabled = false);
                    Dispatcher.Invoke(() => ChooseFileButton.IsEnabled = false);

                    text = file.ReadToEnd();
                    Dispatcher.Invoke(() => ProgressBar.Value = 25);

                    string LZ77Encode_file = LZ77.Compress(10000, 5000, text);
                    Dispatcher.Invoke(() => ProgressBar.Value = 50);

                    Huffman huffman = new Huffman();
                    compressedHuffman = huffman.Compress(LZ77Encode_file);
                    Dispatcher.Invoke(() => ProgressBar.Value = 75);

                    decompressedHuffman = huffman.Decompress(compressedHuffman);
                    Dispatcher.Invoke(() => ProgressBar.Value = 85);

                    decompressedLZ77 = LZ77.Decompress(decompressedHuffman);
                    Dispatcher.Invoke(() => ProgressBar.Value = 95);
                }

                string compressed_filepath = filepath.Replace(".txt", "_compressed.txt");
                using (FileStream compressed_file = new FileStream(compressed_filepath, FileMode.Create))
                {
                    compressed_file.Write(compressedHuffman, 0, compressedHuffman.Length);
                }

                string decompressed_filepath = filepath.Replace(".txt", "_decompressed.txt");
                using (StreamWriter decompressed_file = new StreamWriter(decompressed_filepath))
                {
                    decompressed_file.Write(decompressedLZ77);
                }

                Dispatcher.Invoke(() =>
                {
                    ProgressBar.Value = 100;
                    SelectedFileTextBox.Text = "Готово";

                    Dispatcher.Invoke(() => DeflateStartButton.IsEnabled = true);
                    Dispatcher.Invoke(() => ChooseFileButton.IsEnabled = true);

                    FileInfo SelectedFile = new FileInfo(filepath);
                    long SelectedFileInBytes = SelectedFile.Length;

                    FileInfo EncodedFile = new FileInfo(compressed_filepath);
                    long EncodedFileInBytes = EncodedFile.Length;

                    FileInfo DecodedFile = new FileInfo(decompressed_filepath);
                    long DecodedFileInBytes = DecodedFile.Length;

                    DeflateStartButton.Content = $"{SelectedFileInBytes / 1024} -> {EncodedFileInBytes / 1024} -> {DecodedFileInBytes / 1024}";
                });
            });
        }

    }
}
