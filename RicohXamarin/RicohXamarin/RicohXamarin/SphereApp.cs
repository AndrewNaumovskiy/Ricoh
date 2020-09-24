using System;
using System.Collections.Generic;
using System.Drawing;
//using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using UIKit;
using Urho;
using Urho.Urho2D;
using Xamarin.Forms;
using Application = Urho.Application;
using Color = Urho.Color;
using Image = Urho.Resources.Image;

namespace RicohXamarin
{
	public class SphereApp : Application
	{
		public bool IsSceneLoaded { get; private set; }

		float yaw;
		float pitch;
		float roll;
		const float Sensitivity = .025f;
		Node cameraNode;

		private Camera camera;

        private Image image;

        private Scene scene;

        private StaticModel modelObject;

        private Node node;

        private Material material;

        private Texture2D texture;

        public SphereApp(ApplicationOptions options) : base(options)
		{
			UnhandledException += Application_UnhandledException;
		}

		public SphereApp(IntPtr handle) : base(handle)
		{
			UnhandledException += Application_UnhandledException;
		}

		protected SphereApp(UrhoObjectFlag emptyFlag) : base(emptyFlag)
		{
			UnhandledException += Application_UnhandledException;
		}

		private void Application_UnhandledException(object sender, Urho.UnhandledExceptionEventArgs e)
		{
			e.Handled = true;
		}

		protected override async void Start()
		{
			base.Start();
			await CreateScene();
		}

		public async Task CreateScene()
		{
			// 1 - SCENE
			scene = new Scene();
			scene.CreateComponent<Octree>();

			// 2 - NODE
			node = scene.CreateChild("room");
			node.Position = new Vector3(0, 0, 0);
			node.Rotation = new Quaternion(0, 0, 0);
			node.SetScale(2f);

			// 3 - MODEL OBJECT
			modelObject = node.CreateComponent<StaticModel>();
			modelObject.Model = ResourceCache.GetModel("Models/Sphere.mdl");

			// 3.2 - ZONE
			var zoneNode = scene.CreateChild("zone");
			var zone = zoneNode.CreateComponent<Zone>();
			zone.SetBoundingBox(new BoundingBox(-300.0f, 300.0f));
			zone.AmbientColor = new Color(1f, 1f, 1f);

			// 3.5 - DOWNLOAD IMAGE
			//var webClient = new WebClient() { Encoding = Encoding.UTF8 };
            //
			//// NOTE: The image MUST be in power of 2 resolution (Ex: 512x512, 2048x1024, etc...)
			//var memoryBuffer = new MemoryBuffer(webClient.DownloadData(new Uri("https://video.360cities.net/littleplanet-360-imagery/360Level43Lounge-8K-stable-noaudio-2048x1024.jpg")));

			//UIImage uiimage = UIImage.FromBundle("R0010015.JPG");
			//
			//var memoryBuffer = new MemoryBuffer(uiimage.AsJPEG().ToArray());

            image = new Image();

            //var memory = new MemoryBuffer(memoryBuffer);

            //var isLoaded = image.Load(memoryBuffer);
            //
            //if (!isLoaded)
            //{
            //    throw new Exception();
            //}

            //REMOVE

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

            //await Task.Run(() =>
            //{
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


                        // Оставьте imageBytes пустым
                        //SetImage(imageBytes.ToArray());
                        break;
                        imageBytes.Clear();

                        // Вернитесь к бинарному циклу сбора данных в начале изображения.
                        isLoadStart = false;

                    }
                }

