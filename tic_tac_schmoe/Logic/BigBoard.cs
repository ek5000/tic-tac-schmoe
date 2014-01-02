using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tic_tac_schmoe.Logic
{    
    public class BigBoard : IBigBoard
    {
        public Piece Victor { get; set; }
        private SmallBoard[,] SmallBoards { get; set; }
        private int RowSize { get; set; }
        private int NumberOfBoards { get { return RowSize * RowSize; } }
        public Piece NextPieceTurn { get; set; }
        public int NextX { get; set; }
        public int NextY { get; set; }
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
            this.Victor = Piece.None;

            SetUpSmallBoards(this.SmallBoards, rowSize);
            SetUpVictoryArrays(this.VictoryArrays, rowSize);
        }

        public SmallBoard this[int x, int y] { get { return SmallBoards[x, y]; } }

        public Piece this[int w, int x, int y, int z] { get { return SmallBoards[w, x][y, z]; } }

        public Piece this[Tuple<int, int, int, int> spot]{ get { return this[spot.Item1, spot.Item2, spot.Item3, spot.Item4]; } }

        private static void SetUpVictoryArrays(Dictionary<Piece, int[]> victoryArrays, int rowSize)
        {
            victoryArrays.Add(Piece.Cross, new int[rowSize * 2 + 2]);
            victoryArrays.Add(Piece.Knot, new int[rowSize * 2 + 2]);
        }
        private static void SetUpSmallBoards(SmallBoard[,] smallBoards, int rowSize)
        {
            for (int i = 0; i < rowSize; ++i)
                for (int j = 0; j < rowSize; ++j)
                    smallBoards[i, j] = new SmallBoard(rowSize);
        }
        public bool PlayPiece(Turn turn)
        {
            SmallBoard relevantSmallBoard = SmallBoards[turn.BigX, turn.BigY];
            Piece formerBoardHolder = relevantSmallBoard.Victor;
            bool successful = relevantSmallBoard.PlayPiece(turn);
            Piece newBoardHolder = relevantSmallBoard.Victor;
            if (successful)
            {
                UpdateNextPlayingSpot(turn);
                // Board winner changed on this turn
                if(formerBoardHolder != newBoardHolder)
                {
                    ++SmallBoardsWon;
                    UpdateVictoryArray(turn);
                    CheckVictory(newBoardHolder);
                }
            }
            return successful;
        }

        private void UpdateNextPlayingSpot(Turn turn)
        {
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
        }

        private void CheckVictory(Piece currentPieceTurn)
        {
            if (Victor != Piece.None)
                return;
            if (SmallBoardsWon == NumberOfBoards)
                Victor = Piece.Cat;
            else
            {
                int[] victoryArray = VictoryArrays[currentPieceTurn];
                if (victoryArray.Max() == RowSize)
                    Victor = currentPieceTurn;
            }
        }

        private void UpdateVictoryArray(Turn turn)
        {
            int[] victoryArray = VictoryArrays[turn.PlayingPiece];
            victoryArray[turn.BigX]++; // Updates Row
            victoryArray[turn.BigY + RowSize]++; // Updates Columns
            if (turn.BigX == turn.BigY) // Updates the TL-BR diagonal
                victoryArray[2 * RowSize]++;
            if (turn.BigX - RowSize + 1 == turn.BigY) // Updates the TR-BL diagonal
                victoryArray[2 * RowSize + 1]++;
        }
    }
}
