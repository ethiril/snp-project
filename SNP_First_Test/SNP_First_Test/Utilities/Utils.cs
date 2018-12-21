
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using SNP_Network = SNP_First_Test.Network.Network;


namespace SNP_First_Test.Utilities
{
    class Utils
    {
        // Turn json object into string
        public static string JsonDeserialize(Object jsonInput)
        {
            string jsonParse = JsonConvert.SerializeObject(jsonInput);
            return jsonParse;
        }


        /* 
         * This method will evalute if the provided input is in the correct pattern. The regex rules that we want only contain the +, * and set keys on character a.
         */

        public static void ResetNetwork(SNP_Network initial, SNP_Network network)
        {
            network.CurrentOutput = 0;
            network.GlobalTimer = 0;
            for (int i = 0; i < network.Neurons.Count; i++)
            {
                network.Neurons[i].SpikeCount = initial.Neurons[i].SpikeCount;
                for (int j = 0; j < network.Neurons[i].Rules.Count; j++)
                {
                    network.Neurons[i].Rules[j].Delay = 0;
                }
            }
            network.OutputSet.Clear();
            network.NetworkClear = false;
            network.NetworkEngaged = false;
        }
        
        public static string RegexAppendStrict(string input)
        {
            input = "^" + input + "$";
            return input;
        }
    }


}
