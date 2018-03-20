using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HTMLParser2
{
    public class Report
    {
        private string serialnumber;
        public string SerialNumber
        {
            get
            {
                return serialnumber;
            }
            set
            {
                serialnumber = value;
            } 
        }

        private bool _UUTresult;
        public bool UUTresult
        {
            get
            {
                return _UUTresult;
            }
            set
            {
                _UUTresult = value;
            }
        }
        private DateTime dateTime;
        public DateTime DateTime
        {
            get
            {
                return dateTime;
            }
        }

        private string time;
        public string Time
        {
            set
            {
                time = value;
            }
        }
        private string date;
        public string Date
        {
            set
            {
                date = value;
            }
        }

        public void setDateTime()
        {
            string[] dates = date.Split('-');
            string[] times = time.Split(':');

            int y = int.Parse(dates[0]);
            int m = int.Parse(dates[1]);
            int d = int.Parse(dates[2]);

            int h = int.Parse(times[0]);
            int min = int.Parse(times[1]);
            int s = int.Parse(times[2]);

            dateTime = new DateTime(y, m, d, h, min, s);
        }

    }
}
