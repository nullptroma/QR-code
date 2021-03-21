using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR_code.Tables
{
    static class QRCorrectionBytesAmountForVerAndCLvl
    {
        private static Dictionary<QRCodeServiceInformation.CorrectionLvl, int[]> table = new Dictionary<QRCodeServiceInformation.CorrectionLvl, int[]>()
        {
            { QRCodeServiceInformation.CorrectionLvl.L, new int[] { 7, 10, 15, 20, 26, 18, 20, 24, 30, 18, 20, 24, 26, 30, 22, 24, 28, 30, 28, 28, 28, 28, 30, 30, 26, 28, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 } },
            { QRCodeServiceInformation.CorrectionLvl.M, new int[] { 10, 16, 26, 18, 24, 16, 18, 22, 22, 26, 30, 22, 22, 24, 24, 28, 28, 26, 26, 26, 26, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28 } },
            { QRCodeServiceInformation.CorrectionLvl.Q, new int[] { 13, 22, 18, 26, 18, 24, 18, 22, 20, 24, 28, 26, 24, 20, 30, 24, 28, 28, 26, 30, 28, 30, 30, 30, 30, 28, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 } },
            { QRCodeServiceInformation.CorrectionLvl.H, new int[] { 17, 28, 22, 16, 22, 28, 26, 26, 24, 28, 24, 28, 22, 24, 24, 30, 28, 28, 26, 28, 30, 24, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 } },
        };

        public static int BytesAmount(int version, QRCodeServiceInformation.CorrectionLvl lvl) => table[lvl][version-1];
    }
}
