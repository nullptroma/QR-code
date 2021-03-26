using System.Drawing;


namespace QR_code
{
    partial class QRCodeCreator
    {
        static class QRCodeSaver
        {
            private static Bitmap Image { get; set; }
            private static int[][] QRCode { get; set; }
            private static int SectorSize { get; set; }
            private static string Path { get; set; }

            public static void Save(int[][] qrCode, int sectorSize, string path)
            {
                QRCode = qrCode;
                SectorSize = sectorSize;
                Path = path;
                

                Save();
            }

            private static void Save()
            {
                Image = new Bitmap(QRCode.Length+8, QRCode.Length+8);
                using (var g = Graphics.FromImage(Image))
                    g.Clear(Color.White);
                for (int y = 4; y < Image.Height-4; y++)
                    for (int x = 4; x < Image.Width- 4; x++)
                        Image.SetPixel(x,y, QRCode[y-4][x-4]==1?Color.Black : Color.White);

                Zoom(Image, SectorSize).Save(Path);
            }

            private static Bitmap Zoom(Bitmap image, int k)
            {
                if (k <= 1) return image;
                Bitmap img = image;
                int width = img.Width;
                int height = img.Height;
                Bitmap zoomImg = new Bitmap(width * k, height * k);
                Graphics g = Graphics.FromImage(zoomImg);

                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                    {
                        Color color = img.GetPixel(i, j);
                        g.FillRectangle(new SolidBrush(color), i * k, j * k, k, k);
                    }

                return zoomImg;
            }
        }
    }
}
