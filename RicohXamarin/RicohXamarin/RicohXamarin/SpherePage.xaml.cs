using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RicohXamarin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SpherePage : ContentPage
    {
        public SpherePage()
        {
            InitializeComponent();
        }

        private void GetLive_OnClicked(object sender, EventArgs e)
        {
            StartLivePreview();
        }

        public async void StartLivePreview()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    JObject body = new JObject();
                    body.Add(new JProperty("name", "camera.getLivePreview"));
                    body.Add(new JProperty("parameters", new JObject()));

                    HttpContent content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

                    //HttpContent content =  new MultipartContent("x-mixed-replace");

                    HttpResponseMessage response = await client.PostAsync("http://192.168.0.153:80/osc/commands/execute", content);

                    response.Content = new MultipartContent("x-mixed-replace");// "Content-Type", "multipart/x-mixed-replace");

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        StatusLabel.Text = "Connected";

                        

                        HttpContent responseContent = response.Content;
                        var stream = await responseContent.ReadAsStreamAsync();

                        BinaryReader reader = new BinaryReader(new BufferedStream(stream), new System.Text.ASCIIEncoding());

                        //List<byte> imageBytes = new List<byte>();
                        //bool isLoadStart = false;
                        //byte oldByte = 0;
                        //while (true)
                        //{
                        //    byte byteData = reader.ReadByte();
                        //
                        //    if (!isLoadStart)
                        //    {
                        //        if (oldByte == 0xFF)
                        //        {
                        //            imageBytes.Add(0xFF);
                        //        }
                        //
                        //        if (byteData == 0xD8)
                        //        {
                        //            imageBytes.Add(0xD8);
                        //
                        //            isLoadStart = true;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        imageBytes.Add(byteData);
                        //
                        //        if (oldByte == 0xFF && byteData == 0xD9)
                        //        {
                        //            isLoadStart = false;
                        //        }
                        //    }
                        //
                        //    oldByte = byteData;
                        //    byteArraytoImage(imageBytes.ToArray());
                        //}
                        //
                        ////SuccessShot(json);
                    }
                    else
                    {
                        StatusLabel.Text = "Not connected";
                    }
                }
                catch (Exception ex)
                {
                    StatusLabel.Text = "Not connected";
                    var kek = ex;
                }
            }
        }

        public void byteArraytoImage(byte[] arr)
        {
            string imreBase64Data = Convert.ToBase64String(arr);
            string imgDataUrl = string.Format("data:image/png;base64,{0}", imreBase64Data);
            LivePreview.Source = imgDataUrl;
        }
    }
}