using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace QR_code
{
    partial class QRCodeCreator
    {
        private class QRCodeWriter
        {
            private QRCodeServiceInformation Service { get; set; }
            private byte[] ToWrite;
            private int[][] QRGrid;
            private HashSet<Point> dataWritedPoints;

            public QRCodeWriter(QRCodeServiceInformation _service, byte[] _toWrite)
            {
                WriteQRCode(_service, _toWrite);
            }

            public void WriteQRCode(QRCodeServiceInformation _service, byte[] _toWrite)
            {
                Service = _service;
                ToWrite = _toWrite;

                int[] levelingPatterns = Service.Version > 1 ? Tables.QRLevelingPatternsForVer.LevelingPatterns(Service.Version) : new int[0];
                int side = Service.Version > 1 ? levelingPatterns.Last() + 7 : 21;//длина строны QR кода

                QRGrid = new int[side][];
                for (int i = 0; i < side; i++)
                    QRGrid[i] = Enumerable.Repeat<int>(7, side).ToArray();

                SetFindPatterns();
                SetSyncStrips();
                QRGrid[QRGrid.Length - 8][8] = 1;
                SetAlignmentPatterns(levelingPatterns);
                if(Service.Version>=7)
                    SetVersionCode(Service.Version);
                dataWritedPoints = new HashSet<Point>();
                int[] scoresWithMasks = new int[8];
                for (int i = 0; i < scoresWithMasks.Length; i++)
                {
                    SetMaskAndCLvl(i);
                    WriteDataWithMask(Tables.MaskTable.Mask(i));
                    scoresWithMasks[i] = CalculateScores();
                }
                Console.WriteLine(string.Join(" ", scoresWithMasks));
                int minIndex = scoresWithMasks.ToList().IndexOf(scoresWithMasks.Min());
                SetMaskAndCLvl(minIndex);
                WriteDataWithMask(Tables.MaskTable.Mask(minIndex));
                QRCodeSaver.Save(QRGrid, 5, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @$"\result.jpg");
            }

            private int CalculateScores()
            {
                int scores = 0;
                //First
                //horizontal
                for (int y = 0; y < QRGrid.Length; y++)
                {
                    var rowBlocks = QRGrid[y].Aggregate(new List<List<int>>(), AddRepeatElement);
                    foreach (var block in rowBlocks)
                        if (block.Count >= 5)
                            scores += block.Count - 2;
                }
                //vertical
                for (int x = 0; x < QRGrid.Length; x++)
                {
                    int[] column = new int[QRGrid.Length];
                    for (int y = 0; y < column.Length; y++)
                        column[y] = QRGrid[y][x];
                    var columnBlocks = column.Aggregate(new List<List<int>>(), AddRepeatElement);
                    foreach (var block in columnBlocks)
                        if (block.Count >= 5)
                            scores += block.Count - 2;
                }

                //second
                for (int y = 0; y < QRGrid.Length; y++)
                {
                    for (int x = 0; x < QRGrid.Length; x++)
                    {
                        int currentModule = QRGrid[y][x];//true - black
                        try
                        {
                            if (QRGrid[y][x + 1] == currentModule && QRGrid[y + 1][x] == currentModule && QRGrid[y + 1][x + 1] == currentModule)
                                scores += 3;
                        }
                        catch { }
                    }
                }

                //third
                for (int y = 0; y < QRGrid.Length; y++)
                {
                    var rowBlocks = QRGrid[y].Aggregate(new List<List<int>>(), AddRepeatElement);
                    scores += CountBWBBBWBConstructs(rowBlocks) * 40;
                }
                for (int x = 0; x < QRGrid.Length; x++)
                {
                    int[] column = new int[QRGrid.Length];
                    for (int y = 0; y < column.Length; y++)
                        column[y] = QRGrid[y][x];
                    var columnBlocks = column.Aggregate(new List<List<int>>(), AddRepeatElement);
                    scores += CountBWBBBWBConstructs(columnBlocks) * 40;
                }

                //fourth
                int blackQuant = 0;
                foreach (var row in QRGrid)
                    foreach (var n in row)
                        blackQuant += n;
                double blackPerc = blackQuant * 1.0 / (QRGrid.Length * QRGrid.Length);
                scores += (int)Math.Abs((blackPerc * 100 - 50)) * 2;
                Console.WriteLine(blackPerc);
                return scores;
            }

            private int CountBWBBBWBConstructs(List<List<int>> blocks)
            {
                int count = 0;
                for(int i = 0; i < blocks.Count; i++)
                {
                    if (i < 2 || i > blocks.Count-3)
                        continue;

                    bool isValid = false;
                    var block = blocks[i];
                    if (block.Count == 3 && block[0] == 1)//если это 111
                    {
                        isValid = true;
                        bool needBlack = true;
                        for (int j = i - 2; j <= i + 2; j++)
                        {
                            if((needBlack != (blocks[j][0] == 1)) || blocks[j].Count > 1 && j!=i)
                            {
                                isValid = false;
                                break;
                            }
                            needBlack = !needBlack;
                        }
                    }
                    isValid &= (i - 3 >= 0 ? (blocks[i - 3].Count >= 4 && blocks[i - 3][0] == 0) : false) || (i + 3 < blocks.Count ? (blocks[i + 3].Count >= 4 && blocks[i + 3][0] == 0) : false); ;
                    if (isValid)
                        count++;
                }

                return count;
            }

            private List<List<T>> AddRepeatElement<T>(List<List<T>> list, T value)
            {
                var curr = list.LastOrDefault();
                if (curr != null && curr[0].Equals(value))
                    curr.Add(value);
                else
                {
                    var newList = new List<T> { value };
                    list.Add(newList);
                }

                return list;
            }

            private void WriteDataWithMask(Func<int, int, bool> mask)
            {
                bool[] toWriteBits = QRCodeBytes.BytesToBits(ToWrite.ToArray()).ToArray();//Clone
                int dataIndex = 0;
                int x = QRGrid.Length - 1;
                int y = QRGrid.Length - 1;
                bool top = true; //true - top
                while (dataIndex != toWriteBits.Length)
                {
                    if (top)
                    {
                        if (TrySetPixel(y, x, toWriteBits.Length > dataIndex ? toWriteBits[dataIndex]:false, mask))
                            dataIndex++;
                        x--;
                        if (TrySetPixel(y, x, toWriteBits.Length > dataIndex ? toWriteBits[dataIndex] : false, mask))
                            dataIndex++;
                        x++;
                        y--;
                    }
                    else
                    {
                        if (TrySetPixel(y, x, toWriteBits.Length > dataIndex ? toWriteBits[dataIndex] : false, mask))
                            dataIndex++;
                        x--;
                        if (TrySetPixel(y, x, toWriteBits.Length > dataIndex ? toWriteBits[dataIndex] : false, mask))
                            dataIndex++;
                        x++;
                        y++;
                    }
                    if (y < 0)
                    {
                        x -= 2;
                        y = 0;
                        top = false;
                    }
                    else if (y >= QRGrid.Length)
                    {
                        x -= 2;
                        y = QRGrid.Length - 1;
                        top = true;
                    }
                    if (x == 6)
                        x--;
                    if (x < 0)
                        break;
                }
                Console.WriteLine("dataIndex: " + dataIndex);
                Console.WriteLine("toWriteBits.Length: " + toWriteBits.Length);
            }

            private bool TrySetPixel(int y, int x, bool value, Func<int, int, bool> mask)
            {
                if (QRGrid[y][x] == 7 || dataWritedPoints.Contains(new Point(x, y)))
                {
                    QRGrid[y][x] = mask(x, y) ? (value ? 1 : 0) : (!value ? 1 : 0);
                    dataWritedPoints.Add(new Point(x,y));
                    return true;
                }
                return false;
            }

            private void SetMaskAndCLvl(int maskId)
            {
                int[] value = new int[15];
                for (int i = 0; i < value.Length; i++)
                    value[i] = Tables.QRMaskAndCLvlCodes.Code(Service.Lvl, maskId)[i] ? 1 : 0;
                if (value.Length != 15)
                    return;
                int valueIndex = 0;
                //слева вверху:
                for (int x = 0; x < 8; x++)
                    if (x!=6)
                        QRGrid[8][x] = value[valueIndex++];
                for (int y = 8; y >= 0; y--)
                    if (y!=6)
                        QRGrid[y][8] = value[valueIndex++];

                valueIndex = 0;
                //слева внизу:
                for (int y = QRGrid.Length - 1; y >= QRGrid.Length - 7; y--)
                        QRGrid[y][8] = value[valueIndex++];

                //справа вверху:
                for (int x = QRGrid.Length - 8; x <= QRGrid.Length - 1; x++)
                        QRGrid[8][x] = value[valueIndex++];
            }

            private void SetVersionCode(int version)
            {
                int bitIndex = 0;
                var bits = Tables.QRVersionCodes.VersionCode(version);
                for (int y = QRGrid.Length - 11; y < QRGrid.Length - 8; y++)
                    for (int x = 0; x < 6; x++)
                    {
                        QRGrid[y][x] = bits[bitIndex] ? 1 : 0;
                        QRGrid[x][y] = bits[bitIndex] ? 1 : 0;
                        bitIndex++;
                    }
            }


            private void SetAlignmentPatterns(int[] levelingPatterns)
            {
                for (int x = 0; x < levelingPatterns.Length; x++)
                    for (int y = 0; y < levelingPatterns.Length; y++)
                        if (levelingPatterns.Length < 2 || 
                           (!(x == 0 && y == 0) &&
                            !(x == 0 && y == levelingPatterns.Length - 1) &&
                            !(x == levelingPatterns.Length - 1 && y == 0)))
                        {
                            Rectangle rect = new Rectangle(new Point(levelingPatterns[x]-2, levelingPatterns[y]-2), new Size(5,5));
                            SetRectangle(rect, 1);
                            rect = new Rectangle(new Point(levelingPatterns[x] - 1, levelingPatterns[y] - 1), new Size(3, 3));
                            SetRectangle(rect, 0);
                            rect = new Rectangle(new Point(levelingPatterns[x], levelingPatterns[y]), new Size(1, 1));
                            SetRectangle(rect, 1);
                        }
            }

            private void SetSyncStrips()
            {
                bool value= true;
                for (int y = 6; y < QRGrid.Length-8; y++)
                {
                    QRGrid[y][6] = value ? 1 : 0;
                    value = !value;
                }
                value = true;
                for(int x = 6; x < QRGrid[0].Length-8; x++)
                {
                    QRGrid[6][x] = value ? 1 : 0;
                    value = !value;
                }
            }

            private void SetFindPatterns()
            {
                SetFindPattern(-1, -1);
                SetFindPattern(-1, QRGrid.Length-8);
                SetFindPattern(QRGrid[0].Length-8, -1);
            }
            
            private void SetFindPattern(int y, int x)
            {
                Rectangle rect = new Rectangle(new Point(y,x), new Size(9,9));
                SetRectangle(rect, 0);
                rect = new Rectangle(new Point(y+1, x+1), new Size(7, 7));
                SetRectangle(rect, 1); 
                rect = new Rectangle(new Point(y+2, x+2), new Size(5, 5));
                SetRectangle(rect, 0);
                rect = new Rectangle(new Point(y+3, x+3), new Size(3, 3));
                SetRectangle(rect, 1);
            }

            private void SetRectangle(Rectangle rect, int value)
            {
                for (int x = rect.X; x < rect.X + rect.Width; x++)
                    for (int y = rect.Y; y < rect.Y + rect.Height; y++)
                    {
                        if (y >= 0 && x >= 0)
                            if (QRGrid.Length > y && QRGrid[y].Length > x)
                                QRGrid[y][x] = value;
                    }
            }
        }
    }
}
