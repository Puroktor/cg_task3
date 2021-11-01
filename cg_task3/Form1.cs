using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace cg_task3
{
    public partial class Form1 : Form
    {
        private Matrix points = new Matrix(0, 0);
        private float zPoint = -1000;
        private Matrix projecttion;
        private Matrix scaleT = new Matrix(4, 4);
        public Form1()
        {
            InitializeComponent();
            pictureBox.Focus();
            openFileDialog.Filter = "Stl file|*.stl";
            openFileDialog.FileName = null;
            projecttion = new Matrix(new float[,]{  {1, 0, 0, 0},
                                                    {0, 1, 0, 0},
                                                    {0, 0, 0, 1/zPoint},
                                                    {0, 0, 0, 1}});
            scaleT[0, 0] = scaleT[1, 1] = scaleT[2, 2] = scaleT[3,3] =  1;
            pictureBox.Paint += PictureBox_Paint;
            SizeChanged += Form1_SizeChanged;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            pictureBox.Refresh();
        }

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (points.n == 0)
                return;
            PictureBox box = (PictureBox)sender;
            int w = box.Width, h = box.Height;
            Graphics g = e.Graphics;
            g.TranslateTransform(w / 2F, h / 2F);
            PaintAxes(g, w, h);
            Pen pen = new Pen(Color.Black);
            Matrix p = points * scaleT* projecttion;
            int n = 3;
            for (int i = 0; i < points.n; i += n)
            {
                PointF[] triangle = new PointF[n];
                for (int j = 0; j < n; j++)
                {
                    if (p[i + j, 3] < 0)
                    {
                        goto skip;
                    }
                    triangle[j] = new PointF(p[i + j, 0] / p[i + j, 3], p[i + j, 1] / p[i + j, 3]);
                }
                g.DrawPolygon(pen, triangle);
            skip:;
            }
        }



        private void PaintAxes(Graphics g, int w, int h)
        {
            Pen pen = new Pen(Color.Red, 1);
            g.DrawLine(pen, 0, -h / 2, 0, h / 2);
            pen.Color = Color.Green;
            g.DrawLine(pen, -w / 2, 0, w / 2, 0);
        }

        private void TrackBarX_ValueChanged(object sender, EventArgs e)
        {
            int degree = ((TrackBar)sender).Value;
            RotateX(degree - int.Parse(xLabel.Text));
            xLabel.Text = degree.ToString();
        }

        private void RotateX(int degree)
        {
            double rad = degree * Math.PI / 180;
            float cos = (float)Math.Cos(rad);
            float sin = (float)Math.Sin(rad);

            Matrix T = new Matrix(4, 4);
            T[0, 0] = T[3, 3] = 1;
            T[1, 1] = T[2, 2] = cos;
            T[1, 2] = -sin;
            T[2, 1] = sin;
            points *= T;
            pictureBox.Refresh();
        }

        private void trackBarY_ValueChanged(object sender, EventArgs e)
        {
            int degree = ((TrackBar)sender).Value;
            RotateY(degree - int.Parse(yLabel.Text));
            yLabel.Text = degree.ToString();
        }

        private void RotateY(int degree)
        {
            double rad = degree * Math.PI / 180;
            float cos = (float)Math.Cos(rad);
            float sin = (float)Math.Sin(rad);

            Matrix T = new Matrix(4, 4);
            T[1, 1] = T[3, 3] = 1;
            T[0, 0] = T[2, 2] = cos;
            T[0, 2] = sin;
            T[2, 0] = -sin;
            points *= T;
            pictureBox.Refresh();
        }

        private void trackBarZ_ValueChanged(object sender, EventArgs e)
        {
            int degree = ((TrackBar)sender).Value;
            RotateZ(degree - int.Parse(zLabel.Text));
            zLabel.Text = degree.ToString();
        }

        private void RotateZ(int degree)
        {
            double rad = degree * Math.PI / 180;
            float cos = (float)Math.Cos(rad);
            float sin = (float)Math.Sin(rad);

            Matrix T = new Matrix(4, 4);
            T[2, 2] = T[3, 3] = 1;
            T[0, 0] = T[1, 1] = cos;
            T[0, 1] = -sin;
            T[1, 0] = sin;
            points *= T;
            pictureBox.Refresh();
        }

        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
                return;
            List<float[]> list = new List<float[]>();
            float s = 400;
            float max = 0;
            float min = float.MaxValue;
            using (StreamReader reader = new StreamReader(openFileDialog.OpenFile()))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();
                    if (line.StartsWith("outer loop"))
                    {
                        int n = 3;
                        for (int i = 0; i < n; i++)
                        {
                            string[] strs = Regex.Replace(reader.ReadLine().Trim(), @"\s+", " ").Split();
                            float[] temp = new float[n];
                            for (int j = 0; j < n; j++)
                            {
                                float cord = float.Parse(strs[j + 1], NumberStyles.Float, CultureInfo.InvariantCulture);
                                max = Math.Max(max, cord);
                                min = Math.Min(min, cord);
                                temp[j] = float.Parse(strs[j + 1], NumberStyles.Float, CultureInfo.InvariantCulture);
                            }
                            list.Add(temp);
                        }
                    }
                }
            }
            float[,] matrix = new float[points.n + list.Count, 4];
            for (int i = 0; i < points.n; i++)
            {
                matrix[i, 0] = points[i, 0];
                matrix[i, 1] = points[i, 1];
                matrix[i, 2] = points[i, 2];
                matrix[i, 3] = points[i, 3];
            }
            for (int i = points.n; i < points.n + list.Count; i++)
            {
                matrix[i, 0] = (list[i - points.n][0] - (max + min) / 2) / (max - min) * s;
                matrix[i, 1] = (list[i - points.n][1] - (max + min) / 2) / (max - min) * s;
                matrix[i, 2] = (list[i - points.n][2] - (max + min) / 2) / (max - min) * s;
                matrix[i, 3] = 1;
            }
            points = new Matrix(matrix);
            pictureBox.Refresh();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            points = new Matrix(0, 0);
            pictureBox.Refresh();
        }

        private void transfXbutton_Click(object sender, EventArgs e)
        {
            Matrix T = new Matrix(4, 4);
            T[0, 0] = T[1, 1] = T[2, 2] = T[3, 3] = 1;
            T[3, 0] = float.Parse(transfXtextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture);
            points *= T;
            pictureBox.Refresh();
        }

        private void transfYbutton_Click(object sender, EventArgs e)
        {
            Matrix T = new Matrix(4, 4);
            T[0, 0] = T[1, 1] = T[2, 2] = T[3, 3] = 1;
            T[3, 1] = float.Parse(transfYtextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture);
            points *= T;
            pictureBox.Refresh();
        }

        private void transfZbutton_Click(object sender, EventArgs e)
        {
            Matrix T = new Matrix(4, 4);
            T[0, 0] = T[1, 1] = T[2, 2] = T[3, 3] = 1;
            T[3, 2] = float.Parse(transfZtextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture);
            points *= T;
            pictureBox.Refresh();
        }

        private void scaleXtrackBar_ValueChanged(object sender, EventArgs e)
        {
            int percentage = ((TrackBar)sender).Value;
            scaleT[0, 0] = percentage / 100f;
            pictureBox.Refresh();
            scaleXlabel.Text = percentage.ToString();
        }

        private void scaleYtrackBar_ValueChanged(object sender, EventArgs e)
        {
            int percentage = ((TrackBar)sender).Value;
            scaleT[1, 1] = percentage / 100f;
            pictureBox.Refresh();
            scaleYlabel.Text = percentage.ToString();
        }

        private void scaleZtrackBar_ValueChanged(object sender, EventArgs e)
        {
            int percentage = ((TrackBar)sender).Value;
            scaleT[2, 2] = percentage / 100f;
            pictureBox.Refresh();
            scaleZlabel.Text = percentage.ToString();
        }
    }
}
