using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.HFSS
{
    public class HFSSSolutionSetup
    {
        private string name;
        private string solutiontype;

        public HFSSSolutionSetup(string[] setupText) 
        {
            this.name = setupText[0].Trim().Split(new char[1] { '\'' })[1];
            this.solutiontype = setupText[2].Trim().Split(new char[1] { '\'' })[1];
        }

        public string SolutionName 
        {
            get { return this.name; }
        }

        public string SolutionType
        {
            get { return this.solutiontype; }
        }

        public string[] scriptAnalyze() 
        {
            string[] dummy = new string[2];
            dummy[0] = "oProject.Save";
            dummy[1] =  "oDesign.Analyze \"" + this.SolutionName + "\"";
            return dummy;
        }


    }
}
