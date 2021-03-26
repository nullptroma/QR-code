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

            
            QRCodeCreator qrCodeCreator = new QRCodeCreator();
            qrCodeCreator.CreateQRCode(Encoding.ASCII.GetBytes(input));
            Console.WriteLine("ready");
            Console.ReadKey();
            //QRCode qr = new QRCode(Encoding.ASCII.GetBytes(input));
        }

    }
}
