using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommonLibrary.Basic;
using CommonLibrary.Measurement;
using CommonLibrary.FileIO;
using CommonLibrary.Transmission;
using CommonLibrary.Resonator;
//using CommonLibrary.HFSS;
using System.IO;
using System.Windows;

using CommonLibrary.Output;
using Meta.Numerics;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        RectanglarGraph rg = new RectanglarGraph(0, 150, 10, 10);
        TouchStoneFile ts;
       
        private QCalc qc;
        private double[][] choosedData;
        private double[][] choosedLeft;
        private double[][] choosedRight;
        private LorentzCurveFit lcf;

        private int fittingCount = 0;

        public Form1()
        {

            InitializeComponent();
            rg.setGraphTitle("");
            rg.setXAxisTitle("Frequency[GHz]");
            rg.setYAxisTitle("S Parameter[dB]");
            rg.setTitleFontSize(24);
      

            rg.initSize(this.ClientSize.Width, this.ClientSize.Height - 150);
            this.Controls.Add(rg);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            rg.initSize(this.ClientSize.Width, this.ClientSize.Height - 150);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            //rg.removeAllData();

            ofd.InitialDirectory = "";

            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                TouchStoneFile tsf = new TouchStoneFile(ofd.FileName, 4);
                SParameter s = new SParameter(tsf);

                
                
               

                ofd.Dispose();
            }

            else if (ofd.ShowDialog() == DialogResult.Cancel)
            {
                ofd.Dispose();
            }



            //CylindricalResonator cr = new CylindricalResonator(20e-3, 30e-3, new DielectricMaterial(1.0));
            //double a = cr.teModeResoFreq(0, 1, 1);
            //double qside = cr.teModeResoSideQValue(0, 1, 1, 5.8e7);
            //double qtop = cr.teModeResoTopQValue(0, 1, 1, 5.8e7);

            //double b = 0;



            //Complex z = msl.inputImpedance(5e10, 1e-3, new Complex(0, 0));
            //double a = 0;
            //OpenFileDialog ofd = new OpenFileDialog();


            //ofd.InitialDirectory = @"C:\Documents and Settings\01605645\デスクトップ\QEB43KTOH";

            //ofd.RestoreDirectory = true;
            //if (ofd.ShowDialog() == DialogResult.OK)
            //{
            //    this.csv = new CsvRead(ofd.FileName);
            //    rg.addData(csv.getFreq(), csv.getSparam(), "data");
            //    ofd.Dispose();
            //}
            //else if (ofd.ShowDialog() == DialogResult.Cancel)
            //{
            //    ofd.Dispose();
            //}


            ////BasicComplex[,] a = new BasicComplex[3, 3];
            ////a[0, 0] = new BasicComplex(2, 1, "ri");
            ////a[0, 1] = new BasicComplex(1, 3, "ri");
            ////a[0, 2] = new BasicComplex(0, 0, "ri");
            ////a[1, 0] = new BasicComplex(4, 5, "ri");
            ////a[1, 1] = new BasicComplex(1, 1.5, "ri");
            ////a[1, 2] = new BasicComplex(2, 6, "ri");
            ////a[2, 0] = new BasicComplex(1.4, 0.8, "ri");
            ////a[2, 1] = new BasicComplex(5, 3.3, "ri");
            ////a[2, 2] = new BasicComplex(1, 1, "ri");

            ////BasicComplex a1 = new BasicComplex(1, 1);
            ////BasicComplex a2 = new BasicComplex(1, 1);

            ////bool dummy = (a1 == a2);
            ////bool dummy2 = a1.Equals(a2);

            ////ComplexMatrix c = new ComplexMatrix(a);
            ////ComplexMatrix c2 = new ComplexMatrix(a);

            ////ComplexMatrix cinv = ComplexMatrix.inverseMatrix(c);

            ////ComplexMatrix c3 = c / c2;
            ////ComplexMatrix c4 = c * cinv;
            ////double b = 1;

            //PNA pna = new PNA(16);
            //pna.gpibRemote();

            //BasicComplex[,][] a = pna.getSParameter(1,2,4);
            //double[] freq = pna.getFreq();

            //rg.addData(freq, a);


            //double[] a = pna.getFreq();
            //pna.getCurrentState();
            //pna.setIFBandWidth(100);
            //pna.getAveragingData(10,1);
            //pna.gpibLocal();
            //pna.wait();

            //TouchStoneFile ts = new TouchStoneFile(freq, a, 50);
            //ts.writeCsvFile("G:\\netanaData\\fromProgram3.csv");
            //pna.gpibLocal();

        }

        private void button2_Click(object sender, EventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();

            

            ofd.InitialDirectory = "";

            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.ts = new TouchStoneFile(ofd.FileName, 4);
                SParameter s = new SParameter(ts);
                //BasicComplex[,][] Sparam = s.getParameter();  
                //BasicComplex[] Sdd21 = new BasicComplex[Sparam[0,0].Length];
                //BasicComplex[] Scc21 = new BasicComplex[Sparam[0,0].Length];


                //for (int i = 0; i < Sparam[0, 0].Length; i++)
                //{

                //    Scc21[i] = (Sparam[0, 0][i] + Sparam[0, 1][i] + Sparam[1, 0][i] + Sparam[1, 1][i]) / 2;
                //    Sdd21[i] = (Sparam[0, 0][i] - Sparam[0, 1][i] - Sparam[1, 0][i] + Sparam[1, 1][i]) / 2;
                //}


                s.addDifferentialPair(0, 2);
                s.addDifferentialPair(1, 3);
                s.differentialConversion();
               

              
                //SParameter sopened = s.getOpenedOrShortedSParameter(1, true);
                //TouchStoneFile t = new TouchStoneFile(ts.getFreq(), s.getParameter(), 50);
                //t.writeTouchstone("C:\\Documents and Settings\\01605645\\デスクトップ\\_op.s1p");
                //t.writeCsvFile("C:\\Documents and Settings\\01605645\\デスクトップ\\a.csv");

                s.writeCsvFile(ofd.FileName + ".csv");


                ofd.Dispose();
            }

            //PNA pna = new PNA(16);
            //int ports = 4;
            //int[] portNum = new int[4];
            //portNum[0] = 2;
            //portNum[1] = 1;
            //portNum[2] = 3;
            //portNum[3] = 4;
            //double start = 35e9;
            //double stop = 60e9;
            //int points = 201;
            //double ifBW = 1000;
            //bool smooth = true;
            //bool average = false;
            //int averageFac = 0;

            //try
            //{
            //    pna.gpibRemote();
            //    pna.setMeasuring(ports, portNum, start, stop, points, ifBW, smooth, average, averageFac);
            //    pna.getSmoothState();
            //    pna.gpibLocal();
            //}
            //catch (Exception a) 
            //{
            //    MessageBox.Show(a.Message.ToString());
            //}

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //if (radioButton2.Checked)
            //{
            //    double[] X = this.choosedLeft[0];
            //    double[] Y = this.choosedLeft[1];
            //    rg.addData(X, Y, "左側抽出");
            //}
            


            //if (radioButton3.Checked)
            //{
            //    double[] X = this.choosedRight[0];
            //    double[] Y = this.choosedRight[1];
            //    rg.addData(X, Y, "右側抽出");
            //}
                    

            //OpenFileDialog ofd = new OpenFileDialog();

            //ofd.InitialDirectory = @"C:\Documents and Settings\01605645\デスクトップ\Q導出\touchstone";

            //ofd.RestoreDirectory = true;

            //ofd.ShowDialog();

            //TouchStoneFile ts = new TouchStoneFile(ofd.FileName, 2);


            //QCalc qc = new QCalc(ts.getParameter(), ts.getFreq());

            //double q = qc.get2portQL(38,40,5);

            //StreamWriter sw = new StreamWriter("C:\\test6.csv");
            ////for (int i = 0; i < x.Length; i++)
            ////{
            ////    YFit[i] = coef[0] + coef[1] / (Math.Pow((x[i] - coef[2]), 2) + coef[3]);
            ////    sw.WriteLine(x[i].ToString() + "," + y[i].ToString() + "," + YFit[i].ToString());
            ////}
            ////sw.Flush();
            ////sw.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            //rg.removeAllData();

            ofd.InitialDirectory = "";

            ofd.RestoreDirectory = true;
            if(ofd.ShowDialog()==DialogResult.OK)
            {
                this.ts = new TouchStoneFile(ofd.FileName, 4);
                //BasicComplex[,][] dif = parameterConverter.ToDifferentialParameter(ts.getParameter(), false);
                rg.addData(ts.getFreq(),ts.getParameter()[2,0],"Sdd21(Nwa上)");
                rg.addData(ts.getFreq(), ts.getParameter()[3,1], "Scc21(Nwa上)");

                ofd.Dispose();
            }

            else if (ofd.ShowDialog()==DialogResult.Cancel)
            {
                ofd.Dispose();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //this.qc = new QCalc(ts.getParameter(), ts.getFreq());
           
            //this.choosedData = qc.chooseDataFromGraph(qc.getS21AndFreq(), rg);
            //QCalc qc = new QCalc(csv.getSparam(), csv.getFreq());
            //qc.setPoints(qc.chooseDataFromGraph(qc.getS21AndFreq(), rg), 1, 2);
            //qc.setPoints(qc.getS21AndFreq(), 1, 2);
            //double[] qValue = new double[12];

            //for (int i = 3; i < 15; i++)
            //{
            //    qValue[i - 3] = qc.get2portQLFromGraph(rg, (double)i);
            //    if (i == 14)
            //    {
            //        LaurenzCurveFit lcf = new LaurenzCurveFit(choosedData);
            //        lcf.setFittedCurveData(401);
            //        rg.addData(lcf.getFittedX(), lcf.getFittedY(), "fittedData");
            //    }
            //}
           
            //this.lcf = new LaurenzCurveFit(choosedData);
            //lcf.setFittedCurveData(801);
            //lcf.setFittedCurveDataig(801);
            
            //rg.addFittedData(lcf.getFittedX(), lcf.getFittedY(), "LG");
            //rg.addData(lcf.getigFittedX(), lcf.getigFittedY(), "inicial guess");
            try
            {

                for (int i = 0; i < 10; i++)
                {
                    if (fittingCount == 0)
                    {
                        lcf.setCoef(lcf.calcInitialCoef());
                    }
                    lcf.fit();
                    fittingCount++;
                }
                lcf.setFittedCurveData(801);
                if (fittingCount != 0)
                {
                    rg.removeData("fitted");
                }
                rg.addFittedData(lcf.getFittedX(), lcf.getFittedY(), "fitted");
                //for (int i = 0; i < 10; i++)
                //{
                //    lcf.fit(this.choosedData);
                //}
                //lcf.setFittedCurveDataSimplex(801);
                //rg.addData(lcf.getsimFittedX(), lcf.getsimFittedY(), "simplex(diff^100)*20");
                ////for (int i = 0; i < 50; i++)
                //{
                //    lcf.fit(this.choosedData);
                //}
                //lcf.setFittedCurveDataSimplex(801);
                //rg.addData(lcf.getsimFittedX(), lcf.getsimFittedY(), "simplex(diff^10)*50");
                //for (int i = 0; i < 100; i++)
                //{
                //    lcf.fit(this.choosedData);
                //}
                //lcf.setFittedCurveDataSimplex(801);
                //rg.addData(lcf.getsimFittedX(), lcf.getsimFittedY(), "simplex(diff^10)*100");
              
                //lcf.fitB(this.choosedData);
                
                //lcf.setFittedCurveData(801);
                //rg.addData(lcf.getFittedX(), lcf.getFittedY(), "L_BFGS_B(diff^2)*1");
                //for (int i = 0; i < 100; i++)
                //{
                //    lcf.fitB(this.choosedData);
                //}
                //lcf.setFittedCurveData(801);
                //rg.addData(lcf.getFittedX(), lcf.getFittedY(), "L_BFGS_B(diff^2)*20");
                //for (int i = 0; i < 50; i++)
                //{
                //    lcf.fitB(this.choosedData);
                //}
                //lcf.setFittedCurveData(801);
                //rg.addData(lcf.getFittedX(), lcf.getFittedY(), "L_BFGS_B(diff^2)*50");
                //for (int i = 0; i < 100; i++)
                //{
                //    lcf.fitB(this.choosedData);
                //}
                //lcf.setFittedCurveData(801);
                //rg.addData(lcf.getFittedX(), lcf.getFittedY(), "L_BFGS_B(diff^2)*100");


                //lcf.fit(this.choosedData);

                //lcf.setFittedCurveData(801);
                //rg.addData(lcf.getFittedX(), lcf.getFittedY(), "L_BFGS_B(diff^10)*1");
                //for (int i = 0; i < 10; i++)
                //{
                //    lcf.fit(this.choosedData);
                //}
                //lcf.setFittedCurveData(801);
                //rg.addData(lcf.getFittedX(), lcf.getFittedY(), "L_BFGS_B(diff^100)*20");
                //for (int i = 0; i < 50; i++)
                //{
                //    lcf.fit(this.choosedData);
                //}
                //lcf.setFittedCurveData(801);
                //rg.addData(lcf.getFittedX(), lcf.getFittedY(), "L_BFGS_B(diff^10)*50");
                //for (int i = 0; i < 100; i++)
                //{
                //    lcf.fit(this.choosedData);
                //}
                //lcf.setFittedCurveData(801);
                //rg.addData(lcf.getFittedX(), lcf.getFittedY(), "L_BFGS_B(diff^10)*100");


            }
            catch (NullReferenceException)
            {
                MessageBox.Show("データを選択して下さい");
            }
            //try
            //{
            //    lcf.fit(this.choosedData);
            //    lcf.setFittedCurveData(801);
            //    rg.addData(lcf.getFittedX(), lcf.getFittedY(), "L_BFGS_B");
            //}
            //catch (NullReferenceException)
            //{
            //    MessageBox.Show("データを選択して下さい");
            //}
            //try
            //{
            //    lcf.fit(this.choosedData);
            //    lcf.setFittedCurveDataig(801);
            //    rg.addData(lcf.getigFittedX(), lcf.getigFittedY(), "T_newton");
            //}
            //catch (NullReferenceException)
            //{
            //    MessageBox.Show("データを選択して下さい");
            //}

        


            //double loadedQ = qc.get2portQLFromGraph(rg);
            //double unloadedQ = qc.get2portQUFromGraph(rg);
            //textBox1.Text = "Q(loaded)=" + loadedQ.ToString() + ",Q(Unloaded)=" + unloadedQ.ToString();

            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            rg.removeData(textBox2.Text);
            rg.Refresh();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            rg.removeMarker(int.Parse(textBox3.Text));
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.qc = new QCalc(ts.getParameter(), ts.getFreq());
            if (radioButton1.Checked)
            {
                this.choosedData = qc.chooseDataFromGraph(qc.getS21AndFreq(), rg);
                this.lcf = new LorentzCurveFit(choosedData);
            }
            if (radioButton2.Checked)
            {
                this.choosedData = qc.chooseDataFromGraph(qc.getS21AndFreq(), rg);
                this.choosedLeft = qc.chooseLeftSide(choosedData);
                this.lcf = new LorentzCurveFit(choosedLeft);
                
            }

            if (radioButton3.Checked)
            {
                this.choosedData = qc.chooseDataFromGraph(qc.getS21AndFreq(), rg);
                this.choosedRight = qc.chooseRightSide(choosedData);
                this.lcf = new LorentzCurveFit(choosedRight);

            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            
        }

    }

}
