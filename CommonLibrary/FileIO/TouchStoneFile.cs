using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CommonLibrary.Basic;

namespace CommonLibrary.FileIO
{
    public class TouchStoneFile
    {
        private BasicComplex[,][] parameter;
        private double[] freq;
        private double z0;
        private int portNum;
        string parameterType;

        public TouchStoneFile(double[] freq, BasicComplex[,][] spara, double z0)
        {
            this.parameter = spara;
            this.freq = freq;
            this.z0 = z0;
            this.parameterType = "s";
            if (parameter.GetLength(0) != parameter.GetLength(1)) throw new System.ArrayTypeMismatchException("行列が正方行列ではありません");
            else this.portNum = parameter.GetLength(0);
        }

        public TouchStoneFile(String touchStone,int portNum)
        {
            this.portNum = portNum;
            StreamReader sr = new StreamReader(touchStone);
            string dummy;
            int count = 0;
            do
            {
                dummy = sr.ReadLine();
            }
            while (!dummy.Trim().StartsWith("#"));
            char[] charSeparators = new char[] { ' ','\t' };
            String[] dummy2 = dummy.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            string[] strLine = sr.ReadToEnd().Split('\n');
            sr.Close();
            for (int i = 0; i < strLine.Length; i++)
            {
                strLine[i] = strLine[i].Trim();
                if (!strLine[i].StartsWith("!") && strLine[i]!="") count++;
            }
            this.z0 = double.Parse(dummy2[5]);
            string freqType = dummy2[1].Trim().ToLower();
            this.parameterType = dummy2[2].Trim().ToLower();
            string dataType = dummy2[3].Trim().ToLower();

            double k0=1;
            if (freqType.Equals("khz"))k0=1E3;
            else if (freqType.Equals("mhz")) k0 = 1E6;
            else if (freqType.Equals("ghz")) k0 = 1E9;

            int numOfRowPerFreq;
            if (portNum == 2 | portNum == 1) numOfRowPerFreq=1;
            else if (3 <= portNum && portNum <= 4) numOfRowPerFreq=portNum;
            else numOfRowPerFreq=(int)Math.Ceiling((double)portNum / 4.0)*portNum;
            int numOfData=count/numOfRowPerFreq;

            BasicComplex[,,] parameter2 = new BasicComplex[numOfData, portNum, portNum];
            freq = new double[numOfData];

            string[] rawData=new string[count];
            int iDummy=0;
            for (int i = 0; i < strLine.Length; i++)
            {
                if (!strLine[i].StartsWith("!") && strLine[i] != "")
                {
                    rawData[iDummy] = strLine[i];
                    iDummy++;
                }
            }

            for (int i = 0; i < numOfData; i++)
            {
                if (portNum == 2)
                {
                    String[] dummyb = rawData[i].Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                    freq[i] = double.Parse(dummyb[0])*k0;
                    parameter2[i, 0, 0] = new BasicComplex(double.Parse(dummyb[1]), double.Parse(dummyb[2]), dataType);
                    parameter2[i, 1, 0] = new BasicComplex(double.Parse(dummyb[3]), double.Parse(dummyb[4]), dataType);
                    parameter2[i, 0, 1] = new BasicComplex(double.Parse(dummyb[5]), double.Parse(dummyb[6]), dataType);
                    parameter2[i, 1, 1] = new BasicComplex(double.Parse(dummyb[7]), double.Parse(dummyb[8]), dataType);
                }
                else
                {
                    for (int j = 0; j < portNum; j++)
                    {
                        iDummy = 0;
                        for (int k = 0; k < numOfRowPerFreq/portNum; k++)
                        {
//                            String[] dummya = rawData[i * numOfRowPerFreq + j * portNum + k].Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                            int dummyIndex = i * numOfRowPerFreq + j * numOfRowPerFreq / portNum + k;
                            String[] dummya = rawData[dummyIndex].Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                            for (int l = 0; l < dummya.Length / 2; l++)
                            {
                                if (j == 0 && k == 0)
                                {
                                    this.freq[i] = double.Parse(dummya[0])*k0;
                                    parameter2[i, j, iDummy] = new BasicComplex(double.Parse(dummya[2 * l + 1]), double.Parse(dummya[2 * l + 2]), dataType);
                                    iDummy++;
                                }
                                else
                                {
//                                    if (iDummy>=portNum)
                                    parameter2[i, j, iDummy] = new BasicComplex(double.Parse(dummya[2 * l]), double.Parse(dummya[2 * l + 1]), dataType);
                                    iDummy++;
                                }
                            }
                        }
                    }
                }
            }
            parameter = new BasicComplex[parameter2.GetLength(1), parameter2.GetLength(2)][];
            for (int i = 0; i < parameter2.GetLength(1); i++)
            {
                for (int j = 0; j < parameter2.GetLength(2); j++)
                {
                    parameter[i, j] = new BasicComplex[parameter2.GetLength(0)];
                    for (int k = 0; k < parameter2.GetLength(0); k++)
                    {
                        parameter[i, j][k] = new BasicComplex(parameter2[k, i, j]);
                    }
                }
            }
        }
        public void writeCsvFile(string file)
        {
            StreamWriter sw = new StreamWriter(file);
            string index = "freq[GHz]";
            for (int j = 0; j < portNum; j++)
            {
                for (int k = 0; k < portNum; k++)
                {
                    string sIndex = "S" + (j + 1).ToString() + (k + 1).ToString();
                    index = index + "," + sIndex + " real" + "," + sIndex + " image" + "," + sIndex + " [dB]";
                }
            }
            sw.WriteLine(index);

            for (int i = 0; i < freq.Length; i++)
            {
                string dummy = (freq[i]/1E9).ToString();
                for (int j = 0; j < portNum; j++)
                {
                    for (int k = 0; k < portNum; k++)
                    {
                        dummy = dummy + "," + parameter[j, k][i].getReal().ToString() + "," + parameter[j, k][i].getImage().ToString() + "," + parameter[j, k][i].dbMag().ToString();
                    }
                }
                sw.WriteLine(dummy);
            }
            sw.Flush();
            sw.Close();
        }
        public BasicComplex[,][] getParameter()
        {
            return this.parameter;
        }
        public double[] getFreq()
        {
            return this.freq;
        }
        public double getZ0()
        {
            return this.z0;
        }
        public void writeTouchstoneFile(string file)
        {
            StreamWriter sw = new StreamWriter(file);
            sw.WriteLine("# GHz " + this.parameterType.ToUpper() + " RI R " + this.z0.ToString());
            if (this.portNum == 2)
            {
                for (int i = 0; i < freq.Length; i++)
                {
                    string dummy = (freq[i]/1E9).ToString();
                    dummy = dummy + " " + parameter[0, 0][i].getReal().ToString() + " " + parameter[0, 0][i].getImage().ToString();
                    dummy = dummy + " " + parameter[1, 0][i].getReal().ToString() + " " + parameter[1, 0][i].getImage().ToString();
                    dummy = dummy + " " + parameter[0, 1][i].getReal().ToString() + " " + parameter[0, 1][i].getImage().ToString();
                    dummy = dummy + " " + parameter[1, 1][i].getReal().ToString() + " " + parameter[1, 1][i].getImage().ToString();

                    sw.WriteLine(dummy);
                }
            }
            else if (this.portNum <= 4)
            {
                for (int i = 0; i < freq.Length; i++)
                {
                    for (int j = 0; j < portNum; j++)
                    {
                        string dummy = "";
                        if (j == 0) dummy = (freq[i] / 1E9).ToString();
                        for (int k = 0; k < portNum; k++)
                        {
                            dummy = dummy + " " + parameter[j, k][i].getReal().ToString() + " " + parameter[j, k][i].getImage().ToString();
                        }
                        sw.WriteLine(dummy);
                    }
                }
            }
            else
            {
                for (int i = 0; i < freq.Length; i++)
                {
                    for (int j = 0; j < portNum; j++)
                    {
                        string dummy = "";
                        if (j == 0) dummy = (freq[i] / 1E9).ToString();
                        for (int k = 0; k < portNum; k++)
                        {
                            dummy = dummy + " " + parameter[j, k][i].getReal().ToString() + " " + parameter[j, k][i].getImage().ToString();
                            if ((k+1) % 4 == 0) dummy = dummy + "\n";
                        }
                        sw.WriteLine(dummy);
                    }
                }
            }
            sw.Flush();
            sw.Close();
        }






    }
}
