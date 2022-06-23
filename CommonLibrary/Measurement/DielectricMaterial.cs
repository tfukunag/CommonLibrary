using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Meta.Numerics;
using CommonLibrary.Basic;
using CommonLibrary.Resonator;
using CommonLibrary.Transmission;

namespace CommonLibrary.Measurement
{
    //誘電体の定数のクラス
    public class DielectricMaterial {
        double er;
        double tand;
        public DielectricMaterial(double er, double tand)
        {
            this.er= er;
            this.tand = tand;
        }

        public DielectricMaterial(double er)
        {
            this.er = er;
        }

        public double getEr()
        {
            return this.er;
        }

        public double getTand()
        {
            return this.tand;
        }

        public void setTand(double tand)
        {
            this.tand=tand;
        }
    }
}
