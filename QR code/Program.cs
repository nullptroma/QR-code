using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace QR_code
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+@"\123.txt");

            //List<int> L = new List<int>();
            //List<int> M = new List<int>();
            //List<int> Q = new List<int>();
            //List<int> H = new List<int>();
            //foreach (string str in input.Split('\n'))
            //{
            //    foreach (string numStr in str.Split(' ', '\t'))
            //    {
            //        if (int.TryParse(numStr, out int d))
            //            switch (str[0])
            //            {
            //                case 'L':
            //                    L.Add(d);
            //                    break;
            //                case 'M':
            //                    M.Add(d);
            //                    break;
            //                case 'Q':
            //                    Q.Add(d);
            //                    break;
            //                case 'H':
            //                    H.Add(d);
            //                    break;
            //            }
            //    }
            //}
            //Console.WriteLine("L " + string.Join(", ", L));
            //Console.WriteLine("M " + string.Join(", ", M));
            //Console.WriteLine("Q " + string.Join(", ", Q));
            //Console.WriteLine("H " + string.Join(", ", H));
            //Console.WriteLine(L.Count + " " + M.Count + " " + Q.Count + " " + H.Count );
            //Console.WriteLine();
            input = Console.ReadLine();
            QRCodeCreator qrCodeCreator = new QRCodeCreator();
            qrCodeCreator.CreateQRCode(Encoding.ASCII.GetBytes(input));
            Console.WriteLine("ready");
            //QRCode qr = new QRCode(Encoding.ASCII.GetBytes(input));
        }
    }
}
