using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CommonLibrary.FileIO
{
    public class CsvRead
    {
        private double[] freq;
        private double[] sParam;

        public CsvRead(String CSV)
        {
            StreamReader sr = new StreamReader(CSV);

            string[] strLine = sr.ReadToEnd().Split('\n');

            
            sr.Close();

            freq = new double[strLine.GetLength(0)-2];
            sParam = new double[strLine.GetLength(0)-2];
            string[] dummy = new string[2];
            for (int i = 1; i < strLine.GetLength(0) -1; i++) 
            {
                dummy = strLine[i].Split(',');
                this.freq[i-1]=double.Parse(dummy[0]);
                this.sParam[i-1] = double.Parse(dummy[1].Replace("\n","")); 
            }
 
        }

        public double[] getFreq() 
        {
            return this.freq;
        }

        public double[] getSparam() 
        {
            return this.sParam;
        }
    }
}
