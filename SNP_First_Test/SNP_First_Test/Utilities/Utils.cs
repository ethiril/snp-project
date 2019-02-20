
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using SNP_Network = SNP_First_Test.Network.Network;


namespace SNP_First_Test.Utilities
{
    class Utils
    {
        public static string RegexAppendStrict(string input)
        {
            input = "^" + input + "$";
            return input;
        }
    }




}
