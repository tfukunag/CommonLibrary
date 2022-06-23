using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Basic
{
    public class DoubleMatrix
    {
        private double[,] Matrix;
        private bool SquareMatrix;

        public DoubleMatrix(double[,] a)
        {
            this.Matrix = a;
            if (a.GetLength(0) == a.GetLength(1)) { this.SquareMatrix = true; }
            else { this.SquareMatrix = false; }
        }


       //行列の加算
        public static DoubleMatrix operator +(DoubleMatrix Matrix1, DoubleMatrix Matrix2)
        {
            int length1Row = Matrix1.Matrix.GetLength(0);
            int length2Row = Matrix2.Matrix.GetLength(0);
            int length1Column = Matrix1.Matrix.GetLength(1);
            int length2Column = Matrix2.Matrix.GetLength(1);

            double[,] dummy = new double[length1Row, length1Column];

            if ((length1Row == length2Row) && (length1Column == length2Column))
            {
                for (int j = 0; j < length1Column; j++)
                {
                    for (int i = 0; i < length1Row; i++)
                    {
                        dummy[i, j] = Matrix1.Matrix[i, j] + Matrix2.Matrix[i, j];
                    }
                }
                return new DoubleMatrix(dummy);
            }
            else
            {
                throw new ArrayTypeMismatchException("行列の型が一致していません。");
            }

        }


        //行列の減算
        public static DoubleMatrix operator -(DoubleMatrix Matrix1, DoubleMatrix Matrix2)
        {
            int length1Row = Matrix1.Matrix.GetLength(0);
            int length2Row = Matrix2.Matrix.GetLength(0);
            int length1Column = Matrix1.Matrix.GetLength(1);
            int length2Column = Matrix2.Matrix.GetLength(1);

            double[,] dummy = new double[length1Row, length1Column];

            if ((length1Row == length2Row) && (length1Column == length2Column))
            {
                for (int j = 0; j < length1Column; j++)
                {
                    for (int i = 0; i < length1Row; i++)
                    {
                        dummy[i, j] = Matrix1.Matrix[i, j] - Matrix2.Matrix[i, j];
                    }
                }
                return new DoubleMatrix(dummy);
            }
            else
            {
                throw new ArrayTypeMismatchException("行列の型が一致していません。");
            }

        }




        //行列の乗算
        public static DoubleMatrix operator *(DoubleMatrix Matrix1, DoubleMatrix Matrix2)
        {
            int length1Row = Matrix1.Matrix.GetLength(0);
            int length2Row = Matrix2.Matrix.GetLength(0);
            int length1Column = Matrix1.Matrix.GetLength(1);
            int length2Column = Matrix2.Matrix.GetLength(1);
            double[,] dummy = new double[length1Row, length2Column];

            if (length1Column == length2Row)
            {
                for (int i = 0; i < length1Row; i++)
                {
                    for (int j = 0; j < length2Column; j++)
                    {
                        dummy[i, j] = 0;
                        for (int k = 0; k < length1Column; k++)
                        {
                            dummy[i, j] = dummy[i, j] + Matrix1.Matrix[i, k] * Matrix2.Matrix[k, j];
                        }

                    }
                }
                return new DoubleMatrix(dummy);

            }
            else
            {
                throw new ArrayTypeMismatchException("乗算可能な行列の型同士ではありません。");
            }

        }



        //行列の除算
        public static DoubleMatrix operator /(DoubleMatrix Matrix1, DoubleMatrix Matrix2)
        {
            int length1Row = Matrix1.Matrix.GetLength(0);
            int length2Row = Matrix2.Matrix.GetLength(0);
            int length1Column = Matrix1.Matrix.GetLength(1);
            int length2Column = Matrix2.Matrix.GetLength(1);
            double[,] dummy = new double[length1Row, length2Column];
            DoubleMatrix mDummy = new DoubleMatrix(dummy);

            if (Matrix2.SquareMatrix)
            {
                DoubleMatrix Matrix2Inverse = DoubleMatrix.inverseMatrix(Matrix2);
                mDummy = Matrix1 * Matrix2Inverse;
                return mDummy;
            }
            else { throw new ArrayTypeMismatchException("分母行列の逆行列が計算できません。"); }
      
        }



        //単位行列
        public static DoubleMatrix elementaryMatrix(int n)
        {
            double[,] dummy = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j) { dummy[i, j] = 0; }
                    else { dummy[i, j] = 0; }
                }
            }
            return new DoubleMatrix(dummy);
        }


        //行の入れ替え
        private static DoubleMatrix switchRows(DoubleMatrix Mtrx, int a, int b)
        {
            double[,] dummy = (double[,])Mtrx.Matrix.Clone();

            for (int i = 0; i < Mtrx.Matrix.GetLength(1); i++)
            {
                dummy[a, i] = Mtrx.Matrix[b, i];
                dummy[b, i] = Mtrx.Matrix[a, i];
            }

            return new DoubleMatrix(dummy);
        }

        //列の入れ替え
        private static DoubleMatrix switchColumns(DoubleMatrix Mtrx, int a, int b)
        {
            double[,] dummy = (double[,])Mtrx.Matrix.Clone();

            for (int i = 0; i < Mtrx.Matrix.GetLength(0); i++)
            {
                dummy[i, a] = Mtrx.Matrix[i, b];
                dummy[i, b] = Mtrx.Matrix[i, a];
            }

            return new DoubleMatrix(dummy);
        }

        //a行をc倍する
        private static DoubleMatrix rowMul(DoubleMatrix Mtrx, int a, double c)
        {
            double[,] dummy = (double[,])Mtrx.Matrix.Clone();

            for (int i = 0; i < Mtrx.Matrix.GetLength(1); i++)
            {
                dummy[a, i] = Mtrx.Matrix[a, i] * c;
            }
            return new DoubleMatrix(dummy);
        }

        //a列をc倍する
        private static DoubleMatrix columnMul(DoubleMatrix Mtrx, int a, double c)
        {
            double[,] dummy = (double[,])Mtrx.Matrix.Clone();

            for (int i = 0; i < Mtrx.Matrix.GetLength(0); i++)
            {
                dummy[i, a] = Mtrx.Matrix[i, a] * c;
            }
            return new DoubleMatrix(dummy);
        }

        

        //a行を1/c倍する
        private static DoubleMatrix rowDiv(DoubleMatrix Mtrx, int a, double c)
        {
            double[,] dummy = (double[,])Mtrx.Matrix.Clone();

            for (int i = 0; i < Mtrx.Matrix.GetLength(1); i++)
            {
                dummy[a, i] = Mtrx.Matrix[a, i] / c;
            }
            return new DoubleMatrix(dummy);
        }

        //a列をc倍する
        private static DoubleMatrix columnDiv(DoubleMatrix Mtrx, int a, double c)
        {
            double[,] dummy = (double[,])Mtrx.Matrix.Clone();

            for (int i = 0; i < Mtrx.Matrix.GetLength(0); i++)
            {
                dummy[i, a] = Mtrx.Matrix[i, a] / c;
            }
            return new DoubleMatrix(dummy);
        }



        //b行に、(b行+a行*(Complex)c)を代入して出力
        private static DoubleMatrix rowBPlusRowA(DoubleMatrix Mtrx, int a, int b, double c)
        {
            double[,] dummy = (double[,])Mtrx.Matrix.Clone();

            for (int i = 0; i < Mtrx.Matrix.GetLength(1); i++)
            {
                dummy[b, i] = Mtrx.Matrix[b, i] + Mtrx.Matrix[a, i] * c;
            }

            return new DoubleMatrix(dummy);
        }





        //b行に、(b行-a行*(Complex)c)を代入して出力
        private static DoubleMatrix rowBMinusRowA(DoubleMatrix Mtrx, int a, int b, double c)
        {
            double[,] dummy = (double[,])Mtrx.Matrix.Clone();

            for (int i = 0; i < Mtrx.Matrix.GetLength(1); i++)
            {
                dummy[b, i] = Mtrx.Matrix[b, i] - Mtrx.Matrix[a, i] * c;
            }

            return new DoubleMatrix(dummy);
        }

        //逆行列(掃き出し法)
        public static DoubleMatrix inverseMatrix(DoubleMatrix Mtrx)
        {
            int Rows = Mtrx.Matrix.GetLength(0);
            int Columns = Mtrx.Matrix.GetLength(1);

            if (!Mtrx.SquareMatrix)
            {
                throw new ArrayTypeMismatchException("逆行列に変換できる型の行列ではありません。");
            }

            else
            {
                double[,] inv = elementaryMatrix(Rows).Matrix; //逆行列を掃き出すための単位行列を作成
                double[,] dummy = (double[,])Mtrx.Matrix.Clone(); //掃き出しを行う元行列を格納するダミー配列
                double temp= 0;   //行ごとの逆数をとっておくための値
                
                for (int i = 0; i < Columns; i++)
                {
                    //[i,i]要素が0の場合：下の行と入れ替える作業を続ける
                    if (dummy[i, i]==0)
                    {
                        //for (int j = i+1; j < Rows; j++)
                        //{
                        int iDummy = i+1;
                            do
                            {
                                dummy = switchRows(new DoubleMatrix(dummy), i, iDummy).Matrix;
                                inv = switchRows(new DoubleMatrix(inv), i, iDummy).Matrix;
                                iDummy++;
                            } while (dummy[i, i]==0);

                        //}
                            
                        temp = dummy[i, i];
                        dummy = rowDiv(new DoubleMatrix(dummy), i, temp).Matrix;
                        inv = rowDiv(new DoubleMatrix(inv), i, temp).Matrix;
                        for (int j = 0; j < Rows; j++)
                        {
                            if (j != i)
                            {
                                double temp2 = dummy[j, i];
                                dummy = rowBMinusRowA(new DoubleMatrix(dummy), i, j, temp2).Matrix;
                                inv = rowBMinusRowA(new DoubleMatrix(inv), i, j, temp2).Matrix;
                            }
                        }
                    }

                    //[i,i]要素が0でない場合
                    else
                    {

                        temp = dummy[i, i];
                        dummy = rowDiv(new DoubleMatrix(dummy), i, temp).Matrix;
                        inv = rowDiv(new DoubleMatrix(inv), i, temp).Matrix;
                        for (int j = 0; j < Rows; j++)
                        {
                            if (j != i)
                            {
                                double temp2 = dummy[j, i];
                                dummy = rowBMinusRowA(new DoubleMatrix(dummy), i, j, temp2).Matrix;
                                inv = rowBMinusRowA(new DoubleMatrix(inv), i, j, temp2).Matrix;
                            }
                        }

                    }

                }
                return new DoubleMatrix(inv);
            }


        }
    }
}