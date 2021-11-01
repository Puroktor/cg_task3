using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace cg_task3
{
    static class STL
    {
        public static float[,] ReadFromFile(Stream stream, float s)
        {
            List<float[]> list = new List<float[]>();
            float max = 0;
            float min = float.MaxValue;

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
                        int n = 3;
                        for (int i = 0; i < n; i++)
                        {
                            string[] strs = Regex.Replace(reader.ReadLine().Trim(), @"\s+", " ").Split();
                            float[] point = new float[n];
                            for (int j = 0; j < n; j++)
                            {
                                float cord = float.Parse(strs[j + 1], NumberStyles.Float, CultureInfo.InvariantCulture);
                                max = Math.Max(max, cord);
                                min = Math.Min(min, cord);
                                point[j] = float.Parse(strs[j + 1], NumberStyles.Float, CultureInfo.InvariantCulture);
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
                    int m = 3;
                    for (int j = 0; j < m; j++)
                    {
                        reader.ReadSingle();
                    }
                    for (int j = 0; j < m; j++)
                    {
                        float[] point = new float[m];
                        for (int k = 0; k < m; k++)
                        {
                            point[k] = reader.ReadSingle();
                            max = Math.Max(max, point[k]);
                            min = Math.Min(min, point[k]);
                        }
                        list.Add(point);
                    }
                    reader.ReadUInt16();
                }
            }
            float[,] matrix = new float[list.Count, 4];
            for (int i = 0; i < list.Count; i++)
            {
                matrix[i, 0] = list[i][0] / (max - min) * s;
                matrix[i, 1] = list[i][1] / (max - min) * s;
                matrix[i, 2] = list[i][2] / (max - min) * s;
                matrix[i, 3] = 1;
            }
            return matrix;
        }
    }
}
