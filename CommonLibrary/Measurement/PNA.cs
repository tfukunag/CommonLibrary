using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Meta.Numerics;
using NationalInstruments.NI4882;
using NationalInstruments.VisaNS;
using CommonLibrary.Basic;


namespace CommonLibrary.Measurement
{
    public class PNA
    {
        //////////////////////////////////////////////////////////////////////////
        //プライベート変数
        //
        //
        //////////////////////////////////////////////////////////////////////////
        private GpibInstrument gpib;    //制御用GPIBオブジェクト
        private int _gpibAdrs;  //GPIBアドレス
        private double _startFreq=1e9;  //開始周波数(起動時は1GHz)
        private double _stopFreq=60e9;  //終了周波数(起動時は60GHz)
        private bool _smooth = false;   //スムージングの有無
        private bool _average = false;  //アベレージングの有無
        private int _averagingFactor = 0;    //アベレージ回数
        private int _measuringPoints = 201; //測定点数(起動時は201Point)
        private double _ifBandWidth = 1000;    //IFバンド幅(起動時は1kHz)
        private string[,] _measuringCatalog=null;    //測定トレース管理用
        private bool _nPortMeasurement = false; //  複数ポート測定中のみtrue
        private bool _averageFirstMeasurementFlag = false; //アベレージング測定時、初回のみアベレージを行う為のフラグ

        //////////////////////////////////////////////////////////////////////////
        //コンストラクタ
        //
        //
        //////////////////////////////////////////////////////////////////////////
        public PNA(short gpibAdrs)
        {
            gpib = new GpibInstrument(gpibAdrs);
            this._gpibAdrs = gpibAdrs;
        }



        //////////////////////////////////////////////////////////////////////////
        //基本
        //
        //
        //////////////////////////////////////////////////////////////////////////



        //GPIBをリモートへ
        public void gpibRemote()
        {
            gpib.remote();
        }

        //GPIBをローカルへ
        public void gpibLocal()
        {
            gpib.local();
        }

        //現在の状態を取得
        private void setCurrentState() 
        {
            try
            {
                gpib.setTimeout(25000);
                this._startFreq = this.getStartFreq();
                this.wait();

                this._stopFreq = this.getStopFreq();
                this.wait();

                this._measuringPoints = this.getMeasuringPoints();
                this.wait();

                this._ifBandWidth = this.getIFBandWidth();
                this.wait();

                this._smooth = this.getSmoothState();
                this.wait();


                this._average = this.getAveragingState();
                this.wait();


                this._averagingFactor = this.getAveragingFactor();
                this.wait();


                string[] dummy = gpib.query("CALC:PAR:CAT:EXT?").Replace("\n", "").Replace("\"", "").Split(',');

                this._measuringCatalog = new string[dummy.Length / 2, 2];
                for (int i = 0; i < dummy.Length / 2; i++)
                {
                    this._measuringCatalog[i, 0] = dummy[i * 2];
                    this._measuringCatalog[i, 1] = dummy[i * 2 + 1];
                }
            }
            catch (NullReferenceException) 
            {
                MessageBox.Show("GPIBの設定が正しく行われていません。");
            }
            
        }

        //measuringCatalogに保存された状態にPNAの状態を戻す
        public void setCatalogState()
        {
            gpib.send("DISPlay:WINDow1:STATE OFF");
            gpib.send("DISPlay:WINDow1:STATE ON");
            this.setFreqStrStp(this._startFreq, this._stopFreq);
            this.setMeasuringPoints(this._measuringPoints);

            for (int i = 0; i < this._measuringCatalog.GetLength(0); i++)
            {
                gpib.send("CALC:PAR:DEF:EXT '"+this._measuringCatalog[i,0]+"', '"+this._measuringCatalog[i,1]+"'" );
            }

            for (int i = 0; i < this._measuringCatalog.GetLength(0); i++) 
            {
                gpib.send("DISPlay:WINDow1:TRACe"+(i+1).ToString()+":FEED '" + this._measuringCatalog[i,0] + "'");
            }
        }


        //windowの初期化(S21のみ表示)
        public void cleanUpDisplay()
        {
            gpib.send("DISPlay:WINDow1:STATE OFF");
            this.wait();
            gpib.send("CALCulate1:PARameter:DEFine:EXT 'Meas',S21");
            this.wait();
            gpib.send("DISPlay:WINDow1:STATE ON");
            this.wait();
            gpib.send("DISPlay:WINDow1:TRACe1:FEED 'Meas'");

        }

