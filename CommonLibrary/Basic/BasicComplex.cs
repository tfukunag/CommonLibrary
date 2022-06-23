using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Basic
{
    public class BasicComplex
    {
        private double r;
        private double i;

        

        public BasicComplex(double real, double image)
        {
            this.r = real;
            this.i = image;
        }
        public BasicComplex(BasicComplex a)
        {
            this.r = a.getReal();
            this.i = a.getImage();
        }

        public BasicComplex(double a, double b, string index)
        {
            if (index == "ma")//a:mag,b:angle(deg)
            {
                this.r = a * Math.Cos(b / 180 * Math.PI);
                this.i = a * Math.Sin(b / 180 * Math.PI);
            }
            else if (index == "ri")//a:real,b:image
            {
                this.r = a;
                this.i = b;
            }
            else if (index == "db")//a:mag(dB),b:angle(deg)
            {
                double mag = Math.Pow(10, a / 20);
                this.r = mag * Math.Cos(b / 180 * Math.PI);
                this.i = mag * Math.Sin(b / 180 * Math.PI);
            }
            else
            {
                this.r = 0;
                this.i = 0;
            }
        }
 
        public static BasicComplex operator +(BasicComplex a1, BasicComplex a2)
        {
            return new BasicComplex(a1.getReal() + a2.getReal(), a1.getImage() + a2.getImage());
        }
        public static BasicComplex operator +(BasicComplex a1, double a2)
        {
            return new BasicComplex(a1.getReal() + a2, a1.getImage());
        }
        public static BasicComplex operator +(double a1, BasicComplex a2)
        {
            return new BasicComplex(a1 + a2.getReal(), a2.getImage());
        }
        public static BasicComplex operator -(BasicComplex a1, BasicComplex a2)
        {
            return new BasicComplex(a1.getReal() - a2.getReal(), a1.getImage() - a2.getImage());
        }
        public static BasicComplex operator -(double a1, BasicComplex a2)
        {
            return new BasicComplex(a1 - a2.getReal(), -a2.getImage());
        }
        public static BasicComplex operator -(BasicComplex a1, double a2)
        {
            return new BasicComplex(a1.getReal() - a2, a1.getImage());
        }
        public static BasicComplex operator *(BasicComplex a1, BasicComplex a2)
        {
            return new BasicComplex(a1.getReal() * a2.getReal() - a1.getImage() * a2.getImage(), a1.getImage() * a2.getReal() + a2.getImage() * a1.getReal());
        }
        public static BasicComplex operator *(double a1, BasicComplex a2)
        {
            return new BasicComplex(a1 * a2.getReal(), a1 * a2.getImage());
        }
        public static BasicComplex operator *(BasicComplex a1, double a2)
        {
            return new BasicComplex(a1.getReal() * a2, a1.getImage() * a2);
        }

        public static BasicComplex operator /(BasicComplex a1, BasicComplex a2)
        {
            return a1 * a2.inverse();
        }

        public static BasicComplex operator /(double a1, BasicComplex a2)
        {
            return a1 * a2.inverse();
        }
        public static BasicComplex operator /(BasicComplex a1, double a2)
        {
            return new BasicComplex(a1.getReal() / a2, a1.getImage() / a2);
        }

        public static bool operator ==(BasicComplex a1, BasicComplex a2)
        {
            return (((a1.getReal() == a2.getReal()) && (a1.getImage() == a2.getImage())));
            
        }

        public static bool operator !=(BasicComplex a1, BasicComplex a2) 
        {
            return (((a1.getReal() != a2.getReal()) || (a1.getImage() != a2.getImage()))); 
        }

        public override bool Equals(object o) 
        {
            BasicComplex a = (BasicComplex)o;
            return (((this.getReal() == a.getReal()) && (this.getImage() == a.getImage())));
        }

        public static BasicComplex ComplexSin(BasicComplex a)
        {
            double re = a.getReal();
            double im = a.getImage();
            return new BasicComplex(Math.Sin(re) * Math.Cosh(im), Math.Cos(re) * Math.Sinh(im));
        }

        public static BasicComplex ComplexCos(BasicComplex a) 
        {
            double re = a.getReal();
            double im = a.getImage();
            return new BasicComplex(Math.Cos(re) * Math.Cosh(im), -Math.Sin(re) * Math.Sinh(im));
        }

        public static BasicComplex ComplexTan(BasicComplex a) 
        {
            return ComplexSin(a) / ComplexCos(a);
        }

        public static BasicComplex ComplexSinh(BasicComplex a)
        {
            double re = a.getReal();
            double im = a.getImage();
            return new BasicComplex(Math.Sinh(re) * Math.Cos(im), Math.Cosh(re) * Math.Sin(im));
        }


        public static BasicComplex ComplexCosh(BasicComplex a) 
        {
            double re = a.getReal();
            double im = a.getImage();
            return new BasicComplex(Math.Cosh(re) * Math.Cos(im), Math.Sinh(re) * Math.Sin(im));
        }

        public static BasicComplex ComplexTanh(BasicComplex a) 
        {
            return ComplexSinh(a) / ComplexCosh(a);
        }

        public static BasicComplex[][,] changeArray21to12(BasicComplex[,][] array) 
        {

            BasicComplex[][,] dummy = new BasicComplex[array[0,0].Length][,];
            for (int i = 0; i < array[0, 0].Length; i++) 
            {
                dummy[i] = new BasicComplex[array.GetLength(0), array.GetLength(1)];
                for (int j = 0; j < array.GetLength(0); j++) 
                {
                    for (int k = 0; k < array.GetLength(1); k++) 
                    {
                        dummy[i][j, k] = new BasicComplex(array[j, k][i]);
                    }
                }
            }
            return dummy;
        }

        public static BasicComplex[,][] changeArray12to21(BasicComplex[][,] array)
        {

            BasicComplex[,][] dummy = new BasicComplex[array[0].GetLength(0),array[0].GetLength(1)][];
            for (int i = 0; i < array[0].GetLength(0); i++)
            {
                for (int j = 0; j < array[0].GetLength(1);j++ )
                {

                    dummy[i,j] = new BasicComplex[array.Length];
                
                    for (int k = 0; k < array.GetLength(0); k++)
                    {
                        dummy[i,j][k] = new BasicComplex(array[k][i,j]);
                    }
                }
            }
            return dummy;
        } 



        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public string ToStringComma() 
        {
            return this.getReal().ToString() + "," + this.getImage().ToString();
        }

        public string ToStringSpace()
        {
            return this.getReal().ToString() + " " + this.getImage().ToString();
        }

  

        public double getReal()
        {
            return this.r;
        }
        public double getImage()
        {
            return this.i;
        }
        public double mag()
        {
            return Math.Sqrt(r * r + i * i);
        }
        public double dbMag()
        {
            return 20 * Math.Log10(this.mag());
        }
        public double angleRadian()
        {
            return Math.Atan2(i, r);
        }
        public double angleDeg()
        {
            return Math.Atan2(i, r) / Math.PI * 180;
        }
 
        public BasicComplex addAngleDeg(double thetaDeg) //Degree指定
        {
            return new BasicComplex(this.mag(), this.angleDeg() + thetaDeg, "ma");  
        }


        public BasicComplex addAngleRadian(double thetaRad)//Radian指定
        {
            return new BasicComplex(this.mag(), (this.angleRadian() + thetaRad) / Math.PI * 180, "ma");
        }


        public BasicComplex inverse()
        {
            double dummy = r * r + i * i;
            return new BasicComplex(r / dummy, -i / dummy);
        }
    }
}
