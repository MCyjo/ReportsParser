using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLParser2
{
    class TableCreator
    {
        Main mainForm;
        private HandleInputs handleInputs;

        DataTable dt;

        public TableCreator(Main mForm, HandleInputs handleInputs)
        {
            this.handleInputs = handleInputs;  
            this.mainForm = mForm;
            dt = new DataTable();     
        }

        public void prepareGrid()
        {
            foreach(string str in handleInputs.FieldsToRead)
            {
                mainForm.dataGridView1.Columns.Add(str, str);
            }
            
        }
    }
}
