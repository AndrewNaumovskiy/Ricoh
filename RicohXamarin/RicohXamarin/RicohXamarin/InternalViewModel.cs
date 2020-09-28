using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace RicohXamarin
{
    public class InternalViewModel : ViewModelBase
    {
        private ThetaGalleryPage _galleryPage;
        private ThetaGalleryPageViewModel _galleryPageVM;

        private CameraPresetPage _cameraPresetPage;
        private CameraPresetPageVM _cameraPresetPageVM;

        private ImageSource _image;

        private string _batteryPercent;
        private bool _charging;
        
        private bool _showConnectionLostVisible;

        public bool _queueRunning;


        private RelayCommand _shotCommand;
        private RelayCommand _reStartConnectionCommand;
        private RelayCommand _openGalleryCommand;
        private RelayCommand _popPageCommand;
        private RelayCommand _openCameraPresetCommand;

        public ImageSource Image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        public Color BackgroundColor { get; set; } = new Color(0,0,0,0.3);

        public Color LostConnectionBackGround { get; set; } = new Color(1, 0, 0, 0.3);

        public string BatteryPercent
        {
            get => _batteryPercent;
            set
            {
                _batteryPercent = value;
                OnPropertyChanged(nameof(BatteryPercent));
            }
        }

        public bool Charging
        {
            get => _charging;
            set
            {
                _charging = value;
                OnPropertyChanged(nameof(Charging));
            }
        }

        public bool ShowConnectionLostVisible
        {
            get => _showConnectionLostVisible;
            set
            {
                _showConnectionLostVisible = value;
                OnPropertyChanged(nameof(ShowConnectionLostVisible));
            }
        }

        public RelayCommand ShotCommand
        {
            get
            {
                if(_shotCommand == null)
                    _shotCommand = new RelayCommand((param) => ShotAction());

                return _shotCommand;
            }
        }

        public RelayCommand ReStartConnectionCommand
        {
            get
            {
                if(_reStartConnectionCommand == null)
                    _reStartConnectionCommand = new RelayCommand((param) => CheckConnection());

                return _reStartConnectionCommand;
            }
        }

        public RelayCommand OpenGalleryCommand
        {
            get
            {
                if(_openGalleryCommand == null)
                    _openGalleryCommand = new RelayCommand((param) => OpenGalleryAction());

                return _openGalleryCommand;
            }
        }

        public RelayCommand PopPageCommand
        {
            get
            {
                if(_popPageCommand == null)
                    _popPageCommand = new RelayCommand((param) => PopPageAction());

                return _popPageCommand;
            }
        }

        public RelayCommand OpenCameraPresetCommand
        {
            get
            {
                if(_openCameraPresetCommand == null)
                    _openCameraPresetCommand = new RelayCommand((param) => OpenCameraPresetAction());

                return _openCameraPresetCommand;
            }
        }

        public InternalViewModel()
        {
            //StartConnectionCheckQueue();

            _galleryPageVM = new ThetaGalleryPageViewModel();
            _galleryPage = new ThetaGalleryPage(_galleryPageVM);

            _cameraPresetPageVM = new CameraPresetPageVM();
            _cameraPresetPage = new CameraPresetPage(_cameraPresetPageVM);
        }

        public void SetImage()
        {
            Image = ImageSource.FromFile(Path.GetFullPath("R0010016.JPG"));
        }

        public async void StartConnectionCheckQueue()
        {
            CheckConnection();
            await Task.Delay(300);
            //GetLivePreview();
            SetImage();
            _queueRunning = true;
        }

        private async void CheckConnection()
        {
            await Task.Run( async () =>
            {
                while (_queueRunning)
                {
                    var status = await ThetaProxy.CheckConnection();

                    if (!string.IsNullOrEmpty(status))
                    {
                        _showConnectionLostVisible = false;
                        ShowConnectionLostVisible = false;
                        SetValuesFromCamera(status);
                    }
                    else
                    {
                        ShowConnectionLostVisible = true;
                        _queueRunning = false;
                    }

                    await Task.Delay(10000);
                }
            });
        }


        private async void GetLivePreview()
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
                while (_queueRunning)
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

        private void SetValuesFromCamera(string text)
        {
            JObject json = JsonConvert.DeserializeObject<JObject>(text);

            var State = json.SelectToken("state");

            // ---- battery percent

            double batteryas = double.TryParse(State.SelectToken("batteryLevel").ToString(), out batteryas) ? batteryas : 0;

            BatteryPercent = batteryas * 100 + "%";

            // ---- batteryState -> charging or not

            //state -> _batteryState -> disconnect, charging, charged

            Charging = State.SelectToken("_batteryState").ToString() != "disconnect";
        }

        private async void ShotAction()
        {
            _queueRunning = false;

            await Task.Delay(100);

            // Shot
        }

        private void OpenGalleryAction()
        {
            _queueRunning = false;
            //ShowConnectionLostVisible = true;
            _galleryPageVM.GetImages();
            App.Current.MainPage.Navigation.PushAsync(_galleryPage, true);
        }

        private void SetImage(byte[] arr)
        {
            Image = ImageSource.FromStream(() => new MemoryStream(arr));
        }

        private void PopPageAction()
        {
            App.Current.MainPage.Navigation.PopAsync(true);
        }

        private void OpenCameraPresetAction()
        {
            _cameraPresetPageVM.PreparePresets();
            App.Current.MainPage.Navigation.PushAsync(_cameraPresetPage, true);
        }
    }
}
