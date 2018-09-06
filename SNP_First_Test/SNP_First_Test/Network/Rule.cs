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
        public int Delay { get; set; }
        public bool? Fire { get; set; }

        public Rule(int spikeAmount, int delay, bool? fire)
        {
            SpikeAmount = spikeAmount;
            Delay = delay;
            Fire = fire;
        }

        public bool? isMatched(int currentSpikeAmount)
        {
            if (this.SpikeAmount == currentSpikeAmount)
            {
                if (this.Fire == true)
                {
                    if (this.Delay > 0)
                    {
                        this.Delay -= 1;
                        return false;

                    }
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
