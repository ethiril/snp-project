﻿using System;
using SNP_First_Test.Network;
using SNP_Network = SNP_First_Test.Network.Network;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SNP_First_Test.Genetic_Algorithms;

namespace SNP_First_Test
{
    class SNP_Main
    {

        /* TODO
         * 4.) Check whether no rules apply any more.  
         * 5.) Change network creation to use JSON for now (easier import/export larger networks)
         * 6.) Move onto implementing a genetic algorithm solution
         * 
         */

        // https://stackoverflow.com/questions/2246694/how-to-convert-json-object-to-custom-c-sharp-object
        // test network (tutorial figure 1)

        //https://stackoverflow.com/questions/45245032/c-sharp-parse-one-json-field-to-an-object-with-its-custom-constructor
        //https://stackoverflow.com/questions/2246694/how-to-convert-json-object-to-custom-c-sharp-object
        static readonly int maxSteps = 100;
        static readonly int stepRepetition = 20;
        static int populationSize = 20;
        static float mutationRate = 0.01f;
        static int maximumGenerations = 100;
        // maximum size of each spike grouping in the random new gene
        static private int maxExpSize = 4;
        // set of numbers that I expect to see after the evolution
        //static private List<int> expectedSet = new List<int>() { 2, 4, 6, 8, 10, 12, 14, 16 };
        static private List<int> expectedSet = new List<int>() { 1, 2, 5, 6, 9, 10 };
        // How are they ranked? Just chosen at random? If the parents 
        static private List<string> acceptedRegex = new List<string>() {
            "x", // direct match 
            //"x+", // x followed by one or more
            //"x*", // x followed by zero or more
            //"x?", // x followed by zero or one 
            //"x(y)+", // x followed by one or more y groupings
            //"x(y)*", // x followed by zero or more y groupings
            //"x(y)?", // x followed by zero or one y groupings
        };
        static private int elitism = 2;
        private static GeneticAlgorithm ga;
        static private Random random = new Random((int)DateTime.Now.Ticks);

        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            Console.WriteLine("Let's get started. //");
            Console.WriteLine("Press enter to start the test.");
            Console.ReadLine();
            int count = 0;
            Console.WriteLine("The amount of cycles each network will go through: " + stepRepetition);
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
            Console.WriteLine("----------- Test Started -----------");
            stopWatch.Start();
            ga = new GeneticAlgorithm(populationSize, random, CreateNewRandomNetwork, GenerateRandomExpression, FitnessFunction, elitism);
            for (int i = 0; i < 10; i++ )
            {
                Console.WriteLine("First test generation {0}", i);
                ga.NewGeneration();
            }
            stopWatch.Stop();
            Console.WriteLine();
            Console.WriteLine("Press any key to exit, time elapsed: " + stopWatch.Elapsed.ToString() + "s");
            Console.ReadLine();
        }


        static List<int> SNPRun(SNP_Network network)
        {
            List<int> allOutputs = new List<int>();
            SNP_Network clone = GenerateIdenticalNetwork(network);
            clone.CurrentOutput = 0;
            for (int i = 0; i < stepRepetition; i++)
            {
                clone = GenerateIdenticalNetwork(network);
                while (clone.IsClear == false && clone.GlobalTimer < maxSteps)
                {
                    stepThrough(clone);
                }
                allOutputs.AddRange(clone.OutputSet);
                if (allOutputs.Count == 0 && (i > 5))
                {
                    //Console.WriteLine("No outputs provided");
                    break;
                }
            }
            //allOutputs = allOutputs.Distinct().ToList();
            allOutputs.Sort();
            return allOutputs;
        }

        static void stepThrough(SNP_Network network)
        {
            network.Spike(network);
        }
        // Create new network with the List of new rules.
        // Note that this is a NON-GENERIC way of implementing this method, as I know exactly how many neurons this network will have.

        // not implemented
        static string GenerateRandomExpression(List<string> templates, Random random)
        {
            int templateIndex = random.Next(0, templates.Count);
            string randomSpikeAmount = new string('a', random.Next(1, maxExpSize));
            string randomSpikeGroupingAmount = new string('a', random.Next(1, maxExpSize));
            string generatedExpression = templates[templateIndex].Replace("x", randomSpikeAmount);
            generatedExpression = generatedExpression.Replace("y", randomSpikeGroupingAmount);
            return generatedExpression;
        }


