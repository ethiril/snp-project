using System;
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
        static readonly int stepRepetition = 500;
        static int populationSize = 200;
        static float mutationRate = 0.01f;
        static int maximumGenerations = 1000;
        // maximum size of each spike grouping in the random new gene
        static private int maxExpSize = 4;
        // set of numbers that I expect to see after the evolution
        static private List<int> expectedSet = new List<int>() { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20 };
        // How are they ranked? Just chosen at random? If the parents 
        static private List<string> acceptedRegex = new List<string>() {
            "x", // direct match 
            "x+", // x followed by one or moren
            "x*", // x followed by zero or more
            "x?", // x followed by zero or one 
            "x(y)+", // x followed by one or more y groupings
            "x(y)*", // x followed by zero or more y groupings
            "x(y)?", // x followed by zero or one y groupings
        };

        private Genetic_Algorithms.GeneticAlgorithm<int> ga;
        static private Random random = new Random((int)DateTime.Now.Ticks);

        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            Console.WriteLine("Let's get started. //");
            Console.WriteLine("Press enter to start the test.");
            Console.ReadLine();
            Console.WriteLine("Initial state of the network: ");
            int count = 0;
            SNP_Network generatedNetwork = CreateNewRandomNetwork(random);
            for (int j = 0; j < 10; j++)
            {
                generatedNetwork = CreateNewRandomNetwork(random);
                generatedNetwork.print();
            }
            foreach (Neuron neuron in generatedNetwork.Neurons)
            {
                count++;
                Console.WriteLine("Neuron " + count + ", Amount of spikes: " + neuron.SpikeCount);
            }
            Console.WriteLine("The amount of cycles each network will go through: " + stepRepetition);
            generatedNetwork.print();
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
            Console.WriteLine("----------- Test Started -----------");
            // instantiate the GA for a new SN P Network with the rules as targets
            // ga = new Genetic_Algorithms.GeneticAlgorithm<SNP_Network>(populationSize, targetString.Length, random, );
            // clone the initial network setup for a network reset
            // then loop across, resetting the network after every output
            // test whether the random method is really giving us good random outputs.
            // when a number hits, the next x amounts are also the same????.
            stopWatch.Start();
            for (int i = 0; i <= maximumGenerations; i++)
            {
                List<int> currentOutputs = SNPRun(generatedNetwork, random);
                if (currentOutputs.Count != 0)
                {
                    Console.WriteLine("Final output set: ");
                    currentOutputs.ForEach(x => Console.Write("{0}\t", x));
                    Console.WriteLine("");
                }
                generatedNetwork = GenerateNewRandomNetwork(random);
            }
            stopWatch.Stop();
            Console.WriteLine();
            Console.WriteLine("Press any key to exit, time elapsed: " + stopWatch.Elapsed.ToString() + "s");
            Console.ReadLine();

        }


        static List<int> SNPRun(SNP_Network network, Random random)
        {
            int loopCounter = 0;
            List<int> allOutputs = new List<int>();
            network.CurrentOutput = 0;
            for (int i = 0; i < stepRepetition; i++)
            {
                while (network.IsClear == false && network.GlobalTimer < maxSteps)
                {
                    stepThrough(loopCounter++, network);
                }
                allOutputs.AddRange(network.OutputSet);
                loopCounter = 0;
            }
            allOutputs = allOutputs.Distinct().ToList();
            allOutputs.Sort();
            return allOutputs;
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

        // not implemented
        static Dictionary<int, Dictionary<string, dynamic>> CreateRandomConfiguration()
        {

            return null;
        }


        private float FitnessFunction(int index)
        {
            float score = 0;
            DNA<int> dna = ga.Population[index];

            for (int i = 0; i < ga.Population[index].Genes.Length; i++)
            {

            }
            return 1;
        }


        void Update()
        {
            ga.NewGeneration();

            //UpdateText(ga.BestGenes, ga.BestFitness, ga.Generation, ga.Population.Count, (j) => ga.Population[j].Genes);

            if (ga.BestFitness == 1)
            {
                Console.WriteLine("Succeeded");
                //this.enabled = false;
            }
        }
        /*
	    private char GetRandomCharacter()
	    {
		    int i = random.Next(validCharacters.Length);
		    return validCharacters[i];
	    }

	    private float FitnessFunction(int index)
	    {
		    float score = 0;
		    DNA<char> dna = ga.Population[index];

		    for (int i = 0; i < dna.Genes.Length; i++)
		    {
			    if (dna.Genes[i] == targetString[i])
			    {
				    score += 1;
			    }
		    }

		    score /= targetString.Length;

		    score = (Mathf.Pow(2, score) - 1) / (2 - 1);

		    return score;
	    } */


        // generate a completely random network
        static SNP_Network GenerateNewRandomNetwork(Random random)
        {
            int neuronAmount = random.Next(2, 8);
            int outputNeuron = random.Next(1, neuronAmount + 1);
            List<Neuron> neuronList = new List<Neuron>();
            for (int i = 0; i < neuronAmount; i++)
            {
                int ruleAmount = random.Next(1, 4);
                List<Rule> ruleList = new List<Rule>();
                for (int j = 0; j < ruleAmount; j++)
                {
                    bool next = (random.Next(0, 2) == 1);
                    ruleList.Add(new Rule(GenerateRandomExpression(acceptedRegex, random), random.Next(0, 2), next));
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
                    else if (i != k && !(connections.Contains(k+1)))
                    {
                        if (random.Next(0, 2) == 1)
                        {
                            connections.Add(k + 1);
                        }
                    }
                }
                connections.Sort();
                if (i+1 == outputNeuron)
                {
                    neuronList.Add(new Neuron(ruleList, new string('a', random.Next(0, maxExpSize + 1)), connections, true));
                }
                else
                {
                    neuronList.Add(new Neuron(ruleList, new string('a', random.Next(0, maxExpSize + 1)), connections, false));
                }
            }
            return new SNP_Network(neuronList);
        }

        // Generate new random expressions for a network we know works

        static SNP_Network CreateNewRandomNetwork(Random random)
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


        /* Natural numbers network original 
                static SNP_Network CreateNewNetwork(Dictionary<int, Dictionary<string, dynamic>> networkConfiguration)
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
 */


        /* Even numbers network original
         * static SNP_Network CreateNewRandomNetwork(Random random)
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
          } */

        static void stepThrough(int count, SNP_Network network)
        {
            network.Spike(network);
        }
    }
}