using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using CommonLibrary.Basic;
using CommonLibrary.FileIO;
using CommonLibrary.Output;
using DotNumerics.LinearAlgebra;

namespace CommonLibrary.Measurement
{
    public class QCalc
    {
        private double[][] S21 =new double[2][];  //メイン操作用データ配列　要素[0][]:周波数(GHz)、要素[1][]:S21(dB)
        

        private BasicComplex[,][] sParameter;        //touchstone等からの場合の生データ(RIデータ)

       
        
        ////curvefit-coef予想用の3座標
        //private double[] point0 = new double[2];
        //private double[] point1 = new double[2];
        //private double[] point2 = new double[2];
        
        private LorentzCurveFit cf;
        
       

        //コンストラクタ
        public QCalc(BasicComplex[,][] sParam, double[] freq) 
        {
            this.sParameter = new BasicComplex[sParam.GetLength(0), sParam.GetLength(1)][];
            for (int i = 0; i < sParam.GetLength(0); i++)
            {
                for (int j = 0; j < sParam.GetLength(1); j++)
                {
                    this.sParameter[j, i] = new BasicComplex[sParam[j, i].Length];
                    this.sParameter[j, i] = (BasicComplex[])sParam[j, i].Clone();
                }
            }

           
            this.S21[0] = new double[freq.Length];
            this.S21[1] = new double[sParam[1, 0].Length];

            this.S21[0] = freq;


            for (int i = 0; i < sParam[1, 0].Length; i++)
            {
                this.S21[1][i] = sParam[1, 0][i].dbMag();
            }
            
        }

        public QCalc(double[] sParam, double[] freq)
        {


            this.S21[0] = new double[freq.Length];
            this.S21[1] = new double[sParam.Length];
            this.S21[0] = freq;
            this.S21[1] = sParam;
            

        }


        




        //ある周波数範囲のS21を抽出
        public double[][] chooseData(double[][] sParam, double startF, double stopF)
        {
            double[][] dummy = new double[2][];
            int dummynum=0;

            //要素数決定
            for (int i = 0; i < sParam[1].Length; i++) 
            {
                if ((startF <= sParam[0][i]) && (sParam[0][i] <= stopF)) 
                {
                    dummynum++;
                }
            }

            //周波数範囲で抽出

            dummy[0] = new double[dummynum];
            dummy[1] = new double[dummynum];
            int dummyNum2=0;
            for (int i = 0; i < sParam[1].Length; i++)
            {
                if((startF<=sParam[0][i])&&(sParam[0][i]<=stopF))
                {
                    dummy[0][dummyNum2] = sParam[0][i];
                    dummy[1][dummyNum2] = sParam[1][i];
                    dummyNum2++;
                }
            }

            return dummy;
        }


        //グラフ表示分のデータを抽出

        public double[][] chooseDataFromGraph(double[][] sParam, RectanglarGraph rg)
        {
            double xMax = rg.getXMax()*1e9;
            double xMin = rg.getXMin()*1e9;
            double yMax = rg.getYMax();
            double yMin = rg.getYMin();

            double[][] dummy = new double[2][];
            
            //一度、表示されているデータ全部を抽出
            double[][] graphData = this.chooseData(sParam, xMin, xMax);


            //以下場合分け

            //(1)表示内全域でy>0の場合
            if((graphData[1][0]>=yMin)&&(graphData[1][graphData[1].Length-1]>=yMin))
            {
                dummy = graphData;
            }

            //(2)表示内左右でy<0の部分がある場合
            else if ((graphData[1][0] < yMin) && (graphData[1][graphData[1].Length - 1] < yMin))
            {
                int startindex = 0;
                int stopindex = graphData[1].Length - 1;
                //抽出開始場所のindexを決定
                while ((yMin - graphData[1][startindex]) * (yMin - graphData[1][startindex + 1]) > 0)
                {
                    startindex++;
                }
                //抽出終了場所のindexを決定
                while ((yMin - graphData[1][stopindex - 1]) * (yMin - graphData[1][stopindex]) > 0)
                {
                    stopindex--;
                }
                int choosedDataNum = (stopindex - startindex) + 1;

                //データ抽出
                int dummyNum = 0;
                dummy[0] = new double[choosedDataNum];
                dummy[1] = new double[choosedDataNum];
                for (int i = startindex; i < stopindex + 1; i++)
                {
                    dummy[0][dummyNum] = graphData[0][i];
                    dummy[1][dummyNum] = graphData[1][i];
                    dummyNum++;
                }

               

            }
            //(3)左側はy>=0,右側はy<0
            else if ((graphData[1][0] >= yMin) && (graphData[1][graphData.GetLength(1) - 1] < yMin))
            {
                int stopindex = graphData[1].Length - 1;
               
                
                //抽出終了場所のindexを決定
                while ((yMin - graphData[1][stopindex - 1]) * (yMin - graphData[1][stopindex]) > 0)
                {
                    stopindex--;
                }
                int choosedDataNum = stopindex + 1;

                //データ抽出
                int dummyNum = 0;
                dummy[0] = new double[choosedDataNum];
                dummy[1] = new double[choosedDataNum];
                for (int i = 0; i < stopindex + 1; i++)
                {
                    dummy[0][dummyNum] = graphData[0][i];
                    dummy[1][dummyNum] = graphData[1][i];
                    dummyNum++;
                }

                
            }
            //(4)右側はy>=0,左側はy<0
            else
            {
                int startindex = 0;
                //抽出開始場所のindexを決定
                while ((yMin - graphData[1][startindex]) * (yMin - graphData[1][startindex + 1]) > 0)
                {
                    startindex++;
                }

                int choosedDataNum = (graphData[1].Length - startindex);

                //データ抽出
                int dummyNum = 0;
                dummy[0] = new double[choosedDataNum];
                dummy[1] = new double[choosedDataNum];
                for (int i = startindex; i < graphData[1].Length; i++)
                {
                    dummy[0][dummyNum] = graphData[0][i];
                    dummy[1][dummyNum] = graphData[1][i];
                    dummyNum++;
                }

               
            }

            return dummy;

        }

