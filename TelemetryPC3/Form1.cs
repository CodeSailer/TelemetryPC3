using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OxyPlot;
using OxyPlot.Series;

namespace TelemetryPC3
{
    public partial class Form1 : Form
    {
        PlotModel myModel = new PlotModel { };

        LineSeries speedSeries = new LineSeries() { Title = "speed"};
        LineSeries tFLSeries = new LineSeries() { Title = "t° FL" };
        LineSeries tFRSeries = new LineSeries() { Title = "t° FR" };
        LineSeries tRLSeries = new LineSeries() { Title = "t° RL" };
        LineSeries tRRSeries = new LineSeries() { Title = "t° RR" };
        LineSeries throttleSeries = new LineSeries() { Title = "throttle" };
        LineSeries brakeSeries = new LineSeries() { Title = "brake" };
        LineSeries steeringSeries = new LineSeries() { Title = "steering" };
        string telemetryFileName;

        List<float> gforce_x = new List<float>();
        List<float> gforce_y = new List<float>();
        List<float> gforce_z = new List<float>();

        public Form1( string telemetryFile = null)
        {
            if(telemetryFile != null)
            {
                telemetryFileName = telemetryFile;
            }
            InitializeComponent();
            
            speedSeries.Color = OxyColors.Blue;
            tFLSeries.Color = OxyColors.Orange;
            tFRSeries.Color = OxyColors.DarkOrange;
            tRLSeries.Color = OxyColors.Purple;
            tRRSeries.Color = OxyColors.DarkViolet;
            throttleSeries.Color = OxyColors.Green;
            brakeSeries.Color = OxyColors.Red;
            steeringSeries.Color = OxyColors.Black;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                ClearPlot();
                telemetryFileName = ofd.FileName;
                openTelemetryFile(telemetryFileName, LapBox.Text);
            }
        }

        public void openTelemetryFile(string fileName, string lapNumber)
        {
            List<string> tmp = File.ReadAllLines(fileName).ToList();
            for (int i = 0; i < tmp.Count; i++)
            {
                string[] line_part = tmp[i].Split(' ');
                if (lapNumber != "all")
                {
                    if (line_part[0] == lapNumber)
                    {
                        speedSeries.Points.Add(new DataPoint(i, double.Parse(line_part[1]) * 3.6));
                        s1time.Text = line_part[2];
                        s2time.Text = line_part[3];
                        s3time.Text = line_part[4];
                        laptime.Text = line_part[5];
                        tFLSeries.Points.Add(new DataPoint(i, double.Parse(line_part[6])+(-273.15)));
                        tFRSeries.Points.Add(new DataPoint(i, double.Parse(line_part[7]) + (-273.15)));
                        tRLSeries.Points.Add(new DataPoint(i, double.Parse(line_part[8]) + (-273.15)));
                        tRRSeries.Points.Add(new DataPoint(i, double.Parse(line_part[9]) + (-273.15)));

                        throttleSeries.Points.Add(new DataPoint(i, double.Parse(line_part[10])/255*100));
                        brakeSeries.Points.Add(new DataPoint(i, double.Parse(line_part[11])/255*100));
                        steeringSeries.Points.Add(new DataPoint(i, double.Parse(line_part[12])));
                        gforce_x.Add(float.Parse(line_part[13]) / 10);
                        gforce_y.Add(float.Parse(line_part[14]) / 10);
                        gforce_z.Add(float.Parse(line_part[15]) / 10);
                    }
                }
                else
                {
                    speedSeries.Points.Add(new DataPoint(i, double.Parse(line_part[1]) * 3.6));
                    s1time.Text = line_part[2];
                    s2time.Text = line_part[3];
                    s3time.Text = line_part[4];
                    laptime.Text = line_part[5];
                    tFLSeries.Points.Add(new DataPoint(i, double.Parse(line_part[6]) + (-273.15)));
                    tFRSeries.Points.Add(new DataPoint(i, double.Parse(line_part[7]) + (-273.15)));
                    tRLSeries.Points.Add(new DataPoint(i, double.Parse(line_part[8]) + (-273.15)));
                    tRRSeries.Points.Add(new DataPoint(i, double.Parse(line_part[9]) + (-273.15)));

                    throttleSeries.Points.Add(new DataPoint(i, double.Parse(line_part[10]) / 255 * 100));
                    brakeSeries.Points.Add(new DataPoint(i, double.Parse(line_part[11])/255*100));
                    steeringSeries.Points.Add(new DataPoint(i, double.Parse(line_part[12])));
                    gforce_x.Add(float.Parse(line_part[13]) / 10);
                    gforce_y.Add(float.Parse(line_part[14]) / 10);
                    gforce_z.Add(float.Parse(line_part[15]) / 10);
                }

            }
            myModel.Series.Add(speedSeries);
            myModel.Series.Add(tFLSeries);
            myModel.Series.Add(tFRSeries);
            myModel.Series.Add(tRLSeries);
            myModel.Series.Add(tRRSeries);
            myModel.Series.Add(throttleSeries);
            myModel.Series.Add(brakeSeries);
            myModel.Series.Add(steeringSeries);
            if (gforce_x.Count() != 0)
            {
                gforce_x_min.Text = gforce_x.Min().ToString("0.0");
                gforce_x_max.Text = gforce_x.Max().ToString("0.0");
                gforce_y_min.Text = gforce_y.Min().ToString("0.0");
                gforce_y_max.Text = gforce_y.Max().ToString("0.0");
                gforce_z_min.Text = gforce_z.Min().ToString("0.0");
                gforce_z_max.Text = gforce_z.Max().ToString("0.0");
            }
            this.plotView1.Model = myModel;
        }

        private void btn_update_Click(object sender, EventArgs e)
        {
            /*Form1 nf = new Form1(telemetryFileName);
            nf.Show();
            nf.LapBox.Text = LapBox.Text;
            nf.openTelemetryFile(telemetryFileName, LapBox.Text);*/
            ClearPlot();
            openTelemetryFile(telemetryFileName, LapBox.Text);
        }

        public void ClearPlot()
        {
            // reset plot
            myModel.InvalidatePlot(true);
            myModel.Series.Clear();
            //reset series data
            speedSeries.Points.Clear();
            tFLSeries.Points.Clear();
            tFRSeries.Points.Clear();
            tRLSeries.Points.Clear();
            tRRSeries.Points.Clear();
            throttleSeries.Points.Clear();
            brakeSeries.Points.Clear();
            steeringSeries.Points.Clear();
        }

        private void newWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 nf = new Form1(telemetryFileName);
            nf.Show();
        }
    }
}