        void UpdateGA()
        {
            ga.NewGeneration();
            if (ga.BestFitness == 1)
            {
                Console.WriteLine("End");
            }
        }

        // Testing fitness without GA implementation
        private static float TestFitnessFunction(List<int> output)
        {
            float tp = 0, fp = 0;
            //DNA dna = ga.Population[index];
            List<int> tpFound = new List<int>();
            for (int i = 0; i < output.Count(); i++)
            {
                foreach (int value in expectedSet)
                {
                    if (expectedSet.Contains(output[i]))
                    {
                        tp++;
                        if (!tpFound.Contains(output[i]))
                        {
                            tpFound.Add(output[i]);
                        }
                    }
                    else
                    {
                        fp++;
                    }
                }
            }
            tp = (tp - expectedSet.Count) / (output.Count - expectedSet.Count);
            fp = (fp - expectedSet.Count) / (output.Count - expectedSet.Count);
            float scaledFitness = 0, targetCount = tpFound.Count, tpCount = expectedSet.Count;
            if (tpFound.Count != 0)
            {
                scaledFitness = targetCount / tpCount;
            }
            //tp = tp / expectedSet.Count;
            //fp = fp / expectedSet.Count;
            float fn = expectedSet.Except(output).Count();
            float sensitivity = (tp / (tp + fn));
            float precision = tp / (tp + fp);
            float fitness = ((2 * tp) / ((2 * tp) + fp + fn)) * scaledFitness;
            Console.WriteLine("Precision: ({0} / ({1} + {2}) = {3})", tp, tp, fn, precision);
            Console.WriteLine("Sensitivity: ({0} / ({1} + {2}) = {3})", tp, tp, fp,  sensitivity);
            Console.WriteLine("\nFitness: (((2 * {0}) / ((2 * {1}) + {2} + {3})) * {4}) = {5}", tp, tp, fp, fn, scaledFitness, fitness);
            return fitness;
        }

        private static float FitnessFunction(int index)
        {
            /* If a number is in output and in target then it is a true positive
             * If a number is in output but not in target then it is a false positive 
             * If a number is not in output but is in the target then it is a false negative
             * If a number is not in output and not in the target then it is a true negative
             * 
             * We will never deal in True Negative members as the numbers will always be in the target
             * 
             * Sensitivity = number of true positives / number of true positives + number of false negatives
             * 
             * Specificity = number of true negatives / number of true negatives + number false positives. 
             * 
             * Precision (Positive predictive value) = The number of true positives / number of true positives + false positives 
             * 
             * We will never be able to check for specificity but we can work out the sensitivity and use it for our fitness function.
             * A higher sensitivity is better, as it means the amount of false negatives is lower. 
             * We can check for sensitivity and precision.
             * 
             */
            float tp = 0, fp = 0;
            DNA dna = ga.Population[index];
            List<int> output = SNPRun(dna.Genes);
            //dna.Genes.print();
            List<int> tpFound = new List<int>();

            for (int i = 0; i < output.Count(); i++)
            {
                foreach (int value in expectedSet)
                {
                    if (expectedSet.Contains(output[i]))
                    {
                        tp++;
                        if (!tpFound.Contains(output[i]))
                        {
                            tpFound.Add(output[i]);
                        }
                    }
                    else
                    {
                        fp++;
                    }
                }
            }
            tp = (tp - expectedSet.Count) / (output.Count - expectedSet.Count);
            fp = (fp - expectedSet.Count) / (output.Count - expectedSet.Count);
            float scaledFitness = 0, targetCount = tpFound.Count, tpCount = expectedSet.Count;
            if (tpFound.Count != 0)
            {
                scaledFitness = targetCount / tpCount;
            }
            float fn = expectedSet.Except(output).Count();
            float sensitivity = (tp / (tp + fn));
            float precision = tp / (tp + fp);
            float fitness = ((2 * tp) / ((2 * tp) + fp + fn)) * scaledFitness;
            //Console.WriteLine("Fitness for this Gene: " + fitness);
            return fitness;
        }


        void Update()
        {
            ga.NewGeneration();
            if (ga.BestFitness == 1)
            {
                Console.WriteLine("Succeeded");
                //this.enabled = false;
            }
        }

