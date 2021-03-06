﻿using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;
using Plugin.Permissions.Abstractions;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UploadToServer
{
    public partial class MainPage : ContentPage
    {
        private MediaFile _mediaFile;
        Position savedPosition;
        decimal latitude = 0;
        decimal longitude = 0;
        string adminArea;
        string countryCode;
        string countryName;
        string featureName;
        string locality;
        string postalCode;
        string subAdminArea;
        string subLocality;
        string subThoroughFare;
        string thoroughFare;
        string filename;
        string fileExt;
        string filePath;
        string oriPath;
        string locAddress;

        //private IPlaybackController PlaybackController => CrossMediaManager.Current.PlaybackController;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void TakePhoto_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            GetGPS();

            lblMessage.Text = "Generating Photo...";

            //myImage.jpg need to combine case number
            _mediaFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "myImage.jpg"
                //CompressionQuality = 92
            });

           if (_mediaFile == null)
                return;

            FileImage.IsVisible = true;
            VideoImage.IsVisible = false;
            lblMessage.Text = "Ready to go";
            oriPath = _mediaFile.Path;
            
            filename = _mediaFile.Path.Split('\\').LastOrDefault().Split('/').LastOrDefault();
            fileExt = _mediaFile.Path.Split('.').Last();
            filePath = Constants.ImageRootPath + "/" + filename;

            FileImage.Source = ImageSource.FromStream(() =>
            {
                return _mediaFile.GetStream();
            });
        }

        private async void TakeVideo_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            GetGPS();

            lblMessage.Text = "Generating Video...";

            _mediaFile = await CrossMedia.Current.TakeVideoAsync(new StoreVideoOptions
            {
                Directory = "DefaultVideos",
                Name = "video.mp4"
            });

            if (_mediaFile == null)
                return;

            FileImage.IsVisible = false;
            VideoImage.IsVisible = true;
            lblMessage.Text = "Ready to go";
            oriPath = _mediaFile.Path;

            filename = _mediaFile.Path.Split('\\').LastOrDefault().Split('/').LastOrDefault();
            fileExt = _mediaFile.Path.Split('.').Last();
            filePath = Constants.ImageRootPath + "/" + filename;

            //FileImage.Source = ImageSource.FromStream(() =>
            //{
            //    return _mediaFile.GetStream();
            //});
        }

        private async void UploadFile_Clicked(object sender, EventArgs e)
        {
            if (lblMessage.Text == "Ready to go")
            {
                await UpdateAzureStorage();
                await UpdateSenderInfo();
            }
            else
            {
                lblMessage.Text = "Your image still generating... Please wait a moment...";
            }
        }

        private async Task UpdateAzureStorage()
        {
            try
            {
                lblMessage.Text = "Uploading...";

                var content = new MultipartFormDataContent();

                content.Add(new StreamContent(_mediaFile.GetStream()),
                   "\"file\"",
                   $"\"{_mediaFile.Path}\"");

                var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromMinutes(3);
                var uploadServiceBaseAddress = "http://uploadmediatoserver.azurewebsites.net/api/Files/Upload";
                var httpResponseMessage = await httpClient.PostAsync(uploadServiceBaseAddress, content);
            }
            catch (HttpRequestException ex)
            {
                lblMessage.Text = string.Format("HttpRequestException Storage:An error occurred:: {0} \n", ex.Message);
                DisplayAlert("HttpRequestException Storage:An error occurred: '{0}'", ex.Message, "ok");
            }
            catch (TimeoutException ex2)
            {
                lblMessage.Text = string.Format("HttpRequestException Storage:An error occurred:: {0} \n", ex2.Message);
                DisplayAlert("HttpRequestException Storage:An error occurred: '{0}'", ex2.Message, "ok");
            }
        }

        private async Task UpdateSenderInfo()
        {
            try
            {
                var obj = new Models.BlobData
                {

                    filePath = filePath,
                    fileExt = fileExt,
                    senderNumber = filePath,
                    senderLat = latitude,
                    senderLong = longitude,
                    adminArea = adminArea,
                    countryCode = countryCode,
                    countryName = countryName,
                    featureName = featureName,
                    locality = locality,
                    postalCode = postalCode,
                    subAdminArea = subAdminArea,
                    subLocality = subLocality,
                    subThoroughFare = subThoroughFare,
                    thoroughFare = thoroughFare
                };

                var uploadServiceBaseAddress = "http://uploadmediatoserver.azurewebsites.net/api/Files/UpdateBlobData2";

                var client = new HttpClient();
                client.Timeout = TimeSpan.FromMinutes(3);
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                var content = new StringContent(json, Encoding.Unicode, "application/json");
                HttpResponseMessage response = null;
                response = await client.PostAsync(uploadServiceBaseAddress, content);
                if (response.IsSuccessStatusCode)
                {
                    lblMessage.Text = "Upload successfully";
                }
                else
                {
                    lblMessage.Text = "Upload Failed";
                }
            }
            catch (HttpRequestException ex)
            {
                lblMessage.Text = string.Format("HttpRequestException DB:An error occurred:: {0} \n", ex.Message);
                DisplayAlert("HttpRequestException DB:An error occurred: '{0}'", ex.Message, "ok");
            }
            catch (TimeoutException ex2)
            {
                lblMessage.Text = string.Format("HttpRequestException DB:An error occurred:: {0} \n", ex2.Message);
                DisplayAlert("HttpRequestException DB:An error occurred: '{0}'", ex2.Message, "ok");
            }
        }
       
        private async void GetGPS()
        {
            try
            {
                var hasPermission = await Utils.CheckPermissions(Permission.Location);
                if (!hasPermission)
                    return;
                
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 500;
                labelGPS.Text = "Getting gps...";
                
                var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(30), null, false);
                if (position == null)
                {
                    savedPosition = null;
                    labelGPS.Text = "null gps :(";
                    return;
                }
                else
                {
                    savedPosition = position;
                    labelGPS.Text = string.Format("Time: {0} \nLat: {1} \nLong: {2} \nAltitude: {3}",
                        position.Timestamp, position.Latitude, position.Longitude, position.Altitude);
                    latitude = System.Convert.ToDecimal(position.Latitude);
                    longitude = System.Convert.ToDecimal(position.Longitude);

                    var address = await locator.GetAddressesForPositionAsync(savedPosition, null);
                    if (address == null || address.Count() == 0)
                    {
                        locAddress = "Unable to find address";
                    }

                    var a = address.FirstOrDefault();
                    locAddress = $"Address: Thoroughfare = {a.Thoroughfare}\nLocality = {a.Locality}\nCountryCode = {a.CountryCode}\nCountryName = {a.CountryName}\nPostalCode = {a.PostalCode}\nSubLocality = {a.SubLocality}\nSubThoroughfare = {a.SubThoroughfare}";
                    adminArea = a.AdminArea;
                    countryCode = a.CountryCode;
                    countryName = a.CountryName;
                    featureName = a.FeatureName;
                    locality = a.Locality;
                    postalCode = a.PostalCode;
                    subAdminArea = a.SubAdminArea;
                    subLocality = a.SubLocality;
                    subThoroughFare = a.SubThoroughfare;
                    thoroughFare = a.Thoroughfare;
                }
                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Something went wrong", ex.Message, "OK");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CrossMediaManager.Current.StatusChanged += CurrentOnStatusChanged;
        }

        protected override void OnDisappearing()
        {
            CrossMediaManager.Current.StatusChanged -= CurrentOnStatusChanged;
            base.OnDisappearing();
        }

        private void PlayStop_Clicked(object sender, EventArgs e)
        {
            string videoUrl = oriPath;
            if (playStopButton.Text.ToUpper() == "PLAYBACK")
            {
                CrossMediaManager.Current.Play(videoUrl, MediaFileType.Video);
            }
            else
            {
                CrossMediaManager.Current.Stop();
            }
        }

        private void CurrentOnStatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.Status == MediaPlayerStatus.Playing)
            {
                playStopButton.Text = "Stop";
            }
            else
            {
                playStopButton.Text = "PlayBack";
            }
        }
    }
}
