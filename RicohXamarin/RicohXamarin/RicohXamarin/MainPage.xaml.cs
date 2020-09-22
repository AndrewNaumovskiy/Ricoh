using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            App.Current.MainPage.Navigation.PushAsync(_internalPage, true);
        }
    }
}
