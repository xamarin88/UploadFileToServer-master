//using Android.Graphics;
//using Android.Media;
//using Plugin.Media;
//using Plugin.Media.Abstractions;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using Xamarin.Forms;
//using Xamarin.Forms.Xaml;

using System;
using System.Net.Http;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UploadToServer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MediaPage : ContentPage
    {
        private MediaFile _mediaFile;

        public MediaPage()
        {
            InitializeComponent();

        }
        private async void TakePhoto_Clicked(object sender, EventArgs e)
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                DisplayAlert("No Camera", ":( No camera avaialble.", "OK");
                return;
            }

            var _mediaFile = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                Directory = "Sample",
                Name = "test.jpg"
            });

            if (_mediaFile == null)
            return;

            //DisplayAlert("File Location", file.Path, "OK");
            sourcePath.Text = _mediaFile.Path;

            image.Source = ImageSource.FromStream(() =>
            {
                var stream = _mediaFile.GetStream();
                _mediaFile.Dispose();
                return stream;
            });
        }

        private async void PickPhoto_Clicked(object sender, EventArgs e)
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                return;
            }
            var _mediaFile = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
            });


            if (_mediaFile == null)
                return;

            sourcePath.Text = _mediaFile.Path;

            image.Source = ImageSource.FromStream(() =>
            {
                var stream = _mediaFile.GetStream();
                _mediaFile.Dispose();
                return stream;
            });
        }

        private async void TakeVideo_Clicked(object sender, EventArgs e)
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakeVideoSupported)
            {
                DisplayAlert("No Camera", ":( No camera avaialble.", "OK");
                return;
            }

            var _mediaFile = await CrossMedia.Current.TakeVideoAsync(new Plugin.Media.Abstractions.StoreVideoOptions
            {
                Name = "video.mp4",
                Directory = "DefaultVideos",
            });

            if (_mediaFile == null)
                return;

            //DisplayAlert("Video Recorded", "Location: " + file.Path, "OK");
            sourcePath.Text = _mediaFile.Path;

            image.Source = ImageSource.FromStream(() =>
            {
                var stream = _mediaFile.GetStream();
                _mediaFile.Dispose();
                return stream;
            });

            _mediaFile.Dispose();
        }

        private async void PickVideo_Clicked(object sender, EventArgs e)
        {
            if (!CrossMedia.Current.IsPickVideoSupported)
            {
                DisplayAlert("Videos Not Supported", ":( Permission not granted to videos.", "OK");
                return;
            }
            var _mediaFile = await CrossMedia.Current.PickVideoAsync();

            if (_mediaFile == null)
                return;

            //DisplayAlert("Video Selected", "Location: " + file.Path, "OK");
            sourcePath.Text = _mediaFile.Path;
            _mediaFile.Dispose();
        }

        //private async void UploadFile_Clicked(object sender, EventArgs e)
        //{
        //    var content = new MultipartFormDataContent();

        //    content.Add(new StreamContent(_mediaFile.GetStream()),
        //       "\"file\"",
        //       $"\"{_mediaFile.Path}\"");

        //    var httpClient = new HttpClient();

        //    var uploadServiceBaseAddress = "http://uploadmediatoserver.azurewebsites.net/api/Files/Upload";
        //    //"http://localhost:12214/api/Files/Upload";

        //    var httpResponseMessage = await httpClient.PostAsync(uploadServiceBaseAddress, content);

        //    RemotePathLabel.Text = await httpResponseMessage.Content.ReadAsStringAsync();
        //}

        private async void UploadFile_Clicked(object sender, EventArgs e)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StreamContent(_mediaFile.GetStream()),
                "\"file\"",
                $"\"{_mediaFile.Path}\"");

            var httpClient = new HttpClient();

            var uploadServiceBaseAddress = "http://uploadmediatoserver.azurewebsites.net/api/Files/Upload";

            var httpResponseMessage = await httpClient.PostAsync(uploadServiceBaseAddress, content);

            RemotePathLabel.Text = await httpResponseMessage.Content.ReadAsStringAsync();
        }
    }
}