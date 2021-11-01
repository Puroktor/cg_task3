namespace cg_task3
{
    public class Matrix
    {
        private float[,] matrix;
        public readonly int n, m;
        public float this[int i, int j]
        {
            get { return matrix[i, j]; }
            set { matrix[i, j] = value; }
        }
        public Matrix(int n, int m)
        {
            matrix = new float[n, m];
            this.n = n;
            this.m = m;
        }
        public Matrix(float[,] matrix)
        {
            this.matrix = matrix;
            n = matrix.GetLength(0);
            m = matrix.GetLength(1);
        }

        public static Matrix operator *(Matrix first, Matrix second)
        {
            if (first == null || second == null)
                return null;
            if (first.m != second.n)
                throw new System.Exception("Матрицы разного размера!");
            Matrix res = new Matrix(first.n, second.m);
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

        public void ClearMatrix()
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    matrix[i, j] = 0F;
                }
            }
        }
    }
}
