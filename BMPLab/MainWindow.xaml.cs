using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace BMPLab
{
    public partial class MainWindow : Window
    {

        string? filepath;
        string? filename;
        int width;
        int height;
        int bit_pixel;
        int pixelDataOffset;

        public MainWindow()
        {
            InitializeComponent();
        }

        void ChooseFileButton_Click(object sender, RoutedEventArgs e)
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
                filename = dialog.SafeFileName;
                ChooseFileButton.Content = $"{dialog.SafeFileName}";
                СolorСomponentsButton.IsEnabled = true;
                BitComponentsButton.IsEnabled = true;
                ReadBMPHeader(filepath);
            }
            else
            {
                СolorСomponentsButton.IsEnabled = false;
                BitComponentsButton.IsEnabled = false;
            }
        }

        void ReadBMPHeader(string filepath)
        {
            using FileStream bmpFile = new(filepath, FileMode.Open, FileAccess.Read);

            byte[] header = new byte[54];

            bmpFile.Read(header, 0, header.Length);

            // Сигнатура файла
            string signature = Encoding.ASCII.GetString(header, 0, 2);

            // Размер файла
            int file_size = BitConverter.ToInt32(header, 2);

            // Где начинается сама картинка
            pixelDataOffset = BitConverter.ToInt32(header, 10);

            // Ширина изображения
            width = BitConverter.ToInt32(header, 18);

            // Высота изображения
            height = BitConverter.ToInt32(header, 22);

            // Число плоскостей
            int planes_number = BitConverter.ToInt32(header, 26);

            // Бит/пиксель
            bit_pixel = BitConverter.ToInt32(header, 28);

            // Тип сжатия
            int compressionType = BitConverter.ToInt32(header, 30);

            // Размер сжатого изображения
            int image_encoding_size = BitConverter.ToInt32(header, 34);

            // Горизонтальное разрешение
            int horizontal_size = BitConverter.ToInt32(header, 38);

            // Вертикальное разрешение
            int vertical_size = BitConverter.ToInt32(header, 42);

            // Количество используемых цветов
            int count_used_colors = BitConverter.ToInt32(header, 46);

            // Колличество важных цветов
            int count_important_colors = BitConverter.ToInt32(header, 50);


            TreeViewItem headerNode = (TreeViewItem)TreeViewHeader.ItemContainerGenerator.ContainerFromIndex(0);

            Run codeRun = (Run)headerNode.FindName("Code_Header");
            Run fileSizeRun = (Run)headerNode.FindName("FileSize_Header");
            Run widthRun = (Run)headerNode.FindName("Width_Header");
            Run heightRun = (Run)headerNode.FindName("Height_Header");
            Run pixelOffsetRun = (Run)headerNode.FindName("ImageStart_Header");
            Run pixelCount = (Run)headerNode.FindName("PixelCount_Header");
            Run bitPixel = (Run)headerNode.FindName("BitPixel_Header");
            Run encodingType = (Run)headerNode.FindName("EncodingType_Header");
            Run encodingFileSize = (Run)headerNode.FindName("EncodingFileSize_Header");
            Run horizontal = (Run)headerNode.FindName("Horizontal_Header");
            Run vertical = (Run)headerNode.FindName("Vertical_Header");
            Run countUsed = (Run)headerNode.FindName("CountUsedColors_Header");
            Run countImportant = (Run)headerNode.FindName("CountImportantColors_Header");

            codeRun.Text = signature;
            fileSizeRun.Text = file_size.ToString();
            widthRun.Text = width.ToString();
            heightRun.Text = height.ToString();
            pixelOffsetRun.Text = pixelDataOffset.ToString();
            pixelCount.Text = planes_number.ToString();
            bitPixel.Text = bit_pixel.ToString();
            encodingType.Text = compressionType.ToString();
            encodingFileSize.Text = image_encoding_size.ToString();
            horizontal.Text = horizontal_size.ToString();
            vertical.Text = vertical_size.ToString();
            countUsed.Text = count_used_colors.ToString();
            countImportant.Text = count_important_colors.ToString();
        }

        void СolorСomponentsButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] bmpBytes = File.ReadAllBytes(filepath);

            int bytesPerPixel = bit_pixel / 8; // Сколько байт занимает 1 пиксель

            byte[] redChannel = new byte[width * height];
            byte[] greenChannel = new byte[width * height];
            byte[] blueChannel = new byte[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int pixelOffset = pixelDataOffset + (y * width + x) * bytesPerPixel; // Считаем смещение от начала
                    blueChannel[y * width + x] = bmpBytes[pixelOffset];
                    greenChannel[y * width + x] = bmpBytes[pixelOffset + 1];
                    redChannel[y * width + x] = bmpBytes[pixelOffset + 2];
                }
            }

            SaveColorChannelToFile("_red_channel.bmp", redChannel, width, height, bit_pixel, filepath);
            SaveColorChannelToFile("_green_channel.bmp", greenChannel, width, height, bit_pixel, filepath);
            SaveColorChannelToFile("_blue_channel.bmp", blueChannel, width, height, bit_pixel, filepath);

            ColorComponentsReady.MessageQueue?.Enqueue(
                "Файл BMP был разделён на отдельные каналы",
                null,
                null,
                null,
                false,
                true,
                TimeSpan.FromSeconds(3)
            );
        }

        static void SaveColorChannelToFile(string fileName, byte[] channel, int width, int height, int bitsPerPixel, string filepath)
        {
            int bytesPerPixel = bitsPerPixel / 8;
            byte[] fileBytes = new byte[channel.Length * bytesPerPixel + 54];

            Array.Copy(File.ReadAllBytes(filepath), fileBytes, 54);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int pixelOffset = 54 + (y * width + x) * bytesPerPixel;
                    if (fileName.Contains("red"))
                        fileBytes[pixelOffset + 2] = channel[y * width + x];
                    else if (fileName.Contains("green"))
                        fileBytes[pixelOffset + 1] = channel[y * width + x];
                    else if (fileName.Contains("blue"))
                        fileBytes[pixelOffset] = channel[y * width + x];

                }
            }
            File.WriteAllBytes(filepath.Replace(".bmp", fileName), fileBytes);
        }

        void BitComponentsButton_Click(object sender, RoutedEventArgs e)
        {
            string directory = Path.GetDirectoryName(filepath);
            string fileName = Path.GetFileNameWithoutExtension(filepath);
            string outputDirectory = Path.Combine(directory, fileName + "_slices");
            Directory.CreateDirectory(outputDirectory);

            byte[] header;
            byte[] pixelData;

            using (FileStream imgFile = new(filepath, FileMode.Open, FileAccess.Read))
            {
                header = new byte[54];
                imgFile.Read(header, 0, 54);
                pixelData = new byte[imgFile.Length - 54];
                imgFile.Read(pixelData, 0, pixelData.Length);
            }

            for (int i = 0; i < pixelData.Length - 2; i += 3)
            {
                byte gray = (byte)(0.2126 * pixelData[i + 2] + 0.7152 * pixelData[i + 1] + 0.0722 * pixelData[i]);
                pixelData[i] = gray;      // Компонент B
                pixelData[i + 1] = gray;  // Компонент G
                pixelData[i + 2] = gray;  // Компонент R
            }

            using FileStream grayimage = new(Path.Combine(filepath.Replace(".bmp", "_slices"), $"GRAY.bmp"), FileMode.Create, FileAccess.Write);
            grayimage.Write(header, 0, header.Length);
            grayimage.Write(pixelData, 0, pixelData.Length);

            for (int bitPosition = 0; bitPosition < 8; bitPosition++)
            {
                byte[] slicedPixelData = new byte[pixelData.Length];
                Array.Copy(pixelData, slicedPixelData, pixelData.Length);

                for (int i = 0; i < slicedPixelData.Length; i++) // Пройдемся нашей битовой маской по каждому пикселю
                {
                    int bitValue = (slicedPixelData[i] >> bitPosition) & 1;
                    slicedPixelData[i] = (byte)(bitValue * 255); // Увеличиваем яркость (Каждый 0 бит будет 0, а 1 будет 255)
                }

                string slicedImagePath = Path.Combine(outputDirectory, $"bit_slice_gray_{bitPosition}.bmp");
                using FileStream slicedImage = new(slicedImagePath, FileMode.Create, FileAccess.Write);
                slicedImage.Write(header, 0, header.Length);
                slicedImage.Write(slicedPixelData, 0, slicedPixelData.Length);
            }
            ColorComponentsReady.MessageQueue?.Enqueue(
                "Файл BMP был разделён на срезы",
                null,
                null,
                null,
                false,
                true,
                TimeSpan.FromSeconds(3)
            );
        }

        void GoToDirectory_Click(object sender, RoutedEventArgs e)
        {
            if (filename != null)
            {
                Process.Start("explorer.exe", filepath.Replace(filename, ""));
            }
            else
            {
                ColorComponentsReady.MessageQueue?.Enqueue(
                    "Выберите файл!",
                    null,
                    null,
                    null,
                    false,
                    true,
                    TimeSpan.FromSeconds(3)
                );
            }
        }
    }
}