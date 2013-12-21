using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tic_tac_schmoe.Logic
{    
    public class BigBoard
    {
        private Piece Victor { get; set; }
        private SmallBoard[,] SmallBoards { get; set; }
        private int RowSize { get; set; }
        private Piece NextPieceTurn { get; set; }
        private int NextX { get; set; }
        private int NextY { get; set; }
        private Dictionary<Piece, int[]> VictoryArrays { get; set; }
        private int SmallBoardsWon { get; set; }

        public BigBoard(Piece startingPlayer = Piece.Cross, int rowSize = 3, int firstBoardX = -1, int firstBoardY = -1)
        {
            this.RowSize = rowSize;
            this.NextPieceTurn = startingPlayer;
            this.SmallBoards = new SmallBoard[rowSize, rowSize];
            this.VictoryArrays = new Dictionary<Piece, int[]>();
            this.NextX = firstBoardX;
            this.NextY = firstBoardY;
            this.SmallBoardsWon = 0;

            SetUpSmallBoards(this.SmallBoards, rowSize);
            SetUpVictoryArrays(this.VictoryArrays);
        }

        private void SetUpVictoryArrays(Dictionary<Piece, int[]> victoryArrays)
        {
            victoryArrays.Add(Piece.Cross, new int[RowSize * 2 + 2]);
            victoryArrays.Add(Piece.Knot, new int[RowSize * 2 + 2]);
        }
        private static void SetUpSmallBoards(SmallBoard[,] smallBoards, int rowSize)
        {
            for (int i = 0; i < rowSize; ++i)
                for (int j = 0; j < rowSize; ++j)
                    smallBoards[i, j] = new SmallBoard();
        }
        public bool PlayPiece(Turn turn)
        {
            SmallBoard relevantSmallBoard = SmallBoards[turn.BigX, turn.BigY];
            Piece formerBoardHolder = relevantSmallBoard.Winner;
            bool successful = relevantSmallBoard.PlayPiece(turn);
            if (successful)
            {
                // Set up next X and Y
                SmallBoard sentTo = SmallBoards[turn.SmallX, turn.SmallY];
                if (sentTo.IsFull())
                {
                    NextX = -1;
                    NextY = -1;
                }
                else
                {
                    NextX = turn.SmallX;
                    NextY = turn.SmallY;
                }
                Piece newBoardHolder = relevantSmallBoard.Winner;
                // Board winner changed on this turn
                if(formerBoardHolder != newBoardHolder) {
                    UpdateVictoryArray(turn);
                    CheckVictory();
                }
            }
            return successful;
        }

        private void CheckVictory()
        {
            if (Victor != null)
                return;
            if (SmallBoardsWon == RowSize*RowSize)
            {
                Victor = Piece.Cat;
            }
            else
            {
                foreach (int[] victoryArray in VictoryArrays)
                {
                    Victor = null;
                }
            }
        }

        private void UpdateVictoryArray(Turn turn)
        {
            int[] victoryArray = VictoryArrays[turn.PlayingPiece];
            victoryArray[turn.BigX]++; // Updates Row
            victoryArray[turn.BigY + RowSize]++;
            if (turn.BigX == turn.BigY)
                victoryArray[2 * RowSize]++;
            if (turn.BigX - RowSize == turn.BigY)
                victoryArray[2 * RowSize + 1]++;
        }
    }
}
