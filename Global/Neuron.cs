using LINQtoCSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Interface;

namespace Global
{
    [Serializable]
    public class Neuron : Basic
    {
        [CsvColumn(Name = "name", FieldIndex = 1)]
        public string Name { get; set; }
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

        public Neuron() { }

        public Neuron(string name, float value) 
        { 
            this.Name = name;
            this.Value = value;
        }

        public override string ToString()
        {
            return $"{Name}, {Value}";
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
            if (obj is not Neuron)
                return false;
            Neuron neuron = (Neuron)obj;
            return neuron.Name.Equals(this.Name);
        }
    }
}
