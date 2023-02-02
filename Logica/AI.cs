using Data;
using Global;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Logica
{
    public delegate float DelegateFloat(float num1, float num2); //declaring a delegate
    public class AI
    {
        Task Task;
        private ChessColor ActiveColor;
        private ChessColor AIColor;
        private List<Piece> pieces;
        private CsvFile file = new CsvFile();
        private List<Neuron> results = new List<Neuron>();
        public AI(ChessColor ativeColor, ChessColor AIColor, List<Piece> pieces)
        {
            this.ActiveColor = ativeColor;
            this.AIColor = AIColor;
            this.pieces = pieces;

        }

        public void BadMove(Neuron neuron)
        {
            float points = -0.002F;
            BackTrack(neuron, points);
        }

        public void BadMove(List<Neuron> neurons)
        {
            float points = -0.002F;
            BackTrack(neurons, points);
        }

        public void GoodMove(Neuron neuron)
        {
            float points = 0.001F;
            BackTrack(neuron, points);
        }

        public void PieceTaken(Neuron neuron)
        {
            if (neuron == null)
                return;
            float points = -0.002F;
            BackTrack(neuron, points);
        }

        public void OverTook(Neuron neuron)
        {
            float points = 0.005F;
            BackTrack(neuron, points);

        }

        public void BackTrack(List<Neuron> neurons, float points)
        {
            Reset();
            Debug.WriteLine("Backing");
            Debug.WriteLine("Output");

            List<Synapse> synapses3thToOut = file.ReadSynapse("Synapse3thToOut");
            List<Neuron> neuronsThirdChange = new List<Neuron>();

            foreach (Neuron neuron in neurons)
            {
                neuronsThirdChange.AddRange(BackOutput(synapses3thToOut, neuron, points));
            }

            Debug.WriteLine($"Third {neuronsThirdChange.Count}");

            List<Synapse> synapses2ndTo3th = file.ReadSynapse("Synapse2ndTo3th");
            List<Neuron> neuronsSecondChange = BackThird(synapses2ndTo3th, neuronsThirdChange, points);

            Debug.WriteLine($"Second {neuronsSecondChange.Count}");

            List<Synapse> synapsesInTo2nd = file.ReadSynapse("SynapseInTo2nd");
            BackSecond(synapsesInTo2nd, neuronsSecondChange, points);

            Debug.WriteLine("Write");
            // Write
            Write(synapsesInTo2nd, synapses2ndTo3th, synapses3thToOut);
        }

        public void BackTrack(Neuron neuron, float points)
        {
            Reset();
            Debug.WriteLine("Backing");
            Debug.WriteLine("Output");

            List<Synapse> synapses3thToOut = file.ReadSynapse("Synapse3thToOut");
            List<Neuron> neuronsThirdChange = BackOutput(synapses3thToOut, neuron, points);

            Debug.WriteLine($"Third {neuronsThirdChange.Count}");

            List<Synapse> synapses2ndTo3th = file.ReadSynapse("Synapse2ndTo3th");
            List<Neuron> neuronsSecondChange = BackThird(synapses2ndTo3th, neuronsThirdChange, points);

            Debug.WriteLine($"Second {neuronsSecondChange.Count}");

            List<Synapse> synapsesInTo2nd = file.ReadSynapse("SynapseInTo2nd");
            BackSecond(synapsesInTo2nd, neuronsSecondChange, points);

            Debug.WriteLine("Write");
            // Write
            Write(synapsesInTo2nd, synapses2ndTo3th, synapses3thToOut);
        }

        private float MinMax(float val)
        {
            if (val.IsLessThan(-1))
                return -0.9999F;
            if (val.IsGreaterThan(1))
                return 0.9999F;
            return val;
        }

        private void BackSecond(List<Synapse> synapsesInTo2nd, List<Neuron> neurons, float val)
        {
            List<Neuron> noDups = neurons.DistinctBy(e => e.Name).ToList();
            List<Synapse> smallList = synapsesInTo2nd.FindAll(e => noDups.Contains(e.Neuron2));

            DelegateFloat delAddNumber = AddNum;

            foreach (Synapse synapse in smallList)
            {
                float synapVal = delAddNumber(synapse.Value, val);
                synapse.Value = MinMax(synapVal);
            }

        }

        static float AddNum(float num1, float num2)
        {
            return num1 + num2;
        }

        static float SubNum(float num1, float num2)
        {
            return num1 - num2;
        }

        private List<Neuron> BackThird(List<Synapse> synapses2ndTo3th, List<Neuron> neurons, float val)
        {
            List<Neuron> noDups = neurons.DistinctBy(e => e.Name).ToList();
            List<Synapse> smallList = synapses2ndTo3th.FindAll(e => noDups.Contains(e.Neuron2));

            foreach (Synapse synapse in smallList)
            {
                float synapVal = synapse.Value + val;
                synapse.Value = MinMax(synapVal);
            }
            //List<Neuron[]> list = smallList.Select(e => e.Neuron1).ToList().OrderByDescending(e => e.Value).Chunk(smallList.Count / 2).ToList();
            //if (list == null)
            return smallList.Select(e => e.Neuron1).ToList();
            //return list[0].ToList();
        }

        private List<Neuron> BackOutput(List<Synapse> synapses3thToOut, Neuron neuron, float val)
        {
            List<Synapse> smallList = synapses3thToOut.FindAll(e => e.Neuron2.Equals(neuron));

            foreach (Synapse synapse in smallList)
            {
                float synapVal = synapse.Value + val;
                synapse.Value = MinMax(synapVal);
            }
            //List<Neuron[]> list = smallList.Select(e => e.Neuron1).ToList().OrderByDescending(e => e.Value).Chunk(2).ToList();
            //if (list == null)
            return smallList.Select(e => e.Neuron1).ToList();
            //return list[0].ToList();

        }



        public void Start()
        {
            Debug.WriteLine("Start");

            List<Neuron> neuronsInput = file.ReadNeuron("NeuronInput");
            SetInputValues(neuronsInput);
            Debug.WriteLine("Input");

            List<Synapse> synapsesInTo2nd = file.ReadSynapse("SynapseInTo2nd");
            List<Neuron> neuronSecond = CalculateSecondLayer(synapsesInTo2nd, neuronsInput);
            Debug.WriteLine("Second");

            List<Synapse> synapses2ndTo3th = file.ReadSynapse("Synapse2ndTo3th");
            List<Neuron> neuronThird = CalculateThirdLayer(synapses2ndTo3th, neuronSecond);
            Debug.WriteLine("Third");

            List<Synapse> synapses3thToOut = file.ReadSynapse("Synapse3thToOut");
            List<Neuron> neuronOutput = CalculateOutputLayer(synapses3thToOut, neuronThird);
            Debug.WriteLine("Output");

            results = neuronOutput;
            // Write
            Write(neuronsInput, neuronSecond, neuronThird, neuronOutput, synapsesInTo2nd, synapses2ndTo3th, synapses3thToOut);
        }

        public void Write(List<Synapse> synapsesInTo2nd, List<Synapse> synapses2ndTo3th, List<Synapse> synapses3thToOut)
        {
            file.WriteSynapse(synapsesInTo2nd, "SynapseInTo2nd");
            file.WriteSynapse(synapses2ndTo3th, "Synapse2ndTo3th");
            file.WriteSynapse(synapses3thToOut, "Synapse3thToOut");
        }

        public void Write(List<Neuron> neuronsInput, List<Neuron> neuronSecond, List<Neuron> neuronThird, List<Neuron> neuronOutput, List<Synapse> synapsesInTo2nd, List<Synapse> synapses2ndTo3th, List<Synapse> synapses3thToOut)
        {
            file.WriteNeuron(neuronsInput, "NeuronInput");
            file.WriteNeuron(neuronSecond, "NeuronSecond");
            file.WriteNeuron(neuronThird, "NeuronThird");
            file.WriteNeuron(neuronOutput, "NeuronOutput");

            file.WriteSynapse(synapsesInTo2nd, "SynapseInTo2nd");
            file.WriteSynapse(synapses2ndTo3th, "Synapse2ndTo3th");
            file.WriteSynapse(synapses3thToOut, "Synapse3thToOut");
        }
        public List<Neuron> GetResults()
        {
            return results;
        }

        public void Reset()
        {
            List<Neuron> neuronsInput = file.ReadNeuron("NeuronInput");
            List<Neuron> neuronSecond = file.ReadNeuron("NeuronSecond");
            List<Neuron> neuronThird = file.ReadNeuron("NeuronThird");
            List<Neuron> neuronOutput = file.ReadNeuron("NeuronOutput");

            List<Synapse> synapsesInTo2nd = file.ReadSynapse("SynapseInTo2nd");
            List<Synapse> synapses2ndTo3th = file.ReadSynapse("Synapse2ndTo3th");
            List<Synapse> synapses3thToOut = file.ReadSynapse("Synapse3thToOut");

            neuronsInput.ForEach(e => e.Value = 0);
            neuronSecond.ForEach(e => e.Value = 0);
            neuronThird.ForEach(e => e.Value = 0);
            neuronOutput.ForEach(e => e.Value = 0);

            synapses2ndTo3th.ForEach(e => e.Neuron1.Value = 0);
            synapses2ndTo3th.ForEach(e => e.Neuron2.Value = 0);
            synapses3thToOut.ForEach(e => e.Neuron1.Value = 0);
            synapses3thToOut.ForEach(e => e.Neuron2.Value = 0);
            synapsesInTo2nd.ForEach(e => e.Neuron1.Value = 0);
            synapsesInTo2nd.ForEach(e => e.Neuron2.Value = 0);


            file.WriteNeuron(neuronsInput, "NeuronInput");
            file.WriteNeuron(neuronSecond, "NeuronSecond");
            file.WriteNeuron(neuronThird, "NeuronThird");
            file.WriteNeuron(neuronOutput, "NeuronOutput");

            file.WriteSynapse(synapsesInTo2nd, "SynapseInTo2nd");
            file.WriteSynapse(synapses2ndTo3th, "Synapse2ndTo3th");
            file.WriteSynapse(synapses3thToOut, "Synapse3thToOut");
        }

        private float CalculateSigmoid(float val)
        {
            return (1 / (1 + float.Pow(float.E, -val)));
        }
        private List<Neuron> CalculateOutputLayer(List<Synapse> synapses3thToOut, List<Neuron> neuronThird)
        {
            List<Neuron> second = new List<Neuron>();

            foreach (Synapse synapse in synapses3thToOut)
            {
                Neuron neuron = synapse.Neuron2;
                Neuron neuron1 = neuronThird.Find(e => e.Equals(synapse.Neuron1));
                if (neuron1 == null || neuron == null)
                    continue;

                //float begin = neuron.Value;
                //float val = neuron1.Value * synapse.Value + begin;
                //neuron.Value = CalculateSigmoid(val);
                float val = neuron1.Value * synapse.Value;

                if (second.Find(e => e.Equals(neuron)) == null)
                {
                    neuron.Value = CalculateSigmoid(val);
                    second.Add(neuron);
                }
                else
                {
                    Neuron newNeuron = second.Find(e => e.Equals(neuron));
                    newNeuron.Value = CalculateSigmoid(newNeuron.Value + val);
                }
            }
            return second;

        }

        private List<Neuron> CalculateThirdLayer(List<Synapse> synapses2ndTo3th, List<Neuron> neuronSecond)
        {
            List<Neuron> second = new List<Neuron>();

            foreach (Synapse synapse in synapses2ndTo3th)
            {
                Neuron neuron = synapse.Neuron2;
                Neuron neuron1 = neuronSecond.Find(e => e.Equals(synapse.Neuron1));
                if (neuron1 == null || neuron == null)
                    continue;

                float begin = neuron.Value;
                float val = neuron1.Value * synapse.Value + begin;
                neuron.Value = CalculateSigmoid(val);

                if (second.Find(e => e.Equals(neuron)) == null)
                {
                    neuron.Value = CalculateSigmoid(val);
                    second.Add(neuron);
                }
                else
                {
                    Neuron newNeuron = second.Find(e => e.Equals(neuron));
                    newNeuron.Value = CalculateSigmoid(newNeuron.Value + val);
                }
            }
            return second;

        }

        private List<Neuron> CalculateSecondLayer(List<Synapse> synapsesInTo2nd, List<Neuron> neuronsInput)
        {
            List<Neuron> second = new List<Neuron>();
            foreach (Synapse synapse in synapsesInTo2nd)
            {
                Neuron neuron = synapse.Neuron2;
                Neuron neuron1 = neuronsInput.Find(e => e.Equals(synapse.Neuron1));
                if (neuron1 == null || neuron == null)
                    continue;

                //float begin = neuron.Value;
                //float val = neuron1.Value * synapse.Value + begin;
                //neuron.Value = CalculateSigmoid(val);
                float val = neuron1.Value * synapse.Value;

                if (second.Find(e => e.Equals(neuron)) == null)
                {
                    neuron.Value = CalculateSigmoid(val);
                    second.Add(neuron);
                }
                else
                {
                    Neuron newNeuron = second.Find(e => e.Equals(neuron));
                    newNeuron.Value = CalculateSigmoid(newNeuron.Value + val);
                }
            }
            return second;

        }

        private void SetInputValues(List<Neuron> neurons)
        {
            foreach (Piece piece in pieces)
            {
                List<Neuron> newList = neurons.FindAll(e => e.Name.Contains(piece.Position.ToString().ToLower()));
                newList.OrderBy(e => e.Name);
                int[] array = GetPieceNumber(piece);
                for (int i = 1; i <= array.Length; i++)
                {
                    Neuron neuron = newList.Find(e => e.Name.Contains($"{piece.Position.ToString().ToLower()}_{i}"));
                    if (neuron != null)
                        neuron.Value = array[i - 1];
                }
            }
        }

        private int[] GetPieceNumber(Piece piece)
        {
            int[] array = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            if (piece is Pawn && piece.Color.Equals(AIColor))
                array[0] = 1;
            else if (piece is Pawn && !piece.Color.Equals(AIColor))
                array[6] = 1;
            else if (piece is Rook && piece.Color.Equals(AIColor))
                array[1] = 1;
            else if (piece is Rook && !piece.Color.Equals(AIColor))
                array[7] = 1;
            else if (piece is Bishop && piece.Color.Equals(AIColor))
                array[2] = 1;
            else if (piece is Bishop && !piece.Color.Equals(AIColor))
                array[8] = 1;
            else if (piece is Knight && piece.Color.Equals(AIColor))
                array[3] = 1;
            else if (piece is Knight && !piece.Color.Equals(AIColor))
                array[9] = 1;
            else if (piece is King && piece.Color.Equals(AIColor))
                array[4] = 1;
            else if (piece is King && !piece.Color.Equals(AIColor))
                array[10] = 1;
            else if (piece is Queen && piece.Color.Equals(AIColor))
                array[5] = 1;
            else if (piece is Queen && !piece.Color.Equals(AIColor))
                array[11] = 1;

            return array;
        }
    }
}
