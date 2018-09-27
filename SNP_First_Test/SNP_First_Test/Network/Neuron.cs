using System;
using System.Collections.Generic;
using System.Text;
using SNP_First_Test.Network;
using SNP_Network = SNP_First_Test.Network.Network;

namespace SNP_First_Test
{
    /* 
     *   A neuron is made up of multiple rules and a count of spikes which are currently being held
     *   by the neuron.
     */
    public class Neuron
    {
        // List of rules that will determine whether a neuron will spike
        public List<Rule> Rules { get; set; }
        // Current spike count
        public int SpikeCount { get; set; }
        // List of connections for this neuron
        public List<int> Connections { get; set; }

        public bool IsOutput { get; set; }

        // Constructor
        public Neuron(List<Rule> rules, int spikeCount, List<int> connections, bool isOutput)
        {
            Rules = rules;
            SpikeCount = spikeCount;
            Connections = connections;
            IsOutput = isOutput;
        }

        // Temp code for sending a spike across to another neuron
        // http://bezensek.com/blog/2015/04/12/non-deterministic-finite-state-machine-implementation-in-c-number/
        // Will need the above for a non-deterministic approach to this implementation
        public bool FireSpike(SNP_Network networkRef, List<int> Connections)
        {
            // Attempt at non-deterministic code
            // int ActiveSpikeCount = 0;
            // Go through every rule in the Rules list
            /* 
             * If List has more than one rule
             * Check if the neuron has enough spike to satisfy some or all of the rules
             * If only one rule can proceed, complete that spike
             * If more than one rule can proceed at any one time use the Random() function to determine which will fire
             * Fire the spike on that chosen rule definition
             */
            Random random = new Random();
            int matchedCount = 0;
            foreach (Rule rule in this.Rules)
            {
                // if the code for isMatched returns true (Rule has been fullfilled)
                // this whole piece of logic can be changed to not use nulls, the ones in the rules class
                // aren't really used. Only this method output needs them.
                if ((rule.isMatched(this.SpikeCount) == null) || (rule.isMatched(this.SpikeCount) == true))
                {
                    matchedCount++;
                }
            }
            // if there is just one rule that is fullfilled then the random will always just 0, hence no need to check.
            // otherwise if more rules are matched it will be chosen at random

            /* 
             * 
             * This snippet is broken, connections for snippets need to be corrected. 
             * 
             */
            int index = random.Next(0, matchedCount);
            Console.WriteLine("State: " + this.Rules[index].isMatched(this.SpikeCount));
            if (this.Rules[index].isMatched(this.SpikeCount) == null)
            {
                Console.WriteLine("This one is null.");
                Console.WriteLine("This.SpikeCount = " + this.SpikeCount + ", this.Rules.SpikeAmount = " + this.Rules[index].SpikeAmount);
                this.SpikeCount = this.SpikeCount - this.Rules[index].SpikeAmount;
                return false;
            }
            else
            {
                this.SpikeCount++;
                this.SpikeCount = this.SpikeCount - this.Rules[index].SpikeAmount;
                for (int i = 0; i < Connections.Count; i++)
                {
                    networkRef.Neurons[i].SpikeCount++;
                    networkRef.Neurons[i].SpikeCount = networkRef.Neurons[i].SpikeCount - networkRef.Neurons[i].Rules[index].SpikeAmount;
                    if (this.IsOutput == true)
                    {
                        return true;
                    }
                }
                if (this.IsOutput == true)
                {
                    return true;
                }
                Console.WriteLine("Rules matched, spiked");
                return false;
            }
        }
    }
}
