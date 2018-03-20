using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HTMLParser2
{
    class HandleInputs
    {
        private string[] fieldsToRead;
        public string[] FieldsToRead
        {
            get
            {
                return fieldsToRead;
            }
        }

        public List<string> textFromReport;


        Main mainForm;
        

        public HandleInputs(Main main)
        {
            mainForm = main;
            textFromReport = new List<string>();
        }

        public void LoadReport()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach(string file in openFileDialog1.FileNames)
                {
                    try
                    {
                        textFromReport.Add(System.IO.File.ReadAllText(file));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                    }
                }
                
            }
        }

        public void ReadUserList()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    fieldsToRead = System.IO.File.ReadAllLines(openFileDialog1.FileName);
                    foreach(string str in fieldsToRead)
                    {
                    //    mainForm.textBox1.Text += str+Environment.NewLine;
                    }
                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
    }
}
