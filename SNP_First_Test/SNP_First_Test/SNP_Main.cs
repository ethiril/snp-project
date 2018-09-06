using System;
using SNP_First_Test.Network;
using SNP_Network = SNP_First_Test.Network.Network;
using System.Collections.Generic;
using System.Text;

namespace SNP_First_Test
{
    class SNP_Main
    {
        // test network (tutorial figure 1)
        // This can really be done in a better way. 
        SNP_Network evenNumbers = new SNP_Network(new List<Neuron>() {
                new Neuron(new List<Rule>() {
                    new Rule(2,0,true),
                    new Rule(1,0,null)
                }, 2, new List<Neuron>() { }, false),
                 new Neuron(new List<Rule>() {
                    new Rule(2,0,true),
                    new Rule(1,0,null)
                }, 2, new List<Neuron>() { }, false),
                 new Neuron(new List<Rule>() {
                    new Rule(2,0,true),
                    new Rule(1,0,null)
                }, 2, new List<Neuron>() { }, false),
                  new Neuron(new List<Rule>() {
                    new Rule(1,0,true),
                    new Rule(1,1,null)
                }, 0, new List<Neuron>() { }, false),
                   new Neuron(new List<Rule>() {
                    new Rule(1,0,true),
                }, 0, new List<Neuron>() { }, false),
                   new Neuron(new List<Rule>() {
                    new Rule(1,0,true),
                }, 0, new List<Neuron>() { }, false),
                   new Neuron(new List<Rule>() {
                    new Rule(2,0,true),
                    new Rule(3,0,null)
                }, 2, new List<Neuron>() { }, false),
            }, new Object());


        static void Main(string[] args)
        {
            Console.WriteLine("Let's get started. //");
            Console.WriteLine("Each spike will occur after 1ms.");
            Console.WriteLine("Test completed.");
            Console.ReadLine();
        }
    }
}