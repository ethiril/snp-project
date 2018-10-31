using System;
using SNP_First_Test.Network;
using SNP_Network = SNP_First_Test.Network.Network;
using SNP_First_Test.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace SNP_First_Test
{
    class SNP_Main
    {

        /* TODO
         * 3.) Add binary output to solution.
         * 4.) Check whether no rules apply any more.  
         * 5.) Change network creation to use JSON for now (easier import/export larger networks)
         * 6.) Move onto implementing a genetic algorithm solution
         * 
         */
        // test network (tutorial figure 1)

        // https://dotnetfiddle.net/md2hH6 <- JSON Deserialize for constructor parsing from file
        public static SNP_Network evenNumbers = new SNP_Network(new List<Neuron>() {
                new Neuron(new List<Rule>(){
                    new Rule(2,0,true),
                    new Rule(1,0,false)
                }, 2, new List<int>() {4}, false),
                 new Neuron(new List<Rule>() {
                    new Rule(2,0,true),
                    new Rule(1,0,false)
                }, 2, new List<int>() {5}, false),
                 new Neuron(new List<Rule>() {
                    new Rule(2,0,true),
                    new Rule(1,0,false)
                }, 2, new List<int>() {6}, false),
                  new Neuron(new List<Rule>() {
                    new Rule(1,0,true),
                    new Rule(1,1,false)
                }, 0, new List<int>() {1, 3, 7}, false),
                   new Neuron(new List<Rule>() {
                    new Rule(1,0,true),
                }, 0, new List<int>() {1, 2, 7}, false),
                   new Neuron(new List<Rule>() {
                    new Rule(1,0,true),
                }, 0, new List<int>() { 3, 7}, false),
                   new Neuron(new List<Rule>() {
                    new Rule(2,0,true),
                    new Rule(3,0,false)
                }, 2, new List<int>() { }, true),
            }, new List<int>(), 0, false);

        static int stepAmount = 10000;

        static void Main(string[] args)
        {
            // Stopwatch stopWatch = new Stopwatch();
            Console.WriteLine("Let's get started. //");
            Console.WriteLine("Press enter to start the test.");
            Console.ReadLine();
            // stopWatch.Start();
            Console.WriteLine("----------- Test Started -----------");
            Console.WriteLine("Initial state of the network: ");
            int count = 0;
            foreach (Neuron neuron in evenNumbers.Neurons)
            {
                count++;
                Console.WriteLine("Neuron " + count + ", Amount of spikes: " + neuron.SpikeCount);

            }
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
            int loopCounter = 0;
            while ((evenNumbers.NetworkClear != true) && evenNumbers.GlobalTimer < stepAmount) 
            {
                stepThrough(loopCounter++);
            }
            Console.WriteLine("Final output set: ");
            // stopWatch.Stop();
            evenNumbers.OutputSet.ForEach(i => Console.Write("{0}\t", i));
            Console.WriteLine();
            Console.WriteLine("Press any key to exit");
            // Console.WriteLine("Press any key to exit, time elapsed: " + stopWatch.Elapsed.ToString() + "s");
            Console.ReadLine();

        }

        static void stepThrough(int count)
        {
            Console.WriteLine("Step: " + count + ", generating spike.");
            evenNumbers.Spike(evenNumbers);
            Console.ReadLine();
        }
    }
}