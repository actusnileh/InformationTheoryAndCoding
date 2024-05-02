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
                    byte imgByte = (byte)inputImage.ReadByte(); // Читаем байт изображения
                    if (imgByte == -1) break; // Проверяем что не конец
                    imgByte &= imgMask; // Применяем маску изображения к байту
                    byte bits = (byte)(symbolCode & secretMask); // Получаем биты секрета
                    bits >>= 8 - degree; // Сдвигаем биты
                    imgByte |= bits; // Применяем к изображению
                    outputImage.WriteByte(imgByte); // Записываем измененный байт с секретом
                    symbolCode <<= degree; // Сдвигаем код символа
                }
            }
            inputImage.CopyTo(outputImage); // Записываем остальную часть
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

            FileStream encodeImage = File.Open(encode_path, FileMode.Open, FileAccess.Read); // Открываем закодированный файл
            encodeImage.Seek(BMP_HEADER_SIZE, SeekOrigin.Begin); // Пропускаем заголовок
            (byte _, byte imgMask) = GenerateMasks(degree); // Генерируем маску изображения для извлечения символов
            imgMask = (byte)~imgMask; // Инвертируем маску

            StringBuilder textBuilder = new(); // Тут будем хранить текст
            int read = 0; // Счетник прочитанных символов

            while (read < symbols_count)
            {
                int symbol = 0;

                for (int i = 0; i < 8; i += degree) // Достаем наши байты из картинки
                {
                    byte imgByte = (byte)encodeImage.ReadByte(); // Читаем байт
                    if (imgByte == -1) break; // Проверяем на конец
                    imgByte &= imgMask; // Применяем маску
                    symbol <<= degree; // Сдвигаем символ
                    symbol |= imgByte; // Добавляем новые биты
                }

                if (Convert.ToChar(symbol) == '\n' && Environment.NewLine.Length == 2) // Учитываем символ новой строки (на всякий случай)
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
