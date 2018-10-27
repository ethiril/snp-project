using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SNP_First_Test.Network
{
    /* 
     *   A network is made up of multiple neurons, this can be edited to contain as many or as little
     *   neurons as you would like. 
     */
    public class Network
    {
        public List<Neuron> Neurons { get; set; }
        public List<int> OutputSet { get; set; }
        private int CurrentOutput { get; set; }
        public bool NetworkClear { get; set; }

        public Network(List<Neuron> neurons, List<int> outputSet, int currentOutput, bool networkClear)
        {
            Neurons = neurons;
            OutputSet = outputSet;
            CurrentOutput = currentOutput;
            NetworkClear = networkClear;
        }

        public void Spike(Network networkRef)
        {
            int count = 0;
            List<Neuron> NeuronCopy = new List<Neuron>(this.Neurons);
            /*
             * Run through removing all of the spikes and recording an output
             * then run through adding all of the spikes that are recorded to have a spike go out to their network
             * Check if each spike sends spike across all axons or if it determines which to go through randomly.
             */
            foreach (Neuron neuron in this.Neurons)
            {
                count++;
                Console.WriteLine("Removing Spikes. Neuron " + count + ", Amount of spikes: " + neuron.SpikeCount);
                if (neuron.RemoveSpikes(networkRef, neuron.Connections) == true)
                {
                    if (neuron.IsOutput == true)
                    {

                        this.OutputSet.Add(CurrentOutput);
                        this.CurrentOutput = 0;
                    }
                }

                else
                {
                    if (neuron.IsOutput == true)
                    {
                        this.CurrentOutput++;
                    }
                }
            };
            count = 0;
            Console.WriteLine("After spike removal: ");
            foreach (Neuron neuron in this.Neurons)
            {
                count++;
                Console.WriteLine("Neuron " + count + ", Amount of spikes: " + neuron.SpikeCount);
            }
            count = 0;
            foreach (Neuron neuron in this.Neurons)
            {
                count++;
                neuron.FireSpike(networkRef, neuron.Connections);
            };
            count = 0;
            Console.WriteLine("After Spike addition: ");
            foreach (Neuron neuron in this.Neurons)
            {
                count++;
                Console.WriteLine("Neuron " + count + ", Amount of spikes: " + neuron.SpikeCount);
            }
            Console.Write("The current output set is: ");
            this.OutputSet.ForEach(i => Console.Write("{0}\t", i));

        }
    }
}

/* 
 *  Type myType = myObject.GetType();
 *  IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
 *  
 *  foreach (PropertyInfo prop in props)
 *  {
 *      object propValue = prop.GetValue(myObject, null);
 *  
 *      // Do something with propValue
 *  }
 *  
 */
