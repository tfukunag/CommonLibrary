using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using CommonLibrary.FileIO;
using ZedGraph;
using CommonLibrary.Basic;
using System.IO;

namespace CommonLibrary.Output
{
    public class RectanglarGraph:Panel
    {

        private ZedGraph.ZedGraphControl zedGraphControl1;
        private GraphPane gpane;
        private int count = 0;
        private PointPair p0;
        private int markerCount = 1;

        private int[] markerIDGraphItem;
        private int[] markerIDLineItem;
        private Color[] lineColor = { Color.Blue, Color.Red, Color.Green, Color.Orange, Color.Navy, Color.Brown, Color.Pink, Color.Purple, Color.DarkCyan };
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox6;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox11;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBox10;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBox9;
        private System.Windows.Forms.Label label9;

        public RectanglarGraph(int x0,int y0,int width,int height)
        {
            this.InitializeComponent();
            
            zedGraphControl1.IsShowPointValues = true;
            gpane = this.zedGraphControl1.GraphPane;
            gpane.XAxis.MajorGrid.IsVisible = true;
            gpane.YAxis.MajorGrid.IsVisible = true;
            this.SetBounds(x0, y0, width, height);
            gpane.Fill = new Fill(Color.White, Color.FromArgb(225, 210, 235), 45F);
            this.zedGraphControl1.ContextMenuBuilder += new ZedGraphControl.ContextMenuBuilderEventHandler(MyContextMenuBuilder);
            this.zedGraphControl1.PointValueEvent += new ZedGraphControl.PointValueHandler(MyPointValueHandler);
            this.initSize(width, height);
        }

        private void MyContextMenuBuilder(ZedGraphControl control, ContextMenuStrip menuStrip,
                        Point mousePt, ZedGraphControl.ContextMenuObjectState objState)
        {
            ToolStripMenuItem item = new ToolStripMenuItem();
            item.Name = "軸設定";
            item.Tag = "軸設定";
            item.Text = "軸設定";
            item.Click += new System.EventHandler(ChangeAxisSetting);
            menuStrip.Items.Add(item);

            ToolStripMenuItem item2 = new ToolStripMenuItem();
            item2.Name = "タイトル設定";
            item2.Tag = "タイトル設定";
            item2.Text = "タイトル設定";
            item2.Click += new System.EventHandler(titleSetting);
            menuStrip.Items.Add(item2);

            ToolStripMenuItem item3 = new ToolStripMenuItem();
            item3.Name = "マーカー追加";
            item3.Tag = "マーカー追加";
            item3.Text = "マーカー追加";
            item3.Click += new System.EventHandler(addMarker);
            menuStrip.Items.Add(item3);

            ToolStripMenuItem item4 = new ToolStripMenuItem();
            item4.Name = "最後に追加したトレースの削除";
            item4.Tag = "最後に追加したトレースの削除";
            item4.Text = "最後に追加したトレースの削除";
            item4.Click += new System.EventHandler(removeLastData);
            menuStrip.Items.Add(item4);

            ToolStripMenuItem item5 = new ToolStripMenuItem();
            item5.Name = "全てのトレースの削除";
            item5.Tag = "全てのトレースの削除";
            item5.Text = "全てのトレースの削除";
            item5.Click += new System.EventHandler(removeAllData);
            menuStrip.Items.Add(item5);

            ToolStripMenuItem item6 = new ToolStripMenuItem();
            item6.Name = "DATA保存";
            item6.Tag = "DATA保存";
            item6.Text = "DATA保存";
            item6.Click += new System.EventHandler(this.saveData);
            menuStrip.Items.Add(item6);


        }
        private string MyPointValueHandler(ZedGraphControl control, GraphPane pane, CurveItem curve, int iPt)
        {
            // Get the PointPair that is under the mouse
            PointPair pt = curve[iPt];
            this.p0 = pt;
            return " X " + pt.X.ToString("f2") + " , Y " + pt.Y.ToString("f2");
        }

        public void setGraphTitle(string graphTitle)
        {
            gpane.Title.Text = graphTitle;    // X軸ラベル
        }

