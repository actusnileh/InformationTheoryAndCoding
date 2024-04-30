using System.IO;
using System.Text;

namespace BMPCompression
{
    public class LSB
    {
        const int BMP_HEADER_SIZE = 54;

        public static bool Encode(string input_path, string input_secret, int degree)
        {
            // Проверка, что наша степень равна 1 или 2 и т.д
            if (!(degree == 1 || degree == 2 || degree == 4 || degree == 8))
            {
                return false;
            }
            // Открываем для чтения наше изображение (контейнер)
            FileStream inputImage = File.Open(input_path, FileMode.Open, FileAccess.Read);
            // Открываем для записи изображение с секретом
            FileStream outputImage = File.Open(input_path.Replace(".bmp", "_encoded.bmp"), FileMode.Create, FileAccess.Write);

            byte[] header = new byte[BMP_HEADER_SIZE]; // Создаем массив байт, который будет содержать заголовок
            inputImage.Read(header, 0, BMP_HEADER_SIZE); // Читаем его из нашего контейнера
            outputImage.Write(header, 0, BMP_HEADER_SIZE); // Записываем в контейнер с секретом

            (byte secretMask, byte imgMask) = GenerateMasks(degree); // Создаем маски незначащих битов (где будем хранить наш секрет)

            foreach (var symbol in input_secret) // Проходимся по каждому символу в секрете
            {
                var symbolCode = Convert.ToInt32(symbol); // Преобразуем символ в число (ASCII)

                for (int i = 0; i < 8; i += degree) // Внедряем наш секрет
                {
                    byte imgByte = (byte)inputImage.ReadByte();
                    if (imgByte == -1) break;
                    imgByte &= imgMask;
                    byte bits = (byte)(symbolCode & secretMask);
                    bits >>= 8 - degree;
                    imgByte |= bits;
                    outputImage.WriteByte(imgByte);
                    symbolCode <<= degree;
                }
            }
            inputImage.CopyTo(outputImage);
            inputImage.Close();
            outputImage.Close();
            return true; // Если все без проблем, возвращаем true
        }

        public static string? Decode(string encode_path, int symbols_count, int degree)
        {
            if (!(degree == 1 || degree == 2 || degree == 4 || degree == 8))
            {
                return null;
            }

            FileStream encodeImage = File.Open(encode_path, FileMode.Open, FileAccess.Read);
            encodeImage.Seek(BMP_HEADER_SIZE, SeekOrigin.Begin);
            (byte _, byte imgMask) = GenerateMasks(degree);
            imgMask = (byte)~imgMask;

            StringBuilder textBuilder = new();
            int read = 0;

            while (read < symbols_count)
            {
                int symbol = 0;

                for (int i = 0; i < 8; i += degree)
                {
                    byte imgByte = (byte)encodeImage.ReadByte();
                    if (imgByte == -1) break;
                    imgByte &= imgMask;
                    symbol <<= degree;
                    symbol |= imgByte;
                }

                if (Convert.ToChar(symbol) == '\n' && Environment.NewLine.Length == 2)
                {
                    read++;
                }

                read++;
                textBuilder.Append(Convert.ToChar(symbol)); // Добавляем символ в StringBuilder
            }

            encodeImage.Close();
            return textBuilder.ToString(); // Возвращаем результат в виде строки
        }

        private static (byte, byte) GenerateMasks(int degree)
        {
            // Задаем маски для секрета и изображения
            byte secretMask = 0b11111111; // все биты 1
            byte imgMask = 0b11111111;

            // Сдвигаем маску для секрета на количество degree
            secretMask <<= 8 - degree; // сдвигаем маску на 8 минус степень
            // Выполняем операцию модуля 256, чтобы оставить только 8 младших бит в маске для секрета
            secretMask = (byte)(secretMask % 256); // применяем операцию модуля к результату сдвига

            imgMask >>= degree; // сдвигаем маску вправо на degree
            imgMask <<= degree; // сдвигаем маску обратно влево degree

            return (secretMask, imgMask);
        }
    }
}
