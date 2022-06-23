using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Basic;
using CommonLibrary.Measurement;
using CommonLibrary.Transmission;


namespace CommonLibrary.Resonator
{
    //矩形共振器のクラス
    public class RectangularResonator
    {
        double width;
        double height;
        double length;
        DielectricMaterial dem;
        double sigma;
        public RectangularResonator(DielectricMaterial dem, double width, double height, double length)
        {
            this.width = width;
            this.height = height;
            this.length = length;
            this.dem = dem;
        }
        public RectangularResonator(DielectricMaterial dem, double sigma,double width, double height, double length)
        {
            this.width = width;
            this.height = height;
            this.length = length;
            this.dem = dem;
            this.sigma = sigma;
        }
        public double resonantFreq(int l, int m, int n)
        {
            return Math.Sqrt(Math.Pow(l*Math.PI/width,2)+Math.Pow(m*Math.PI/height,2)+Math.Pow(n*Math.PI/length,2))*2.99792458E8/Math.Sqrt(dem.getEr())/(2*Math.PI);
        }
        public double tmModeQValue(int l, int m, int n, double sigma)
        {
            double f0 = this.resonantFreq(l, m, n);
            double mu0 = 1.2566370614E-6;
            double ds = 1/Math.Sqrt(Math.PI * f0 * mu0*sigma);
            double a = this.width;
            double b = this.height;
            double c = this.length;
            double en = 2;
            if (n == 0) en = 1;
            double q0 = 1 / ds * (Math.Pow(l / a, 2) + Math.Pow(m / b, 2))  / (Math.Pow(l / a, 2) * (2 / a + en / c) + Math.Pow(m / b, 2) * (2 / b + en / c));
            return q0;
        }
        public double teModeQValue(int l, int m, int n, double sigma)
        {
            double f0 = this.resonantFreq(l, m, n);
            double mu0 = 4*Math.PI*1E-7;
            double ds = 1 / Math.Sqrt(Math.PI * f0 * mu0 * sigma);
            double a = this.width;
            double b = this.height;
            double c = this.length;
            double em = 2;
            if (m == 0) em = 1;
            double q0 = 1 / ds * (Math.Pow(l / a, 2) + Math.Pow(m / b, 2)) * (Math.Pow(l / a, 2) + Math.Pow(m / b, 2) + Math.Pow(n / c, 2)) / ((2 / a + em / b) * Math.Pow(Math.Pow(l / a, 2) + Math.Pow(m / b, 2), 2) + Math.Pow(n / c, 2) * ((em / b + 2 / c) * Math.Pow(l / a, 2) + 2 * (1 / a + 1 / c) * Math.Pow(m / b, 2)));
            return q0;
        }
        public double getTeModeSigma(int l, int m, int n, double qu)
        {
            double f0 = this.resonantFreq(l, m, n);
            double mu0 = 1.2566370614E-6;
            double a = this.width;
            double b = this.height;
            double c = this.length;
            double em = 2;
            if (m == 0) em = 1;
            double ds = 1 / qu * (Math.Pow(l / a, 2) + Math.Pow(m / b, 2)) * (Math.Pow(l / a, 2) + Math.Pow(m / b, 2) + Math.Pow(n / c, 2)) / ((2 / a + em / b) * Math.Pow(Math.Pow(l / a, 2) + Math.Pow(m / b, 2), 2) + Math.Pow(n / c, 2) * ((em / b + 2 / c) * Math.Pow(l / a, 2) + 2 * (1 / a + 1 / c) * Math.Pow(m / b, 2)));
            return 1 / (ds * ds * Math.PI * f0 * mu0);
        }
        public double getTmModeSigma(int l, int m, int n, double qu)
        {
            double f0 = this.resonantFreq(l, m, n);
            double mu0 = 1.2566370614E-6;
            double a = this.width;
            double b = this.height;
            double c = this.length;
            double en = 2;
            if (n == 0) en = 1;
            double ds = 1 / qu * (Math.Pow(l / a, 2) + Math.Pow(m / b, 2)) / (Math.Pow(l / a, 2) * (2 / a + en / c) + Math.Pow(m / b, 2) * (2 / b + en / c)) ;
            return 1 / (ds * ds * Math.PI * f0 * mu0);
        }
    }
}
