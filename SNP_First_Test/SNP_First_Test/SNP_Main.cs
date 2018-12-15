using System;
using SNP_First_Test.Network;
using SNP_Network = SNP_First_Test.Network.Network;
using SNP_First_Test.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
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

        // https://dotnetfiddle.net/md2hH6 <- JSON Deserialize for constructor parsing from file
        //https://stackoverflow.com/questions/45245032/c-sharp-parse-one-json-field-to-an-object-with-its-custom-constructor
        //https://stackoverflow.com/questions/2246694/how-to-convert-json-object-to-custom-c-sharp-object
        static SNP_Network evenNumbers = new SNP_Network(new List<Neuron>() {
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
                    new Rule("a",0,true),
                    new Rule("a",1,true)
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
            }, new List<int>(), 0, false);

        static int stepAmount = 1000;
        static int stepRepetition = 50;

        static void Main(string[] args)
        {
             Stopwatch stopWatch = new Stopwatch();
            Console.WriteLine("Let's get started. //");
            Console.WriteLine("Press enter to start the test.");
            Console.ReadLine();
            Console.WriteLine("Initial state of the network: ");
            int count = 0;
            foreach (Neuron neuron in evenNumbers.Neurons)
            {
                count++;
                Console.WriteLine("Neuron " + count + ", Amount of spikes: " + neuron.SpikeCount);

            }
            Console.WriteLine("The amount of steps a network will run through: " + stepAmount);
            Console.WriteLine("The amount of cycles each network will go through: " + stepRepetition);
            Console.WriteLine("The total amount of steps that the networks will run through: " + stepAmount*stepRepetition);
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
            Console.WriteLine("----------- Test Started -----------");
            int loopCounter = 0;
            List<int> allOutputs = new List<int>();
            stopWatch.Start();
            for (int i = 0; i < stepRepetition; i++)
            {
                SNP_Network evenNumbersTest = CreateNewNetwork();
                evenNumbersTest.CurrentOutput = 1;
                while ((evenNumbersTest.NetworkClear != true) && evenNumbersTest.GlobalTimer < stepAmount)
                    {
                        stepThrough(loopCounter++, evenNumbersTest);
                    }
                allOutputs.AddRange(evenNumbersTest.OutputSet);
                loopCounter = 0;
            }
            stopWatch.Stop();
            Console.WriteLine("Final output set: ");
            allOutputs = allOutputs.Distinct().ToList();
            allOutputs.Sort();
            allOutputs.ForEach(x => Console.Write("{0}\t", x)); ;
            Console.WriteLine();
            //Console.WriteLine("Press any key to exit");
             Console.WriteLine("Press any key to exit, time elapsed: " + stopWatch.Elapsed.ToString() + "s");
            Console.ReadLine();

        }

        static SNP_Network CreateNewNetwork()
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
                    new Rule("a",0,true),
                    new Rule("a",1,true)
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
            }, new List<int>(), 0, false);

        }
        static void stepThrough(int count, SNP_Network network)
        {
            //Console.WriteLine("Step: " + count + ", generating spike.");
            network.Spike(network);
            //Console.ReadLine();
        }
    }
}