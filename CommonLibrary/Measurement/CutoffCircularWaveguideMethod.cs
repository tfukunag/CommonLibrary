using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Meta.Numerics;
using Meta.Numerics.Functions;
using Meta.Numerics.Matrices;
using System.IO;
using CommonLibrary.Basic;
using CommonLibrary.Resonator;
using CommonLibrary.Transmission;



namespace CommonLibrary.Measurement
{
    //誘電体Qの導出(遮断円筒導波管法)
    public class CutoffCircularWaveguideMethod
    {
        private double a;
        private double r;
        private double h;
        private double t;
        private double delta0=0.0001;

        public CutoffCircularWaveguideMethod(double a, double r,double h,double t)
        {
            this.a = a;
            this.r = r;
            this.h = h;
            this.t = t;
        }

        public double approximationErFromTe111Mode(double f0)
        {
            CylindricalCutoffResonator ccr = new CylindricalCutoffResonator(r, h, t);
            return ccr.getTeModeEr(1, 1, f0);
        }

        private double calcDetH0(int n0, int k0, double er,double f0,double r0,double t0,double g0,double h0)
        {
            try
            {
                double c0 = 2.99792458E8;
                double[,] jmn = BesselZero.getJmnMatrix();
                double[,] jpmn = BesselZero.getJpmnMatrix();
                double er_air = 1.00059;

                //            SquareMatrix sm = new SquareMatrix(n0);
                //            for (int i = 0; i < n0; i++) for (int j = 0; j < n0; j++) sm[i, j] = 0;
                SymmetricMatrix sm2 = new SymmetricMatrix(n0);
                for (int i = 0; i < n0; i++) for (int j = i; j < n0; j++) sm2[i, j] = 0;

                double k0r2 = Math.Pow(2 * Math.PI * f0 / c0 * r0, 2);

                Complex[] xp = new Complex[k0];
                Complex[] zp = new Complex[k0];
                Complex[] cpbp = new Complex[k0];
                Complex[] coef = new Complex[k0];
                Complex[] yq = new Complex[n0];
                double l1 = t0 / 2;
                double l2 = l1 + g0;
                for (int i = 0; i < k0; i++)
                {
                    double xp2 = er * k0r2 - Math.Pow(r0 / a * jpmn[0, i], 2);
                    double zp2 = er_air * k0r2 - Math.Pow(r0 / a * jpmn[0, i], 2);

                    if (xp2 >= 0) xp[i] = new Complex(Math.Sqrt(xp2), 0);
                    else xp[i] = new Complex(0, Math.Sqrt(-xp2));
                    if (zp2 >= 0) zp[i] = new Complex(Math.Sqrt(zp2), 0);
                    else zp[i] = new Complex(0, Math.Sqrt(-zp2));
                    cpbp[i] = -(xp[i] * ComplexMath.Tan(xp[i] * l1 / r0) - zp[i] * ComplexMath.Tan(zp[i] * l1 / r0)) / (xp[i] * ComplexMath.Tan(xp[i] * l1 / r0) * ComplexMath.Tan(zp[i] * l1 / r0) + zp[i]);
                    coef[i] = zp[i] * (ComplexMath.Tan(zp[i] * l2 / r0) - cpbp[i]) / (1 + cpbp[i] * ComplexMath.Tan(zp[i] * l2 / r0)) / Math.Pow(a / r0 * AdvancedMath.BesselJ(0, jpmn[0, i]), 2);
                    coef[i] = coef[i] * Math.Pow(AdvancedMath.BesselJ(1, r0 / a * jpmn[0, i]), 2);
                }


                //対角成分の計算
                for (int i = 0; i < n0; i++)
                {
                    double yq2 = k0r2 - Math.Pow(jpmn[0, i], 2);
                    if (yq2 >= 0) yq[i] = new Complex(Math.Sqrt(yq2), 0);
                    else yq[i] = new Complex(0, Math.Sqrt(-yq2));
                    //                sm[i, i] = (yq[i] * Math.Pow(AdvancedMath.BesselJ(0, jpmn[0, i]), 2) / ComplexMath.Tan(yq[i] * h0 / 2 / r0)).Re / 4;
                    sm2[i, i] = (yq[i] * Math.Pow(AdvancedMath.BesselJ(0, jpmn[0, i]), 2) / ComplexMath.Tan(yq[i] * h0 / 2 / r0)).Re / 4;
                    for (int k = 0; k < k0; k++)
                    {
                        double pik = jpmn[0, i] * AdvancedMath.BesselJ(0, jpmn[0, i]) / (Math.Pow(r0 / a * jpmn[0, k], 2) - Math.Pow(jpmn[0, i], 2));
                        //                    sm[i, i] = sm[i, i] - pik * pik * coef[k].Re;
                        sm2[i, i] = sm2[i, i] - pik * pik * coef[k].Re;
                    }
                }


                //非対角成分の計算 対称行列なので計算省略
                for (int i = 0; i < n0; i++)
                {
                    for (int j = i + 1; j < n0; j++)
                    {
                        for (int k = 0; k < k0; k++)
                        {
                            double pik = jpmn[0, i] * AdvancedMath.BesselJ(0, jpmn[0, i]) / (Math.Pow(r0 / a * jpmn[0, k], 2) - Math.Pow(jpmn[0, i], 2));
                            double pkj = jpmn[0, j] * AdvancedMath.BesselJ(0, jpmn[0, j]) / (Math.Pow(r0 / a * jpmn[0, k], 2) - Math.Pow(jpmn[0, j], 2));
                            //                        sm[i, j] = sm[i, j] - pik * pkj * coef[k].Re;
                            sm2[i, j] = sm2[i, j] - pik * pkj * coef[k].Re;
                        }
                        //                    sm[j, i] = sm[i, j];
                    }
                }




                //            double detH0 = sm.LUDecomposition().Determinant();
                double detH0 = sm2.CholeskyDecomposition().Determinant();

                return detH0;
            }
            catch (DivideByZeroException) 
            {
                //System.Windows.Forms.MessageBox.Show(e.Message,"error", System.Windows.Forms.MessageBoxButtons.OK);
                throw new Exception("計算中に問題が発生しました。");
            }
        }

