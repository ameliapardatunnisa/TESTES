using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;

namespace BelajarSplitter
{
    class Program
    {
        private static int instrument_id = 1;
        private static string no_lab = "";
        private static List<string> arr_result = new List<string>();
        private static List<string> arrTest = new List<string>(new string[] {
            "GLU", "GOT", "GPT","URE","CRE","UA","CHO","HDL","LDL","TRG","BILT","BILD","PRO","ALB","GGT","ALP"
});
        private static string original_data = "";
        private static string date_time_on_machine = "";

        static void Main(string[] args)
        {
            readFile();
            Console.WriteLine("\n Press any key to continue...");
            Console.ReadKey();
        }

        private static void readFile()
        {
            string text = File.ReadAllText("data8.txt");
            original_data = text;
            no_lab = original_data.ToString().Substring(0, 15).TrimStart();
            string data = text.Trim();
            data = data.Substring(0, data.Length - 3);

            List<string> dataspliteSpace = new List<string>(data.Split(null));

            string dt = DateTime.Now.ToString("ddMMyyyyHHmmss");
            for (int i = 0; i < arrTest.Count(); i++)
            {
                string strData = arrTest[i].ToString();
                var match = dataspliteSpace.FirstOrDefault(stringToCheck => stringToCheck.Contains(strData));
                if (match != null)
                {
                    string strTest = arrTest[i].ToString();
                    string getResult = match.Replace(strTest, "^").Split("^")[1];
                    var doubleArray = Regex.Split(getResult, @"[^0-9\.]+")[0]; //menghapus semua karakter kecuali angka 0-9
                    string combineStr = no_lab + "|" + strTest + "|" + doubleArray + "|" + dt;
                    arr_result.Add(combineStr);
                }
            }
            convertToJson();
        }


        // private static void splitData(string data)
        // {
        //     try
        //     {
        //         no_lab = data.Substring(8, 7);

        //         string GLU = data.Substring(134, 7);
        //         string GOT = data.Substring(24, 7);
        //         string GPT = data.Substring(35, 7);
        //         string URE = data.Substring(46, 7);
        //         string CRE = data.Substring(57, 7);
        //         string UA = data.Substring(68, 7);
        //         string CHO = data.Substring(79, 7);
        //         string HDL = data.Substring(156, 7);
        //         string LDL = data.Substring(167, 7);
        //         string TRG = data.Substring(101, 7);
        //         string BILT = data.Substring(178, 7);
        //         string BILD = data.Substring(189, 7);
        //         string PRO = data.Substring(123, 7);
        //         string ALB = data.Substring(90, 7);
        //         string GGT = data.Substring(112, 7);
        //         string ALP = data.Substring(145, 7);

        //         date_time_on_machine = DateTime.Now.ToString("yyyyMMddHHmmss");

        //         string konket1, konket2, konket3, konket4, konket5, konket6, konket7, konket8, konket9, konket10, konket11, konket12, konket13, konket14, konket15, konket16 = "";

        //         konket1 = no_lab + "|" + "GLU" + "|" + GLU + "|" + date_time_on_machine;
        //         konket2 = no_lab + "|" + "GOT" + "|" + GOT + "|" + date_time_on_machine;
        //         konket3 = no_lab + "|" + "GPT" + "|" + GPT + "|" + date_time_on_machine;
        //         konket4 = no_lab + "|" + "URE" + "|" + URE + "|" + date_time_on_machine;
        //         konket5 = no_lab + "|" + "CRE" + "|" + CRE + "|" + date_time_on_machine;
        //         konket6 = no_lab + "|" + "UA" + "|" + UA + "|" + date_time_on_machine;
        //         konket7 = no_lab + "|" + "CHO" + "|" + CHO + "|" + date_time_on_machine;
        //         konket8 = no_lab + "|" + "HDL" + "|" + HDL + "|" + date_time_on_machine;
        //         konket9 = no_lab + "|" + "LDL" + "|" + LDL + "|" + date_time_on_machine;
        //         konket10 = no_lab + "|" + "TRG" + "|" + TRG + "|" + date_time_on_machine;
        //         konket11 = no_lab + "|" + "BILT" + "|" + BILT + "|" + date_time_on_machine;
        //         konket12 = no_lab + "|" + "BILD" + "|" + BILD + "|" + date_time_on_machine;
        //         konket13 = no_lab + "|" + "PRO" + "|" + PRO + "|" + date_time_on_machine;
        //         konket14 = no_lab + "|" + "ALB" + "|" + ALB + "|" + date_time_on_machine;
        //         konket15 = no_lab + "|" + "GGT" + "|" + GGT + "|" + date_time_on_machine;
        //         konket16 = no_lab + "|" + "ALP" + "|" + ALP + "|" + date_time_on_machine;

        //         arr_result.Add(konket1); arr_result.Add(konket2); arr_result.Add(konket3); arr_result.Add(konket4);
        //         arr_result.Add(konket5); arr_result.Add(konket6); arr_result.Add(konket7); arr_result.Add(konket8);
        //         arr_result.Add(konket9); arr_result.Add(konket10); arr_result.Add(konket11); arr_result.Add(konket12);
        //         arr_result.Add(konket13); arr_result.Add(konket14); arr_result.Add(konket15); arr_result.Add(konket16);
        //     }
        //     catch (Exception e)
        //     {
        //         Console.WriteLine($"Error parsing date: {e.Message}");
        //     }
        // }

        private static void convertToJson()
        {
            if (arr_result.Count > 0)
            {
                Thread.Sleep(1500);
                no_lab = no_lab != "" ? no_lab : DateTime.Now.ToString("ddMMyyyyHHmmss");
                string dt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                JObject obj = JObject.FromObject(new
                {
                    date = DateTime.Now,
                    instrument_id = instrument_id,
                    no_labs = no_lab,
                    result = arr_result,
                    date_time_machine = date_time_on_machine,
                    sample_destination_codes = "",
                    text_distinction_codes = "",
                    message_instrument = "",
                    type = "ASTM"
                });

                Console.WriteLine("Data JSON " + obj.ToString());
            }
        }
    }
}