using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HFSSMacro.Data
{
    class HFSSSetup
    {
        //solution type 1=DrivenModal, 2=EigenMode, 3=DrivenTerminal
        private int solutionType;

        //DrivenModal解析
        private double f0;
        private int numOfPasses;
        private double maxDeltaS;
        private int numOfPoints;
        private double startFreq;
        private double stopFreq;
        private double stepSize;

        //EigenMode解析
        private int numOfModes;
        private double minimumFreq;

        public HFSSSetup(int solutionType)
        {
            this.solutionType = solutionType;
        }

        public void setDrivenModalSetup(double f0, int numOfPasses, double maxDeltaS)
        {
            if (this.solutionType != 1) throw new Exception("SolutionTypeが違います");
            this.f0 = f0;
            this.numOfPasses = numOfPasses;
            this.maxDeltaS = maxDeltaS;
        }

        public void setSweepSetup(double startFreq, double stopFreq, int numOfPoints)
        {
            if (this.solutionType != 1) throw new Exception("SolutionTypeが違います");
            this.startFreq = startFreq;
            this.stopFreq = stopFreq;
            this.numOfPoints = numOfPoints;
        }

        public void setSweepSetup(double startFreq, double stopFreq, double stepSize)
        {
            if (this.solutionType != 1) throw new Exception("SolutionTypeが違います");
            this.startFreq = startFreq;
            this.stopFreq = stopFreq;
            this.stepSize=stepSize;
        }

        public void setEigenModeSetup(double minimumFreq, int numOfModes)
        {
            if (this.solutionType != 2) throw new Exception("SolutionTypeが違います");
            this.minimumFreq = minimumFreq;
            this.numOfModes = numOfModes;
        }
    }
}
