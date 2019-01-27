using System;
using System.Collections.Generic;
using System.Text;

namespace SNP_First_Test.Genetic_Algorithms
{
    public class GeneticAlgorithm<T>
    {
        public List<DNA<T>> Population { get; set; }
        public int Generation { get; set; }
        public float BestFitness { get; set; }
        public T[] BestGenes { get; set; }
        public float MutationRate;
        private Random random;
        private float fitnessSum;

        public GeneticAlgorithm(int populationSize, int dnaSize, Random random, Func<T> getRandomGene, Func<float, int> fitnessFunction, float mutationRate = 0.01f)
        {
            Generation = 1;
            MutationRate = mutationRate;
            Population = new List<DNA<T>>();
            this.random = random;
            for (int i = 0; i< populationSize; i++)
            {
                Population.Add(new DNA<T>(dnaSize, random, getRandomGene, fitnessFunction, initGenes: true));
            }
        }

        public void NewGeneration()
        {
            if (Population.Count <= 0)
            {
                return;
            }
            CalculateFitness();
            List<DNA<T>> newPopulation = new List<DNA<T>>();
            for (int i = 0; i < Population.Count; i++)
            {
                DNA<T> parent1 = ChooseParent()
                DNA<T> parent2 = ChooseParent();
                DNA<T> child = parent1.Crossover(parent2);
                child.Mutate(MutationRate);
                newPopulation.Add(child);
            }
            Population = newPopulation;
            Generation++;
        }

        public void CalculateFitness()
        {
            fitnessSum = 0;
            DNA<T> best = Population[0];

            for (int i = 0; i < Population.Count; i++)
            {
                fitnessSum += Population[i].CalculateFitness(i);
                if (Population[i].Fitness > best.Fitness) {
                    best = Population[i];
                }
            }
            BestFitness = best.Fitness;
            best.Genes.CopyTo(BestGenes, 0);
        }

        private DNA<T> ChooseParent()
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
