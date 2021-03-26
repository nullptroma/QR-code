using System;
using System.Collections.Generic;
using System.Text;

namespace QR_code.Tables
{
    static class MaskTable
    {
        private static Func<int, int, bool>[] masks = new Func<int, int, bool>[] 
        {
            new Func<int, int, bool>((X,Y) => (X+Y) % 2 == 1),
            new Func<int, int, bool>((X,Y) => Y % 2 == 1),
            new Func<int, int, bool>((X,Y) => X % 3 == 1),
            new Func<int, int, bool>((X,Y) => (X + Y) % 3 == 1),
            new Func<int, int, bool>((X,Y) => (X/3 + Y/2) % 2 == 1),
            new Func<int, int, bool>((X,Y) => (X*Y) % 2 + (X*Y) % 3 == 1),
            new Func<int, int, bool>((X,Y) => ((X*Y) % 2 + (X*Y) % 3) % 2 == 1),
            new Func<int, int, bool>((X,Y) => ((X*Y) % 3 + (X+Y) % 2) % 2 == 1),
        };

        public static Func<int, int, bool> Mask(int id) => masks[id];
    }
}
