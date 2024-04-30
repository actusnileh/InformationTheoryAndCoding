using System.Text;

namespace BMPCompression
{
    public class LZ77
    {
        public static string Compress(int dict, int buffer, string input)
        {
            input = input.Replace(" ", "\u2400")
                .Replace("\t", "\u2401")
                .Replace("\n", "\u2402")
                .Replace(":", "\u2403")
                .Replace(",", "\u2404");

            List<Tuple<int, int, char>> compressed = new List<Tuple<int, int, char>>();

            int currentIndex = 0;

            // Начинаем идти по всему тексту
            while (currentIndex < input.Length)
            {
                int maxMatchLength = 0;
                int maxMatchIndex = 0;

                // Ищем самое большое совпадение
                for (int i = Math.Max(0, currentIndex - dict); i < currentIndex; i++)
                {
                    int matchLength = 0;

                    // Находим (увеличиваем) самое большое совпадение
                    while ((matchLength < buffer) &&
                        ((currentIndex + matchLength) < input.Length) &&
                        input[i + matchLength] == input[currentIndex + matchLength])
                    {
                        matchLength++;
                    }

                    // Если наше совпадение больше чем прошлое, то меняем значения
                    if (matchLength > maxMatchLength)
                    {
                        maxMatchLength = matchLength;
                        maxMatchIndex = i;
                    }
                }
                // Если мы не нашли не одного совпадения, то добавляем новый символ
                if (maxMatchLength == 0)
                {
                    compressed.Add(new Tuple<int, int, char>(0, 0, input[currentIndex]));
                    currentIndex++;
                }
                // Если есть совпадение, то добавляем как обычно
                else if (maxMatchLength > 0)
                {
                    if (currentIndex + maxMatchLength < input.Length)
                    {
                        compressed.Add(new Tuple<int, int, char>(
                            currentIndex - maxMatchIndex, maxMatchLength, input[currentIndex + maxMatchLength]));
                        currentIndex += maxMatchLength + 1;
                    }
                    else
                    {
                        // Если наше совпадение это конец строки
                        compressed.Add(new Tuple<int, int, char>(currentIndex - maxMatchIndex, maxMatchLength, '\0'));
                        break;
                    }
                }
            }
            string result = string.Join(",", compressed.Select(t => $"{t.Item1}:{t.Item2}:{t.Item3}"));
            return result;
        }

        public static string Decompress(string input)
        {
            string[] parts = input.Split(',');

            var compressed = new List<(int start, int length, char symbol)>();
            foreach (string part in parts)
            {
                string[] tupleParts = part.Split(':');
                int start = int.Parse(tupleParts[0]);
                int length = int.Parse(tupleParts[1]);
                char symbol = tupleParts[2][0];
                compressed.Add((start, length, symbol));
            }

            var decompressedBuilder = new StringBuilder();
            int decompressedLength = 0;

            foreach (var tuple in compressed)
            {
                if (tuple.start == 0 && tuple.length == 0)
                {
                    decompressedBuilder.Append(tuple.symbol);
                    decompressedLength++;
                }
                else
                {
                    int startIndex = decompressedLength - tuple.start;
                    int endIndex = startIndex + tuple.length;

                    for (int i = 0; i < tuple.length; i++)
                    {
                        decompressedBuilder.Append(decompressedBuilder[startIndex + i]);
                        decompressedLength++;
                    }

                    decompressedBuilder.Append(tuple.symbol);
                    decompressedLength++;
                }
            }

            string decompressed = decompressedBuilder.ToString();

            // Замена специальных символов
            decompressed = decompressed
                .Replace("\u2400", " ")
                .Replace("\u2401", "\t")
                .Replace("\u2402", "\n")
                .Replace("\u2403", ":")
                .Replace("\u2404", ",");

            return decompressed;
        }
    }
}
