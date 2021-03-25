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
                Image = new Bitmap(QRCode.Length, QRCode.Length);
                for (int y = 0; y < QRCode.Length; y++)
                    for (int x = 0; x < QRCode.Length; x++)
                        Image.SetPixel(x,y, QRCode[y][x]==1?Color.Black : Color.White);

                Zoom(Image, 50).Save(Path);
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
