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
    //"StripLine"
    //ストリップラインのクラス
    //
    //-----------------------------------------------------------------------------------------//

    public class StripLine
    {
        //プライベート変数
        private double epr, width, height;
        private double zc;

        //コンストラクタ
        /*        public StripLine(double epr, double width, double breadth, double thickness)
                {
                    this.epr = epr;
                    this.width = width;
                    this.breadth = breadth;
                    this.thickness = thickness;

                    //特性インピーダンスzcの導出
                    //from"Transmission Line Design Handbook" Brian C.Wadell
            
                    double m =6.0/(3.0+(2.0*thickness)/(breadth-thickness));
                    double dw;
                    dw = thickness * (Math.Log(5.0 * (breadth - thickness) / thickness)) / 3.2;
                    //dw = thickness/Math.PI*(1.0-0.5*Math.Log(Math.Pow(1.0/((2.0*(breadth-thickness)/thickness)+1.0),2)+Math.Pow((1/(4*Math.PI)/(width/thickness+1.1)),m)));
             
                    double wprime = width + dw;
                    double x = 4.0*(breadth-thickness)/(Math.PI*wprime);

                    this.zc = 60 / Math.Sqrt(epr) * Math.Log((1.0 + x) * (2 * x + Math.Sqrt(Math.Pow(2 * x, 2) + 6.27)));

                }*/

        public StripLine(double epr, double width, double height)
        {
            this.epr = epr;
            this.width = width;
            this.height = height;

            //特性インピーダンスzcの導出
            //from"Transmission Line Design Handbook" Brian C.Wadell

            this.zc = this.calcZc(epr, width, height);

        }





        //コンストラクタ(2)
        public StripLine(double epr, double zc)
        {
            this.zc = zc;
            this.epr = epr;
        }


        private double calcZc(double epr, double width, double height)
        {


            double k = 1 / Math.Cosh(Math.PI * width / (2.0 * height));
            double kpr = Math.Tanh(Math.PI * width / (2.0 * height));

            return 30 * Math.PI / Math.Sqrt(epr) * AdvancedMath.EllipticK(k) / AdvancedMath.EllipticK(kpr);

        }

        public double calcWidth(double epr, double breadth, double zc)
        {
            double a = FunctionMath.FindZero(delegate(double x) { return calcZc(epr, x, breadth) - zc; }, 0);
            return a;
        }


        //getメソッド
        public double getEpr() { return epr; }
        public double getHeight() { return height; }
        public double getWidth() { return width; }
        public double getZc() { return zc; }

        public BasicComplex gamma(double f)
        {
            return new BasicComplex(0, 2 * Math.PI * f / PhisicalConstant.c0 * Math.Sqrt(this.epr));
        }


        //位相角
        public BasicComplex phase(double f, double length)
        {
            return this.gamma(f) * (length);
        }

        //入力インピーダンス
        public BasicComplex inputImpedance(double f, double length, BasicComplex zl)
        {
            BasicComplex bunshi = zl + zc * BasicComplex.ComplexTanh(phase(f, length));
            //zc + zl tan(βl)
            BasicComplex bunbo = zc + zl * BasicComplex.ComplexTanh(phase(f, length));

            return zc * ((bunshi) / (bunbo));

        }

        //入力インピーダンス（電気角定義）
        public BasicComplex inputImpedanceDegree(double f, double deg, BasicComplex zl)
        {
            //zl + zc tan(βl)
            BasicComplex bunshi = zl + zc * new BasicComplex(0, Math.Tan(deg / 180 * Math.PI));
            //zc + zl tan(βl)
            BasicComplex bunbo = zc + zl * new BasicComplex(0, Math.Tan(deg / 180 * Math.PI));

            return zc * ((bunshi) / (bunbo));

        }


    }
}
