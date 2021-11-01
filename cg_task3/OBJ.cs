using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace cg_task3
{
    static class OBJ
    {
        public static float[,] ReadFromFile(Stream stream, float s)
        {
            using StreamReader reader = new StreamReader(stream);
            List<float[]> v = new List<float[]>();
            List<float[]> f = new List<float[]>();

            float max = 0;
            float min = float.MaxValue;

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line.StartsWith("v"))
                {
                    string[] cords = Regex.Replace(line.Trim(), @"\s+", " ").Split();
                    float[] temp = new float[3];
                    for (int i = 1; i < 4; i++)
                    {
                        float value = float.Parse(cords[i], CultureInfo.InvariantCulture);
                        temp[i - 1] = value;
                        max = Math.Max(max, value);
                        min = Math.Min(min, value);
                    }
                    v.Add(temp);
                    continue;
                }
                if (line.StartsWith("f"))
                {
                    string[] cords = Regex.Replace(line.Trim(), @"\s+", " ").Split();
                    for (int i = 1; i < 4; i++)
                    {
                        f.Add(v[int.Parse(cords[i].Split('/')[0]) - 1]);
                    }
                    continue;
                }
            }
            float[,] mat = new float[f.Count, 4];
            for (int i = 0; i < f.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    mat[i, j] = f[i][j] / (max - min) * s;
                }
                mat[i, 3] = 1;
            }
            return mat;
        }
    }
}
