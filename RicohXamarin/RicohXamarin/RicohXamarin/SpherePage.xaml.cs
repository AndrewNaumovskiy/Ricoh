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
        private VM _vm;

        public SpherePage()
        {
            InitializeComponent();

            _vm = new VM();
            BindingContext = _vm;
        }

        private void GetLive_OnClicked(object sender, EventArgs e)
        {
            StartLivePreview();
        }

        public async void StartLivePreview()
        {
            string url = "http://192.168.1.1:80/osc/commands/execute";

            var request = HttpWebRequest.Create(url);
            HttpWebResponse response = null;
            request.Method = "POST";
            request.Timeout = (int)(30 * 10000f);
            request.ContentType = "application/json;charset=utf-8";

            byte[] postBytes = Encoding.Default.GetBytes("{ \"name\": \"camera.getLivePreview\"}");
            request.ContentLength = postBytes.Length;

            Stream reqStream = request.GetRequestStream();
            reqStream.Write(postBytes, 0, postBytes.Length);
            reqStream.Close();
            var resp = request.GetResponse();

            var stream = resp.GetResponseStream();

            BinaryReader reader = new BinaryReader(new BufferedStream(stream), new System.Text.ASCIIEncoding());

            List<byte> imageBytes = new List<byte>();
            bool isLoadStart = false; // 画像の頭のバイナリとったかフラグ
            byte oldByte = 0; // 1つ前のByteデータを格納する

            await Task.Run(() =>
            {
                while (true)
                {
                    byte byteData = reader.ReadByte();

                    if (!isLoadStart)
                    {
                        if (oldByte == 0xFF)
                        {
                            // Первый двоичный файл изображения
                            imageBytes.Add(0xFF);
                        }

                        if (byteData == 0xD8)
                        {
                            // Второй двоичный файл изображения
                            imageBytes.Add(0xD8);

                            // Я взял заголовок изображения, поэтому беру его, пока не получу конечный двоичный файл
                            isLoadStart = true;
                        }
                    }
                    else
                    {
                        // Поместите в массив двоичных файлов изображений
                        imageBytes.Add(byteData);

                        // Когда байт является конечным байтом
                        // 0xFF -> 0xD9В случае конечного байта
                        if (oldByte == 0xFF && byteData == 0xD9)
                        {
                            // Потому что это конечный байт изображения
                            // Вы можете создать изображение из накопленных здесь байтов и создать текстуру.
                            // Отразить изображение в байтах в текстуре
                            SetImage(imageBytes.ToArray());
                            // Оставьте imageBytes пустым

                            imageBytes.Clear();
                            
                            // Вернитесь к бинарному циклу сбора данных в начале изображения.
                            isLoadStart = false;

                        }
                    }

                    oldByte = byteData;
                }
            });
        }

        public void SetImage(byte[] arr)
        {
            if (arr == null || arr.Length == 0) return;

            var image = ImageSource.FromStream(() => new MemoryStream(arr));

            _vm.SetImage(image);
            //LivePreview.Source = image;
        }
    }
}