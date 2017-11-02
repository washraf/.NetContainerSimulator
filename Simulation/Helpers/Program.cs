using System;
using System.Linq;

namespace Helpers
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            
            Console.WriteLine("\nBegin linear regression demo\n");
            int rows = 10;
            int seed = 1;

            Console.WriteLine("Creating " + rows + " rows synthetic data");
            double[][] data = DummyData(rows, seed);
            Console.WriteLine("Done\n");

            //double[][] data = MatrixLoad("..\\..\\IncomeData.txt", true, ',');

            Console.WriteLine("Education-Work-Sex-Income data:\n");
            ShowMatrix(data, 2);

            //Console.WriteLine("\nCreating design matrix from data");
            //double[][] design = lR.Design(data); // 'design matrix'
            //Console.WriteLine("Done\n");

            //Console.WriteLine("Design matrix:\n");
            //ShowMatrix(design, 2);

            Console.WriteLine("\nFinding coefficients using inversion");
            var lR = new LinerRegressionLeastSquares(data);
            Console.WriteLine("Done\n");

            //Console.WriteLine("Coefficients are:\n");
            //ShowVector(coef, 4);
            //Console.WriteLine("");

            //Console.WriteLine("Computing R-squared\n");
            //double R2 = lR.RSquared(data); // use initial data
            //Console.WriteLine("R-squared = " + R2.ToString("F4"));

            Console.WriteLine("\nPredicting income for ");
            Console.WriteLine("Education = 14");
            Console.WriteLine("Work      = 12");
            Console.WriteLine("Sex       = 0 (male)");

            double y = lR.FindEstimatedValue(11);
            Console.WriteLine("\nPredicted income = " + y.ToString("F2"));

            Console.WriteLine("\nEnd linear regression demo\n");
            Console.ReadLine();

            ///////////////////
            //double[] values = { 4.8, 4.8, 4.5, 3.9, 4.4, 3.6, 3.6, 2.9, 3.5, 3.0, 2.5, 2.2, 2.6, 2.1, 2.2 };
            double[] values = data.Select(x=>x[1]).ToArray();
            double xAvg = 0;
            double yAvg = 0;

            for (int x = 0; x < values.Length; x++)
            {
                xAvg += x;
                yAvg += values[x];
            }

            xAvg = xAvg / values.Length;
            yAvg = yAvg / values.Length;

            double v1 = 0;
            double v2 = 0;

            for (int x = 0; x < values.Length; x++)
            {
                v1 += (x - xAvg) * (values[x] - yAvg);
                v2 += Math.Pow(x - xAvg, 2);
            }

            double a = v1 / v2;
            double b = yAvg - a * xAvg;

            Console.WriteLine("y = ax + b");
            Console.WriteLine("a = {0}, the slope of the trend line.", Math.Round(a, 2));
            Console.WriteLine("b = {0}, the intercept of the trend line.", Math.Round(b, 2));
            Console.WriteLine($"result is {a*11+b}");
            Console.ReadLine();
        } // Main

        private static double[][] DummyData(int rows, int seed)
        {
            // generate dummy data for linear regression problem
            double b0 = 15.0;
            double b1 = 0.8; // education years
            double b2 = 0.5; // work years
            double b3 = -3.0; // sex = 0 male, 1 female
            Random rnd = new Random(seed);

            double[][] result = new double[rows][];
            for (int i = 0; i < rows; ++i)
                result[i] = new double[2];

            for (int i = 0; i < rows; ++i)
            {
                int ed = rnd.Next(12, 17); // 12, 16]
                int work = rnd.Next(10, 31); // [10, 30]
                int sex = rnd.Next(0, 2); // 0 or 1
                double y = b0 + (b1*ed) + (b2*work) + (b3*sex);
                y += 10.0*rnd.NextDouble() - 5.0; // random [-5 +5]

                result[i][0] = i;
                //result[i][1] = work;
                //result[i][2] = sex;
                result[i][1] = y; // income
            }
            return result;
        }

        private static void ShowMatrix(double[][] m, int dec)
        {
            for (int i = 0; i < m.Length; ++i)
            {
                for (int j = 0; j < m[i].Length; ++j)
                {
                    Console.Write(m[i][j].ToString("F" + dec) + "  ");
                }
                Console.WriteLine("");
            }
        }

        private static void ShowVector(double[] v, int dec)
        {
            for (int i = 0; i < v.Length; ++i)
                Console.Write(v[i].ToString("F" + dec) + "  ");
            Console.WriteLine("");
        }


        //static double[][] MatrixRandom(int rows, int cols,
        //  double minVal, double maxVal, int seed)
        //{
        //    // return a matrix with random values
        //    Random ran = new Random(seed);
        //    double[][] result = MatrixCreate(rows, cols);
        //    for (int i = 0; i < rows; ++i)
        //        for (int j = 0; j < cols; ++j)
        //            result[i][j] = (maxVal - minVal) *
        //              ran.NextDouble() + minVal;
        //    return result;
        //}

        // -------------------------------------------------------------

        //static double[][] MatrixLoad(string file, bool header,
        //  char sep)
        //{
        //    // load a matrix from a text file
        //    string line = "";
        //    string[] tokens = null;
        //    int ct = 0;
        //    int rows, cols;
        //    // determined # rows and cols
        //    System.IO.FileStream ifs =
        //      new System.IO.FileStream(file, System.IO.FileMode.Open);
        //    System.IO.StreamReader sr =
        //      new System.IO.StreamReader(ifs);
        //    while ((line = sr.ReadLine()) != null)
        //    {
        //        ++ct;
        //        tokens = line.Split(sep); // do validation here
        //    }
        //    sr.Close(); ifs.Close();
        //    if (header == true)
        //        rows = ct - 1;
        //    else
        //        rows = ct;
        //    cols = tokens.Length;
        //    double[][] result = MatrixCreate(rows, cols);

        //    // load
        //    int i = 0; // row index
        //    ifs = new System.IO.FileStream(file, System.IO.FileMode.Open);
        //    sr = new System.IO.StreamReader(ifs);

        //    if (header == true)
        //        line = sr.ReadLine();  // consume header
        //    while ((line = sr.ReadLine()) != null)
        //    {
        //        tokens = line.Split(sep);
        //        for (int j = 0; j < cols; ++j)
        //            result[i][j] = double.Parse(tokens[j]);
        //        ++i; // next row
        //    }
        //    sr.Close(); ifs.Close();
        //    return result;
        //}

        // -------------------------------------------------------------
        //static double[][] MatrixIdentity(int n)
        //{
        //    // return an n x n Identity matrix
        //    double[][] result = MatrixCreate(n, n);
        //    for (int i = 0; i < n; ++i)
        //        result[i][i] = 1.0;

        //    return result;
        //}

        //// -------------------------------------------------------------

        //static string MatrixAsString(double[][] matrix, int dec)
        //{
        //    string s = "";
        //    for (int i = 0; i < matrix.Length; ++i)
        //    {
        //        for (int j = 0; j < matrix[i].Length; ++j)
        //            s += matrix[i][j].ToString("F" + dec).PadLeft(8) + " ";
        //        s += Environment.NewLine;
        //    }
        //    return s;
        //}
        //static bool MatrixAreEqual(double[][] matrixA,
        // double[][] matrixB, double epsilon)
        //{
        //    // true if all values in matrixA == corresponding values in matrixB
        //    int aRows = matrixA.Length; int aCols = matrixA[0].Length;
        //    int bRows = matrixB.Length; int bCols = matrixB[0].Length;
        //    if (aRows != bRows || aCols != bCols)
        //        throw new Exception("Non-conformable matrices in MatrixAreEqual");

        //    for (int i = 0; i < aRows; ++i) // each row of A and B
        //        for (int j = 0; j < aCols; ++j) // each col of A and B
        //                                        //if (matrixA[i][j] != matrixB[i][j])
        //            if (Math.Abs(matrixA[i][j] - matrixB[i][j]) > epsilon)
        //                return false;
        //    return true;
        //}

        //// -------------------------------------------------------------
        //static double[] MatrixVectorProduct(double[][] matrix, double[] vector)
        //{
        //    // result of multiplying an n x m matrix by a m x 1 column vector (yielding an n x 1 column vector)
        //    int mRows = matrix.Length; int mCols = matrix[0].Length;
        //int vRows = vector.Length;
        //    if (mCols != vRows)
        //        throw new Exception("Non-conformable matrix and vector in MatrixVectorProduct");
        //double[] result = new double[mRows]; // an n x m matrix times a m x 1 column vector is a n x 1 column vector
        //    for (int i = 0; i<mRows; ++i)
        //        for (int j = 0; j<mCols; ++j)
        //            result[i] += matrix[i][j] * vector[j];
        //    return result;
        //}

        // -------------------------------------------------------------
        //private double MatrixDeterminant(double[][] matrix)
        //{
        //    int[] perm;
        //    int toggle;
        //    double[][] lum = MatrixDecompose(matrix, out perm, out toggle);
        //    if (lum == null)
        //        throw new Exception("Unable to compute MatrixDeterminant");
        //    double result = toggle;
        //    for (int i = 0; i < lum.Length; ++i)
        //        result *= lum[i][i];
        //    return result;
        //}

        //// -------------------------------------------------------------
        //-------------------------------------------------------------
        //private double[][] ExtractLower(double[][] matrix)
        //{
        //    // lower part of a Doolittle decomp (1.0s on diagonal, 0.0s in upper)
        //    int rows = matrix.Length; int cols = matrix[0].Length;
        //    double[][] result = MatrixCreate(rows, cols);
        //    for (int i = 0; i < rows; ++i)
        //    {
        //        for (int j = 0; j < cols; ++j)
        //        {
        //            if (i == j)
        //                result[i][j] = 1.0;
        //            else if (i > j)
        //                result[i][j] = matrix[i][j];
        //        }
        //    }
        //    return result;
        //}

        //private double[][] ExtractUpper(double[][] matrix)
        //{
        //    // upper part of a Doolittle decomp (0.0s in the strictly lower part)
        //    int rows = matrix.Length; int cols = matrix[0].Length;
        //    double[][] result = MatrixCreate(rows, cols);
        //    for (int i = 0; i < rows; ++i)
        //    {
        //        for (int j = 0; j < cols; ++j)
        //        {
        //            if (i <= j)
        //                result[i][j] = matrix[i][j];
        //        }
        //    }
        //    return result;
        //}

        //// -------------------------------------------------------------

        //private double[][] PermArrayToMatrix(int[] perm)
        //{
        //    // convert Doolittle perm array to corresponding perm matrix
        //    int n = perm.Length;
        //    double[][] result = MatrixCreate(n, n);
        //    for (int i = 0; i < n; ++i)
        //        result[i][perm[i]] = 1.0;
        //    return result;
        //}

        //private double[][] UnPermute(double[][] luProduct, int[] perm)
        //{
        //    // unpermute product of Doolittle lower * upper matrix according to perm[]
        //    // no real use except to demo LU decomposition, or for consistency testing
        //    double[][] result = MatrixDuplicate(luProduct);

        //    int[] unperm = new int[perm.Length];
        //    for (int i = 0; i < perm.Length; ++i)
        //        unperm[perm[i]] = i;

        //    for (int r = 0; r < luProduct.Length; ++r)
        //        result[r] = luProduct[unperm[r]];

        //    return result;
        //} // UnPermute


        //// =====
    }

}
