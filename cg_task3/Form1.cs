using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace cg_task3
{
    public partial class Form1 : Form
    {
        private Matrix4 points = new Matrix4(0);
        private readonly float vPoint = -5;
        private readonly float viewPoint = 2;
        private readonly float lightX = 0, lightY = 0, lightZ = 2;
        private readonly Matrix4 projecttion;
        private readonly Matrix4 scaleT = new Matrix4(4);
        private const int K = 3;
        public Form1()
        {
            InitializeComponent();
            pictureBox.Focus();
            openFileDialog.Filter = "stl model|*.stl|obj model|*obj";
            openFileDialog.FileName = null;
            projecttion = new Matrix4(new float[,]{ {1, 0, 0, 0},
                                                    {0, 1, 0, 0},
                                                    {0, 0, 0, 1/vPoint},
                                                    {0, 0, 1/viewPoint, 1}});
            scaleT[0, 0] = scaleT[1, 1] = scaleT[2, 2] = scaleT[3, 3] = 1;
            pictureBox.Paint += PictureBox_Paint;
            SizeChanged += Form1_SizeChanged;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            pictureBox.Refresh();
        }

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            PictureBox box = (PictureBox)sender;
            float w = box.Width / 2F, h = box.Height / 2F;
            float m = Math.Min(w, h);
            Graphics g = e.Graphics;
            g.TranslateTransform(w, h);

            Matrix4 matrix = new Matrix4(points.GetMatrix()) * scaleT;
            matrix.AddLines(new float[,]{{ -2, 0, 0, 1 }, { 0, 0, 0, 1 } , { 2, 0, 0, 1 },
                                        { 0, -2, 0, 1 }, { 0, 0, 0, 1 } , { 0, 2, 0, 1 },
                                        { 0, 0, -2, 1 }, { 0, 0, 0, 1 } , { 0, 0, 2, 1 }});

            Vector3D lightRay = new Vector3D(0, 0, 0) - new Vector3D(lightX, lightY, lightZ);
            Matrix4 p = matrix * projecttion;
            SortTriangles(p);
            Vector3D[] n = FindNormalsToVertices(p);
            float[] dotProds = new float[p.n];
            float max = 0;
            for (int i = 0; i < p.n; i++)
            {
                dotProds[i] = n[i] * lightRay;
                max = Math.Max(max, dotProds[i]);
            }
            if (max == 0)
            {
                max = 1;
            }
            for (int i = 0; i < p.n; i += K)
            {
                Vector3D v1 = p[i], v2 = p[i + 1], v3 = p[i + 2];
                if (v1.Z > 0 && v2.Z > 0 && v3.Z > 0)
                {
                    DrawTriangle(g, (int)(v1.X * m), (int)(v1.Y * m), (int)(v2.X * m), (int)(v2.Y * m),
                        (int)(v3.X * m), (int)(v3.Y * m), dotProds[i] / max, dotProds[i + 1] / max, dotProds[i + 2] / max);
                }
            }
        }
        private void DrawTriangle(Graphics g, int x0, int y0, int x1, int y1, int x2, int y2, float I0, float I1, float I2)
        {
            if (y0 == y1 && y1 == y2)
            {
                DrawLine(g, x0, y1, x2 - x0, 0, 0);
            }
            if (y0 > y1)
            {
                Swap(ref y0, ref y1);
                Swap(ref x0, ref x1);
                Swap(ref I0, ref I1);
            }

            if (y0 > y2)
            {
                Swap(ref y0, ref y2);
                Swap(ref x0, ref x2);
                Swap(ref I0, ref I2);
            }

            if (y1 > y2)
            {
                Swap(ref y1, ref y2);
                Swap(ref x1, ref x2);
                Swap(ref I1, ref I2);
            }

            int cross_x1;
            int cross_x2;
            int dx1 = x1 - x0;
            int dy1 = y1 - y0;
            int dx2 = x2 - x0;
            int dy2 = y2 - y0;

            float cross_I1 = I0;
            float dI1 = (I1 - I0) / dy1;
            float cross_I2 = I0;
            float dI2 = (I2 - I0) / dy2;

            int top_y = y0;

            int[] x = new int[] { x0, x1, x2 };
            int[] y = new int[] { y0, y1, y2 };

            for (int i = 0; i < 2; i++)
            {
                while (top_y < y[i + 1])
                {
                    cross_x1 = x[i] + dx1 * (top_y - y[i]) / dy1;
                    cross_x2 = x0 + dx2 * (top_y - y0) / dy2;

                    if (cross_x1 > cross_x2)
                    {
                        DrawLine(g, cross_x2, top_y, cross_x1 - cross_x2, cross_I2, cross_I1);
                    }
                    else
                    {
                        DrawLine(g, cross_x1, top_y, cross_x2 - cross_x1, cross_I1, cross_I2);
                    }
                    cross_I1 += dI1;
                    cross_I2 += dI2;
                    top_y++;
                }
                dx1 = x2 - x1;
                dy1 = y2 - y1;
                dI1 = (I2 - I1) / dy1;
            }
        }

        private void DrawLine(Graphics g, int x, int y, int w, float I1, float I2)
        {
            float dI = (I2 - I1) / (w + 1);
            for (int tx = x; tx <= x + w; tx++)
            {
                if (I1 >= 0)
                {
                    using Pen pen = new Pen(Color.FromArgb(0, (int)(45 + I1 * 210), 0));
                    g.DrawRectangle(pen, tx, y, 1, 1);
                    I1 += dI;
                }
            }
        }

        private void Swap<T>(ref T f, ref T s)
        {
            T temp = f;
            f = s;
            s = temp;
        }
        private Vector3D[] FindNormalsToVertices(Matrix4 mat)
        {
            Vector3D[] fN = new Vector3D[mat.n / K];
            for (int i = 0; i < mat.n; i += K)
            {
                fN[i / K] = (mat[i + 1] - mat[i]) ^ (mat[i + 2] - mat[i]);
            }
            Vector3D[] v = new Vector3D[mat.n];
            for (int i = 0; i < mat.n; i++)
            {
                v[i] = mat[i];
            }
            Vector3D[] vN = new Vector3D[mat.n];
            for (int i = 0; i < mat.n; i++)
            {
                int count = 0;
                Vector3D sum = new Vector3D(0, 0, 0);
                for (int j = 0; j < mat.n; j++)
                {
                    if (v[j].Equals(v[i]))
                    {
                        sum += fN[j / K];
                        count++;
                    }
                }
                vN[i] = sum / count;
            }
            return vN;
        }
        private void SortTriangles(Matrix4 mat)
        {
            float[] mins = new float[mat.n / K];
            Vector3D[][] triangles = new Vector3D[mat.n / K][];
            for (int i = 0; i < mat.n / K; i++)
            {
                mins[i] = float.MaxValue;
                Vector3D[] triangle = new Vector3D[K];
                for (int j = 0; j < K; j++)
                {
                    triangle[j] = mat[3 * i + j];
                    mins[i] = Math.Min(mins[i], triangle[j].Z);
                }
                triangles[i] = triangle;
            }
            Array.Sort(mins, triangles);
            for (int i = 0; i < mat.n / K; i++)
            {
                for (int j = 0; j < K; j++)
                {
                    mat[3 * i + j] = triangles[i][j];
                }
            }
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

            Matrix4 T = new Matrix4(4);
            T[0, 0] = T[3, 3] = 1;
            T[1, 1] = T[2, 2] = cos;
            T[1, 2] = -sin;
            T[2, 1] = sin;
            points *= T;
            pictureBox.Refresh();
        }

        private void TrackBarY_ValueChanged(object sender, EventArgs e)
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

            Matrix4 T = new Matrix4(4);
            T[1, 1] = T[3, 3] = 1;
            T[0, 0] = T[2, 2] = cos;
            T[0, 2] = sin;
            T[2, 0] = -sin;
            points *= T;
            pictureBox.Refresh();
        }

        private void TrackBarZ_ValueChanged(object sender, EventArgs e)
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

            Matrix4 T = new Matrix4(4);
            T[2, 2] = T[3, 3] = 1;
            T[0, 0] = T[1, 1] = cos;
            T[0, 1] = -sin;
            T[1, 0] = sin;
            points *= T;
            pictureBox.Refresh();
        }

        private void MenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
                return;
            float size = 0.5f;
            float[,] newFigure;
            if (Path.GetExtension(openFileDialog.FileName).ToLower() == ".stl")
            {
                newFigure = STL.ReadFromFile(openFileDialog.OpenFile(), size);
            }
            else
            {
                newFigure = OBJ.ReadFromFile(openFileDialog.OpenFile(), size);
            }
            points.AddLines(newFigure);
            pictureBox.Refresh();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            points = new Matrix4(0);
            pictureBox.Refresh();
        }

        private void TransfXbutton_Click(object sender, EventArgs e)
        {
            Matrix4 T = new Matrix4(4);
            T[0, 0] = T[1, 1] = T[2, 2] = T[3, 3] = 1;
            T[3, 0] = float.Parse(transfXtextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture);
            points *= T;
            pictureBox.Refresh();
        }

        private void TransfYbutton_Click(object sender, EventArgs e)
        {
            Matrix4 T = new Matrix4(4);
            T[0, 0] = T[1, 1] = T[2, 2] = T[3, 3] = 1;
            T[3, 1] = float.Parse(transfYtextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture);
            points *= T;
            pictureBox.Refresh();
        }

        private void TransfZbutton_Click(object sender, EventArgs e)
        {
            Matrix4 T = new Matrix4(4);
            T[0, 0] = T[1, 1] = T[2, 2] = T[3, 3] = 1;
            T[3, 2] = float.Parse(transfZtextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture);
            points *= T;
            pictureBox.Refresh();
        }

        private void ScaleXtrackBar_ValueChanged(object sender, EventArgs e)
        {
            int percentage = ((TrackBar)sender).Value;
            scaleT[0, 0] = percentage / 100f;
            pictureBox.Refresh();
            scaleXlabel.Text = percentage.ToString();
        }

        private void ScaleYtrackBar_ValueChanged(object sender, EventArgs e)
        {
            int percentage = ((TrackBar)sender).Value;
            scaleT[1, 1] = percentage / 100f;
            pictureBox.Refresh();
            scaleYlabel.Text = percentage.ToString();
        }

        private void ScaleZtrackBar_ValueChanged(object sender, EventArgs e)
        {
            int percentage = ((TrackBar)sender).Value;
            scaleT[2, 2] = percentage / 100f;
            pictureBox.Refresh();
            scaleZlabel.Text = percentage.ToString();
        }
    }
}
