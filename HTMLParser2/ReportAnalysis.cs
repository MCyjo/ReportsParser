using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace HTMLParser2
{
    public delegate void DispLimits(object sender, DataGridViewCellMouseEventArgs e);


    class ReportAnalysis
    {

        private HandleInputs handleInputs;
        private Main mainForm;
        public DataTable dataTable;

        private static int columnIndex = 0;
        private List<DataRow> rowToAdd;
        public List<Report> reports;



        public ReportAnalysis(HandleInputs handleInputs, Main mainForm)
        {
            this.handleInputs = handleInputs;
            this.mainForm = mainForm;

            this.dataTable = new DataTable("Table");
            this.reports = new List<Report>();
            this.rowToAdd = new List<DataRow>();
        }

      

        public  void analysisFiles()
        {
            try
            {
                mainForm.dataGridView1.Invoke(new Action(delegate ()
                {
                    string textFromFirstReport = System.IO.File.ReadAllText(handleInputs.fileNames[0]);
                    createGrid(textFromFirstReport);

                    foreach (string filePath in handleInputs.fileNames)
                    {
                        string report;

                        try
                        {
                            report = (System.IO.File.ReadAllText(filePath));
                            
                            reports.Add(new Report());
                            analysisMultipleReport(report);
                            reports[reports.Count - 1].setDateTime();
                        }
                        catch (ArgumentOutOfRangeException ex)
                        {
                            // MessageBox.Show(filePath + Environment.NewLine + "Error: Could not read file from disk. Original error: " + ex.Message);
                        }
                    }
                    mainForm.dataGridView1.DataSource = dataTable; //przypisuje tablice do datagrida

                    foreach (DataGridViewColumn dc in mainForm.dataGridView1.Columns)
                    {
                        dc.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }

                    checkIfCellsPassed();   //set green or red cell background color in order to teststep result
                }
           ));
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
           
        }
            
            
        

        public void createGrid(string TextFromReport)
        {
            TextFromReport = TextFromReport.Substring(TextFromReport.IndexOf("Begin Sequence:"));

            dataTable.Columns.Add("Serial number");
            dataTable.Columns.Add("UUTResult");
            dataTable.Columns.Add("Date");
            dataTable.Columns.Add("Time");
            dataTable.Columns.Add("Date Time");
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
                                extendedColumn.DefineMultipleLimitsType(TextFromReport, measure, x);
                                extendedColumn.ColumnName += extendedColumn.findLimitsMulti(index, x, TextFromReport);
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
                        extendedColumn.DefineLimitsType(TextFromReport);
                        extendedColumn.ColumnName += extendedColumn.findLimits(index, TextFromReport);
                        dataTable.Columns.Add(extendedColumn);
                    }

                }
                catch
                {
                    //tutaj dasz w komórce tabeli wartość null
                } 
            }
        }


        public void analysisMultipleReport(string textFromReport) //dla plików wynikowych gdzie zpaisane są testy kilku wyrobów
        {
            if (mainForm.comboBox1.SelectedItem.ToString() == "1")  //jeden wyró w raporcie
            {
                analysis(textFromReport);
            }
            else if (mainForm.comboBox1.SelectedItem.ToString() == "2") //dwa wyroby w raporcie
            {

                int index1 = textFromReport.IndexOf("UUT Report");
                int index2 = textFromReport.IndexOf("UUT Report", index1 + 10);
                int length = index2 - index1;

                string textFromReport1 = textFromReport.Substring(index1, length);
                string textFromReport2 = textFromReport.Substring(index2);

                analysis(textFromReport1);
                analysis(textFromReport2);
            }
            else if (mainForm.comboBox1.SelectedItem.ToString() == "4") //cztery wyroby w raporcie
            {
                int index1 = textFromReport.IndexOf("UUT Report");
                int index2 = textFromReport.IndexOf("UUT Report", index1 + 10);
                int length1 = index2 - index1;

                int index3 = textFromReport.IndexOf("UUT Report", index2+10);
                int index4 = textFromReport.IndexOf("UUT Report", index3 + 10);
                int length2 = index4 - index3;

                int index5 = textFromReport.IndexOf("UUT Report", index4+10);
                int index6 = textFromReport.IndexOf("UUT Report", index5 + 10);
                int length3 = index6 - index5;

                int index7 = textFromReport.IndexOf("UUT Report", index6+10);
                int index8 = textFromReport.IndexOf("UUT Report", index7 + 10);
                int length4 = index8 - index7;

                string textFromReport1 = textFromReport.Substring(index1, length1);
                string textFromReport2 = textFromReport.Substring(index3, length2);
                string textFromReport3 = textFromReport.Substring(index5, length3);
                string textFromReport4 = textFromReport.Substring(index7, length4);

                analysis(textFromReport1);
                analysis(textFromReport2);
                analysis(textFromReport3);
                analysis(textFromReport4);
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
            giveMeDateTime();
            foreach (string measure in handleInputs.FieldsToRead)
            {
                try
                {   int indexBeginSequence = TextFromReport.IndexOf("Begin Sequence:"); //(usuwa bug z pobieraniem wartosci z nagłówka, gdzie wypisane są kroki na których był fail)
                    int index = TextFromReport.IndexOf(measure, indexBeginSequence);
                    int index2 = TextFromReport.IndexOf("Module Time:", index);
                    string substr = TextFromReport.Substring(index, (index2 - index));
                    if (substr.Contains("Measurement[0]"))
                    {
                        giveMeValues(index, index2, TextFromReport);

                    }
                    else
                    {
                        giveMeValue(index, TextFromReport);

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


        public void giveMeValue(int index, string TextFromReport)  // zwraca wartosc 'zwyklego' pomiaru
        {
            try
            {
                int index2 = TextFromReport.IndexOf("Measurement:", index);
                int index3 = TextFromReport.IndexOf("<FONT SIZE=-1>", index2);
                int index4 = TextFromReport.IndexOf("\r\n<TR>", index3);
                int length = index4 - (index3 + 14);

                rowToAdd[rowToAdd.Count - 1][columnIndex] = Double.Parse(TextFromReport.Substring(index3 + 14, length).Replace('.', ','));
            }
            catch (Exception e)
            {
                MessageBox.Show("Błąd w metodzie giveMeValue" );
            }

            columnIndex++;
        }



        public void giveMeValues(int index, int index2, string TextFromReport) //zwraca wartosci z kroku gdzie jest kilka pomiarow
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

                        Double result = Double.Parse(TextFromReport.Substring(index4 + 14, length).Replace('.', ','));

                        rowToAdd[rowToAdd.Count - 1][columnIndex] = result;
                        
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Błąd w metodzie giveMeValues"  );
                    }
                    columnIndex++;

                }
                else
                {
                    break;
                }

            }
        }

     

       public void checkIfCellsPassed()     //przelatuje całą tabelę i porównuje z poranymi limitami
        {
            for (int j = 0; j < dataTable.Rows.Count; j++)
            {
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    if (dataTable.Columns[i] is ExtendedColumn)
                    {
                        ExtendedColumn extCol = (ExtendedColumn)dataTable.Columns[i];
                        if (extCol._checkLimitDelegate((mainForm.dataGridView1.Rows[j].Cells[i]))) 
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
        

        public static string changeDateFormatDot(string date)   //zmienia format daty z yyyy.MM.dd
        {
            return date.Replace('.', '-');
        }

        public static string changeDateFormatEng(string date)   // zmienia format daty z nazwą miesiąca słownie po angielsku
        {
            string[] splittedDate = date.Split(',');
            string year = splittedDate[2].Substring(1);
            string month;
            string day;

            string[] splittedSubstring1 = splittedDate[1].Split(' ');
            day = splittedSubstring1[2];

            string monthWord = splittedSubstring1[1];

            if (monthWord.Contains("Jan"))
            {
                month = "01";
            }
            else if (monthWord.Contains("Feb"))
            {
                month = "02";
            }
            else if (monthWord.Contains("Mar"))
            {
                month = "03";
            }
            else if (monthWord.Contains("Apr"))
            {
                month = "04";
            }
            else if (monthWord.Contains("May"))
            {
                month = "05";
            }
            else if (monthWord.Contains("Jun"))
            {
                month = "06";
            }
            else if (monthWord.Contains("Jul"))
            {
                month = "07";
            }
            else if (monthWord.Contains("Aug"))
            {
                month = "08";
            }
            else if (monthWord.Contains("Sep"))
            {
                month = "09";
            }
            else if (monthWord.Contains("Oct"))
            {
                month = "10";
            }
            else if (monthWord.Contains("Nov"))
            {
                month = "11";
            }
            else if (monthWord.Contains("Dec"))
            {
                month = "12";
            }
            else
            {
                month = "sorry :(";
            }

            return year + "-" + month + "-" + day;
        }

        public static string changeDateFormatPol(string date)   // zmienia format daty z polską nazwą miesiąca
        {
            string[] splittedDate = date.Split(' ');
            string year = splittedDate[2];
            string month;
            string day;

            if (int.Parse(splittedDate[0])>=10)
            {
                day = splittedDate[0];
            }
            else
            {
                day = "0" + splittedDate[0];
            }

            if(splittedDate[1].Contains("sty"))
            {
                month = "01";
            }
            else if(splittedDate[1].Contains("lut"))
            {
                month = "02";
            }
            else if (splittedDate[1].Contains("mar"))
            {
                month = "03";
            }
            else if (splittedDate[1].Contains("kwi"))
            {
                month = "04";
            }
            else if (splittedDate[1].Contains("maj"))
            {
                month = "05";
            }
            else if (splittedDate[1].Contains("cze"))
            {
                month = "06";
            }
            else if (splittedDate[1].Contains("lip"))
            {
                month = "07";
            }
            else if (splittedDate[1].Contains("sie"))
            {
                month = "08";
            }
            else if (splittedDate[1].Contains("wrz"))
            {
                month = "09";
            }
            else if (splittedDate[1].Contains("pa"))
            {
                month = "10";
            }
            else if (splittedDate[1].Contains("lis"))
            {
                month = "11";
            }
            else if (splittedDate[1].Contains("gru"))
            {
                month = "12";
            }
            else
            {
                month = "sorry :(";
            }

            return year + "-" + month + "-" + day;
        }
        

        public void giveMeDate(string TextFromReport)   // pobiera datę z raportu
        {
            try
            {
                int textLength = "Date: </B><TD><B>".Length;
                int index1 = TextFromReport.IndexOf("Date: </B><TD><B>");
                int index2 = TextFromReport.IndexOf("</B>", index1 + textLength);
                int length = index2 - (index1 + textLength);
                string data = TextFromReport.Substring(index1 + textLength, length);

                
                if (data.Contains(','))
                {
                    data = changeDateFormatEng(data);
                }
                else if (data.Contains('.'))
                {
                    data = changeDateFormatDot(data);
                }
                else if((!data.Contains('.')) && (!data.Contains('-')))
                {
                    data = changeDateFormatPol(data);
                }
                
               
                rowToAdd[rowToAdd.Count - 1][columnIndex] = data;
                reports[reports.Count - 1].Date = data;


            }
           catch (ArgumentOutOfRangeException ex)
            {
                
            }
            columnIndex++;
        }

        public void giveMeTime(string TextFromReport)
        {
            try
            {
                int textLength = "Time: </B><TD><B>".Length;
                int index1 = TextFromReport.IndexOf("Time: </B><TD><B>");
                int index2 = TextFromReport.IndexOf("</B>", index1 + textLength);
                int length = index2 - (index1 + textLength);
                string data = TextFromReport.Substring(index1 + textLength, length);
                rowToAdd[rowToAdd.Count - 1][columnIndex] = data;
                reports[reports.Count - 1].Time = data;

            }
            catch (ArgumentOutOfRangeException ex)
            {
                
            }
            columnIndex++;
        }

        public void giveMeDateTime()
        {
            string dateTime = rowToAdd[rowToAdd.Count - 1][columnIndex-2].ToString() + ' ' + rowToAdd[rowToAdd.Count - 1][columnIndex-1].ToString();
            rowToAdd[rowToAdd.Count - 1][columnIndex] = dateTime;
            columnIndex++;
        }


        public void giveMeSerialNumber(string TextFromReport) //zwraca numer seryjny
        {
            try
            {
                int textLength = "<LI>Serial Number: </B><TD><B>".Length;
                int index1 = TextFromReport.IndexOf("<LI>Serial Number: </B><TD><B>");
                int index2 = TextFromReport.IndexOf("</B>", index1 + textLength);
                int length = index2 - (index1 + textLength);
                string data = TextFromReport.Substring(index1 + textLength, length);
                rowToAdd[rowToAdd.Count - 1][columnIndex] = data;
                reports[reports.Count - 1].SerialNumber = data;
            }
            catch
            {
                string data = "Serial error";
                rowToAdd[rowToAdd.Count - 1][columnIndex] = data;
                reports[reports.Count - 1].SerialNumber = data;
            }           

            columnIndex++;
        }

        public void giveMeUUTREsult(string TextFromReport) //zwraca wynik testu
        {
            try
            {
                int textLength = "UUT Result: </B><TD><B><FONT COLOR=#008000>".Length;
                int index1 = TextFromReport.IndexOf("UUT Result: </B><TD><B><FONT COLOR=");
                int index2 = TextFromReport.IndexOf("</FONT>", index1 + textLength);
                int length = index2 - (index1 + textLength);

                rowToAdd[rowToAdd.Count - 1][columnIndex] = TextFromReport.Substring(index1 + textLength, length);
            }
            catch
            {
                rowToAdd[rowToAdd.Count - 1][columnIndex] = "UUT Error";
            }

            columnIndex++;
        }

        /*
        public void displayLimits(DataGridViewCellMouseEventArgs e)  //wyswietla limity po rightclicku na header kolumny (nieuzywane)
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
        */


        public void saveToCSV(DataGridView dataGridView1)   //zapisuje datagrida do csv
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "csv";
            saveFileDialog.AddExtension = true;
            if(saveFileDialog.ShowDialog()==DialogResult.OK)
            {
                
                try
                {
                    dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
                    dataGridView1.SelectAll();
                    DataObject dataObject = dataGridView1.GetClipboardContent();
                    File.WriteAllText(saveFileDialog.FileName, dataObject.GetText(TextDataFormat.CommaSeparatedValue));
                }
                catch (Exception exceptionObject)
                {
                    MessageBox.Show(exceptionObject.ToString());
                }
            }
        }
    }
}
