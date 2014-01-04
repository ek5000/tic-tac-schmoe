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

namespace tic_tac_schmoe.Pages
{
    public partial class CrossSetupPage : PhoneApplicationPage
    {
        private string KnotName;
        private string KnotColor;
        private string KnotIcon;
        public CrossSetupPage()
        {
            InitializeComponent();
            BuildApplicationBar();
            IconPicker.ItemsSource = new IconList(new SolidColorBrush(), KnotIcon);
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
            IconPicker.ItemsSource = new IconList(((ColorItem)ColorPicker.SelectedItem).color, KnotIcon);
            IconPicker.SelectedIndex = selected;
        }
        private void AcceptPress(object sender, EventArgs e)
        {
            MessageBoxResult result =
                MessageBox.Show("Do you accept these settings?",
                "Confirm", MessageBoxButton.OKCancel);
            string uri = "/Pages/GamePage.xaml?knotname=" + KnotName + "&knotcolor=" + KnotColor + "&knoticon=" + KnotIcon +
                                                   "&crossname=" + CrossName.Text + "&crosscolor=" + ColorPicker.SelectedItem.ToString() + "&crossicon=" + IconPicker.SelectedItem.ToString();
            if (result == MessageBoxResult.OK)
            {
                NavigationService.Navigate(new Uri(uri, UriKind.Relative));
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            NavigationContext.QueryString.TryGetValue("knotname", out KnotName);
            NavigationContext.QueryString.TryGetValue("knotcolor", out KnotColor);
            NavigationContext.QueryString.TryGetValue("knoticon", out KnotIcon);
            if (e.NavigationMode == NavigationMode.New)
            {
                ColorPicker.ItemsSource = new ColorList((Color)Application.Current.Resources["PhoneAccentColor"], ThemeColors.StringToColor(KnotColor));
                ColorPicker.SelectionChanged += ColorPicker_SelectionChanged;
            }
        }
    }
}