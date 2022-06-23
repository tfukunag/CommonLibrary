using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using DotNumerics.LinearAlgebra;
using DotNumerics.Optimization;
using CommonLibrary.Measurement;

namespace CommonLibrary.Basic
{
    public class LorentzCurveFit
    {
       
        private Simplex simplex = new Simplex();

        private double[] Y;
        private double[] X;
        private OptSimplexBoundVariable[] simplexVariables;
        private double[] XFitted;
        private double[] YFitted;
        private double[] coefficient = new double[4];
    
        //coef推定用ポイントは1dB,2dBのポイント

        public LorentzCurveFit(double[] X, double[] Y)
        {


            this.X = new double[X.GetLength(0)];
            this.X = (double[])X.Clone();

            this.Y = new double[Y.GetLength(0)];
            this.Y = (double[])Y.Clone();
            //this.fittingCount = 0;
      
        }

        public LorentzCurveFit(double[][] xyData)
        {

            this.X = xyData[0];
            this.Y = xyData[1];
            //this.fittingCount = 0;
        }

        public void fit()
        {

         

            this.simplexVariables = new OptSimplexBoundVariable[4];
            this.simplexVariables[0] = new OptSimplexBoundVariable("y0", this.coefficient[0]);
            this.simplexVariables[1] = new OptSimplexBoundVariable("A", this.coefficient[1]);
            this.simplexVariables[2] = new OptSimplexBoundVariable("x0", this.coefficient[2]);
            this.simplexVariables[3] = new OptSimplexBoundVariable("B", this.coefficient[3]);

           
            this.simplex.Tolerance = 1e-9;
         

            this.coefficient = this.simplex.ComputeMin(ObjetiveFunction, this.simplexVariables);


           
        }

        

        public double[] calcInitialCoef()
        {
            double c2 = this.getMax()[0];
            double y0 = this.getMax()[1];

            double x1 = this.getPoint1(3)[0];
            double y1 = this.getPoint1(3)[1];
            double x2 = this.getPoint1(5)[0];
            double y2 = this.getPoint1(5)[1];

            double[] initialCoef = new double[4];
            initialCoef[2] = c2;
            initialCoef[0] = (y2 * (x2 - c2) * (x2 - c2) * (y0 - y1) - y1 * (x1 - c2) * (x1 - c2) * (y0 - y2)) / ((x2 - c2) * (x2 - c2) * (y0 - y1) - (x1 - c2) * (x1 - c2) * (y0 - y2));
            initialCoef[3] = ((y1 - y2) * (x1 - c2) * (x1 - c2) * (x2 - c2) * (x2 - c2)) / ((x2 - c2) * (x2 - c2) * (y0 - y1) - (x1 - c2) * (x1 - c2) * (y0 - y2));
            initialCoef[1] = initialCoef[3] * (y0 - initialCoef[0]);

            return initialCoef;
        }

        public double[] calcInitialCoefRightSide()
        {
            double c2 = this.getMax()[0];
            double y0 = this.getMax()[1];

            double x1 = this.getPoint2(3)[0];
            double y1 = this.getPoint2(3)[1];
            double x2 = this.getPoint2(5)[0];
            double y2 = this.getPoint2(5)[1];

            double[] initialCoef = new double[4];
            initialCoef[2] = c2;
            initialCoef[0] = (y2 * (x2 - c2) * (x2 - c2) * (y0 - y1) - y1 * (x1 - c2) * (x1 - c2) * (y0 - y2)) / ((x2 - c2) * (x2 - c2) * (y0 - y1) - (x1 - c2) * (x1 - c2) * (y0 - y2));
            initialCoef[3] = ((y1 - y2) * (x1 - c2) * (x1 - c2) * (x2 - c2) * (x2 - c2)) / ((x2 - c2) * (x2 - c2) * (y0 - y1) - (x1 - c2) * (x1 - c2) * (y0 - y2));
            initialCoef[1] = initialCoef[3] * (y0 - initialCoef[0]);

            return initialCoef;
        }
        
        public double[] getX() 
        {
            return this.X;
        }


        public double[] getY()
        {
            return this.Y;
        }


        public double[] getCoef()
        {
            return this.coefficient;

        }

        public void setCoef(double[] cf) 
        {
            for (int i=0; i < cf.Length; i++) 
            {
                this.coefficient[i] = cf[i];
            }
        }

        //目的の関数
        private double ObjetiveFunction(double[] p)
        {
            double f = 0;
            double t = 0;
            int numPoints = this.X.GetLength(0);
            for (int i = 0; i < numPoints; i++)
            {
                t = this.X[i];

                double difference = p[0] + p[1] / (Math.Pow((t - p[2]), 2) + p[3]) - this.Y[i];
 
                //f += Math.Pow(difference, 2);
                f += Math.Pow(difference, 2);
              
            }
            return f;
        }

      

        private double LorentzFunction(double[] p,double x) 
        {
                return p[0] + (p[1] / (Math.Pow(x - p[2], 2) + p[3]));
            

        }

   


