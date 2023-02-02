using Global;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Database
    {
        DBConnection conn;
        public Database(DBConnection conn) 
        { 
            this.conn = conn;
        }

        public List<Piece> ReadPieces(string query)
        {
            List<Piece> pieces = new List<Piece>();
            if (conn.IsConnect())
            {
                var cmd = new MySqlCommand(query, conn.Connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string name = reader.GetString(0);
                    string position = reader.GetString(1);
                    string color = reader.GetString(2);

                    Position pos = (Position)Enum.Parse(typeof(Position), position, true); // true == ignore case
                    ChessColor col = (ChessColor)Enum.Parse(typeof(ChessColor), color, true); // true == ignore case

                    if (name.Contains("pawn"))
                        pieces.Add(new Pawn(name, false, pos, col));
                    if (name.Contains("rook"))
                        pieces.Add(new Rook(name, false, pos, col));
                    if (name.Contains("bishop"))
                        pieces.Add(new Bishop(name, false, pos, col));
                    if (name.Contains("knight"))
                        pieces.Add(new Knight(name, false, pos, col));
                    if (name.Contains("king"))
                        pieces.Add(new King(name, false, pos, col));
                    if (name.Contains("queen"))
                        pieces.Add(new Queen(name, false, pos, col));
                }
                conn.Close();
            }

            return pieces;
        }
    }
}
