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
        public string SpikeCount { get; set; }
        // List of connections for this neuron
        public List<int> Connections { get; set; }

        public bool IsOutput { get; set; }

        // Constructor
        public Neuron(List<Rule> rules, string spikeCount, List<int> connections, bool isOutput)
        {
            Rules = rules;
            SpikeCount = spikeCount;
            Connections = connections;
            IsOutput = isOutput;
        }


        int DetermineIndex(int count)
        {
            // if there is just one rule that is fullfilled then the random will always just 0, hence no need to check.
            // otherwise if more rules are matched it will be chosen at random

            Random random = new Random();
            return random.Next(0, count);
        }

        int MatchRules()
        {
            int matchedCount = 0;
            foreach (Rule rule in this.Rules)
            {
                if ((rule.IsMatched(this.SpikeCount) == null) || (rule.IsMatched(this.SpikeCount) == true))
                {
                    matchedCount++;
                }
            }
            return matchedCount;
        }


        // This code will loop over the entire network and remove any spikes which match the correct rules.
        public bool? RemoveSpikes(SNP_Network networkRef, List<int> Connections)
        {
            //int index = indexOfSpike();
            int index = 0;
            int matchedCount = MatchRules();
            if (matchedCount > 1)
            {
                index = DetermineIndex(matchedCount);
                foreach (Rule rule in this.Rules)
                {
                    if (this.Rules[index].IsMatched(this.SpikeCount) == null)
                    {
                        // this state needs storing somehow, as the spike needs to realise that it was just nulled and should not then spike. 
                        this.SpikeCount = "";
                        return null;
                    }
                    else if (this.Rules[index].IsMatched(this.SpikeCount) == false)
                    {
                        return false;
                    }
                    else
                    {
                        this.SpikeCount = "";
                        return true;
                    }
                }
            }
            else
            {
                //Console.Error.WriteLine("DETERMINISTIC PATH");
                foreach (Rule rule in this.Rules)
                {
                    if (rule.IsMatched(this.SpikeCount) == null)
                    {
                        this.SpikeCount = "";
                        return null;
                    }
                    else if (rule.IsMatched(this.SpikeCount) == false)
                    {
                        return false;
                    }
                    else
                    {
                        this.SpikeCount = "";
                        return true;
                    }
                }
            }
            // this should never happen.
            Console.Error.WriteLine("Foreach loop failed.");
            return false;
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
            int index = 0;
            int matchedCount = MatchRules();
            if (matchedCount > 1)
            {
                index = DetermineIndex(matchedCount);
                foreach (int connection in Connections)
                {
                    foreach (Rule rule in this.Rules)
                    {
                        if (this.Rules[index].Fire == true)
                        {
                            networkRef.Neurons[connection - 1].SpikeCount = networkRef.Neurons[connection - 1].SpikeCount + "a";
                        }
                        else
                        {
                            Console.WriteLine("Wiping spike from system on connection " + connection + ", current rule has a delay of: " + this.Rules[index].Delay);
                        }
                    }
                }
            }
            else
            {
                foreach (int connection in Connections)
                {
                    foreach (Rule rule in this.Rules)
                    {
                        if (rule.Fire == true)
                        {
                            networkRef.Neurons[connection - 1].SpikeCount = networkRef.Neurons[connection - 1].SpikeCount + "a";
                        }
                        else
                        {
                            Console.WriteLine("Wiping spike from system on connection " + connection + ", current rule has a delay of: " + rule.Delay);
                        }
                    }
                }
            }
            // just stagger this, do removal then addition. 
            // Not sure how to do this in parallel, as all removals would have to be done independantly...
            // Could split the two functions and simply make it a two-part func that starts from Network?
            // I.e. Network -> Neuron.Fire() -> Neuron.Resolve() -> Output...
        }
        // Temp code for sending a spike across to another neuron
        // http://bezensek.com/blog/2015/04/12/non-deterministic-finite-state-machine-implementation-in-c-number/
        // Will need the above for a non-deterministic approach to this implementation
        // This method should also be rewritten to be synchronous
    }
}
