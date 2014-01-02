using System;
namespace tic_tac_schmoe.Logic
{
    public interface IBigBoard
    {
        SmallBoard this[int x, int y] { get; }
        Piece this[int w, int x, int y, int z] { get; }
        Piece this[Tuple<int, int, int, int> spot] { get; }

        bool PlayPiece(Turn turn);

        Piece Victor { get; set; }
        int NextX { get; set; }
        int NextY { get; set; }
        Piece NextPieceTurn { get; set; }
    }
}
