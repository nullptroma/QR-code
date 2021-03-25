using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QR_code.Tables;

namespace QR_code
{
    partial class QRCodeCreator
    {
        public object CreateQRCode(byte[] bytesToCode)
        {
            QRCodeBytes qr = new QRCodeBytes(bytesToCode);
            QRCodeWriter qrwr = new QRCodeWriter(qr.Service, qr.ToWrite);
            return null;
        }

        private class QRCodeBytes
        {
            private static bool[][] MissingBits = new bool[][] { new bool[] { true, true, true, false, true, true, false, false }, new bool[] { false, false, false, true, false, false, false, true } };//11101100 00010001
            public QRCodeServiceInformation Service;
            public byte[] ToWrite;

            public QRCodeBytes(byte[] bytesToCode)
            {
                GenerateBytesToWrite(bytesToCode);
            }

            public void GenerateBytesToWrite(byte[] bytesToCode)
            {
                Service = QRPayloadLengthForVerAndCLvl.GetServiceInformationForDataLength(bytesToCode.Length * 8, QRCodeServiceInformation.CorrectionLvl.H);
                bool[] allBits = GetQRBits(bytesToCode);
                byte[] allBytes = BitsToBytes(allBits);
                byte[][] allBlocks = CreateQRBlocks(allBytes);
                byte[][] correctionBytes = CreateCorrectionBytes(allBlocks);
                ToWrite = AlternatelyCombining(allBlocks, correctionBytes);
                Console.WriteLine("Service: " + Service);
                Console.WriteLine("ToWrite: " + ToWrite.Length);
            }

            //подготавливает биты, записывает в них способ кодирования, количество данных и сами данные
            private bool[] GetQRBits(byte[] bytesToCode)
            {
                List<bool> allBits = BytesToBits(bytesToCode);
                bool[] dataAmount = new bool[Service.LengthOfDataAmount];
                int bytesAmount = bytesToCode.Length;//нам нужно количество в байтах
                for (int i = dataAmount.Length - 1; i >= 0; i--)
                {
                    dataAmount[i] = (bytesAmount & 1) != 0;
                    bytesAmount = bytesAmount >> 1;
                }
                allBits.InsertRange(0, dataAmount);//записываем количество данных
                allBits.InsertRange(0, new bool[] { false, true, false, false });//записываем способ кодирования
                allBits.AddRange(Enumerable.Repeat(false, allBits.Count % 8));//добавляем нули в конец
                while (allBits.Count < Service.PayloadBitsAmount)//заполняем недостающие биты, чередуя последовательности
                {
                    allBits.AddRange(MissingBits[0]);//11101100 
                    if (allBits.Count < Service.PayloadBitsAmount)
                        allBits.AddRange(MissingBits[1]);//00010001
                }
                return allBits.ToArray();
            }

            private byte[][] CreateQRBlocks(byte[] allBytes)
            {
                int blocksAmount = Service.BlocksAmount;
                byte[][] blocks = new byte[blocksAmount][];
                int blockLength = allBytes.Length / blocksAmount;//длина 1 блока
                int overflowBlocks = allBytes.Length % blocksAmount;//сколько блоков с переполнением
                for (int i = 0; i < blocks.Length; i++)
                {
                    if (i <= (blocks.Length - 1) - overflowBlocks)//если этот блок не входит в последние переполененные блоки
                        blocks[i] = new byte[blockLength];
                    else//если это переполненный блок
                        blocks[i] = new byte[blockLength + 1];
                }
                int byteIndex = 0;
                foreach (byte[] block in blocks)
                    for (int i = 0; i < block.Length; i++)
                        block[i] = allBytes[byteIndex++];
                return blocks;
            }

