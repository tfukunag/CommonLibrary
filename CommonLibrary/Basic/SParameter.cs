using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.FileIO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.IO;

namespace CommonLibrary.Basic
{
    public class SParameter 
    {
        private BasicComplex[,][] parameter;

        private double[] frequency;
        private int numOfPorts;
        private double[] previousz0;
        private double[] currentz0;
        private bool renormalized = true;
        private bool[] differentialConvertedPair = null;
        private int numOfDifferentialPair = 0;
        private Point[] differentialPair;
        private bool[] differentialPairSelected;
        private string[] portIndex;
        
       

        public SParameter(TouchStoneFile tsf)
        {
            
            this.numOfPorts = tsf.getParameter().GetLength(0);
            this.frequency = tsf.getFreq();
            this.parameter = tsf.getParameter();
            this.previousz0 = new double[this.numOfPorts];
            this.currentz0 = new double[this.numOfPorts];
            this.differentialPairSelected = new bool[this.numOfPorts];
            this.portIndex = new string[this.numOfPorts];
            for (int i = 0; i < this.numOfPorts; i++)
            {
                this.previousz0[i] = tsf.getZ0();
                this.currentz0[i] = tsf.getZ0();
                this.differentialPairSelected[i] = false;
                this.portIndex[i] = (i + 1).ToString();
            }
        }

        public SParameter(CitiFile cf)
        {
            this.numOfPorts = cf.getParameter().GetLength(0);
            this.frequency = cf.getFreq();
            this.parameter = cf.getParameter();
            this.previousz0 = new double[this.numOfPorts];
            this.currentz0 = new double[this.numOfPorts];
            this.differentialPairSelected = new bool[this.numOfPorts];
            this.portIndex = new string[this.numOfPorts];
            for (int i = 0; i < this.numOfPorts; i++)
            {
                this.previousz0[i] = cf.getZ0();
                this.currentz0[i] = cf.getZ0();
                this.differentialPairSelected[i] = false;
                this.portIndex[i] = (i + 1).ToString();
            }
        }



        public SParameter(BasicComplex[,][] param, double[] freq, double[] z0)
        {
            this.numOfPorts = param.GetLength(0);
            this.frequency = freq;
            this.parameter = param;
            this.previousz0 = (double[])z0.Clone();
            this.currentz0 = (double[])z0.Clone();
            this.differentialPairSelected = new bool[this.numOfPorts];
            this.portIndex = new string[this.numOfPorts];

            for (int i = 0; i < this.numOfPorts; i++)
            {
                this.differentialPairSelected[i] = false;
                this.portIndex[i] = (i + 1).ToString();
            }


        }

        public SParameter(BasicComplex[,][] param, double[] freq, double z0)
        {
            this.parameter = param;
            this.frequency = freq;
            this.numOfPorts = param.GetLength(0);
            this.previousz0 = new double[this.numOfPorts];
            this.differentialPairSelected = new bool[this.numOfPorts];
            this.portIndex = new string[this.numOfPorts];

            for (int i = 0; i < this.numOfPorts; i++)
            {
                this.previousz0[i] = z0;
                this.currentz0[i] = z0;
                this.differentialPairSelected[i] = false;
                this.portIndex[i] = (i + 1).ToString();
            }
        }
        /// <summary>
        /// ポートのインピーダンスを指定値に変更します。
        /// </summary>
        /// <param name="numOfPorts">指定ポート(0～)</param>
        /// <param name="z">変更インピーダンス</param>
        public void setPortImpedance(int numOfPorts, double z) //port:0～指定
        {
            this.currentz0[numOfPorts] = z;
            this.renormalized = false;
        }

        /// <summary>
        /// ポートのインピーダンスを指定値に変更します。
        /// </summary>
        /// <param name="z">変更インピーダンス</param>
        public void setPortImpedance(double[] z)
        {
            if (z.Length != this.currentz0.Length)
            {
                throw new ArrayTypeMismatchException("基準インピーダンスを設定できません。");
            }
            else
            {
                this.currentz0 = (double[])z.Clone();
                this.renormalized = false;
            }
        }

