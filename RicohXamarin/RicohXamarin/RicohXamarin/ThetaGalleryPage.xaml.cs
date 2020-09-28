using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RicohXamarin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ThetaGalleryPage : ContentPage
    {
        public ThetaGalleryPage(ThetaGalleryPageViewModel VM)
        {
            InitializeComponent();

            BindingContext = VM;
        }
    }
}