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
    class Network
    {
        List<Neuron> Neurons { get; set; }
        Object Output { get; set; }

        public Network(List<Neuron> neurons, Object output) {
            Neurons = neurons;
            Output = output;
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