        //次のコマンドまで待つ
        public void wait()
        {
            gpib.send("*WAI");
        }

        //作業中断
        public void abort()
        {
            gpib.send("ABOR");
        }

        //presetをかける
        public void preset()
        {
            gpib.send("SYST:PRES");
        }

        //IDの取得
        private string getIDnum()
        {
            return gpib.query("*IDN?");
        }




        //////////////////////////////////////////////////////////////////////////
        //データ取得関連
        //
        //
        //////////////////////////////////////////////////////////////////////////

        //周波数データ[GHz]の取得、double配列への格納
        public double[] getFreq()
        {
            this.setCurrentState();
            string a = "CALC:PAR:SEL '" + this._measuringCatalog[0, 0] + "'";
            gpib.setTimeout((int)(12500 * this._measuringPoints / this._ifBandWidth));
            
            gpib.send(a);
            if (!_average)
            {
                this.singleTrig();
                this.wait();
            }
            gpib.send("CALC:X?");
            string d = gpib.receive(10000000);
            string[] dummy = d.Split(',');
            double[] dummy2 = new double[dummy.Length];
            for (int i = 0; i < dummy.Length; i++)
            {
                dummy2[i] = double.Parse(dummy[i]);
            }
            this.continuousTrig();
            return dummy2;
            
        }
     
       
        
        

        //1ポートのデータ測定
        public BasicComplex[,][] getSParameter(int port1) 
        {
            this._nPortMeasurement = false;
            
            this.createTrace(port1);
            this.setReferencePosition(10);
            BasicComplex[,][] dummy = new BasicComplex[1, 1][];
            if(!this._average)
            {
                dummy[0,0] = this.getCurrentTraceSParameter(1);
            }
            else
            {
                dummy[0,0] = this.getAveragedCurrentTrace(1);
            }

            this.continuousTrig();
            return dummy;
        }