        private double calcDetH0(int n0, int k0, double er, double f0, double r0, double t0, double g0, double h0, bool pec)
        {
            
                double c0 = 2.99792458E8;
                double[,] jmn = BesselZero.getJmnMatrix();
                double[,] jpmn = BesselZero.getJpmnMatrix();
                double er_air = 1.00059;

                SquareMatrix sm = new SquareMatrix(n0);
                for (int i = 0; i < n0; i++) for (int j = 0; j < n0; j++) sm[i, j] = 0;
                //            SymmetricMatrix sm2 = new SymmetricMatrix(n0);
                //            for (int i = 0; i < n0; i++) for (int j = i; j < n0; j++) sm2[i, j] = 0;

                double[] up = new double[k0];

                double k0r2 = Math.Pow(2 * Math.PI * f0 / c0 * r0, 2);

                Complex[] xp = new Complex[k0];
                Complex[] zp = new Complex[k0];
                Complex[] cpbp = new Complex[k0];
                Complex[] coef = new Complex[k0];
                Complex[] yq = new Complex[n0];
                double l1 = t0 / 2;
                double l2 = l1 + g0;
                for (int i = 0; i < k0; i++)
                {
                    if (pec) up[i] = jpmn[0, i];
                    else up[i] = jmn[0, i];
                    double xp2 = er * k0r2 - Math.Pow(r0 / a * up[i], 2);
                    double zp2 = er_air * k0r2 - Math.Pow(r0 / a * up[i], 2);

                    if (xp2 >= 0) xp[i] = new Complex(Math.Sqrt(xp2), 0);
                    else xp[i] = new Complex(0, Math.Sqrt(-xp2));
                    if (zp2 >= 0) zp[i] = new Complex(Math.Sqrt(zp2), 0);
                    else zp[i] = new Complex(0, Math.Sqrt(-zp2));
                    cpbp[i] = -(xp[i] * ComplexMath.Tan(xp[i] * l1 / r0) - zp[i] * ComplexMath.Tan(zp[i] * l1 / r0)) / (xp[i] * ComplexMath.Tan(xp[i] * l1 / r0) * ComplexMath.Tan(zp[i] * l1 / r0) + zp[i]);
                    coef[i] = zp[i] * (ComplexMath.Tan(zp[i] * l2 / r0) - cpbp[i]) / (1 + cpbp[i] * ComplexMath.Tan(zp[i] * l2 / r0)) / (Math.Pow(AdvancedMath.BesselJ(0, up[i]), 2) + Math.Pow(AdvancedMath.BesselJ(1, up[i]), 2)) / Math.Pow(a / r0, 2);
                    coef[i] = coef[i] * Math.Pow(AdvancedMath.BesselJ(1, r0 / a * up[i]), 2);
                }


                //対角成分の計算
                for (int i = 0; i < n0; i++)
                {
                    double yq2 = k0r2 - Math.Pow(jpmn[0, i], 2);
                    if (yq2 >= 0) yq[i] = new Complex(Math.Sqrt(yq2), 0);
                    else yq[i] = new Complex(0, Math.Sqrt(-yq2));
                    sm[i, i] = (yq[i] * Math.Pow(AdvancedMath.BesselJ(0, jpmn[0, i]), 2) / ComplexMath.Tan(yq[i] * h0 / 2 / r0)).Re / 4;
                    //                sm2[i, i] = (yq[i] * Math.Pow(AdvancedMath.BesselJ(0, jpmn[0, i]), 2) / ComplexMath.Tan(yq[i] * h0 / 2 / r0)).Re / 4;
                    for (int k = 0; k < k0; k++)
                    {
                        double pik = jpmn[0, i] * AdvancedMath.BesselJ(0, jpmn[0, i]) / (Math.Pow(r0 / a * up[k], 2) - Math.Pow(jpmn[0, i], 2));
                        sm[i, i] = sm[i, i] - pik * pik * coef[k].Re;
                        //                    sm2[i, i] = sm2[i, i] - pik * pik * coef[k].Re;
                    }
                }


                //非対角成分の計算 対称行列なので計算省略
                for (int i = 0; i < n0; i++)
                {
                    for (int j = i + 1; j < n0; j++)
                    {
                        for (int k = 0; k < k0; k++)
                        {
                            double pik = jpmn[0, i] * AdvancedMath.BesselJ(0, jpmn[0, i]) / (Math.Pow(r0 / a * up[k], 2) - Math.Pow(jpmn[0, i], 2));
                            double pkj = jpmn[0, j] * AdvancedMath.BesselJ(0, jpmn[0, j]) / (Math.Pow(r0 / a * up[k], 2) - Math.Pow(jpmn[0, j], 2));
                            sm[i, j] = sm[i, j] - pik * pkj * coef[k].Re;
                            //                        sm2[i, j] = sm2[i, j] - pik * pkj * coef[k].Re;
                        }
                        sm[j, i] = sm[i, j];
                    }
                }




                double detH0 = sm.LUDecomposition().Determinant();
                //            double detH0 = sm2.CholeskyDecomposition().Determinant();

                return detH0;
          
           

        }




