using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace deflate_lab
{
    public class Huffman
    {
        public class HuffmanNode
        {
            public char Symbol { get; set; } // Символ
            public int Frequency { get; set; } // Частота
            public HuffmanNode Left { get; set; } // Левый потомок
            public HuffmanNode Right { get; set; } // Правый потомок
        }

        private Dictionary<char, string> encodingTable;
        private HuffmanNode root;

        public byte[] Compress(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Пустой файл подали ;(.");

            Dictionary<char, int> frequencies = GenerateFrequencies(input);

            // Создаем узлы хаффмана основываясь на частотах
            var nodes = frequencies.Select(pair => new HuffmanNode { Symbol = pair.Key, Frequency = pair.Value }).ToList();

            // Строим само дерево Хаффмана
            root = BuildHuffmanTree(nodes);

            encodingTable = new Dictionary<char, string>();

            // Генерация кодов символов
            GenerateCodes(root, "");

            List<byte> encodedBytes = new List<byte>();
            StringBuilder encodedBits = new StringBuilder();

            // Кодируем данные сохраняя последовательность
            foreach (char c in input)
            {
                // По кусочкам добавляем символы
                encodedBits.Append(encodingTable[c]);

                // Как только набираем 8 битов строим байт
                while (encodedBits.Length >= 8)
                {
                    string byteString = encodedBits.ToString(0, 8); // извлекаем 8 битов
                    encodedBits.Remove(0, 8); // Удаляем из строки байты которые забрали
                    encodedBytes.Add(Convert.ToByte(byteString, 2)); // Преобразуем из битов в байты
                }
            }

            // Добавляем оставшиеся биты в
            if (encodedBits.Length > 0)
            {
                // Если у нас меньше 8 битов, то
                while (encodedBits.Length < 8)
                {
                    encodedBits.Append('0'); // добавляем байты пока не полусим целый бит
                }
                encodedBytes.Add(Convert.ToByte(encodedBits.ToString(), 2));
            }

            return encodedBytes.ToArray();
        }

        // Генерация таблицы частот символов
        private Dictionary<char, int> GenerateFrequencies(string input)
        {
            if (string.IsNullOrEmpty(input))
                return new Dictionary<char, int>();
            // Генерируем таблицу частот в формате <буква><колличество встреч>
            return input.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
        }

        // Из узлов строим дерево
        private HuffmanNode BuildHuffmanTree(List<HuffmanNode> nodes)
        {
            // Пока есть хотя бы 1 узел
            while (nodes.Count > 1)
            {
                // Отсортируем по частоте
                nodes.Sort((x, y) => x.Frequency.CompareTo(y.Frequency));
                HuffmanNode left = nodes[0]; // Первые два с наименьшей частотой
                HuffmanNode right = nodes[1]; // второй ;)

                // Получаем родительский узел, объединяя два узла
                HuffmanNode parent = new HuffmanNode
                {
                    Symbol = '\0', // Родительский узел без символа
                    Frequency = left.Frequency + right.Frequency,
                    Left = left,
                    Right = right
                };

                nodes.Remove(left); // Удаляем первый использованный узел
                nodes.Remove(right); // Второй ;))))
                nodes.Add(parent); // Добавляем родителя
            }

            // Узел который остался последний наш корневой
            return nodes.Single();
        }

        private void GenerateCodes(HuffmanNode node, string code)
        {
            if (node.Left == null && node.Right == null)
            {
                // Если достигнут корень добавляем его символ и код в таблицу
                encodingTable[node.Symbol] = code;
            }
            else
            {
                GenerateCodes(node.Left, code + "0"); // Рекурсивно обходим через право и лево
                GenerateCodes(node.Right, code + "1"); // Право ;))))))))
            }
        }

        public string Decompress(byte[] encodedBytes)
        {
            if (encodedBytes == null || encodedBytes.Length == 0)
                throw new ArgumentException("Пустой файл подан.");

            StringBuilder decodedBits = new StringBuilder();

            // Декодируем байты обратно в биты
            foreach (byte b in encodedBytes)
            {
                string bits = Convert.ToString(b, 2).PadLeft(8, '0');
                decodedBits.Append(bits);
            }

            // Убираем нули в конце если есть
            int paddingCount = decodedBits[decodedBits.Length - 1] - '0';
            decodedBits.Remove(decodedBits.Length - paddingCount, paddingCount);

            string encodedMessage = decodedBits.ToString(); // Переводим биты в строчку
            string decodedMessage = "";

            HuffmanNode currentNode = root; // Получаем наше дерево что бы раскодировать

            // Двигаемся по дереву
            foreach (char bit in encodedMessage)
            {
                if (bit == '0') // Если 0 переходим к левому потомку
                    currentNode = currentNode.Left;
                else if (bit == '1') // Наоборот
                    currentNode = currentNode.Right;

                // Если получили узел, то добавляем его символ к раскодированному сообщению
                if (currentNode.Left == null && currentNode.Right == null)
                {
                    decodedMessage += currentNode.Symbol;
                    currentNode = root; // возвращаесся к корню
                }
            }
            return decodedMessage;
        }
    }
}
