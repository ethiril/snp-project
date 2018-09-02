using System;
using System.Collections.Generic;
using System.Text;
using SNP_First_Test.Network;

namespace SNP_First_Test
{
    /* 
     *   A neuron is made up of multiple rules and a count of spikes which are currently being held
     *   by the neuron.
     */
    public class Neuron
    {
        List<Rule> Rules { get; set; }
        int SpikeCount { get; set; }

        public Neuron(List<Rule> rules, int spikeCount)
        {

            Rules = rules;
            SpikeCount = spikeCount;

        }
        public bool FireSpike()
        {
            foreach (Rule rule in Rules)
                {
                if (rule.isMatched() == true)
                    {
                        return true;
                    }
                } 
            return false;
        }

    }
}
