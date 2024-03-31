using System;
using System.Collections.Generic;

namespace deflate_lab
{
    public class LZ77
    {
        public static List<Tuple<int, int, char>> Compress(int dict, int buffer, string input)
        {
            List<Tuple<int, int, char>> compressed = new List<Tuple<int, int, char>>();

            int currentIndex = 0;

            while (currentIndex < input.Length)
            {
                int maxMatchLength = 0;
                int maxMatchIndex = 0;

                for (int i = Math.Max(0, currentIndex - dict); i < currentIndex; i++)
                {
                    int matchLength = 0;

                    while ((matchLength < buffer) &&
                        ((currentIndex + matchLength) < input.Length) &&
                        input[i + matchLength] == input[currentIndex + matchLength])
                    {
                        matchLength++;
                    }

                    if (matchLength > maxMatchLength)
                    {
                        maxMatchLength = matchLength;
                        maxMatchIndex = i;
                    }
                }
                if (maxMatchLength == 0)
                {
                    compressed.Add(new Tuple<int, int, char>(0, 0, input[currentIndex]));
                    currentIndex++;
                }
                else if (maxMatchLength > 0)
                {
                    if (currentIndex + 1 < input.Length)
                    {
                        compressed.Add(new Tuple<int, int, char>(
                            currentIndex - maxMatchIndex, maxMatchLength, input[currentIndex + maxMatchLength]));
                        currentIndex += maxMatchLength + 1;
                    }
                    else
                    {
                        compressed.Add(new Tuple<int, int, char>(0, 0, input[currentIndex]));
                        break;
                    }
                }
            }
            return compressed;
        }

        public static string Decompress(List<Tuple<int, int, char>> compressed)
        {
            string decompressed = "";

            foreach (var tuple in compressed)
            {
                if (tuple.Item1 == 0 && tuple.Item2 == 0)
                {
                    decompressed += tuple.Item3;
                }
                else
                {
                    int startIndex = decompressed.Length - tuple.Item1;
                    int endIndex = startIndex + tuple.Item2;

                    for (int i = startIndex; i < endIndex; i++)
                    {
                        decompressed += decompressed[i];
                    }

                    decompressed += tuple.Item3;
                }
            }
            return decompressed;
        }
    }
}
