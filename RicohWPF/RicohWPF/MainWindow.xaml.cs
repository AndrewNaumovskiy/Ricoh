using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Newtonsoft.Json;
using Ricoh;
using Ricoh.CameraController;
using Path = System.Windows.Shapes.Path;
using Newtonsoft.Json.Linq;
using Color = System.Drawing.Color;

namespace RicohWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///

    public partial class MainWindow : Window
    {
        private VM _vm;

        public MainWindow()
        {
            InitializeComponent();

            _vm = new VM();

            DataContext = _vm;
        }

        private void Createthumnail(object sender, RoutedEventArgs e)
        {
            var text =
                "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yWXoQeRVG98XazfuHnvZXYDAZmJIHpmsOigRYk1C6mOXuHJ+tQl3b7zE/U03FHFABSjFHFOBFAhRmnAH0pA9LvpALg0ikg5GaPMo34pgP3uM4zmmAyhgwzkUvmGl8w0gLsd9dtjequR3YDP51O1zK4+8FPqKzBKR3pfNPrQBqrdFesjkj0OKf/aThSodtp6jceaxvMPrQZDRYDSa9z2X8qb/AGjIvCvge1Zu80m6iwGidSmz980xtQlI5Y1Q3Um6iwyw907dWNQNIT3pmaaTTsAu4g5Bwav2Ov6vprFrPUbmHJyQshwfw6VnUUAf/9XwCikooAKKKKACiiigAooooAKKKKACiiigAooooAWikpaACiiigAooooAKM0UUALmjNJRQAuaXNNooAdmlzTKWgB2aN1NooAfmkzTaM0AOzRmm5ooAXNGaSigApKKKACiiigD/1vn+iiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKAClpKKAFopKKAFopKWgAooooAKKKKACiiigAooooAKKKKACiiigAoopKAFpKKKAP/X+f6KKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigD//Q+f6KKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigD//Z";

            byte[] bytes = Convert.FromBase64String(text);
            SetImage(bytes);
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                
            }
        }

        public async void PLEASE()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    JObject body = new JObject();
                    body.Add(new JProperty("name", "camera.getLivePreview"));
                    body.Add(new JProperty("parameters", new JObject()));

                    HttpContent content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8,
                        "application/json");

                    HttpResponseMessage response =
                        await client.PostAsync("http://192.168.1.1:80/osc/commands/execute", content);


                    JObject bodykke = new JObject();
                    bodykke.Add(new JProperty("name", "camera.takePicture"));
                    bodykke.Add(new JProperty("parameters", new JObject()));
                    
                    content = new StringContent(JsonConvert.SerializeObject(bodykke), Encoding.UTF8, "application/json");
                    
                    response =
                        await client.PostAsync("http://192.168.1.1:80/osc/commands/execute", content);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent responseContent = response.Content;

                        var stream = await responseContent.ReadAsStringAsync();



                        //BinaryReader reader =
                        //    new BinaryReader(new BufferedStream(stream), new System.Text.ASCIIEncoding());


                    }
                }
                catch (Exception ex)
                {
                    var kek = ex;
                }
            }
        }

        public async void GOODTRY()
        {
            int i = 0;

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

            //resp.ContentType = "multipart/x-mixed-replace";

            var kek = 0;

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
                            //if (i == 10)
                            //{
                            //    reader.Close();
                            //    break;
                            //}
                            //else
                            //{
                            //    i++;
                            //}

                            // Вернитесь к бинарному циклу сбора данных в начале изображения.
                            isLoadStart = false;

                        }
                    }

                    oldByte = byteData;
                }
            });

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //PLEASE();

            //GOODTRY();

            // sphere view of image

            Sphere();
        }


        public async void SetImage(byte[] array)
        {
            if (array == null || array.Length == 0) return;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(array))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();

            _vm.SetImage(image);

            await Task.Delay(100);
        }

        private BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            ms.Seek(0, SeekOrigin.Begin);
            bi.StreamSource = ms;
            bi.EndInit();
            return bi;
        }

        double phi0 = 0.0;
        double phi1 = Math.PI;
        double theta0 = 0.0;
        double theta1 = 2.0 * Math.PI;
        private double radius = 100;

        public void Sphere()
        {
            System.Drawing.Image image1 = new Bitmap("R0010015.JPG");
            Bitmap imgBitmap = new Bitmap(image1);

            Bitmap output = new Bitmap(imgBitmap.Width, imgBitmap.Height);

            for (int i = 0; i < imgBitmap.Width; i++)
            {
                for (int j = 0; j < imgBitmap.Height; j++)
                {
                    // map the angles from image coordinates
                    double theta = MapCoordinate(0.0, imgBitmap.Width - 1,
                        theta1, theta0, i);
                    double phi = MapCoordinate(0.0, imgBitmap.Height - 1, phi0,
                        phi1, j);
                    // find the cartesian coordinates
                    double x = radius * Math.Sin(phi) * Math.Cos(theta);
                    double y = radius * Math.Sin(phi) * Math.Sin(theta);
                    double z = radius * Math.Cos(phi);
                    // apply rotation around X and Y axis to reposition the sphere
                    RotX(90, ref y, ref z);
                    RotY(0, ref x, ref z);
                    // plot only positive points
                    if (z > 0)
                    {
                        System.Drawing.Color color = imgBitmap.GetPixel(i, j);
                        System.Drawing.Brush brs = new SolidBrush(color);
                        int ix = (int)x + 100;
                        int iy = (int)y + 100;

                        
                        var kekColor = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);

                        output.SetPixel(ix,iy, color);

                        //SetPixel(kekColor, ix, iy);

                        brs.Dispose();
                    }
                }
            }

            _vm.SetImage(ToBitmapImage(output));
        }

        private async void SetPixel(System.Windows.Media.Color kekColor, int ix, int iy)
        {
            System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
            Canvas.SetTop(rec, ix);
            Canvas.SetLeft(rec, iy);
            rec.Width = 1;
            rec.Height = 1;

            rec.Fill = new SolidColorBrush(kekColor);
        }


        public static void RotX(double angle, ref double y, ref double z)
        {
            double y1 = y * System.Math.Cos(angle) - z * System.Math.Sin(angle);
            double z1 = y * System.Math.Sin(angle) + z * System.Math.Cos(angle);
            y = y1;
            z = z1;
        }
        public static void RotY(double angle, ref double x, ref double z)
        {
            double x1 = x * System.Math.Cos(angle) - z * System.Math.Sin(angle);
            double z1 = x * System.Math.Sin(angle) + z * System.Math.Cos(angle);
            x = x1;
            z = z1;
        }
        public static void RotZ(double angle, ref double x, ref double y)
        {
            double x1 = x * System.Math.Cos(angle) - y * System.Math.Sin(angle);
            double y1 = x * System.Math.Sin(angle) + y * System.Math.Cos(angle);
            x = x1;
            y = y1;
        }

        public static double MapCoordinate(double i1, double i2, double w1, double w2, double p)
        {
            return ((p - i1) / (i2 - i1)) * (w2 - w1) + w1;
        }
    }
}