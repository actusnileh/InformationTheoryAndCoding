using System.Text;

namespace Hamming
{
    public static class HammingAlgorithm
    {
        private static int CalculateControlBits(int dataLength)
        {
            int controlBits = 0;
            while ((1 << controlBits) < dataLength + controlBits + 1)
                controlBits++;

            return controlBits;
        }

        public static string Encode(string data)
        {
            int dataLength = data.Length;
            int controlBits = CalculateControlBits(dataLength);

            StringBuilder encodedData = new(dataLength + controlBits);
            int dataIndex = 0;

            for (int i = 0; i < dataLength + controlBits; i++)
            {
                if ((i & (i + 1)) == 0)
                    encodedData.Append('0');
                else
                    encodedData.Append(data[dataIndex++]);
            }

            for (int i = 0; i < controlBits; i++)
            {
                int controlBitIndex = (1 << i) - 1;
                int parity = 0;

                for (int j = controlBitIndex; j < dataLength + controlBits; j += (2 * controlBitIndex + 2))
                {
                    for (int k = 0; k <= controlBitIndex && j + k < dataLength + controlBits; k++)
                        parity ^= (encodedData[j + k] - '0');
                }

                encodedData[controlBitIndex] = (char)(parity + '0');
            }

            return encodedData.ToString();
        }

        public static (string decodedData, int errorIndex) Decode(string encodedData)
        {
            int controlBits = CalculateControlBits(encodedData.Length);
            int dataLength = encodedData.Length - controlBits;
            StringBuilder decodedData = new(dataLength);

            int errorBitIndex = 0;

            for (int i = 0; i < controlBits; i++)
            {
                int controlBitIndex = (1 << i) - 1;
                int parity = 0;

                for (int j = controlBitIndex; j < encodedData.Length; j += (2 * controlBitIndex + 2))
                {
                    for (int k = 0; k <= controlBitIndex && j + k < encodedData.Length; k++)
                        parity ^= (encodedData[j + k] - '0');
                }

                if (parity != 0)
                    errorBitIndex += controlBitIndex + 1;
            }

            if (errorBitIndex != 0)
                encodedData = encodedData.Remove(errorBitIndex - 1, 1).Insert(errorBitIndex - 1, (encodedData[errorBitIndex - 1] == '0' ? "1" : "0"));

            for (int i = 0; i < dataLength; i++)
                decodedData.Append(encodedData[i + CalculateControlBits(i + 1)]);

            return (decodedData.ToString(), errorBitIndex);
        }
    }
}
