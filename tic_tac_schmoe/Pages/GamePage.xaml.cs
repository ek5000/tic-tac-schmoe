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

        Player Knot;
        Player Cross;
        Player Cat;

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
            throw new NotImplementedException();
        }

        private void AcceptPress(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string knotname, crossname;
            NavigationContext.QueryString.TryGetValue("knotname", out knotname);
            NavigationContext.QueryString.TryGetValue("crossname", out crossname);
            knotname = knotname ?? "Knot";
            crossname = crossname ?? "Cross";

            string knoticon, crossicon;
            NavigationContext.QueryString.TryGetValue("knoticon", out knoticon);
            NavigationContext.QueryString.TryGetValue("crossicon", out crossicon);
            knoticon = knoticon ?? "/Images/Pieces/Piece0.png";
            crossicon = crossicon ?? "/Images/Pieces/Piece1.png";

            string rowSizeString;
            NavigationContext.QueryString.TryGetValue("rowsize", out rowSizeString);
            rowSizeString = rowSizeString ?? "3";
            int rowsize = int.Parse(rowSizeString);

            SetUpVariables(rowsize);
            SetUpDefaultColors();
        }
        private void SetUpVariables(int rowSize = 3)
        {
            Grids = new Image[rowSize, rowSize];
            GridColors = new Rectangle[rowSize, rowSize];
            Brushes = new Dictionary<string, SolidColorBrush>();
            ThemeColor = (Color)Application.Current.Resources["PhoneAccentColor"];
            Spots = new Image[rowSize, rowSize, rowSize, rowSize];
            SpotColors = new Rectangle[rowSize, rowSize, rowSize, rowSize];
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
        private void setUpPlayers(string knot, string cross, string knoticon, string crossicon)
        {
            Knot = new Player(knot, Piece.knot, brushes["knot"].Color, knoticon);
            Cross = new Player(cross, Piece.cross, brushes["cross"].Color, crossicon);
            Cat = new Player("cat", Piece.cat, brushes["cat"].Color, crossicon);
        }
    }
}