# Spiking Neural P Systems evolved with the use of Genetic Algorithms
#### Researched and Developed by Michael Stachowicz, 15068126 - MMU

This project relies on .netcore2.0 and will not compile without it.

Reflection Cloner (ClonerHelpers.cs, ICloneFactory.cs, ReflectionCloner.cs) provided by Nuclex, you can find it at: http://blog.nuclex-games.com/mono-dotnet/fast-deep-cloning/

Thanks to Newtonsoft for the JSON parser libraries which are provided with this software

## How to build

To run the compiled binaries, extract the win10-x64.zip file and find the SNP_First_Test.exe file and run it.

To build the project, ensure that you are running the correct platform and simply press F5 or hit the Run button.

## Using the GUI

In the GUI, there are 7 options, some of which contain sub-options from which you can choose from, as shown below:
1. Change the configuration of the system
  1. Change the maximum steps a network can take
  2. Change the amount of step-throughs a network will undergo before generating an output
  3. Change the Genetic Algorithm population size
  4. Change the Mutation Rate
  5. Change the Maximum Generations limit (how many generations will evolve before stopping the code)
  6. Change the Experimental Rule settings (allow or deny a varied template setting)
  7. Return to a default configuration
2. Evolve an SN P system based on a natural numbers set
3. Evolve an SN P system based on a even numbers set
4. Run the natural numbers SN P system without evolution
5. Run the even numbers SN P system without evolution
6. Evolve an experimental network (not advised!)
7. Import a pre-generated network from a file (JSON format)

To return to any previous menu, simply press the ESC key.

## Understanding the evolution outputs in the console
For options 2, 3 and 6, outputs in the console will appear as a fitness of the general population increases with every generation. This will show you what the current generated network contains, including its Neuron structure, Rules per neuron and the distinct outputs that it has generated to give the network such a high fitness.

For outputs 4 and 5 and 7, the screen will be filled with numbers that were generated through running the SN P network specified, as well as a message informing you how long the network took to run. To import your own network (Generated networks from options 2,3 and 6 will be saved to individual folders, as described in the message at the end of a run) simply choose option 7 and provide the correct Folder and file name from the base directory of the executable. This should look like this: `6234234242322/Network.json`, then press enter and the program will run the provided network configuration and provide an output.
