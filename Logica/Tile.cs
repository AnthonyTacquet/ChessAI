using Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica
{
    public class Tile
    {
        public bool IsValidMove { get; set; } = false;
        public Position Position { get; set; }
        public ChessColor Color { get; set; }
        public Tile(Position position, ChessColor color) 
        { 
            this.Position = position;
            this.Color = color;
        }
    }
}
