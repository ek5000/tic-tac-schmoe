using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tic_tac_schmoe.Logic
{    
    public class BigBoard
    {
        private SmallBoard[,] SmallBoards { get; set; }
        private Piece NextPieceTurn {get; set; }
        private int RowSize { get; set; }

        public BigBoard(Piece startingPlayer = Piece.Cross, int rowSize = 3)
        {
            this.RowSize = rowSize;
            this.SmallBoards = new SmallBoard[rowSize, rowSize];
            this.NextPieceTurn = startingPlayer;

            SetUpSmallBoards(this.SmallBoards, rowSize);
        }
        private static void SetUpSmallBoards(SmallBoard[,] smallBoards, int rowSize)
        {
            for (int i = 0; i < rowSize; ++i)
                for (int j = 0; j < rowSize; ++j)
                    smallBoards[i, j] = new SmallBoard();
        }
        public bool PlayPiece(Turn turn)
        {
            bool successful = SmallBoards[turn.BigX, turn.BigY].PlayPiece(turn);
            if (successful)
            {

            }
            return false;
        }
    }
}
