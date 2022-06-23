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
    //円筒導波管のクラス(伝送路として計算する)
    public class CircularWaveguide
    {
        private DielectricMaterial dem;
        private double r;
        public CircularWaveguide(DielectricMaterial dem, double radius)
        {
            this.dem = dem;
            this.r = radius;
        }
        public CircularWaveguide(double er, double radius)
        {
            this.dem = new DielectricMaterial(er);
            this.r = radius;
        }
        //伝搬するかどうか
        public bool teModePropagated(int m, int n, double f0)
        {
            double k0 = 2 * Math.PI * f0 / 2.99792458E8 * Math.Sqrt(dem.getEr());
            double jpmn = BesselZero.getJpmn(m,n-1);
            double kt =jpmn/this.r;
            if (k0 * k0 - kt * kt > 0) return true;
            else return false;
        }
        public bool tmModePropagated(int m, int n, double f0)
        {
            double k0 = 2 * Math.PI * f0 / 2.99792458E8 * Math.Sqrt(dem.getEr());
            double jmn = BesselZero.getJmn(m,n-1);
            double kt = jmn / this.r;
            if (k0 * k0 - kt * kt > 0) return true;
            else return false;
        }

        public double teModeCuttoffWavelength(int m, int n)
        {
            double jpmn = BesselZero.getJpmn(m,n-1);

            return 2 * Math.PI * this.r / jpmn;
        }
        public double tmModeCuttoffWavelength(int m, int n)
        {
            double jmn = BesselZero.getJmn(m, n - 1);

            return 2 * Math.PI * this.r / jmn;
        }

        public double teModeCutoffFreq(int m, int n)
        {
            return 2.99792458E8 / this.teModeCuttoffWavelength(m, n);
        }
        public double tmModeCutoffFreq(int m, int n)
        {
            return 2.99792458E8 / this.tmModeCuttoffWavelength(m, n);
        }
        public Complex teModeGamma(int m, int n, double f0)
        {
            double k0 = 2 * Math.PI * f0 / 2.99792458E8 * Math.Sqrt(dem.getEr());
            double jpmn = BesselZero.getJpmn(m,n-1);
            double kt = jpmn / this.r;
            double alpha;
            double beta;
            if (teModePropagated(m, n, f0))
            {
                beta = Math.Sqrt(k0 * k0 - kt * kt);
                alpha = 0;
            }
            else
            {
                alpha = Math.Sqrt(kt * kt - k0 * k0);
                beta = 0;
            }
            return new Complex(alpha, beta);
        }
        public Complex tmModeGamma(int m, int n, double f0)
        {
            double k0 = 2 * Math.PI * f0 / 2.99792458E8 * Math.Sqrt(dem.getEr());
            double jmn = BesselZero.getJmn(m,n-1);
            double kt = jmn / this.r;
            double alpha;
            double beta;
            if (tmModePropagated(m, n, f0))
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
        public Complex teModePhase(int m, int n, double length, double f0)
        {
            return this.teModeGamma(m, n, f0) * length;
        }
        public Complex tmModePhase(int m, int n, double length, double f0)
        {
            return this.tmModeGamma(m, n, f0) * length;
        }
        public double teModeWaveLength(int m, int n, double f0)
        {
            if (this.teModePropagated(m, n, f0)) return 2 * Math.PI / this.teModeGamma(m, n, f0).Im;
            else return -1;
        }
        public double tmModeWaveLength(int m, int n, double f0)
        {
            if (this.tmModePropagated(m, n, f0)) return 2 * Math.PI / this.tmModeGamma(m, n, f0).Im;
            else return -1;
        }
        public Complex teModeCharacteristicImpedance(int m, int n, double f0)
        {
            return new Complex(0, 2 * Math.PI * f0 * 1.2566370614E-6) / this.teModeGamma(m, n, f0);
        }
        public Complex tmModeCharacteristicImpedance(int m, int n, double f0)
        {
            return this.tmModeGamma(m, n, f0) / new Complex(0, 2 * Math.PI * f0 * this.dem.getEr() * 8.85418782E-12);
        }
        public Complex teModeInputImpedance(int m, int n, double f0, double l0, Complex zl)
        {
            Complex zc=this.teModeCharacteristicImpedance(m,n,f0);
            Complex tanh= ComplexMath.Tanh(this.teModePhase(m, n, l0, f0));
            return zc * (zl + zc * tanh) / (zc + zl * tanh);
        }
        public Complex tmModeInputImpedance(int m, int n, double f0, double l0, Complex zl)
        {
            Complex zc=this.tmModeCharacteristicImpedance(m,n,f0);
            Complex tanh= ComplexMath.Tanh(this.tmModePhase(m, n, l0, f0));
            return zc * (zl + zc * tanh) / (zc + zl * tanh);
        }
    }
}