        /// <summary>
        /// 全てのポートのインピーダンスを指定値に変更します。
        /// </summary>
        /// <param name="z">変更インピーダンス</param>
        public void setAllPortImpedance(double z)
        {
            for (int i = 0; i < this.numOfPorts; i++)
            {
                this.currentz0[i] = z;
            }
            this.renormalized = false;

        }
        /// <summary>
        /// 指定ポート(ポート番号は0から指定)を差動ペアに指定します。(変換後、番号の若い方からcommon[差動ペア数],differential[差動ペア数])
        /// 指定後、differentialConversionメソッドで変換完了します。
        /// </summary>
        /// <param name="a">差動変換ポート(1)</param>
        /// <param name="b">差動変換ポート(2)</param>
        public void addDifferentialPair(int a, int b)　//a,b:0～指定
        {
            if (((!this.differentialPairSelected[a]) || (!this.differentialPairSelected[b])) && (a != b))
            {
                if (a > b)
                {
                    int dummya = a;
                    int dummyb = b;
                    a = dummyb;
                    b = dummya;
                }

                if (this.numOfDifferentialPair == 0)
                {
                    this.numOfDifferentialPair++;
                    this.differentialPair = new Point[numOfDifferentialPair];
                    this.differentialConvertedPair = new bool[numOfDifferentialPair];

                    this.differentialPair[numOfDifferentialPair - 1] = new Point(a, b);
                    this.differentialConvertedPair[numOfDifferentialPair - 1] = false;

                    this.portIndex[a] = "c" + this.numOfDifferentialPair.ToString();
                    this.portIndex[b] = "d" + this.numOfDifferentialPair.ToString();
                    for (int i = 0; i < this.portIndex.Length; i++)
                    {
                        if ((i != a) && (i != b))
                        {
                            this.portIndex[i] = "s" + this.portIndex[i];
                        }
                    }

                    this.differentialPairSelected[a] = true;
                    this.differentialPairSelected[b] = true;


                }
                else
                {
                    this.numOfDifferentialPair++;
                    Array.Resize(ref this.differentialPair, this.numOfDifferentialPair);
                    Array.Resize(ref this.differentialConvertedPair, this.numOfDifferentialPair);
                    this.differentialPair[numOfDifferentialPair - 1] = new Point(a, b);
                    this.differentialConvertedPair[numOfDifferentialPair - 1] = false;

                    this.portIndex[a] = "c" + this.numOfDifferentialPair.ToString();
                    this.portIndex[b] = "d" + this.numOfDifferentialPair.ToString();

                    this.differentialPairSelected[a] = true;
                    this.differentialPairSelected[b] = true;


                }
            }
        }


        /// <summary>
        /// addDifferentialPairメソッドで指定したペアを差動変換します。
        /// </summary>
        public void differentialConversion()
        {

            ComplexMatrix[] dummyMat = this.getParameterMatrix();
            BasicComplex[][,] dummy = new BasicComplex[dummyMat.Length][,];

            for (int i = 0; i < this.numOfDifferentialPair; i++)
            {
                if (!this.differentialConvertedPair[i])
                {
                    int a = this.differentialPair[i].X;
                    int b = this.differentialPair[i].Y;

                    ComplexMatrix convertMatrix = ComplexMatrix.elementaryMatrix(dummyMat[0].getMatrix().GetLength(0));
                    convertMatrix.setValue(a, a, new BasicComplex((1 / Math.Sqrt(2)), 0));
                    convertMatrix.setValue(a, b, new BasicComplex((1 / Math.Sqrt(2)), 0));
                    convertMatrix.setValue(b, a, new BasicComplex((1 / Math.Sqrt(2)), 0));
                    convertMatrix.setValue(b, b, new BasicComplex((-1 / Math.Sqrt(2)), 0));

                    for (int j = 0; j < dummyMat.Length; j++)
                    {
                        dummyMat[j] = convertMatrix * dummyMat[j] * convertMatrix;
                    }
                    this.differentialConvertedPair[i] = true;
                }
            }

            for (int i = 0; i < dummyMat.Length; i++)
            {
                dummy[i] = dummyMat[i].getMatrix();
            }

            this.parameter = BasicComplex.changeArray12to21(dummy);
        }