        public double calcEr(int n0, int k0, double f0, double er0)
        {
            double roots = FunctionMath.FindZero(delegate(double er) { return this.calcDetH0(n0, k0, er, f0, r, t, 0, h); }, er0);
            return roots;
        }


        public double calcEr(int n0, int k0, double f0, double er0, bool pec)
        {
            double roots = FunctionMath.FindZero(delegate(double er) { return this.calcDetH0(n0, k0, er, f0, r, t, 0, h, pec); }, er0);
            return roots;
        }

        public double calcF0(int n0, int k0, double er, double f0init)
        {
            double roots = FunctionMath.FindZero(delegate(double f0) { return this.calcDetH0(n0, k0, er, f0, r, t, 0, h); }, f0init);
            return roots;
        }


        public double calcF0(int n0, int k0, double er, double f0init, bool pec)
        {
            double roots = FunctionMath.FindZero(delegate(double f0) { return this.calcDetH0(n0, k0, er, f0, r, t, 0, h, pec); }, f0init);
            return roots;
        }


        public double calcQc(int n0, int k0, double er0, double f0, double sigma)
        {
            double fr1 = FunctionMath.FindZero(delegate(double f1) { return this.calcDetH0(n0, k0, er0, f1, r * (1+this.delta0/100), t, 0, h); }, f0);
            double fr2 = FunctionMath.FindZero(delegate(double f1) { return this.calcDetH0(n0, k0, er0, f1, r * (1 - this.delta0 / 100), t, 0, h); }, f0);
//            double dummyfr1 = this.calcDetH0(n0, k0, er0, fr1, r * 1.001, t,0, h);
//            double dummyfr2 = this.calcDetH0(n0, k0, er0, fr2, r * 0.999, t,0, h);

            double ft1 = FunctionMath.FindZero(delegate(double f1) { return this.calcDetH0(n0, k0, er0, f1, r, t, 0.00001 * t, h); }, f0);
            double ft2 = FunctionMath.FindZero(delegate(double f1) { return this.calcDetH0(n0, k0, er0, f1, r, t, 0, h); }, f0);
//            double dummyft1 = this.calcDetH0(n0, k0, er0, ft1, r, t * 1.001,0, h);
//            double dummyft2 = this.calcDetH0(n0, k0, er0, ft2, r, t * 0.999,0. h);

            double fh1 = FunctionMath.FindZero(delegate(double f1) { return this.calcDetH0(n0, k0, er0, f1, r, t, 0, h * (1 + this.delta0 / 100)); }, f0);
            double fh2 = FunctionMath.FindZero(delegate(double f1) { return this.calcDetH0(n0, k0, er0, f1, r, t, 0, h * (1 - this.delta0 / 100)); }, f0);
//            double dummyfh1 = this.calcDetH0(n0, k0, er0, fh1, r, t, h * 1.001);
//            double dummyfh2 = this.calcDetH0(n0, k0, er0, fh2, r, t, h * 0.999);

            double delta = 1 / Math.Sqrt(Math.PI * f0 * 4 * Math.PI * 1E-7 * sigma);
            double dummy = delta / f0 * ((fr1 - fr2) / (2 * this.delta0 / 100 * r) + (ft1 - ft2) / (0.00001 * t) + 2 * (fh1 - fh2) / (2 * this.delta0 / 100 * h));
/*            double dummy1 = -1 / (delta / f0 * ((fr1 - fr2) / (0.002 * r)));
            double dummy2 = -1 / (delta / f0 * (2 * (ft1 - ft2) / (0.00001 * t)));
            double dummy3 = -1/(delta / f0 * (2 * (fh1 - fh2) / (0.002 * h)));*/
            return -1 / dummy;
        }

