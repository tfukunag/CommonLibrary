using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.HFSS;
using System.IO;

namespace CommonLibrary.HFSS
{
    public class HFSSFile
    {
        private string[] AllLines; //ファイル全文
        //private string[] AllLinesChanged; //

        private HFSSModel[] designs; 
        //private int[] modelWritingStartIndex; //モデル記述が始まる行数を示すインデックス
        
        //private string[][] modelText; //[モデル番号]のモデルの記述文全文

        //private int[][] variableIndex; //[モデル番号][変数番号]が、AllLinesの何行目で記述されているかを示すインデックス
        //private string[][] variableText; //[モデル番号][変数番号]の記述文

        

        private string[][] modelSolutionSetupName; //[モデル番号][セットアップ番号]のセットアップ名
        private string filePath; //ファイルパス
        private string fileName; //ファイル名

        public HFSSFile(string file) //コンストラクタ(ファイルから)
        {
            filePath = file;
            string[] dummy = filePath.Split(new char[1]{'\\'});
            fileName = dummy[dummy.Length - 1].Remove(dummy[dummy.Length - 1].ToCharArray().Length-5);

            StreamReader sr = new StreamReader(file);

            AllLines = sr.ReadToEnd().Split('\n');
            

            List<int> start = new List<int>();
            List<int> stop = new List<int>();

            for (int i = 0; i < AllLines.Length; i++)
            {
                AllLines[i] = AllLines[i].Trim();
                if (AllLines[i].ToLower().StartsWith("$begin \'hfssmodel\'"))
                {
                    start.Add(i);
                }
                else if (AllLines[i].ToLower().StartsWith("$end \'hfssmodel\'"))
                {
                    stop.Add(i);
                }
            }

            int[] modelStart = start.ToArray();
            int[] modelStop = stop.ToArray();

            if (modelStart.Length != 0)
            {
                designs = new HFSSModel[modelStart.Length];
                List<string> variableName = new List<string>();
                List<string> variableUnit = new List<string>();
                List<double> variableValue = new List<double>();


                







                    for (int i = 0; i < modelStart.Length; i++)
                    {

                        List<int> setupstart = new List<int>();
                        List<int> setupstop = new List<int>();

                        for (int j = modelStart[i]; j < modelStop[i]; j++)
                        {
                            if (AllLines[j].Trim().StartsWith("SetupType="))
                            {
                                string dumName = AllLines[j - 2].Trim().Split(new char[1] { '\'' })[1];//終了部分識別用

                                setupstart.Add(j-2);

                            }
                            if()
                                while (!HasString(AllLines[j + a - 2], dumName));
                                setupText[j] = new string[a];
                                for (int j = 0; j < a; j++)
                                {
                                    setupText[setupNumber][j] = solveSetupTextAll[i - 2 + j];

                                }
                                setupNumber++;
                            }
                        }


                        int iDummy = 0;
                        for (int j = modelStart[i]; j < modelStop[i]; j++)
                        {
                            if ((AllLines[j].ToLower().StartsWith("name=\'")) && (iDummy == 0))
                            {
                                designs[i] = new HFSSModel(AllLines[j].Split('\'')[1]);
                            }
                            else if (AllLines[j].ToLower().StartsWith("VariableProp(\'"))
                            {
                                string[] dum = AllLines[j].Split('\'');
                                variableName.Add(dummy[1]);
                                int numOfChar = dummy[7].Length;
                                int kDummy = 0;
                                double dDummy = double.MinValue;
                                for (int k = 1; k < numOfChar; k++)
                                {
                                    if (double.TryParse(dummy[7].Substring(0, k), out dDummy)) kDummy = k;
                                }
                                variableValue.Add(double.Parse(dummy[7].Substring(0, kDummy)));
                                variableUnit.Add(dummy[7].Substring(kDummy));






                            }














                        }
                        designs[i].setVariableName((string[])variableName.ToArray());
                        designs[i].setvariabletUnit((string[])variableUnit.ToArray());
                        designs[i].setValue((double[])variableValue.ToArray());
                    }
            }

        }

            //モデル数の確認
            
   
            //int models = 0;


            ////全体でforを回す
            //for (int i = 0; i < AllLines.Length; i++)
            //{
            //    if (AllLines[i].Trim().StartsWith("$begin \'HFSSModel\'")) models++;
            //}

            
            ////各変数配列の決定
            //designs = new HFSSModel[models];
            //modelWritingStartIndex = new int[models];
            //modelText = new string[models][];
            //variableText = new string[models][];
          
            //modelSolutionSetupName = new string[models][];
            //variableIndex = new int[models][];
          

            //int modelNumber = 0;

            ////もう一度全体でfor
            //for (int i = 0; i < AllLines.Length; i++) 
            //{

            //    //モデル毎のテキスト抽出
            //    if (AllLines[i].Trim().StartsWith("$begin \'HFSSModel\'"))
            //    {
            //        modelWritingStartIndex[modelNumber] = i;
            //        int k = 0;