        /// <summary>
        /// インピーダンス変更を行ったSパラメータを変換します。
        /// </summary>
        public void renormalize()
        {
            int NumOfPorts = this.parameter.GetLength(0);
            int NumOfFreq = this.parameter[0, 0].GetLength(0);
            ComplexMatrix[] S = this.getParameterMatrix();
            ComplexMatrix[] Sprime = new ComplexMatrix[NumOfFreq];
            ComplexMatrix W = this.getWMatrixForRenormalize(this.previousz0, this.currentz0);
            ComplexMatrix Winv = ComplexMatrix.inverseMatrix(W);
            ComplexMatrix Gamma = this.getGammaMatrixForRenormalize(this.previousz0, this.currentz0);

            BasicComplex[][,] renormalizedParameter = new BasicComplex[NumOfFreq][,];

            if (this.renormalized == false)
            {
                for (int i = 0; i < NumOfFreq; i++)
                {
                    renormalizedParameter[i] = new BasicComplex[NumOfPorts, NumOfPorts];
                    Sprime[i] = Winv * (S[i] - Gamma) * ComplexMatrix.inverseMatrix((ComplexMatrix.elementaryMatrix(NumOfPorts) - Gamma * S[i])) * W;
                    renormalizedParameter[i] = (BasicComplex[,])Sprime[i].getMatrix().Clone();

                }
                this.parameter = BasicComplex.changeArray12to21(renormalizedParameter);
                this.renormalized = true;
            }

        }

        /// <summary>
        /// 指定ポートをOpenまたはShortとした場合の(n-1)*(n-1)Sパラメータを返します。
        /// </summary>
        /// <param name="PortNum">指定ポート(0-指定)</param>
        /// <param name="opened">指定ポートを(true:open,false:short)する</param>
        /// <returns></returns>
        public SParameter getOpenedOrShortedSParameter(int PortNum, bool opened) //portNumber:配列番号で指定
        {
            //各種操作用配列
            ComplexMatrix[] ParameterMat = this.getParameterMatrix();
            ComplexMatrix[] newParameterMat = new ComplexMatrix[ParameterMat.Length];
            BasicComplex[][,] newParameter = new BasicComplex[ParameterMat.Length][,];
            int ports = ParameterMat[0].getMatrix().GetLength(0);


            //open or short Portの値を拾う
            for (int i = 0; i < ParameterMat.Length; i++)
            {
                BasicComplex SselectPort;
                if (opened)
                {
                    SselectPort = ParameterMat[i].getMatrix()[PortNum, PortNum] - 1;
                }
                else
                {
                    SselectPort = ParameterMat[i].getMatrix()[PortNum, PortNum] + 1;
                }
                ParameterMat[i].setValue(PortNum, PortNum, SselectPort);


                for (int j = 0; j < ports; j++)
                {
                    if (j != PortNum)
                    {
                        BasicComplex sj = ParameterMat[i].getMatrix()[j, PortNum];
                        ParameterMat[i] = ComplexMatrix.rowBMinusRowA(ParameterMat[i], PortNum, j, sj / SselectPort);
                    }
                }

                newParameterMat[i] = ComplexMatrix.deleteRowAndColumn(ParameterMat[i], PortNum);
                newParameter[i] = newParameterMat[i].getMatrix();
            }

            double[] newSz0 = new double[this.previousz0.Length - 1];
            bool[] newDifferentialConvertedPair;


            if (this.differentialConvertedPair != null)
            {
                newDifferentialConvertedPair = new bool[this.differentialConvertedPair.Length - 1];
            }
            else
            {
                newDifferentialConvertedPair = null;
            }

            int newNumOfDifferentialPair = this.numOfDifferentialPair;
            Point[] newDifferentialPair = (Point[])this.differentialPair.Clone();
            bool[] newDifPairSelected = new bool[this.differentialPairSelected.Length - 1];
            string[] newPortNumberIndex = new string[this.portIndex.Length - 1];

            for (int i = 0; i < this.numOfPorts; i++)
            {

                if (i < PortNum)
                {
                    newSz0[i] = this.previousz0[i];
                    if (differentialConvertedPair != null)
                    {
                        newDifferentialConvertedPair[i] = this.differentialConvertedPair[i];
                    }
                    newDifPairSelected[i] = this.differentialPairSelected[i];
                    newPortNumberIndex[i] = this.portIndex[i];
                }
                else if (i > PortNum)
                {
                    newSz0[i - 1] = this.previousz0[i];
                    if (differentialConvertedPair != null)
                    {
                        newDifferentialConvertedPair[i - 1] = this.differentialConvertedPair[i];
                    }
                    newDifPairSelected[i - 1] = this.differentialPairSelected[i];
                    newPortNumberIndex[i - 1] = this.portIndex[i];
                }
            }
            if (this.differentialPair != null)
            {
                for (int i = 0; i < this.differentialPair.Length; i++)
                {
                    if (newDifferentialPair[i].X > PortNum)
                    {
                        newDifferentialPair[i].X--;
                        newDifferentialPair[i].Y--;
                    }
                    else if (newDifferentialPair[i].Y > PortNum)
                    {
                        newDifferentialPair[i].Y--;
                    }
                }
            }


            SParameter newS = new SParameter(BasicComplex.changeArray12to21(newParameter), this.frequency, newSz0);

            newS.currentz0 = newSz0;
            newS.differentialConvertedPair = newDifferentialConvertedPair;
            newS.differentialPair = newDifferentialPair;
            newS.numOfDifferentialPair = newNumOfDifferentialPair;
            newS.differentialPairSelected = newDifPairSelected;
            newS.portIndex = newPortNumberIndex;

            return newS;
        }


