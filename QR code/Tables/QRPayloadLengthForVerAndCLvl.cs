using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR_code.Tables
{
    static class QRPayloadLengthForVerAndCLvl
    {
        private static Dictionary<QRCodeServiceInformation.CorrectionLvl, int[]> table = new Dictionary<QRCodeServiceInformation.CorrectionLvl, int[]>()
        {
            { QRCodeServiceInformation.CorrectionLvl.L, new int[] { 152, 272, 440, 640, 864, 1088, 1248, 1552, 1856, 2192, 2592, 2960, 3424, 3688, 4184, 4712, 5176, 5768, 6360, 6888, 7456, 8048, 8752, 9392, 10208, 10960, 11744, 12248, 13048, 13880, 14744, 15640, 16568, 17528, 18448, 19472, 20528, 21616, 22496, 23648 } },
            { QRCodeServiceInformation.CorrectionLvl.M, new int[] { 128, 224, 352, 512, 688, 864, 992, 1232, 1456, 1728, 2032, 2320, 2672, 2920, 3320, 3624, 4056, 4504, 5016, 5352, 5712, 6256, 6880, 7312, 8000, 8496, 9024, 9544, 10136, 10984, 11640, 12328, 13048, 13800, 14496, 15312, 15936, 16816, 17728, 18672 } },
            { QRCodeServiceInformation.CorrectionLvl.Q, new int[] { 104, 176, 272, 384, 496, 608, 704, 880, 1056, 1232, 1440, 1648, 1952, 2088, 2360, 2600, 2936, 3176, 3560, 3880, 4096, 4544, 4912, 5312, 5744, 6032, 6464, 6968, 7288, 7880, 8264, 8920, 9368, 9848, 10288, 10832, 11408, 12016, 12656, 13328 } },
            { QRCodeServiceInformation.CorrectionLvl.H, new int[] { 72, 128, 208, 288, 368, 480, 528, 688, 800, 976, 1120, 1264, 1440, 1576, 1784, 2024, 2264, 2504, 2728, 3080, 3248, 3536, 3712, 4112, 4304, 4768, 5024, 5288, 5608, 5960, 6344, 6760, 7208, 7688, 7888, 8432, 8768, 9136, 9776, 10208 } },
        };

        public static QRCodeServiceInformation GetServiceInformationForDataLength(int PayloadLength, QRCodeServiceInformation.CorrectionLvl correctionLvl)
        {
            for (int version = 0; version < 40; version++)
                if (table[correctionLvl][version] >= PayloadLength + 4 + ((version + 1) <= 9 ? 8 : 16))//4 - способ кодирования, 8 или 16 - длина количества информации, 8(если версия <10)
                    return new QRCodeServiceInformation() { Lvl = correctionLvl, Version = version + 1 };
            throw new Exception();
        }

        public static int BitsAmount(int version, QRCodeServiceInformation.CorrectionLvl lvl) => table[lvl][version-1];
    }
}
