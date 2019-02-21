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
        public SNP_Network BestGenes { get; private set; }

        public int Elitism;
        public float MutationRate;

        private List<DNA> newPopulation;
        private Random random;
        private float fitnessSum;
        private Func<SNP_Network> getRandomNetwork;
        private Func<List<string>, Random, string> getModifiedRule;
        private Func<int, float> fitnessFunction;

        public GeneticAlgorithm(int populationSize, Random random, Func<SNP_Network> getRandomNetwork, Func<List<string>, Random, string> getModifiedRule, Func<int, float> fitnessFunction,
            int elitism, float mutationRate = 0.01f)
        {
            Generation = 1;
            Elitism = elitism;
            MutationRate = mutationRate;
            Population = new List<DNA>(populationSize);
            newPopulation = new List<DNA>(populationSize);
            this.random = random;
            this.getRandomNetwork = getRandomNetwork;
            this.getModifiedRule = getModifiedRule;
            this.fitnessFunction = fitnessFunction;
            for (int i = 0; i < populationSize; i++)
            {
                Population.Add(new DNA(random, getRandomNetwork, getModifiedRule, fitnessFunction, init: true));
            }
        }
        public void NewGeneration(int numNewDNA = 0, bool crossoverNewDNA = false)
        {
            int finalCount = Population.Count + numNewDNA;

            if (finalCount <= 0)
            {
                return;
            }
            if (Population.Count > 0)
            {
                CalculateFitness();
                Population.Sort(CompareDNA);
            }
            newPopulation.Clear();

            for (int i = 0; i < Population.Count; i++)
            {
                if (i < Elitism && i < Population.Count)
                {
                    newPopulation.Add(Population[i]);
                }
                else if (i < Population.Count || crossoverNewDNA)
                {
                    DNA parent1 = ChooseParent();
                    //Console.WriteLine("Chosen Parent 1, its fitness is: {0}", parent1.Fitness);
                    DNA parent2 = ChooseParent();
                    //Console.WriteLine("Chosen Parent 2, its fitness is: {0}", parent2.Fitness);
                    DNA child = parent1.Crossover(parent2);

                    child.Mutate(MutationRate);

                    newPopulation.Add(child);
                }
                else
                {
                    newPopulation.Add(new DNA(random, getRandomNetwork, getModifiedRule, fitnessFunction, init: true));
                }
            }
            List<DNA> tmpList = Population;
            Population = newPopulation;
            newPopulation = tmpList;
            Generation++;
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
                    if (best.Fitness >= 1)
                    {
                        best.Genes.minifiedPrint();
                        
                    }
                    Console.WriteLine("\nCurrent best fitness: {0}", best.Fitness);
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
            return null;
        }
    }
}
