using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tic_tac_schmoe.Logic
{
    class Player
    {
        private string p;
        private Piece piece;
        private System.Windows.Media.Color color;
        private string crossicon;

        public Player(string p, Piece piece, System.Windows.Media.Color color, string crossicon)
        {
            // TODO: Complete member initialization
            this.p = p;
            this.piece = piece;
            this.color = color;
            this.crossicon = crossicon;
        }
    }
}
