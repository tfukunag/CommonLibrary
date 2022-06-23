using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.HFSS
{
    public class HFSSModel
    {
        private string name;

        private string[] variableName;
        private double[] variableValue;
        private string[] variableValueText;
        private string[] variableUnit;
        private bool[] variableByAnother;

        private int[] setupWritingStartIndex;
        private string[] solveSetupTextAll;
        private string[][] setupText;
        private List<HFSSSolutionSetup> solutionSetups;
        private HFSSSolutionSetup[] setups = null;
        private int numOfSetups = 0;

        public HFSSModel(string modelName) 
        {
            this.name = modelName;
        }

        public void setVariableName(string[] variableName)
        {
            this.variableName = variableName;
        }

        public void setValue(double[] value)
        {
            this.variableValue = value;
        }

        public void setvariableValue(int variableIndex, double variableValue)
        {
            this.variableValue[variableIndex] = variableValue;
        }

        public void setvariabletUnit(int variableIndex, string variableUnit)
        {
            this.variableUnit[variableIndex] = variableUnit;
        }

        public void setvariabletUnit(string[] variableUnit)
        {
            this.variableUnit = variableUnit;
        }






        public HFSSModel(string[] modelText)
        {
            int NumOfVariable = 0;

            //変数数・setup数の確認
            for (int i = 0; i < modelText.Length; i++)
            {
                //変数数の確認
                if ((modelText[i].Trim().StartsWith("VariableProp")))//変数に関する記述のある行
                {
                    NumOfVariable++;
                }

                //setup数の確認
                if (modelText[i].Trim().StartsWith("$begin \'SolveSetups\'"))//setupに関する記述のある行
                {
                    //setup全体の記述文を抽出
                    int a= 0;
                    do
                    {
                        a++;
                    }
                    while (!modelText[i+a].Trim().StartsWith("$end \'SolveSetups\'"));
                    
                    solveSetupTextAll = new string[a];
                    
                    int b = 0;
                    do
                    {
                        solveSetupTextAll[b] = modelText[i + b];
                        if (solveSetupTextAll[b].Trim().StartsWith("SetupType=")) numOfSetups++;

                        b++;
                    }
                    while (!modelText[i+b].Trim().StartsWith("$end \'SolveSetups\'"));

                }
            }


            //各setupの記述文を抽出する準備
            setupText = new string[numOfSetups][];

            int setupNumber = 0;

            //setup文を抽出する
            for (int i = 0; i < solveSetupTextAll.Length; i++) 
            {
                //setupの書き出しを識別するため、2行後のワード"SetupType"で判別
                if (solveSetupTextAll[i].Trim().StartsWith("SetupType=")) 
                {
                    int a = 2;

                    
                    string dumName = solveSetupTextAll[i - 2].Trim().Split(new char[1] { '\'' })[1];//終了部分識別用

                    do
                    {
                        a++;
                    }
                    while (!HasString(solveSetupTextAll[i + a - 2],dumName));
                    setupText[setupNumber] = new string[a];
                    for (int j = 0; j < a; j++) 
                    {
                        setupText[setupNumber][j] = solveSetupTextAll[i - 2 + j];
                        
                    }
                    setupNumber++;
                }

            }

            if (setupNumber != 0) 
            {
                setups = new HFSSSolutionSetup[setupNumber];
                for(int i = 0;i<setupNumber;i++)
                {
                    setups[i] = new HFSSSolutionSetup(setupText[i]);
                }
            }
            //変数記述(string)配列の作成
            variableName = new string[NumOfVariable];
            variableValue = new double[NumOfVariable];
            variableUnit = new string[NumOfVariable];
            variableValueText = new string[NumOfVariable];
            variableByAnother = new bool[NumOfVariable]; 
            string[] variableText = new string[NumOfVariable];

            int k = 0;
            for (int i = 0; i < modelText.Length; i++)
            {
                name = modelText[2].Trim().Split(new char[1] { '\'' })[1];


                if ((modelText[i].Trim().StartsWith("VariableProp")))
                {
                    

                        variableText[k] = modelText[i];
                        variableName[k] = variableText[k].Trim().Split(new char[1] { '\'' })[1];
                        variableValueText[k] = variableText[k].Trim().Split(new char[1] { '\'' })[7];

                        k++;
                    
                }

            }

            //変数値の値と単位の分離
            for (int i = 0; i < NumOfVariable; i++)
            {
                //変数が他変数の計算で表現されたものかを識別し、そうでない場合は値と単位で分離する
                if (!HasString(variableValueText[i], variableName))                
                {
                    variableByAnother[i] = false; 
                    char[] dummy = variableValueText[i].ToCharArray();
                    int length = 0;
                    do
                    {
                        length++;
                    } while ((dummy[length] == '0') || (dummy[length] == '1') || (dummy[length] == '2') || (dummy[length] == '3') || (dummy[length] == '4') || (dummy[length] == '5') || (dummy[length] == '6') || (dummy[length] == '7') || (dummy[length] == '8') || (dummy[length] == '9') || (dummy[length] == '-') || (dummy[length] == '.'));
                

                //double配列とstring配列に格納
                variableValue[i] = double.Parse(variableValueText[i].Substring(0, length));
                variableUnit[i] = variableValueText[i].Substring(length, dummy.Length - length);
                }

                //他変数の計算で表された変数は分離せず、stringのみで管理する
                else
                {
                    variableValue[i] = 0;
                    variableUnit[i] = "";
                    variableByAnother[i] = true; 
                }
            }

        }

       
        //変数の設定
        public void setVariables(int indexOfVariable, string newName, double newValue, string newUnit) 
        {
            //名前と単位は""の場合変更しない
            if (newName != "") this.variableName[indexOfVariable] = newName;
            if (newUnit != "") this.variableUnit[indexOfVariable] = newUnit;
            this.variableValue[indexOfVariable] = newValue;

            if (newUnit != "") this.variableValueText[indexOfVariable] = newValue.ToString() + newUnit;
            else this.variableValueText[indexOfVariable] = newValue.ToString() + VariableUnit[indexOfVariable];
        }


        //変数の設定(単位は変更しない)
        public void setVariables(int indexOfVariable, string newName, double newValue)
        {
            
            if (newName != "") this.variableName[indexOfVariable] = newName;
            
            this.variableValue[indexOfVariable] = newValue;
            this.variableValueText[indexOfVariable] = newValue.ToString() + VariableUnit[indexOfVariable];
        }

        public void setSolution(string name) 
        {

        }





        public string ModelName 
        {
            get { return this.name; } 
        }

        public string[] VariableName
        {
            get{return this.variableName;}
        }

        public double[] VariableValue 
        {
            get { return this.variableValue; }
        }

        public string[] VariableUnit
        {
            get { return this.variableUnit; }
        }

        public bool[] VariableByAnotherVar 
        {
            get { return this.variableByAnother; }
        }

        public int NumOfSetups 
        {
            get { return this.numOfSetups; }
        }

        //SolutionSetupの名前を引き出す

        public string[] getSolutionSetupNames()
        {
            if (this.NumOfSetups != 0)
            {
                string[] names = new string[this.NumOfSetups];
                for (int i = 0; i < NumOfSetups; i++)
                {
                    names[i] = setups[i].SolutionName;
                }
                return names;
            }
            else return null;
        }

        //文字列操作用(targetがwordを含めばtrueを返す)
        private static bool HasString(string target, string word)
        {
            if (word == "")
                return false;
            if (target.IndexOf(word) >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //文字列操作用(targetが配列wordのうちどれかを含めばtrueを返す)

        private static bool HasString(string target, string[] word)
        {
            bool a = false;
            if (word == null)
                a = false;

            int dum = 0;
            for (int i = 0; i < word.Length; i++)
            {
                if (target.IndexOf(word[i]) >= 0)
                {
                    dum++;
                }
            }

            if (dum != 0) a = true;
            else a = false;
            return a;
        }

        public string hfssFormatString(int variableIndex) 
        {
            return "\t\t\t\tVariableProp(\'" + variableName[variableIndex] + "\', \'UD\', \'\', \'" + VariableValue[variableIndex].ToString() + variableUnit[variableIndex] + "\')";
        }




        public string scriptSettingVar(int IndexOfVariable, double newValue)
        {
            return "oDesign.ChangeProperty Array(\"NAME:AllTabs\", Array(\"NAME:LocalVariableTab\", Array(\"NAME:PropServers\", \"LocalVariables\"), Array(\"NAME:ChangedProps\", Array(\"NAME:"+variableName[IndexOfVariable]+"\", \"Value:=\", \""+newValue.ToString()+variableUnit[IndexOfVariable]+"\"))))";
        }

        public string scriptSettingVar(int IndexOfVariable)
        {
            return "oDesign.ChangeProperty Array(\"NAME:AllTabs\", Array(\"NAME:LocalVariableTab\", Array(\"NAME:PropServers\", \"LocalVariables\"), Array(\"NAME:ChangedProps\", Array(\"NAME:" + variableName[IndexOfVariable] + "\", \"Value:=\", \"" + variableValue[IndexOfVariable].ToString() + variableUnit[IndexOfVariable] + "\"))))";
        }


        public string scriptModelSetting() 
        {
            return "Set oDesign = oProject.SetActiveDesign(\"" + this.ModelName + "\")";
        }

        public string[] scriptAnalyze(int setup) 
        {
            return this.setups[setup].scriptAnalyze();
        }
    }
}
