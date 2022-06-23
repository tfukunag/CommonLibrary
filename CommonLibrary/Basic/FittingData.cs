using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.FileIO;

namespace CommonLibrary.Basic
{
    public class FittingData
    {
        private double[] freq;
        private double[] s21;

        public FittingData(double[] freq, double[] s21)
        {
            this.freq = freq;
            this.s21 = s21;
        }

        public FittingData(TouchStoneFile tsf)
        {
            this.freq = tsf.getFreq();
            BasicComplex[] dummy = tsf.getParameter()[1, 0];
            s21 = new double[dummy.Length];
            for (int i = 0; i < dummy.Length; i++) s21[i] = dummy[i].dbMag();
        }

        public int getIndexOfTargetFreq(double freq0)
        {
            int index = 0;
            for (int i = 1; i < freq.Length; i++) if (freq[i] < freq0) index = i;
            return index;
        }

        public int getFreqIndexOfMaxS21()
        {
            int index=0;
            double dummy=s21[0];
            for (int i = 1; i < s21.Length; i++)
            {
                if (dummy < s21[i])
                {
                    dummy = s21[i];
                    index = i;
                }
            }
            return index;
        }

        public int getLowerIndexOfTargetS21(double targetDb)
        {
            int index = 0;

            while (((s21[index] - targetDb) * (s21[index + 1] - targetDb) > 0) && (index <= s21.Length)) index++;
            return index;
        }

        public int getUpperIndexOfTargetS21(double targetDb)
        {
            int index = s21.Length-1;
            while (((s21[index] - targetDb) * (s21[index - 1] - targetDb) > 0) && (index <= s21.Length)) index--;
            return index;
        }

        public FittingData getSubFittingData(double f1, double f2)
        {
            int index1 = this.getIndexOfTargetFreq(f1);
            int index2 = this.getIndexOfTargetFreq(f2);
            int numOfData = index2 - index1 + 1;
            double[] x0 = new double[numOfData];
            double[] y0 = new double[numOfData];
            for (int i = 0; i < numOfData; i++)
            {
                x0[i] = freq[i + index1];
                y0[i] = s21[i + index1];
            }
            return new FittingData(x0, y0);
        }

        public FittingData getSubFittingData(int index1, int index2)
        {
            int numOfData = index2 - index1 + 1;
            double[] x0 = new double[numOfData];
            double[] y0 = new double[numOfData];
            for (int i = 0; i < numOfData; i++)
            {
                x0[i] = freq[i + index1];
                y0[i] = s21[i + index1];
            }
            return new FittingData(x0, y0);
        }

        public double[] getFreq()
        {
            return freq;
        }
        public double[] getS21()
        {
            return s21;
        }

        public double getFreqOfMaxS21()
        {
            int index = this.getFreqIndexOfMaxS21();
            return freq[index];
        }

        public double getMaxValueOfS21()
        {
            int index = this.getFreqIndexOfMaxS21();
            return s21[index];
        }

        public double getLowerFreqOfTargetS21(double targetDb)
        {
            int index = this.getLowerIndexOfTargetS21(targetDb);
            return freq[index];
        }

        public double getUpperFreqOfTargetS21(double targetDb)
        {
            int index = this.getUpperIndexOfTargetS21(targetDb);
            return freq[index];
        }

        public FittingData getFittingDataDbFromMax(double tagetDb)
        {
            double target = this.getMaxValueOfS21() - tagetDb;
            int index1 = this.getLowerIndexOfTargetS21(target);
            int index2 = this.getUpperIndexOfTargetS21(target);
            return this.getSubFittingData(index1, index2);
        }

        public FittingData getFittingDataOfLeftFromMax()
        {
            int max = this.getFreqIndexOfMaxS21();
            double[] x0 = new double[2 * max + 1];
            double[] y0 = new double[2 * max + 1];
            for (int i = 0; i < max + 1; i++)
            {
                x0[i] = freq[i];
                y0[i] = s21[i];
            }
            for (int i = 0; i < max; i++)
            {
                x0[max + 1 + i] = x0[max - 1 - i];
                y0[max + 1 + i] = y0[max - 1 - i];
            }
            return new FittingData(x0, y0);
        }

        public FittingData getFittingDataOfRightFromMax()
        {
            int max = this.getFreqIndexOfMaxS21();
            int numOfData = 2 * (freq.Length - max) + 1;
            double[] x0 = new double[numOfData];
            double[] y0 = new double[numOfData];
            for (int i = max; i < freq.Length; i++)
            {
                x0[i + freq.Length - 2*max+1] = freq[i];
                y0[i + freq.Length - 2*max+1] = s21[i];
            }
            for (int i = max-1; i < freq.Length; i++)
            {
                x0[i -max+1] = x0[-i + 2*freq.Length -  max - 1];
                y0[i -max+1] = y0[-i + 2*freq.Length -  max - 1];
            }
            return new FittingData(x0, y0);
        }

        public double[] initLorentzianCoefficientLowerSide(double att) 
        {
            double att1 = att;
            double att2 = att * 0.6;

            double freqOfMaxS21 = this.getFreqOfMaxS21();

            double f1 = this.getLowerFreqOfTargetS21(att1);
            double f2 = this.getLowerFreqOfTargetS21(att2);

            double y1 = this.s21[this.getLowerIndexOfTargetS21(att1)];
            double y2 = this.s21[this.getLowerIndexOfTargetS21(att2)];
            double yMax = this.getMaxValueOfS21();

            double[] dummy = new double[4];

            dummy[2] = freqOfMaxS21;
            dummy[3] = (y1 - y2) * (f1 - dummy[2]) * (f1 - dummy[2]) * (f2 - dummy[2]) * (f2 - dummy[2]) / ((f2 - dummy[2]) * (f2 - dummy[2]) * (yMax - y1) - (f1 - dummy[2]) * (f1 - dummy[2]) * (yMax - y2));
            dummy[1] = (f1 - dummy[2]) * (f1 - dummy[2]) / (yMax - y2);
            dummy[0] = yMax - dummy[1] / dummy[3];

            return dummy;
        }

        public double[] initLorentzianCoefficientUpperSide(double att)
        {
            double att1 = att;
            double att2 = att * 0.6;

            double freqOfMaxS21 = this.getFreqOfMaxS21();

            double f1 = this.getUpperFreqOfTargetS21(att1);
            double f2 = this.getUpperFreqOfTargetS21(att2);

            double y1 = this.s21[this.getUpperIndexOfTargetS21(att1)];
            double y2 = this.s21[this.getUpperIndexOfTargetS21(att2)];
            double yMax = this.getMaxValueOfS21();

            double[] dummy = new double[4];

            dummy[2] = freqOfMaxS21;
            dummy[3] = (y1 - y2) * (f1 - dummy[2]) * (f1 - dummy[2]) * (f2 - dummy[2]) * (f2 - dummy[2]) / ((f2 - dummy[2]) * (f2 - dummy[2]) * (yMax - y1) - (f1 - dummy[2]) * (f1 - dummy[2]) * (yMax - y2));
            dummy[1] = (f1 - dummy[2]) * (f1 - dummy[2]) / (yMax - y2);
            dummy[0] = yMax - dummy[1] / dummy[3];

            return dummy;
        }



    }
}
