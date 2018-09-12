﻿using System;
using SNP_First_Test.Network;
using SNP_Network = SNP_First_Test.Network.Network;
using System.Collections.Generic;
using System.Text;

namespace SNP_First_Test
{
    class SNP_Main
    {

        /* TODO 
         * 1.) Implement step through function (slow) to simulate the network (x)
         * 2.) Make the Rules non-deterministic
         * 3.) Change the code in rules to simply show false rather than null
         * 4.) Change network creation to use JSON for now (easier import/export larger networks)
         * 5.) Move onto implementing a genetic algorithm solution
         */
        // test network (tutorial figure 1)

        public static SNP_Network evenNumbers = new SNP_Network(new List<Neuron>() {
                new Neuron(new List<Rule>(){
                    new Rule(2,0,true),
                    new Rule(1,0,null)
                }, 2, new List<int>() {4}, false),
                 new Neuron(new List<Rule>() {

                    new Rule(2,0,true),
                    new Rule(1,0,null)
                }, 2, new List<int>() {5}, false),
                 new Neuron(new List<Rule>() {
                    new Rule(2,0,true),
                    new Rule(1,0,null)
                }, 2, new List<int>() {6}, false),
                  new Neuron(new List<Rule>() {
                    new Rule(1,0,true),
                    new Rule(1,1,null)
                }, 0, new List<int>() {1, 3, 7}, false),
                   new Neuron(new List<Rule>() {
                    new Rule(1,0,true),
                }, 0, new List<int>() {1, 2, 7}, false),
                   new Neuron(new List<Rule>() {
                    new Rule(1,0,true),
                }, 0, new List<int>() { 3, 7}, false),
                   new Neuron(new List<Rule>() {
                    new Rule(2,0,true),
                    new Rule(3,0,null)
                }, 2, new List<int>() { }, true),
            }, new Object());


        static void Main(string[] args)
        {
            Console.WriteLine("Let's get started. //");
            Console.WriteLine("Press enter to start the test.");
            Console.ReadLine();
            Console.WriteLine("----------- Test Started -----------");
            Random random = new Random();
            Console.WriteLine("Initial state of the network: ");
            int count = 0;
            foreach(Neuron neuron in evenNumbers.Neurons)
            {
                count++;
                Console.WriteLine("Neuron " + count + ", Amount of spikes: " + neuron.SpikeCount);
               
            }
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
            for (int i = 0; i < 20; i++)
            {
                stepThrough(i);
            }
            
        }

        static void stepThrough(int count)
        {
            Console.WriteLine("Step: " + count + ", generating spike.");
            evenNumbers.Spike(evenNumbers);
            Console.ReadLine();




        }
    }
}