using System;
using SNP_First_Test.Network;
using System.Collections.Generic;
using System.Text;

namespace SNP_First_Test
{
    class SNP_Main
    {
        static List<Rule> rules = new List<Rule>() { new Rule(), new Rule() };
        static Neuron test = new Neuron(rules, 0);

        static void Main(string[] args)
        {
            Console.WriteLine("Let's get started. //");
            Console.WriteLine("Each spike will occur after 1ms.");
            if (test.FireSpike() == true)
            {
                Console.WriteLine("Test completed.");
            }
            Console.ReadLine();
        }
    }
}
