using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SteganographyAPI.Common
{
    public class SteganographyHelper
    {
        public SteganographyHelper()
        {
        }

        // - public
        public static string decrypt(string nameImage, string key, string weight, out Exception exception)
        {
            var result = "";
            try
            {
                int[][] K = getKeyData(key);
                int[][] W = getWeightData(weight);
                int width = K.Length;
                int height = K[0].Length;

                var folder = FileManager.resultFolder();
                var fullPath = Path.Combine(folder, nameImage);
                Bitmap bitmap = new Bitmap(fullPath, true);

                int r = (int)(Math.Log2(width * height + 1));
                string messageBinary = "";
                for (int fj = 0; fj + height - 1 < bitmap.Height; fj += height)
                {
                    for (int fi = 0; fi + width - 1 < bitmap.Width; fi += width)
                    {
                        int sum = 0;
                        for (int x = fj; x < fj + height; x++)
                            for (int y = fi; y < fi + width; y++)
                            {
                                int bit = bitmap.GetPixel(y, x).R % 2;
                                sum += (bit ^ K[x % height][y % width]) * W[x % height][y % width];
                                sum %= r;
                            }

                        string messageBinarySegment = decToBin(sum);
                        messageBinary += messageBinarySegment;
                    }
                }

                for (int i=0;i<messageBinary.Length;i += 8)
                {
                    string bitsOfChar = messageBinary.Substring(i, 8);
                    int code = binToDec(bitsOfChar);
                    char character = (char)code;
                    result += character.ToString();
                }

                exception = null;
                return result;
            } catch (Exception ex)
            {
                exception = ex;
                return result;
            }
        }

        public static Bitmap encrypt(string nameImage, string message, string key, string weight, out Exception exception)
        {
            try
            {
                int[][] K = getKeyData(key);
                int[][] W = getWeightData(weight);
                int width = K.Length;
                int height = K[0].Length;

                var folder = FileManager.imageFolder();
                var fullPath = Path.Combine(folder, nameImage);
                Bitmap bitmap = new Bitmap(fullPath, true);

                string messageBinary = textToBin(message);
                int messageBinarySegmentIndex = 0;

                int r = (int)(Math.Log2(width * height + 1));
                int n = (int)Math.Pow(2, r);
                for (int fj = 0; fj+height-1 < bitmap.Height; fj += height)
                {
                    for (int fi = 0; fi + width - 1 < bitmap.Width; fi += width)
                    {
                        if (messageBinarySegmentIndex >= messageBinary.Length)
                            break;

                        int sum = 0;
                        for (int x = fj; x < fj + height; x++)
                            for (int y = fi; y < fi + width; y++)
                            {
                                int bit = bitmap.GetPixel(y, x).R % 2;
                                sum += (bit ^ K[x % height][y % width]) * W[x % height][y % width];
                                sum %= n;
                            }
                        string messageBinarySegment = messageBinary.Substring(messageBinarySegmentIndex, Math.Min(r, messageBinary.Length - messageBinarySegmentIndex));
                        messageBinarySegmentIndex += r;
                        int messageSegmentValue = binToDec(messageBinarySegment);

                        // int alpha = sum - binToDec(messageBinarySegment);
                        if (sum == messageSegmentValue)
                            continue;

                        Dictionary<int, List<KeyValuePair<int, int>>> resultInOperator1 = new Dictionary<int, List<KeyValuePair<int, int>>>();
                        for (int x = fj; x < fj + height; x++)
                            for (int y = fi; y < fi + width; y++)
                            {
                                int bit = (bitmap.GetPixel(y, x).R % 2) ^ K[x % height][y % width];
                                if (bit == 1)
                                {
                                    if (!resultInOperator1.ContainsKey((sum + n * 2 - W[x % height][y % width]) % n))
                                        resultInOperator1[(sum + n * 2 - W[x % height][y % width]) % n] = new List<KeyValuePair<int, int>>();
                                    resultInOperator1[(sum + n * 2 - W[x % height][y % width]) % n].Add(new KeyValuePair<int, int>(x, y));
                                }
                                else
                                {
                                    if (!resultInOperator1.ContainsKey((sum + W[x % height][y % width]) % n))
                                        resultInOperator1[(sum + W[x % height][y % width]) % n] = new List<KeyValuePair<int, int>>();
                                    resultInOperator1[(sum + W[x % height][y % width]) % n].Add(new KeyValuePair<int, int>(x, y));
                                }
                            }

                        if (resultInOperator1.ContainsKey(messageSegmentValue))
                        {
                            int x = resultInOperator1[messageSegmentValue][0].Key;
                            int y = resultInOperator1[messageSegmentValue][0].Value;
                            bitmap = reverseBit(bitmap, x, y);
                            continue;
                        }

                        var isHasResult = false;
                        KeyValuePair<int, int> firstPosition = new();
                        KeyValuePair<int, int> secondPosition = new();
                        for (int x = fj; x < fj + height; x++)
                        {
                            for (int y = fi; y < fi + width; y++)
                            {
                                int bit = (bitmap.GetPixel(y, x).R % 2) ^ K[x % height][y % width];
                                int val;
                                if (bit == 1)
                                    val = (messageSegmentValue + W[x % height][y % width]) % n;
                                else
                                    val = (messageSegmentValue - W[x % height][y % width] + 2 * n) % n;

                                if (resultInOperator1.ContainsKey(val))
                                {
                                    foreach (var obj in resultInOperator1[val])
                                    {
                                        if (!(obj.Key == x && obj.Value == y))
                                        {
                                            firstPosition = obj;
                                            secondPosition = new KeyValuePair<int, int>(x, y);
                                            isHasResult = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (isHasResult)
                                break;
                        }

                        if (isHasResult)
                        {
                            bitmap = reverseBit(bitmap, firstPosition.Key, firstPosition.Value);
                            bitmap = reverseBit(bitmap, secondPosition.Key, secondPosition.Value);
                        }
                        else
                        {
                            // ???
                        }
                    }

                    if (messageBinarySegmentIndex >= r)
                        break;
                }

                exception = null;
                return bitmap;
               
            }
            catch (Exception ex)
            {
                exception = ex;
                return null;
            }
        }

        // - private
        static string decToBin(int dec)
        {
            string result = "";

            while (dec > 0)
            {
                int bit = dec % 2;
                dec /= 2;
                result = bit.ToString() + result;
            }

            while (result.Length < 8)
            {
                result = "0" + result;
            }

            return result;
        }

        static int binToDec(string bin)
        {
            int result = 0;

            int ll = 1;
            for (int i=bin.Length-1; i>=0; i--)
            {
                result += int.Parse(bin[i].ToString()) * ll;
                ll *= 2;
            }

            return result;
        }

        public static string textToBin(string text)
        {
            var result = "";
            for (int i=0; i<text.Length; i++)
            {
                int code = (int)text[i];
                result += decToBin(code);
            }
            return result;
        }

        static int[][] getKeyData(string key)
        {
            string [] data = File.ReadAllText(Path.Combine(FileManager.keyFolder(), key + ".key")).Split(" ");
            int width = int.Parse(data[0]);
            int height= int.Parse(data[1]);
            int[][] result = new int [width][];
            int pointerIndex = 2;
            for (int i = 0; i < width; i++)
            {
                result[i] = new int [height];
                for (int j=0;j<height;j++)
                {
                    result[i][j] = int.Parse(data[pointerIndex]);
                    pointerIndex++;
                }
            }

            return result;
        }

        static int[][] getWeightData(string weight)
        {
            string[] data = File.ReadAllText(Path.Combine(FileManager.weightFolder(), weight + ".weight")).Split(" ");
            int width = int.Parse(data[0]);
            int height = int.Parse(data[1]);
            int[][] result = new int[width][];
            int pointerIndex = 2;
            for (int i = 0; i < width; i++)
            {
                result[i] = new int[height];
                for (int j = 0; j < height; j++)
                {
                    result[i][j] = int.Parse(data[pointerIndex]);
                    pointerIndex++;
                }
            }

            return result;
        }

        public static void generateKey(int width, int height)
        {
            var result = width.ToString() + " " + height.ToString();
            var rand = new Random();
            for (int i = 0; i<width*height; i++)
            {
                result += " " + rand.Next(0,2).ToString();
            }

            File.WriteAllTextAsync(Path.Combine(FileManager.keyFolder(), MD5Hash(DateTime.Now.ToString()) + ".key"), result);
        }

        public static void generateWeight(int width, int height)
        {
            var result = width.ToString() + " " + height.ToString();
            var value = new List<int>();
            var rand = new Random();

            for (int i = 0; i < width * height; i++)
                value.Add(i);

            for (int i = 0; i < width * height; i++)
            {
                int index = rand.Next(0, value.Count);
                result += " " + value[index];
                value.RemoveAt(index);
            }

            File.WriteAllTextAsync(Path.Combine(FileManager.weightFolder(), MD5Hash(DateTime.Now.ToString()) + ".weight"), result);
        }

        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }

        static Bitmap reverseBit(Bitmap bitmap, int x, int y)
        {
            var color = bitmap.GetPixel(y, x);
            int red = color.R;
            if (color.R % 2 == 0)
            {
                red = color.R + 1;
            }
            else
            {
                red = color.R - 1;
            }

            bitmap.SetPixel(x, y, Color.FromArgb(color.A, red, color.G, color.B));
            return bitmap;
        }
    }
}
