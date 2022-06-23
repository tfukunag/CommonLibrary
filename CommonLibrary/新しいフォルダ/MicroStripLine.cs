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
    //"MicroStripLine"
    //マイクロストリップラインのクラス
    //
    //-----------------------------------------------------------------------------------------/
    public class MicroStripLine
    {
        //プライベート変数
        private double epr, width, height;
        private double epeff, zc;

        //コンストラクタ
        public MicroStripLine(double epr, double width, double height)
        {
            this.epr = epr;
            this.width = width;
            this.height = height;


            //インピーダンスzc,実効誘電率epeffの導出
            //from"ACCURATE MODELS FOR MICROSTRIP COMPUTER-AIDED DESIGN" Hammerstad and Jensen

            double u, f, a, b;

            u = width / height;

            f = (6 / ((2 * Math.PI) - 6)) * Math.Exp(-Math.Pow((30.666 / u), 0.7528));

            a = 1 + ((1 / 49) * Math.Log((Math.Pow(u, 4) + Math.Pow(u / 52, 2)) / (Math.Pow(u, 4) + 0.432))) + ((1 / 18.7) * Math.Log(1 + Math.Pow(u / 18.1, 3)));

            b = 0.564 * Math.Pow(((epr - 0.9) / (epr + 3)), 0.053);

            this.epeff = ((epr + 1) / 2) + ((epr - 1) / 2) * Math.Pow((1 + 10 / u), (-a * b));

            this.zc = this.calcZc(u, this.epr);

        }

        public MicroStripLine(double zc)
        {

            this.zc = zc;
        }

        private double calcZc(double u, double epr)
        {
            double f;

            double a, b, z0;

            f = 6 + (((2 * Math.PI) - 6) * (Math.Exp(-(Math.Pow((30.666 / u), 0.7528)))));

            a = 1 + ((1 / 49) * Math.Log((Math.Pow(u, 4) + Math.Pow(u / 52, 2)) / (Math.Pow(u, 4) + 0.432))) + ((1 / 18.7) * Math.Log(1 + Math.Pow(u / 18.1, 3)));

            b = 0.564 * Math.Pow(((epr - 0.9) / (epr + 3)), 0.053);

            epeff = ((epr + 1) / 2) + ((epr - 1) / 2) * Math.Pow((1 + 10 / u), (-a * b));

            z0 = 60 * Math.Log((f / u) + (Math.Sqrt(1 + Math.Pow((2 / u), 2))));

            return z0 / Math.Sqrt(epeff);
        }


        //インピーダンスzからu=width/heightを求める
        private double calcU(double epr, double z)
        {
            return FunctionMath.FindZero(delegate(double x) { return calcZc(x, epr) - z; }, 1.0);
        }

        //uからwidthを求める
        public double calcWidth(double epr, double ht, double z)
        {
            return this.calcU(epr, z) * ht;
        }




        //getメソッド
        public double getEpr() { return this.epr; }
        public double getHeight() { return this.height; }
        public double getWidth() { return this.width; }
        public double getEpreff() { return this.epeff; }
        public double getZc() { return this.zc; }

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
            //zl + zc tan(βl)
            BasicComplex bunshi = zl + zc * BasicComplex.ComplexTanh(phase(f, length));
            //zc + zl tan(βl)
            BasicComplex bunbo = zc + zl * BasicComplex.ComplexTanh(phase(f, length));

            return zc * ((bunshi) / (bunbo));

        }

        //入力インピーダンス（電気角定義）
        public BasicComplex inputImpedanceDegree(double f, double deg, BasicComplex zl)
        {
            BasicComplex bunshi = zl + zc * new BasicComplex(0, Math.Tan(deg / 180 * Math.PI));
            //zc + zl tan(βl)
            BasicComplex bunbo = zc + zl * new BasicComplex(0, Math.Tan(deg / 180 * Math.PI));

            return zc * ((bunshi) / (bunbo));

        }
    }
}