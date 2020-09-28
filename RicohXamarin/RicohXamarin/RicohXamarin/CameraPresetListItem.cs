using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace RicohXamarin
{
    public class CameraPresetListItem : ViewModelBase
    {
        private string _name;
        private string _aperture;
        private string _exposureCompensation;
        private string _iso;

        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        [JsonProperty(PropertyName = "aperture")]
        public string Aperture
        {
            get => _aperture;
            set
            {
                _aperture = value;
                OnPropertyChanged(nameof(Aperture));
            }
        }

        [JsonProperty(PropertyName = "exposureCompensation")]
        public string ExposureCompensation
        {
            get => _exposureCompensation;
            set
            {
                _exposureCompensation = value;
                OnPropertyChanged(nameof(ExposureCompensation));
            }
        }

        [JsonProperty(PropertyName = "iso")]
        public string Iso
        {
            get => _iso;
            set
            {
                _iso = value;
                OnPropertyChanged(nameof(Iso));
            }
        }
    }
}
