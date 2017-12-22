using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions.Abstractions;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;

namespace UploadToServer
{
    public partial class MainPage : ContentPage
    {
        private MediaFile _mediaFile;
        Position savedPosition;

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

            lblMessage.Text = "Generating Photo...";

            //myImage.jpg need to combine sender phone number
            _mediaFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "myImage.jpg"
                //CompressionQuality = 92
            });

           if (_mediaFile == null)
                return;

            lblMessage.Text = "Ready to go";

            //LocalPathLabel.Text = _mediaFile.Path;

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

            lblMessage.Text = "Generating Video...";

            _mediaFile = await CrossMedia.Current.TakeVideoAsync(new StoreVideoOptions
            {
                Directory = "DefaultVideos",
                Name = "video.mp4"
            });

            if (_mediaFile == null)
                return;

            lblMessage.Text = "Ready to go";

            //LocalPathLabel.Text = _mediaFile.Path;

            FileImage.Source = ImageSource.FromStream(() =>
            {
                return _mediaFile.GetStream();
            });
        }

        private async void UploadFile_Clicked(object sender, EventArgs e)
        {
            if (lblMessage.Text == "Ready to go")
            {
                savedPosition = null;
                lblMessage.Text = "Uploading...";

                var content = new MultipartFormDataContent();

                content.Add(new StreamContent(_mediaFile.GetStream()),
                   "\"file\"",
                   $"\"{_mediaFile.Path}\"");

                var httpClient = new HttpClient();

                var uploadServiceBaseAddress = "http://uploadmediatoserver.azurewebsites.net/api/Files/Upload";

                var httpResponseMessage = await httpClient.PostAsync(uploadServiceBaseAddress, content);
                var blobPathInfo = await httpResponseMessage.Content.ReadAsStringAsync();
                var unQuotedblobPathInfo = blobPathInfo.TrimStart('"').TrimEnd('"');
                // If the characters are the same, then you only need one call to Trim('"'):
                unQuotedblobPathInfo = blobPathInfo.Trim('"');
                UpdateSenderInfo(unQuotedblobPathInfo);
            }
            else
            {
                lblMessage.Text = "Your image still generating... Please wait a moment...";
            }
        }

        private async void UpdateSenderInfo(string blobPathInfo)
        {
            try
            {
                GetGPS();
                
                //var obj = new Models.BlobData
                //{

                //    filePath = blobPathInfo,
                //    fileExt = blobPathInfo.Split('.').Last(),
                //    senderNumber = blobPathInfo,
                //    senderLat = System.Convert.ToDecimal(savedPosition.Latitude),
                //    senderLong = System.Convert.ToDecimal(savedPosition.Longitude),
                //};

                var obj = new Models.BlobData
                {

                    filePath = blobPathInfo,
                    fileExt = blobPathInfo.Split('.').Last(),
                    senderNumber = blobPathInfo,
                    senderLat = System.Convert.ToDecimal(savedPosition.Latitude),
                    senderLong = System.Convert.ToDecimal(savedPosition.Longitude),
                };

                var uploadServiceBaseAddress = "http://uploadmediatoserver.azurewebsites.net/api/Files/UpdateBlobData";

                var client = new HttpClient();
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                var content = new StringContent(json, Encoding.Unicode, "application/json");
                HttpResponseMessage response = null;
                response = await client.PostAsync(uploadServiceBaseAddress, content);
                if (response.IsSuccessStatusCode)
                {
                    //DisplayAlert("Upload successfully....", "success", "ok");
                    lblMessage.Text = "Upload successfully";
                }
                else
                {
                    //DisplayAlert("Failed to upload...", "failed", "ok");
                    lblMessage.Text = "Upload Failed";
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("An error occurred: '{0}'", ex.Message, "ok");
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
                
                var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10), null, false);
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
                    //labelGPS.Text = string.Format("Time: {0} \nLat: {1} \nLong: {2} \nAltitude: {3} \nAltitude Accuracy: {4} \nAccuracy: {5} \nHeading: {6} \nSpeed: {7}",
                    //    position.Timestamp, position.Latitude, position.Longitude,
                    //    position.Altitude, position.AltitudeAccuracy, position.Accuracy, position.Heading, position.Speed);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Something went wrong", ex.Message, "OK");
            }
        }

        //private async void ButtonGetGPS_Clicked(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        var hasPermission = await Utils.CheckPermissions(Permission.Location);
        //        if (!hasPermission)
        //            return;

        //        ButtonGetGPS.IsEnabled = false;

        //        var locator = CrossGeolocator.Current;
        //        locator.DesiredAccuracy = 500;
        //        labelGPS.Text = "Getting gps...";

        //        var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(20), null, false);

        //        if (position == null)
        //        {
        //            labelGPS.Text = "null gps :(";
        //            return;
        //        }
        //        savedPosition = position;
        //        ButtonAddressForPosition.IsEnabled = true;
        //        labelGPS.Text = string.Format("Time: {0} \nLat: {1} \nLong: {2} \nAltitude: {3} \nAltitude Accuracy: {4} \nAccuracy: {5} \nHeading: {6} \nSpeed: {7}",
        //            position.Timestamp, position.Latitude, position.Longitude,
        //            position.Altitude, position.AltitudeAccuracy, position.Accuracy, position.Heading, position.Speed);

        //    }
        //    catch (Exception ex)
        //    {
        //        await DisplayAlert("Uh oh", "Something went wrong, but don't worry we captured for analysis! Thanks.", "OK");
        //    }
        //    finally
        //    {
        //        ButtonGetGPS.IsEnabled = true;
        //    }
        //}

        //private async void ButtonAddressForPosition_Clicked(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (savedPosition == null)
        //            return;

        //        var hasPermission = await Utils.CheckPermissions(Permission.Location);
        //        if (!hasPermission)
        //            return;

        //        ButtonAddressForPosition.IsEnabled = false;

        //        var locator = CrossGeolocator.Current;

        //        var address = await locator.GetAddressesForPositionAsync(savedPosition, "RJHqIE53Onrqons5CNOx~FrDr3XhjDTyEXEjng-CRoA~Aj69MhNManYUKxo6QcwZ0wmXBtyva0zwuHB04rFYAPf7qqGJ5cHb03RCDw1jIW8l");
        //        if (address == null || address.Count() == 0)
        //        {
        //            LabelAddress.Text = "Unable to find address";
        //        }

        //        var a = address.FirstOrDefault();
        //        LabelAddress.Text = $"Address: Thoroughfare = {a.Thoroughfare}\nLocality = {a.Locality}\nCountryCode = {a.CountryCode}\nCountryName = {a.CountryName}\nPostalCode = {a.PostalCode}\nSubLocality = {a.SubLocality}\nSubThoroughfare = {a.SubThoroughfare}";

        //    }
        //    catch (Exception ex)
        //    {
        //        await DisplayAlert("Uh oh", "Something went wrong, but don't worry we captured for analysis! Thanks.", "OK");
        //    }
        //    finally
        //    {
        //        ButtonAddressForPosition.IsEnabled = true;
        //    }
        //}

    }
}
