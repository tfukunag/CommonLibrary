using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Meta.Numerics;
using CommonLibrary.Basic;
using CommonLibrary.Measurement;
using CommonLibrary.Resonator;

namespace CommonLibrary.Transmission
{
    //矩形導波管のクラス(伝送路)
    public class RectangularWaveguide
    {
        private DielectricMaterial dem;
        private double width;
        private double height;

        public RectangularWaveguide(DielectricMaterial dem, double width, double height)
        {
            this.dem = dem;
            this.width = width;
            this.height = height;
        }

        public bool propagated(int n, int m, double f0)
        {
            double k0 = 2 * Math.PI * f0 / 2.99792458E8 * Math.Sqrt(dem.getEr());
            double kt = Math.Sqrt(Math.Pow(n * Math.PI / width, 2) + Math.Pow(m * Math.PI / height, 2));
            if (k0 * k0 - kt * kt > 0) return true;
            else return false;
        }

        public double cuttoffWavelength(int n, int m)
        {
            return 2 * width * height / Math.Sqrt(n * n * height * height + m * m * width * width)*Math.Sqrt(dem.getEr());
        }

        public double cutoffFreq(int n, int m)
        {
            return 2.99792458E8 / this.cuttoffWavelength(n, m);
        }
        public Complex gamma(int n, int m, double f0)
        {
            double k0 = 2 * Math.PI * f0 / 2.99792458E8 * Math.Sqrt(dem.getEr());
            double kt = Math.Sqrt(Math.Pow(n * Math.PI / width, 2) + Math.Pow(m * Math.PI / height, 2));
            double alpha;
            double beta;
            if (propagated(n, m, f0))
            {
                beta = Math.Sqrt(k0 * k0 - kt * kt);
                alpha = 0;
            }
            else
            {
                alpha = -Math.Sqrt(kt * kt - k0 * k0);
                beta = 0;
            }
            return new Complex(alpha, beta);
        }
        public Complex phase(int n, int m, double length, double f0)
        {
            return this.gamma(n, m, f0) * length;
        }
        public double waveLength(int n, int m, double f0)
        {
            if (this.propagated(n, m, f0)) return 2 * Math.PI / this.gamma(n, m, f0).Im;
            else return 0;
        }
    }
}