        // generate a completely random network
        private static SNP_Network GenerateIdenticalNetwork(SNP_Network network)
        {
            int neuronAmount = network.Neurons.Count;
            int outputNeuron = 0;
            for (int i = 0; i < neuronAmount; i++)
            {
                if (network.Neurons[i].IsOutput)
                {
                    outputNeuron = i;
                }
            }
            List<Neuron> neurons = new List<Neuron>();
            for (int i = 0; i < neuronAmount; i++)
            {
                int ruleAmount = network.Neurons[i].Rules.Count;
                List<Rule> rules = new List<Rule>();
                for (int j = 0; j < ruleAmount; j++)
                {
                    rules.Add(new Rule(network.Neurons[i].Rules[j].RuleExpression, network.Neurons[i].Rules[j].Delay, network.Neurons[i].Rules[j].Fire));
                }
                List<int> connections = new List<int>();
                int connectionCount = network.Neurons[i].Connections.Count;
                for (int k = 0; k < connectionCount; k++)
                {
                    connections.Add(network.Neurons[i].Connections[k]);
                }
                neurons.Add(new Neuron(rules, network.Neurons[i].SpikeCount, connections, (i == outputNeuron)));
            }
            return new SNP_Network(neurons);
        }


        private static SNP_Network GenerateNewRandomNetwork(Random random)
        {
            int neuronAmount = random.Next(2, 8);
            int outputNeuron = random.Next(1, neuronAmount + 1);
            List<Neuron> neurons = new List<Neuron>();
            for (int i = 0; i < neuronAmount; i++)
            {
                int ruleAmount = random.Next(1, 4);
                List<Rule> rules = new List<Rule>();
                for (int j = 0; j < ruleAmount; j++)
                {
                    bool next = (random.Next(0, 2) == 1);
                    rules.Add(new Rule(GenerateRandomExpression(acceptedRegex, random), random.Next(0, 2), next));
                }
                List<int> connections = new List<int>();
                for (int k = 0; k < neuronAmount; k++)
                {
                    // add at least one connection
                    if (connections.Count == 0)
                    {
                        int randomFirstConnection = random.Next(1, neuronAmount + 1);
                        while (randomFirstConnection == (i + 1))
                        {
                            randomFirstConnection = random.Next(1, neuronAmount + 1);
                        }
                        connections.Add(randomFirstConnection);
                    }
                    else if (i != k && !(connections.Contains(k + 1)))
                    {
                        if (random.Next(0, 2) == 1)
                        {
                            connections.Add(k + 1);
                        }
                    }
                }
                connections.Sort();
                neurons.Add(new Neuron(rules, new string('a', random.Next(0, maxExpSize + 1)), connections, (i + 1 == outputNeuron)));
            }
            return new SNP_Network(neurons);
        }

        // Generate new random expressions for a network we know work
        private static SNP_Network CreateNewRandomNetwork()
        {
            return new SNP_Network(new List<Neuron>() {
                      new Neuron(new List<Rule>(){
                          new Rule(GenerateRandomExpression(acceptedRegex, random),0,true),
                          new Rule(GenerateRandomExpression(acceptedRegex, random),0,false)
                      }, "aa", new List<int>() {4}, false),
                       new Neuron(new List<Rule>() {
                          new Rule(GenerateRandomExpression(acceptedRegex, random),0,true),
                          new Rule(GenerateRandomExpression(acceptedRegex, random),0,false)
                      }, "aa", new List<int>() {5}, false),
                       new Neuron(new List<Rule>() {
                          new Rule(GenerateRandomExpression(acceptedRegex, random),0,true),
                          new Rule(GenerateRandomExpression(acceptedRegex, random),0,false)
                      }, "aa", new List<int>() {6}, false),
                        new Neuron(new List<Rule>() {
                          new Rule(GenerateRandomExpression(acceptedRegex, random),1,true),
                          new Rule(GenerateRandomExpression(acceptedRegex, random),0,true)
                      }, "", new List<int>() {1, 2, 3, 7}, false),
                         new Neuron(new List<Rule>() {
                          new Rule(GenerateRandomExpression(acceptedRegex, random),0,true),
                      }, "", new List<int>() {1, 2, 7}, false),
                         new Neuron(new List<Rule>() {
                          new Rule(GenerateRandomExpression(acceptedRegex, random),0,true),
                      }, "", new List<int>() { 3, 7}, false),
                         new Neuron(new List<Rule>() {
                          new Rule(GenerateRandomExpression(acceptedRegex, random),0,true),
                          new Rule(GenerateRandomExpression(acceptedRegex, random),0,false)
                      }, "aa", new List<int>() { }, true),
                  });
        }

