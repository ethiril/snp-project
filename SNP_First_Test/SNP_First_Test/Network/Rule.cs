using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using SNP_First_Test.Utilities;

namespace SNP_First_Test.Network
{
    // https://stackoverflow.com/questions/6488034/how-to-implement-a-rule-engine could be an interesting implementation
    /* Each Rule will contain a set of instructions which will fire a neuron across an axon */
    public class Rule
    {
        public string RuleExpression { get; set; }
        public int DelayAmount { get; set; }
        public int Delay { get; set; }
        public bool Fire { get; set; }

        public Rule(string ruleExpression, int delayAmount, bool fire)
        {
            RuleExpression = ruleExpression;
            DelayAmount = delayAmount;
            Fire = fire;
            Delay = delayAmount;
        }

        /* 
         * This method checks whether the provided regex and the provided spike input string, which should be equal to the amount of spikes within the neuron 
         * If the input rule isn't null, append strict ruling on the rule
         * If the rule matches directly with the given spikes, return true
         * otherwise the rule did not match, therefore return false
         * if the rule was empty, return an error and a false flag.
         */

        public bool RegexMatch(string spikes)
        {
            if (this.RuleExpression != null) {
                string input = Utils.RegexAppendStrict(RuleExpression);
                Regex rgx = new Regex(input);
                if (rgx.IsMatch(spikes)) {
                    return true;
                } else
                {
                    return false;
                }
            } else
            {
                Console.Error.WriteLine("No rule provided");
                return false;
            }
        }

        public bool? IsMatched(string currentSpikeAmount)
        {
            if (RegexMatch(currentSpikeAmount))
            {
                if (this.Fire)
                {
                    if (this.DelayAmount > 0)
                    {
                        Console.WriteLine("This rule has a delay of " + this.DelayAmount + " step(s), of which there are " + this.Delay + " step(s) left.");
                        this.Delay--;
                        return false;

                    }
                    else
                    {
                        this.Delay = this.DelayAmount;
                        return true;
                    }
                }
                else
                {
                    return null;
                }

            }
            else
            {
                return false;
            }
        }
    }
}
