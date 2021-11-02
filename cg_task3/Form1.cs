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
        private readonly float vPoint = 5;
        private readonly float viewPoint = 2;
        private readonly float lightX = 0, lightY = 0, lightZ = 2;
        private readonly Matrix4 projecttion;
        private readonly Matrix4 scaleT = new Matrix4(4);
        public Form1()
        {
            InitializeComponent();
            pictureBox.Focus();
            openFileDialog.Filter = "stl model|*.stl|obj model|*obj";
            openFileDialog.FileName = null;
            projecttion = new Matrix4(new float[,]{  {1, 0, 0, 0},
                                                    {0, 1, 0, 0},
                                                    {0, 0, 0, -1/vPoint},
                                                    {0, 0, -1/viewPoint, 1}});
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
            if (points.n == 0)
                return;
            PictureBox box = (PictureBox)sender;
            float w = box.Width / 2F, h = box.Height / 2F;
            float m = Math.Min(w, h);
            Graphics g = e.Graphics;
            g.TranslateTransform(w, h);
            //PaintAxes(g, w, h);

            Vector3D lightRay = new Vector3D(0, 0, 0) - new Vector3D(lightX, lightY, lightZ);
            Matrix4 p = points * scaleT * projecttion;
            int k = 3;
            float[] dotProds = new float[points.n / k];
            float max = 0;
            for (int i = 0; i < points.n; i += k)
            {
                Vector3D normal = (p[i + 1] - p[i]) ^ (p[i + 2] - p[i + 1]);
                float dotProd = normal * lightRay;
                dotProds[i / k] = dotProd;
                max = Math.Max(max, dotProd);
            }
            for (int i = 0; i < points.n; i += k)
            {
                if (dotProds[i / k] > 0 && p[i].Z < 0 && p[i + 1].Z < 0 && p[i + 2].Z < 0)
                {
                    float normilized = dotProds[i / k] / max;
                    int color = (int)(45 + normilized * 210);
                    Brush brush = new SolidBrush(Color.FromArgb(0, color, 0));
                    g.FillPolygon(brush, new PointF[] { (p[i] * m).XY(), (p[i + 1] * m).XY(), (p[i + 2] * m).XY() });
                }
            }
        }

        private void PaintAxes(Graphics g, float w, float h)
        {
            Pen pen = new Pen(Color.Red, 1);
            g.DrawLine(pen, 0, -h, 0, h);
            pen.Color = Color.Green;
            g.DrawLine(pen, -w, 0, w, 0);
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
