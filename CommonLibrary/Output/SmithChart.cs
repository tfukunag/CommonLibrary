using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommonLibrary.FileIO;
using CommonLibrary.Basic;
using System.Drawing.Drawing2D;


namespace CommonLibrary.Output
{
    public class SmithChart : Panel
    {
        private double[][] freq;
        private BasicComplex[][] s11;
        private int numOfData = 0;
        private int drawRange;
        private double factor = .9;
        public int[] numOfMarker;
        public int[][] marker;
        private int markerCount = 0;
        private int fSize;
        private bool showAdmittance = false;
        private bool showImpedance = true;
        public SmithChart(int x0, int y0, int width, int height)
        {
            this.SetBounds(x0, y0, width, height);

        }
        private void drawGraphFrame(Graphics g)
        {
            Pen frame = new Pen(Color.Black, 2);
            Pen inner = new Pen(Color.Gray, 1);
            Pen inner2 = new Pen(Color.Gray, 1);

            inner.DashStyle = DashStyle.Dash;
            inner2.DashStyle = DashStyle.Dash;

            Font font0 = new Font("Times New Roman", fSize, FontStyle.Bold);

            double[] x0 = new double[] { 0.25, .5, 1, 2, 4 };
            for (int i = 0; i < x0.Length; i++)
            {

                double angle = Math.Atan2(2 / (x0[i] * x0[i] + 1), -(x0[i] * x0[i] - 1) / x0[i] / (x0[i] * x0[i] + 1));
                angle = angle / Math.PI * 180;
                if (this.showAdmittance)
                {
                    g.DrawArc(inner2, this.pointX(-1 - 1 / x0[i]), this.pointY(2 / x0[i]), this.length(2 / x0[i]), this.length(2 / x0[i]), 90, -(int)angle);
                    g.DrawArc(inner2, this.pointX(-1 - 1 / x0[i]), this.pointY(0), this.length(2 / x0[i]), this.length(2 / x0[i]), -90, (int)angle);

                }
                if (this.showImpedance)
                {
                    /*                    double xa = (1 - x0[i])*(1 - x0[i]) / (1 + x0[i] * x0[i]);
                                        double ya = 2*x0[i] / (1 + x0[i] * x0[i]);
                                        g.DrawString(x0[i].ToString(), font0, Brushes.Gray, this.point(xa, ya));
                                        g.DrawString((-x0[i]).ToString(), font0, Brushes.Gray, this.point(xa, -ya));*/
                    g.DrawArc(inner, this.pointX(1 - 1 / x0[i]), this.pointY(2 / x0[i]), this.length(2 / x0[i]), this.length(2 / x0[i]), 90, (int)angle);
                    g.DrawArc(inner, this.pointX(1 - 1 / x0[i]), this.pointY(0), this.length(2 / x0[i]), this.length(2 / x0[i]), -90, -(int)angle);
                }
            }

            double[] r0 = new double[] { 0.25, .5, 1, 2, 4 };
            for (int i = 0; i < r0.Length; i++)
            {
                g.DrawLine(frame, this.point(-1, 0), this.point(1, 0));
                g.DrawEllipse(frame, this.pointX(-1), this.pointY(1), this.length(2), this.length(2));
                if (this.showAdmittance) g.DrawEllipse(inner2, this.pointX(-1), this.pointY(1 / (r0[i] + 1)), this.length(2 / (r0[i] + 1)), this.length(2 / (r0[i] + 1)));
                if (this.showImpedance) g.DrawEllipse(inner, this.pointX((r0[i] - 1) / (r0[i] + 1)), this.pointY(1 / (r0[i] + 1)), this.length(2 / (r0[i] + 1)), this.length(2 / (r0[i] + 1)));
            }


        }
        public void addData(double[] freq, BasicComplex[] s11)
        {
            numOfData++;
            if (numOfData == 1)
            {
                this.freq = new double[numOfData][];
                this.s11 = new BasicComplex[numOfData][];
                this.numOfMarker = new int[numOfData];
                this.marker = new int[numOfData][];
            }
            else
            {
                Array.Resize(ref this.freq, numOfData);
                Array.Resize(ref this.s11, numOfData);
                Array.Resize(ref this.numOfMarker, numOfData);
                Array.Resize(ref this.marker, numOfData);
            }
            this.freq[numOfData - 1] = freq;
            this.s11[numOfData - 1] = s11;
            this.numOfMarker[numOfData - 1] = 0;
        }

        public void removeData()
        {
            this.freq[numOfData - 1] = null;
            this.s11[numOfData - 1] = null;
            this.numOfMarker[numOfData - 1] = 0;
            Array.Resize(ref this.freq, numOfData - 1);
            Array.Resize(ref this.s11, numOfData - 1);
            Array.Resize(ref this.numOfMarker, numOfData - 1);
            Array.Resize(ref this.marker, numOfData - 1);
            numOfData--;
        }

        private int pointX(double x0)
        {
            int centerX = this.ClientSize.Width / 2;
            if (this.ClientSize.Width > this.ClientSize.Height) drawRange = (int)(ClientSize.Height * factor / 2);
            else drawRange = (int)(ClientSize.Width * factor / 2);
            return centerX + (int)(drawRange * x0);
        }
        private int pointY(double y0)
        {
            int centerY = this.ClientSize.Height / 2;
            if (this.ClientSize.Width > this.ClientSize.Height) drawRange = (int)(ClientSize.Height * factor / 2);
            else drawRange = (int)(ClientSize.Width * factor / 2);
            return centerY - (int)(drawRange * y0);
        }
        private int length(double l0)
        {
            if (this.ClientSize.Width > this.ClientSize.Height) drawRange = (int)(ClientSize.Height * factor / 2);
            else drawRange = (int)(ClientSize.Width * factor / 2);
            return (int)(l0 * drawRange);
        }

