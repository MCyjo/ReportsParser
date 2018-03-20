using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HTMLParser2
{
    public delegate void DispLimits(object sender, DataGridViewCellMouseEventArgs e);

    class ReportAnalysis
    {
        TableCreator tablecreator;
        HandleInputs handleInputs;
        Main mainForm;
        public DataTable dataTable;

        private static int columnIndex = 0;
        private List<string> measuermentsFromReport;
        private List<DataRow> rowToAdd;
        public List<Report> reports;

        public ReportAnalysis(HandleInputs handleInputs, Main mainForm, TableCreator tablecreator)
        {
            this.tablecreator = tablecreator;
            this.handleInputs = handleInputs;
            this.mainForm = mainForm;

            this.dataTable = new DataTable("Table");
            this.reports = new List<Report>();
            this.rowToAdd = new List<DataRow>();
        }

        public void analysisFiles()
        {
            createGrid(handleInputs.textFromReport[0]);
            foreach (string report in handleInputs.textFromReport)
            {
                reports.Add(new Report());
                analysis(report);
                reports[reports.Count - 1].setDateTime();

            }
            mainForm.dataGridView1.DataSource = dataTable; //przypisuje tablice do datagrida
            foreach (DataGridViewColumn dc in mainForm.dataGridView1.Columns)
            {
                dc.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            checkIfCellsPassed();
        }

        public void createGrid(string TextFromReport)
        {
            dataTable.Columns.Add("Serial number");
            dataTable.Columns.Add("UUTResult");
            dataTable.Columns.Add("Date");
            dataTable.Columns.Add("Time");
            foreach (string measure in handleInputs.FieldsToRead)
            {
                try
                {
                    int index = TextFromReport.IndexOf(measure);
                    int indexend = TextFromReport.IndexOf("Module Time:", index);
                    string substr = TextFromReport.Substring(index, (indexend - index));
                    if (substr.Contains("Measurement[0]"))
                    {
                        for (int x = 0; ; x++)
                        {
                            if (TextFromReport.Substring(index, (indexend - index)).Contains("Measurement[" + x.ToString() + "]"))
                            {
                                ExtendedColumn extendedColumn = new ExtendedColumn(measure + " Measurement[" + x.ToString() + "]");
                                extendedColumn.findLimitsMulti(index, x, TextFromReport);
                                dataTable.Columns.Add(extendedColumn);

                            }
                            else
                            {
                                break;
                            }
                        }

                    }
                    else
                    {
                        ExtendedColumn extendedColumn = new ExtendedColumn(measure);
                        extendedColumn.findLimits(index, TextFromReport);
                        dataTable.Columns.Add(extendedColumn);

                    }

                }
                catch
                {
                    //tutaj dasz w komórce tabeli wartość null
                }


            }
        }




        public void analysis(string TextFromReport)      //poszukuje kolejnych pomiarow w pliku z raportem
        {
            rowToAdd.Add(dataTable.NewRow());
            columnIndex = 0;

            giveMeSerialNumber(TextFromReport);
            giveMeUUTREsult(TextFromReport);
            giveMeDate(TextFromReport);
            giveMeTime(TextFromReport);
            foreach (string measure in handleInputs.FieldsToRead)
            {
                try
                {
                    int index = TextFromReport.IndexOf(measure);
                    int index2 = TextFromReport.IndexOf("Module Time:", index);
                    string substr = TextFromReport.Substring(index, (index2 - index));
                    if (substr.Contains("Measurement[0]"))
                    {
                        giveMeValues(index, index2, measure, TextFromReport);

                    }
                    else
                    {
                        giveMeValue(index, measure, TextFromReport);

                    }
                }
                catch (Exception e)
                {
                //    MessageBox.Show(e.ToString());
                    //tutaj dasz w komórce tabeli wartość null
                   
                }

            }
            
            dataTable.Rows.Add(rowToAdd[rowToAdd.Count - 1]);

        }


        public void giveMeValue(int index, string measureName, string TextFromReport)  // zwraca wartosc 'zwyklego' pomiaru
        {
            try
            {
                int index2 = TextFromReport.IndexOf("Measurement", index);
                int index3 = TextFromReport.IndexOf("<FONT SIZE=-1>", index2);
                int index4 = TextFromReport.IndexOf("\r\n<TR>", index3);
                int length = index4 - (index3 + 14);


                rowToAdd[rowToAdd.Count - 1][columnIndex] = Double.Parse(TextFromReport.Substring(index3 + 14, length).Replace('.', ','));
            }
            catch
            {
                
            }

            columnIndex++;



        }

        public void giveMeValues(int index, int index2, string measureName, string TextFromReport) //zwraca wartosc pomiarow z kroku gdzie jest kilka pomiarow
        {
            for (int x = 0; ; x++)
            {
                if (TextFromReport.Substring(index, (index2 - index)).Contains("Measurement[" + x.ToString() + "]"))
                {
                    try
                    {
                        int index3 = TextFromReport.IndexOf("Data", index);
                        int index4 = TextFromReport.IndexOf("<FONT SIZE=-1>", index3);
                        int index5 = TextFromReport.IndexOf("\r\n<TR>", index3);
                        index = index5;
                        int length = index5 - (index4 + 14);

                        rowToAdd[rowToAdd.Count - 1][columnIndex] = Double.Parse(TextFromReport.Substring(index4 + 14, length).Replace('.', ','));
                    }
                    catch
                    {
                        
                    }
                    columnIndex++;

                }
                else
                {
                    break;
                }

            }
        }

        public void checkIfCellsPassed()
        {
            for(int j=0; j<dataTable.Rows.Count; j++)
            {
                for(int i =0; i<dataTable.Columns.Count; i++)
                {
                    if(dataTable.Columns[i] is ExtendedColumn)
                    {
                        ExtendedColumn extCol = (ExtendedColumn)dataTable.Columns[i];
                        if(extCol.checkLimits((mainForm.dataGridView1.Rows[j].Cells[i])))
                        {
                            mainForm.dataGridView1.Rows[j].Cells[i].Style.BackColor = Color.Green;
                        }
                        else
                        {
                            mainForm.dataGridView1.Rows[j].Cells[i].Style.BackColor = Color.Red;
                        }
                    }
                }

            }
        }

        public void giveMeDate(string TextFromReport)
        {
            int textLength = "Date: </B><TD><B>".Length;
            int index1 = TextFromReport.IndexOf("Date: </B><TD><B>");
            int index2 = TextFromReport.IndexOf("</B>", index1 + textLength);
            int length = index2 - (index1 + textLength);
            string data = TextFromReport.Substring(index1 + textLength, length);
            rowToAdd[rowToAdd.Count - 1][columnIndex] = data;
            reports[reports.Count - 1].Date = data;

            columnIndex++;
        }

        public void giveMeTime(string TextFromReport)
        {
            int textLength = "Time: </B><TD><B>".Length;
            int index1 = TextFromReport.IndexOf("Time: </B><TD><B>");
            int index2 = TextFromReport.IndexOf("</B>", index1 + textLength);
            int length = index2 - (index1 + textLength);
            string data = TextFromReport.Substring(index1 + textLength, length);
            rowToAdd[rowToAdd.Count - 1][columnIndex] = data;
            reports[reports.Count - 1].Time = data;

            columnIndex++;
        }


        public void giveMeSerialNumber(string TextFromReport) //zwraca numer seryjny
        {
            int textLength = "Serial Number: </B><TD><B>".Length;
            int index1 = TextFromReport.IndexOf("Serial Number: </B><TD><B>");
            int index2 = TextFromReport.IndexOf("</B>", index1+ textLength);
            int length = index2 - (index1 + textLength);
            string data = TextFromReport.Substring(index1 + textLength, length);
            rowToAdd[rowToAdd.Count - 1][columnIndex] = data;
            reports[reports.Count - 1].SerialNumber = data;

            columnIndex++;
        }

        public void giveMeUUTREsult(string TextFromReport) //zwraca numer seryjny
        {
            int textLength = "UUT Result: </B><TD><B><FONT COLOR=#008000>".Length;
            int index1 = TextFromReport.IndexOf("UUT Result: </B><TD><B><FONT COLOR=");
            int index2 = TextFromReport.IndexOf("</FONT>", index1 + textLength);
            int length = index2 - (index1 + textLength);

            rowToAdd[rowToAdd.Count - 1][columnIndex] = TextFromReport.Substring(index1 + textLength, length);

            columnIndex++;
        }

        public void displayLimits(DataGridViewCellMouseEventArgs e)  //wyswietla limity po rightclicku na header kolumny
        {
            if(e.Button==MouseButtons.Right)
            {
                if(dataTable.Columns[e.ColumnIndex] is ExtendedColumn)
                {
                    ExtendedColumn extCol = (ExtendedColumn)dataTable.Columns[e.ColumnIndex];
                    MessageBox.Show(extCol.giveLimits());
                }  
            }
        }

    }
}
