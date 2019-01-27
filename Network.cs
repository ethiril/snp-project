using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SNP_First_Test.Utilities;

namespace SNP_First_Test.Network
{
    /*
     *   A network is made up of multiple neurons, this can be edited to contain as many or as little
     *   neurons as you would like.
     */
    public class Network
    {
        public List<Neuron> Neurons;
        public List<int> OutputSet;
        public int CurrentOutput;
        public bool IsClear;
        public int GlobalTimer;
        public bool IsEngaged;

        public Network(List<Neuron> neurons)
        {
            Neurons = neurons;
            OutputSet = new List<int>() { };
            CurrentOutput = 0;
            IsClear = false;
            IsEngaged = false;
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
            List<Neuron> NeuronAdditionCopy = ReflectionCloner.DeepFieldClone(this.Neurons);
            //Console.WriteLine("Before Spikes: ");
            //Console.WriteLine("---- TESTING NEURON ----");
            //PrintNetwork(this.Neurons);
            //Console.WriteLine("Is network engaged?: " + this.NetworkEngaged);
            Parallel.ForEach(NeuronCopy, neuron =>
            {
                if (neuron.RemoveSpikes(networkRef, neuron.Connections) == true)
                {
                    if (neuron.IsOutput == true && this.IsEngaged == true)
                    {
                        this.OutputSet.Add(++this.CurrentOutput);
                        this.IsClear = true;
                    }
                    else if (neuron.IsOutput == true && this.IsEngaged == false)
                    {
                        //Console.WriteLine("Setting the networkEngaged to true.");
                        this.IsEngaged = true;
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
            //Console.WriteLine("After Spike Removal: ");
            //Console.WriteLine("---- TESTING NEURON ----");
            //PrintNetwork(this.Neurons);
            //Console.WriteLine("Neuron addition copy should be the initial config");
            //PrintNetwork(NeuronAdditionCopy);
            Parallel.ForEach(NeuronAdditionCopy, neuron =>
            {
                if (neuron.ActiveDelay == 0)
                {
                    neuron.FireSpike(networkRef, neuron.Connections);
                }
            });
            //Console.WriteLine("After spike addition: ");
            //Console.WriteLine("---- TESTING NEURON ----");
            //PrintNetwork(this.Neurons);
            //Console.WriteLine("Copy network");
            //PrintNetwork(NeuronAdditionCopy);
            this.GlobalTimer++;
            //Console.Write("Global Timer: " + this.GlobalTimer + ", The current output set is: ");
            //this.OutputSet.ForEach(i => Console.Write("{0}\t", i));

        }
        private void PrintNetwork(List<Neuron> neurons)
        {
            int count = 0;
            foreach (Neuron neuron in neurons)
            {
                count++;
                //Console.Write("Adding Spikes. Neuron " + count + ", Amount of spikes: " + neuron.SpikeCount + ", the rules: ");
                foreach (Rule rule in neuron.Rules) { Console.Write(rule.RuleExpression + "->" + rule.Fire + ";" + rule.Delay + ", "); };
                //Console.WriteLine("");
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

        // Reflection cloner thanks to http://blog.nuclex-games.com/mono-dotnet/fast-deep-cloning/


        /*
        public static List<Neuron> Clone(List<Neuron> originalList)
        {
            List<Neuron> newList = originalList.ToList();
            return newList;
        }*/



        // could do this with protobuf https://theburningmonk.com/2011/08/performance-test-binaryformatter-vs-protobuf-net/ !!!!!!!
        /* public static T DeepClone<T>(T obj)
         {
             using (var ms = new MemoryStream())
             {
                 var formatter = new BinaryFormatter();
                 formatter.Serialize(ms, obj);
                 ms.Position = 0;

                 return (T)formatter.Deserialize(ms);
             }
         } */

    }
}