                oldByte = byteData;
            }
            //});

            // REMOVE

            image = new Image();
            //



            var memory = new MemoryBuffer(new MemoryStream(imageBytes.ToArray()));

            var isLoaded = image.Load(memory);

            var mefw = image.Resize(2048, 1024);

            // 3.6 TEXTURE
            texture = new Texture2D();
            var isTextureLoaded = texture.SetData(image);

            //if (!isTextureLoaded)
            //{
            //    throw new Exception();
            //}

			// 3.8 - MATERIAL
            material = new Material();
            material.SetTexture(TextureUnit.Diffuse, texture);
            material.SetTechnique(0, CoreAssets.Techniques.DiffNormal, 0, 0);
            material.CullMode = CullMode.Cw;
            modelObject.SetMaterial(material);

            // 4 - LIGHTS
            Node light = scene.CreateChild(name: "light");
            light.SetDirection(new Vector3(0f, -0f, 0f));
            light.CreateComponent<Light>();

            // 5 - CAMERA
            cameraNode = scene.CreateChild(name: "camera");
            cameraNode.LookAt(new Vector3(0, 1, 2), new Vector3(0, 1, 0));
            camera = cameraNode.CreateComponent<Camera>();
            camera.Fov = 50;
            camera.Orthographic = false;

            cameraNode.Rotation = new Quaternion(0, 0, 0);

			// 6 - VIEWPORT
			Renderer.SetViewport(0, new Viewport(scene, camera, null));

			// 7 - ACTIONS
			//await node.RunActionsAsync(new RepeatForever(new RotateBy(duration: 4f, deltaAngleX: 0, deltaAngleY: 40, deltaAngleZ: 0)));
			IsSceneLoaded = true;
		}

		private float prev;

		protected override void OnUpdate(float timeStep)
		{
			base.OnUpdate(timeStep);
			if (Input.NumTouches == 1 && IsSceneLoaded)
			{
				var touch = Input.GetTouch(0);
				yaw += Sensitivity * touch.Delta.X;

				pitch += Sensitivity * touch.Delta.Y;

				pitch = MathHelper.Clamp(pitch, -90, 90);

				roll = 0;

				cameraNode.Rotation = new Quaternion(-pitch, -yaw, roll);
			}

			if (Input.NumTouches > 1 && IsSceneLoaded)
			{
				var f = Input.GetTouch(0);
				var s = Input.GetTouch(1);

				var distance = IntVector2.Distance(f.Position, s.Position);
				//System.Console.WriteLine($"{distance - prev}");
				camera.Fov += (distance - prev);
				prev = distance;
            }
		}

        public void SetImage(byte[] arr)
        {
            //UIImage uiimage = UIImage.FromBundle("R0010015.JPG");
            //
            //var memoryBuffer = new MemoryBuffer(uiimage.AsJPEG().ToArray());

            var webClient = new WebClient() { Encoding = Encoding.UTF8 };
            //
            //// NOTE: The image MUST be in power of 2 resolution (Ex: 512x512, 2048x1024, etc...)
            var memoryBuffer = new MemoryBuffer(webClient.DownloadData(new Uri("https://video.360cities.net/littleplanet-360-imagery/360Level43Lounge-8K-stable-noaudio-2048x1024.jpg")));

            image = new Image();
                //

                var memory = new MemoryBuffer(new MemoryStream(arr));

                //var isLoaded = image.Load(memory);
                var isLoaded = image.Load(memoryBuffer);

                if (!isLoaded)
                {
                    throw new Exception();
                }

                // 3.6 TEXTURE
                //texture = new Texture2D();

                //

                var resize = image.Resize(2048, 1024);


            var isTextureLoaded = texture.SetData(image);

            //if (!isTextureLoaded)
            //{
            //    throw new Exception();
            //}

            // 3.8 - MATERIAL
           material.SetTexture(TextureUnit.Diffuse, texture);

                material.SetTechnique(0, CoreAssets.Techniques.DiffNormal, 0, 0);
                material.CullMode = CullMode.Cw;
                modelObject.SetMaterial(material);

                //Renderer.SetViewport(0, new Viewport(scene, camera, null));



        }

        public void StartConnection()
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

            //await Task.Run(() =>
            //{
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
                            
                            
                            // Оставьте imageBytes пустым
                            SetImage(imageBytes.ToArray());
                            break;
                            imageBytes.Clear();

                            // Вернитесь к бинарному циклу сбора данных в начале изображения.
                            isLoadStart = false;

                        }
                    }

                    oldByte = byteData;
                }
            //});
        }

        public byte[] ScaleImage(byte[] arr)
        {
            float height = 1024;
            float width = 2048;

            try
            {
                NSData data = NSData.FromArray(arr);
                UIImage image = UIImage.LoadFromData(data);
                CGSize scaleSize = new CGSize(width, height);
                UIGraphics.BeginImageContextWithOptions(scaleSize, false, 0);
                image.Draw(new CGRect(0, 0, scaleSize.Width, scaleSize.Height));
                UIImage resizedImage = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();
                return resizedImage.AsJPEG().ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
