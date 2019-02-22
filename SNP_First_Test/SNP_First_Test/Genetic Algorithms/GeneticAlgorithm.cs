using System;
using System.Collections.Generic;
using SNP_First_Test.Network;
using SNP_First_Test.Utilities;
using SNP_Network = SNP_First_Test.Network.Network;

namespace SNP_First_Test.Genetic_Algorithms
{
    public class GeneticAlgorithm
    {
        public List<DNA> Population { get; private set; }
        public int Generation { get; private set; }
        public float BestFitness { get; private set; }
        public List<float> FitnessList = new List<float>();
        public SNP_Network BestGenes { get; private set; }

        public int Elitism;
        public float MutationRate;

        private List<DNA> newPopulation;
        private int PopulationSize;
        private Random random;
        private float fitnessSum;
        private int erroneousNetworks;
        private Func<SNP_Network> getRandomNetwork;
        private List<string> acceptedRegex;
        private Func<List<string>, Random, string> getModifiedRule;
        private Func<int, float> fitnessFunction;

        public GeneticAlgorithm(int populationSize, Random random, Func<SNP_Network> getRandomNetwork, Func<List<string>, Random, string> getModifiedRule, List<string> acceptedRegex, Func<int, float> fitnessFunction,
            int elitism, float mutationRate = 0.01f)
        {
            Generation = 1;
            erroneousNetworks = 0;
            Elitism = elitism;
            MutationRate = mutationRate;
            Population = new List<DNA>(populationSize);
            PopulationSize = populationSize;
            newPopulation = new List<DNA>(populationSize);
            this.random = random;
            this.getRandomNetwork = getRandomNetwork;
            this.getModifiedRule = getModifiedRule;
            this.acceptedRegex = acceptedRegex;
            this.fitnessFunction = fitnessFunction;
            for (int i = 0; i < populationSize; i++)
            {
                Population.Add(new DNA(random, getRandomNetwork, getModifiedRule, acceptedRegex, fitnessFunction, init: true));
            }
        }


        public void NewGeneration(int NewDNACount = 0, bool crossoverNewDNA = false)
        {
            int finalCount = Population.Count + NewDNACount;

            if (finalCount <= 0)
            {
                return;
            }
            if (Generation == 1)
            {
                CalculateFitness();
                Population.Sort(CompareDNA);
                erroneousNetworks = Population.Count;
                while (erroneousNetworks > 0)
                {
                    for (int i = 0; i < Population.Count; i++)
                    {
                        // Eliminate the possibility of erroneous networks or 0 fitness networks. The networks should all at least provide a minimal fitness (produce one number out of the 
                        // target set
                        if (Population[i].Fitness == 0 || float.IsNaN(Population[i].Fitness) || Population[i].Equals(null))
                        {
                            Population[i] = new DNA(random, getRandomNetwork, getModifiedRule, acceptedRegex, fitnessFunction, init: true);
                        }
                        else
                        {
                            erroneousNetworks--;
                        }
                    }
                    //TestFitnesses();
                    if (erroneousNetworks == 0)
                    {
                        break;
                    }
                    else
                    {
                        NewGeneration();
                    }
                }
            }
            else if (Population.Count > 0)
            {
                CalculateFitness();
                Population.Sort(CompareDNA);
            }
            newPopulation.Clear();

            for (int i = 0; i < finalCount; i++)
            {
                if (i < Elitism && i < Population.Count)
                {
                    newPopulation.Add(Population[i]);
                }
                else if (i < Population.Count || crossoverNewDNA)
                {
                    DNA parent1 = ChooseParent();
                    DNA parent2 = ChooseParent();
                    DNA child = parent1.Crossover(parent2);

                    child.Mutate(MutationRate);

                    newPopulation.Add(child);
                }
                else
                {
                    newPopulation.Add(new DNA(random, getRandomNetwork, getModifiedRule, acceptedRegex, fitnessFunction, init: true));
                }
            }
            List<DNA> tmpList = Population;
            Population = newPopulation;
            newPopulation = tmpList;
            Generation++;
        }


        private void TestFitnesses()
        {
            for (int i = 0; i < Population.Count; i++)
            {
                Console.Write("Fitness {0}: {1};", i, Population[i].Fitness);
            }
        }

        private int CompareDNA(DNA a, DNA b)
        {
            if (a.Fitness > b.Fitness)
            {
                return -1;
            }
            else if (a.Fitness < b.Fitness)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        // compare the fitnesses of the two networks
        private void CalculateFitness()
        {
            fitnessSum = 0;
            DNA best = Population[0];

            for (int i = 0; i < Population.Count; i++)
            {
                fitnessSum += Population[i].CalculateFitness(i);
                if (Population[i].Fitness > best.Fitness)
                {
                    best = Population[i];
                    best.Genes.minifiedPrint();
                    best.Genes.OutputSet.ForEach(x => Console.Write("{0}\t", x));
                    Console.WriteLine("\nCurrent best fitness: {0}", best.Fitness);
                    FitnessList.Add(best.Fitness);
                }
            }
            BestFitness = best.Fitness;
            BestGenes = ReflectionCloner.DeepFieldClone(best.Genes);
        }

        private DNA ChooseParent()
        {
            double randomNumber = random.NextDouble() * fitnessSum;

            for (int i = 0; i < Population.Count; i++)
            {
                if (randomNumber < Population[i].Fitness)
                {
                    return Population[i];
                }
                randomNumber -= Population[i].Fitness;
            }
            // if the parent could not be chosen for some reason, choose randomly from the top 10% of fittest parents
            int randomParent = random.Next(0, (Convert.ToInt32(Math.Floor(Population.Count * 0.1)) + 1));
            return Population[randomParent];
        }
    }
}
