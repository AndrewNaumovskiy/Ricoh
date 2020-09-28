using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace RicohXamarin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SpherePage : ContentPage
    {
        private VM _vm;

        private SphereApp app;

        public SpherePage()
        {
            InitializeComponent();

            _vm = new VM();
            BindingContext = _vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            StartSphere();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            app.Dispose();
        }

        private void GetLive_OnClicked(object sender, EventArgs e)
        {
            //StartLivePreview();
            
            app.StartConnection();



            //app.SetImage(new byte[]{0xff});

            //StartLivePreview();
        }

        public async void StartLivePreview()
        {
            
        }

        public void SetImage(byte[] arr)
        {
            if (arr == null || arr.Length == 0) return;

            //app.SetImage(arr);

            //var image = ImageSource.FromStream(() => new MemoryStream(arr));
            //
            //_vm.SetImage(image);
            //LivePreview.Source = image;
        }

        public async void StartSphere()
        {
            app = await UrhoSurface.Show<SphereApp>(new Urho.ApplicationOptions(assetsFolder: "Data"));
            //await app.CreateScene();
        }

        
    }
}