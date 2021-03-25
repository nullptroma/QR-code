using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR_code.Tables
{
    static class QRLevelingPatternsForVer
    {
        private static Dictionary<int, int[]> table = new Dictionary<int, int[]>()
        {
            { 2, new int[] { 18  } },
            { 3, new int[] { 22 } },
            { 4, new int[] { 26 } },
            { 5, new int[] { 30 } },
            { 6, new int[] { 34 } },
            { 7, new int[] { 6,22,38 } },
            { 8, new int[] { 6,24,42 } },
            { 9, new int[] { 6,26,46 } },
            { 10, new int[] { 6,28,50 } },
            { 11, new int[] { 6,30,54 } },
            { 12, new int[] { 6,32,58 } },
            { 13, new int[] { 6,34,62 } },
            { 14, new int[] { 6,26,46,66 } },
            { 15, new int[] { 6,26,48,70 } },
            { 16, new int[] { 6,26,50,74 } },
            { 17, new int[] { 6,30,54,78 } },
            { 18, new int[] { 6,30,56,82 } },
            { 19, new int[] { 6,30,58,86 } },
            { 20, new int[] { 6,34,62,90 } },
            { 21, new int[] { 6,28,50,72,94 } },
            { 22, new int[] { 6,26,50,74,98 } },
            { 23, new int[] { 6,30,54,78,102 } },
            { 24, new int[] { 6,28,54,80,106 } },
            { 25, new int[] { 6,32,58,84,110 } },
            { 26, new int[] { 6,30,58,86,114 } },
            { 27, new int[] { 6,34,62,90,118 } },
            { 28, new int[] { 6,26,50,74,98,122 } },
            { 29, new int[] { 6,30,54,78,102,126 } },
            { 30, new int[] { 6,26,52,78,104, 130 } },
            { 31, new int[] { 6,30,56,82,108,134 } },
            { 32, new int[] { 6,34,60,86,112,138 } },
            { 33, new int[] { 6,30,58,86,114,142 } },
            { 34, new int[] { 6,34,62,90,118,146 } },
            { 35, new int[] { 6,30,54,78,102,126,150 } },
            { 36, new int[] { 6,24,50,76,102,128,154 } },
            { 37, new int[] { 6,28,54,80,106,132,158 } },
            { 38, new int[] { 6,32,58,84,110,136,162 } },
            { 39, new int[] { 6,26,54,82,110,138,166 } },
            { 40, new int[] { 6,30,58,86,114,142,170 } },
        };

        public static int[] LevelingPatterns(int version) => table[version];
    }
}