        //データのMaxから左半分を抽出し、そのミラーコピーデータを作成

        public double[][] chooseLeftSide(double[][] sParam) 
        {
            int maxIndex = 0;
            double dummy = -1e9;
            for (int i = 0; i < sParam[1].Length; i++)
            {
                if (dummy < sParam[1][i])
                {
                    dummy = sParam[1][i];
                    maxIndex = i;
                }
            }


            int choosedDataNum = maxIndex*2+1;
            int dummyNum = 0;
            double[][] choosedData = new double[2][];

            choosedData[0] = new double[choosedDataNum];
            choosedData[1] = new double[choosedDataNum];

            for (int i = 0; i < choosedDataNum; i++)
            {
                if(dummyNum<=maxIndex)
                {
                    choosedData[0][dummyNum] = sParam[0][i];
                    choosedData[1][dummyNum] = sParam[1][i];
                    dummyNum++;
                }
                else if (dummyNum > maxIndex) 
                {
                    choosedData[0][dummyNum] = sParam[0][maxIndex]+(sParam[0][maxIndex]-sParam[0][maxIndex-(i-maxIndex)]);
                    choosedData[1][dummyNum] = sParam[1][maxIndex-(i-maxIndex)];
                    dummyNum++;

                }
            }

            return choosedData;

        }



        
        public double[][] chooseRightSide(double[][] sParam)
        {
            int maxIndex = 0;
            double dummy = -1e9;

            for (int i = sParam[1].Length-1; i > -1 ; i--)
            {
                if (dummy < sParam[1][i])
                {
                    dummy = sParam[1][i];
                    maxIndex = i;

                }
            }

            int choosedDataNum = ((sParam[1].Length-1)- maxIndex) * 2 + 1;
            

            int dummyNum = sParam[1].Length-1;
            double[][] choosedData = new double[2][];

            choosedData[0] = new double[choosedDataNum];
            choosedData[1] = new double[choosedDataNum];

            for (int i = 0; i < choosedDataNum; i++)
            {
                if (dummyNum >= maxIndex)
                {
                    choosedData[0][choosedDataNum-1 - i] = sParam[0][dummyNum];
                    choosedData[1][choosedDataNum-1 - i] = sParam[1][dummyNum];
                    dummyNum--;
                }
                else if (dummyNum < maxIndex)
                {
                    choosedData[0][choosedDataNum-1-i] = sParam[0][maxIndex] - (sParam[0][maxIndex+(maxIndex-dummyNum)] - sParam[0][maxIndex]);
                    choosedData[1][choosedDataNum-1-i] = sParam[1][maxIndex+(maxIndex-dummyNum)];
                    dummyNum--;
                    


                }
            }

            return choosedData;

        }






        //Max-xdBを抽出

        public double[][] chooseMaxToXdBData(double[][] sParam,double xdB) 
        {
            //Maxの探索
            int maxIndex = 0 ;
            double dummy = -200;
            for (int i = 0; i < sParam[1].Length; i++)
            {
                if (dummy < sParam[1][i])
                {
                    dummy = sParam[1][i];
                    maxIndex = i;
                }
            }

            double maxToXdB = dummy - xdB;
            int startindex=0;
            int stopindex = sParam[1].Length-1;
            //抽出開始場所のindexを決定
            while ((maxToXdB - sParam[1][startindex]) * (maxToXdB - sParam[1][startindex+1]) > 0)
            {
                startindex++;
            }
            //抽出終了場所のindexを決定
            while ((maxToXdB - sParam[1][stopindex - 1]) * (maxToXdB - sParam[1][stopindex]) > 0)
            {
                stopindex--;
            }
            int choosedDataNum = (stopindex-startindex)+1;

            //データ抽出
            int dummyNum=0;
            double[][] choosedData = new double[2][];

            choosedData[0] = new double[choosedDataNum];
            choosedData[1] = new double[choosedDataNum];

            for (int i = startindex; i < stopindex+1; i++) 
            {
                    choosedData[0][dummyNum] = sParam[0][i];
                    choosedData[1][dummyNum] = sParam[1][i];
                    dummyNum++;
            }

            return choosedData;

        }


