using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CommonLibrary.Basic;

namespace CommonLibrary.FileIO
{
    public class CitiFile
    {
        private BasicComplex[,][] parameter;
        private double[] freq;
        private double z0;
        private int portNum;
//        string parameterType;
        public CitiFile(double[] freq, BasicComplex[,][] spara, double z0)
        {
            this.freq = freq;
            this.parameter = spara;
            this.z0 = z0;
            if (parameter.GetLength(1) != parameter.GetLength(2)) throw new System.ArrayTypeMismatchException("行列が正方行列ではありません");
            else this.portNum = parameter.GetLength(1);
        }
        public CitiFile(string file)
        {
            StreamReader sr = new StreamReader(file);
            char[] charSeparators = new char[] { ' ','[' };
            string[] strLine = sr.ReadToEnd().Split('\n');
            portNum=0;
            int numOfPoint=0;
            int countDummy=0;
            int freqStartPoint = 0;
            double fstart=0;
            double fstop=0;
            string dataType="";
            for (int i = 0; i < strLine.Length; i++) if (strLine[i].Trim() != "") countDummy++;
            string[] strLine2 = new string[countDummy];

            //スペースしかない行や、改行コードのみの行を除去する。
            countDummy = 0;
            this.z0 = 50;
            for (int i = 0; i < strLine.Length; i++) 
            {
                string dummy=strLine[i].Trim().ToLower();
                if (dummy != "")
                {
                    strLine2[countDummy]=dummy;
                    countDummy++;
                }
            }


            for (int i=0;i<strLine2.Length;i++)
            {
                string dummy=strLine2[i].Trim().ToLower();
                if (dummy.StartsWith("data"))
                {
                    portNum++;
                    string[] dummy2 = strLine2[i].Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                    dataType = dummy2[2];
                }

                else if (dummy == "var_list_begin") freqStartPoint = i + 1;
                else if (dummy == "var_list_end") numOfPoint = i - freqStartPoint;

                else if (dummy == "seg_list_begin")
                {
                    string[] dummy2 = strLine2[i + 1].Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                    fstart = double.Parse(dummy2[1]);
                    fstop = double.Parse(dummy2[2]);
                    numOfPoint = int.Parse(dummy2[3]);
                }
            }

            portNum = (int)Math.Sqrt((double)portNum);
            freq = new double[numOfPoint];
//            parameter = new BasicComplex[numOfPoint, portNum, portNum];
            parameter = new BasicComplex[portNum, portNum][];
            if (freqStartPoint != 0) for (int i = 0; i < numOfPoint; i++) freq[i] = double.Parse(strLine2[freqStartPoint + i]);
            else for (int i = 0; i < numOfPoint; i++) freq[i] = fstart + (fstop - fstart) / (numOfPoint - 1) * i;
            int[] dataIndex = new int[portNum*portNum];
            int[] arrayIndexA = new int[portNum * portNum];
            int[] arrayIndexB = new int[portNum * portNum];
            int iDummyA=0;
            int iDummyC=0;
            for (int i = 0; i < strLine2.Length; i++)
            {
                if (strLine2[i].StartsWith("begin"))
                {
                    dataIndex[iDummyA] = i + 1;
                    iDummyA++;
                }
                else if (strLine2[i].StartsWith("data"))
                {
                    string[] dummy = strLine2[i].Split('[', ',', ']');
                    arrayIndexA[iDummyC] = int.Parse(dummy[1])-1;
                    arrayIndexB[iDummyC] = int.Parse(dummy[2])-1;
                    iDummyC++;
                }

            }
            for (int i = 0; i < portNum * portNum; i++)
            {
                parameter[arrayIndexA[i], arrayIndexB[i]] = new BasicComplex[numOfPoint];

                for (int j = 0; j < numOfPoint; j++)
                {
                    string[] dummy = strLine2[dataIndex[i]+j].Split(',');
                    parameter[arrayIndexA[i], arrayIndexB[i]][j] = new BasicComplex(double.Parse(dummy[0]), double.Parse(dummy[1]));
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
                string dummy = (freq[i] / 1E9).ToString();
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
        public void writeCitiFile(string file)
        {
            StreamWriter sw = new StreamWriter(file);
            sw.WriteLine("CITIFILE A.01.00");
            for (int i = 0; i < portNum; i++) for (int j = 0; j < portNum; j++) sw.WriteLine("DATA S[" + (i + 1).ToString() + "," + (j + 1).ToString() + "] RI");
            sw.WriteLine("VAR_LIST_BEGIN");
            for (int i = 0; i < freq.Length; i++) sw.WriteLine(freq[i].ToString());
            sw.WriteLine("VAR_LIST_END");
            for (int i = 0; i < portNum; i++)
            {
                for (int j = 0; j < portNum; j++)
                {
                    sw.WriteLine("BEGIN");
                    for (int k = 0; k < freq.Length; k++)
                    {
                        sw.WriteLine(parameter[j, k][i].getReal().ToString() + "," + parameter[j, k][i].getImage().ToString());
                    }
                    sw.WriteLine("END");
                }
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
            return freq;
        }
        public int getPortNum()
        {
            return this.portNum;
        }
        public double getZ0()
        {
            return this.z0;
        }

    }
}
