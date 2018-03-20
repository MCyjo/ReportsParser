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
       private List<double> measurements;
       private List<DateTime> dates;
       private List<DateMeasurement> datesMeasures;
       private string columnName;
       

        public ChartForm(List<Report> reportes, DataGridView datagrid, DataGridViewCellMouseEventArgs de)
        {
            this.reports = reportes;
            this.dataGridView = datagrid;
            this.measurements = new List<double>();
            this.dates = new List<DateTime>();
            this.datesMeasures = new List<DateMeasurement>();
            int columnIndex = de.ColumnIndex;
            columnName = dataGridView.Columns[columnIndex].HeaderText.ToString();
           
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
                            measurements.Add(g);
                            dt = reports[i].DateTime;
                            dates.Add(dt);
                        }
                        catch (Exception e)
                        {
                         //   MessageBox.Show("1:  " + i + "    " + e.ToString());
                        }
                
                  
                    
                }
            }

             CreateChart();
        }

        public void CreateChart()
        {

            /*
            var chartArea = new ChartArea();
        //    chartArea.AxisX.LabelStyle.Format = "YYYY/MMM/dd/hh:mm";
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.LabelStyle.Font = new Font("Consolas",8);
            chartArea.AxisX.LabelStyle.Font = new Font("Consolas", 8);
            chartArea.AxisX.LabelStyle.Format = "MM-DD-YYYY hh:mm";
            
            chart1.ChartAreas.Add(chartArea);

            var series = new Series();
            series.XValueType = ChartValueType.DateTime;
            series.Name = columnName; //do modyfikacji
            series.ChartType = SeriesChartType.FastLine;
            chart1.Series.Add(series);

            chart1.Series[columnName].XValueType = ChartValueType.DateTime;
            chart1.Series[columnName].Points.DataBindXY(dates, measurements);

            chart1.Invalidate();
            */

            chart1.ChartAreas[0].Name = columnName;
            chart1.ChartAreas[columnName].AxisX.LabelStyle.Format= "YYYY/MMM/dd/hh:mm";
            chart1.Series[0].Name = columnName;
            chart1.Series[columnName].XValueType = ChartValueType.DateTime;
            chart1.Series[columnName].ChartType = SeriesChartType.Point;
            chart1.Series[columnName].Points.DataBindXY(dates, measurements);

            chart1.Invalidate();
        }
    }

    public class DateMeasurement
    {
        DateTime date;
        double measurement;

        public DateMeasurement(DateTime date, double  measurements)
        {
            this.date = date;
            this.measurement = measurements;
        }


    }
}
