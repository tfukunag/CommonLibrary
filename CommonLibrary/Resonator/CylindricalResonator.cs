using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Basic;
using CommonLibrary.Measurement;
using CommonLibrary.Transmission;


namespace CommonLibrary.Resonator
{
    //円筒導波管共振器のクラス(導波管の両端短絡)
    public class CylindricalResonator
    {
        private double radius;
        private double height;
        private DielectricMaterial dem;
        private double[,] jpmn;
        private double[,] jmn;

        public CylindricalResonator(double radius, double height, DielectricMaterial dem)
        {
            this.radius = radius;
            this.height = height;
            this.dem = dem;
            jpmn = BesselZero.getJpmnMatrix();
            jmn = BesselZero.getJmnMatrix();
        }
        public double teModeResoFreq(int m, int n, int p)
        {
            double freq = Math.Sqrt(Math.Pow(jpmn[m, n - 1] / this.radius, 2) + Math.Pow(p * Math.PI / this.height, 2)) * 2.99792458E8 / (2 * Math.PI) / Math.Sqrt(dem.getEr());
            return freq;
        }

        public double tmModeResoFreq(int m, int n, int p)
        {
            double freq = Math.Sqrt(Math.Pow(jmn[m, n - 1] / this.radius, 2) + Math.Pow(p * Math.PI / this.height, 2)) * 2.99792458E8 / (2 * Math.PI) / Math.Sqrt(dem.getEr());
            return freq;
        }

        public double tmModeResoQValue(int m, int n, int p, double sigma)
        {
            double f0 = this.tmModeResoFreq(m, n, p);
            double mu0 = 4 * Math.PI * 1E-7;
            double ds = 1 / Math.Sqrt(Math.PI * f0 * mu0 * sigma);
            double ep = 2;
            if (p == 0) ep = 1;
            double qu = this.radius / ds / (1 + ep * this.radius / this.height);
            return qu;
        }

        public double getTmModeSigma(int m, int n, int p, double qu)
        {
            double f0 = this.tmModeResoFreq(m, n, p);
            double mu0 = 4 * Math.PI * 1E-7;
            //            double ds = 1 / Math.Sqrt(Math.PI * f0 * mu0 * sigma);
            double ep = 2;
            if (p == 0) ep = 1;
            double ds = this.radius / qu / (1 + ep * this.radius / this.height);
            return 1 / (ds * ds * Math.PI * f0 * mu0);
        }

        public double teModeResoQValue(int m, int n, int p, double sigma)
        {
            double f0 = this.teModeResoFreq(m, n, p);
            double mu0 = 4 * Math.PI * 1E-7;
            double ds = 1 / Math.Sqrt(Math.PI * f0 * mu0 * sigma);
            double upmn = jpmn[m, n - 1];
            double a = this.radius;
            double l = this.height;
            double qu = a * (upmn * upmn - m * m) * (upmn * upmn + Math.Pow(p * Math.PI * a / l, 2)) / ds;
            qu = qu / (Math.Pow(upmn, 4) + (upmn * upmn - m * m) * 2 * Math.Pow(Math.PI * p, 2) * a * a * a / l / l / l + Math.Pow(Math.PI * m * p * a / l, 2));
            return qu;
        }

        public double teModeResoTopQValue(int m, int n, int p, double sigma)
        {
            double f0 = this.teModeResoFreq(m, n, p);
            double mu0 = 4 * Math.PI * 1E-7;
            double ds = 1 / Math.Sqrt(Math.PI * f0 * mu0 * sigma);
            double upmn = jpmn[m, n - 1];
            double a = this.radius;
            double l = this.height;
            double qu = a * (upmn * upmn - m * m) * (upmn * upmn + Math.Pow(p * Math.PI * a / l, 2)) / ds;
            qu = qu / ((upmn * upmn - m * m) * 2 * Math.Pow(Math.PI * p, 2) * a * a * a / l / l / l) * 2;
            return qu;
        }

        public double GetTeModeSigmaFormTopQValue(int m, int n, int p, double Qc)
        {
            double f0 = this.teModeResoFreq(m, n, p);
            double mu0 = 4 * Math.PI * 1E-7;
            double upmn = jpmn[m, n - 1];
            double a = this.radius;
            double l = this.height;
            double ds = a * (upmn * upmn - m * m) * (upmn * upmn + Math.Pow(p * Math.PI * a / l, 2)) / Qc;
            ds = ds / ((upmn * upmn - m * m) * 2 * Math.Pow(Math.PI * p, 2) * a * a * a / l / l / l) * 2;
            double sigma = 1 / (Math.PI * f0 * mu0 * ds * ds);
            return sigma;
        }

        public double GetTeModeSigmaFormTopQValue(int m, int n, int p, double Qc,double f0)
        {
            double mu0 = 4 * Math.PI * 1E-7;
            double upmn = jpmn[m, n - 1];
            double a = this.radius;
            double l = this.height;
            double ds = a * (upmn * upmn - m * m) * (upmn * upmn + Math.Pow(p * Math.PI * a / l, 2)) / Qc;
            ds = ds / ((upmn * upmn - m * m) * 2 * Math.Pow(Math.PI * p, 2) * a * a * a / l / l / l) * 2;
            double sigma = 1 / (Math.PI * f0 * mu0 * ds * ds);
            return sigma;
        }








        public double teModeResoSideQValue(int m, int n, int p, double sigma)
        {
            double f0 = this.teModeResoFreq(m, n, p);
            double mu0 = 4 * Math.PI * 1E-7;
            double ds = 1 / Math.Sqrt(Math.PI * f0 * mu0 * sigma);
            double upmn = jpmn[m, n - 1];
            double a = this.radius;
            double l = this.height;
            double qu = a * (upmn * upmn - m * m) * (upmn * upmn + Math.Pow(p * Math.PI * a / l, 2)) / ds;
            qu = qu / (Math.Pow(upmn, 4) + Math.Pow(Math.PI * m * p * a / l, 2));
            return qu;
        }

        public double teModeResoQValuePer(int m, int n, int p, double sigma)
        {
            double f0 = this.teModeResoFreq(m, n, p);
            double mu0 = 4 * Math.PI * 1E-7;
            double ds = 1 / Math.Sqrt(Math.PI * f0 * mu0 * sigma);
            double upmn = jpmn[m, n - 1];
            double a = this.radius;
            double l = this.height;
            double qu = a * (upmn * upmn + Math.Pow(p * Math.PI * a / l, 2)) / ds;
            qu = qu / (Math.Pow(upmn, 2) + 2 * Math.Pow(Math.PI * p, 2) * Math.Pow(a / l, 3));
            return qu;
        }

        public double getTeModeSigma(int m, int n, int p, double qu)
        {
            double f0 = this.teModeResoFreq(m, n, p);
            double mu0 = 1.2566370614E-6;
            double upmn = jpmn[m, n - 1];
            double a = this.radius;
            double l = this.height;
            double ds = a * (upmn * upmn - m * m) * (upmn * upmn + Math.Pow(p * Math.PI * a / l, 2)) / qu;
            ds = ds / (Math.Pow(upmn, 4) + (upmn * upmn - m * m) * 2 * Math.Pow(Math.PI * p, 2) * a * a * a / l / l / l + Math.Pow(Math.PI * m * p * a / l, 2));
            return 1 / (ds * ds * Math.PI * f0 * mu0);
        }

    }
}