        private ComplexMatrix[] getParameterMatrix()
        {
            int freqPoints = this.parameter[0, 0].GetLength(0);
            int ports = this.parameter.GetLength(0);
            ComplexMatrix[] dummy = new ComplexMatrix[freqPoints];
            BasicComplex[][,] dummyd = BasicComplex.changeArray21to12(this.parameter);
            for (int i = 0; i < freqPoints; i++)
            {
                dummy[i] = new ComplexMatrix(dummyd[i]);
            }
            return dummy;
        }


        private ComplexMatrix getWMatrixForRenormalize(double[] previousz0, double[] currentz0)
        {
            int freqPoints = this.parameter[0, 0].GetLength(0);
            int ports = this.parameter.GetLength(0);
            ComplexMatrix dummy;
            BasicComplex[,] dummyd = new BasicComplex[ports, ports];

            for (int j = 0; j < ports; j++)
            {
                for (int k = 0; k < ports; k++)
                {
                    if (j == k)
                    {
                        dummyd[k, j] = new BasicComplex(2 * Math.Sqrt(previousz0[k] * currentz0[k]) / (previousz0[k] + currentz0[k]), 0);
                    }
                    else
                    {
                        dummyd[k, j] = new BasicComplex(0, 0);
                    }
                }
            }
            dummy = new ComplexMatrix(dummyd);

            return dummy;

        }


        private ComplexMatrix getGammaMatrixForRenormalize(double[] previousz0, double[] currentz0)
        {

            int ports = this.parameter.GetLength(0);
            ComplexMatrix dummy;
            BasicComplex[,] dummyd = new BasicComplex[ports, ports];


            for (int j = 0; j < ports; j++)
            {
                for (int k = 0; k < ports; k++)
                {
                    if (j == k)
                    {
                        dummyd[k, j] = new BasicComplex((currentz0[k] - previousz0[k]) / (currentz0[k] + previousz0[k]), 0);
                    }
                    else
                    {
                        dummyd[k, j] = new BasicComplex(0, 0);
                    }
                }
            }
            dummy = new ComplexMatrix(dummyd);

            return dummy;

        }

        /// <summary>
        /// Sパラメータを返します。
        /// </summary>
        /// <returns></returns>
        public BasicComplex[,][] getParameter()
        {
            return this.parameter;
        }

        /// <summary>
        /// 指定ポートのSパラメータをdB値で返します。
        /// </summary>
        /// <param name="portNumOfOutput">出力ポート</param>
        /// <param name="portNumOfInput">入力ポート</param>
        /// <returns></returns>
        public double[] getdBParameter(int portNumOfOutput, int portNumOfInput)
        {
            double[] dummy = new double[this.parameter[0, 0].Length];
            for (int i = 0; i < this.parameter[0, 0].Length; i++)
            {
                dummy[i] = this.parameter[portNumOfOutput, portNumOfInput][i].dbMag();
            }
            return dummy;
        }

