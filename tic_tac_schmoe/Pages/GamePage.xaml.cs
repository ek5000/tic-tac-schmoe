using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Shapes;
using System.Windows.Media;
using tic_tac_schmoe.Logic;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Newtonsoft.Json;

namespace tic_tac_schmoe.Pages
{
    public partial class GamePage : PhoneApplicationPage
    {
        Image[,] Grids;
        Rectangle[,] GridColors;
        Image[, , ,] Spots;
        Rectangle[, , ,] SpotColors;
        Color ThemeColor;
        Dictionary<String, SolidColorBrush> Brushes;
        Tuple<int, int, int, int> CurrentSpot;
        int RowSize;
        IBigBoard board;

        Storyboard Blink;
        Dictionary<Piece, Player> Players;

        public GamePage()
        {
            InitializeComponent();
            BuildApplicationBar();
        }
        protected void BuildApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton acceptButton = new ApplicationBarIconButton(new Uri("/Images/AppBar/accept.png", UriKind.Relative));
            ApplicationBarIconButton resetButton = new ApplicationBarIconButton(new Uri("/Images/AppBar/reset.png", UriKind.Relative));
            ApplicationBarIconButton saveButton = new ApplicationBarIconButton(new Uri("/Images/AppBar/save.png", UriKind.Relative));
            acceptButton.Text = "Accept";
            resetButton.Text = "Reset";
            saveButton.Text = "Save";
            acceptButton.Click += AcceptPress;
            resetButton.Click += ResetPress;
            saveButton.Click += SavePress;
            ApplicationBar.Buttons.Add(acceptButton);
            ApplicationBar.Buttons.Add(resetButton);
            ApplicationBar.Buttons.Add(saveButton);
            // Create a new menu item with the localized string from AppResources.
            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem("Dunno Yet");
            ApplicationBar.MenuItems.Add(appBarMenuItem);
        }

