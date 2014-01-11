using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using tic_tac_schmoe.Resources;
using System.Collections.ObjectModel;

namespace tic_tac_schmoe.Pages
{
    public partial class LocalDiscoverPeerPage : PhoneApplicationPage
    {
        ObservableCollection<PeerItem> peerListBacking;

        DataReader dataReader;
        DataWriter dataWriter;
        public LocalDiscoverPeerPage()
        {
            InitializeComponent();
            BuildApplicationBar();
        }
        private void BuildApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton refreshButton = new ApplicationBarIconButton(new Uri("/Images/AppBar/reset.png", UriKind.Relative));
            refreshButton.Text = "Refresh";
            refreshButton.Click += RefreshPress;
            ApplicationBar.Buttons.Add(refreshButton);
            // Create a new menu item with the localized string from AppResources.
            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem("Dunno Yet");
            ApplicationBar.MenuItems.Add(appBarMenuItem);
        }

        private async void RefreshPress(object sender, EventArgs e)
        {
            await LookForConnection();
        }
        private async Task LookForConnection()
        {
            PeerFinder.DisplayName = "Test";
            PeerFinder.Start();
            var peers = await PeerFinder.FindAllPeersAsync();
            peerListBacking.Clear();
            if (peers.Count > 0)
            {
                foreach (PeerInformation peer in peers)
                {
                    peerListBacking.Add(new PeerItem(peer));
                }
            }
        }

        private async void SendMessageButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (dataWriter == null)
                dataWriter = new DataWriter(App.Socket.OutputStream);

            string message = SendingTextBox.Text;
            dataWriter.WriteInt32(message.Length);
            await dataWriter.StoreAsync();

            dataWriter.WriteString(message);
            await dataWriter.StoreAsync();
        }

        private async void ConnectToPeer(PeerInformation peerInfo)
        {
            StreamSocket socket = await PeerFinder.ConnectAsync(peerInfo);
            App.Socket = socket;
            ListenForIncomingMessage();
        }

        private void ConnectionRequested(object sender, ConnectionRequestedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                // Ask the user if they want to accept the incoming request.
                var result = MessageBox.Show("Connect to " +  e.PeerInformation.DisplayName
                                             , "IDK TItle", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    ConnectToPeer(e.PeerInformation);
                }
                else
                {
                    // Currently no method to tell the sender that the connection was rejected.
                }
            });
        }

        private async void ListenForIncomingMessage()
        {
            try
            {
                var message = await GetMessage();

                // Nope
                MessageTextBlock.Text = message;

                // Start listening for the next message.
                ListenForIncomingMessage();
            }
            catch (Exception)
            {
            }
        }
        private async Task<string> GetMessage()
        {
            if (dataReader == null)
                dataReader = new DataReader(App.Socket.InputStream);

            // Each message is sent in two blocks.
            // The first is the size of the message.
            // The second if the message itself.
            //var len = await GetMessageSize();
            await dataReader.LoadAsync(4);
            uint messageLen = (uint)dataReader.ReadInt32();
            await dataReader.LoadAsync(messageLen);
            return dataReader.ReadString(messageLen);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            PeerFinder.ConnectionRequested += ConnectionRequested;

            peerListBacking = new ObservableCollection<PeerItem>();
            FoundPeerList.ItemsSource = peerListBacking;
            await LookForConnection();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            PeerFinder.ConnectionRequested -= ConnectionRequested;
            PeerFinder.Stop();
            CloseConnection();
        }

        private void CloseConnection()
        {
            if (dataReader != null)
            {
                dataReader.Dispose();
                dataReader = null;
            }

            if (dataWriter != null)
            {
                dataWriter.Dispose();
                dataWriter = null;
            }

            //if (App.Socket != null)
            //{
            //    App.Socket.Dispose();
            //    App.Socket = null;
            //}

            PeerFinder.Stop();
        }

        private void FoundPeerList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PeerItem selected = (PeerItem)FoundPeerList.SelectedItem;
            if (selected != null)
            {
                ConnectToPeer(selected.PeerInformation);
            }
        }

    }
}