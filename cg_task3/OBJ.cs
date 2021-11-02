using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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

            int k = 3;
            float max = 0;
            float min = float.MaxValue;
            float[] sum = new float[k];

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line.StartsWith("v"))
                {
                    string[] cords = Regex.Replace(line.Trim(), @"\s+", " ").Split();
                    float[] temp = new float[k];
                    for (int i = 1; i < k + 1; i++)
                    {
                        float value = float.Parse(cords[i], CultureInfo.InvariantCulture);
                        temp[i - 1] = value;
                        sum[i - 1] += value;
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
            return STL.Convert3DCord(f, max, min, s, sum.Select(x => x / v.Count).ToArray());
        }
    }
}
