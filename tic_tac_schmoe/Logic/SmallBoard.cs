using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tic_tac_schmoe.Logic
{
    public class SmallBoard
    {
        private Piece[,] Board { get; set; }
        private int RowSize { get; set; }
        private int NumSpots { get { return RowSize * RowSize; } }
        private int NumPiecesPlayed { get; set; }
        private Dictionary<Piece, int[]> VictoryArrays { get; set; }

        public Piece Victor { get; set; }

        public SmallBoard(int rowSize)
        {
            this.Board = new Piece[rowSize, rowSize];
            this.RowSize = rowSize;
            this.NumPiecesPlayed = 0;
            this.VictoryArrays = new Dictionary<Piece, int[]>();
            this.Victor = Piece.None;

            SetUpBoard(Board, rowSize);
            SetUpVictoryArrays(VictoryArrays, rowSize);
        }

        public Piece this[int x, int y]
        {
            get { return Board[x, y]; }
            set { Board[x, y] = value; }
        }

        private static void SetUpBoard(Piece[,] board, int rowSize)
        {
            for (int i = 0; i < rowSize; ++i)
                for (int j = 0; j < rowSize; ++j)
                    board[i, j] = Piece.None;
        }
        private static void SetUpVictoryArrays(Dictionary<Piece, int[]> victoryArrays, int rowSize)
        {
            victoryArrays.Add(Piece.Cross, new int[rowSize * 2 + 2]);
            victoryArrays.Add(Piece.Knot, new int[rowSize * 2 + 2]);
        }
        internal bool PlayPiece(Turn turn)
        {
            if (NumPiecesPlayed == NumSpots || Board[turn.SmallX, turn.SmallY] != Piece.None)
                return false;
            else
            {
                Board[turn.SmallX, turn.SmallY] = turn.PlayingPiece;
                ++NumPiecesPlayed;
                UpdateVictoryArray(turn);
                CheckVictory(turn.PlayingPiece);
                return true;
            }
        }        

        internal bool IsFull()
        {
            return NumPiecesPlayed == NumSpots;
        }

        private void UpdateVictoryArray(Turn turn)
        {
            int[] victoryArray = VictoryArrays[turn.PlayingPiece];
            victoryArray[turn.BigX]++; // Updates Row
            victoryArray[turn.BigY + RowSize]++; // Updates Columns
            if (turn.BigX == turn.BigY) // Updates the TL-BR diagonal
                victoryArray[2 * RowSize]++;
            if (turn.BigX - RowSize == turn.BigY) // Updates the TR-BL diagonal
                victoryArray[2 * RowSize + 1]++;
        }
        private void CheckVictory(Piece currentPieceTurn)
        {
            if (Victor != Piece.None)
                return;
            if (NumPiecesPlayed == NumSpots)
                Victor = Piece.Cat;
            else
            {
                int[] victoryArray = VictoryArrays[currentPieceTurn];
                if (victoryArray.Max() == RowSize)
                    Victor = currentPieceTurn;
            }
        }
    }
}