        public void setFittedCurveData(int points) 
        {
            this.XFitted = new double[points];
            this.YFitted = new double[points];

            double t = (X[X.GetLength(0) - 1] - X[0]) / points;

            for (int i = 0; i < points; i++)
            {
                XFitted[i] = X[0] + (double)(t * i);
                YFitted[i] = this.LorentzFunction(this.getCoef(),this.XFitted[i]);
            }
        }

     


        public double[] getFittedX() 
        {
            return this.XFitted;
        }

        public double[] getFittedY() 
        {
            return this.YFitted;
        }

        public double[,] getFittedXY() 
        {
            double[,] dummy = new double[2, this.XFitted.GetLength(0)];
            for (int i = 0; i < this.XFitted.GetLength(0); i++) 
            {
                dummy[0, i] = this.XFitted[i];
                dummy[1, i] = this.YFitted[i];
            }
            return dummy;
        }


        //以下、InicialGuess推定用ポイントの設定メソッド(private)

        private double[] getPoint1(double xdB)
        {
            //Maxの探索
            int maxIndex = 0;
            double dummy = -1e9;
            for (int i = 0; i < this.Y.Length; i++)
            {
                if (dummy < this.Y[i])
                {
                    dummy = this.Y[i];
                    maxIndex = i;
                }
            }

            double maxToXdB = dummy - xdB;
            int startindex = 0;
            int stopindex = this.Y.Length - 1;
            //抽出開始場所のindexを決定
            while ((maxToXdB - this.Y[startindex]) * (maxToXdB - this.Y[startindex + 1]) > 0)
            {
                startindex++;
            }
            //抽出終了場所のindexを決定
            while ((maxToXdB - this.Y[stopindex - 1]) * (maxToXdB - this.Y[stopindex]) > 0)
            {
                stopindex--;
            }
            int choosedDataNum = (stopindex - startindex) + 1;

            //データ抽出
            int dummyNum = 0;
            double[,] choosedData = new double[2, choosedDataNum];
            for (int i = startindex; i < stopindex + 1; i++)
            {
                choosedData[0, dummyNum] = this.X[i];
                choosedData[1, dummyNum] = this.Y[i];
                dummyNum++;
            }

            //抽出データより端点をget
            double[] a = new double[2];
            a[0] = choosedData[0, 0];
            a[1] = choosedData[1, 0];

            return a;

        }

        private double[] getPoint2(double xdB)
        {
            //Maxの探索
            int maxIndex = 0;
            double dummy = -1e9;
            for (int i = 0; i < this.Y.Length; i++)
            {
                if (dummy < this.Y[i])
                {
                    dummy = this.Y[i];
                    maxIndex = i;
                }
            }

            


            double maxToXdB = dummy - xdB;
            int startindex = 0;
            int stopindex = this.Y.Length- 1;
            //抽出開始場所のindexを決定
            while ((maxToXdB - this.Y[startindex]) * (maxToXdB - this.Y[startindex + 1]) > 0)
            {
                startindex++;
            }
            //抽出終了場所のindexを決定
            while ((maxToXdB - this.Y[stopindex - 1]) * (maxToXdB - this.Y[stopindex]) > 0)
            {
                stopindex--;
            }
            int choosedDataNum = (stopindex - startindex) + 1;

            //データ抽出
            int dummyNum = 0;
            double[,] choosedData = new double[2, choosedDataNum];
            for (int i = startindex; i < stopindex + 1; i++)
            {
                choosedData[0, dummyNum] = this.X[i];
                choosedData[1, dummyNum] = this.Y[i];
                dummyNum++;
            }

            //抽出データより端点をget
            double[] a = new double[2];
            a[0] = choosedData[0, choosedDataNum-1];
            a[1] = choosedData[1, choosedDataNum-1];

            return a;

        }



        //X座標:表示範囲のcenter、Y座標:Maxの値
        private double[] getMax()
        {
            //Maxの探索
            int maxIndex = 0;
            double dummy = -1e9;
            for (int i = 0; i < this.Y.Length; i++)
            {
                if (dummy < this.Y[i])
                {
                    dummy = this.Y[i];
                    maxIndex = i;
                }
            }


            double[] dummyD = new double[2];
            //dummyD[0] = sParam[0, maxIndex];
            dummyD[0] = this.X[this.centerIndex(3.0)];

            dummyD[1] = this.Y[maxIndex];
            return dummyD;
        }


        //中心周波数インデックスの探索
        private int centerIndex(double xdB) 
        {
            int iMin = 0;
            int iMax = X.Length-1;
            double YMax = Y.Max();

            while(Y[iMin]+xdB-YMax<0)
            {
                iMin++;
            }
            while (Y[iMax] + xdB-YMax < 0) 
            {
                iMax--;
            }
            return (int)((double)(iMax + iMin) / 2);
        }
       

      


    }      
    
}
