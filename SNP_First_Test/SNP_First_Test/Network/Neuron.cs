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
        // List of rules that will determine whether a neuron will spike
        List<Rule> Rules { get; set; }
        // Current spike count
        int SpikeCount { get; set; }
        // List of connections for this neuron
        List<Neuron> Connections { get; set; }

        bool IsOutput { get; set; }

        // Constructor
        public Neuron(List<Rule> rules, int spikeCount, List<Neuron> connections, bool isOutput)
        {
            Rules = rules;
            SpikeCount = spikeCount;
            Connections = connections;
            IsOutput = isOutput;
        }

        // Temp code for sending a spike across to another neuron
        // http://bezensek.com/blog/2015/04/12/non-deterministic-finite-state-machine-implementation-in-c-number/
        // Will need the above for a non-deterministic approach to this implementation
        public bool? FireSpike(List<Neuron>Connections)
        {
            // Attempt at non-deterministic code
            // int ActiveSpikeCount = 0;
            // Go through every rule in the Rules list
            foreach (Rule rule in this.Rules)
            {
                // if the code for isMatched returns true (Rule has been fullfilled)
                // this whole piece of logic can be changed to not use nulls, the ones in the rules class
                // aren't really used. Only this method output needs them.
                if ((rule.isMatched(this.SpikeCount) == true) || (rule.isMatched(this.SpikeCount) == null))
                {
                    if (rule.isMatched(this.SpikeCount) == null)
                    {
                        this.SpikeCount = this.SpikeCount - rule.SpikeAmount;
                        return null;
                    }
                    else
                    {
                        this.SpikeCount = this.SpikeCount-rule.SpikeAmount;
                        
                        foreach (Neuron neuron in this.Connections)
                        {
                            neuron.SpikeCount++;
                        }
                        return true;
                    }
                }
                Console.WriteLine("No Rules matched.");
                return false;
            }
            // default
            return false;
        }

    }
}