            //        do
            //        {
            //            k++;
            //        }
            //        while (!AllLines[i + k].Trim().StartsWith("$end \'HFSSModel\'"));

            //        modelText[modelNumber] = new string[k];

            //        int l = 0;

            //        do
            //        {
            //            modelText[modelNumber][l] = AllLines[i + l];
            //            l++;
            //        }
            //        while (!AllLines[i + l].Trim().StartsWith("$end \'HFSSModel\'"));

            //        modelNumber++;
            //    }

            //}


            ////各モデルごとにHFSSModelクラスで変数を抽出、その他記述変更用に必要な記述データを抽出
            //for (int i = 0; i < models; i++) 
            //{
            //    //変数の値はHFSSModelにて格納
            //    designs[i] = new HFSSModel(modelText[i]);

            //    int NumOfVariable = 0;

            //    //i番目モデルの変数数の確認
            //    for (int j = 0; j < modelText[i].Length; j++) 
            //    {
            //        if (modelText[i][j].Trim().StartsWith("VariableProp"))
            //        {
            //            NumOfVariable++;
            //        }
            //    }

            //    //i番目のモデルの変数インデックス(int)配列・変数記述(string)配列の作成
            //    variableIndex[i] = new int[NumOfVariable];
            //    variableText[i] = new string[NumOfVariable];


            //    int k = 0;
            //    for (int j = 0; j < modelText[i].Length; j++)
            //    {                    
                    

                    
            //        if (modelText[i][j].Trim().StartsWith("VariableProp"))
            //        {
                        
            //                 variableText[i][k] = modelText[i][j];
                    
            //                 variableIndex[i][k] = j + modelWritingStartIndex[i];
            //                 k++;
                       
            //        }
            //    }

                
            //}
            //for(int i = 0;i<designs.Length;i++)
            //    {
            //        if (designs[i].NumOfSetups != 0)
            //        {
            //            for (int j = 0; j < designs[i].NumOfSetups; j++)
            //            {
            //                modelSolutionSetupName[i] = designs[i].getSolutionSetupNames();
            //            }
            //        }
            //        else modelSolutionSetupName[i] = null;
            //    }
        }



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



        //変数値変更、記述変更
        public void changeVariables(int modelSelect, int variableSelect, double value, string unit)
        {
            this.designs[modelSelect].setVariables(variableSelect, "", value, unit);

            //string[] dummy = variableText[modelSelect][variableSelect].Split(new char[1] { '\'' });
            //dummy[7] = value.ToString() + unit;

            //string returnedText = "";

            //for (int i = 0; i < dummy.Length; i++)
            //{
            //    if (i < dummy.Length - 1) returnedText = returnedText + dummy[i] + "\'";
            //    else returnedText = returnedText + dummy[i];
            //}

            //variableText[modelSelect][variableSelect] = returnedText;
            //modelText[modelSelect][variableIndex[modelSelect][variableSelect] - modelWritingStartIndex[modelSelect]] = returnedText;
            //AllLinesChanged[variableIndex[modelSelect][variableSelect]] = returnedText;
        }


        //記述変更済みのデータをHFSSファイルとして出力する
        public void writeHFSSFile(string file) 
        {
            StreamWriter sw = new StreamWriter(file);

            if (designs.Length != 0)
            {
                int count = 0;
                for (int i = 0; i < AllLines.Length; i++)
                {
                    if (!AllLines[i].Trim().StartsWith("VariableProp")) sw.WriteLine(AllLines[i]);
                    else
                    {
                        for (int j = 0; j < designs[count].VariableName.Length; j++)
                        {
                            sw.WriteLine(designs[count].hfssFormatString(j));
                        }
                        count++;
                    }

                }
            }

            else
            {
                for (int i = 0; i < AllLines.Length; i++)
                {
                    sw.WriteLine(AllLines[i]);
                }
            }

            sw.Flush();
            sw.Close();
        }







        public string[] scriptHFSSFileInfo()
        {
            string[] dummy = new string[11];
            dummy[0] = "Dim oAnsoftApp";
            dummy[1] = "Dim oDesktop";
            dummy[2] = "Dim oProject";
            dummy[3] = "Dim oDesign";
            dummy[4] = "Dim oEditor";
            dummy[5] = "Dim oModule";
            dummy[6] = "Set oAnsoftApp = CreateObject(\"AnsoftHfss.HfssScriptInterface\")";
            dummy[7] = "Set oDesktop = oAnsoftApp.GetAppDesktop()";
            dummy[8] = "oDesktop.OpenProject \"" + filePath + "\"";
            dummy[9] = "oDesktop.RestoreWindow";
            dummy[10] = "Set oProject = oDesktop.SetActiveProject(\"" + fileName + "\")";

            return dummy;
        }

        public string scriptChangeVar(int model, int var) 
        {
            return this.designs[model].scriptSettingVar(var);
        }

        public string scriptModelSetting(int model) 
        {
            return this.designs[model].scriptModelSetting();
        }

        public string[] scriptAnalyze(int model, int setup) 
        {
            return this.designs[model].scriptAnalyze(setup);
        }



    }
}
