using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HFSSMacro.Data;
using System.IO;

namespace HFSSMacro.Data
{
    public class HFSSModel
    {
        private string name;
        private string[] variableName;
        private double[] value;
        private string[] unit;
        private HFSSSetup[] setup;

        public HFSSModel(string name)
        {
            this.name = name;
        }

        public void setVariableName(string[] variableName)
        {
            this.variableName = variableName;
        }

        public void setValue(double[] value)
        {
            this.value = value;
        }

        public void setvariableValue(int variableIndex, double variableValue)
        {
            this.value[variableIndex] = variableValue;
        }

        public void setvariabletUnit(int variableIndex, string variableUnit)
        {
            this.unit[variableIndex] = variableUnit;
        }

        public void setvariabletUnit(string[] variableUnit)
        {
            this.unit= variableUnit;
        }

        public string getHfssFormatString(int variableIndex)
        {
            return "				VariableProp(\'" + variableName[variableIndex] + "\', \'UD\', \'\', \'" + value[variableIndex].ToString() + unit[variableIndex] + "\')";
        }

        public void writeHfssFormatString(StreamWriter writer)
        {
            for (int i = 0; i < this.variableName.Length; i++)
            {
                writer.WriteLine(this.getHfssFormatString(i));
            }
        }

    }
}
