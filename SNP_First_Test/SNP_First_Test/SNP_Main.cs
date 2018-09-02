using System;
using SNP_First_Test.Network;
using SNP_Network = SNP_First_Test.Network.Network;
using System.Collections.Generic;
using System.Text;

namespace SNP_First_Test
{
    class SNP_Main
    {
        static SNP_Network evenNumbers = new SNP_Network() {
            new List<Neuron>() {
                new Neuron(new List<Rule>() {
                    new Rule() { new Regex() },
                    new Rule() { new Regex() }}, 0),
            },
            new Object()
        };
   

        static void Main(string[] args)
        {
            Console.WriteLine("Let's get started. //");
            Console.WriteLine("Each spike will occur after 1ms.");
            Console.WriteLine("Test completed.");
            Console.ReadLine();
        }
    }
}