
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SNP_Network = SNP_First_Test.Network.Network;


/// <summary>
/// Network To Json conversion done using the Newtonsoft JSON library
/// </summary>

namespace SNP_First_Test.Utilities
{
    class Utils
    {
        public static string RegexAppendStrict(string input)
        {
            input = "^" + input + "$";
            return input;
        }

        public static string ConvertNetworkToJson(SNP_Network network)
        {
            return JsonConvert.SerializeObject(network, Formatting.Indented);
        }

        public static SNP_Network ConvertJsonToNetwork(string json)
        {
            return JsonConvert.DeserializeObject<SNP_Network>(json);
        }
        public static void SaveToFile(string data, string filename)
        {
            try
            {
                string path = Directory.GetCurrentDirectory() + "/" + filename;
                File.WriteAllText(path, data);
                Console.WriteLine("Saved the file to: {0}", path);
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
            }
        }

        public static SNP_Network ReadNetworkFromFile(string filename)
        {
            try
            {
                string path = Directory.GetCurrentDirectory();
                try
                {
                    using (StreamReader r = new StreamReader(filename))
                    {
                        string parsedJson = r.ReadToEnd();
                        return ConvertJsonToNetwork(parsedJson);
                    }
                } catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static void SaveNetwork(SNP_Network network, string filename)
        {
            Console.WriteLine("----------- Saving Network -----------");
            string json = ConvertNetworkToJson(network);
            SaveToFile(json, filename);
        }

        public static string ConvertToCSV(List<float> fitnessList)
        {
            return String.Join(",", fitnessList.Select(x => x.ToString()).ToArray());
        }

        public static void CreateFolder(string folderName)
        {
            Console.WriteLine("----------- Creating Folder {0} -----------", folderName);
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/" + folderName);
        }

        public static void SaveCSV(List<float> fitnessList, string filename)
        {
            Console.WriteLine("----------- Saving Data -----------");
            string csv = ConvertToCSV(fitnessList);
            SaveToFile(csv, filename);
        }




    }





}
