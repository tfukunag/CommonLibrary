using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Basic;

namespace CommonLibrary.Transmission
{
    public class IdealTransmissionLine
    {
        private double z0;
        private double er;
        private double mur;
        private double l0;
        public IdealTransmissionLine(double z0,double er,double mur,double l0)
        {
            this.z0 = z0;
            this.er = er;
            this.mur = mur;
            this.l0 = l0;
        }
        public BasicComplex inputImpedance(BasicComplex zl, double f0)
        {
            double theta0 = 2 * Math.PI * f0 / 2.9979458E8 *Math.Sqrt(er*mur)* l0;
            return (zl + new BasicComplex(0, z0 * Math.Tan(theta0))) / (z0 + new BasicComplex(0, Math.Tan(theta0))*zl) * z0;
        }
    }
}
