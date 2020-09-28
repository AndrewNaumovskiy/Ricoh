using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace RicohXamarin
{
    public class ThetaImageItem : ViewModelBase
    {
        private ImageSource _thumbnailImage;

        [JsonProperty(PropertyName = "dateTimeZone")]
        public string DateTimeZone { get; set; }

        [JsonProperty(PropertyName = "fileUrl")]
        public string FileUrl { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "thumbnail")]
        public string Thumbnail { get; set; }


        public ImageSource ThumbnailImage
        {
            get => _thumbnailImage;
            set
            {
                _thumbnailImage = value;
                OnPropertyChanged(nameof(Thumbnail));
            }
        }

        public void SetThumbnailImage()
        {
            byte[] bytes = Convert.FromBase64String(Thumbnail);
            ThumbnailImage = ImageSource.FromStream(() =>new MemoryStream(bytes));
        }
    }
}
