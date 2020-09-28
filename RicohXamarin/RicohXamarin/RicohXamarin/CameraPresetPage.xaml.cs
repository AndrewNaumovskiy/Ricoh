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
    public partial class CameraPresetPage : ContentPage
    {
        private CameraPresetPageVM _vm;

        public CameraPresetPage(CameraPresetPageVM VM)
        {
            InitializeComponent();

            _vm = VM;

            BindingContext = VM;
        }
    }
}