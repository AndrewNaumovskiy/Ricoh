using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RicohXamarin
{
    public interface IFileWorker
    {
        string GetHandheldImage(string filename);
        Task<bool> SaveHandheldImageAsync(string filename, ImageSource imageSource);
    }
}
