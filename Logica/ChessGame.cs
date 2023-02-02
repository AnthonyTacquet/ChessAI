using Data;
using Global;
using K4os.Compression.LZ4.Streams;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Net.Quic;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Logica
{
    public class ChessGame
    {
        //Database database;
        private List<Piece> pieces = new List<Piece>();
        private List<Tile> tiles = new List<Tile>();
        public ChessColor ActiveColor = ChessColor.WHITE;
        private CsvFile file = new CsvFile();
        private AI AiBlack;
        private AI AiWhite;
        private Neuron LastMove;
        public bool Draw { get; set; } = false;
        public bool Load { get; set; } = false;
        public bool Stop { get; set; } = false;
        public bool WhiteWin { get; set; } = false;
        public bool BlackWin { get; set; } = false;

        public Task thread { get; set; }

        private List<Neuron> PreviousMovesBlack = new List<Neuron>();
        private List<Neuron> PreviousMovesWhite = new List<Neuron>();

        public ChessGame()
        {
            //DBConnection conn = new DBConnection("127.0.0.1", "chess", "anthony", "Anthony2013");
            //database = new Database(conn);
            //pieces = database.ReadPieces("SELECT * FROM chess.chess_piece;");
            pieces = file.ReadPieces();
            AiBlack = new AI(ActiveColor, ChessColor.BLACK, pieces);
            AiWhite = new AI(ActiveColor, ChessColor.WHITE, pieces);

            CreateBoard();

            // AI vs AI
            //StartAi(ActiveColor);
        }

        public Tile GetTile(Position position)
        {
            return tiles.Find(e => e.Position.Equals(position));
        }

        public void changeActiveColor()
        {
            if (ActiveColor == ChessColor.WHITE)
                ActiveColor = ChessColor.BLACK;
            else
                ActiveColor = ChessColor.WHITE;

            // AI vs AI
            if (ActiveColor == ChessColor.BLACK)
                StartAi(ActiveColor);
        }

        private void StartAi(ChessColor color)
        {
            if (Stop)
            {
                return;
            }
            thread = new Task(() => {
                if (BlackWin || WhiteWin)
                    return;

                Load = true;
                if (color == ChessColor.WHITE)
                {
                    AiWhite.Start();
                    List<Neuron> neurons = AiWhite.GetResults();
                    neurons.OrderByDescending(e => e.Value);
                    MoveAiPiece(neurons);
                    AiWhite.Reset();
                }
                else
                {
                    AiBlack.Start();
                    List<Neuron> neurons = AiBlack.GetResults();
                    neurons.OrderByDescending(e => e.Value);
                    MoveAiPiece(neurons);
                    AiBlack.Reset();
                }
                CheckCheckMate();

                Load = false;
                Draw = true;
                Thread.Sleep(1000);
                changeActiveColor();
            });

            thread.Start();
        }

        private void CheckCheckMate()
        {
            King kingBlack = (King)pieces.Find(e => e.Name.Contains("king") && e.Color.Equals(ChessColor.BLACK));
            if (kingBlack == null || kingBlack.Captured)
                WhiteWin = true;
            King kingWhite = (King)pieces.Find(e => e.Name.Contains("king") && e.Color.Equals(ChessColor.WHITE));
            if (kingWhite == null || kingWhite.Captured)
                BlackWin = true;

        }

        private void MoveAiPiece(List<Neuron> neurons)
        {
            List<Neuron> badMoves = new List<Neuron>();
            foreach (Neuron neuron in neurons)
            {
                ResetMoves();
                string[] elements = neuron.Name.Split('_');

                string name = $"{elements[0]}_{elements[1]}";
                Piece piece = pieces.Find(e => e.Name.Equals(name) && e.Color.Equals(ActiveColor));
                if (piece == null)
                    continue;
                ShowPossibleMoves(piece);
                Position? position = null;

                Debug.WriteLine(neuron.ToString());

                LastMove = neuron;


                // Knight SWITCH 
                if (piece is Knight)
                {
                    if (elements[2].Equals("O2"))
                        position = GetPositionMove(piece.Position, piece.Color, 2, -1);
                    else if (elements[2].Equals("W1"))
                        position = GetPositionMove(piece.Position, piece.Color, -2, 1);
                    else if (elements[2].Equals("W2"))
                        position = GetPositionMove(piece.Position, piece.Color, -2, -1);
                    else if (elements[2].Equals("N1"))
                        position = GetPositionMove(piece.Position, piece.Color, 1, 2);
                    else if (elements[2].Equals("S1"))
                        position = GetPositionMove(piece.Position, piece.Color, 1, -2);
                    else if (elements[2].Equals("N2"))
                        position = GetPositionMove(piece.Position, piece.Color, -1, 2);
                    else if (elements[2].Equals("S2"))
                        position = GetPositionMove(piece.Position, piece.Color, -1, -2);
                    else if (elements[2].Equals("W2"))
                        position = GetPositionMove(piece.Position, piece.Color, 2, 1);
                }
                else
                {
                    if (elements[2].Length == 3)
                    {
                        if (elements[2][..2].Equals("NE"))
                            position = GetPositionMove(piece.Position, piece.Color, (int)Char.GetNumericValue(elements[2][2]), (int)Char.GetNumericValue(elements[2][2]));
                        else if (elements[2][..2].Equals("SE"))
                            position = GetPositionMove(piece.Position, piece.Color, (int)Char.GetNumericValue(elements[2][2]), (int)Char.GetNumericValue(elements[2][2]));
                        else if (elements[2][..2].Equals("SW"))
                            position = GetPositionMove(piece.Position, piece.Color, (int)Char.GetNumericValue(elements[2][2]), (int)Char.GetNumericValue(elements[2][2]));
                        else if (elements[2][..2].Equals("NW"))
                            position = GetPositionMove(piece.Position, piece.Color, (int)Char.GetNumericValue(elements[2][2]), (int)Char.GetNumericValue(elements[2][2]));
                    }
                    else
                    {
                        if (elements[2][0].Equals('N'))
                            position = GetPositionMove(piece.Position, piece.Color, 0, (int)Char.GetNumericValue(elements[2][1]));
                        else if (elements[2][0].Equals('E'))
                            position = GetPositionMove(piece.Position, piece.Color, (int)Char.GetNumericValue(elements[2][1]), 0);
                        else if (elements[2][0].Equals('S'))
                            position = GetPositionMove(piece.Position, piece.Color, 0, (int)Char.GetNumericValue(elements[2][1]));
                        else if (elements[2][0].Equals('W'))
                            position = GetPositionMove(piece.Position, piece.Color, (int)Char.GetNumericValue(elements[2][1]), 0);
                    }
                }
                // Others

                if (position == null)
                    continue;

                if (!GetTile((Position)position).IsValidMove)
                {
                    badMoves.Add(neuron);
                    continue;
                }

                if (IsEmptyTile((Position)position))
                {
                    piece.Position = (Position)position;
                    break;
                }

                if (TakeOver(piece, PositionToCoordinates((Position)position)))
                {
                    if (ActiveColor == ChessColor.WHITE)
                        AiWhite.OverTook(neuron);
                    else
                        AiBlack.OverTook(neuron);

                    break;
                }

                badMoves.Add(neuron);
                continue;


            }
            if (ActiveColor == ChessColor.WHITE)
            {
                PreviousMovesWhite.Add(LastMove);
                AiWhite.BadMove(badMoves);
            }
            else
            {
                PreviousMovesBlack.Add(LastMove);
                AiBlack.BadMove(badMoves);
            }
            CheckRepeat();
            ResetMoves();
        }

        public void CheckRepeat()
        {
            if (ActiveColor == ChessColor.WHITE)
            {
                int i = PreviousMovesWhite.SkipLast(1).ToList().FindLastIndex(e => e.Equals(LastMove));
                Debug.WriteLine($"I: {i}");
                if (i == -1)
                    return;
                int val = PreviousMovesWhite.Count - i - 1;
                Debug.WriteLine($"Val: {val}");
                if (val == 2)
                {
                    AiWhite.BadMove(LastMove);
                    Debug.WriteLine($"Repeat: {LastMove}");
                }
            }
            else
            {
                int i = PreviousMovesBlack.SkipLast(1).ToList().FindLastIndex(e => e.Equals(LastMove));
                Debug.WriteLine($"I: {i}");
                if (i == -1)
                    return;
                int val = PreviousMovesBlack.Count - i - 1;
                Debug.WriteLine($"Val: {val}");
                if (val == 2)
                {
                    AiBlack.BadMove(LastMove);
                    Debug.WriteLine($"Repeat: {LastMove}");
                }
            }
        }

        public void AiGoodMove()
        {
            /*
            if ()
            ai.GoodMove(LastMove);
            */
        }

        public bool TakeOver(Piece piece, Coordinates coordinates)
        {
            Piece destinationPiece = GetPiece(coordinates);
            if (destinationPiece.Color.Equals(piece.Color))
                return false;
            pieces.Remove(destinationPiece);
            ResetMoves();

            piece.Position = CoordinatesToPosition(coordinates);

            AiBlack.PieceTaken(LastMove);

            return true;
        }

        public void ResetMoves()
        {
            tiles.ForEach(e => e.IsValidMove = false);
        }

        public void ShowPossibleMoves(Piece piece)
        {
            tiles.ForEach(e => e.IsValidMove = false);
            if (piece is Pawn)
            {
                Pawn pawn = (Pawn)piece;
                PawnMoves(pawn);
            }
            else if (piece is Knight)
            {
                Knight knight = (Knight)piece;
                KnightMoves(knight);
            }
            else if (piece is Rook)
            {
                Rook rook = (Rook)piece;
                RookMoves(rook);
            }
            else if (piece is Bishop)
            {
                Bishop bishop = (Bishop)piece;
                BishopMoves(bishop);
            }
            else if (piece is Queen)
            {
                Queen queen = (Queen)piece;
                QueenMoves(queen);
            }
            else if (piece is King)
            {
                King king = (King)piece;
                KingMoves(king);
            }
        }

        public void PawnMoves(Pawn pawn)
        {
            int iteration = 1;
            if (pawn.Moved == false)
                iteration = 2;
            Debug.WriteLine(iteration);

            // Forward moves
            Position newPos = pawn.Position;
            Move(newPos, pawn, iteration, 0, 1);


            // Take right
            newPos = pawn.Position;
            Position? pos = GetPositionMove(newPos, pawn.Color, 1, 1);
            if (pos != null)
            {
                bool? boo = IsTeamTile((Position)pos, pawn);
                if (boo == false)
                    tiles.Find(e => e.Position.Equals((Position)pos)).IsValidMove = true;
            }

            // Take left
            newPos = pawn.Position;
            pos = GetPositionMove(newPos, pawn.Color, -1, 1);
            if (pos != null)
            {
                bool? boo = IsTeamTile((Position)pos, pawn);
                if (boo == false)
                    tiles.Find(e => e.Position.Equals((Position)pos)).IsValidMove = true;
            }

        }

        public void KingMoves(King king)
        {
            int x;
            int y;
            for (int j = 0; j < 8; j++)
            {
                Position newPos = king.Position;
                switch (j)
                {
                    case 0: x = 1; y = 1; break;
                    case 1: x = -1; y = 1; break;
                    case 2: x = 1; y = -1; break;
                    case 3: x = 0; y = 1; break;
                    case 4: x = 1; y = 0; break;
                    case 5: x = 0; y = -1; break;
                    case 6: x = -1; y = 0; break;
                    default: x = -1; y = -1; break;

                }
                Move(newPos, king, 1, x, y);

            }
        }

        public void QueenMoves(Queen queen)
        {
            int x;
            int y;
            for (int j = 0; j < 8; j++)
            {
                Position newPos = queen.Position;
                switch (j)
                {
                    case 0: x = 1; y = 1; break;
                    case 1: x = -1; y = 1; break;
                    case 2: x = 1; y = -1; break;
                    case 3: x = 0; y = 1; break;
                    case 4: x = 1; y = 0; break;
                    case 5: x = 0; y = -1; break;
                    case 6: x = -1; y = 0; break;
                    default: x = -1; y = -1; break;

                }
                Move(newPos, queen, 8, x, y);

            }
        }

        public void BishopMoves(Bishop bishop)
        {
            int x;
            int y;
            for (int j = 0; j < 4; j++)
            {
                Position newPos = bishop.Position;
                switch (j)
                {
                    case 0: x = 1; y = 1; break;
                    case 1: x = -1; y = 1; break;
                    case 2: x = 1; y = -1; break;
                    default: x = -1; y = -1; break;

                }
                Move(newPos, bishop, 8, x, y);

            }
        }

        public void RookMoves(Rook rook)
        {
            int x;
            int y;
            for (int j = 0; j < 4; j++)
            {
                Position newPos = rook.Position;
                switch (j)
                {
                    case 0: x = 0; y = 1; break;
                    case 1: x = 1; y = 0; break;
                    case 2: x = 0; y = -1; break;
                    default: x = -1; y = 0; break;

                }
                Move(newPos, rook, 8, x, y);
            }
        }

        public void KnightMoves(Knight knight)
        {
            int x;
            int y;
            Position newPos = knight.Position;
            for (int i = 0; i < 8; i++)
            {
                switch (i)
                {
                    case 1: x = 2; y = -1; break;
                    case 2: x = -2; y = 1; break;
                    case 3: x = -2; y = -1; break;
                    case 4: x = 1; y = 2; break;
                    case 5: x = 1; y = -2; break;
                    case 6: x = -1; y = 2; break;
                    case 7: x = -1; y = -2; break;
                    default: x = 2; y = 1; break;
                }

                Move(newPos, knight, 1, x, y);
            }
        }

        public void Move(Position newPos, Piece piece, int time, int x, int y)
        {
            bool next = false;
            Position temp = newPos;
            Position? pos;
            for (int i = 0; i < time; i++)
            {
                pos = GetPositionMove(temp, piece.Color, x, y);
                if (pos == null)
                {
                    if (piece is Knight)
                        continue;
                    break;
                }


                bool? boo = IsTeamTile((Position)pos, piece);
                if (boo == true || next)
                {
                    if (piece is Knight)
                        continue;
                    else
                        break;
                }
                else if (boo == false && piece is Pawn)
                    break;
                else if (boo == false)
                    next = true;



                tiles.Find(e => e.Position.Equals(pos)).IsValidMove = true;
                temp = (Position)pos;
            }

        }

        public List<Piece> GetPieces()
        {
            return pieces;
        }

        public Position? GetPositionMove(Position position, ChessColor color, int x, int y)
        {
            char[] letters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
            string pos = position.ToString().ToLower();

            int baseValX = Array.IndexOf(letters, pos[0]);
            int baseValY = (int)Char.GetNumericValue(pos[1]);

            if (color == ChessColor.WHITE)
                baseValY += y;
            else
                baseValY -= y;

            if (color == ChessColor.WHITE)
                baseValX += x;
            else
                baseValX -= x;

            if (baseValX >= 8 || baseValX <= -1 || baseValY >= 9 || baseValY <= 0)
                return null;

            pos = $"{letters[baseValX]}{baseValY}";
            return (Position)Enum.Parse(typeof(Position), pos, true);
        }

        public List<Tile> GetTiles()
        {
            return tiles;
        }

        public bool IsEmptyTile(Position position)
        {
            if (pieces.Find(e => e.Position.Equals(position)) == null)
                return true;
            return false;
        }

        public bool IsEmptyTile(Tile tile)
        {
            if (pieces.Find(e => e.Position.Equals(tile.Position)) == null)
                return true;
            return false;
        }

        public bool? IsTeamTile(Position position, Piece comparePiece)
        {
            Piece piece = pieces.Find(e => e.Position.Equals(position));
            if (piece == null)
                return null;
            if (piece.Color.Equals(comparePiece.Color))
                return true;
            return false;
        }

        public void CreateBoard()
        {
            char[] letters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
            ChessColor color = ChessColor.BLACK;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    string pos = $"{letters[i]}{j + 1}";
                    Position position = (Position)Enum.Parse(typeof(Position), pos, true);
                    Tile tile = new Tile(position, color);

                    tiles.Add(tile);

                    color = ChangeChessColor(color);
                }
                color = ChangeChessColor(color);
            }
        }

        public ChessColor ChangeChessColor(ChessColor color)
        {
            if (color == ChessColor.WHITE)
                return ChessColor.BLACK;
            else
                return ChessColor.WHITE;
        }

        public Piece GetPiece(Coordinates coordinates)
        {
            Position position = CoordinatesToPosition(coordinates);
            Piece piece = pieces.Find(e => e.Position.Equals(position));
            if (piece == null)
                throw new Exception("Piece Not Found");
            return piece;
        }

        public ChessPieces GetPieceFromName(Piece piece)
        {
            if (piece.Name.ToLower().Contains("pawn"))
                return ChessPieces.PAWN;
            else if (piece.Name.ToLower().Contains("rook"))
                return ChessPieces.ROOK;
            else if (piece.Name.ToLower().Contains("bishop"))
                return ChessPieces.BISHOP;
            else if (piece.Name.ToLower().Contains("knight"))
                return ChessPieces.KNIGHT;
            else if (piece.Name.ToLower().Contains("king"))
                return ChessPieces.KING;
            else if (piece.Name.ToLower().Contains("queen"))
                return ChessPieces.QUEEN;
            return ChessPieces.NONE;

        }
        public Position CoordinatesToPosition(Coordinates coordinates)
        {
            char[] letters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
            int number = coordinates.X / 40;
            if (number > 7)
                number = 7;
            if (number < 0)
                number = 0;
            string pos = $"{letters[number]}{8 - (int)coordinates.Y / 40}";
            Console.WriteLine(pos);

            Position position = (Position)Enum.Parse(typeof(Position), pos, true);

            return position;

        }
        public Coordinates PositionToCoordinates(Position position)
        {
            string pos = position.ToString();
            int dis = 10;
            int baseVal1 = 8 - (int)Char.GetNumericValue(pos[1]);
            int baseVal2 = CharToInt(pos[0]);


            Coordinates coordinates = new Coordinates((40 * baseVal2) + dis, (40 * baseVal1) + dis);

            return coordinates;
        }

        public int CharToInt(char letter)
        {
            if (letter.Equals('A'))
                return 0;
            else if (letter.Equals('B'))
                return 1;
            else if (letter.Equals('C'))
                return 2;
            else if (letter.Equals('D'))
                return 3;
            else if (letter.Equals('E'))
                return 4;
            else if (letter.Equals('F'))
                return 5;
            else if (letter.Equals('G'))
                return 6;
            else if (letter.Equals('H'))
                return 7;
            return -1;
        }
    }
}
