using System.Text;

namespace Hamming
{
    public static class HammingAlgorithm
    {
        private static int CalculateControlBits(int dataLength)
        {
            int controlBits = 0;
            while ((1 << controlBits) < dataLength + controlBits + 1) // Вычисляем количество контрольных битов (Степень двойки)
                controlBits++;

            return controlBits;
        }

        public static (string, int[,]) Encode(string data)
        {
            int dataLength = data.Length; // Длина нашей строки
            int controlBits = CalculateControlBits(dataLength); // Количество котрольных бит

            StringBuilder encodedData = new StringBuilder(dataLength + controlBits); // Создаем строку
            int dataIndex = 0;

            for (int i = 0; i < dataLength + controlBits; i++) // Проходимся через каждую позицию бита
            {
                if ((i & (i + 1)) == 0) // Если наша текущая позиция - котрольный бит, то добавляем 0
                    encodedData.Append('0');
                else
                    encodedData.Append(data[dataIndex++]); // Иначе добавляем бит из нашей строки
            }

            int[,] parityMatrix = new int[controlBits, dataLength + controlBits];

            for (int i = 0; i < controlBits; i++) // Вычисляем сами контрольные биты и заполняем матрицу контрольного суммирования
            {
                int controlBitIndex = (1 << i) - 1; // Индекс контрольного бита
                int parity = 0;

                for (int j = controlBitIndex; j < dataLength + controlBits; j += (2 * controlBitIndex + 2)) // Вычисляем четность для котрольного бита
                {
                    for (int k = 0; k <= controlBitIndex && j + k < dataLength + controlBits; k++)
                    {
                        parity ^= (encodedData[j + k] - '0');
                        parityMatrix[i, j + k] = 1; // Устанавливаем в матрице контрольного суммирования соответствующее значение
                    }
                }

                encodedData[controlBitIndex] = (char)(parity + '0'); // Устанавливаем само значение контрольного бита
            }
            return (encodedData.ToString(), parityMatrix);
        }

        public static (string, int, int[,]) Decode(string encodedData)
        {
            int controlBits = CalculateControlBits(encodedData.Length);
            int dataLength = encodedData.Length - controlBits;
            StringBuilder decodedData = new(dataLength);

            int errorBitIndex = 0;

            int[,] parityMatrix = new int[controlBits, dataLength + controlBits];

            for (int i = 0; i < controlBits; i++) // Проходимся через каждый контрольный бит
            {
                int controlBitIndex = (1 << i) - 1; // Индекс нашего бита
                int parity = 0;

                for (int j = controlBitIndex; j < encodedData.Length; j += (2 * controlBitIndex + 2)) // Вычисляем по четности контрольный бит
                {
                    for (int k = 0; k <= controlBitIndex && j + k < encodedData.Length; k++)
                    {
                        parity ^= (encodedData[j + k] - '0');
                        parityMatrix[i, j + k] = 1; // Устанавливаем в матрице контрольного суммирования соответствующее значение
                    }
                }

                if (parity != 0) // Если четность не равна 0, значит мы нашли ошибку
                    errorBitIndex += controlBitIndex + 1; // Индекс бита с ошибкой
            }

            if (errorBitIndex != 0) // Пробуем исправить. Удаляем этот символ. Если наш символ был 0, вставляем 1 иначе наоборот
                encodedData = encodedData.Remove(errorBitIndex - 1, 1).Insert(errorBitIndex - 1, (encodedData[errorBitIndex - 1] == '0' ? "1" : "0"));

            if (dataLength % 2 != 0)
                dataLength++;

            for (int i = 0; i < dataLength; i++) // Вставляем исходные данные из закодированой строки
                decodedData.Append(encodedData[i + CalculateControlBits(i + 1)]);

            return (decodedData.ToString(), errorBitIndex, parityMatrix);
        }
    }
}
