using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace tic_tac_schmoe.Logic
{
    public class Player
    {
        public Piece Piece { get; private set; }
        public string Name { get; private set; }
        public Color Color { get; private set; }
        public BitmapImage Image { get; private set; }
        public Player(string name, Piece piece, Color color, string image)
        {
            if (name == null)
            {
                switch (piece)
                {
                    case Piece.Cat:
                        name = "Cat";
                        break;
                    case Piece.Cross:
                        name = "Cross";
                        break;
                    case Piece.Knot:
                        name = "Circle";
                        break;
                    default:
                        name = "Unknown";
                        break;
                }
            }
            Name = name;
            Piece = piece;
            Image = new BitmapImage(new Uri(image, UriKind.Relative));
            Image.CreateOptions = BitmapCreateOptions.BackgroundCreation;
            Color = color;


        }
        public string getPieceName()
        {
            switch (Piece)
            {
                case Piece.Cat:
                    return "cat";
                case Piece.Cross:
                    return "cross";
                case Piece.Knot:
                    return "knot";
                default:
                    return "weirdo";
            }
        }
    }
}
