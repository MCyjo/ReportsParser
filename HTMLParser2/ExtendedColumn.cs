using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HTMLParser2
{
    public delegate bool CheckLimitDelegate(DataGridViewCell dC);

    public class ExtendedColumn : DataColumn
    {

        private CheckLimitDelegate checkLimitDelegate;  //delegata do metod sprawdzających limity
        public CheckLimitDelegate _checkLimitDelegate
        {
            get
            {
                return checkLimitDelegate;
            }
        }

        private double? lowlimit;   
        public double? Lowlimit
        {
            get
            {
                return lowlimit;
            }
        }
        private double? highlimit;
        public double? Highlimit
        {
            get
            {
                return highlimit;
            }
        }

        public ExtendedColumn(string str) : base(str)   //konstruktor
        {
            this.ColumnName = str;
            checkLimitDelegate = new CheckLimitDelegate(checkLimitsGELE);
        }

        public string giveLimits()  //zwraca limity danej kolumny (pobierane do nagłówków kolumn)
        {
            return "Low: " + lowlimit + ";  High: " + highlimit;
        }

        public string findLimitsMulti(int index, int x, string TextFromReport)  //wyszukuje limity w krokach, w których zawarte jest kilka pomiarów
        {
            try
            {
                int index2 = TextFromReport.IndexOf("Measurement[" + x.ToString() + "]", index);
                int indexEnd = TextFromReport.IndexOf("Status", index2 + 15);
                int leng = indexEnd - index2;

                string subtext = TextFromReport.Substring(index2, leng);

                int index3 = subtext.IndexOf("Low:");
                int index4 = subtext.IndexOf("<FONT SIZE=-1>", index3);
                int index5 = subtext.IndexOf("\r\n<TR>", index3);
                int length = index5 - (index4 + "<FONT SIZE=-1>".Length);

                string lowLimitString = subtext.Substring(index4 + "<FONT SIZE=-1>".Length, length);
                lowlimit = double.Parse(lowLimitString.Replace('.', ','));
            }
            catch
            {
                lowlimit = null;
            }

            try
            {
                int index2 = TextFromReport.IndexOf("Measurement[" + x.ToString() + "]", index);
                int indexEnd = TextFromReport.IndexOf("Status", index2 + 15);
                int leng = indexEnd - index2;

                string subtext = TextFromReport.Substring(index2, leng);

                int index3 = subtext.IndexOf("High:");
                int index4 = subtext.IndexOf("<FONT SIZE=-1>", index3);
                int index5 = subtext.IndexOf("\r\n<TR>", index4);
                int length = index5 - (index4 + "<FONT SIZE=-1>".Length);

                string highLimitString = subtext.Substring(index4 + "<FONT SIZE=-1>".Length, length);
                highlimit = double.Parse(highLimitString.Replace('.', ','));
            }
            catch
            {
                highlimit = null;
            }

            return ("   Lower: " + lowlimit + "   Upper:" + highlimit);
        }

        public string findLimits(int index, string TextFromReport)  //pobiera limity
        {
            try
            {
                int index2 = TextFromReport.IndexOf("Low:", index, 1000);
                int index3 = TextFromReport.IndexOf("<FONT SIZE=-1>", index2);
                int index4 = TextFromReport.IndexOf("\r\n<TR>", index3);
                int length = index4 - (index3 + "<FONT SIZE=-1>".Length);
                string test = TextFromReport.Substring(index3 + "<FONT SIZE=-1>".Length, length).Replace('.', ',');
                try
                {
                    string lowLimitString = TextFromReport.Substring(index3 + "<FONT SIZE=-1>".Length, length);
                    lowlimit = double.Parse(lowLimitString.Replace('.', ','));
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
                int index2 = TextFromReport.IndexOf("High:", index, 1000);
                int index3 = TextFromReport.IndexOf("<FONT SIZE=-1>", index2);
                int index4 = TextFromReport.IndexOf("\r\n<TR>", index3);
                int length = index4 - (index3 + "<FONT SIZE=-1>".Length);
                string test = TextFromReport.Substring(index3 + "<FONT SIZE=-1>".Length, length).Replace('.', ',');
                try
                {
                    string highLimitString = TextFromReport.Substring(index3 + "<FONT SIZE=-1>".Length, length);
                 //   highLimitString = highLimitString.Replace('.', ',');
                    highlimit = double.Parse(highLimitString.Replace('.', ','));
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

            return ("   Lower: " + lowlimit + "   Upper:" + highlimit);
        }

        public void DefineMultipleLimitsType(string textFromReport, string stepName, int iteration)     //określa rodzaj limitów w ktokach z wieloma pomiarami
                                                                                                        //(równe, mniejszy od, mniejszy od/większy od itd)
        {
            try
            {
                int index0 = textFromReport.IndexOf(stepName);
                int index1 = textFromReport.IndexOf("Measurement[" + iteration + "]", index0);
                int index2 = textFromReport.IndexOf("Comparison Type:", index1);
                int index3 = textFromReport.IndexOf("<FONT SIZE=-1>", index2);
                int index4 = textFromReport.IndexOf("<TR><TD", index3);
                int length = index4 - (index3 + "<FONT SIZE=-1>".Length);
                string limitsType = textFromReport.Substring(index3 + "<FONT SIZE=-1>".Length, length);

                switch (limitsType)
                {
                    case "GELE (>= <=)\r\n":
                        checkLimitDelegate = checkLimitsGELE;
                        break;
                    case "LEGE (<= >=)r\n":
                        checkLimitDelegate = checkLimitsLEGE;
                        break;
                    case "LE (<=)\r\n":
                        checkLimitDelegate = checkLimitsLE;
                        break;
                    case "GE (<=)\r\n":
                        checkLimitDelegate = checkLimitsGE;
                        break;
                    case "EQ (==)\r\n":
                        checkLimitDelegate = checkLimitsEQ;
                        break;
                    case "GT (>)\r\n":
                        checkLimitDelegate = checkLimitsGT;
                        break;
                    case "LT (>)\r\n":
                        checkLimitDelegate = checkLimitsLT;
                        break;
                    case "No Comparison\r\n":
                        checkLimitDelegate = checkLimitsNoComp;
                        break;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

       

        internal void DefineLimitsType(string textFromReport)   //określa rodzaj limitów dla danego kroku
        {
            try
            {
                int index1 = textFromReport.IndexOf(ColumnName);
                int index2 = textFromReport.IndexOf("Comparison Type:", index1);
                int index3 = textFromReport.IndexOf("<FONT SIZE=-1>", index2);
                int index4 = textFromReport.IndexOf("<TR><TD", index3);
                int length = index4 - (index3 + "<FONT SIZE=-1>".Length);
                string limitsType = textFromReport.Substring(index3 + "<FONT SIZE=-1>".Length, length);

                switch (limitsType)
                {
                    case "GELE (>= <=)\r\n":
                        checkLimitDelegate = checkLimitsGELE;
                        break;
                    case "LEGE (<= >=)r\n":
                        checkLimitDelegate = checkLimitsLEGE;
                        break;
                    case "LE (<=)\r\n":
                        checkLimitDelegate = checkLimitsLE;
                        break;
                    case "GE (<=)\r\n":
                        checkLimitDelegate = checkLimitsGE;
                        break;
                    case "EQ (==)\r\n":
                        checkLimitDelegate = checkLimitsEQ;
                        break;
                    case "GT (>)\r\n":
                        checkLimitDelegate = checkLimitsGT;
                        break;
                    case "LT (>)\r\n":
                        checkLimitDelegate = checkLimitsLT;
                        break;
                    case "No Comparison\r\n":
                        checkLimitDelegate = checkLimitsNoComp;
                        break;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private bool checkLimitsNoComp(DataGridViewCell dC)     //dlakroków bez określonego limitu
        {
            return true;
        }

        public bool checkLimitsGELE(DataGridViewCell dC)   
        {
            string str = dC.Value.ToString();

            try
            {

                double g = double.Parse(str);


                if (g >= lowlimit && g <= highlimit)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool checkLimitsLEGE(DataGridViewCell dC)
        {
            string str = dC.Value.ToString();

            try
            {

                double g = double.Parse(str);


                if (g <= lowlimit && g >= highlimit)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool checkLimitsLE(DataGridViewCell dC)
        {
            string str = dC.Value.ToString();

            try
            {

                double g = double.Parse(str);


                if (g <= lowlimit)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool checkLimitsLT(DataGridViewCell dC)
        {
            string str = dC.Value.ToString();

            try
            {

                double g = double.Parse(str);


                if (g < lowlimit)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool checkLimitsGE(DataGridViewCell dC)
        {
            string str = dC.Value.ToString();

            try
            {

                double g = double.Parse(str);


                if (g >= highlimit)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool checkLimitsGT(DataGridViewCell dC)
        {
            string str = dC.Value.ToString();

            try
            {

                double g = double.Parse(str);


                if (g > highlimit)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool checkLimitsEQ(DataGridViewCell dC)
        {
            string str = dC.Value.ToString();

            try
            {
                double g = double.Parse(str);

                if (g == lowlimit)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}


       

