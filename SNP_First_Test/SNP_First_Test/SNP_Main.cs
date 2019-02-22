using System;
using SNP_First_Test.Network;
using SNP_Network = SNP_First_Test.Network.Network;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SNP_First_Test.Utilities;
using SNP_First_Test.Genetic_Algorithms;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace SNP_First_Test
{
    class SNP_Main
    {

        /* TODO
         * 4.) Check whether no rules apply any more.  
         * 5.) Change network creation to use JSON for now (easier import/export larger networks)
         * 6.) Move onto implementing a genetic algorithm solution
         * 
         */

        // https://stackoverflow.com/questions/2246694/how-to-convert-json-object-to-custom-c-sharp-object
        // test network (tutorial figure 1)

        //https://stackoverflow.com/questions/45245032/c-sharp-parse-one-json-field-to-an-object-with-its-custom-constructor
        //https://stackoverflow.com/questions/2246694/how-to-convert-json-object-to-custom-c-sharp-object
        static int maxSteps = 50;
        static int stepRepetition = 50;
        static int populationSize = 50;
        static float mutationRate = 0.1f;
        static int maximumGenerations = 25;
        static int testBestFitness = 5;
        // maximum size of each spike grouping in the random new gene
        static private int maxExpSize = 4;
        // set of numbers that I expect to see after the evolution
        static private List<int> expectedSet = new List<int>() { 2, 4, 6, 8, 10, 12, 14, 16 };
        static private bool experimentalRules = true;
        static private List<string> acceptedRegex = new List<string>()
        {
            "x" // direct match
        };
        static private List<string> acceptedRegexExperimental = new List<string>() {
            "x", // direct match 
            "x+", // x followed by one or more
            "x*", // x followed by zero or more
            "x?", // x followed by zero or one 
            "x(y)+", // x followed by one or more y groupings
            "x(y)*", // x followed by zero or more y groupings
            "x(y)?", // x followed by zero or one y groupings
        };
        static string[] options = new string[8] {
            "1. Change the configuration",
            "2. Evolve a natural numbers network",
            "3. Evolve an even numbers network",
            "4. Run natural numbers network",
            "5. Run evens network",
            "6. Evolve experimental network",
            "7. Import Network from file",
            "8. Exit"
        };
        static private int elitism = 4;
        private static GeneticAlgorithm ga;
        static private Random random = new Random((int)DateTime.Now.Ticks);

        static void Main(string[] args)
        {
            Console.SetWindowSize(Console.WindowWidth, Console.WindowHeight + 5);
            Menu();
            Console.WriteLine("Thanks for testing! :)");
            Console.ReadLine();
        }


        // based on https://stackoverflow.com/questions/46908148/controlling-menu-with-the-arrow-keys-and-enter
        static int MultipleChoiceMenuDisplay(bool cancel, params string[] options)
        {

            const int startX = 4;
            const int startY = 20;
            const int optionsPerLine = 1;
            const int spacingPerLine = 2;
            int maxLength = 0;
            for (int i = 0; i < options.Length; i++)
            {
                maxLength = (options[i].Length > maxLength) ? options[i].Length + 4 : maxLength;
            }
            int currentSelection = 0;

            ConsoleKey key;

            Console.CursorVisible = false;

            do
            {
                Console.Clear();
                Splash();
                ConfigurationDisplay();
                Console.WriteLine(new String(' ', startX) + new String('-', maxLength));
                for (int i = 0; i < options.Length; i++)
                {
                    Console.SetCursorPosition(startX + (i % optionsPerLine) * spacingPerLine, startY + i / optionsPerLine);
                    if (i == currentSelection)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                    }
                    Console.Write("| " + options[i] + new String(' ', maxLength - options[i].Length - 4) + " |\n");
                    Console.ResetColor();
                }
                Console.WriteLine(new String(' ', startX) + new String('-', maxLength));
                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                        {
                            if (currentSelection % optionsPerLine > 0)
                                currentSelection--;
                            break;
                        }
                    case ConsoleKey.RightArrow:
                        {
                            if (currentSelection % optionsPerLine < optionsPerLine - 1)
                                currentSelection++;
                            break;
                        }
                    case ConsoleKey.UpArrow:
                        {
                            if (currentSelection >= optionsPerLine)
                                currentSelection -= optionsPerLine;
                            break;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            if (currentSelection + optionsPerLine < options.Length)
                                currentSelection += optionsPerLine;
                            break;
                        }
                    case ConsoleKey.Escape:
                        {
                            if (cancel)
                                return -1;
                            break;
                        }
                }
            } while (key != ConsoleKey.Enter);

            Console.CursorVisible = true;

            return currentSelection;
        }

        static void ConfigurationDisplay()
        {
            Console.WriteLine(" Current Configuration:");
            Console.Write(" Enabled Experimental Rules : ");
            if (experimentalRules)
            {
                SingleColour(ConsoleColor.Green, experimentalRules);
            }
            else
            {
                SingleColour(ConsoleColor.Red, experimentalRules);
            }
            Console.Write("; Max Steps per network: ");
            SingleColour(ConsoleColor.Cyan, maxSteps);
            Console.Write("; Step-Through amount per network: ");
            SingleColour(ConsoleColor.Cyan, stepRepetition);
            Console.Write(";\n Genetic Algorithm Population Size: ");
            SingleColour(ConsoleColor.Cyan, populationSize);
            Console.Write("; Mutation Rate: ");
            SingleColour(ConsoleColor.Cyan, mutationRate);
            Console.Write("; Maximum Number of Generations: ");
            SingleColour(ConsoleColor.Cyan, maximumGenerations);
            Console.Write(";\n Expected Set: {");
            expectedSet.ForEach(x => Console.Write("{0}\t", x));
            Console.Write("}\n\n");
        }

        static void SingleColour(ConsoleColor color, object word)
        {
            Console.ForegroundColor = color;
            Console.Write(word);
            Console.ResetColor();
        }

        static void Splash()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  _____ _   _  ______   _____           _                     ");
            Console.WriteLine(" /  ___| \\ | | | ___ \\ /  ___|         | |                    ");
            Console.WriteLine(" \\ `--.|  \\| | | |_/ / \\ `--. _   _ ___| |_ ___ _ __ ___  ___ ");
            Console.WriteLine("  `--. \\ . ` | |  __/   `--. \\ | | / __| __/ _ \\ '_ ` _ \\/ __|");
            Console.WriteLine(" /\\__/ / |\\  | | |     /\\__/ / |_| \\__ \\ ||  __/ | | | | \\__ \\");
            Console.WriteLine(" \\____/\\_| \\_/ \\_|     \\____/ \\__, |___/\\__\\___|_| |_| |_|___/");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("  _____         _           _");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("  ._/ |\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" |   __|_ _ ___| |_ _ ___ _| |");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(".|___/ \n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" |   __| | | . | | | | -_| . |");
            Console.WriteLine(" |_____|\\_/|___|_|\\_/|___|___|\n\n");
            Console.ResetColor();
            Console.WriteLine(" Please select an option from the menu below using your arrow and enter keys:\n");
        }

        static void Menu(bool defaultChoice = false, bool configChoice = false, int defaultNum = 0, int defaultConfig = 0)
        {
            int choice;
            int config;
            if (defaultChoice)
            {
                choice = defaultNum;
            }
            else
            {
                choice = MultipleChoiceMenuDisplay(false, options);
            }
            switch (choice)
            /*
            "1. Change the configuration", ./
            "2. Evolve a natural numbers network", ./
            "3. Evolve an even numbers network", ./
            "4. Evolve a natural numbers network with random delays",
            "5. Evolve an even numbers network with random delays",
            "6. Run standard natural numbers network",
            "7. Run standard evens network",
            "8. Run experimental network",
            "9. Import Network from file" };
            */
            {
                case 0:
                    if (configChoice)
                    {
                        config = defaultConfig;
                    }
                    else
                    {
                        config = MultipleChoiceMenuDisplay(true, "Max Steps", "Step-Through Amount", "GA Population Size", "Mutation Rate", "Max Generations", "Expected Set", "Experimental Rules", "Default Config");
                        if (config == -1)
                        {
                            Menu();
                        }
                    }
                    switch (config)
                    {
                        case 0:
                            Console.Clear();
                            Splash();
                            Console.WriteLine("Please provide the maximum steps amount, or press ESC to return to the last menu: ");
                            Console.Write("Enter Number: ");
                            int steps;
                            if (int.TryParse(readLineWithCancel(false, 0), out steps))
                            {
                                maxSteps = steps;
                            }
                            else
                            {
                                Console.WriteLine("\nNumber was not an integer, please try press enter to try again. ");
                                Console.ReadLine();
                                Menu(true, true, 0, 0);
                            }
                            Menu();
                            break;
                        case 1:
                            Console.Clear();
                            Splash();
                            Console.WriteLine("Please provide the step-through amount, or press ESC to return to the last menu: ");
                            Console.Write("Enter Number: ");
                            int st;
                            if (int.TryParse(readLineWithCancel(false, 0), out st))
                            {
                                stepRepetition = st;
                            }
                            else
                            {
                                Console.WriteLine("\nNumber was not an integer, please try press enter to try again. ");
                                Console.ReadLine();
                                Menu(true, true, 0, 1);
                            }
                            Menu();
                            break;
                        case 2:
                            Console.Clear();
                            Splash();
                            Console.WriteLine("Please provide the GA Population size, or press ESC to return to the last menu: ");
                            Console.Write("Enter Number: ");
                            int gaSize;
                            if (int.TryParse(readLineWithCancel(false, 0), out gaSize))
                            {
                                populationSize = gaSize;
                            }
                            else
                            {
                                Console.WriteLine("\nNumber was not an integer, please try press enter to try again. ");
                                Console.ReadLine();
                                Menu(true, true, 0, 2);
                            }
                            Menu();
                            break;
                        case 3:
                            Console.Clear();
                            Splash();
                            Console.WriteLine("Please provide the mutation rate, or press ESC to return to the last menu: ");
                            Console.Write("Enter float: ");
                            float mutation;
                            if (float.TryParse(readLineWithCancel(false, 0), out mutation))
                            {
                                mutationRate = mutation;
                            }
                            else
                            {
                                Console.WriteLine("\nNumber was not a float, please try press enter to try again. ");
                                Console.ReadLine();
                                Menu(true, true, 0, 3);
                            }
                            Menu();
                            break;
                        case 4:
                            Console.Clear();
                            Splash();
                            Console.WriteLine("Please provide the maximum generations amount, or press ESC to return to the last menu: ");
                            Console.Write("Enter Number: ");
                            int maxGen;
                            if (int.TryParse(readLineWithCancel(false, 0), out maxGen))
                            {
                                maximumGenerations = maxGen;
                            }
                            else
                            {
                                Console.WriteLine("\nNumber was not an integer, please try press enter to try again. ");
                                Console.ReadLine();
                                Menu(true, true, 0, 4);
                            }
                            Menu();
                            break;
                        case 5:
                            Console.Clear();
                            Splash();
                            Console.WriteLine("Please provide the list of integers separated by a comma that you wish to use as your expected set, or press ESC to return to the last menu: ");
                            Console.Write("Enter Set: ");
                            string result = readLineWithCancel(false, 0);
                            List<int> list = new List<int>();
                            list = result.Split(',')
                                        .Select(s =>
                                         {
                                             int i;
                                             return Int32.TryParse(s, out i) ? i : -1;
                                         }).ToList();
                            foreach (int i in list)
                            {
                                if (i == -1)
                                {
                                    Console.WriteLine("\nA Number in the set was not an integer, please try press enter to try again. ");
                                    Console.ReadLine();
                                    expectedSet = new List<int>() { 2, 4, 6, 8, 10, 12, 14, 16 };
                                    Menu(true, true, 0, 5);
                                }
                            }
                            expectedSet = list;
                            Menu();
                            break;
                        case 6:
                            Console.Clear();
                            Splash();
                            Console.WriteLine("Please select which rule setting you would like to use: ");
                            int expChoice = MultipleChoiceMenuDisplay(true, "DISABLED", "ENABLED");
                            if (expChoice == -1)
                            {
                                Menu(true, false);
                            }
                            else
                            {
                                experimentalRules = (expChoice == 1);
                            }
                            Menu();
                            break;
                        case 7:
                            Console.Clear();
                            Splash();
                            Console.WriteLine("Please confirm whether to load default values for the configuration: ");
                            int defaults = MultipleChoiceMenuDisplay(true, "YES", "NO");
                            if (defaults == -1)
                            {
                                Menu(true, false);
                            }
                            else
                            {
                                if (defaults == 0)
                                {
                                    LoadDefaultConfiguration();
                                }
                            }
                            Menu();
                            break;
                        default:
                            Menu();
                            break;
                    }
                    break;
                case 1:
                    Console.Clear();
                    long time = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
                    string normFileName = "/" + time + "/NatNumsNet.json";
                    Console.WriteLine("The File will be saved to: {0}/{1}.json", Directory.GetCurrentDirectory(), normFileName);
                    Console.WriteLine("Press the enter key to carry out this test.");
                    Console.ReadLine();
                    Console.WriteLine("---------- Evolving a network based on the Natural Numbers Spiking Neural P System ----------");
                    ga = new GeneticAlgorithm(populationSize, random, CreateNewRandomNormalNetwork, GenerateRandomExpression, (experimentalRules) ? acceptedRegexExperimental : acceptedRegex, FitnessFunction, elitism, mutationRate);
                    UpdateGA(ga);
                    Utils.CreateFolder(time.ToString());
                    string normCSVFile = "/" + time + "/NatNumsNet.csv";
                    Utils.SaveCSV(ga.FitnessList, normCSVFile);
                    Utils.SaveNetwork(ga.BestGenes, normFileName);

                    // afterwards save the data
                    break;
                case 2:
                    Console.Clear();
                    string evensFileName = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) + "EvenNumsNet.json";
                    Console.WriteLine("The File will be saved to: {0}/{1}.json", Directory.GetCurrentDirectory(), evensFileName);
                    Console.WriteLine("Press the enter key to carry out this test.");
                    Console.ReadLine();
                    Console.WriteLine("---------- Evolving a network based on the Evens Spiking Neural P System ----------");
                    ga = new GeneticAlgorithm(populationSize, random, CreateNewRandomEvensNetwork, GenerateRandomExpression, (experimentalRules) ? acceptedRegexExperimental : acceptedRegex, FitnessFunction, elitism, mutationRate);
                    UpdateGA(ga);
                    string evensCSVFile = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) + "EvenNumsNet.csv";
                    Utils.SaveCSV(ga.FitnessList, evensCSVFile);
                    // afterwards save the data
                    break;
                case 3:
                    Console.Clear();
                    Console.WriteLine("---------- Running a standard test to get an output from a Natural Numbers Network ----------");
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    List<int> currentOutputs = SNPRun(CreateNaturalNumbersNetwork());
                    stopWatch.Stop();
                    Console.WriteLine("Final output set: ");
                    currentOutputs.ForEach(x => Console.Write("{0}\t", x)); ;
                    Console.WriteLine();
                    Console.WriteLine("Press any key to exit, time elapsed: " + stopWatch.Elapsed.ToString() + "s");
                    Console.ReadLine();
                    break;
                case 4:
                    Console.Clear();
                    Console.WriteLine("---------- Running a standard test to get an output from an Even Numbers Network ----------");
                    Stopwatch evenStopWatch = new Stopwatch();
                    evenStopWatch.Start();
                    List<int> evenOutputs = SNPRun(CreateNaturalNumbersNetwork());
                    evenStopWatch.Stop();
                    Console.WriteLine("Final output set: ");
                    evenOutputs.ForEach(x => Console.Write("{0}\t", x)); ;
                    Console.WriteLine();
                    Console.WriteLine("Press any key to exit, time elapsed: " + evenStopWatch.Elapsed.ToString() + "s");
                    Console.ReadLine();
                    break;
                case 5:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("!!! These features are experimental and will not provide meaningful results, are you sure you wish to continue? !!!");
                    Console.ResetColor();
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                        string expFileName = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) + "ExpNet.json";
                        Console.WriteLine("The File will be saved to: {0}/{1}.json", Directory.GetCurrentDirectory(), expFileName);
                        Console.WriteLine("Press the enter key to carry out this test.");
                        Console.ReadLine();
                        Console.WriteLine("---------- Evolving a network based on the Evens Spiking Neural P System ----------");
                        ga = new GeneticAlgorithm(populationSize, random, GenerateNewRandomNetwork, GenerateRandomExpression, (experimentalRules) ? acceptedRegexExperimental : acceptedRegex, FitnessFunction, elitism, mutationRate);
                        string expCSVFile = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) + "ExpNet.csv";
                        Utils.SaveCSV(ga.FitnessList, expCSVFile);
                    }
                    else
                    {
                        Menu();
                        break;
                    }
                    break;
                case 6:
                    Console.Clear();
                    Splash();
                    Console.WriteLine("Please enter a filename to import from, or press the ESC key to return to the last menu: ");
                    Console.Write("Enter Filename (WITH the extension): ");
                    string import = readLineWithCancel(true, 6);
                    SNP_Network importedNetwork = Utils.ReadNetworkFromFile(import);
                    if (importedNetwork == null)
                    {
                        Console.WriteLine("Filepath was wrong. Press Enter to try again. ");
                        Console.ReadLine();
                        Menu(true, false, 6);
                    }
                    else
                    {
                        Console.WriteLine("\n");
                        importedNetwork.minifiedPrint();
                        Stopwatch importStopWatch = new Stopwatch();
                        importStopWatch.Start();
                        List<int> importOutputs = SNPRun(CreateNaturalNumbersNetwork());
                        importStopWatch.Stop();
                        Console.WriteLine("Final output set: ");
                        importOutputs.ForEach(x => Console.Write("{0}\t", x)); ;
                        Console.WriteLine();
                        Console.WriteLine("Press any key to exit, time elapsed: " + importStopWatch.Elapsed.ToString() + "s");
                        Console.ReadLine();
                    }
                    break;
                case 7:
                    Console.Clear();
                    Splash();
                    Console.WriteLine("Are you sure you wish to exit? ");
                    int exit = MultipleChoiceMenuDisplay(true, "YES", "NO");
                    if (exit == -1)
                    {
                        Menu();
                    }
                    else
                    {
                        if (exit == 0)
                        {
                            Environment.Exit(0);
                        }
                        else
                        {
                            Menu();
                        }
                    }
                    break;
            }
        }

        public static Boolean isAlphaNumeric(string strToCheck)
        {
            Regex rg = new Regex(@"^[a-zA-Z0-9\s,]*$");
            return rg.IsMatch(strToCheck);
        }

        private static string readLineWithCancel(bool main = false, int moveToMenu = 0)
        {
            string result = null;

            StringBuilder buffer = new StringBuilder();

            //The key is read passing true for the intercept argument to prevent
            //any characters from displaying when the Escape key is pressed.
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter && info.Key != ConsoleKey.Escape)
            {
                if (info.Key != ConsoleKey.Backspace && isAlphaNumeric(info.KeyChar.ToString()))
                {
                    Console.Write(info.KeyChar);
                    buffer.Append(info.KeyChar);
                    info = Console.ReadKey(true);
                }
                else
                {
                    if (buffer.Length != 0)
                    {
                        buffer.Length--;
                        Console.Write("\b \b");
                    }
                    else
                    {
                        buffer.Length = 0;
                    }
                    info = Console.ReadKey(true);
                }

            }
            if (info.Key == ConsoleKey.Escape)
            {
                if (main)
                {
                    Menu(false, false);
                }
                else
                {
                    Menu(true, false, moveToMenu);
                }
            }

            if (info.Key == ConsoleKey.Enter)
            {
                result = buffer.ToString();
            }

            return result;
        }

        static List<int> SNPRun(SNP_Network network)
        {
            List<int> allOutputs = new List<int>();
            SNP_Network clone = GenerateIdenticalNetwork(network);
            clone.CurrentOutput = 0;
            for (int i = 0; i < stepRepetition; i++)
            {
                clone = GenerateIdenticalNetwork(network);
                while (clone.IsClear == false && clone.GlobalTimer < maxSteps)
                {
                    stepThrough(clone);
                }
                allOutputs.AddRange(clone.OutputSet);
                if (allOutputs.Count == 0 && (i > 5))
                {
                    //Console.WriteLine("No outputs provided");
                    break;
                }
            }
            //allOutputs = allOutputs.Distinct().ToList();
            allOutputs.Sort();
            return allOutputs;
        }

        static void stepThrough(SNP_Network network)
        {
            network.Spike(network);
        }
        // Create new network with the List of new rules.
        // Note that this is a NON-GENERIC way of implementing this method, as I know exactly how many neurons this network will have.

        static string GenerateRandomExpression(List<string> templates, Random random)
        {
            int templateIndex = random.Next(0, templates.Count);
            string randomSpikeAmount = new string('a', random.Next(1, maxExpSize));
            string randomSpikeGroupingAmount = new string('a', random.Next(1, maxExpSize));
            string generatedExpression = templates[templateIndex].Replace("x", randomSpikeAmount);
            generatedExpression = generatedExpression.Replace("y", randomSpikeGroupingAmount);
            return generatedExpression;
        }

        static void LoadDefaultConfiguration()
        {
            maxSteps = 50;
            stepRepetition = 50;
            populationSize = 50;
            mutationRate = 0.1f;
            maximumGenerations = 25;
            testBestFitness = 5;
            expectedSet = new List<int>() { 2, 4, 6, 8, 10, 12, 14, 16 };
            experimentalRules = true;
        }

        static void UpdateGA(GeneticAlgorithm ga)
        {
            for (int i = 0; i < maximumGenerations; i++)
            {
                Console.WriteLine("Running Generation {0}", i);
                ga.NewGeneration((populationSize / 10));
                if (ga.BestFitness >= 0.985)
                {
                    Console.WriteLine("Testing the best fitness for repeated success.");
                    if (TestBestNetwork(ga.BestGenes, ga.BestFitness))
                    {
                        Console.WriteLine("Fitness over 0.985, stopping . . .");
                        break;
                    }
                }
            }
        }

        static bool TestBestNetwork(SNP_Network bestNetwork, float bestGAFitness)
        {
            float bestFitness = 0;
            for (int i = 0; i <= testBestFitness; i++)
            {
                float currentFitness = TestFitnessFunction(SNPRun(bestNetwork));
                bestFitness = (currentFitness > bestFitness) ? currentFitness : bestFitness;
            }
            return (bestFitness > bestGAFitness);
        }



        private static float FitnessFunction(int index)
        {
            /* If a number is in output and in target then it is a true positive
             * If a number is in output but not in target then it is a false positive 
             * If a number is not in output but is in the target then it is a false negative
             * If a number is not in output and not in the target then it is a true negative
             * 
             * We will never deal in True Negative members as the numbers will always be in the target
             * 
             * Sensitivity = number of true positives / number of true positives + number of false negatives
             * 
             * Specificity = number of true negatives / number of true negatives + number false positives. 
             * 
             * Precision (Positive predictive value) = The number of true positives / number of true positives + false positives 
             * 
             * We will never be able to check for specificity but we can work out the sensitivity and use it for our fitness function.
             * A higher sensitivity is better, as it means the amount of false negatives is lower. 
             * We can check for sensitivity and precision.
             * 
             */
            float tp = 0, fp = 0;
            DNA dna = ga.Population[index];
            List<int> output = SNPRun(dna.Genes);
            List<int> tpFound = new List<int>();
            for (int i = 0; i < output.Count(); i++)
            {
                foreach (int value in expectedSet)
                {
                    if (expectedSet.Contains(output[i]))
                    {
                        tp++;
                        if (!tpFound.Contains(output[i]))
                        {
                            tpFound.Add(output[i]);
                        }
                    }
                    else
                    {
                        fp++;
                    }
                }
            }
            if (output.Count > 0)
            {
                tp = (tp > expectedSet.Count) ? (tp - expectedSet.Count) / (output.Count - expectedSet.Count) : 0;
                fp = (fp > expectedSet.Count) ? (fp - expectedSet.Count) / (output.Count - expectedSet.Count) : 0;
                float scaledFitness = 0, targetCount = tpFound.Count, tpCount = expectedSet.Count;
                if (tpFound.Count != 0)
                {
                    scaledFitness = targetCount / tpCount;
                }
                float fn = expectedSet.Except(output).Count();
                float sensitivity = (tp / (tp + fn));
                float precision = tp / (tp + fp);
                float fitness = ((2 * tp) / ((2 * tp) + fp + fn)) * scaledFitness;
                if (fitness > 0.9)
                {
                    output = output.Distinct().ToList();
                }
                return fitness;
            }
            else
            {
                return 0;
            }
        }

        // generate a completely random network
        private static SNP_Network GenerateIdenticalNetwork(SNP_Network network)
        {
            int neuronAmount = network.Neurons.Count;
            int outputNeuron = 0;
            for (int i = 0; i < neuronAmount; i++)
            {
                if (network.Neurons[i].IsOutput)
                {
                    outputNeuron = i;
                }
            }
            List<Neuron> neurons = new List<Neuron>();
            for (int i = 0; i < neuronAmount; i++)
            {
                int ruleAmount = network.Neurons[i].Rules.Count;
                List<Rule> rules = new List<Rule>();
                for (int j = 0; j < ruleAmount; j++)
                {
                    rules.Add(new Rule(network.Neurons[i].Rules[j].RuleExpression, network.Neurons[i].Rules[j].Delay, network.Neurons[i].Rules[j].Fire));
                }
                List<int> connections = new List<int>();
                int connectionCount = network.Neurons[i].Connections.Count;
                for (int k = 0; k < connectionCount; k++)
                {
                    connections.Add(network.Neurons[i].Connections[k]);
                }
                neurons.Add(new Neuron(rules, network.Neurons[i].SpikeCount, connections, (i == outputNeuron)));
            }
            return new SNP_Network(neurons);
        }

        // Generate a completely random network
        private static SNP_Network GenerateNewRandomNetwork()
        {
            int neuronAmount = random.Next(2, 8);
            int outputNeuron = random.Next(1, neuronAmount + 1);
            List<Neuron> neurons = new List<Neuron>();
            for (int i = 0; i < neuronAmount; i++)
            {
                int ruleAmount = random.Next(1, 4);
                List<Rule> rules = new List<Rule>();
                for (int j = 0; j < ruleAmount; j++)
                {
                    bool next = (random.Next(0, 2) == 1);
                    rules.Add(new Rule(GenerateRandomExpression(acceptedRegex, random), random.Next(0, 2), next));
                }
                List<int> connections = new List<int>();
                for (int k = 0; k < neuronAmount; k++)
                {
                    // add at least one connection
                    if (connections.Count == 0)
                    {
                        int randomFirstConnection = random.Next(1, neuronAmount + 1);
                        while (randomFirstConnection == (i + 1))
                        {
                            randomFirstConnection = random.Next(1, neuronAmount + 1);
                        }
                        connections.Add(randomFirstConnection);
                    }
                    else if (i != k && !(connections.Contains(k + 1)))
                    {
                        if (random.Next(0, 2) == 1)
                        {
                            connections.Add(k + 1);
                        }
                    }
                }
                connections.Sort();
                neurons.Add(new Neuron(rules, new string('a', random.Next(0, maxExpSize + 1)), connections, (i + 1 == outputNeuron)));
            }
            return new SNP_Network(neurons);
        }

        // Generate new random expressions for a network we know work
        private static SNP_Network CreateNewRandomEvensNetwork()
        {
            return new SNP_Network(new List<Neuron>() {
                      new Neuron(new List<Rule>(){
                          new Rule(GenerateRandomExpression(acceptedRegex, random),0,true),
                          new Rule(GenerateRandomExpression(acceptedRegex, random),0,false)
                      }, "aa", new List<int>() {4}, false),
                       new Neuron(new List<Rule>() {
                          new Rule(GenerateRandomExpression(acceptedRegex, random),0,true),
                          new Rule(GenerateRandomExpression(acceptedRegex, random),0,false)
                      }, "aa", new List<int>() {5}, false),
                       new Neuron(new List<Rule>() {
                          new Rule(GenerateRandomExpression(acceptedRegex, random),0,true),
                          new Rule(GenerateRandomExpression(acceptedRegex, random),0,false)
                      }, "aa", new List<int>() {6}, false),
                        new Neuron(new List<Rule>() {
                          new Rule(GenerateRandomExpression(acceptedRegex, random),1,true),
                          new Rule(GenerateRandomExpression(acceptedRegex, random),0,true)
                      }, "", new List<int>() {1, 2, 3, 7}, false),
                         new Neuron(new List<Rule>() {
                          new Rule(GenerateRandomExpression(acceptedRegex, random),0,true),
                      }, "", new List<int>() {1, 2, 7}, false),
                         new Neuron(new List<Rule>() {
                          new Rule(GenerateRandomExpression(acceptedRegex, random),0,true),
                      }, "", new List<int>() { 3, 7}, false),
                         new Neuron(new List<Rule>() {
                          new Rule(GenerateRandomExpression(acceptedRegex, random),0,true),
                          new Rule(GenerateRandomExpression(acceptedRegex, random),0,false)
                      }, "aa", new List<int>() { }, true),
                  });
        }

        private static SNP_Network CreateNewRandomNormalNetwork()
        {
            return new SNP_Network(new List<Neuron>() {
                   new Neuron(new List<Rule>(){
                    new Rule(GenerateRandomExpression(acceptedRegex, random),0,true),
                    new Rule(GenerateRandomExpression(acceptedRegex, random),0,false)
                }, "aa", new List<int>() {2, 3, 4}, false),
                   new Neuron(new List<Rule>() {
                    new Rule(GenerateRandomExpression(acceptedRegex, random),0,true),
                    new Rule(GenerateRandomExpression(acceptedRegex, random),1,false)
                }, "aa", new List<int>() {1,3,4}, false),
                   new Neuron(new List<Rule>() {
                    new Rule(GenerateRandomExpression(acceptedRegex, random),0,true),
                    new Rule(GenerateRandomExpression(acceptedRegex, random),1,true)
                }, "aa", new List<int>() {1,2,4}, false),
                   new Neuron(new List<Rule>() {
                    new Rule(GenerateRandomExpression(acceptedRegex, random),0,true),
                    new Rule(GenerateRandomExpression(acceptedRegex, random),0,false)
                }, "aa",new List<int>() { }, true),
            });

        }

        //Natural numbers network original 
        static SNP_Network CreateNaturalNumbersNetwork()
        {
            return new SNP_Network(new List<Neuron>() {
                   new Neuron(new List<Rule>(){
                    new Rule("aa",0,true),
                    new Rule("a",0,false)
                }, "aa", new List<int>() {2, 3, 4}, false),
                   new Neuron(new List<Rule>() {
                    new Rule("aa",0,true),
                    new Rule("a",1,false)
                }, "aa", new List<int>() {1,3,4}, false),
                   new Neuron(new List<Rule>() {
                    new Rule("aa",0,true),
                    new Rule("aa",1,true)
                }, "aa", new List<int>() {1,2,4}, false),
                   new Neuron(new List<Rule>() {
                    new Rule("aa",0,true),
                    new Rule("aaa",0,false)
                }, "aa",new List<int>() { }, true),
            });
        }

        // Even numbers network original
        static SNP_Network CreateEvenNumbersNetwork()
        {
            return new SNP_Network(new List<Neuron>() {
                      new Neuron(new List<Rule>(){
                          new Rule("aa",0,true),
                          new Rule("a",0,false)
                      }, "aa", new List<int>() {4}, false),
                       new Neuron(new List<Rule>() {
                          new Rule("aa",0,true),
                          new Rule("a",0,false)
                      }, "aa", new List<int>() {5}, false),
                       new Neuron(new List<Rule>() {
                          new Rule("aa",0,true),
                          new Rule("a",0,false)
                      }, "aa", new List<int>() {6}, false),
                        new Neuron(new List<Rule>() {
                          new Rule("a",1,true),
                          new Rule("a",0,true)
                      }, "", new List<int>() {1, 2, 3, 7}, false),
                         new Neuron(new List<Rule>() {
                          new Rule("a",0,true),
                      }, "", new List<int>() {1, 2, 7}, false),
                         new Neuron(new List<Rule>() {
                          new Rule("a",0,true),
                      }, "", new List<int>() { 3, 7}, false),
                         new Neuron(new List<Rule>() {
                          new Rule("aa",0,true),
                          new Rule("aaa",0,false)
                      }, "aa", new List<int>() { }, true),
                  });
        }

        /*  Natural Numbers fully auto generated
           return new SNP_Network(new List<Neuron>() {
                   new Neuron(new List<Rule>(){
                       new Rule(networkConfiguration[0]["Expression"], networkConfiguration[0]["Delay"], networkConfiguration[0]["Fire"]),
                       new Rule(networkConfiguration[1]["Expression"], networkConfiguration[1]["Delay"], networkConfiguration[1]["Fire"]),
                }, "aa", new List<int>() {2, 3, 4}, false),
                   new Neuron(new List<Rule>() {
                       new Rule(networkConfiguration[2]["Expression"], networkConfiguration[2]["Delay"], networkConfiguration[2]["Fire"]),
                       new Rule(networkConfiguration[3]["Expression"], networkConfiguration[3]["Delay"], networkConfiguration[3]["Fire"]),
                }, "aa", new List<int>() {1,3,4}, false),
                   new Neuron(new List<Rule>() {
                       new Rule(networkConfiguration[4]["Expression"], networkConfiguration[4]["Delay"], networkConfiguration[4]["Fire"]),
                       new Rule(networkConfiguration[5]["Expression"], networkConfiguration[5]["Delay"], networkConfiguration[5]["Fire"]),
                }, "aa", new List<int>() {1,2,4}, false),
                   new Neuron(new List<Rule>() {
                       new Rule(networkConfiguration[6]["Expression"], networkConfiguration[6]["Delay"], networkConfiguration[6]["Fire"]),
                       new Rule(networkConfiguration[7]["Expression"], networkConfiguration[7]["Delay"], networkConfiguration[7]["Fire"]),
                }, "aa",new List<int>() { }, true),
            }); */

        // Testing fitness without GA implementation
        private static float TestFitnessFunction(List<int> output)
        {
            float tp = 0, fp = 0;
            List<int> tpFound = new List<int>();
            for (int i = 0; i < output.Count(); i++)
            {
                foreach (int value in expectedSet)
                {
                    if (expectedSet.Contains(output[i]))
                    {
                        tp++;
                        if (!tpFound.Contains(output[i]))
                        {
                            tpFound.Add(output[i]);
                        }
                    }
                    else
                    {
                        fp++;
                    }
                }
            }
            if (output.Count > 0)
            {
                // normalize
                tp = (tp - expectedSet.Count) / (output.Count - expectedSet.Count);
                fp = (fp > expectedSet.Count) ? (fp - expectedSet.Count) / (output.Count - expectedSet.Count) : 0;
                float scaledFitness = 0, targetCount = tpFound.Count, tpCount = expectedSet.Count;
                if (tpFound.Count != 0)
                {
                    scaledFitness = targetCount / tpCount;
                }
                float fn = expectedSet.Except(output).Count();
                float sensitivity = (tp / (tp + fn));
                float precision = tp / (tp + fp);
                float fitness = ((2 * tp) / ((2 * tp) + fp + fn)) * scaledFitness;
                output = output.Distinct().ToList();
                output.Sort();
                return fitness;
            }
            else
            {
                return 0;
            }

        }

    }
}