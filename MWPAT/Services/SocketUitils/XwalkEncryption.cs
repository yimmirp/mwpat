using System.Text;

namespace SiteManage.Services.SocketUitils
{
    public class XwalkEncryption
    {
        static int[]? ebcdicTable;
        static int[]? asciiTable;

        public XwalkEncryption()
        {
            string EtoA = "01 02 03 9c 09 86 7f 97 8d 8e 0b 0c 0d 0e 0f 10 11 12 13 9d 85 08 87 18 19 92 8f 1c 1d 1e 1f 80 81 82 83 84 0a 17 1b 88 89 8a 8b 8c 05 06 07 90 91 16 93 94 95 96 04 98 99 9a 9b 14 15 9e 1a 20 a0 a1 a2 a3 a4 a5 a6 a7 a8 5b 2e 3c 28 2b 21 26 a9 aa ab ac ad ae af b0 b1 5d 24 2a 29 3b 5e 2d 2f b2 b3 b4 b5 b6 b7 b8 b9 7c 2c 25 5f 3e 3f ba bb bc bd be bf c0 c1 c2 60 3a 23 40 27 3d 22 c3 61 62 63 64 65 66 67 68 69 c4 c5 c6 c7 c8 c9 ca 6a 6b 6c 6d 6e 6f 70 71 72 cb cc cd ce cf d0 d1 7e 73 74 75 76 77 78 79 7a d2 d3 d4 d5 d6 d7 d8 d9 da db dc dd de df e0 e1 e2 e3 e4 e5 e6 e7 7b 41 42 43 44 45 46 47 48 49 e8 e9 ea eb ec ed 7d 4a 4b 4c 4d 4e 4f 50 51 52 ee ef f0 f1 f2 f3 5c 9f 53 54 55 56 57 58 59 5a f4 f5 f6 f7 f8 f9 30 31 32 33 34 35 36 37 38 39 fa fb fc fd fe ff ";
            string AtoE = "01 02 03 37 2d 2e 2f 16 05 25 0b 0c 0d 0e 0f 10 11 12 13 3c 3d 32 26 18 19 3f 27 1c 1d 1e 1f 40 4f 7f 7b 5b 6c 50 7d 4d 5d 5c 4e 6b 60 4b 61 f0 f1 f2 f3 f4 f5 f6 f7 f8 f9 7a 5e 4c 7e 6e 6f 7c c1 c2 c3 c4 c5 c6 c7 c8 c9 d1 d2 d3 d4 d5 d6 d7 d8 d9 e2 e3 e4 e5 e6 e7 e8 e9 4a e0 5a 5f 6d 79 81 82 83 84 85 86 87 88 89 91 92 93 94 95 96 97 98 99 a2 a3 a4 a5 a6 a7 a8 a9 c0 6a d0 a1 07 20 21 22 23 24 15 06 17 28 29 2a 2b 2c 09 0a 1b 30 31 1a 33 34 35 36 08 38 39 3a 3b 04 14 3e e1 41 42 43 44 45 46 47 48 49 51 52 53 54 55 56 57 58 59 62 63 64 65 66 67 68 69 70 71 72 73 74 75 76 77 78 80 8a 8b 8c 8d 8e 8f 90 9a 9b 9c 9d 9e 9f a0 aa ab ac ad ae af b0 b1 b2 b3 b4 b5 b6 b7 b8 b9 ba bb bc bd be bf ca cb cc cd ce cf da db dc dd de df ea eb ec ed ee ef fa fb fc fd fe ff ";
            string hexValue;
            int j = 0;

            ebcdicTable = new int[256];
            asciiTable = new int[256];

            for (int i = 0; i < EtoA.Length; i = i + 3)
            {
                j++;
                hexValue = EtoA.Substring(i, 2).Trim();
                ebcdicTable[j] = int.Parse(EtoA.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
                asciiTable[j] = int.Parse(AtoE.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
            }
        }
        public string EBCDICtoASCII(List<byte> buffer)
        {

            StringBuilder result = new StringBuilder();

            int index = 0;

            foreach (var _byte in buffer)
            {

                if (_byte == 255 || _byte == 239)
                    continue;

                index = Convert.ToInt32(ebcdicTable[_byte]);

                if (index == 0)
                    result.Append(" ");
                else
                    result.Append(Byte2Chr(index));
            }


            return result.ToString();
        }


        private static string Byte2Chr(int value)
        {
            if (value > 255)
                throw new ArgumentOutOfRangeException(nameof(value), value, "CharCode must be between 0 and 255.");

            return Encoding.ASCII.GetString(new[] { (byte)value });
        }


        public byte[] ASCIItoEBCDIC(string value, bool addDelimiters = true)
        {


            byte[] result;

            if (addDelimiters)
                result = new byte[value.Length + 2];
            else
                result = new byte[value.Length];

            for (int i = 0; i < value.Length; i++)
                result[i] = Convert.ToByte(asciiTable[vb6Asc(value.Substring(i, 1))]);

            if (addDelimiters)
            {
                result[value.Length + 0] = 255;
                result[value.Length + 1] = 239;
            }
            return result;
        }

        private static int vb6Asc(string value)
        {
            return Encoding.ASCII.GetBytes(value)[0];


        }
    }
}
