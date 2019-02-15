﻿using System;
using System.Collections.Generic;
using System.Text;
using SNP_First_Test.Network;
using SNP_Network = SNP_First_Test.Network.Network;


namespace SNP_First_Test.Genetic_Algorithms
{
    public class DNA
    {
        // any arbitrary type of gene
        public SNP_Network Genes { get; set; }
        public float Fitness { get; set; }
        private Random random;
        private Func<SNP_Network> getRandomNetwork;
        private Func<List<string>, Random, string> getModifiedRule;
        private Func<int, float> fitnessFunction;
        static private List<string> acceptedRegex = new List<string>() {
            "x", // direct match 
            //"x+", // x followed by one or more
            //"x*", // x followed by zero or more
            //"x?", // x followed by zero or one 
            //"x(y)+", // x followed by one or more y groupings
            //"x(y)*", // x followed by zero or more y groupings
            //"x(y)?", // x followed by zero or one y groupings
        };

        // Gene is of type SNP_Network
        public DNA (Random random, Func<SNP_Network> getRandomNetwork, Func<List<string>, Random, string> getModifiedRule, Func<int, float> fitnessFunction, bool init = true)
        {

            this.random = random;
            this.getRandomNetwork = getRandomNetwork;
            this.getModifiedRule = getModifiedRule;
            this.fitnessFunction = fitnessFunction;
            if (init)
            {
                // start a new random Network
                    Genes = getRandomNetwork();
            }
        }

        public float CalculateFitness(int index)
        {
            // calculate the fitness for the current gene
            Fitness = fitnessFunction(index);
            return Fitness;
        } 
        
    
        
        public DNA Crossover(DNA secondParent)
        {
            DNA child = new DNA(random, getRandomNetwork, getModifiedRule, fitnessFunction, init: false);

            // for now crossover with always the same amount of genes and rules 
            int neuronAmount = Genes.Neurons.Count;
            int outputNeuron = 0;
            for (int i = 0; i < neuronAmount; i++)
            {
                if (Genes.Neurons[i].IsOutput)
                {
                    outputNeuron = i;
                }
            }
            List<Neuron> neurons = new List<Neuron>();
            for (int i = 0; i < neuronAmount; i++)
            {
                int ruleAmount = Genes.Neurons[i].Rules.Count;
                List<Rule> rules = new List<Rule>();
                for (int j = 0; j < ruleAmount; j++)
                {
                    // since for this test the delays and Fire rules will remain the same through every network, we only need to generate new rule expressions.
                    string nextExp = random.NextDouble() < 0.5 ? Genes.Neurons[i].Rules[j].RuleExpression : secondParent.Genes.Neurons[i].Rules[j].RuleExpression;
                    rules.Add(new Rule(nextExp, Genes.Neurons[i].Rules[j].Delay, Genes.Neurons[i].Rules[j].Fire));
                }
                List<int> connections = new List<int>();
                int connectionCount = Genes.Neurons[i].Connections.Count;
                // we want to repeat the connections for all of the networks, these aren't changing for now.
                for (int k = 0; k < connectionCount; k++)
                {
                    connections.Add(Genes.Neurons[i].Connections[k]);
                }
                neurons.Add(new Neuron(rules, Genes.Neurons[i].SpikeCount, connections, (i == outputNeuron)));
            }
            child.Genes = new SNP_Network(neurons);
            return child;
        }



        public void Mutate(float mutationRate)
        {
                if (random.NextDouble() < mutationRate)
                {
                // Change one of the rules at random
                for (int i = 0; i < Genes.Neurons.Count; i++)
                {
                    for (int j = 0; i < Genes.Neurons[i].Rules.Count; j++)
                    {
                        Genes.Neurons[i].Rules[j] = new Rule(getModifiedRule(acceptedRegex, random), Genes.Neurons[i].Rules[j].Delay, Genes.Neurons[i].Rules[j].Fire);
                    }
                }
                }
        }

    }
}
