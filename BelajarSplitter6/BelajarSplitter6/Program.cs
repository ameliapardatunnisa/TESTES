using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;

namespace BelajarSplitter
{
    class Program
    {
        private static int instrument_id = 2;
        private static string no_lab = "";
        private static List<string> arr_result = new List<string>();
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
            string text = File.ReadAllText("data6.txt");
            original_data = text;
            string[] data_split = text.ToString().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var myString in data_split)
            {
                bool status_string = string.IsNullOrEmpty(myString);
                if (status_string == false)
                {
                    splitData(myString);
                }

            }
            convertToJson();
        }

        private static void splitData(string data)
        {
            try
            {
                date_time_on_machine = DateTime.Now.ToString("yyyyMMddHHmmss");
                no_lab = data.Substring(14, 9);
                string Natrium = data.Substring(34, 5);
                string Kalsium = data.Substring(29, 4);
                string Kalium = data.Substring(40, 5);
                string NCA = data.Substring(46, 4);
                string konket1, konket2, konket3, konket4 = "";

                konket1 = no_lab + "|" + "Na" + "|" + Natrium + "|" + date_time_on_machine;
                konket2 = no_lab + "|" + "K" + "|" + Kalsium + "|" + date_time_on_machine;
                konket3 = no_lab + "|" + "Cl" + "|" + Kalium + "|" + date_time_on_machine;
                konket4 = no_lab + "|" + "NCA" + "|" + NCA + "|" + date_time_on_machine;

                arr_result.Add(konket1); arr_result.Add(konket2); arr_result.Add(konket3); arr_result.Add(konket4);

                // CARA LAIN

                // StringBuilder hasilBuilders = new StringBuilder();
                // hasilBuilders.Append(noLab).Append("|Na|").Append(Natrium).Append(date_time_on_machine);
                // arr_result.Add(konketBuilders.ToString());

                // konketBuilders.Clear();
                // hasilBuilders.Append(noLab).Append("|K|").Append(Kalsium).Append(date_time_on_machine);
                // arr_result.Add(konketBuilders.ToString());

                // konketBuilders.Clear();
                // hasilBuilders.Append(noLab).Append("|Cl|").Append(Kalium).Append(date_time_on_machine);
                // arr_result.Add(konketBuilders.ToString());

                // konketBuilders.Clear();
                // hasilBuilders.Append(noLab).Append("|NCA|").Append(NCA).Append(date_time_on_machine);
                // arr_result.Add(konketBuilders.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error parsing date: {e.Message}");
            }
        }

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