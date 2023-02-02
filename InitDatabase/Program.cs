using Global;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using LINQtoCSV;
using Data;
using Org.BouncyCastle.Asn1.X500;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json.Linq;

namespace InitDatabase
{
    public class Program
    {
        CsvFile file = new CsvFile();
        public delegate float DelegateFloat(float num1, float num2); //declaring a delegate

        static void Main(string[] args)
        {
            Program program= new Program();
            CsvFile file = new CsvFile();
            /*
            List<Neuron> neuronList = file.ReadNeuron("NeuronOutput");
            neuronList.OrderByDescending(e => e.Value);
            foreach (Neuron neuron in neuronList)
            {
                Console.WriteLine(neuron.ToString());
            }*/

            /*
            Console.WriteLine("3ToOut");
            program.CreateSynapse3thToOut();
            Console.WriteLine("inTo2nd");
            program.CreateSynapseInTo2nd();
            Console.WriteLine("2ndTo3th");
            program.CreateSynapse2ndTo3th();

            program.InputLayer();
            program.SecondLayer();
            program.ThirdLayer();
            program.OutputLayer();
            */
            DelegateFloat del = AddNum;
            Console.WriteLine(del(0.5F, 0.5F));

            //List<Synapse> synapses = file.ReadSynapse("SynapseInTo2nd");
            //Console.WriteLine(synapses.Find(e => e.Neuron1.Name.Equals("c7_12")).ToString());
        }

        static float AddNum(float num1, float num2)
        {
            return num1 + num2;
        }

        public void CreateSynapse3thToOut()
        {
            List<Neuron> neuronsThird = file.ReadNeuron("NeuronThird");
            List<Neuron> neuronsOut = file.ReadNeuron("NeuronOutput");
            List<Synapse> synapses = new List<Synapse>();
            Console.WriteLine(neuronsOut.Count);

            int counter = 0;
            Random rnd = new Random();

            foreach (Neuron neuron in neuronsThird)
            {
                /*
                for (int i = 0; i < 2; i++)
                {
                    int number = rnd.Next(1, 512);
                    string name = $"3th_layer_n{number}";

                    Random random = new Random();
                    double value = random.NextDouble();
                    if (i == 1)
                        value = -value;

                    Neuron neuron1 = neuronsThird.Find(e => e.Name.Equals(name));
                    Synapse synapse = new Synapse(neuron1, neuron, ((float)value));
                    synapses.Add(synapse);
                }
                */
                Random random = new Random();

                foreach (Neuron neuron1 in neuronsOut)
                {
                    double value = random.NextDouble();
                    Synapse synapse = new Synapse(neuron, neuron1, (float)value);
                    synapses.Add(synapse);
                    counter++;
                }
                
            }
            file.WriteSynapse(synapses, "Synapse3thToOut");
        }

        public void CreateSynapse2ndTo3th()
        {
            List<Neuron> neurons2nd = file.ReadNeuron("NeuronSecond");
            List<Neuron> neurons3th = file.ReadNeuron("NeuronThird");
            List<Synapse> synapses = new List<Synapse>();
            int counter = 0;
            Random rnd = new Random();

            foreach (Neuron neuron in neurons3th)
            {
                /*
                for (int i = 0; i < 2; i++)
                {
                    int number = rnd.Next(1, 512);
                    string name = $"2nd_layer_n{number}";

                    Random random = new Random();
                    double value = random.NextDouble();
                    if (i == 1)
                        value = -value;

                    Neuron neuron1 = neurons2nd.Find(e => e.Name.Equals(name));
                    Synapse synapse = new Synapse(neuron1, neuron, ((float)value));
                    synapses.Add(synapse);
                }
                */
                // Deep neural network
                Random random = new Random();

                foreach (Neuron neuron1 in neurons2nd)
                {
                    double value = random.NextDouble();
                    Synapse synapse = new Synapse(neuron1, neuron, (float)value);
                    synapses.Add(synapse);
                    counter++;
                }
                
            }
            file.WriteSynapse(synapses, "Synapse2ndTo3th");
        }

        public void CreateSynapseInTo2nd()
        {
            List<Neuron> neuronsIn = file.ReadNeuron("NeuronInput");
            List<Neuron> neurons2nd = file.ReadNeuron("NeuronSecond");
            List<Synapse> synapses = new List<Synapse>();
            Console.WriteLine(neuronsIn.Count);

            Console.WriteLine(neurons2nd.Count);

            int counter = 0;
            Random rnd = new Random();

            foreach (Neuron neuron in neuronsIn)
            {
                /*
                for (int i = 0; i < 2; i++)
                {
                    int number = rnd.Next(1, 512);
                    string name = $"2nd_layer_n{number}";

                    Random random = new Random();
                    double value = random.NextDouble();
                    if (i == 1)
                        value = -value;

                    Neuron neuron1 = neurons2nd.Find(e => e.Name.Equals(name));
                    Synapse synapse = new Synapse(neuron, neuron1, ((float)value));
                    synapses.Add(synapse);
                }
                */
                Random random = new Random();
                foreach (Neuron neuron1 in neurons2nd)
                {
                    double value = random.NextDouble();
                    Synapse synapse = new Synapse(neuron, neuron1, (float)value);
                    synapses.Add(synapse);
                    counter++;
                }
            }
            file.WriteSynapse(synapses, "SynapseInTo2nd");
        }

