﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Global
{
    public class Bishop : Piece
    {
        public Bishop(string Name, bool Captured, Position position, ChessColor chessColor) : base(Name, Captured, position, chessColor)
        {
        }

        public override ChessPieces GetPieceType()
        {
            return ChessPieces.BISHOP;
        }


    }
}
