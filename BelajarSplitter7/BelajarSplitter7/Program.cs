using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleSplitterErbaLauraPlazaMedika
{
    class Program
    {
        private static string sample_no = "";
        private static string jenis_kelamin = "";
        private static string message = "";
        private static List<string> arr_result = new List<string>();
        private static string original_data = "";
        private static string date_time_on_machine = "";
        private static List<string> test_urine = new List<string> { "WBC", "LYM", "MON", "GRA", "LY%", "MO%", "GR%", "RBC", "HGB", "HCT", "MCV", "MCH", "MCHC", "RDWc", "RDWs", "PLT", "PCT", "MPV", "PDWc", "PDWs", "PLCC", "PLCR" };
        private static string no_lab = "";
        private static int instrument_id = 14;
        private static string instrument_name = "ERBA LAURA";

        static void Main(string[] args)
        {
            string text = File.ReadAllText("data7.txt");
            original_data = text;
            string[] data_split = text.ToString().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            List<char> charsToRemove = new List<char>() { '-', 'E' };
            string dateOnly = null;
            string timeOnly = null;
            string dateAndTime = null;

            date_time_on_machine = DateTime.Now.ToString("yyyyMMddHHmmss"); ;
            foreach (var myString in data_split)
            {
                bool status_string = string.IsNullOrEmpty(myString);
                if (status_string == false)
                {
                    if (myString.Contains("Sample ID"))
                    {
                        string tempNoLab = Regex.Match(myString, @"\d+").Value;
                        if (!String.IsNullOrEmpty(tempNoLab))
                        {
                            no_lab = tempNoLab.Substring(0);
                        }
                        else
                        {
                            no_lab = DateTime.Now.ToString("yyyyMMddHHmmss");

                        }
                    }

                    if (myString.Contains("Test date"))
                    {
                        dateOnly = myString.Trim().Substring(16);
                    }

                    if (myString.Contains("Test time"))
                    {
                        timeOnly = myString.Trim().Substring(15);
                    }

                    if (!String.IsNullOrEmpty(dateOnly) && !String.IsNullOrEmpty(timeOnly))
                    {
                        dateAndTime = dateOnly + " " + timeOnly;
                        DateTime dateTime = DateTime.ParseExact(dateAndTime, "yyyyMMdd HHmmss", CultureInfo.InvariantCulture);
                        date_time_on_machine = dateTime.ToString("yyyyMMddHHmmss");
                    }

                    foreach (var x in test_urine)
                    {
                        bool cari_test = myString.ToString().Contains(x);
                        if (cari_test == true)
                        {
                            string[] str = myString.Filter(charsToRemove).TrimStart().ToString().Split(null);
                            str = str.Where(y => !string.IsNullOrEmpty(y)).ToArray();
                            string testParams = str[0].ToString().Trim();
                            string testResult = "";
                            if (str.Length >= 4)    //memnyesuaikan array
                            {
                                testResult = str[1].ToString().Trim();
                            }
                            else if (str.Length <= 3)
                            {
                                testResult = str[0].ToString().Trim();
                            }
                            string konket = "";

                            konket = no_lab + "|" + testParams + "|" + testResult + "|" + date_time_on_machine;
                            arr_result.Add(konket);
                        }
                    }
                }
            }
            convertToJson();
            Console.WriteLine("Press any key to continue...");
            Console.WriteLine();
            Console.ReadKey();
        }

        private static void convertToJson()
        {
            if (arr_result.Count > 0)
            {
                Thread.Sleep(1500);
                string dt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                JObject obj = JObject.FromObject(new
                {
                    date = DateTime.Now,
                    instrument_id = instrument_id,
                    no_labs = no_lab,
                    result = arr_result.Distinct().ToArray(),
                    original_result = original_data,
                    date_time_machine = date_time_on_machine,
                    sample_destination_codes = "",
                    text_distinction_codes = "",
                    message_instrument = "",
                    type = "ASTM_ALPHANUMERIC"
                });

                string strQueue = obj.ToString() + "@" + instrument_name + "@1";
                Console.WriteLine("Data JSON " + obj.ToString());
            }
        }
    }

    public static class Extensions
    {
        //https://www.techiedelight.com/remove-specific-characters-from-string-csharp/
        public static string Filter(this string str, List<char> charsToRemove)
        {
            foreach (char c in charsToRemove)
            {
                str = str.Replace(c.ToString(), String.Empty);
            }

            return str;
        }
    }
}