        public void OutputLayer() // Add directions
        {
            List<Neuron> neurons = new List<Neuron>();
            Neuron neuron;
            int counter = 0;
            string name;

            for (int j = 0; j < 2; j++) // ROOK
            {
                for (int z = 0; z < 7; z++)
                {
                    char[] array = { 'N', 'E', 'S', 'W'};
                    foreach (char letter in array)
                    {
                        name = $"rook_{j + 1}_{letter}{z + 1}";

                        neuron = new Neuron(name, 0);
                        neurons.Add(neuron);

                        Console.WriteLine(name);
                        counter++;
                    }
                }

            }

            for (int j = 0; j < 2; j++) // BISHOP
            {
                for (int z = 0; z < 7; z++)
                {
                    string[] array = { "NE", "SE", "SW", "NW" };
                    foreach (string letter in array)
                    {
                        name = $"bishop_{j + 1}_{letter}{z + 1}";

                        neuron = new Neuron(name, 0);
                        neurons.Add(neuron);

                        Console.WriteLine(name);
                        counter++;
                    }
                }
            }

            for (int j = 0; j < 2; j++) // KNIGHT
            {

                string[] array = { "N1", "N2", "E1", "E2", "S1", "S2", "W1", "W2" };
                foreach (string letter in array)
                {
                    name = $"knight_{j + 1}_{letter}";

                    neuron = new Neuron(name, 0);
                    neurons.Add(neuron);

                    Console.WriteLine(name);
                    counter++;
                }

            }

            for (int j = 0; j < 8; j++) // PAWN
            {
                string[] array = { "N1", "NW", "NE", "N2"};
                foreach (string letter in array)
                {
                    name = $"pawn_{j + 1}_{letter}";

                    neuron = new Neuron(name, 0);
                    neurons.Add(neuron);

                    Console.WriteLine(name);
                    counter++;
                }
            }


            string[] array1 = { "N", "NE", "E", "SE", "S", "SW", "W", "NW"};
            foreach (string letter in array1)
            {
                name = $"king_{letter}";

                neuron = new Neuron(name, 0);
                neurons.Add(neuron);

                Console.WriteLine(name);
                counter++;
            }

            for (int z = 0; z < 7; z++)
            {
                string[] array2 = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
                foreach (string letter in array2)
                {
                    name = $"queen_{letter}{z + 1}";

                    neuron = new Neuron(name, 0);
                    neurons.Add(neuron);

                    Console.WriteLine(name);
                    counter++;
                }
            }

            name = $"castle_E";
            neuron = new Neuron(name, 0);
            neurons.Add(neuron);

            Console.WriteLine(name);
            counter++;

            name = $"castle_W";
            neuron = new Neuron(name, 0);
            neurons.Add(neuron);

            Console.WriteLine(name);
            counter++;

            Console.WriteLine(counter);
            file.WriteNeuron(neurons, "NeuronOutput");

        }

        public void ThirdLayer()
        {
            List<Neuron> neurons = new List<Neuron>();
            for (int i = 0; i < 512; i++)
            {
                string name = $"3th_layer_n{i + 1}";
                Neuron neuron = new Neuron(name, 0);
                neurons.Add(neuron);
                Console.WriteLine(name);
            }
            file.WriteNeuron(neurons, "NeuronThird");
        }

        public void SecondLayer()
        {
            List<Neuron> neurons = new List<Neuron>();
            for (int i = 0; i < 512; i++)
            {
                string name = $"2nd_layer_n{i + 1}";
                Neuron neuron = new Neuron(name, 0);
                neurons.Add(neuron);
                Console.WriteLine(name);
            }
            file.WriteNeuron(neurons, "NeuronSecond");
        }


        public void InputLayer()
        {
            char[] letters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
            List<Neuron> neurons = new List<Neuron>();
            int counter = 0;

            string name;
            int letter = 0;
            int times = 0;

            for (int i = 0; i < 64; i++)
            {
                if (i % 8 == 0 && i != 0)
                {
                    letter++;
                    times++;
                }

                for (int j = 0; j < 12; j++)
                {
                    counter++;
                    name = $"{letters[letter]}{i + 1 - (8 * times)}_{j + 1}";

                    Neuron neuron = new Neuron(name, 0);
                    neurons.Add(neuron);
                    Console.WriteLine(name);
                }
            }

            file.WriteNeuron(neurons, "NeuronInput");

            Console.WriteLine(counter);
        }
    }
}