        /// <summary>
        /// 周波数配列を返します。
        /// </summary>
        /// <returns></returns>
        public double[] getFrequency()
        {
            return this.frequency;
        }

        /// <summary>
        /// ポート番号のインデックスを返します。
        /// </summary>
        /// <returns></returns>
        public string[] getPortNumberIndex()
        {
            return this.portIndex;
        }

        /// <summary>
        /// ポート数を返します。
        /// </summary>
        /// <returns></returns>
        public int getNumOfPorts()
        {
            return this.numOfPorts;
        }

        /// <summary>
        /// SパラメータをCSVに変換して出力します。
        /// </summary>
        /// <param name="file">ファイルパス</param>
        public void writeCsvFile(string file)
        {
            StreamWriter sw = new StreamWriter(file);
            string index = "freq[GHz]";
            for (int j = 0; j < this.numOfPorts; j++)
            {
                for (int k = 0; k < this.numOfPorts; k++)
                {
                    string sIndex = "S" + portIndex[j].Substring(0, 1) + portIndex[k].Substring(0, 1) + portIndex[j].Substring(1, 1) + portIndex[k].Substring(1, 1);
                    index = index + "," + sIndex + " real" + "," + sIndex + " image" + "," + sIndex + " [dB]";
                }
            }
            sw.WriteLine(index);

            for (int i = 0; i < this.frequency.Length; i++)
            {
                string dummy = (this.frequency[i] / 1E9).ToString();
                for (int j = 0; j < this.numOfPorts; j++)
                {
                    for (int k = 0; k < this.numOfPorts; k++)
                    {
                        dummy = dummy + "," + parameter[j, k][i].getReal().ToString() + "," + parameter[j, k][i].getImage().ToString() + "," + parameter[j, k][i].dbMag().ToString();
                    }
                }
                sw.WriteLine(dummy);
            }
            sw.Flush();
            sw.Close();
        }

        public TouchStoneFile returnTouchStone()
        {
            return new TouchStoneFile(this.frequency, this.parameter, this.currentz0[0]);
        }

        private bool differentImpedanceEachPort()
        {
            ArrayList list = new ArrayList();
            for (int i = 0; i < currentz0.Length; i++)
            {
                if (list.Contains(currentz0[i]))
                {
                }
                else
                {
                    list.Add(currentz0[i]);
                }
            }
            if (list.ToArray().Length != 1) return true;
            else return false;
        }





