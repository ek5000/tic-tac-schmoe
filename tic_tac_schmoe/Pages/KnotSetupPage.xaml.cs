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
    public partial class KnotSetupPage : PhoneApplicationPage
    {
        public KnotSetupPage()
        {
            InitializeComponent();
            BuildApplicationBar();
            ColorPicker.ItemsSource = new ColorList((Color)Application.Current.Resources["PhoneAccentColor"]);
            ColorPicker.SelectionChanged += ColorPicker_SelectionChanged;
            IconPicker.ItemsSource = new IconList(((ColorItem)ColorPicker.SelectedItem).color);
            ((IconItem)IconPicker.SelectedItem).IconColor = ((ColorItem)ColorPicker.SelectedItem).color;
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
        private void ColorPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selected = IconPicker.SelectedIndex;
            IconPicker.ItemsSource = new IconList(((ColorItem)ColorPicker.SelectedItem).color);
            IconPicker.SelectedIndex = selected;
        }
        private void AcceptPress(object sender, EventArgs e)
        {
            MessageBoxResult result =
                MessageBox.Show("Do you accept these settings?",
                "Confirm", MessageBoxButton.OKCancel);
            string navigation = "/Pages/CrossSetupPage.xaml?knotname=" + KnotName.Text + "&knotcolor=" + ColorPicker.SelectedItem.ToString() + "&knoticon=" + IconPicker.SelectedItem.ToString();
            if (result == MessageBoxResult.OK)
            {
                NavigationService.Navigate(new Uri(navigation, UriKind.Relative));
            }
        }
    }
    
}