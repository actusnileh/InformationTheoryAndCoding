using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

        void Button_Click(object sender, RoutedEventArgs e)
        {
            string compressed = LZ77.Compress(9, 7, "@@@@!@@$!@%f423f2!");

            Huffman huffman = new Huffman();
            string compressedHuffman = huffman.Compress(compressed);

            string decompressedString = huffman.Decompress(compressedHuffman);

            var result = LZ77.Decompress(decompressedString);
        }

        void ButtonChooseFile(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".txt"
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
    }
}
