﻿using System;
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
    public class Network
    {
        public List<Neuron> Neurons { get; set; }
        public List<int> OutputSet { get; set; }
        private int CurrentOutput { get; set; }
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
             */
            Console.WriteLine("After spike removal: ");
            List<Neuron> NeuronCopy = new List<Neuron>(this.Neurons);
            object sync = new object();
            Parallel.ForEach(NeuronCopy, neuron =>
            {
                if (neuron.RemoveSpikes(networkRef, neuron.Connections) == true)
                {
                    Console.WriteLine("REMOVING SPIKE.");
                    if (neuron.IsOutput == true)
                    {
                        Console.Error.WriteLine("REMOVING OUTPUT SPIKE.");
                        this.OutputSet.Add(++this.CurrentOutput);
                        this.CurrentOutput = 0;
                    }
                }
                else
                {
                    Console.WriteLine("ADDING SPIKE.");
                    if (neuron.IsOutput == true)
                    {
                        this.CurrentOutput++;
                    }
                }
            });
            PrintNetwork(this.Neurons);
            Console.WriteLine("After spike addition: ");
            NeuronCopy = new List<Neuron>(this.Neurons);
            Parallel.ForEach(NeuronCopy, neuron =>
            {
                neuron.FireSpike(networkRef, neuron.Connections);
            });
            PrintNetwork(this.Neurons);
            this.GlobalTimer++;
            Console.Write("Global Timer: " + this.GlobalTimer + ", The current output set is: ");
            this.OutputSet.ForEach(i => Console.Write("{0}\t", i));

        }
        private void PrintNetwork(List<Neuron> neurons)
        {
            int count = 0;
            foreach (Neuron neuron in this.Neurons)
            {
                count++;
                Console.WriteLine("Adding Spikes. Neuron " + count + ", Amount of spikes: " + neuron.SpikeCount.Length + ", the rules: ");
                //foreach (Rule rule in neuron.Rules) { Console.WriteLine(rule.RuleExpression); }; 
            }
        }
    }
}
