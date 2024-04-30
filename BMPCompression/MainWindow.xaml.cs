using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace BMPCompression
{
    public partial class MainWindow : Window
    {
        string filepath;
        byte[] compressedHuffman;
        byte[] bytes;
        string base64String;

        Huffman huffman = new();

        string container_filepath;
        string secret_filepath;
        public MainWindow()
        {
            InitializeComponent();
        }

        async void DeflateImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".bmp",
                Filter = "(*.bmp)|*.bmp",
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                filepath = dialog.FileName;
                TextCompressed.Text = $"Выбран файл: {dialog.SafeFileName}";
                await Task.Run(() =>
                {
                    Dispatcher.Invoke(() => DeflateImage.IsEnabled = false);
                    Dispatcher.Invoke(() => InflateImage.IsEnabled = false);
                    Dispatcher.Invoke(() => ProgressBar.Value = 25);

                    using (FileStream BMPAsBytes = new(filepath, FileMode.Open, FileAccess.Read))
                    {
                        bytes = new byte[BMPAsBytes.Length];
                        BMPAsBytes.Read(bytes, 0, bytes.Length);
                    }

                    base64String = Convert.ToBase64String(bytes);
                    string LZ77Encode_file = LZ77.Compress(10000, 5000, base64String);
                    Dispatcher.Invoke(() => ProgressBar.Value = 50);

                    compressedHuffman = huffman.Compress(LZ77Encode_file);
                    Dispatcher.Invoke(() => ProgressBar.Value = 85);

                    string compressed_filepath = filepath.Replace(".bmp", "_compressed.bmp");
                    using (FileStream compressed_file = new FileStream(compressed_filepath, FileMode.Create))
                    {
                        compressed_file.Write(compressedHuffman, 0, compressedHuffman.Length);
                    }

                    Dispatcher.Invoke(() =>
                    {
                        ProgressBar.Value = 100;
                        TextCompressed.Text = "Готово";

                        Dispatcher.Invoke(() => DeflateImage.IsEnabled = true);
                        Dispatcher.Invoke(() => InflateImage.IsEnabled = true);
                    });
                });
            }
            else
            {
                TextCompressed.Text = "Файл не выбран";
            }
        }

        async void InflateImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".bmp",
                Filter = "(*.bmp)|*.bmp",
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                await Task.Run(() =>
                {

                    Dispatcher.Invoke(() => DeflateImage.IsEnabled = false);
                    Dispatcher.Invoke(() => InflateImage.IsEnabled = false);
                    Dispatcher.Invoke(() => TextCompressed.Text = $"Выбран файл: {dialog.SafeFileName}");
                    string filepath = dialog.FileName;

                    byte[] compressedBytes = File.ReadAllBytes(filepath);
                    Dispatcher.Invoke(() => ProgressBar.Value = 25);

                    string decompressedHuffman = huffman.Decompress(compressedBytes);
                    Dispatcher.Invoke(() => ProgressBar.Value = 50);

                    string decompressedLZ77 = LZ77.Decompress(decompressedHuffman);
                    Dispatcher.Invoke(() => ProgressBar.Value = 85);

                    string decompressedFilePath = filepath.Replace(".bmp", "_decompressed.bmp");
                    byte[] convertedBytes = Convert.FromBase64String(decompressedLZ77);
                    using (FileStream fileStream = new(decompressedFilePath, FileMode.Create, FileAccess.Write))
                    {
                        fileStream.Write(convertedBytes, 0, convertedBytes.Length);
                    }
                    Dispatcher.Invoke(() =>
                    {
                        TextCompressed.Text = "Готово";
                        ProgressBar.Value = 100;

                        DeflateImage.IsEnabled = true;
                        InflateImage.IsEnabled = true;
                    });

                });
            }
            else
                TextCompressed.Text = "Файл не выбран";
        }

        void ContainerButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".bmp",
                Filter = "(*.bmp)|*.bmp",
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                SecretButton.IsEnabled = true;
                container_filepath = dialog.FileName;
                ContainerButton.Content = dialog.SafeFileName;
            }
            else
                ContainerButton.Content = "Файл не выбран";

        }

        void SecretButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".bmp",
                Filter = "(*.bmp)|*.bmp",
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                secret_filepath = dialog.FileName;
                SecretButton.Content = dialog.SafeFileName;
                CreateSecretButton.IsEnabled = true;
            }
            else
            {
                SecretButton.Content = "Файл не выбран";
            }
        }

        void CreateSecretButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}