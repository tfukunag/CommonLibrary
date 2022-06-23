using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using HFSSMacro.Data;

namespace HFSSMacro.FileIO
{
    public class HFSSFile
    {
        private HFSSModel[] model;
        private string[] line;
        public HFSSFile (string file)
        {
            try
            {
                StreamReader sr = new StreamReader(file);
                line = sr.ReadToEnd().Split('\n');

                List<int> start = new List<int>();
                List<int> stop = new List<int>();

                for (int i = 0; i < line.Length; i++)
                {
                    line[i] = line[i].Trim();
                    if (line[i].ToLower().StartsWith("$begin \'hfssmodel\'"))
                    {
                        start.Add(i);
                    }
                    else if (line[i].ToLower().StartsWith("$end \'hfssmodel\'"))
                    {
                        stop.Add(i);
                    }
                }

                int[] modelStart = start.ToArray();
                int[] modelStop = stop.ToArray();

                if (modelStart.Length != 0)
                {
                    model = new HFSSModel[modelStart.Length];
                    List<string> variableName = new List<string>();
                    List<string> variableUnit = new List<string>();
                    List<double> variableValue = new List<double>();

                    for (int i = 0; i < modelStart.Length; i++)
                    {
                        int iDummy = 0;
                        for (int j = modelStart[i]; j < modelStop[i]; j++)
                        {
                            if ((line[j].ToLower().StartsWith("name=\'")) && (iDummy == 0))
                            {
                                model[i] = new HFSSModel(line[j].Split('\'')[1]);
                            }
                            else if (line[j].ToLower().StartsWith("VariableProp(\'"))
                            {
                                string[] dummy = line[j].Split('\'');
                                variableName.Add(dummy[1]);
                                int numOfChar = dummy[7].Length;
                                int kDummy = 0;
                                double dDummy=double.MinValue;
                                for (int k = 1; k < numOfChar; k++)
                                {
                                    if (double.TryParse(dummy[7].Substring(0, k), out dDummy)) kDummy = k;
                                }
                                variableValue.Add(double.Parse(dummy[7].Substring(0, kDummy)));
                                variableUnit.Add(dummy[7].Substring(kDummy));
                            }

                        }
                        model[i].setVariableName((string[])variableName.ToArray());
                        model[i].setvariabletUnit((string[])variableUnit.ToArray());
                        model[i].setValue((double[])variableValue.ToArray());
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(),"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
    }
}
