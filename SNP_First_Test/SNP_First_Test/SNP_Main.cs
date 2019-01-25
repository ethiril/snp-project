using System;
using SNP_First_Test.Network;
using SNP_Network = SNP_First_Test.Network.Network;
using System.Collections.Generic;
using System.Diagnostics;
using SNP_First_Test.Utilities;
using System.Linq;

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
        static readonly int maxSteps = 1000;
        static readonly int stepRepetition = 10000;

        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            Console.WriteLine("Let's get started. //");
            Console.WriteLine("Press enter to start the test.");
            Console.ReadLine();
            Console.WriteLine("Initial state of the network: ");
            int count = 0;
            SNP_Network evenNumbers = CreateNewNetwork();
            foreach (Neuron neuron in evenNumbers.Neurons)
            {
                count++;
                Console.WriteLine("Neuron " + count + ", Amount of spikes: " + neuron.SpikeCount);

            }
            Console.WriteLine("The amount of cycles each network will go through: " + stepRepetition);
            evenNumbers.print();
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
            Console.WriteLine("----------- Test Started -----------");
            int loopCounter = 0;
            List<int> allOutputs = new List<int>();
            stopWatch.Start();
            // clone the initial network setup for a network reset
            // then loop across, resetting the network after every output
            // test whether the random method is really giving us good random outputs.
            // when a number hits, the next x amounts are also the same????
            SNP_Network RuleState = ReflectionCloner.DeepFieldClone(evenNumbers);
            evenNumbers.CurrentOutput = 0;
            for (int i = 0; i < stepRepetition; i++)
            {
                //Console.WriteLine("---------- Iteration " + i + "----------");
                evenNumbers = CreateNewNetwork();
                //Console.WriteLine("EvenNumbers new network engaged state: " + evenNumbers.NetworkEngaged);
                while (evenNumbers.NetworkClear == false && evenNumbers.GlobalTimer < maxSteps)
                {
                    stepThrough(loopCounter++, evenNumbers);
                }
                allOutputs.AddRange(evenNumbers.OutputSet);
                //Utils.ResetNetwork(RuleState, evenNumbers);
                loopCounter = 0;
            }
            stopWatch.Stop();
            Console.WriteLine("Final output set: ");
            allOutputs = allOutputs.Distinct().ToList();
            allOutputs.Sort();
            allOutputs.ForEach(x => Console.Write("{0}\t", x)); ;
            Console.WriteLine();
            Console.WriteLine("Press any key to exit, time elapsed: " + stopWatch.Elapsed.ToString() + "s");
            Console.ReadLine();

        }

        static SNP_Network CreateNewNetwork()
        {
            return new SNP_Network(new List<Neuron>() {
                   new Neuron(new List<Rule>() {
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


         /*static SNP_Network CreateNewNetwork()
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
            //Console.WriteLine("Step: " + count + ", generating spike.");
            network.Spike(network);
            //Console.ReadLine();
        }
    }
}