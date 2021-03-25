using System;
using System.Collections.Generic;
using System.Text;

namespace QR_code.Tables
{
    static class QRMaskAndCLvlCodes
    {
        private static Dictionary<QRCodeServiceInformation.CorrectionLvl, string[]> table = new Dictionary<QRCodeServiceInformation.CorrectionLvl, string[]>()
        {
            { QRCodeServiceInformation.CorrectionLvl.L, new string[] { "111011111000100", "111001011110011", "111110110101010", "111100010011101", "110011000101111", "110001100011000", "110110001000001", "110100101110110" } },
            { QRCodeServiceInformation.CorrectionLvl.M, new string[] { "101010000010010", "101000100100101", "101111001111100", "101101101001011", "100010111111001", "100000011001110", "100111110010111", "100101010100000" } },
            { QRCodeServiceInformation.CorrectionLvl.Q, new string[] { "011010101011111", "011000001101000", "011111100110001", "011101000000110", "010010010110100", "010000110000011", "010111011011010", "010101111101101" } },
            { QRCodeServiceInformation.CorrectionLvl.H, new string[] { "001011010001001", "001001110111110", "001110011100111", "001100111010000", "000011101100010", "000001001010101", "000110100001100", "000100000111011" } },
        };

        public static bool[] Code(QRCodeServiceInformation.CorrectionLvl clvl, int mask)
        {
            bool[] code = new bool[15];
            for (int i = 0; i < code.Length; i++)
                code[i] = table[clvl][mask][i] == '1';
            return code;
        }
    }
}
