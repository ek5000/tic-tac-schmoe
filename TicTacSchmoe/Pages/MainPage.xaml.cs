using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Tic_Tac_Toe.Resources;
using System.Windows.Threading;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Diagnostics;
using Tic_Tac_Toe.Logic;
using Facebook.Client;
using Facebook;
using System.Text.RegularExpressions;
using System.IO.IsolatedStorage;

namespace Tic_Tac_Toe
{
    public partial class MainPage : PhoneApplicationPage
    {

        
            
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            // Sample code to localize the ApplicationBar
            ExtraList.ItemsSource = new Extras();
            LocalList.ItemsSource = new Locals();
            BuildLocalizedApplicationBar();
        }
        


        // Sample code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
            appBarButton.Text = AppResources.AppBarButtonText;
            ApplicationBar.Buttons.Add(appBarButton);

            // Create a new menu item with the localized string from AppResources.
            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
            ApplicationBar.MenuItems.Add(appBarMenuItem);
        }
        public class Extras : ObservableCollection<ListItem>
        {
            public Extras()
            {
                Add(new ListItem("About", "Version, Contact Info, Special Thanks", "extras_about"));
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
        public class Onlines : ObservableCollection<ListItem>
        {
            public Onlines()
            {
                Add(new ListItem("Logout", "Logout of Facebook.", "onlines_logout"));
                Add(new ListItem("Friends", "Find friends who play this game", "onlines_friends"));
            }
        }

        private void ExtraList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ListItem selection = (ListItem)ExtraList.SelectedItem;
            if (selection == null)
                return;
            switch (selection.id)
            {
                case "extras_about":
                    Extras_AboutPress();
                    break;
            }
        }
        private void Extras_AboutPress()
        {
            NavigationService.Navigate(new Uri("/Pages/AboutPage.xaml", UriKind.Relative));
        }

        private void LocalList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ListItem selection = (ListItem)LocalList.SelectedItem;
            if (selection == null)
                return;
            switch (selection.id)
            {
                case "locals_quick":
                    Locals_QuickPress();
                    break;
                case "locals_standard":
                    Locals_StandardPress();
                    break;
                case "locals_saved":
                    Locals_SavedPress();
                    break;
                case "locals_bluetooth":
                    Locals_BluetoothPress();
                    break;
            }
        }

        private void Locals_BluetoothPress()
        {
            NavigationService.Navigate(new Uri("/Pages/MultiplayerGamePage.xaml", UriKind.Relative));
        }
        private void Locals_SavedPress()
        {
            NavigationService.Navigate(new Uri("/Pages/SavedGamePage.xaml", UriKind.Relative));
        }
        private void Locals_StandardPress()
        {
            NavigationService.Navigate(new Uri("/Pages/KnotSetupPage.xaml", UriKind.Relative));
        }
        private void Locals_QuickPress()
        {
            NavigationService.Navigate(new Uri("/Pages/GamePage.xaml", UriKind.Relative));
        }

        private void OnlineList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ListItem selection = (ListItem)OnlineList.SelectedItem;
            if (selection == null)
                return;
            switch (selection.id)
            {
                case "onlines_login":
                    Onlines_LoginPress();
                    break;
                case "onlines_logout":
                    Onlines_LogoutPress();
                    break;
                case "onlines_friends":
                    Onlines_Friends();
                    break;
            }
        }
        private void Onlines_LoginPress()
        {
            NavigationService.Navigate(new Uri("/Pages/FacebookLoginPage.xaml", UriKind.Relative));
        }
        private void Onlines_LogoutPress()
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to logout of Facebook?", "Are you sure", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                Facebook_Logout();
                
            }
        }
        private void Onlines_Friends()
        {
            NavigationService.Navigate(new Uri("/Pages/FriendSelectPage.xaml", UriKind.Relative));
        }

        private void Facebook_Logout()
        {
            this.IsEnabled = false;
            LogoutProgress.Visibility = System.Windows.Visibility.Visible;
            WebBrowser wb = new WebBrowser();
            App.FacebookSessionClient.Logout();
            LoadCompletedEventHandler loadCompleted = null;
            loadCompleted = (senders, es) =>
            {
                if (wb.SaveToString().Contains("logout_form"))
                {
                    wb.InvokeScript("eval", "document.forms['logout_form'].submit();");

                    wb.Visibility = Visibility.Collapsed;
                    wb.LoadCompleted -= loadCompleted;
                }
                App.isAuthenticated = false;
                OnlineList.ItemsSource = new ObservableCollection<ListItem>(
                    new ListItem[] { new ListItem("Login", "Login to facebook to play against friends.", "onlines_login") });
                this.IsEnabled = true;
                LogoutProgress.Visibility = System.Windows.Visibility.Collapsed;
                IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
                if(settings.Contains("FacebookSessionClient"))
                    settings.Remove("FacebookSessionClient");
            };
            wb.LoadCompleted += loadCompleted;
            wb.Navigate(new Uri("https://www.facebook.com/logout.php"));
        }

        private void WBNavigated(object sender, NavigationEventArgs e)
        {
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            //Check if you're signed in. Changes the options based on whether or not you are.
            if (!App.isAuthenticated)
            {
                OnlineList.ItemsSource = new ObservableCollection<ListItem>(
                    new ListItem[] { new ListItem("Login", "Login to facebook to play against friends.", "onlines_login") });
            }
            else
            {
                OnlineList.ItemsSource = new Onlines();
            }
            base.OnNavigatedTo(e);
        }
    }
}