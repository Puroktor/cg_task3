namespace cg_task3
{
    public class Matrix4
    {
        private float[,] matrix;
        public int n;
        public int m = 4;

        public Vector3D this[int i]
        {
            get
            {
                return new Vector3D(matrix[i, 0], matrix[i, 1], matrix[i, 2], matrix[i, 3]);
            }
            set
            {
                matrix[i, 0] = value.X;
                matrix[i, 1] = value.Y;
                matrix[i, 2] = value.Z;
                matrix[i, 3] = 1;
            }
        }
        public float this[int i, int j]
        {
            get { return matrix[i, j]; }
            set { matrix[i, j] = value; }
        }
        public Matrix4(int n)
        {
            matrix = new float[n, m];
            this.n = n;
        }
        public Matrix4(float[,] matrix)
        {
            if (m != matrix.GetLength(1))
                throw new System.Exception("Разное количество столбцов!");
            this.matrix = matrix;
            n = matrix.GetLength(0);
        }

        public float[,] GetMatrix()
        {
            return matrix;
        }
        public void AddLines(float[,] other)
        {
            if (m != other.GetLength(1))
                throw new System.Exception("Разное количество столбцов!");

            float[,] newMat = new float[n + other.GetLength(0), 4];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    newMat[i, j] = matrix[i, j];
                }
            }
            for (int i = n; i < n + other.GetLength(0); i++)
            {
                for (int j = 0; j < m; j++)
                {
                    newMat[i, j] = other[i - n, j];
                }
            }
            matrix = newMat;
            n += other.GetLength(0);
        }

        public static Matrix4 operator *(Matrix4 first, Matrix4 second)
        {
            if (first == null || second == null)
                return null;
            if (first.m != second.n)
                throw new System.Exception("Матрицы разного размера!");
            Matrix4 res = new Matrix4(first.n);
            for (int i = 0; i < first.n; i++)
            {
                for (int j = 0; j < second.m; j++)
                {
                    float sum = 0;
                    for (int k = 0; k < first.m; k++)
                    {
                        sum += first[i, k] * second[k, j];
                    }
                    res[i, j] = sum;
                }
            }
            return res;
        }
    }
}
