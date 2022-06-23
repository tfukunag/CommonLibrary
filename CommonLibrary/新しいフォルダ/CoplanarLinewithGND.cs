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
    //"CoplanarLinewithGND"
    //GND付コプレーナラインのクラス
    //
    //-----------------------------------------------------------------------------------------//
    public class CoplanarLineWithGND
    {
        //プライベート変数
        private double epr, widtha, widthb, height;
        private double epeff, zc;

        //コンストラクタ
        public CoplanarLineWithGND(double epr, double widtha, double widthb, double height)
        {
            this.epr = epr;
            this.widtha = widtha;
            this.widthb = widthb;
            this.height = height;



            //インピーダンスzc,実効誘電率epeffの導出
            //from"Transmission Line Design Handbook" Brian C. Wadell

            this.zc = this.calcZc(epr, widtha, (widthb - widtha) / 2, height);
            this.epeff = this.calcEpeff(epr, widtha, widthb, height);

        }


        //コンストラクタ(2)
        public CoplanarLineWithGND(double zc)
        {
            this.zc = zc;
        }

        //        private double calcEpeff(double epr, double widtha, double widthb, double height, double thickness)
        private double calcEpeff(double epr, double widtha, double widthb, double height)
        {
            double k, k1, kpr, k1pr;


            k = widtha / widthb;

            k1 = Math.Tanh((Math.PI * widtha) / (4.0 * height)) / Math.Tanh((Math.PI * widthb) / (4.0 * height));

            kpr = Math.Sqrt(1.0 - Math.Pow(k, 2));

            k1pr = Math.Sqrt(1.0 - Math.Pow(k1, 2));

            //実効比誘電率
            return (1.0 + ((epr * AdvancedMath.EllipticK(kpr) * AdvancedMath.EllipticK(k1)) / (AdvancedMath.EllipticK(k) * AdvancedMath.EllipticK(k1pr)))) / (1.0 + ((AdvancedMath.EllipticK(kpr) * AdvancedMath.EllipticK(k1)) / (AdvancedMath.EllipticK(k) * AdvancedMath.EllipticK(k1pr))));


        }

        private double calcZc(double epr, double widtha, double w, double height)
        {

            double k, k1, kpr, k1pr, epeff;

            widthb = widtha + (2 * w);

            k = widtha / widthb;

            k1 = Math.Tanh((Math.PI * widtha) / (4.0 * height)) / Math.Tanh((Math.PI * widthb) / (4.0 * height));

            kpr = Math.Sqrt(1.0 - Math.Pow(k, 2));

            k1pr = Math.Sqrt(1.0 - Math.Pow(k1, 2));

            //実効比誘電率

            epeff = (1.0 + ((epr * AdvancedMath.EllipticK(kpr) * AdvancedMath.EllipticK(k1)) / (AdvancedMath.EllipticK(k) * AdvancedMath.EllipticK(k1pr)))) / (1.0 + ((AdvancedMath.EllipticK(kpr) * AdvancedMath.EllipticK(k1)) / (AdvancedMath.EllipticK(k) * AdvancedMath.EllipticK(k1pr))));

            //インピーダンス
            return 60 * Math.PI / Math.Sqrt(epeff) / ((AdvancedMath.EllipticK(k) / AdvancedMath.EllipticK(kpr)) + (AdvancedMath.EllipticK(k1) / AdvancedMath.EllipticK(k1pr)));

        }

        //インピーダンスからwidtha,bを求める

        public double calcWidtha(double epr, double w, double height, double z)
        {
            return FunctionMath.FindZero(delegate(double x) { return calcZc(epr, x, w, height) - z; }, 0.0001);//初期値を小さく抑えた結果動くようになった
        }


        //uからwidthを求める
        //インピーダンスからeprを求める




        //getメソッド
        public double getEpr() { return epr; }
        public double getHeight() { return height; }
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
            BasicComplex zcc = new BasicComplex(zc, 0);
            //zl + zc tanh(γl)
            BasicComplex bunshi = zl + (zcc * (BasicComplex.ComplexTanh(phase(f, length))));
            //zc + zl tanh(γl)
            BasicComplex bunbo = zcc + (zl * (BasicComplex.ComplexTanh(phase(f, length))));

            return zcc * ((bunshi) / (bunbo));

        }

        //入力インピーダンス（電気角定義）
        public BasicComplex inputImpedanceDegree(double f, double deg, BasicComplex zl)
        {
            BasicComplex bunshi = zl + zc * new BasicComplex(0, Math.Tan(deg / 180 * Math.PI));
            //zc + zl tanh(γl)
            BasicComplex bunbo = zc + zl * new BasicComplex(0, Math.Tan(deg / 180 * Math.PI));

            return zc * ((bunshi) / (bunbo));

        }




    }
}
