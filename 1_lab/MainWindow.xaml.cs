using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

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
            List<Tuple<int, int, char>> compressed = LZ77.Compress(9, 7, "ДЕНИС_ГИРИЧЕВ_ЧМО!");

        }
    }
}
