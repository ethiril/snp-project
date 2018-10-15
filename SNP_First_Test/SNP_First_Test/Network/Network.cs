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
        public String Output { get; set; }
        public bool NetworkClear { get; set; }

        public Network(List<Neuron> neurons, String output, bool networkClear)
        {
            Neurons = neurons;
            Output = output;
            NetworkClear = networkClear;
        }

        public void Spike(Network networkRef)
        {
            int count = 0;
            List<Neuron> NeuronCopy = new List<Neuron>(this.Neurons);
            object sync = new object();

            Parallel.ForEach(NeuronCopy, neuron =>
            {
                lock (sync)
                {
                    count++;
                    Console.WriteLine("Neuron " + count + ", Amount of spikes: " + neuron.SpikeCount);
                    if (neuron.FireSpike(networkRef, neuron.Connections) == true)
                    {
                        if(neuron.IsOutput == true)
                        {
                            this.Output = this.Output + 1;
                            Console.WriteLine("The current output is: " + this.Output);
                        }
                    }
                    else
                    {
                        if (neuron.IsOutput == true)
                        {
                            this.Output = this.Output + 0;
                            Console.WriteLine("The current output is: " + this.Output);
                        }
                    }
                }
            });
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
