using LINQtoCSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Global
{
    [Serializable]
    public class Synapse
    {
        [CsvColumn(Name = "neuron1", FieldIndex = 1)]
        public Neuron Neuron1 { get; set; }

        private float val;
        [CsvColumn(Name = "value", FieldIndex = 2)]
        public float Value
        {
            get { return val; }
            set
            {
                if (value < 0 || value > 1)
                    val = 0; // Can implement Sigmoid function
                val = value;
            }
        }

        [CsvColumn(Name = "neuron2", FieldIndex = 3)]
        public Neuron Neuron2 { get; set; }

        public Synapse() { }

        public Synapse(Neuron neuron1, Neuron neuron2, float value) 
        { 
            Neuron1 = neuron1;
            Neuron2 = neuron2;
            Value = value;
        }

        public static Synapse FromCsv(string csvLine)
        {
            string[] values = csvLine.Split('\"');
            if (values.Length == 7) 
            {
                Synapse synapse = new Synapse();
                string[] neur1 = Convert.ToString(values[1]).Replace("\"", "").Split(',');
                string[] neur2 = Convert.ToString(values[5]).Replace("\"", "").Split(',');
                synapse.Neuron1 = new Neuron(neur1[0], float.Parse(neur1[1]));
                synapse.Value = (float)Convert.ToDouble(values[3].Replace("\"", ""));
                synapse.Neuron2 = new Neuron(neur2[0], float.Parse(neur2[1])); ;
                return synapse;
            } else {
                Synapse synapse = new Synapse();
                string[] neur1 = Convert.ToString(values[1]).Replace("\"", "").Split(',');
                string[] neur2 = Convert.ToString(values[3]).Replace("\"", "").Split(',');
                synapse.Neuron1 = new Neuron(neur1[0], float.Parse(neur1[1]));
                synapse.Value = (float)Convert.ToDouble(values[2].Replace("\"", "").Replace(",", ""));
                synapse.Neuron2 = new Neuron(neur2[0], float.Parse(neur2[1])); ;
                return synapse;
            }
        }


        public override string ToString()
        {
            return $"{Neuron1} - {Neuron2}: {Value}";
        }
    }
}