        public double calcQc(int n0, int k0, double er0, double f0, double sigma, bool pec)
        {
            double fr1 = FunctionMath.FindZero(delegate(double f1) { return this.calcDetH0(n0, k0, er0, f1, r * (1 + this.delta0 / 100), t, 0, h, pec); }, f0);
            double fr2 = FunctionMath.FindZero(delegate(double f1) { return this.calcDetH0(n0, k0, er0, f1, r * (1 - this.delta0 / 100), t, 0, h, pec); }, f0);

            double ft1 = FunctionMath.FindZero(delegate(double f1) { return this.calcDetH0(n0, k0, er0, f1, r, t, 0.00001 * t, h, pec); }, f0);
            double ft2 = FunctionMath.FindZero(delegate(double f1) { return this.calcDetH0(n0, k0, er0, f1, r, t, 0, h, pec); }, f0);

            double fh1 = FunctionMath.FindZero(delegate(double f1) { return this.calcDetH0(n0, k0, er0, f1, r, t, 0, h * (1 + this.delta0 / 100), pec); }, f0);
            double fh2 = FunctionMath.FindZero(delegate(double f1) { return this.calcDetH0(n0, k0, er0, f1, r, t, 0, h * (1 - this.delta0 / 100), pec); }, f0);

            double delta = 1 / Math.Sqrt(Math.PI * f0 * 4 * Math.PI * 1E-7 * sigma);
            double dummy = delta / f0 * ((fr1 - fr2) / (2 * this.delta0 / 100 * r) + (ft1 - ft2) / (0.00001 * t) + 2 * (fh1 - fh2) / (2 * this.delta0 / 100 * h));
            return -1 / dummy;
        }



        public double calcTand(int n0, int k0, double er0, double f0, double sigma, double qc, double qu)
        {
            double fe1 = FunctionMath.FindZero(delegate(double f1) { return this.calcDetH0(n0, k0, er0 * (1 + this.delta0 / 100), f1, r, t, 0, h); }, f0);
            double fe2 = FunctionMath.FindZero(delegate(double f1) { return this.calcDetH0(n0, k0, er0 * (1 - this.delta0 / 100), f1, r, t, 0, h); }, f0);
            double tand = f0 / (2 * er0 * (-(fe1 - fe2) / (2*this.delta0/100) / er0)) * (1 / qu - 1 / qc);
            return tand;
        }

        public double calcTand(int n0, int k0, double er0, double f0, double sigma, double qc, double qu,bool pec)
        {
            double fe1 = FunctionMath.FindZero(delegate(double f1) { return this.calcDetH0(n0, k0, er0 * (1 + this.delta0 / 100), f1, r, t, 0, h, pec); }, f0);
            double fe2 = FunctionMath.FindZero(delegate(double f1) { return this.calcDetH0(n0, k0, er0 * (1 - this.delta0 / 100), f1, r, t, 0, h, pec); }, f0);
            double tand = f0 / (2 * er0 * (-(fe1 - fe2) / (2 * this.delta0 / 100) / er0)) * (1 / qu - 1 / qc);
            return tand;
        }

    }
}
