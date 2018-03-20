using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HTMLParser2
{
    public class ExtendedColumn : DataColumn
    {
        private double? lowlimit;
        private double? highlimit;

        public ExtendedColumn(string str) : base(str)
        {
            this.ColumnName = str;
        }

        public string giveLimits()
        {
            return "Low: "+lowlimit+";  High: " + highlimit;
        }

        public void findLimitsMulti(int index, int x, string TextFromReport)
        {
            try
            {
                int index2 = TextFromReport.IndexOf("Measurement[" + x.ToString() + "]",index);
                int index3 = TextFromReport.IndexOf("Low:", index2);
                int index4 = TextFromReport.IndexOf("<FONT SIZE=-1>", index3);
                int index5 = TextFromReport.IndexOf("\r\n<TR>", index3);
                int length = index5 - (index4 + 14);

                lowlimit = double.Parse(TextFromReport.Substring(index4 + 14, length));
            }
            catch
            {
                lowlimit = null;
            }

            try
            {
                int index2 = TextFromReport.IndexOf("Measurement[" + x.ToString() + "]",index);
                int index3 = TextFromReport.IndexOf("High:", index2);
                int index4 = TextFromReport.IndexOf("<FONT SIZE=-1>", index3);
                int index5 = TextFromReport.IndexOf("\r\n<TR>", index4);
                int length = index5 - (index4 + 14);

                highlimit = double.Parse(TextFromReport.Substring(index4 + 14, length));
            }
            catch
            {
                highlimit = null;
            }
        }

        public void findLimits(int index, string TextFromReport)
        {
            try
            {
                int index2 = TextFromReport.IndexOf("Low:", index,1000);
                int index3 = TextFromReport.IndexOf("<FONT SIZE=-1>", index2);
                int index4 = TextFromReport.IndexOf("\r\n<TR>", index3);
                int length = index4 - (index3 + "<FONT SIZE=-1>".Length);
                string test = TextFromReport.Substring(index3 + "<FONT SIZE=-1>".Length, length).Replace('.', ',');
                try
                {
                    lowlimit = double.Parse((TextFromReport.Substring(index3 + "<FONT SIZE=-1>".Length, length)).Replace('.',','));
                }
                catch (Exception e)
                {
                 //   MessageBox.Show(e.ToString());
                }
            }
            catch
            {
                lowlimit = null;
            }

            try
            {
                int index2 = TextFromReport.IndexOf("High:", index,1000);
                int index3 = TextFromReport.IndexOf("<FONT SIZE=-1>", index2);
                int index4 = TextFromReport.IndexOf("\r\n<TR>", index3);
                int length = index4 - (index3 + "<FONT SIZE=-1>".Length);
                string test = TextFromReport.Substring(index3 + "<FONT SIZE=-1>".Length, length).Replace('.', ',');
                try
                {
                    highlimit = double.Parse((TextFromReport.Substring(index3 + "<FONT SIZE=-1>".Length, length)).Replace('.', ','));
                }
                catch (Exception e)
                {
                 //   MessageBox.Show(e.ToString());
                }
            }
            catch
            {
      
                highlimit = null;
            }
        }


        public bool checkLimits(DataGridViewCell dC)   //do poprawki
        {
            string str = dC.Value.ToString();

           try
            {

                double g = double.Parse(str);

                if ((lowlimit != null) && (highlimit != null))
                {
                    if (g >= lowlimit && g <= highlimit)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (lowlimit != null)
                {
                    if (g <= lowlimit)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (highlimit != null)
                {
                    if (g >= highlimit)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
                
            }
          
        }
    }