        //Natural numbers network original 
        static SNP_Network CreateNewRandomXNetwork()
        {
            return new SNP_Network(new List<Neuron>() {
                   new Neuron(new List<Rule>(){
                    new Rule("aa",0,true),
                    new Rule("a",0,false)
                }, "aa", new List<int>() {2, 3, 4}, false),
                   new Neuron(new List<Rule>() {
                    new Rule("aa",0,true),
                    new Rule("a",1,false)
                }, "aa", new List<int>() {1,3,4}, false),
                   new Neuron(new List<Rule>() {
                    new Rule("aa",0,true),
                    new Rule("aa",1,true)
                }, "aa", new List<int>() {1,2,4}, false),
                   new Neuron(new List<Rule>() {
                    new Rule("aa",0,true),
                    new Rule("aaa",0,false)
                }, "aa",new List<int>() { }, true),
            });
        }

        /*  Natural Numbers fully auto generated
           return new SNP_Network(new List<Neuron>() {
                   new Neuron(new List<Rule>(){
                       new Rule(networkConfiguration[0]["Expression"], networkConfiguration[0]["Delay"], networkConfiguration[0]["Fire"]),
                       new Rule(networkConfiguration[1]["Expression"], networkConfiguration[1]["Delay"], networkConfiguration[1]["Fire"]),
                }, "aa", new List<int>() {2, 3, 4}, false),
                   new Neuron(new List<Rule>() {
                       new Rule(networkConfiguration[2]["Expression"], networkConfiguration[2]["Delay"], networkConfiguration[2]["Fire"]),
                       new Rule(networkConfiguration[3]["Expression"], networkConfiguration[3]["Delay"], networkConfiguration[3]["Fire"]),
                }, "aa", new List<int>() {1,3,4}, false),
                   new Neuron(new List<Rule>() {
                       new Rule(networkConfiguration[4]["Expression"], networkConfiguration[4]["Delay"], networkConfiguration[4]["Fire"]),
                       new Rule(networkConfiguration[5]["Expression"], networkConfiguration[5]["Delay"], networkConfiguration[5]["Fire"]),
                }, "aa", new List<int>() {1,2,4}, false),
                   new Neuron(new List<Rule>() {
                       new Rule(networkConfiguration[6]["Expression"], networkConfiguration[6]["Delay"], networkConfiguration[6]["Fire"]),
                       new Rule(networkConfiguration[7]["Expression"], networkConfiguration[7]["Delay"], networkConfiguration[7]["Fire"]),
                }, "aa",new List<int>() { }, true),
            }); */

        // Even numbers network original
        /*static SNP_Network CreateNewRandomNetwork()
        {
            return new SNP_Network(new List<Neuron>() {
                      new Neuron(new List<Rule>(){
                          new Rule("aa",0,true),
                          new Rule("a",0,false)
                      }, "aa", new List<int>() {4}, false),
                       new Neuron(new List<Rule>() {
                          new Rule("aa",0,true),
                          new Rule("a",0,false)
                      }, "aa", new List<int>() {5}, false),
                       new Neuron(new List<Rule>() {
                          new Rule("aa",0,true),
                          new Rule("a",0,false)
                      }, "aa", new List<int>() {6}, false),
                        new Neuron(new List<Rule>() {
                          new Rule("a",1,true),
                          new Rule("a",0,true)
                      }, "", new List<int>() {1, 2, 3, 7}, false),
                         new Neuron(new List<Rule>() {
                          new Rule("a",0,true),
                      }, "", new List<int>() {1, 2, 7}, false),
                         new Neuron(new List<Rule>() {
                          new Rule("a",0,true),
                      }, "", new List<int>() { 3, 7}, false),
                         new Neuron(new List<Rule>() {
                          new Rule("aa",0,true),
                          new Rule("aaa",0,false)
                      }, "aa", new List<int>() { }, true),
                  });
        }*/
    }
}