        //Maxポイント
        public double[] getMax(double[][] sParam)
        {
            //Maxの探索
            int maxIndex = 0;
            double dummy = -1e9;
            for (int i = 0; i < sParam[1].Length; i++)
            {
                if (dummy < sParam[1][i])
                {
                    dummy = sParam[1][i];
                    maxIndex = i;
                }
            }

            double[] dummyD = new double[2];
            dummyD[0] = sParam[0][maxIndex];
            dummyD[1] = sParam[1][maxIndex];

            return dummyD;
        }


      

        

        //getメソッド

        public double[] getFreq()
        {
            return this.S21[0];
        }


        public double[] getS21()
        {
            return this.S21[1];
        }

        public double[][] getS21AndFreq()
        {
            return this.S21;
        }



      

        private double loadedQ(double[] coef) 
        {

            //return coef[2] / (2 * Math.Abs(coef[3])) * Math.Sqrt((coef[1] - 10 * Math.Log10(2) * coef[3]) / (10 * Math.Log10(2)));
            double q = coef[2] / (2 * Math.Abs(coef[3])) * Math.Sqrt((coef[1] - 10 * Math.Log10(2) * coef[3]) / (10 * Math.Log10(2)));
            return 1 / ((1 / q) - Math.Pow(1 / q, 2) / 4);
        }

        private double loadedQ(double[] coef,double maxToXdB)
        {

            //return coef[2] / (2 * Math.Abs(coef[3])) * Math.Sqrt((coef[1] - 10 * Math.Log10(2) * coef[3]) / (10 * Math.Log10(2)));
            double q = Math.Sqrt(Math.Pow(10,maxToXdB/10)-1) *coef[2] / (2 * Math.Abs(coef[3])) * Math.Sqrt((coef[1] - maxToXdB * coef[3]) / maxToXdB);
            return 1 / ((1 / q) - Math.Pow(1 / q, 2) / 4);
        }


       


        //2portでのLoaded-Qの導出

        

        public double get2portQL(double startf,double stopf,double maxToXdB)
        {
            double[][] choosedData = this.chooseData(this.S21, startf, stopf);
            double[][] xdBChoosedData = this.chooseMaxToXdBData(choosedData, maxToXdB);
            this.cf = new LorentzCurveFit(xdBChoosedData);
            return loadedQ(this.cf.getCoef());
        }

        public double get2portQL(double maxToXdB)
        {
          
            double[][] xdBChoosedData = this.chooseMaxToXdBData(this.S21, maxToXdB);
            this.cf = new LorentzCurveFit(xdBChoosedData);
            return loadedQ(this.cf.getCoef());
        }



        public double get2portQLFromGraph(RectanglarGraph rg)
        {
            this.cf = new LorentzCurveFit(this.chooseDataFromGraph(this.S21, rg));
            return loadedQ(this.cf.getCoef());
        }

        //public double get2portQLFromGraph(RectanglarGraph rg, double maxToXdB)
        //{
        //    double[,] xdBChoosedData = this.chooseMaxToXdBData(this.chooseDataFromGraph(this.S21,rg), maxToXdB);
        //    this.cf = new LorentzCurveFit(xdBChoosedData);
        //    return loadedQ(this.cf.getCoef());
        //}


        public double get2portQUFromGraph(RectanglarGraph rg)
        {
            double loadedQ = this.get2portQLFromGraph(rg);
            return loadedQ / (1 - Math.Pow(10, (rg.getYMax() / 20)));
            
        }

        public double get2portQL(LorentzCurveFit lcf)
        {
            return loadedQ(lcf.getCoef());
        }

        public double get2portQU(LorentzCurveFit lcf)
        {
            double YMax = lcf.getCoef()[0] + (lcf.getCoef()[1] / lcf.getCoef()[3]);
            return this.get2portQL(lcf) / (1 - Math.Pow(10, (YMax / 20)));
        }

        public double get2portQE(LorentzCurveFit lcf) 
        {
            double qu = this.get2portQU(lcf);
            double ql = this.get2portQL(lcf);
            return 2 * qu * ql / (qu - ql);
        }

        public double get2portQL(LorentzCurveFit lcf,double maxToXdB)
        {
            return loadedQ(lcf.getCoef(),maxToXdB);
        }

        public double get2portQU(LorentzCurveFit lcf,double maxToXdB)
        {
            double YMax = lcf.getCoef()[0] + (lcf.getCoef()[1] / lcf.getCoef()[3]);
            return this.get2portQL(lcf,maxToXdB) / (1 - Math.Pow(10, (YMax / 20)));
        }

        public double get2portQE(LorentzCurveFit lcf, double maxToXdB)
        {
            double qu = this.get2portQU(lcf, maxToXdB);
            double ql = this.get2portQL(lcf, maxToXdB);
            return 2 * qu * ql / (qu - ql);


        }




    }
}
