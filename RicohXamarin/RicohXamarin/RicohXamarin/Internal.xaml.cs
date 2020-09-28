using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RicohXamarin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Internal : ContentPage
    {
        private SpherePage spherePage;

        private InternalViewModel _vm;

        public Internal()
        {
            InitializeComponent();

            _vm = new InternalViewModel();

            BindingContext = _vm;
            
            CheckForCamera();

            //OpenImage();
            //spherePage = new SpherePage();
        }

        private int i = 0;
        protected override void OnAppearing()
        {
            base.OnAppearing();

            //NotifyClickWifiButton();
            if (i++ == 0)
            {
                _vm.StartConnectionCheckQueue();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _vm._queueRunning = false;
        }

        public async void NotifyClickWifiButton()
        {
            // fix me
            await DisplayAlert("Ricoh", "Please, click on Wifi button on camera", "Ok");
        }

        public async void CheckForCamera()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpRequestMessage request = new HttpRequestMessage())
                {
                    request.RequestUri = new Uri("http://192.168.1.1:80/osc/state");
                    request.Method = HttpMethod.Post;
                    request.Headers.Add("Accept", "application/json");

                    client.Timeout = TimeSpan.FromSeconds(5);

                    try
                    {
                        HttpResponseMessage response = await client.SendAsync(request);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            //StatusLabel.Text = "Connected";
                            HttpContent responseContent = response.Content;
                            var json = await responseContent.ReadAsStringAsync();
                            SetCameraValues(json);
                        }
                        else
                        {
                            //StatusLabel.Text = "Not connected";
                        }
                    }
                    catch (Exception e)
                    {
                       //StatusLabel.Text = "Not connected";
                        var kek = e;
                    }
                }
            }
        }

        public void OpenImage()
        {
            _vm.SetImage();
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            CheckForCamera();
        }

        private void SetCameraValues(string text)
        {
            JObject json = JsonConvert.DeserializeObject<JObject>(text);
            string battery = json.SelectToken("state").SelectToken("batteryLevel").ToString();

            double batteryas = double.TryParse(battery, out batteryas) ? batteryas : 0;

            //BatteryLabel.Text = batteryas * 100 + "%";
        }

        private void Shot_OnClicked(object sender, EventArgs e)
        { 
            Shot();
        }

        private async void Shot()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    JObject body = new JObject();
                    body.Add(new JProperty("name", "camera.takePicture"));
                    body.Add(new JProperty("parameters", new JObject()));

                    HttpContent content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("http://192.168.1.1:80/osc/commands/execute", content);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent responseContent = response.Content;
                        var json = await responseContent.ReadAsStringAsync();

                        SuccessShot(json);
                    }
                    else
                    {
                        //StatusLabel.Text = "Not connected";
                    }
                }
                catch (Exception e)
                {
                   // StatusLabel.Text = "Not connected";
                    var kek = e;
                }
            }
        }

        public void SuccessShot(string text)
        {
            JObject json = JsonConvert.DeserializeObject<JObject>(text);

            string id = json.SelectToken("id").ToString();

            ShotNotTaken(id);
            


        }

        public async void ShotNotTaken(string id)
        {
            await Task.Delay(2000);

            CommandStatus(id);
            
        }

        public async void CommandStatus(string id)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    JObject body = new JObject();
                    body.Add(new JProperty("id", id));

                    HttpContent content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("http://192.168.1.1:80/osc/commands/status", content);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent responseContent = response.Content;
                        var text = await responseContent.ReadAsStringAsync();

                        ShowImage(text);
                    }
                    else
                    {
            //            StatusLabel.Text = "Not connected";
                    }
                }
                catch (Exception e)
                {
            //        StatusLabel.Text = "Not connected";
                    var kek = e;
                }
            }
        }

        public void ShowImage(string text)
        {
            JObject json = JsonConvert.DeserializeObject<JObject>(text);

            var status = json.SelectToken("state").ToString();

            var url = json.SelectToken("results").SelectToken("fileUrl").ToString();

            //shotStatus.Text = status;

            //imageUrl.Text = url;

            DownloadImage(url);
        }

        public async void DownloadImage(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent responseContent = response.Content;
                        var imageArray = await responseContent.ReadAsByteArrayAsync();

                        CreateImage(imageArray);
                    }
                    else
                    {
            //            StatusLabel.Text = "Not connected";
                    }
                }
                catch (Exception e)
                {
             //       StatusLabel.Text = "Not connected";
                    var kek = e;
                }
            }
        }

        public ImageSource Image;

        public void CreateImage(byte[] arr)
        {
            Image = ImageSource.FromStream(() => new MemoryStream(arr));
            //image.Source = Image;
        }

        private void OpenSphere_OnClicked(object sender, EventArgs e)
        {
            App.Current.MainPage.Navigation.PushAsync(spherePage, true);
        }
    }
}