            private byte[][] CreateCorrectionBytes(byte[][] allBlocks)
            {
                int[] GeneratingPolynomial = Service.GeneratingPolynomial;//генерирующий многочлен
                int correctionBytesAmountForBlock = Service.CorrectionBytesAmount;
                int blocksAmount = allBlocks.Length;
                byte[][] correctionBytes = new byte[blocksAmount][];
                for (int blockIndex = 0; blockIndex < blocksAmount; blockIndex++)
                {
                    List<byte> correctionBlock = new List<byte>(new byte[Math.Max(correctionBytesAmountForBlock, allBlocks[blockIndex].Length)]);//байты коррекции для этого блока
                    for (int byteIndex = 0; byteIndex < allBlocks[blockIndex].Length; byteIndex++)
                        correctionBlock[byteIndex] = allBlocks[blockIndex][byteIndex];

                    for (int i = 0; i < allBlocks[blockIndex].Length; i++)
                    {
                        byte firstEl = correctionBlock[0];
                        correctionBlock.RemoveAt(0);
                        correctionBlock.Add(0);
                        if (firstEl == 0)
                            continue;

                        int antiGalois = GaloisField2_8.GetAntiGalois(firstEl);
                        for (int correctByteIndex = 0; correctByteIndex < correctionBytesAmountForBlock; correctByteIndex++)
                        {
                            int generatingSum = GeneratingPolynomial[correctByteIndex] + antiGalois;
                            if (generatingSum > 254)
                                generatingSum = generatingSum % 255;
                            int galois = GaloisField2_8.GetGalois(generatingSum);
                            correctionBlock[correctByteIndex] ^= (byte)galois;
                        }
                    }

                    correctionBytes[blockIndex] = correctionBlock.GetRange(0, correctionBytesAmountForBlock).ToArray();
                }
                return correctionBytes;
            }

            private T[] AlternatelyCombining<T>(params T[][][] twoArrays)
            {
                int length = 0;
                foreach (T[][] twoArray in twoArrays)
                    foreach (T[] array in twoArray)
                        length += array.Length;
                T[] outStream = new T[length];
                int outIndex = 0;
                foreach (T[][] twoArray in twoArrays)
                {
                    int maxLength = twoArray.Max(ar=>ar.Length);
                    for (int i = 0; i < maxLength; i++)
                    {
                        foreach (T[] array in twoArray)
                            if (array.Length > i)
                                outStream[outIndex++] = array[i];
                    }
                }
                return outStream;
            }

            public static List<bool> BytesToBits(byte[] bytes)
            {
                List<bool> bits = new List<bool>(bytes.Length * 8);
                foreach (byte b in bytes)
                {
                    int count = 7;
                    while (count >= 0)
                        bits.Add((b & (1 << count--)) != 0);
                }
                return bits;
            }

            private static byte[] BitsToBytes(bool[] bits)
            {
                byte[] answer = new byte[(bits.Count() + (bits.Count() % 8)) / 8];
                int bitIndex = 0;
                for (int i = 0; i < answer.Length; i++)
                {
                    int bitCount = 0;
                    while (bitCount++ < 8)
                        answer[i] = (byte)((answer[i] << 1) + (bits[bitIndex++] ? 1 : 0));
                }
                return answer;
            }
        }
    }

    struct QRCodeServiceInformation
    {
        public CorrectionLvl Lvl { get; set; }
        public int Version { get; set; }
        public int LengthOfDataAmount { get => Version <= 9 ? 8 : 16; }
        public int PayloadBitsAmount { get => QRPayloadLengthForVerAndCLvl.BitsAmount(Version, Lvl);  }
        public int CorrectionBytesAmount { get => QRCorrectionBytesAmountForVerAndCLvl.BytesAmount(Version, Lvl);  }
        public int BlocksAmount { get => QRBlocksAmountForVerAndCLvl.BlocksAmount(Version, Lvl);  }
        public int[] GeneratingPolynomial { get => QRGeneratingPolynomialsForCBytesAmount.GeneratingPolynomial(CorrectionBytesAmount);  }


        public enum CorrectionLvl
        {
            L,
            M,
            Q,
            H
        }

        public override string ToString()
        {
            return Version + Lvl.ToString();
        }
    }
}
