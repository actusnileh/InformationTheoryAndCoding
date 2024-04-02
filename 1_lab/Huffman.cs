using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Windows.Controls;

namespace deflate_lab
{
    public class Huffman
    {
        public class HuffmanNode
        {
            public char Symbol { get; set; }
            public int Frequency { get; set; }
            public HuffmanNode Left { get; set; }
            public HuffmanNode Right { get; set; }
        }

        private Dictionary<char, string> encodingTable;
        private HuffmanNode root;

        public string Compress(string input)
        {
            Dictionary<char, int> frequencies = GenerateFrequencies(input);
            var nodes = frequencies.Select(pair => new HuffmanNode { Symbol = pair.Key, Frequency = pair.Value }).ToList();
            root = BuildHuffmanTree(nodes);
            encodingTable = new Dictionary<char, string>();
            GenerateCodes(root, "");

            string encodedMessage = "";
            foreach (char c in input)
            {
                encodedMessage += encodingTable[c];
            }
            return encodedMessage;
        }

        private Dictionary<char, int> GenerateFrequencies(string input)
        {
            Dictionary<char, int> frequencies = new Dictionary<char, int>();
            for (int i = 0; i < input.Length; i++)
            {
                if (!frequencies.ContainsKey(input[i]))
                    frequencies.Add(input[i], 1);
                else
                    frequencies[input[i]]++;

            }
            frequencies = frequencies.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            return frequencies;
        }
        private HuffmanNode BuildHuffmanTree(List<HuffmanNode> nodes)
        {
            while (nodes.Count > 1)
            {
                nodes.Sort((x, y) => x.Frequency.CompareTo(y.Frequency));
                HuffmanNode left = nodes[0];
                HuffmanNode right = nodes[1];

                HuffmanNode parent = new HuffmanNode
                {
                    Symbol = '\0',
                    Frequency = left.Frequency + right.Frequency,
                    Left = left,
                    Right = right
                };

                nodes.Remove(left);
                nodes.Remove(right);
                nodes.Add(parent);
            }

            return nodes.Single();
        }

        private void GenerateCodes(HuffmanNode node, string code)
        {
            if (node.Left == null && node.Right == null)
            {
                encodingTable[node.Symbol] = code;
            }
            else
            {
                GenerateCodes(node.Left, code + "0");
                GenerateCodes(node.Right, code + "1");
            }
        }

        // ДЕКОДИРОВАНИЕ

        public string Decompress(string encodedMessage)
        {
            if (string.IsNullOrEmpty(encodedMessage))
            {
                throw new InvalidOperationException("Таблички нет!");
            }

            string decodedMessage = "";

            HuffmanNode currentNode = root;

            foreach (char bit in encodedMessage)
            {
                if (bit == '0')
                    currentNode = currentNode.Left;
                else if (bit == '1')
                    currentNode = currentNode.Right;
                if (currentNode.Left == null && currentNode.Right == null)
                {
                    decodedMessage += currentNode.Symbol;
                    currentNode = root;
                }
            }
            return decodedMessage;
        }
    }
}