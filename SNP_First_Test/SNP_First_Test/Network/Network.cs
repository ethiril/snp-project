using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SNP_First_Test.Network
{
    /* 
     *   A network is made up of multiple neurons, this can be edited to contain as many or as little
     *   neurons as you would like. 
     */
    [Serializable()]
    public class Network
    {
        public List<Neuron> Neurons { get; set; }
        public List<int> OutputSet { get; set; }
        public int CurrentOutput { get; set; }
        public bool NetworkClear { get; set; }
        public int GlobalTimer { get; set; }
        public bool NetworkEngaged { get; set; }
        public Network(List<Neuron> neurons, List<int> outputSet, bool networkClear)
        {
            Neurons = neurons;
            OutputSet = outputSet;
            CurrentOutput = 0;
            NetworkClear = networkClear;
            NetworkEngaged = false;
        }

        public void Spike(Network networkRef)
        {
            /*
             * Run through removing all of the spikes and recording an output
             * then run through adding all of the spikes that are recorded to have a spike go out to their network
             * Check if each spike sends spike across all axons or if it determines which to go through randomly.
             * Add needs to be done on a copy of the network BEFORE the removal happens (original network)
             */
            List<Neuron> NeuronCopy = new List<Neuron>(this.Neurons);
            List<Neuron> NeuronAdditionCopy = this.Neurons.DeepClone();
            Console.WriteLine("Before Spikes: ");
            Console.WriteLine("---- TESTING NEURON ----");
            PrintNetwork(this.Neurons);
            Console.WriteLine("Is network engaged?: " + this.NetworkEngaged);
            // this doesn't really work.
            Parallel.ForEach(NeuronCopy, neuron =>
            {
                if (neuron.RemoveSpikes(networkRef, neuron.Connections) == true)
                {
                    if (neuron.IsOutput == true && this.NetworkEngaged == true)
                    {
                        this.OutputSet.Add(++this.CurrentOutput);
                        this.NetworkClear = true;
                    }
                    else if (neuron.IsOutput == true && this.NetworkEngaged == false)
                    {
                        Console.WriteLine("Setting the networkEngaged to true.");
                        this.NetworkEngaged = true;
                    }
                }
                else
                {
                    if (neuron.IsOutput == true)
                    {
                        this.CurrentOutput++;
                    }
                }
            });
            Console.WriteLine("After Spike Removal: ");
            Console.WriteLine("---- TESTING NEURON ----");
            PrintNetwork(this.Neurons);
            Console.WriteLine("Neuron addition copy should be the initial config");
            PrintNetwork(NeuronAdditionCopy);
            Parallel.ForEach(NeuronAdditionCopy, neuron =>
             {
                 if (neuron.ActiveDelay == 0)
                 {
                     neuron.FireSpike(networkRef, neuron.Connections);
                 }
             });
            Console.WriteLine("After spike addition: ");
            Console.WriteLine("---- TESTING NEURON ----");
            PrintNetwork(this.Neurons);
            Console.WriteLine("Copy network");
            PrintNetwork(NeuronAdditionCopy);
            this.GlobalTimer++;
            Console.Write("Global Timer: " + this.GlobalTimer + ", The current output set is: ");
            this.OutputSet.ForEach(i => Console.Write("{0}\t", i));

        }
        private void PrintNetwork(List<Neuron> neurons)
        {
            int count = 0;
            foreach (Neuron neuron in neurons)
            {
                count++;
                Console.Write("Adding Spikes. Neuron " + count + ", Amount of spikes: " + neuron.SpikeCount + ", the rules: ");
                foreach (Rule rule in neuron.Rules) { Console.Write(rule.RuleExpression + "->" + rule.Fire + ";" + rule.Delay + ", "); };
                Console.WriteLine("");
            }
        }

        public void print()
        {
            int count = 0;
            Console.WriteLine("Network printout: ");
            Console.WriteLine("Neuron amount: " + this.Neurons.Count);
            Console.WriteLine("Network breakdown");
            foreach (Neuron neuron in this.Neurons)
            {
                count++;
                Console.WriteLine("Neuron: " + count);
                Console.WriteLine("Current spikes: " + neuron.SpikeCount);
                Console.WriteLine("Rule amount: " + neuron.Rules.Count);
                Console.Write("Current Rules: ");
                foreach (Rule rule in neuron.Rules) { Console.Write(rule.RuleExpression + "->" + rule.Fire + ";" + rule.Delay + ", "); };
                Console.WriteLine();
                Console.Write("Neuron connections: ");
                foreach (int connection in neuron.Connections) { Console.Write(connection + ", "); };
                Console.WriteLine("\n");

            }
        }

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
