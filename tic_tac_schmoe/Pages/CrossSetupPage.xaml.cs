using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using Newtonsoft.Json;

namespace tic_tac_schmoe.Pages
{
    public partial class CrossSetupPage : PhoneApplicationPage
    {
        private GameInfo gameInfo;

        public CrossSetupPage()
        {
            InitializeComponent();
            BuildApplicationBar();
        }
        private void BuildApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton acceptButton = new ApplicationBarIconButton(new Uri("/Images/AppBar/accept.png", UriKind.Relative));
            acceptButton.Text = "Accept";
            acceptButton.Click += AcceptPress;
            ApplicationBar.Buttons.Add(acceptButton);
            // Create a new menu item with the localized string from AppResources.
            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem("Dunno Yet");
            ApplicationBar.MenuItems.Add(appBarMenuItem);
        }
        void ColorPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selected = IconPicker.SelectedIndex;
            IconPicker.ItemsSource = new IconList(((ColorItem)ColorPicker.SelectedItem).color, gameInfo.KnotIcon);
            IconPicker.SelectedIndex = selected;
        }
        private void AcceptPress(object sender, EventArgs e)
        {
            MessageBoxResult result =
                MessageBox.Show("Do you accept these settings?",
                "Confirm", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                gameInfo.CrossColor = ColorPicker.SelectedItem.ToString();
                gameInfo.CrossIcon = IconPicker.SelectedItem.ToString();
                gameInfo.CrossName = CrossName.Text;
                string gameInfoString = JsonConvert.SerializeObject(gameInfo);
                string uri = String.Format("/Pages/GamePage.xaml?gameinfo={0}",
                    Uri.EscapeUriString(gameInfoString));
                NavigationService.Navigate(new Uri(uri, UriKind.Relative));
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string gameInfoString;
            NavigationContext.QueryString.TryGetValue("gameinfo", out gameInfoString);
            gameInfo = JsonConvert.DeserializeObject<GameInfo>(gameInfoString);
            if (e.NavigationMode == NavigationMode.New)
            {
                ColorPicker.ItemsSource = new ColorList((Color)Application.Current.Resources["PhoneAccentColor"],
                    ThemeColors.StringToColor(gameInfo.KnotColor));
                ColorPicker.SelectionChanged += ColorPicker_SelectionChanged;
            }
            IconPicker.ItemsSource = new IconList(new SolidColorBrush(), gameInfo.KnotIcon);
        }
    }
}