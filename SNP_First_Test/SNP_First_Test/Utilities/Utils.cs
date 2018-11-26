﻿
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace SNP_First_Test.Utilities
{
    class Utils
    {
        public static void JsonDeserialize()
        {
        }
        /*
        public int RegexResolve(string input)
        {
            if (Regex.IsMatch(input, @"(\([^)]*\)|\w)\^(\([^)]*\)|\w)"))
            {

            };
            int count = Regex.Matches(input.Substring(0), "a").Count;
            return count;
        } */

        /* 
         *  Regex just needs to parse in a valid expression which will then evaluate to the pattern of spikes that is accepted.
         * 
         * 
         */


        /* 
         * This method will evalute if the provided input is in the correct pattern. The regex rules that we want only contain the +, * and set keys on character a.
         */ 
        public static string RegexAppendStrict(string input)
        {
            input = "^" + input + "$";
            return input;
        }

        double EvaluateExpression(string input)
        {
            /*
             * Split the string into "groups" of a's, then do some arithmetic on them
             * Amount of spikes has to be a natural number larger than 1.
             */
            string[] groupings = Regex.Split(input, @"\[?\@?\-?\w+\]?|\'[\w\s\/]*\'");
            List<int> evaluatedGroupings = new List<int>();
            foreach (string group in groupings)
            {
                evaluatedGroupings.Add(EvaluateSpikeGroupings(group));
            }
            // add these back into the original expression as numbers.
            // for example: aaa(aa)+a^3 would equate to 3(2)+3

            var output = Regex.Replace(input, "[^MT]", string.Empty);
            return Convert.ToDouble(new DataTable().Compute(input, null));
        }

        bool IsExpression(string expression)
        {
            var regex = new Regex(@"(?x)
                ^
                (?> (?<p> \( )* (?>-?\d+(?:\.\d+)?) (?<-p> \) )* )
                (?>(?:
                    [-+*/]
                    (?> (?<p> \( )* (?>-?\d+(?:\.\d+)?) (?<-p> \) )* )
                )*)
                (?(p)(?!))
                $
            ");
            return (regex.IsMatch(expression) ? true : false);
        }

        //return 
        int EvaluateSpikeGroupings(string input)
        {
            int count = 0;
            foreach (char c in input)
                if (c == 'a') count++;
            return count;
        }
    }


}
