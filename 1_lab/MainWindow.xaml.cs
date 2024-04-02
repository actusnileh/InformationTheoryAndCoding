using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;

namespace deflate_lab
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
    }
}
