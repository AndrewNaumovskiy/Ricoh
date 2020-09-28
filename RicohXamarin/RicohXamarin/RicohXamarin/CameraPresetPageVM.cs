using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using CarPlay;
using Newtonsoft.Json;

namespace RicohXamarin
{
    public class CameraPresetPageVM : ViewModelBase
    {
        public ObservableCollection<CameraPresetListItem> CameraPresets { get; set; }

        public CameraPresetPageVM()
        {
            CameraPresets = new ObservableCollection<CameraPresetListItem>();

            var cameraPresets = JsonConvert.DeserializeObject<ObservableCollection<CameraPresetListItem>>(GetPresetValue());

            foreach (var item in cameraPresets)
            {
                CameraPresets.Add(item);
            }
        }

        public void PreparePresets()
        {
        }










        private string GetPresetValue()
        {
            return "[{\"name\":\"Showroom\",\"aperture\":\"0\",\"exposureCompensation\":\" + 1.0\",\"iso\":\"64\"},{\"name\":\"Shooting Scenario Outdoors\",\"aperture\":\"0\",\"exposureCompensation\":\"0\",\"iso\":\"64\"},{\"name\":\"Studio\",\"aperture\":\"0\",\"exposureCompensation\":\" + 0.7\",\"iso\":\"64\"}]";
        }
    }
}
