using LINQtoCSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Global
{
    [Serializable]
    public class Piece
    {
        [CsvColumn(Name = "name", FieldIndex = 1)]
        public string Name { get; set; }
        [CsvColumn(Name = "captured", FieldIndex = 2, OutputFormat = "Bool")]
        public bool Captured { get; set; } = false;
        [CsvColumn(Name = "position", FieldIndex = 3)]
        public Position Position { get; set; }
        [CsvColumn(Name = "color", FieldIndex = 4)]
        public ChessColor Color { get; set; }
        [CsvColumn(Name = "moved", FieldIndex = 5, OutputFormat = "Bool")]
        public bool Moved { get; set; } = false;

        public Piece() { }
        public Piece(Piece piece)
        {
            this.Name = piece.Name;
            this.Captured = piece.Captured;
            this.Position = piece.Position;
            this.Color = piece.Color;
            this.Moved = piece.Moved;
        }


        public Piece(string Name, Position position, ChessColor chessColor)
        {
            this.Name = Name;
            this.Position = position;
            this.Color = chessColor;
        }

        public Piece(string Name, bool Captured, Position position, ChessColor chessColor)
        {
            this.Name = Name;
            this.Position = position;
            this.Color = chessColor;
            this.Captured = Captured;
        }

        public virtual ChessPieces GetPieceType()
        {
            return ChessPieces.NONE;
        }

        public override string ToString()
        {
            Func<string, string> convert = delegate (string input)
            { return input.ToUpper(); };

            string output = $"{Name}, {Captured}, {Position.ToString()}, {Color.ToString()}, {Moved}";
            return convert(output);
        }
    }
}