        /// <summary>
        /// Sパラメータをtouchstoneに変換して出力します。
        /// </summary>
        /// <param name="file">ファイルパス</param>
        public void writeTouchStoneFile(string file)
        {
            if (numOfDifferentialPair == 0)
            {
                if (differentImpedanceEachPort() == false)
                {
                    TouchStoneFile tsf = new TouchStoneFile(this.frequency, this.parameter, this.currentz0[0]);
                    tsf.writeTouchstoneFile(file);
                }
                else
                {
                    StreamWriter sw = new StreamWriter(file);

                    for (int i = 0; i < this.numOfPorts; i++)
                    {
                        sw.WriteLine("!" + "port[" + (i + 1).ToString() + "]:" + this.currentz0[i] + "Ω");
                    }


                    sw.WriteLine("# GHz " + "S" + " RI R " + this.currentz0[0].ToString());



                    if (this.numOfPorts == 2)
                    {
                        for (int i = 0; i < this.frequency.Length; i++)
                        {
                            string dummy = (this.frequency[i] / 1E9).ToString();
                            dummy = dummy + " " + parameter[0, 0][i].getReal().ToString() + " " + parameter[0, 0][i].getImage().ToString();
                            dummy = dummy + " " + parameter[1, 0][i].getReal().ToString() + " " + parameter[1, 0][i].getImage().ToString();
                            dummy = dummy + " " + parameter[0, 1][i].getReal().ToString() + " " + parameter[0, 1][i].getImage().ToString();
                            dummy = dummy + " " + parameter[1, 1][i].getReal().ToString() + " " + parameter[1, 1][i].getImage().ToString();

                            sw.WriteLine(dummy);
                        }
                    }
                    else if (this.numOfPorts <= 4)
                    {

                        for (int i = 0; i < this.frequency.Length; i++)
                        {
                            for (int j = 0; j < this.numOfPorts; j++)
                            {
                                string dummy = "";
                                if (j == 0) dummy = (this.frequency[i] / 1E9).ToString();
                                for (int k = 0; k < this.numOfPorts; k++)
                                {
                                    dummy = dummy + " " + parameter[j, k][i].getReal().ToString() + " " + parameter[j, k][i].getImage().ToString();
                                }
                                sw.WriteLine(dummy);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < this.frequency.Length; i++)
                        {
                            for (int j = 0; j < this.numOfPorts; j++)
                            {
                                string dummy = "";
                                if (j == 0) dummy = (this.frequency[i] / 1E9).ToString();
                                for (int k = 0; k < this.numOfPorts; k++)
                                {
                                    dummy = dummy + " " + parameter[j, k][i].getReal().ToString() + " " + parameter[j, k][i].getImage().ToString();
                                    if ((k + 1) % 4 == 0) dummy = dummy + "\n";
                                }
                                sw.WriteLine(dummy);
                            }
                        }
                    }
                    sw.Flush();
                    sw.Close();

                }
            }

            else
            {
                StreamWriter sw = new StreamWriter(file);


                for (int i = 0; i < this.numOfPorts; i++)
                {
                    sw.WriteLine("!" + "port[" + (i + 1).ToString() + "]:" + this.portIndex[i]);
                }

                if (differentImpedanceEachPort() == true)
                {
                    for (int i = 0; i < this.numOfPorts; i++)
                    {
                        sw.WriteLine("!" + "port[" + (i + 1).ToString() + "]:" + this.currentz0[i] + "Ω");
                    }
                }

                sw.WriteLine("# GHz " + "S" + " RI R " + this.currentz0[0].ToString());



                if (this.numOfPorts == 2)
                {
                    for (int i = 0; i < this.frequency.Length; i++)
                    {
                        string dummy = (this.frequency[i] / 1E9).ToString();
                        dummy = dummy + " " + parameter[0, 0][i].getReal().ToString() + " " + parameter[0, 0][i].getImage().ToString();
                        dummy = dummy + " " + parameter[1, 0][i].getReal().ToString() + " " + parameter[1, 0][i].getImage().ToString();
                        dummy = dummy + " " + parameter[0, 1][i].getReal().ToString() + " " + parameter[0, 1][i].getImage().ToString();
                        dummy = dummy + " " + parameter[1, 1][i].getReal().ToString() + " " + parameter[1, 1][i].getImage().ToString();

                        sw.WriteLine(dummy);
                    }
                }
                else if (this.numOfPorts <= 4)
                {

                    for (int i = 0; i < this.frequency.Length; i++)
                    {
                        for (int j = 0; j < this.numOfPorts; j++)
                        {
                            string dummy = "";
                            if (j == 0) dummy = (this.frequency[i] / 1E9).ToString();
                            for (int k = 0; k < this.numOfPorts; k++)
                            {
                                dummy = dummy + " " + parameter[j, k][i].getReal().ToString() + " " + parameter[j, k][i].getImage().ToString();
                            }
                            sw.WriteLine(dummy);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < this.frequency.Length; i++)
                    {
                        for (int j = 0; j < this.numOfPorts; j++)
                        {
                            string dummy = "";
                            if (j == 0) dummy = (this.frequency[i] / 1E9).ToString();
                            for (int k = 0; k < this.numOfPorts; k++)
                            {
                                dummy = dummy + " " + parameter[j, k][i].getReal().ToString() + " " + parameter[j, k][i].getImage().ToString();
                                if ((k + 1) % 4 == 0) dummy = dummy + "\n";
                            }
                            sw.WriteLine(dummy);
                        }
                    }
                }
                sw.Flush();
                sw.Close();
            }

        }
        public double[] getZ0()
        {
            return this.currentz0;
        }

        public SParameter Clone() 
        {
            return (SParameter)this.MemberwiseClone();
        }
    }

}
