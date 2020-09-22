using System;
using System.Collections.Generic;
using System.Text;
using AudioUnit;
using MediaPlayer;
using Xamarin.Forms;

namespace RicohXamarin
{
    public class VM : ViewModelBase
    {
        private ImageSource _image;

        public ImageSource Image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        public void SetImage(ImageSource img)
        {
            Image = img;
        }
    }
}
