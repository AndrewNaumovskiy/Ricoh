using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkExtension;
using Xamarin.Forms;

namespace RicohXamarin
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private Internal _internalPage;

        public MainPage()
        {
            InitializeComponent();

            _internalPage = new Internal();
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            // Alert ->
            // Ricoh Theta
            // Please switch the camera on and ensure WiFi icon is flashing. If not, press WiFi button (middle button on right of camera). Then, connect to the Thetaxxx WiFi connection from phone settings


            // ask to connect to specific wifi
            // List from config

            ConnectToWifi();

            App.Current.MainPage.Navigation.PushAsync(_internalPage, true);
        }

        private async void ConnectToWifi()
        {
            try
            {
                var config = new NEHotspotConfiguration("THETAYL02102089.OSC", "02102089", false) {JoinOnce = true};
                var configManager = new NEHotspotConfigurationManager();
                await configManager.ApplyConfigurationAsync(config);

                Console.WriteLine("Connected");
            }
            catch (Foundation.NSErrorException error)
            {
                var kek = error;
            }
            catch (Exception e)
            {
                var kek = e;
            }

        }
    }
}
