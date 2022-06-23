using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Basic
{
    public class MicrowaveFilter
    {

        private string filterCharacteristic;
        private string filterKind;
        private double bandWidth;
        private double f0;
        private double fCutoff;
        private int n;
     
        private double[] protoTypeFilterConst;
        private double[] frequency;
        private double ripple = 0.01;
        private SParameter sparam;

        public MicrowaveFilter(string chara,string kind) 
        {
            this.filterCharacteristic = chara;
            this.filterKind = kind;
        }

        public void setBandWidth(double bw)
        {
            this.bandWidth = bw;
        }

        public void setf0(double f0)
        {
            this.f0 = f0;
        }

        public void setfCutoff(double f)
        {
            this.fCutoff = f;
        }

        public void setN(int n) 
        {
            this.n = n;
            this.protoTypeFilterConst = new double[n + 2];
            if (this.filterCharacteristic == "f")
            {
                this.protoTypeFilterConst[0] = 1.0;
                this.protoTypeFilterConst[n + 1] = 1.0;
                for (int i = 1; i < n+1; i++)
                {
                    this.protoTypeFilterConst[i] = 2 * Math.Sin((2 * i - 1) * Math.PI / (2 * n));
                }
            }
            else if (this.filterCharacteristic == "c") 
            {
                double beta = Math.Log(1 / Math.Tanh(this.ripple / 17.37))/4;
                this.protoTypeFilterConst[0] = 1.0;
                this.protoTypeFilterConst[1] = 2 * Math.Sin(Math.PI / (2 * n)) / Math.Sinh(beta/(2*n));
                    
                if (n % 2 == 1)
                {
                    this.protoTypeFilterConst[n + 1] = 1.0;
                }
                else 
                {
                    this.protoTypeFilterConst[n + 1] =Math.Pow( 1 / Math.Tanh(beta),2);
                }

                for (int i = 1; i < n; i++)
                {
                    this.protoTypeFilterConst[i + 1] = 4 * Math.Sin((2 * i - 1) * Math.PI / (2 * n)) * Math.Sin((2 * i + 1) * Math.PI / (2 * n)) / ((Math.Pow((Math.Sinh(beta / (2 * n))), 2) + Math.Pow((Math.Sin(i*Math.PI/n)), 2)) * this.protoTypeFilterConst[i]);
                }

            }
        }

    }
}