        //2ポートのデータ測定
        public BasicComplex[,][] getSParameter(int port1,int port2)
        {
            this._nPortMeasurement = true;

            this.createTraces(port1, port2);
            this.setReferencePosition(10);
            BasicComplex[,][] dummy;
            
            //配列の初期化
            dummy = new BasicComplex[2,2][];
            this.singleTrig();
            if (!this._average)
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        dummy[j, i] = this.getCurrentTraceSParameter(i * 2 + (j + 1));
                    }
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        if ((i==0)&&(j == 0)) _averageFirstMeasurementFlag = true;
                        else _averageFirstMeasurementFlag = false;
                        dummy[j, i] = this.getAveragedCurrentTrace(i * 2 + (j + 1));
                    }
                }
            }
            this.continuousTrig();
            return dummy;
        }

        
        //3ポートのデータ測定

        public BasicComplex[,][] getSParameter(int port1,int port2,int port3)
        {
            this._nPortMeasurement = true;
            this.createTraces(port1, port2, port3);
            this.setReferencePosition(10);
            BasicComplex[,][] dummy;
            int a = this.getMeasuringPoints();

            //配列の初期化
            dummy = new BasicComplex[3,3][];
            this.singleTrig();
            if (!this._average)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        

                        dummy[j, i] = this.getCurrentTraceSParameter(i * 3 + (j + 1));
                    }
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if ((i == 0) && (j == 0)) _averageFirstMeasurementFlag = true;
                        else _averageFirstMeasurementFlag = false;

                        dummy[j, i] = this.getAveragedCurrentTrace(i * 3 + (j + 1));
                    }
                }
            }
            this.continuousTrig();
            return dummy;
        }


        //4ポート
        public BasicComplex[,][] getSParameter()
        {
            this._nPortMeasurement = true;
            
            this.createTraces();
            this.setReferencePosition(10);
            BasicComplex[,][] dummy;
            int a = this.getMeasuringPoints();

            //配列の初期化
            dummy = new BasicComplex[4, 4][];
            this.singleTrig();
            if (!this._average)
            {

                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        dummy[j, i] = this.getCurrentTraceSParameter(i * 4 + (j + 1));
                    }
                }
            }

            else
            {

                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((i == 0) && (j == 0)) _averageFirstMeasurementFlag = true;
                        else _averageFirstMeasurementFlag = false;
                        dummy[j, i] = this.getAveragedCurrentTrace(i * 4 + (j + 1));
                    }
                }
            }
            this.continuousTrig();
            return dummy;
            
        }



        

        //アベレージング測定(private)
        private BasicComplex[] getAveragedCurrentTrace(int traceNum)
        {
            if (!this._average) { throw new Exception("アベレージング設定を行なって下さい。"); }
            else if (this._averagingFactor == 0) { throw new Exception("アベレージング回数の設定を行なって下さい。"); }
            else
            {
                gpib.setTimeout((int)2000 * this._measuringPoints * this._averagingFactor / (int)this._ifBandWidth);
                //複数トレースの測定時、初回のみアベレージし、その結果をホールドする
                if (_averageFirstMeasurementFlag)
                {
                    gpib.send("INIT:CONT ON");
                    this.wait();
                    //groupトリガーの設定(アベレージ回数+1)
                    gpib.send("SENS:SWE:GRO:COUN " + (this._averagingFactor + 1).ToString());
                    this.wait();
                    gpib.send("SENS:AVER:CLE");
                    this.wait();
                    this.averagingIsOn(true);
                    this.wait();
                    //groupトリガーでアベレージ回数分トリガーを回す
                    gpib.send("SENS:SWE:MODE GRO");
                    this.wait();
                    this.holdTrig();
                }
                //トリガー終了後、hold状態でデータを取得
                BasicComplex[] sParam = this.getCurrentTraceSParameter(traceNum);
                this.wait();

                return sParam;
            }
        }

        //Y軸データを取得、BasicComplex配列で返す(private)
        private BasicComplex[] getCurrentTraceSParameter(int traceNum)
        {
            this.traceSelect(traceNum);
            this.wait();

            if (_average)
            {
                gpib.setTimeout((int)(5000 * this._measuringPoints * (_averagingFactor+10) / this._ifBandWidth));

            }
            else
            {
                gpib.setTimeout((int)(5000 * this._measuringPoints / this._ifBandWidth));
            }

            if ((!_nPortMeasurement) && (!this._average))
            {
                this.singleTrig();
                this.wait();
            }
            gpib.send("CALC:DATA? SDATA");//データ取得(ri)            
            string d = gpib.receive(25 * this._measuringPoints * 2);
            string[] dummy = d.Split(',');
            BasicComplex[] dummy2 = new BasicComplex[dummy.Length / 2];
            for (int i = 0; i < dummy.Length / 2; i++)
            {
                dummy2[i] = new BasicComplex(double.Parse(dummy[2 * i]), double.Parse(dummy[2 * i + 1]));
            }
            //continiusには戻さない

            return dummy2;
        }

        //////////////////////////////////////////////////////////////////////////
        //各種設定
        //
        //
        //////////////////////////////////////////////////////////////////////////




        //周波数の設定(Start,Stop指定)
        public void setFreqStrStp(double start, double stop)
        {
            this.holdTrig();
            if (start < stop)
            {
                gpib.send("SENS:FREQ:STAR " + start.ToString());
                gpib.send("SENS:FREQ:STOP " + stop.ToString());
                this._startFreq = start;
                this._stopFreq = stop;
            }
            else if (stop < start)
            {
                gpib.send("SENS:FREQ:STAR " + stop.ToString());
                gpib.send("SENS:FREQ:STOP " + start.ToString());
                this._startFreq = stop;
                this._stopFreq = stop;
            }
            else
            {
                throw new Exception("入力エラーです。");
            }
            this.continuousTrig();
        }

        //周波数の設定(Center,Span指定)
        public void setFreqCenSpn(double center, double span)
        {
            this.holdTrig();
            gpib.send("SENS:FREQ:CENT " + center.ToString());
            gpib.send("SENS:FREQ:SPAN " + span.ToString());
            this._startFreq = center - span / 2;
            this._stopFreq = center + span / 2;
            this.continuousTrig();
        }

        //開始周波数の取得
        public double getStartFreq()
        {
            return double.Parse(gpib.query("SENS:FREQ:STAR?"));
        }

        //終了周波数の取得
        public double getStopFreq() 
        {
            return double.Parse(gpib.query("SENS:FREQ:STOP?"));
        }

        //ポイント数の設定
        
        public void setMeasuringPoints(int points)
        {
            this.holdTrig();
            gpib.send("SENS:SWE:POIN " + points.ToString());
            this._measuringPoints = points;
            this.continuousTrig();
        }


        //ポイント数の取得
        public int getMeasuringPoints()
        {
            return int.Parse(gpib.query("SENS:SWE:POIN?"));
        }


       

        //スムージングのON,OFF

        public void smoothIsOn(bool a) 
        {
            if (a)
            {
                this._smooth = true;
                gpib.send("CALC:SMO ON");
            }
            else 
            {
                this._smooth = false;
                gpib.send("CALC:SMO OFF");
            }
        }

       
       

        //スムージング状態の取得

        public bool getSmoothState() 
        {
            int a = int.Parse(gpib.query("CALC:SMO:STAT?"));
            if (a == 1)  return true; 
            else return false;
        }

        //アベレージングの設定

        public void averagingIsOn(bool a)
        {
            if (a)
            {
                this._average = true;
                gpib.send("SENS:AVER ON");
            }
            else
            {
                this._average = false;
                gpib.send("SENS:AVER OFF");
               
            }
        }

        public void setAveragingFactor(int factor) 
        {
            gpib.send("SENS:AVER:COUN " + factor.ToString());
            this._averagingFactor = factor;
        }

        public bool getAveragingState() 
        {
            int a = int.Parse(gpib.query("SENS:AVER:STAT?"));
            if (a == 1) return true;
            else return false;
        }

        public int getAveragingFactor() 
        {
            return int.Parse(gpib.query("SENS:AVER:COUN?"));
        }

        //IFバンド幅の設定
        public void setIFBandWidth(double BandWidth)
        {
            this.holdTrig();
            gpib.send("SENS:BWID " + BandWidth.ToString());
            this._ifBandWidth = BandWidth;
            this.continuousTrig();
        }

        //IFバンド幅の取得
        public double getIFBandWidth() 
        {
            return double.Parse(gpib.query("SENS:BWID?"));
        }
        
        //トリガーをContinuousに設定
        public void continuousTrig()
        {
            gpib.send("SENS:SWE:MODE CONT");
        }
        //トリガーをHoldに設定
        public void holdTrig()
        {
            gpib.send("SENS:SWE:MODE HOLD");
        }
        //トリガーをSingleに設定
        public void singleTrig()
        {
            gpib.send("SENS:SWE:MODE SING");
        }

       

        //一括設定
        public void setMeasuring(int portNum,int[] usingPortNum,double startF,double stopF,int measPoints,double ifBW,bool smooth,bool average,int averageFactor) 
        {
            if (portNum != usingPortNum.Length)
            {
                throw new Exception("ポート数が正しく設定されていません。");
            }
            else
            {
                this.setFreqStrStp(startF, stopF);
                this.setMeasuringPoints(measPoints);
                this.setIFBandWidth(ifBW);
                this.smoothIsOn(smooth);

                if (portNum == 1)
                {
                    this.createTrace(usingPortNum[0]);
                }
                else if (portNum == 2) 
                {
                    this.createTraces(usingPortNum[0], usingPortNum[1]);
                }
                else if (portNum == 3)
                {
                    this.createTraces(usingPortNum[0], usingPortNum[1],usingPortNum[2]);
                }
                else if(portNum == 4)
                {
                    this.createTraces();
                }
                this.averagingIsOn(average);
                this.setAveragingFactor(averageFactor);
                this.setReferencePosition(10);
                this.gpib.setTimeout(600000);
            }
        }




        //////////////////////////////////////////////////////////////////////////
        //画面表示
        //
        //
        //////////////////////////////////////////////////////////////////////////
        


        //ポート数に合わせてトレースを作成するメソッド

        //1ポート
        private void createTrace(int usingPort1)
        {

            //gpib.send("DISPlay:WINDow1:STATE OFF");//windowの初期化
            gpib.send("CALC:PAR:DEL:ALL");  //traceの消去

            //traceの生成
            gpib.send("CALCulate1:PARameter:DEFine:EXT 'Meas1',S"+usingPort1.ToString()+usingPort1.ToString());
            gpib.send("DISPlay:WINDow1:STATE OFF");//windowの初期化
            gpib.send("DISPlay:WINDow1:STATE ON");//window1を表示

            //生成したtraceをwindow1に表示
            gpib.send("DISPlay:WINDow1:TRACe1:FEED 'Meas1'");
            
        }

        //2ポート
        private void createTraces(int usingPort1, int usingPort2)
        {

            if (usingPort1 == usingPort2)
            {
                throw new Exception("ポート番号の設定が正しく行われていません。");
            }
            else
            {
                gpib.send("CALC:PAR:DEL:ALL");  //traceの消去
                int[] ports = new int[2];
                ports[0] = usingPort1;
                ports[1] = usingPort2;
                Array.Sort(ports);

                //traceの生成

                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        gpib.send("CALCulate1:PARameter:DEFine:EXT 'Meas" + (2 * i + (j + 1)).ToString() + "',S" + ports[i].ToString() + ports[j].ToString());
                    }
                }


                gpib.send("DISPlay:WINDow1:STATE OFF");//windowの初期化
                gpib.send("DISPlay:WINDow1:STATE ON");//window1を表示

                //生成したtraceをwindow1に表示
                int measNumber = 1;
                for (int i = 1; i < 3; i++)
                {
                    for (int j = 1; j < 3; j++)
                    {
                        gpib.send("DISPlay:WINDow1:TRACe" + measNumber.ToString() + ":FEED 'Meas" + measNumber.ToString() + "'");
                        measNumber++;
                    }
                }
            }
            

        }


        //3ポート
        private void createTraces(int usingPort1, int usingPort2,int usingPort3)
        {
            if ((usingPort1 == usingPort2)||(usingPort1 == usingPort3)||(usingPort2==usingPort3))
            {
                throw new Exception("ポート番号の設定が正しく行われていません。");
            }
            else
            {

                gpib.send("CALC:PAR:DEL:ALL");  //traceの消去
                int[] ports = new int[3];
                ports[0] = usingPort1;
                ports[1] = usingPort2;
                ports[2] = usingPort3;
                Array.Sort(ports);



                //traceの生成

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        gpib.send("CALCulate1:PARameter:DEFine:EXT 'Meas" + (3 * i + (j + 1)).ToString() + "',S" + ports[i].ToString() + ports[j].ToString());
                    }
                }


                gpib.send("DISPlay:WINDow1:STATE OFF");//window1を表示
                gpib.send("DISPlay:WINDow1:STATE ON");//window1を表示

                //生成したtraceをwindow1に表示
                int measNumber = 1;
                for (int i = 1; i < 4; i++)
                {
                    for (int j = 1; j < 4; j++)
                    {
                        gpib.send("DISPlay:WINDow1:TRACe" + measNumber.ToString() + ":FEED 'Meas" + measNumber.ToString() + "'");
                        measNumber++;
                    }
                }
            }
            

        }

        //4ポート
        private void createTraces()
        {            
            gpib.send("CALC:PAR:DEL:ALL");  //traceの消去
            
            //traceの生成

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    gpib.send("CALCulate1:PARameter:DEFine:EXT 'Meas" + (4 * i + (j + 1)).ToString() + "',S" + (i+1).ToString() + (j+1).ToString());
                }
            }


            gpib.send("DISPlay:WINDow1:STATE OFF");//windowの初期化
            gpib.send("DISPlay:WINDow1:STATE ON");//window1を表示

            //生成したtraceをwindow1に表示
            int measNumber = 1;
            for (int i = 1; i < 5; i++)
            {
                for (int j = 1; j < 5; j++)
                {
                    gpib.send("DISPlay:WINDow1:TRACe" + measNumber.ToString() + ":FEED 'Meas" + measNumber.ToString() + "'");
                    measNumber++;
                }
            }
            this.setReferencePosition(10);


        }

        //ポート番号を配列で指定(nポート対応)
        private void createTraces(int[] usingPort)
        {

            gpib.send("DISPlay:WINDow1:STATE OFF");//windowの初期化
            gpib.send("CALC:PAR:DEL:ALL");  //traceの消去
            Array.Sort(usingPort);

            //traceの生成


            for (int i = 0; i < usingPort.Length; i++)
            {
                for (int j = 0; j < usingPort.Length; j++)
                {
                    gpib.send("CALCulate1:PARameter:DEFine:EXT 'Meas" + (usingPort.Length * i + (j + 1)).ToString() + "',S" + usingPort[i].ToString() + usingPort[j].ToString());
                }
            }



            gpib.send("DISPlay:WINDow1:STATE ON");//window1を表示

            //生成したtraceをwindow1に表示
            int measNumber = 1;
            for (int i = 1; i < usingPort.Length+1; i++)
            {
                for (int j = 1; j < usingPort.Length+1; j++)
                {
                    gpib.send("DISPlay:WINDow1:TRACe" + measNumber.ToString() + ":FEED 'Meas" + measNumber.ToString() + "'");
                    measNumber++;
                }
            }
            


        }




        //traceの選択
        private void traceSelect(int TNum)
        {
            gpib.send("CALC:PAR:MNUM " + TNum.ToString());
        }
       
        //Y軸をautoscaleする
        private void autoScale()
        {
            gpib.send("DISP:WIND:Y:AUTO");
        }

        //referencePositionの設定
        public void setReferencePosition(int division) 
        {
            gpib.send("DISPlay:WINDow:TRACe:Y:RPOS "+division.ToString());
        }
    }

}

