﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR_code.Tables
{
    static class QRBlocksAmountForVerAndCLvl
    {
        private static Dictionary<QRCodeServiceInformation.CorrectionLvl, int[]> table = new Dictionary<QRCodeServiceInformation.CorrectionLvl, int[]>()
        {
            { QRCodeServiceInformation.CorrectionLvl.L, new int[] { 1, 1, 1, 1, 1, 2, 2, 2, 2, 4, 4, 4, 4, 4, 6, 6, 6, 6, 7, 8, 8, 9, 9, 10, 12, 12, 12, 13, 14, 15, 16, 17, 18, 19, 19, 20, 21, 22, 24, 25 } },
            { QRCodeServiceInformation.CorrectionLvl.M, new int[] { 1, 1, 1, 2, 2, 4, 4, 4, 5, 5, 5, 8, 9, 9, 10, 10, 11, 13, 14, 16, 17, 17, 18, 20, 21, 23, 25, 26, 28, 29, 31, 33, 35, 37, 38, 40, 43, 45, 47, 49 } },
            { QRCodeServiceInformation.CorrectionLvl.Q, new int[] { 1, 1, 2, 2, 4, 4, 6, 6, 8, 8, 8, 10, 12, 16, 12, 17, 16, 18, 21, 20, 23, 23, 25, 27, 29, 34, 34, 35, 38, 40, 43, 45, 48, 51, 53, 56, 59, 62, 65, 68 } },
            { QRCodeServiceInformation.CorrectionLvl.H, new int[] { 1, 1, 2, 4, 4, 4, 5, 6, 8, 8, 11, 11, 16, 16, 18, 16, 19, 21, 25, 25, 25, 34, 30, 32, 35, 37, 40, 42, 45, 48, 51, 54, 57, 60, 63, 66, 70, 74, 77, 81 } },
        };

        public static int BlocksAmount(int version, QRCodeServiceInformation.CorrectionLvl lvl) => table[lvl][version-1];
    }
}
