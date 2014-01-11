using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;

namespace tic_tac_schmoe.Pages
{
    public partial class PanoramaPage1 : PhoneApplicationPage
    {
        public PanoramaPage1()
        {
            InitializeComponent();

            LocalList.ItemsSource = new Locals();

        }
        private void LocalList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ListItem selection = (ListItem)LocalList.SelectedItem;
            if (selection == null)
                return;
            switch (selection.id)
            {
                case "locals_quick":
                    NavigationService.Navigate(new Uri("/Pages/GamePage.xaml", UriKind.Relative));
                    break;
                case "locals_standard":
                    NavigationService.Navigate(new Uri("/Pages/KnotSetupPage.xaml", UriKind.Relative));
                    break;
                case "locals_saved":
                    NavigationService.Navigate(new Uri("/Pages/SavedGamePage.xaml", UriKind.Relative));
                    break;
                case "locals_bluetooth":
                    NavigationService.Navigate(new Uri("/Pages/LocalDiscoverPeerPage.xaml", UriKind.Relative));
                    break;
            }
        }
    }
    public class Locals : ObservableCollection<ListItem>
    {
        public Locals()
        {
            Add(new ListItem("Standard", "Standard untimed match; One phone", "locals_standard"));
            Add(new ListItem("Quick Match", "Standard untimed match; Default Settings", "locals_quick"));
            Add(new ListItem("Bluetooth", "Standard match played over bluetooth", "locals_bluetooth"));
            Add(new ListItem("Saved", "Resume a saved gme", "locals_saved"));
        }
    }
}