using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Meta.Numerics;
using Meta.Numerics.Functions;
using CommonLibrary.Basic;
using CommonLibrary.Resonator;
using CommonLibrary.Transmission;


namespace CommonLibrary.Measurement
{
    //漏れ出る部分を金属壁でcutして計算する(簡単な形状にして解析解を求め、おおまかな値を予測する)
    public class CylindricalCutoffResonator
    {
        private CircularWaveguide cw1;
        private CircularWaveguide cw2;
        private double er;
        private double t0;
        private double r0;
        private double l0;

        public CylindricalCutoffResonator(double r0, double l0, double er, double t0)
        {
            this.r0 = r0;
            this.l0 = l0;
            this.er = er;
            this.t0 = t0;
            cw1 = new CircularWaveguide(1.0, r0);
            cw2 = new CircularWaveguide(er, r0);
        }

        public CylindricalCutoffResonator(double r0, double l0, double t0)
        {
            this.r0 = r0;
            this.l0 = l0;
            this.t0 = t0;
            cw1 = new CircularWaveguide(1.0, r0);
            cw2 = new CircularWaveguide(1.0, r0);
        }

        public double teModeResonantFreq(int m, int n)
        {
            double kr=BesselZero.getJpmn(m, n - 1) / this.r0; 
            double roots = FunctionMath.FindZero(delegate(double x1) { return this.functionOfTeMode(m, n, x1); }, 0);
            double f0 = 2.99792458E8 / (2 * Math.PI * Math.Sqrt(er)) * Math.Sqrt(roots * roots / (t0 / 2 * t0 / 2) + kr * kr);
            return f0;
        }


        private double functionOfTeMode(int m, int n, double x)
        {
            double kr = BesselZero.getJpmn(m, n - 1) / this.r0;
            double dummy = ((er - 1) * kr * kr - x * x / (t0 / 2 * t0 / 2)) / er;
            if (dummy < 0) return -x * Math.Tan(x * this.t0 / 2) / t0;
            else return Math.Sqrt(((er - 1) * kr * kr - x * x / (t0 / 2 * t0 / 2)) / er) - x * Math.Tan(x) / (t0 / 2);
        }



        public double tmModeResonantFreq(int m, int n)
        {
            double roots = FunctionMath.FindZero(delegate(double x1) { return this.functionOfTmMode(m, n, x1); }, 0);
            double kr = BesselZero.getJmn(m, n - 1) / this.r0;
            double f0 = 2.99792458E8 / (2 * Math.PI * Math.Sqrt(er)) * Math.Sqrt(roots * roots / (t0 / 2 * t0 / 2) + kr * kr);
            return f0;
        }

        private double functionOfTmMode(int m, int n, double x)
        {
            double kr = BesselZero.getJmn(m, n - 1) / this.r0;
            double dummy = ((er - 1) * kr * kr - x * x / (t0 / 2 * t0 / 2)) / er;
            if (dummy < 0) return -x * Math.Tan(x * this.t0 / 2) / t0;
            else return Math.Sqrt(((er - 1) * kr * kr - x * x / (t0 / 2 * t0 / 2)) / er) - x * Math.Tan(x) / (t0 / 2)/er;
        }

        public double getTeModeEr(int m, int n, double f0)
        {
            double kr = BesselZero.getJpmn(m, n - 1) / this.r0;
            double k0 = 2 * Math.PI * f0 / 2.99792458E8;
            double alpha = Math.Sqrt(kr * kr - k0 * k0);
            double roots = FunctionMath.FindZero(delegate(double x1) { return alpha - x1 * Math.Tan(x1 * t0 / 2); }, 0);
            return (roots * roots + kr * kr) / (k0 * k0);
        }

        public double getTmModeEr(int m, int n, double f0)
        {
            double kr = BesselZero.getJmn(m, n - 1) / this.r0;
            double k0 = 2 * Math.PI * f0 / 2.99792458E8;
            double alpha = Math.Sqrt(kr * kr - k0 * k0);
            double roots = FunctionMath.FindZero(delegate(double x1) { return x1*alpha-Math.Sqrt(x1*k0*k0-kr*kr)*Math.Tan(Math.Sqrt(x1*k0*k0-kr*kr)*this.t0/2); }, 5);
            return roots;
        }

    }
}
