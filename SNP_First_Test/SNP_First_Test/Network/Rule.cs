using System;
using System.Collections.Generic;
using System.Text;

namespace SNP_First_Test.Network
{
    // https://stackoverflow.com/questions/6488034/how-to-implement-a-rule-engine could be an interesting implementation
    /* Each Rule will contain a set of instructions which will fire a neuron across an axon */
    public class Rule
    {
        public int SpikeAmount { get; set; }
        public int DelayAmount { get; set; }
        public int Delay { get; set; }
        public bool? Fire { get; set; }

        public Rule(int spikeAmount, int delayAmount, bool? fire)
        {
            SpikeAmount = spikeAmount;
            DelayAmount = delayAmount;
            Fire = fire;
            Delay = delayAmount;
        }

        public bool? isMatched(int currentSpikeAmount)
        {
            if (this.SpikeAmount >= currentSpikeAmount)
            {
                if (this.Fire == true)
                {
                    if (this.DelayAmount > 0)
                    {
                        this.Delay--;
                        return false;

                    }
                    this.Delay = this.DelayAmount;
                    return true;
                }
                else
                {
                    return null;
                }

            }
            else
            {
                return false;
            }
        }
    }
}
