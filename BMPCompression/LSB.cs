using System.IO;

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
                    byte imgByte = (byte)inputImage.ReadByte(); // Читаем байт из нашего изображения
                    if (imgByte == -1) break; // Проверяем что мы не в конце
                    imgByte &= secretMask; // Применяем маску к нашему байту (изображению)
                    byte bits = (byte)(symbolCode & secretMask); // Достаем бит из символа

                    bits >>= 8 - degree; // Сдвигаем биты нашего секрета в нужную позицию
                    imgByte |= bits; // Внедряем бит секрета в байт контейнера

                    outputImage.WriteByte(imgByte); // Записываем байт в контейнер с секретом
                    symbolCode <<= degree; // Сдвигаемся на следующую секретную позицию
                }
            }
            inputImage.CopyTo(outputImage); // Копируем оставшуюся часть

            return true; // Если все без проблем, возвращаем true
        }

        public static (byte, byte) GenerateMasks(int degree)
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
