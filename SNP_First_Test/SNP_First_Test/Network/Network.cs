using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SNP_First_Test.Network
{
    /* 
     *   A network is made up of multiple neurons, this can be edited to contain as many or as little
     *   neurons as you would like. 
     */
    public class Network
    {
        public List<Neuron> Neurons { get; set; }
        public Object Output { get; set; }

        public Network(List<Neuron> neurons, Object output) {
            Neurons = neurons;
            Output = output;
        }

        public void Spike(Network networkRef)
        {
            int count = 0;
            foreach (Neuron neuron in Neurons)
            {
                count++;
                Console.WriteLine("Neuron " + count  + ", Amount of spikes: " + neuron.SpikeCount);
                neuron.FireSpike(networkRef, neuron.Connections);
            }
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