using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace HTMLParser2
{
   

    public partial class Main : Form
    {
        HandleInputs handleInputs;
        ReportAnalysis reportAnalysis;
     
   

        public Main()
        {
            InitializeComponent();
            handleInputs = new HandleInputs(this);
           
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            handleInputs.ReadUserList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            handleInputs.LoadReport();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedItem!=null)
            {
                label1.Text = "Loading...";
                reportAnalysis = new ReportAnalysis(handleInputs, this);
                backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("Firstly select number of units in panel.");
            } 
        }

//Wylaczona funkcja rysowania wykresów
/*
        private void dataGridView1_ColumnHeaderMouseClick_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            ChartForm chartForm = new ChartForm(reportAnalysis.reports, dataGridView1, e, reportAnalysis.dataTable);
            chartForm.Show();
        }
*/

        private void btnSaveToCSV_Click(object sender, EventArgs e)
        {
            reportAnalysis.saveToCSV(dataGridView1);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            reportAnalysis.analysisFiles();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnSaveToCSV.Enabled = true;
            label1.Text = "Done";
        }

    }
}
