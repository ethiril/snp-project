using System;
using System.Collections;
using System.Collections.Generic;
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

        public Network(List<Neuron> neurons, List<int> outputSet, int currentOutput, bool networkClear)
        {
            Neurons = neurons;
            OutputSet = outputSet;
            CurrentOutput = currentOutput;
            NetworkClear = networkClear;
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
           // Console.WriteLine("Before Spikes: ");
            //Console.WriteLine("---- TESTING NEURON ----");
            //PrintNetwork(this.Neurons);
            Parallel.ForEach(NeuronCopy, neuron =>
            {
                if (neuron.RemoveSpikes(networkRef, neuron.Connections) == true)
                {
                    if (neuron.IsOutput == true)
                    {
                        this.OutputSet.Add(++this.CurrentOutput);
                        this.CurrentOutput = 0;
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
                 neuron.FireSpike(networkRef, neuron.Connections);
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
                Console.Write("Adding Spikes. Neuron " + count + ", Amount of spikes: " + neuron.SpikeCount + ", the rules: ");
                foreach (Rule rule in neuron.Rules) { Console.Write(rule.RuleExpression + ", "); };
                Console.WriteLine("");
            }
        }
    }
}
