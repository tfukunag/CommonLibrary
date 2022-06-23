using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Basic
{
    public class ComplexMatrix
    {
        private BasicComplex[,] Matrix;
        private bool SquareMatrix;
        private int rank;

        public ComplexMatrix(BasicComplex[,] a)
        {
            this.Matrix = a;
            if (a.GetLength(0) == a.GetLength(1)) { this.SquareMatrix = true; this.rank = a.GetLength(0); }
            else { this.SquareMatrix = false; }
        }

        public BasicComplex[,] getMatrix() 
        {
            return this.Matrix;
        }


        //行列の(i,j)要素を指定した値に変換する
        public void setValue(int numOfRow, int numOfColumn, BasicComplex valueToSet) 
        {
            this.Matrix[numOfRow, numOfColumn] = valueToSet;
        }




       //行列の加算
        public static ComplexMatrix operator +(ComplexMatrix Matrix1, ComplexMatrix Matrix2)
        {
            int length1Row = Matrix1.Matrix.GetLength(0);
            int length2Row = Matrix2.Matrix.GetLength(0);
            int length1Column = Matrix1.Matrix.GetLength(1);
            int length2Column = Matrix2.Matrix.GetLength(1);

            BasicComplex[,] dummy = new BasicComplex[length1Row, length1Column];

            if ((length1Row == length2Row) && (length1Column == length2Column))
            {
                for (int j = 0; j < length1Column; j++)
                {
                    for (int i = 0; i < length1Row; i++)
                    {
                        dummy[i, j] = Matrix1.Matrix[i, j] + Matrix2.Matrix[i, j];
                    }
                }
                return new ComplexMatrix(dummy);
            }
            else
            {
                throw new ArrayTypeMismatchException("行列の型が一致していません。");
            }

        }


        //行列の減算
        public static ComplexMatrix operator -(ComplexMatrix Matrix1, ComplexMatrix Matrix2)
        {
            int length1Row = Matrix1.Matrix.GetLength(0);
            int length2Row = Matrix2.Matrix.GetLength(0);
            int length1Column = Matrix1.Matrix.GetLength(1);
            int length2Column = Matrix2.Matrix.GetLength(1);

            BasicComplex[,] dummy = new BasicComplex[length1Row, length1Column];

            if ((length1Row == length2Row) && (length1Column == length2Column))
            {
                for (int j = 0; j < length1Column; j++)
                {
                    for (int i = 0; i < length1Row; i++)
                    {
                        dummy[i, j] = Matrix1.Matrix[i, j] - Matrix2.Matrix[i, j];
                    }
                }
                return new ComplexMatrix(dummy);
            }
            else
            {
                throw new ArrayTypeMismatchException("行列の型が一致していません。");
            }

        }




        //行列の乗算
        public static ComplexMatrix operator *(ComplexMatrix Matrix1, ComplexMatrix Matrix2)
        {
            int length1Row = Matrix1.Matrix.GetLength(0);
            int length2Row = Matrix2.Matrix.GetLength(0);
            int length1Column = Matrix1.Matrix.GetLength(1);
            int length2Column = Matrix2.Matrix.GetLength(1);
            BasicComplex[,] dummy = new BasicComplex[length1Row, length2Column];

            if (length1Column == length2Row)
            {
                for (int i = 0; i < length1Row; i++)
                {
                    for (int j = 0; j < length2Column; j++)
                    {
                        dummy[i, j] = new BasicComplex(0, 0);
                        for (int k = 0; k < length1Column; k++)
                        {
                            dummy[i, j] = dummy[i, j] + Matrix1.Matrix[i, k] * Matrix2.Matrix[k, j];
                        }

                    }
                }
                return new ComplexMatrix(dummy);

            }
            else
            {
                throw new ArrayTypeMismatchException("乗算可能な行列の型同士ではありません。");
            }

        }



        //行列の除算
        public static ComplexMatrix operator /(ComplexMatrix Matrix1, ComplexMatrix Matrix2)
        {
            int length1Row = Matrix1.Matrix.GetLength(0);
            int length2Row = Matrix2.Matrix.GetLength(0);
            int length1Column = Matrix1.Matrix.GetLength(1);
            int length2Column = Matrix2.Matrix.GetLength(1);
            BasicComplex[,] dummy = new BasicComplex[length1Row, length2Column];
            ComplexMatrix mDummy = new ComplexMatrix(dummy);

            if (Matrix2.SquareMatrix)
            {
                ComplexMatrix Matrix2Inverse = ComplexMatrix.inverseMatrix(Matrix2);
                mDummy = Matrix1 * Matrix2Inverse;
                return mDummy;
            }
            else { throw new ArrayTypeMismatchException("分母行列の逆行列が計算できません。"); }
      
        }



        //零行列
        public static ComplexMatrix zeroMatrix(int n) 
        {
            BasicComplex[,] dummy = new BasicComplex[n, n];

            for (int i = 0; i < n; i++) 
            {
                for(int j = 0;j<n;j++)
                {
                    dummy[i,j] = new BasicComplex(0,0);
                }
            }
            return new ComplexMatrix(dummy);
        }

        



        //単位行列
        public static ComplexMatrix elementaryMatrix(int n)
        {
            BasicComplex[,] dummy = new BasicComplex[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j) { dummy[i, j] = new BasicComplex(1, 0); }
                    else { dummy[i, j] = new BasicComplex(0, 0); }
                }
            }
            return new ComplexMatrix(dummy);
        }


        //行の入れ替え
        private static ComplexMatrix switchRows(ComplexMatrix Mtrx, int a, int b)
        {
            BasicComplex[,] dummy = (BasicComplex[,])Mtrx.Matrix.Clone();

            for (int i = 0; i < Mtrx.Matrix.GetLength(1); i++)
            {
                dummy[a, i] = new BasicComplex(Mtrx.Matrix[b, i]);
                dummy[b, i] = new BasicComplex(Mtrx.Matrix[a, i]);
            }

            return new ComplexMatrix(dummy);
        }

        //列の入れ替え
        private static ComplexMatrix switchColumns(ComplexMatrix Mtrx, int a, int b)
        {
            BasicComplex[,] dummy = (BasicComplex[,])Mtrx.Matrix.Clone();

            for (int i = 0; i < Mtrx.Matrix.GetLength(0); i++)
            {
                dummy[i, a] = new BasicComplex(Mtrx.Matrix[i, b]);
                dummy[i, b] = new BasicComplex(Mtrx.Matrix[i, a]);
            }

            return new ComplexMatrix(dummy);
        }

        //a行を(Complex)c倍する
        private static ComplexMatrix rowMul(ComplexMatrix Mtrx, int a, BasicComplex c)
        {
            BasicComplex[,] dummy = (BasicComplex[,])Mtrx.Matrix.Clone();

            for (int i = 0; i < Mtrx.Matrix.GetLength(1); i++)
            {
                dummy[a, i] = Mtrx.Matrix[a, i] * c;
            }
            return new ComplexMatrix(dummy);
        }

        //a列を(Complex)c倍する
        private static ComplexMatrix columnMul(ComplexMatrix Mtrx, int a, BasicComplex c)
        {
            BasicComplex[,] dummy = (BasicComplex[,])Mtrx.Matrix.Clone();

            for (int i = 0; i < Mtrx.Matrix.GetLength(0); i++)
            {
                dummy[i, a] = Mtrx.Matrix[i, a] * c;
            }
            return new ComplexMatrix(dummy);
        }

        //b行に、(b行+a行*(Complex)c)を代入して出力
        public static ComplexMatrix rowBPlusRowA(ComplexMatrix Mtrx, int a, int b, BasicComplex c)
        {
            BasicComplex[,] dummy = (BasicComplex[,])Mtrx.Matrix.Clone();

            for (int i = 0; i < Mtrx.Matrix.GetLength(1); i++)
            {
                dummy[b, i] = Mtrx.Matrix[b, i] + Mtrx.Matrix[a, i] * c;
            }

            return new ComplexMatrix(dummy);
        }

        //b行に、(b行-a行*(Complex)c)を代入して出力
        public static ComplexMatrix rowBMinusRowA(ComplexMatrix Mtrx, int a, int b, BasicComplex c)
        {
            BasicComplex[,] dummy = (BasicComplex[,])Mtrx.Matrix.Clone();

            for (int i = 0; i < Mtrx.Matrix.GetLength(1); i++)
            {
                dummy[b, i] = Mtrx.Matrix[b, i] - Mtrx.Matrix[a, i] * c;
            }

            return new ComplexMatrix(dummy);
        }


        //逆行列(掃き出し法)
        public static ComplexMatrix inverseMatrix(ComplexMatrix Mtrx)
        {
            int Rows = Mtrx.Matrix.GetLength(0);
            int Columns = Mtrx.Matrix.GetLength(1);

            if (!Mtrx.SquareMatrix)
            {
                throw new ArrayTypeMismatchException("逆行列に変換できる型の行列ではありません。");
            }

            else
            {
                BasicComplex[,] inv = elementaryMatrix(Rows).Matrix; //逆行列を掃き出すための単位行列を作成
                BasicComplex[,] dummy = (BasicComplex[,])Mtrx.Matrix.Clone(); //掃き出しを行う元行列を格納するダミー配列
                BasicComplex temp = new BasicComplex(0, 0);   //行ごとの逆数をとっておくための値

                for (int i = 0; i < Columns; i++)
                {
                    //[i,i]要素がゼロならば下の行と入れ替える作業を続ける
                    if ((dummy[i, i].getReal() == 0) && (dummy[i, i].getImage() == 0))
                    {
                        //for (int j = i+1; j < Rows; j++)
                        //{
                        int iDummy = i + 1;
                        do
                        {
                            dummy = switchRows(new ComplexMatrix(dummy), i, iDummy).Matrix;
                            inv = switchRows(new ComplexMatrix(inv), i, iDummy).Matrix;
                            iDummy++;
                        } while ((dummy[i, i].getReal() == 0) && (dummy[i, i].getImage() == 0));

                        //}
                        try
                        {
                            temp = dummy[i, i].inverse();
                        }
                        catch (Exception e) { System.Console.WriteLine(e.Message); }
                        dummy = rowMul(new ComplexMatrix(dummy), i, temp).Matrix;
                        inv = rowMul(new ComplexMatrix(inv), i, temp).Matrix;
                        for (int j = 0; j < Rows; j++)
                        {
                            if (j != i)
                            {
                                BasicComplex temp2 = dummy[j, i];
                                dummy = rowBMinusRowA(new ComplexMatrix(dummy), i, j, temp2).Matrix;
                                inv = rowBMinusRowA(new ComplexMatrix(inv), i, j, temp2).Matrix;
                            }
                        }
                    }

                    //[i,i]要素が0でない場合
                    else
                    {

                        temp = dummy[i, i].inverse();
                        dummy = rowMul(new ComplexMatrix(dummy), i, temp).Matrix;
                        inv = rowMul(new ComplexMatrix(inv), i, temp).Matrix;
                        for (int j = 0; j < Rows; j++)
                        {
                            if (j != i)
                            {
                                BasicComplex temp2 = dummy[j, i];
                                dummy = rowBMinusRowA(new ComplexMatrix(dummy), i, j, temp2).Matrix;
                                inv = rowBMinusRowA(new ComplexMatrix(inv), i, j, temp2).Matrix;
                            }
                        }

                    }

                }
                return new ComplexMatrix(inv);
            }



        }

        //a行,a列を除外した行列を出力
        public static ComplexMatrix deleteRowAndColumn(ComplexMatrix Mtrx, int a) 
        {
            BasicComplex[,] Data = Mtrx.Matrix;
            BasicComplex[,] newData = new BasicComplex[Mtrx.rank - 1, Mtrx.rank - 1];

            for (int i = 0; i < Mtrx.rank; i++) 
            {
                for(int j = 0;j<Mtrx.rank;j++)
                {
                    if ((i < a) && (j < a))
                    {
                        newData[i, j] = Data[i, j];
                    }
                    else if ((i < a) && (j > a))
                    {
                        newData[i, j] = Data[i, j + 1];
                    }
                    else if((i>a)&&(j<a))
                    {
                        newData[i, j] = Data[i + 1, j];
                    }
                    else if ((i > a) && (j > a)) 
                    {
                        newData[i, j] = Data[i + 1, j + 1];
                    }
                }
            }

            return new ComplexMatrix(newData);
        }



    }
}
