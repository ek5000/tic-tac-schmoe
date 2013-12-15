using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tic_tac_schmoe.Logic
{
    public struct Turn
    {
        private readonly int _BigBoardX;
        private readonly int _BigBoardY;
        private readonly int _SmallBoardX;
        private readonly int _SmallBoardY;
        private readonly Piece _PlayingPiece;
        public int BigX { get { return _BigBoardX; } }
        public int BigY { get { return _BigBoardY; } }
        public int SmallX { get { return _SmallBoardX; } }
        public int SmallY { get { return _SmallBoardY; } }
        public Piece PlayingPiece { get { return _PlayingPiece; } }
        public Turn(int bigBoardX, int bigBoardY, int smallBoardX, int smallBoardY, Piece playingPiece)
        {
            _BigBoardX = bigBoardX;
            _BigBoardY = bigBoardY;
            _SmallBoardX = smallBoardX;
            _SmallBoardY = smallBoardY;
            _PlayingPiece = playingPiece;
        }
    }
}
