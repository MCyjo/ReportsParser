using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization;
using System.Windows.Forms.DataVisualization.Charting;

namespace HTMLParser2
{
    public partial class ChartForm : Form
    {
       private List<Report> reports;
       private DataGridView dataGridView;
       private DataTable dataTable;
       private List<double> measurements;
       private List<DateTime> dates;
       private List<DateMeasurement> datesMeasures;
       private string columnName;
       private int columnIndex;
       private double? upperLimit;
       private double? lowerLimit;


        public ChartForm(List<Report> reportes, DataGridView datagrid, DataGridViewCellMouseEventArgs de, DataTable datatable)
        {
            this.reports = reportes;
            this.dataGridView = datagrid;
            this.dataTable = datatable;
            this.measurements = new List<double>();
            this.dates = new List<DateTime>();
            this.datesMeasures = new List<DateMeasurement>();
            this.columnIndex = de.ColumnIndex;
            columnName = dataGridView.Columns[columnIndex].HeaderText.ToString();
            columnIndex = de.ColumnIndex;
            InitializeComponent();
            
            for(int i =0; i<(dataGridView.Rows.Count-1); i++)
            {
                if(dataGridView.Rows[i].Cells[columnIndex]!=null)
                {
                    
                        double g=0;
                        DateTime dt=DateTime.Now;
                        try
                        {
                            g = Double.Parse(datagrid.Rows[i].Cells[columnIndex].Value.ToString());
                            dt = reports[i].DateTime;
                            datesMeasures.Add(new DateMeasurement(dt, g));
                            
                        //    dt = reports[i].DateTime;
                         //   dates.Add(dt);
                        }
                        catch (Exception e)
                        {
                         //   MessageBox.Show("1:  " + i + "    " + e.ToString());
                        }
                
                  
                    
                }
            }

            foreach(DateMeasurement dm in datesMeasures)    //tworzy posortowane listy pomiarów i dat
            {
                measurements.Add(dm.measurement);
                dates.Add(dm.date);
            }

            if(dataTable.Columns[columnIndex] is ExtendedColumn)    //pobiera limity dolny i górny
            {
                ExtendedColumn eC = (ExtendedColumn)dataTable.Columns[columnIndex];
                
                upperLimit = eC.Highlimit;
                lowerLimit = eC.Lowlimit;
            }

             CreateChart();
        }

        public void CreateChart()
        { 
            chart1.ChartAreas[0].Name = columnName;
            chart1.ChartAreas[columnName].AxisX.LabelStyle.Format= "MMM/dd/hh:mm";
            chart1.ChartAreas[columnName].AxisX.IntervalType = DateTimeIntervalType.Minutes;
           
            setAxisInterval();
          //  drawLimits();

            chart1.Series[0].Name = columnName;
            chart1.Series[columnName].XValueType = ChartValueType.DateTime;
            chart1.Series[columnName].ChartType = SeriesChartType.Point;
            chart1.Series[columnName].Points.DataBindXY(dates, measurements);

            chart1.Invalidate();
        }


        public void sortDates()
        {
            datesMeasures.Sort((a,b)=>a.date.CompareTo(b.date));
        }



        private void drawLimits()
        {
            //upper limit
            if(upperLimit!=null)
            {
                StripLine stripline = new StripLine();
            //    stripline.Interval = 0.0;
                stripline.Text = "Upper limit";
                stripline.IntervalOffset = (double)upperLimit;
                stripline.StripWidth = 0.5;
                stripline.BackColor = Color.Blue;
                chart1.ChartAreas[0].AxisY.StripLines.Add(stripline);
            }

            if(lowerLimit!=null)
            {
                StripLine stripline = new StripLine();
                stripline.Text = "Lower limit";
              //  stripline.Interval = 0.0;
                stripline.IntervalOffset = (double)lowerLimit;
                stripline.StripWidth = 0.5;
                stripline.BackColor = Color.Blue;
                chart1.ChartAreas[0].AxisY.StripLines.Add(stripline);  
            }
        }

