using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SNP_First_Test.Network;
using SNP_Network = SNP_First_Test.Network.Network;

namespace SNP_First_Test
{
    /* 
     *   A neuron is made up of multiple rules and a count of spikes which are currently being held
     *   by the neuron.
     */
    [Serializable()]
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
            int matchedIndex = 0;
            int count = 0;
            //Console.WriteLine("This.rules.count = " + this.Rules.Count);
            foreach (Rule rule in this.Rules)
            {
                if (rule.IsMatched(this.SpikeCount).Equals(null) || rule.IsMatched(this.SpikeCount).Equals(true))
                {
                    matchedCount++;
                    matchedIndex = count;
                }
                ++count;
            }
            if (matchedCount > 1)
            {
                Console.WriteLine("More than one kenobi");
                return matchedCount;
            }
            else
            {
                return matchedIndex;
            }
        }


        // This code will loop over the entire network and remove any spikes which match the correct rules.
        public bool? RemoveSpikes(SNP_Network networkRef, List<int> Connections)
        {
            int index;
            int matchedCount = MatchRules();
            if (matchedCount > 1)
            {
                index = DetermineIndex(matchedCount);
                //Console.WriteLine("Random.next(0," + matchedCount + ")");
                //Console.WriteLine("The index is:" + index);
                if (this.Rules[index].IsMatched(this.SpikeCount).Equals(null))
                {
                    // this state needs storing somehow, as the spike needs to realise that it was just nulled and should not then spike. 
                    Console.WriteLine("NONDETERMINISTIC - Rule " + this.Rules[index].RuleExpression + " returned null, wiping spikes anyway");
                    this.SpikeCount = "";
                    return null;
                }
                else if (this.Rules[index].IsMatched(this.SpikeCount).Equals(false))
                {
                    Console.WriteLine("No rules matched, returning false");
                    return false;
                }
                else if (this.Rules[index].IsMatched(this.SpikeCount).Equals(true))
                {
                    Console.WriteLine("Delay is: " + this.Rules[index].Delay);
                    if (this.Rules[index].Delay > 0)
                    {
                        Console.WriteLine("Maintaining delay");
                        this.Rules[index].Delay--;
                        Console.WriteLine("Delay is: " + this.Rules[index].Delay + ", returning false");
                        return false;
                    }
                    else if (this.Rules[index].Delay == 0)
                    {
                        this.Rules[index].Delay = this.Rules[index].DelayAmount;
                        Console.WriteLine("Delay is: " + this.Rules[index].Delay + ", wiping spikes and returning true");
                    }
                    this.SpikeCount = "";
                    return true;
                }
            }
            else
            {
                index = matchedCount;
                //Console.WriteLine("The index is:" + index);
                if (this.Rules[index].IsMatched(this.SpikeCount).Equals(null))
                {
                    // this state needs storing somehow, as the spike needs to realise that it was just nulled and should not then spike. 
                    Console.WriteLine("DETERMINISTIC - Rule " + this.Rules[index].RuleExpression + " returned null, wiping spikes anyway");
                    this.SpikeCount = "";
                    return null;
                }
                else if (this.Rules[index].IsMatched(this.SpikeCount).Equals(false))
                {
                    Console.WriteLine("Delay is: " + this.Rules[index].Delay);
                    if (this.Rules[index].Delay > 0)
                    {
                        Console.WriteLine("Maintaining delay");
                        this.Rules[index].Delay--;
                        return false;
                    }
                    else if (this.Rules[index].Delay == 0)
                    {
                        Console.WriteLine("Rule: " + this.Rules[index].RuleExpression + ", has a current delay of 0 and a rule expression delay of " + this.Rules[index].DelayAmount + ".");
                        this.Rules[index].Delay = this.Rules[index].DelayAmount;
                    }
                    return false;
                }
                else if (this.Rules[index].IsMatched(this.SpikeCount).Equals(true))
                {
                    Console.WriteLine("Delay is: " + this.Rules[index].Delay);
                    if (this.Rules[index].Delay > 0)
                    {
                        Console.WriteLine("Maintaining delay");
                        this.Rules[index].Delay--;
                        return false;
                    }
                    else if (this.Rules[index].Delay == 0)
                    {
                        Console.WriteLine("Rule: " + this.Rules[index].RuleExpression + ", has a current delay of 0 and a rule expression delay of " + this.Rules[index].DelayAmount + ".");
                        this.Rules[index].Delay = this.Rules[index].DelayAmount;
                    }
                    this.SpikeCount = "";
                    return true;
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
                //Console.WriteLine("Random.next(0," + matchedCount + ")");
                foreach (int connection in Connections)
                {
                    if (this.Rules[index].IsMatched(this.SpikeCount).Equals(true))
                    {
                        Console.WriteLine("Delay is: " + this.Rules[index].Delay);
                        if (this.Rules[index].Delay > 0)
                        {
                            Console.WriteLine("Maintaining delay");
                        }
                        else if (this.Rules[index].Fire)
                        {
                            networkRef.Neurons[connection - 1].SpikeCount = networkRef.Neurons[connection - 1].SpikeCount + "a";
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
                        if (rule.IsMatched(this.SpikeCount).Equals(true))
                        {
                            Console.WriteLine("Delay is: " + this.Rules[index].Delay);
                            if (this.Rules[index].Delay > 0)
                            {
                                Console.WriteLine("Maintaining delay");
                            }
                            else if (rule.Fire)
                            {
                                networkRef.Neurons[connection - 1].SpikeCount = networkRef.Neurons[connection - 1].SpikeCount + "a";
                            }
                        }
                    }
                }
            }
        }

        // https://stackoverflow.com/questions/129389/how-do-you-do-a-deep-copy-of-an-object-in-net-c-specifically/1213649#1213649
        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