        public void setXAxisTitle(string xAxisTitle)
        {
            gpane.XAxis.Title.Text = xAxisTitle;    // X軸ラベル
        }

        public void setYAxisTitle(string yAxisTitle)
        {
            gpane.YAxis.Title.Text = yAxisTitle;    // X軸ラベル
        }

        public void setTitleFontSize(int size)
        {
            gpane.Title.FontSpec.Size = size;
            gpane.XAxis.Title.FontSpec.Size = size;
            gpane.YAxis.Title.FontSpec.Size = size;
        }

        private void InitializeComponent()
        {
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // zedGraphControl1
            // 
            this.zedGraphControl1.Location = new System.Drawing.Point(0, 0);
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.PanButtons2 = System.Windows.Forms.MouseButtons.Right;
            this.zedGraphControl1.ScrollGrace = 0D;
            this.zedGraphControl1.ScrollMaxX = 0D;
            this.zedGraphControl1.ScrollMaxY = 0D;
            this.zedGraphControl1.ScrollMaxY2 = 0D;
            this.zedGraphControl1.ScrollMinX = 0D;
            this.zedGraphControl1.ScrollMinY = 0D;
            this.zedGraphControl1.ScrollMinY2 = 0D;
            this.zedGraphControl1.Size = new System.Drawing.Size(150, 138);
            this.zedGraphControl1.TabIndex = 0;
            this.zedGraphControl1.ZoomEvent += new ZedGraph.ZedGraphControl.ZoomEventHandler(this.zedGraphControl1_ZoomEvent);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Location = new System.Drawing.Point(453, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(406, 297);
            this.panel1.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(230, 253);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 10;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(65, 253);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox6);
            this.groupBox2.Controls.Add(this.checkBox4);
            this.groupBox2.Controls.Add(this.checkBox2);
            this.groupBox2.Controls.Add(this.textBox5);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textBox6);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.textBox7);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.textBox8);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Location = new System.Drawing.Point(187, 19);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(144, 228);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Y Axis";
            // 
            // checkBox6
            // 
            this.checkBox6.AutoSize = true;
            this.checkBox6.Location = new System.Drawing.Point(31, 199);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(87, 16);
            this.checkBox6.TabIndex = 11;
            this.checkBox6.Text = "MinorGridOn";
            this.checkBox6.UseVisualStyleBackColor = true;
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(31, 177);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(87, 16);
            this.checkBox4.TabIndex = 10;
            this.checkBox4.Text = "MajorGridOn";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(31, 155);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(76, 16);
            this.checkBox2.TabIndex = 9;
            this.checkBox2.Text = "AutoScale";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // textBox5
            // 
            this.textBox5.Enabled = false;
            this.textBox5.Location = new System.Drawing.Point(66, 120);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(64, 19);
            this.textBox5.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 123);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 12);
            this.label5.TabIndex = 6;
            this.label5.Text = "Minor Div";
            // 
            // textBox6
            // 
            this.textBox6.Enabled = false;
            this.textBox6.Location = new System.Drawing.Point(66, 89);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(64, 19);
            this.textBox6.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 92);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 12);
            this.label6.TabIndex = 4;
            this.label6.Text = "Major Div";
            // 
            // textBox7
            // 
            this.textBox7.Enabled = false;
            this.textBox7.Location = new System.Drawing.Point(66, 59);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(64, 19);
            this.textBox7.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 62);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(23, 12);
            this.label7.TabIndex = 2;
            this.label7.Text = "Min";
            // 
            // textBox8
            // 
            this.textBox8.Enabled = false;
            this.textBox8.Location = new System.Drawing.Point(66, 27);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(64, 19);
            this.textBox8.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 30);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(26, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "Max";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox5);
            this.groupBox1.Controls.Add(this.checkBox3);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.textBox4);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBox3);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(22, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(144, 228);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "X Axis";
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Location = new System.Drawing.Point(32, 199);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(87, 16);
            this.checkBox5.TabIndex = 10;
            this.checkBox5.Text = "MinorGridOn";
            this.checkBox5.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(32, 177);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(87, 16);
            this.checkBox3.TabIndex = 9;
            this.checkBox3.Text = "MajorGridOn";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(32, 155);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(76, 16);
            this.checkBox1.TabIndex = 8;
            this.checkBox1.Text = "AutoScale";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // textBox4
            // 
            this.textBox4.Enabled = false;
            this.textBox4.Location = new System.Drawing.Point(66, 120);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(64, 19);
            this.textBox4.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "Minor Div";
            // 
            // textBox3
            // 
            this.textBox3.Enabled = false;
            this.textBox3.Location = new System.Drawing.Point(66, 89);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(64, 19);
            this.textBox3.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "Major Div";
            // 
            // textBox2
            // 
            this.textBox2.Enabled = false;
            this.textBox2.Location = new System.Drawing.Point(66, 59);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(64, 19);
            this.textBox2.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Min";
            // 
            // textBox1
            // 
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(66, 27);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(64, 19);
            this.textBox1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Max";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button4);
            this.panel2.Controls.Add(this.button3);
            this.panel2.Controls.Add(this.textBox11);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.textBox10);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.textBox9);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Location = new System.Drawing.Point(39, 98);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(339, 189);
            this.panel2.TabIndex = 2;
            this.panel2.Visible = false;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(182, 102);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(78, 27);
            this.button4.TabIndex = 7;
            this.button4.Text = "Cancel";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(73, 102);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(78, 27);
            this.button3.TabIndex = 6;
            this.button3.Text = "Update";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // textBox11
            // 
            this.textBox11.Location = new System.Drawing.Point(132, 77);
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new System.Drawing.Size(163, 19);
            this.textBox11.TabIndex = 5;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(43, 80);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(66, 12);
            this.label11.TabIndex = 4;
            this.label11.Text = "Y Axis Title";
            // 
            // textBox10
            // 
            this.textBox10.Location = new System.Drawing.Point(132, 52);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(163, 19);
            this.textBox10.TabIndex = 3;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(43, 56);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(66, 12);
            this.label10.TabIndex = 2;
            this.label10.Text = "X Axis Title";
            // 
            // textBox9
            // 
            this.textBox9.Location = new System.Drawing.Point(132, 27);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(163, 19);
            this.textBox9.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(43, 30);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(62, 12);
            this.label9.TabIndex = 0;
            this.label9.Text = "Graph Title";
            // 
            // RectanglarGraph
            // 
            this.Controls.Add(this.zedGraphControl1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.panel1.Visible = false;
            this.panel2.Visible = false;
            this.zedGraphControl1.Visible = true;
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Invalidate();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
            {
                gpane.XAxis.Scale.Max = double.Parse(this.textBox1.Text);
                gpane.XAxis.Scale.Min = double.Parse(this.textBox2.Text);
                gpane.XAxis.Scale.MajorStep = double.Parse(this.textBox3.Text);
                gpane.XAxis.Scale.MinorStep = double.Parse(this.textBox4.Text);
            }
            else
            {
                gpane.XAxis.Scale.MajorStepAuto = true;
                gpane.XAxis.Scale.MaxAuto = true;
                gpane.XAxis.Scale.MinAuto = true;
                gpane.XAxis.Scale.MinorStepAuto = true;
                this.zedGraphControl1.Refresh();
            }
            if (!checkBox2.Checked)
            {
                gpane.YAxis.Scale.Max = double.Parse(this.textBox8.Text);
                gpane.YAxis.Scale.Min = double.Parse(this.textBox7.Text);
                gpane.YAxis.Scale.MajorStep = double.Parse(this.textBox6.Text);
                gpane.YAxis.Scale.MinorStep = double.Parse(this.textBox5.Text);
            }
            else
            {
                gpane.YAxis.Scale.Max = 0;
                gpane.YAxis.Scale.MajorStepAuto = true;
                gpane.YAxis.Scale.MinorStepAuto = true;
                gpane.YAxis.Scale.MinAuto = true;
                this.zedGraphControl1.Refresh();

            }
            gpane.XAxis.MajorGrid.IsVisible = this.checkBox3.Checked;
            gpane.YAxis.MajorGrid.IsVisible = this.checkBox4.Checked;
            gpane.XAxis.MinorGrid.IsVisible = this.checkBox5.Checked;
            gpane.YAxis.MinorGrid.IsVisible = this.checkBox6.Checked;
            this.panel1.Visible = false;
            this.panel2.Visible = false;
            this.zedGraphControl1.Visible = true;
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Invalidate();

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.checkBox1.Checked)
            {
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
            }
            else
            {
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                textBox4.Enabled = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.checkBox2.Checked)
            {
                textBox5.Enabled = true;
                textBox6.Enabled = true;
                textBox7.Enabled = true;
                textBox8.Enabled = true;
            }
            else
            {
                textBox5.Enabled = false;
                textBox6.Enabled = false;
                textBox7.Enabled = false;
                textBox8.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            gpane.Title.Text = this.textBox9.Text;    // グラフ タイトル
            gpane.XAxis.Title.Text = this.textBox10.Text;    // X軸ラベル
            gpane.YAxis.Title.Text = this.textBox11.Text;   // Y軸ラベル
            this.panel2.Visible = false;
            this.zedGraphControl1.Visible = true;
            this.panel1.Visible = false;
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Invalidate();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.panel2.Visible = false;
            this.zedGraphControl1.Visible = true;
            this.panel1.Visible = false;
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Invalidate();

        }
        private void ChangeAxisSetting(object sender, EventArgs args)
        {
            // Get a reference to the "Beta" curve IPointListEdit
            this.zedGraphControl1.Visible = false;
            this.textBox1.Text = gpane.XAxis.Scale.Max.ToString();
            this.textBox2.Text = gpane.XAxis.Scale.Min.ToString();
            this.textBox3.Text = gpane.XAxis.Scale.MajorStep.ToString();
            this.textBox4.Text = gpane.XAxis.Scale.MinorStep.ToString();
            this.textBox8.Text = gpane.YAxis.Scale.Max.ToString();
            this.textBox7.Text = gpane.YAxis.Scale.Min.ToString();
            this.textBox6.Text = gpane.YAxis.Scale.MajorStep.ToString();
            this.textBox5.Text = gpane.YAxis.Scale.MinorStep.ToString();
            this.checkBox1.Checked = gpane.XAxis.Scale.MajorStepAuto;
            this.checkBox2.Checked = gpane.YAxis.Scale.MajorStepAuto;
            this.checkBox3.Checked = gpane.XAxis.MajorGrid.IsVisible;
            this.checkBox4.Checked = gpane.YAxis.MajorGrid.IsVisible;
            this.checkBox5.Checked = gpane.XAxis.MinorGrid.IsVisible;
            this.checkBox6.Checked = gpane.YAxis.MinorGrid.IsVisible;
            this.panel1.Visible = true;
            this.panel2.Visible = false;
        }
        private void titleSetting(object sender, EventArgs args)
        {
            this.zedGraphControl1.Visible = false;
            this.panel1.Visible = false;
            this.textBox9.Text = gpane.Title.Text;    // グラフ タイトル
            this.textBox10.Text = gpane.XAxis.Title.Text;    // X軸ラベル
            this.textBox11.Text = gpane.YAxis.Title.Text;   // Y軸ラベル
            this.panel2.Visible = true;

        }
        private void addMarker(object sender, EventArgs args)
        {
            double[] x = new double[1];
            double[] y = new double[1];
            x[0] = p0.X;
            y[0] = p0.Y;
            double dDummy = (gpane.YAxis.Scale.Max - gpane.YAxis.Scale.Min) / 20;
            LineItem marker;
             string dummy = this.markerCount.ToString() + ": X=" + x[0].ToString("F2") + ", Y=" + y[0].ToString("F2");
             marker = this.gpane.AddCurve(dummy, x, y, Color.Black, SymbolType.Triangle);
             marker.Symbol.Fill = new Fill(Color.Black);
             marker.Symbol.Size = 10;
             marker.IsSelectable = true;
             TextObj marker1 = new TextObj(this.markerCount.ToString(), x[0], y[0] - dDummy);
             marker1.FontSpec.FontColor = Color.Black;
             marker1.FontSpec.Fill.IsVisible = false;
             marker1.FontSpec.Border.IsVisible = false;
             this.gpane.GraphObjList.Add(marker1);
             this.markerCount++;
             if (markerCount == 1)
             {
                 markerIDGraphItem = new int[markerCount];
                 markerIDLineItem = new int[markerCount];
             }
             else
             {
                 Array.Resize(ref markerIDLineItem, markerCount);
                 Array.Resize(ref markerIDGraphItem, markerCount);
             }

             markerIDGraphItem[markerCount - 1] = this.gpane.GraphObjList.Count;
             markerIDLineItem[markerCount - 1] = this.gpane.CurveList.Count;
           

           
            
            //this.gpane.GraphObjList.Add(marker1);
           

            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Invalidate();

        }

        public void initSize(int w0,int h0)
        {
//            this.SetBounds(x0, y0, w0, h0);
            this.Width = w0;
            this.Height = h0;
            this.zedGraphControl1.SetBounds(0, 0, w0, h0);
            panel1.SetBounds(0, 0, w0, h0);
            panel2.SetBounds(0, 0, w0, h0);
            int centerX = this.Width / 2;
            int centerY = this.Height / 2;
            this.groupBox1.SetBounds(centerX - 154, centerY - 147, 144, 228);
            this.groupBox2.SetBounds(centerX + 10, centerY - 147, 144, 228);
            this.button1.SetBounds(centerX - 111, centerY + 87, 75, 23);
            this.button2.SetBounds(centerX + 53, centerY + 87, 75, 23);
/*            this.checkBox1.Checked = gpane.XAxis.Scale.MajorStepAuto;
            this.checkBox2.Checked = gpane.YAxis.Scale.MajorStepAuto;
            this.checkBox3.Checked = gpane.XAxis.MajorGrid.IsVisible;
            this.checkBox3.Checked = gpane.YAxis.MajorGrid.IsVisible;
            this.checkBox3.Checked = gpane.XAxis.MinorGrid.IsVisible;
            this.checkBox3.Checked = gpane.YAxis.MinorGrid.IsVisible;*/
            this.button3.SetBounds(centerX - 96, centerY + 21, 78, 27);
            this.button4.SetBounds(centerX + 13, centerY + 21, 78, 27);
            this.label9.SetBounds(centerX - 126, centerY - 51, 66, 12);
            this.label10.SetBounds(centerX - 126, centerY - 25, 66, 12);
            this.label11.SetBounds(centerX - 126, centerY - 1, 66, 12);
            this.textBox9.SetBounds(centerX - 37, centerY - 54, 163, 19);
            this.textBox10.SetBounds(centerX - 37, centerY - 29, 163, 19);
            this.textBox11.SetBounds(centerX - 37, centerY - 4, 163, 19);
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Invalidate();

        }


        public void addData(double[] freq, BasicComplex[, ,] s)
        {

            PointPairList[,] s0 = new PointPairList[s.GetLength(1), s.GetLength(2)];
            for (int i = 0; i < s0.GetLength(0); i++)
            {
                for (int j = 0; j < s0.GetLength(1); j++)
                {
                    s0[i, j] = new PointPairList();
                    for (int k=0;k<s.GetLength(0);k++)
                    {
                        s0[i, j].Add(new PointPair(freq[k]/1E9, s[k, i, j].dbMag()));
                    }
                }
            }
            LineItem[,] data = new LineItem[s.GetLength(1), s.GetLength(2)];
            for (int i = 0; i < s0.GetLength(0); i++)
            {
                for (int j = 0; j < s0.GetLength(1); j++)
                {
                    data[i, j] = gpane.AddCurve("S" + (i + 1).ToString() + (j + 1).ToString() , s0[i,j], this.lineColor[(count % lineColor.Length)], SymbolType.None);
                    count++;
                    data[i,j].Line.Width = 2;
                }
            }
            

            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Invalidate();
        }

        
        public void addData(double[] freq, BasicComplex[,][] s)
        {

            PointPairList[,] s0 = new PointPairList[s.GetLength(0), s.GetLength(1)];
            for (int i = 0; i < s0.GetLength(0); i++)
            {
                for (int j = 0; j < s0.GetLength(1); j++)
                {
                    s0[i, j] = new PointPairList();
                    for (int k = 0; k < s[i,j].GetLength(0); k++)
                    {
                        s0[i, j].Add(new PointPair(freq[k] / 1E9, s[i, j][k].dbMag()));
                    }
                }
            }
            LineItem[,] data = new LineItem[s.GetLength(0), s.GetLength(1)];
            for (int i = 0; i < s0.GetLength(0); i++)
            {
                for (int j = 0; j < s0.GetLength(1); j++)
                {
                    data[i, j] = gpane.AddCurve("S" + (i + 1).ToString() + (j + 1).ToString(), s0[i, j], this.lineColor[(count % lineColor.Length)], SymbolType.None);
                    count++;
                    data[i, j].Line.Width = 2;
                }
            }


            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Invalidate();
        }




        public void addData(double[] freq, BasicComplex[] s, string dataName)
        {
            PointPairList s0 = new PointPairList();
            for (int k = 0; k < s.GetLength(0); k++)
            {
                s0.Add(new PointPair(freq[k]/1E9 , s[k].dbMag()));
            }
            LineItem data = gpane.AddCurve(dataName, s0, this.lineColor[(count % lineColor.Length)], SymbolType.None);
            data.Line.Width = 2;

            count++;
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Invalidate();
        }

        public void addData(double[] freq, double[] s, string dataName)
        {
            PointPairList s0 = new PointPairList();
            for (int k = 0; k < s.GetLength(0); k++)
            {
                s0.Add(new PointPair(freq[k]/1E9 , s[k]));
            }
            LineItem data = gpane.AddCurve(dataName, s0, this.lineColor[(count % lineColor.Length)], SymbolType.None);
            data.Line.Width = 2;
            count++;
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Invalidate();
        }

        public void addFittedData(double[] freq, double[] s, string dataName)
        {
            PointPairList s0 = new PointPairList();
            for (int k = 0; k < s.GetLength(0); k++)
            {
                s0.Add(new PointPair(freq[k] / 1E9, s[k]));
            }
            LineItem data = gpane.AddCurve(dataName, s0, Color.Black, SymbolType.None);
            data.Line.Width = 1;
            data.Line.DashOn = 5;
            data.Line.DashOff = 5;
            data.Line.Style = DashStyle.Dash;
            count++;
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Invalidate();
        }





        public void removeData(string dataName) 
        {
            this.gpane.CurveList.Remove(this.gpane.CurveList[dataName]);
            this.Refresh();
        }

        //最後に追加したトレースの削除
        public void removeLastData(object sender, EventArgs e) 
        {
            if(this.gpane.CurveList.Count > 0)
            {
                this.gpane.CurveList.Remove(this.gpane.CurveList[this.gpane.CurveList.Count - 1]);
                this.count--;
                this.Refresh();
            }
        }



        public void removeMarker(int marker) 
        {
            this.gpane.GraphObjList.Remove(this.gpane.GraphObjList[markerIDGraphItem[marker]-1]);
            this.gpane.CurveList.Remove(this.gpane.CurveList[markerIDLineItem[marker]-1]);  
            this.Refresh();

            
        }


        //全てのトレースの削除
        public void removeAllData()
        {
            this.gpane.CurveList.Clear();
            this.gpane.GraphObjList.Clear();
            this.markerCount = 1;
            this.count = 0;
            this.markerIDGraphItem = null;
            this.markerIDLineItem = null;
            this.Refresh();
        }


        public void removeAllData(object sender, EventArgs e)
        {
            this.gpane.CurveList.Clear();
            this.gpane.GraphObjList.Clear();
            this.markerCount = 1;
            this.count = 0;
            this.markerIDGraphItem = null;
            this.markerIDLineItem = null;
            this.Refresh();
        }


        public double getXMin()
        {
            return gpane.XAxis.Scale.Min;
        }
        public double getXMax()
        {
            return gpane.XAxis.Scale.Max;
        }
        public double getYMin()
        {
            return gpane.YAxis.Scale.Min;
        }
        public double getYMax()
        {
            return gpane.YAxis.Scale.Max;
        }

        public void zoom(double startX, double stopX,double maxY,double minY,double marginY) 
        {
            this.gpane.XAxis.Scale.Min = startX;
            this.gpane.XAxis.Scale.Max = stopX;
            this.gpane.YAxis.Scale.Max = maxY + marginY;
            this.gpane.YAxis.Scale.Min = minY - marginY;
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Invalidate();
        }

        public void autoScale() 
        {
            this.gpane.XAxis.Scale.MaxAuto = true;
            this.gpane.XAxis.Scale.MinAuto = true;

            this.gpane.YAxis.Scale.MaxAuto = true;
            this.gpane.YAxis.Scale.MinAuto = true;

            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Invalidate();
        }

        private void zedGraphControl1_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            //double dummy = this.getYMax() - this.getYMin() / 20;
            //for (int i = 0; i < this.gpane.GraphObjList.Count; i++)
            //{
            //    this.gpane.GraphObjList[i].Location = dummy;
            //}
        }
        public void resize(Graphics g,int x0,int y0,int width,int height)
        {
            this.SetBounds(x0, y0, width, height);
            this.gpane.ReSize(g,this.ClientRectangle);
            this.zedGraphControl1.SetBounds(0, 0, width, height);
            
        }
        public void saveData(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            PointPair[][] pp;
            sfd.Filter ="CSVファイル(*.csv)|*.csv";
            sfd.Title = "保存先のファイルを選択してください";
            sfd.RestoreDirectory = true;
            sfd.OverwritePrompt = true;
            sfd.CheckPathExists = true;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                System.IO.Stream stream;
                stream = sfd.OpenFile();
                if (stream != null)
                {
                    StreamWriter sw = new StreamWriter(stream);
                    CurveItem[] ci=gpane.CurveList.ToArray();
                    string[] dataName = new string[ci.Length];
                    pp=new PointPair[ci.Length][];
                    for (int i = 0; i < ci.Length; i++)
                    {
                        PointPairList ppl = new PointPairList(ci[i].Points);
                        pp[i]=ppl.ToArray();
                        dataName[i] = ci[i].Label.Text;
                    }
                    string index = "";
                    for (int i = 0; i < dataName.Length; i++)
                    {
                        if (i == 0) index = index + dataName[i] + "_X" + "," + dataName[i] + "_Y";
                        else index = index + "," + dataName[i] + "_X" + "," + dataName[i] + "_Y";
                    }
                    sw.WriteLine(index);
                    int iDummy = pp[0].Length;
                    if (pp.Length > 0)
                    {
                        for (int i = 1; i < pp.Length; i++)
                        {
                            if (pp[i].Length > iDummy) iDummy = pp[i].Length;
                        }
                    }

                    for (int j = 0; j < iDummy; j++)
                    {
                        string dummy = "";
                        for (int k = 0; k < pp.Length; k++)
                        {
                            if (k == 0)
                            {
                                if (j >= pp[k].Length)
                                {
                                    dummy = ",";
                                }
                                else
                                {
                                    dummy = pp[k][j].X.ToString() + "," + pp[k][j].Y.ToString();
                                }
                            }
                            else
                            {
                                if (j >= pp[k].Length)
                                {
                                    dummy = dummy + ", , ";
                                }
                                else
                                {
                                    dummy = dummy + "," + pp[k][j].X.ToString() + "," + pp[k][j].Y.ToString();
                                }
                            }
                        }
                        sw.WriteLine(dummy);
                    }
                    sw.Flush();
                    sw.Close();
                    stream.Close();
                }
            }
        }
    }
}
