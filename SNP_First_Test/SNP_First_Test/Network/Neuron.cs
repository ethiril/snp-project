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


        int indexOfSpike()
        {
            // if there is just one rule that is fullfilled then the random will always just 0, hence no need to check.
            // otherwise if more rules are matched it will be chosen at random

            Random random = new Random();
            return random.Next(0, MatchRules());
        }

        int MatchRules()
        {
            int matchedCount = 0;
            foreach (Rule rule in this.Rules)
            {
                // if the code for isMatched returns true (Rule has been fullfilled)
                // this whole piece of logic can be changed to not use nulls, the ones in the rules class
                // aren't really used. Only this method output needs them.
                // Neuron 7 returns 0 matched only sometimes?
                if ((rule.isMatched(this.SpikeCount) == null) || (rule.isMatched(this.SpikeCount) == true))
                {
                    matchedCount++;
                }
            }
            return matchedCount;

        }


        // This code will loop over the entire network and remove any spikes which match the correct rules.
        public bool? RemoveSpikes(SNP_Network networkRef, List<int> Connections)
        {
            int index = indexOfSpike();
            if (this.Rules[index].isMatched(this.SpikeCount) == null)
            {
                // this state needs storing somehow, as the spike needs to realise that it was just nulled and should not then spike. 
                //Console.WriteLine("This spike is null. This.SpikeCount = " + this.SpikeCount + ", this.Rules.SpikeAmount = " + this.Rules[index].SpikeAmount);
                this.SpikeCount = this.SpikeCount - this.Rules[index].SpikeAmount;
                return null;
            }
            else if (this.Rules[index].isMatched(this.SpikeCount) == false)
            {
                return false;
            }
            else
            {
                this.SpikeCount = this.SpikeCount - this.Rules[index].SpikeAmount;
                //Console.WriteLine(this.Rules[index].SpikeAmount + " spikes have been removed from the current count. " + this.SpikeCount + " spikes left within this neuron.");
                return true;
            }
        }


        public void FireSpike(SNP_Network networkRef, List<int> Connections)
        {
            /* 
             * If List has more than one rule
             * Check if the neuron has enough spike to satisfy some or all of the rules
             * If only one rule can proceed, complete that spike
             * If more than one rule can proceed at any one time use the Random() function to determine which will fire
             * Fire the spike on that chosen rule definition
             * We do not need to worry about the removal of spikes as that is done in RemoveSpikes()
             */
            int index = indexOfSpike();
            int matchedCount = MatchRules();
            //Console.WriteLine("Matched Count:" + matchedCount + ", Index chosen: " + index);
            //Console.WriteLine("State: " + this.Rules[index].isMatched(this.SpikeCount));
            // just stagger this, do removal then addition. 
            // Not sure how to do this in parallel, as all removals would have to be done independantly...
            // Could split the two functions and simply make it a two-part func that starts from Network?
            // I.e. Network -> Neuron.Fire() -> Neuron.Resolve() -> Output...
            if (this.IsOutput == true)
            {
                this.SpikeCount++;
            }
            foreach (int connection in Connections)
            {
                if (this.Rules[index].Fire == true)
                {
                  //  Console.WriteLine("Sending spike to Neuron " + connection + ", current rule has a delay of: " + this.Rules[index].Delay);
                    networkRef.Neurons[connection - 1].SpikeCount++;
                }
                else
                {
                  //  Console.WriteLine("Wiping spike from system on connection " + connection + ", current rule has a delay of: " + this.Rules[index].Delay);
                }
            }
          //  Console.WriteLine("Rules matched, spiked");
        }
        // Temp code for sending a spike across to another neuron
        // http://bezensek.com/blog/2015/04/12/non-deterministic-finite-state-machine-implementation-in-c-number/
        // Will need the above for a non-deterministic approach to this implementation
        // This method should also be rewritten to be synchronous
    }
}