        public void setAxisInterval()
        {
            //ustawia podziałkę osi X i Y
            try
            {
                //interwal to 5% okresu czasu z ktorego są raporty
                TimeSpan period = dates[dates.Count - 1] - dates[0];
                double dPeriodInHours = period.TotalMinutes;
                chart1.ChartAreas[0].AxisX.Interval = 0.05 * dPeriodInHours;

            }
            catch
            {

            }

            try
            {
                double max = measurements.Max();
                double min = measurements.Min();
                chart1.ChartAreas[0].AxisY.Maximum =  max + (0.1 * max);
                chart1.ChartAreas[0].AxisY.Minimum =  min - (0.1 * max);

                double[] maxmin = { chart1.ChartAreas[0].AxisY.Maximum, chart1.ChartAreas[0].AxisY.Minimum };
         
                if(upperLimit!=null)
                {
                    label1.Text = "Upper limit: " + upperLimit;

                    if (((double)upperLimit) > maxmin.Max())
                    {
                        chart1.ChartAreas[0].AxisY.Maximum = (double)upperLimit + (0.1 * max);
                    }
                    else if (((double)upperLimit) < maxmin.Min())
                    {
                        chart1.ChartAreas[0].AxisY.Minimum = (double)upperLimit - (0.1 * max);
                    }
                }
                    
                if(lowerLimit!=null)
                {
                    label2.Text = "Lower limit:  " + lowerLimit;
                    if (((double)lowerLimit) > maxmin.Max())
                    {
                        chart1.ChartAreas[0].AxisY.Maximum = (double)lowerLimit + (0.1 * max);
                    }
                    else if (((double)lowerLimit) < maxmin.Min())
                    {
                        chart1.ChartAreas[0].AxisY.Minimum = (double)lowerLimit - (0.1 * max);
                    }
                }

                double amplitude = chart1.ChartAreas[0].AxisY.Maximum - chart1.ChartAreas[0].AxisY.Minimum;
                chart1.ChartAreas[0].AxisY.Interval = 0.05 * amplitude;
            }
            catch
            {

            }

            /*
            try
            {
                double max = measurements.Max();
                double min = measurements.Min();
                double lL, uL;


                if (upperLimit != null)
                {
                    uL = (double)upperLimit;
                    chart1.ChartAreas[0].AxisY.Maximum = uL + (0.1 * uL);
                    if (max > uL)
                    {
                        chart1.ChartAreas[0].AxisY.Maximum = max + (0.1 * max);
                    }
                    if (uL<min)
                    {
                        chart1.ChartAreas[0].AxisY.Maximum = uL - (0.1 * uL);
                    }
                }
                else
                {
                    chart1.ChartAreas[0].AxisY.Maximum = max + (0.1 * max);
                }
                if (lowerLimit != null)
                {
                    lL = (double)lowerLimit;
                    chart1.ChartAreas[0].AxisY.Minimum = min- (0.1 * max);
                    if (min<lL)
                    {
                        chart1.ChartAreas[0].AxisY.Minimum = min - (0.1 * max);
                    }
                    if(lL>max)
                    {
                        chart1.ChartAreas[0].AxisY.Maximum = lL + (0.1 * lL);
                    }
                }
                else
                {
                    chart1.ChartAreas[0].AxisY.Minimum = min - (0.1 * max);
                }

      

                chart1.ChartAreas[columnName].AxisY.Minimum = measurements.Min() - (0.1 * measurements.Min());
                MessageBox.Show("ul: " + upperLimit + Environment.NewLine +
                                 "lL:  " + lowerLimit + Environment.NewLine +
                                 "max:  " + max + Environment.NewLine+
                                 "min:  " + min + Environment.NewLine);
            }
            catch
            {

            }
            */
        }

        private void chart1_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePoint = new Point(e.X, e.Y);
            chart1.ChartAreas[0].CursorX.SetCursorPixelPosition(mousePoint, true);
            chart1.ChartAreas[0].CursorY.SetCursorPixelPosition(mousePoint, true);

            label3.Text = "Y: " + Math.Round(chart1.ChartAreas[0].AxisY.PixelPositionToValue(e.Y),3).ToString();
            //label4.Text = "Y: " + Math.Round(chart1.ChartAreas[0].AxisY.PixelPositionToValue(e.Y),2).ToString();
            DateTime now = DateTime.FromOADate(chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.X));
            label4.Text = "X: " + now.ToString();

            HitTestResult result = chart1.HitTest(e.X, e.Y);
        }
    }

    public class DateMeasurement
    {
       public  DateTime date;
       public double measurement;

        public DateMeasurement(DateTime date, double  measurements)
        {
            this.date = date;
            this.measurement = measurements;
        }


    }
}