        private Point point(double x0, double y0)
        {
            int centerX = this.ClientSize.Width / 2;
            int centerY = this.ClientSize.Height / 2;
            if (this.ClientSize.Width > this.ClientSize.Height) drawRange = (int)(ClientSize.Height * factor / 2);
            else drawRange = (int)(ClientSize.Width * factor / 2);
            return new Point(centerX + (int)(x0 * drawRange), centerY - (int)(y0 * drawRange));
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }


        private void plotData(Graphics g)
        {
            Color[] penColor = new Color[] { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Pink, Color.Orange, Color.Navy, Color.LimeGreen };
            Pen[] line = new Pen[penColor.Length];
            for (int i = 0; i < penColor.Length; i++)
            {
                line[i] = new Pen(penColor[i], 1);
            }
            Point[][] d0;

            if (numOfData != 0)
            {
                int dummy = this.s11[0].Length;
                d0 = new Point[numOfData][];
                for (int i = 0; i < numOfData; i++)
                {
                    d0[i] = new Point[dummy];
                    for (int j = 0; j < dummy; j++)
                    {
                        d0[i][j] = this.point(s11[i][j].getReal(), s11[i][j].getImage());
                    }
                    g.DrawLines(line[i % line.Length], d0[i]);
                }
            }

        }

        private void plotMarker(Graphics g)
        {
            int tSize = (int)((double)this.ClientSize.Height / 600 * 20);
            Brush[] penColor = new Brush[] { Brushes.Red, Brushes.Blue, Brushes.Green, Brushes.Yellow, Brushes.Pink, Brushes.Orange, Brushes.Navy, Brushes.LimeGreen };
            for (int i = 0; i < this.numOfData; i++)
            {
                for (int j = 0; j < this.numOfMarker[i]; j++)
                {
                    markerCount++;
                    this.drawTriangle(g, this.point(s11[i][marker[i][j]].getReal(), s11[i][marker[i][j]].getImage()), tSize, penColor[i % penColor.Length], markerCount);
                    this.makeLegend(s11[i][marker[i][j]], freq[i][marker[i][j]] / 1E9, g, penColor[i % penColor.Length], markerCount);
                }
            }
            markerCount = 0;
        }

        public void addMaker(int dataIndex, double freq)
        {
            if (dataIndex > numOfData - 1) throw new IndexOutOfRangeException("配列の範囲をオーバしています。");
            this.numOfMarker[dataIndex]++;
            if (numOfMarker[dataIndex] == 1)
            {
                marker[dataIndex] = new int[numOfMarker[dataIndex]];
            }
            else
            {
                Array.Resize(ref marker[dataIndex], numOfMarker[dataIndex]);
            }
            marker[dataIndex][numOfMarker[dataIndex] - 1] = this.freqIndex(dataIndex, freq);
        }

        private int freqIndex(int dataIndex, double f0)
        {
            int dummy = 0;
            while ((freq[dataIndex][dummy] < f0)) dummy++;
            if ((f0 - freq[dataIndex][dummy - 1]) > (freq[dataIndex][dummy] - f0))
            {
                return dummy;
            }
            else return (dummy - 1);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            fSize = (int)((double)this.ClientSize.Height / 600 * 20);
            Graphics g = e.Graphics;
            this.drawGraphFrame(g);
            this.plotData(g);
            this.plotMarker(g);
        }
        private BasicComplex calcImpedance(BasicComplex s11)
        {
            return (1 + s11) / (1 - s11) * 50;
        }
        private void drawTriangle(Graphics g, Point p0, int size, Brush bs, int m0)
        {
            Point p1 = new Point(p0.X - (int)(size / 2), p0.Y + (int)(size * Math.Sqrt(3) / 2));
            Point p2 = new Point(p0.X + (int)(size / 2), p0.Y + (int)(size * Math.Sqrt(3) / 2));
            Point[] plot = new Point[] { p0, p1, p2 };
            g.FillPolygon(bs, plot, FillMode.Alternate);
            Pen frame = new Pen(Color.Black, 1);
            g.DrawPolygon(frame, plot);
            string dummy = m0.ToString();
            Font font0 = new Font("Times New Roman", fSize, FontStyle.Bold);
            SizeF fsize = g.MeasureString(dummy, font0);
            g.DrawString(dummy, font0, bs, p0.X - fsize.Width / 2, p1.Y);
        }
        private void makeLegend(BasicComplex s11, double freq, Graphics g, Brush bs, int markerCount)
        {
            BasicComplex zin = this.calcImpedance(s11);
            string dummy = markerCount.ToString() + ": f=" + freq.ToString("F2") + " R=" + zin.getReal().ToString("F2") + " X=" + zin.getImage().ToString("F2");
            Font font0 = new Font("Times New Roman", fSize, FontStyle.Bold);
            SizeF fsize = g.MeasureString(dummy, font0);
            g.DrawString(dummy, font0, bs, this.ClientSize.Width - fsize.Width, this.ClientSize.Height - fsize.Height * markerCount);
        }
        public void showAdmittanceChart(bool showAdmittance)
        {
            this.showAdmittance = showAdmittance;
        }
        public void showImpedanceChart(bool showImpedance)
        {
            this.showImpedance = showImpedance;
        }
    }
}
