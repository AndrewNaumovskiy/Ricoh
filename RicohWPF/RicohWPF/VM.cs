using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using RicohWPF.ViewModels;

namespace RicohWPF
{
    public class VM : ViewModelBase
    {
        private int i = 0;

        private BitmapImage _image;

        public BitmapImage Image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        List<BitmapImage> images = new List<BitmapImage>();

        public void SetImage(BitmapImage img)
        {
            //if (Image == null)
                Image = img;
            //images.Add(img);
        }

        public void ShowNext()
        {
            Image = images[i++];
        }
    }
}
