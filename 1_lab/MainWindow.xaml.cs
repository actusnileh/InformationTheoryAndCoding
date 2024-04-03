using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media;

namespace deflate_lab
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string filepath;
        string filename;

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string text, compressedHuffman, decompressedHuffman, decompressedLZ77;

            using (StreamReader file = new StreamReader(filepath))
            {
                text = file.ReadToEnd();
                string LZ77Encode_file = LZ77.Compress(100, 50, text);

                Huffman huffman = new Huffman();
                compressedHuffman = huffman.Compress(LZ77Encode_file);

                decompressedHuffman = huffman.Decompress(compressedHuffman);
                decompressedLZ77 = LZ77.Decompress(decompressedHuffman);
            }

            string compressed_filepath = filepath.Replace(".txt", "_compressed.txt");
            using (StreamWriter compressed_file = new StreamWriter(compressed_filepath))
            {
                compressed_file.Write(compressedHuffman);
            }
            SelectedFileTextBox.Text = "Закодировано";

            string decompressed_filepath = filepath.Replace(".txt", "_decompressed.txt");
            using (StreamWriter decompressed_file = new StreamWriter(decompressed_filepath))
            {
                decompressed_file.Write(decompressedLZ77);
            }
            SelectedFileTextBox.Text = "Раскодировано";
        }
    }
}
