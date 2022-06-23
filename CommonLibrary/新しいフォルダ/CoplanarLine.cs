using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Meta.Numerics;
using Meta.Numerics.Functions;
using CommonLibrary.Basic;
using CommonLibrary.FileIO;
using CommonLibrary.Measurement;


namespace CommonLibrary.Transmission
{
    //-----------------------------------------------------------------------------------------//
    //"CoplanarLine"
    //GND無コプレーナラインのクラス
    //
    //-----------------------------------------------------------------------------------------//
    public class CoplanarLine
    {
        //プライベート変数
        private double epr, widtha, widthb, height, thickness;
        private double epeff, zc;

        //コンストラクタ

        public CoplanarLine(double epr, double widtha, double widthb, double height, double thickness)
        {
            this.epr = epr;
            this.widtha = widtha;
            this.widthb = widthb;
            this.height = height;
            this.thickness = thickness;

            //インピーダンスzc,実効誘電率epeffの導出
            //from"ACCURATE MODELS FOR MICROSTRIP COMPUTER-AIDED DESIGN" Hammerstad and Jensen

            this.zc = this.calcZc(epr, widtha, (widthb - widtha) / 2, height, thickness);
            this.epeff = this.calcEpeff(epr, widtha, widthb, height, thickness);
        }

        //コンストラクタ(2)
        public CoplanarLine(double zc)
        {
            this.zc = zc;
        }

        private double calcEpeff(double epr, double widtha, double widthb, double height, double thickness)
        {
            double kt, k, k1, at, bt, kpr, ktpr, k1pr;

            at = widtha + (1.25 * thickness / Math.PI) * (1.0 + Math.Log(4.0 * Math.PI * widtha / thickness));

            bt = widthb - (1.25 * thickness / Math.PI) * (1.0 + Math.Log(4.0 * Math.PI * widtha / thickness));

            k = widtha / widthb;

            kt = at / bt;

            k1 = Math.Sinh((Math.PI * at) / (4.0 * height)) / Math.Sinh((Math.PI * bt) / (4.0 * height));

            kpr = Math.Sqrt(1.0 - Math.Pow(k, 2));

            ktpr = Math.Sqrt(1.0 - Math.Pow(kt, 2));

            k1pr = Math.Sqrt(1.0 - Math.Pow(k1, 2));

            return 1.0 + (epr - 1.0) / 2.0 * (AdvancedMath.EllipticK(kpr) * AdvancedMath.EllipticK(k1)) / (AdvancedMath.EllipticK(k) * AdvancedMath.EllipticK(k1pr));

        }


        //インピーダンスzからu=width/heightを求める
        private double calcZc(double epr, double widtha, double w, double height, double thickness)
        {
            double widthb = widtha + 2 * w;

            double k, kt, k1, at, bt, kpr, ktpr, k1pr, epeff;

            k = widtha / widthb;

            at = widtha + (1.25 * thickness / Math.PI) * (1.0 + Math.Log(4.0 * Math.PI * widtha / thickness));

            bt = widthb - (1.25 * thickness / Math.PI) * (1.0 + Math.Log(4.0 * Math.PI * widtha / thickness));

            kt = at / bt;

            k1 = Math.Sinh((Math.PI * at) / (4.0 * height)) / Math.Sinh((Math.PI * bt) / (4.0 * height));

            kpr = Math.Sqrt(1.0 - Math.Pow(k, 2));

            ktpr = Math.Sqrt(1.0 - Math.Pow(kt, 2));

            k1pr = Math.Sqrt(1.0 - Math.Pow(k1, 2));



            epeff = 1.0 + (epr - 1.0) / 2.0 * (AdvancedMath.EllipticK(kpr) * AdvancedMath.EllipticK(k1)) / (AdvancedMath.EllipticK(k) * AdvancedMath.EllipticK(k1pr));


            double epefft = epeff - ((epeff - 1.0) / ((((widthb - widtha) / 2.0) / (0.7 * thickness) * AdvancedMath.EllipticK(k) / AdvancedMath.EllipticK(kpr)) + 1.0));

            return (30.0 * Math.PI / Math.Sqrt(epefft)) * (AdvancedMath.EllipticK(ktpr) / AdvancedMath.EllipticK(kt));

        }

        //uからwidthを求める

        public double calcWidtha(double epr, double w, double height, double thickness, double zc)
        {
            return FunctionMath.FindZero(delegate(double x) { return calcZc(epr, x, w, height, thickness) - zc; }, 0.01);
        }


        //getメソッド
        public double getEpr() { return epr; }
        public double getHeight() { return height; }
        public double getThickness() { return thickness; }
        public double getWidtha() { return widtha; }
        public double getWidthb() { return widthb; }
        public double getEpreff() { return epeff; }
        public double getZc() { return zc; }

        public BasicComplex gamma(double f)
        {
            return new BasicComplex(0, 2 * Math.PI * f / PhisicalConstant.c0 * Math.Sqrt(this.epeff));
        }


        //位相角
        public BasicComplex phase(double f, double length)
        {
            return this.gamma(f) * (length);
        }

        //入力インピーダンス
        public BasicComplex inputImpedance(double f, double length, BasicComplex zl)
        {
            //zl + zc tanh(γl)
            BasicComplex bunshi = zl + zc * BasicComplex.ComplexTanh(phase(f, length));
            //zc + zl tanh(γl)
            BasicComplex bunbo = zc + zl * BasicComplex.ComplexTanh(phase(f, length));

            return zc * ((bunshi) / (bunbo));

        }

        //入力インピーダンス（電気角定義）
        public BasicComplex inputImpedanceDegree(double f, double deg, BasicComplex zl)
        {
            //zl + zc tanh(γl)
            BasicComplex bunshi = zl + zc * new BasicComplex(0, Math.Tan(deg / 180 * Math.PI));
            //zc + zl tanh(γl)
            BasicComplex bunbo = zc + zl * new BasicComplex(0, Math.Tan(deg / 180 * Math.PI));

            return zc * ((bunshi) / (bunbo));

        }
    }
}
