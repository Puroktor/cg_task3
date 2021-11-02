using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace cg_task3
{
    static class STL
    {
        public static float[,] ReadFromFile(Stream stream, float s)
        {
            List<float[]> list = new List<float[]>();
            int k = 3;
            float max = 0;
            float min = float.MaxValue;
            float[] sum = new float[k];

            byte[] buff = new byte[80];
            stream.Read(buff, 0, buff.Length);
            string str = Encoding.ASCII.GetString(buff);
            if (str.StartsWith("solid"))
            {
                stream.Position = 0;
                using StreamReader reader = new StreamReader(stream);
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();
                    if (line.StartsWith("outer loop"))
                    {
                        for (int i = 0; i < k; i++)
                        {
                            string[] strs = Regex.Replace(reader.ReadLine().Trim(), @"\s+", " ").Split();
                            float[] point = new float[k];
                            for (int j = 0; j < k; j++)
                            {
                                point[j] = float.Parse(strs[j + 1], NumberStyles.Float, CultureInfo.InvariantCulture);
                                sum[j] += point[j];
                                max = Math.Max(max, point[j]);
                                min = Math.Min(min, point[j]);
                            }
                            list.Add(point);
                        }
                    }
                }
            }
            else
            {
                using BinaryReader reader = new BinaryReader(stream);
                int n = reader.ReadInt32();
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < k; j++)
                    {
                        reader.ReadSingle();
                    }
                    for (int j = 0; j < k; j++)
                    {
                        float[] point = new float[k];
                        for (int m = 0; m < k; m++)
                        {
                            point[m] = reader.ReadSingle();
                            sum[m] += point[m];
                            max = Math.Max(max, point[m]);
                            min = Math.Min(min, point[m]);
                        }
                        list.Add(point);
                    }
                    reader.ReadUInt16();
                }
            }
            return Convert3DCord(list, max, min, s, sum.Select(x => x / list.Count).ToArray());
        }

        public static float[,] Convert3DCord(List<float[]> list, float max, float min, float s, float[] centers)
        {
            float[,] matrix = new float[list.Count, 4];
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    matrix[i, j] = (list[i][j] - centers[j]) / (max - min) * 2 * s;
                }
                matrix[i, 3] = 1;
            }
            return matrix;
        }
    }
}
