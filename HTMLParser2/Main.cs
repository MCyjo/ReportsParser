using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HTMLParser2
{
    public partial class Main : Form
    {
        HandleInputs handleInputs;
        ReportAnalysis reportAnalysis;
        TableCreator tableCreator;

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
            tableCreator = new TableCreator(this, handleInputs);
            reportAnalysis = new ReportAnalysis(handleInputs, this, tableCreator);
            reportAnalysis.analysisFiles();
        }



        private void dataGridView1_ColumnHeaderMouseClick_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            ChartForm chartForm = new ChartForm(reportAnalysis.reports, dataGridView1, e);
            chartForm.Show();
        }
    }
}
