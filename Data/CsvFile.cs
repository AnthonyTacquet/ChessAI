using Global;
using LINQtoCSV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Data
{
    public class CsvFile
    {

        public List<Piece> ReadPieces()
        {
            string path = "./Resources/Pieces.csv";
            CsvFileDescription csvFileDescription = new CsvFileDescription
            {
                FirstLineHasColumnNames = true,
                IgnoreUnknownColumns = true,
                SeparatorChar = ',',
                UseFieldIndexForReadingData = false,
            };

            var csvContect = new CsvContext();
            var pieces = csvContect.Read<Piece>(path, csvFileDescription);
            List<Piece> pieces1 = pieces.ToList();
            List<Piece> newList = new List<Piece>();

            foreach (Piece piece1 in pieces1)
            {
                if (piece1.Name.Contains("pawn"))
                    newList.Add(new Pawn(piece1.Name, piece1.Captured, piece1.Position, piece1.Color));
                if (piece1.Name.Contains("rook"))
                    newList.Add(new Rook(piece1.Name, piece1.Captured, piece1.Position, piece1.Color));
                if (piece1.Name.Contains("bishop"))
                    newList.Add(new Bishop(piece1.Name, piece1.Captured, piece1.Position, piece1.Color));
                if (piece1.Name.Contains("knight"))
                    newList.Add(new Knight(piece1.Name, piece1.Captured, piece1.Position, piece1.Color));
                if (piece1.Name.Contains("king"))
                    newList.Add(new King(piece1.Name, piece1.Captured, piece1.Position, piece1.Color));
                if (piece1.Name.Contains("queen"))
                    newList.Add(new Queen(piece1.Name, piece1.Captured, piece1.Position, piece1.Color));
            }

            return newList;
        }

        public List<Neuron> ReadNeuron(string file)
        {
            string path = $"./Resources/{file}.csv";
            CsvFileDescription csvFileDescription = new CsvFileDescription
            {
                FirstLineHasColumnNames = true,
                IgnoreUnknownColumns = true,
                SeparatorChar = ',',
                UseFieldIndexForReadingData = false,
            };

            var csvContect = new CsvContext();
            var neurons = csvContect.Read<Neuron>(path, csvFileDescription);
            List<Neuron> neurons1 = neurons.ToList();

            return neurons1;
        }

        public List<Synapse> ReadSynapse(string file)
        {
            string path = $"./Resources/{file}.csv";
            List<Synapse> values = File.ReadAllLines(path)
                               .Skip(1)
                               .Select(v => Synapse.FromCsv(v))
                               .ToList();

            return values;
        }







        public void WriteNeuron(List<Neuron> neuron, string name)
        {
            string path = $"./Resources/{name}.csv";
            CsvFileDescription csvFileDescription = new CsvFileDescription
            {
                FirstLineHasColumnNames = true,
                SeparatorChar = ',',
            };
            var csvContect = new CsvContext();
            csvContect.Write<Neuron>(neuron, path, csvFileDescription);


        }

        public void WriteSynapse(List<Synapse> synapses, string name)
        {
            string path = $"./Resources/{name}.csv";
            CsvFileDescription csvFileDescription = new CsvFileDescription
            {
                FirstLineHasColumnNames = true,
                SeparatorChar = ',',
            };
            var csvContect = new CsvContext();
            csvContect.Write<Synapse>(synapses, path, csvFileDescription);


        }

        
    }
}
