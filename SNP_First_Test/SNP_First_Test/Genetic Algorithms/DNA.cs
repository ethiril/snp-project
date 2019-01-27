using System;
using System.Collections.Generic;
using System.Text;

namespace SNP_First_Test.Genetic_Algorithms
{
    public class DNA<T>
    {
        // any arbitrary type of gene
        public T[] Genes { get; set; }
        public float Fitness { get; set; }
        private Random random;
        private Func<T> getRandomGene;
        private Func<float, int> fitnessFunction;
        public DNA (int size, Random random, Func<T> getRandomGene, Func<float, int> fitnessFunction, bool initGenes = true)
        {
            Genes = new T[size];
            this.random = random;
            this.getRandomGene = getRandomGene;
            if (initGenes)
            {
                for (int i = 0; i < Genes.Length; i++)
                {
                    Genes[i] = getRandomGene();

                }
            }
        }



        public float CalculateFitness(int index)
        {
            Fitness = fitnessFunction(index);
            return Fitness;
        }        

        public DNA<T> Crossover(DNA<T> secondParent)
        {
            DNA<T> child = new DNA<T>(Genes.Length, random, getRandomGene, fitnessFunction, initGenes: false);
            for (int i = 0; i < Genes.Length; i++)
            {
                child.Genes[i] = random.NextDouble() < 0.5 ? Genes[i] : secondParent.Genes[i];
            }
            return child;
        }


        public void Mutate(float mutationRate)
        {
            for (int i = 0; i < Genes.Length; i++)
            {
                if (random.NextDouble() < mutationRate)
                {
                    Genes[i]
                }
            }
        }

    }
}