        private void SavePress(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ResetPress(object sender, EventArgs e)
        {
            MessageBoxResult result =
                MessageBox.Show("This will reset the entire board, and a new game will begin.",
                "Are You Sure?", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                SetUpGame(RowSize);
                ResetBoardState();
                BeginBlink();
            }
        }

        private void AcceptPress(object sender, EventArgs e)
        {
            if (CurrentSpot.Item1 == -1) // No spot currently selected
                return;
            if (board.PlayPiece(new Turn(CurrentSpot, board.NextPieceTurn)))
            {
                SpotColors[CurrentSpot.Item1, CurrentSpot.Item2, CurrentSpot.Item3, CurrentSpot.Item4].Fill = board.NextPieceTurn == Piece.Knot ? Brushes["knot"] : Brushes["cross"];
                Player wonBy = Players[board[CurrentSpot.Item1, CurrentSpot.Item2].Victor];
                if (wonBy != null) // Someone won this smallboard
                {
                    GridColors[CurrentSpot.Item1, CurrentSpot.Item2].Fill = Brushes[wonBy.getPieceName()];
                    if (board.Victor != Piece.None) //Someone has one the big game
                    {
                        WinBigBoard(wonBy);
                        return;
                    }
                }
                board.NextPieceTurn = board.NextPieceTurn == Piece.Knot ? Piece.Cross : Piece.Knot;
                StatusText.Text = Players[board.NextPieceTurn].Name + "'s Turn";
            }
            CurrentSpot = new Tuple<int, int, int, int>(-1, -1, -1, -1);
            BeginBlink();
        }

        protected void WinBigBoard(Player p)
        {
            for (int i = 0; i < RowSize; ++i)
                for (int j = 0; j < RowSize; ++j)
                    GridColors[i, j].Fill = Brushes[p.getPieceName()];
            for (int w = 0; w < RowSize; ++w)
                for (int x = 0; x < RowSize; ++x)
                    for (int y = 0; y < RowSize; ++y)
                        for (int z = 0; z < RowSize; ++z)
                        {
                            SpotColors[w, x, y, z].IsHitTestVisible = false;
                            Spots[w, x, y, z].IsHitTestVisible = false;
                        }
            StatusText.Text = p.Name + " wins!";
        }

        protected void OnSpotTap(object sender, EventArgs e)
        {
            FrameworkElement spot = (FrameworkElement)sender;
            int[] tag = (int[])spot.Tag;
            if (changeSpot(tag[0], tag[1], tag[2], tag[3]) && board[CurrentSpot] == Piece.None)
                SpotColors[tag[0], tag[1], tag[2], tag[3]].Fill = Brushes["white"];
            
        }
        protected bool changeSpot(int w, int x, int y, int z)
        {

            if (board[w, x, y, z] != Piece.None)
                return false;
            else if ((board.NextX != w || board.NextY != x) && board.NextX != -1)
                return false;
            else
            {
                Image spot = Spots[w, x, y, z];
                spot.Source = Players[board.NextPieceTurn].Image;
                if (CurrentSpot.Item1 != -1 && board[CurrentSpot] == Piece.None)
                    SpotColors[CurrentSpot.Item1, CurrentSpot.Item2, CurrentSpot.Item3, CurrentSpot.Item4].Fill = Brushes["black"];
                CurrentSpot = new Tuple<int, int, int, int>(w, x, y, z);
                return true;
            }
        }
        protected void BeginBlink()
        {
            Blink.Stop();
            Blink.Children.Clear();
            if (board.NextX == -1) // All spots are available
            {
                for (int i = 0; i < 3; ++i)
                    for (int j = 0; j < 3; ++j)
                    {
                        ColorAnimation c = new ColorAnimation();
                        if (board[i, j].Victor != Piece.None)
                            c.From = ThemeColors.LightenColor(Players[board[i, j].Victor].Color);
                        else
                            c.From = ThemeColors.LightenColor(ThemeColor);
                        Storyboard.SetTarget(c, GridColors[i, j]);
                        Blink.Children.Add(c);
                    }
            }
            else
            {
                int i = board.NextX;
                int j = board.NextY;
                ColorAnimation c = new ColorAnimation();
                if (board[i, j].Victor != Piece.None)
                    c.From = ThemeColors.LightenColor(Players[board[i, j].Victor].Color);
                else
                    c.From = ThemeColors.LightenColor(ThemeColor);
                Storyboard.SetTarget(c, GridColors[i, j]);
                Blink.Children.Add(c);
            }
            Blink.Begin();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string gameInfoString;
            NavigationContext.QueryString.TryGetValue("gameinfo", out gameInfoString);
            GameInfo gameInfo = JsonConvert.DeserializeObject<GameInfo>(gameInfoString);
            string knotname = gameInfo.KnotName;
            string crossname = gameInfo.CrossName;
            string knoticon = gameInfo.KnotIcon;
            string crossicon = gameInfo.CrossIcon;
            int rowsize = gameInfo.RowSize;

            SetUpVariables(rowsize);
            #region SetUpColors();
            if (!gameInfo.QuickGame)
            {
                //Also remove the last 2 pages, since they're the setup pages.
                NavigationService.RemoveBackEntry();
                NavigationService.RemoveBackEntry();
                SetUpColors(gameInfo.KnotColor, gameInfo.CrossColor);
            }
            else
                SetUpDefaultColors();
            #endregion
            SetUpPlayers(knotname, crossname, knoticon, crossicon);
            SetUpGame(rowsize);
            SetUpBigBoard();
            SetUpSmallBoards();
            BeginBlink();
        }
        private void SetUpVariables(int rowSize = 3)
        {
            Grids = new Image[rowSize, rowSize];
            GridColors = new Rectangle[rowSize, rowSize];
            Brushes = new Dictionary<string, SolidColorBrush>();
            Players = new Dictionary<Piece, Player>();
            ThemeColor = (Color)Application.Current.Resources["PhoneAccentColor"];
            Spots = new Image[rowSize, rowSize, rowSize, rowSize];
            SpotColors = new Rectangle[rowSize, rowSize, rowSize, rowSize];
            Blink = new Storyboard() {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                Duration = new Duration(TimeSpan.FromMilliseconds(1500)) };
            Storyboard.SetTargetProperty(Blink, new PropertyPath("(Shape.Fill).(SolidColorBrush.Color)"));
            RowSize = rowSize;
        }
        private void SetUpDefaultColors()
        {
            switch (ThemeColor.ToString())
            {
                //Emerald, Lime, Green
                case "#FF008A00":
                case "#FF60A917":
                case "#FFA4C400":
                    SetUpColors("Cobalt", "Red", "Amber");
                    break;
                //Teal, Cyan, Cobalt
                case "#FF00ABA9":
                case "#FF0050EF":
                case "#FF1BA1E2":
                    SetUpColors("Red", "Steel", "Green");
                    break;

                //Indigo, Violet, Pink
                case "#FF6A00FF":
                case "#FFAA00FF":
                case "#FFF472D0":
                    SetUpColors("Green", "Amber", "Mauve");
                    break;

                //Crimson, Magenta, Red
                case "#FFA20025":
                case "#FFE51400":
                case "#FFD80073":
                    SetUpColors("Indigo", "Emerald", "Olive");
                    break;

                //Yellow, Amber, Orange
                case "#FFE3C800":
                case "#FFF0A30A":
                case "#FFFA6800":
                    SetUpColors("Magenta", "Lime", "Taupe");
                    break;

                //Steel, Olive, Mauve, Brown, Taupe
                case "#FF647687":
                case "#FF6D8764":
                case "#FF76608A":
                case "#FF825A2C":
                case "#FF87794E":
                    SetUpColors("Green", "Indigo", "Red");
                    break;

                // Mobile operator or hardware manufacturer color
                default:
                    ThemeColor = ThemeColors.Burnt_Orange;
                    SetUpColors("Magenta", "Lime", "Taupe");
                    break;
            }

        }
        private void SetUpColors(string knot, string cross, string cat = null)
        {
            Color knotColor = ThemeColors.StringToColor(knot);
            Color crossColor = ThemeColors.StringToColor(cross);
            Color catColor = cat == null ? ThemeColors.FindDifferentColor(knotColor, crossColor) : ThemeColors.StringToColor(cat);
            Brushes["white"] = new SolidColorBrush(Colors.White);
            Brushes["black"] = new SolidColorBrush(Colors.Black);
            Brushes["theme"] = new SolidColorBrush(ThemeColor);
            Brushes["knot"] = new SolidColorBrush(knotColor);
            Brushes["cross"] = new SolidColorBrush(crossColor);
            Brushes["cat"] = new SolidColorBrush(catColor);
        }
        private void SetUpPlayers(string knot, string cross, string knoticon, string crossicon)
        {
            Players[Piece.Knot] = new Player(knot, Piece.Knot, Brushes["knot"].Color, knoticon);
            Players[Piece.Cross] = new Player(cross, Piece.Cross, Brushes["cross"].Color, crossicon);
            Players[Piece.Cat] = new Player("cat", Piece.Cat, Brushes["cat"].Color, crossicon);
            Players[Piece.None] = null;
        }
        private void SetUpGame(int rowSize)
        {
            board = new BigBoard(rowSize: rowSize);
            StatusText.Text = Players[board.NextPieceTurn].Name + "'s Turn";
            CurrentSpot = new Tuple<int, int, int, int>(-1, -1, -1, -1);
        }
        private void SetUpBigBoard()
        {
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    Grids[i, j] = new Image()
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Width = 151,
                        Height = 151,
                        Margin = new Thickness(150 * i, 150 * j, 0, 0),
                        IsHitTestVisible = false
                    };
                    Grids[i, j].Source = new BitmapImage(new Uri("/Images/Board.png", UriKind.Relative));

                    GridColors[i, j] = new Rectangle()
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Width = 151,
                        Height = 151,
                        Margin = new Thickness(150 * i, 150 * j, 0, 0),
                        Fill = Brushes[GetBoardOwner(i, j)],
                        IsHitTestVisible = false
                    };
                    Canvas.SetZIndex(GridColors[i, j], 1);
                    Canvas.SetZIndex(Grids[i, j], 25);
                    ContentPanel.Children.Add(GridColors[i, j]);
                    ContentPanel.Children.Add(Grids[i, j]);
                }
            }
        }
        private void SetUpSmallBoards()
        {
            for (int w = 0; w < 3; ++w)
                for (int x = 0; x < 3; ++x)
                    for (int y = 0; y < 3; ++y)
                        for (int z = 0; z < 3; ++z)
                        {
                            SpotColors[w, x, y, z] = new Rectangle()
                            {
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Top,
                                Width = 30,
                                Height = 30,
                                Margin = new Thickness(w * 150 + y * 40 + 22, x * 150 + z * 40 + 22, 0, 0),
                                Fill = Brushes[GetSpotFill(w, x, y, z)],
                                Tag = new int[] { w, x, y, z }
                            };
                            Spots[w, x, y, z] = new Image()
                            {
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Top,
                                Width = 32,
                                Height = 32,
                                Margin = new Thickness(w * 150 + y * 40 + 21, x * 150 + z * 40 + 21, 0, 0),
                                Tag = new int[] { w, x, y, z }
                            };

                            Spots[w, x, y, z].Source = GetSpotImage(w, x, y, z);
                            Canvas.SetZIndex(SpotColors[w, x, y, z], 50);
                            Canvas.SetZIndex(Spots[w, x, y, z], 150);
                            Spots[w, x, y, z].MouseEnter += OnSpotTap;
                            Spots[w, x, y, z].Tap += OnSpotTap;
                            SpotColors[w, x, y, z].MouseEnter += OnSpotTap;
                            SpotColors[w, x, y, z].Tap += OnSpotTap;
                            ContentPanel.Children.Add(Spots[w, x, y, z]);
                            ContentPanel.Children.Add(SpotColors[w, x, y, z]);
                        }

        }
        
        private string GetBoardOwner(int i, int j)
        {
            if (board[i, j].Victor == Piece.None)
                return "theme";
            switch (board[i, j].Victor)
            {
                case Piece.Cat:
                    return "cat";
                case Piece.Cross:
                    return "cross";
                case Piece.Knot:
                    return "knot";
                default:
                    return "theme";
            }
        }
        private string GetSpotFill(int w, int x, int y, int z)
        {
            switch (board[w, x, y, z])
            {
                case Piece.Cross:
                    return "cross";
                case Piece.Knot:
                    return "knot";
                default:
                    return "black";
            }
        }
        private BitmapImage GetSpotImage(int w, int x, int y, int z)
        {
            switch (board[w, x, y, z])
            {
                case Piece.Cross:
                    return Players[Piece.Cross].Image;
                case Piece.Knot:
                    return Players[Piece.Knot].Image;
                default:
                    return new BitmapImage();
            }
        }

        private void ResetBoardState()
        {
            for (int w = 0; w < 3; ++w)
                for (int x = 0; x < 3; ++x)
                {
                    for (int y = 0; y < 3; ++y)
                        for (int z = 0; z < 3; ++z)
                        {
                            SpotColors[w, x, y, z].Fill = Brushes["black"];
                            Spots[w, x, y, z].Source = new BitmapImage();
                            Canvas.SetZIndex(SpotColors[w, x, y, z], 50);
                            Canvas.SetZIndex(Spots[w, x, y, z], 150);
                        }
                    GridColors[w, x].Fill = Brushes["theme"];

                }
        }
    }
}