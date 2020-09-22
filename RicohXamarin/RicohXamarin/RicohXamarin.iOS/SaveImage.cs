using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace RicohXamarin.iOS
{
    class FileWorker : IFileWorker
    {
        public string GetHandheldImage(string filename)
        {
            var handheldsFolder = GetDocumentsFolderPath("handheldImages");
            return Path.Combine(handheldsFolder, filename);
        }

        private string GetDocumentsFolderPath(string subFolderName)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var folder = Path.Combine(documents, "AutoloadIT", subFolderName);
            //var guidesFolder = Path.Combine(documents, "..", "Library", "Caches");

            //if (Directory.Exists(guidesFolderDoc)) { Directory.Delete(guidesFolderDoc, true); }
            //if (!Directory.Exists(guidesFolder)){ Directory.CreateDirectory(guidesFolder); }
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
                NSFileManager.SetSkipBackupAttribute(folder, true);
            }

            return folder;
        }

        public async Task<bool> SaveHandheldImageAsync(string filename, ImageSource imageSource)
        {
            NSData imgData = null;
            var renderer = new Xamarin.Forms.Platform.iOS.StreamImagesourceHandler();
            UIKit.UIImage photo = await renderer.LoadImageAsync(imageSource);

            var savedImageFilename = GetHandheldImage(filename);
            var directory = Path.GetDirectoryName(savedImageFilename);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                NSFileManager.SetSkipBackupAttribute(directory, true);
            }

            //var isDirectory = true;
            //if(!NSFileManager.DefaultManager.FileExists(directory, ref isDirectory))
            //{
            //    if (!isDirectory) 
            //    { 
            //        NSFileManager.DefaultManager.CreateDirectory(directory, true, null);
            //        NSFileManager.SetSkipBackupAttribute(directory, true);
            //    }
            //}

            if (Path.GetExtension(filename).ToLower() == ".png")
                imgData = photo.AsPNG();
            else
                imgData = photo.AsJPEG(100);

            bool success = false;
            NSError err = null;
            success = await Task.Run(() => imgData.Save(savedImageFilename, NSDataWritingOptions.Atomic, out err));

            return success;
        }
    }
}