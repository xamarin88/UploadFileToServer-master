﻿//using Android.Graphics;
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

            var blobPathInfo = await httpResponseMessage.Content.ReadAsStringAsync();
            var unQuotedblobPathInfo = blobPathInfo.TrimStart('"').TrimEnd('"');
            // If the characters are the same, then you only need one call to Trim('"'):
            unQuotedblobPathInfo = blobPathInfo.Trim('"');
            //UpdateSenderInfo(unQuotedblobPathInfo);
        }

        //private async void UpdateSenderInfo(string blobPathInfo)
        //{
        //    try
        //    {
        //        var obj = new Models.BlobData
        //        {

        //            filePath = blobPathInfo,
        //            fileExt = blobPathInfo.Split('.').Last(),
        //            senderNumber = blobPathInfo,
        //            senderLoc = 1
        //        };

        //        var uploadServiceBaseAddress = "http://uploadmediatoserver.azurewebsites.net/api/Files/UpdateBlobData";

        //        var client = new HttpClient();

        //        string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        //        var content = new StringContent(json, Encoding.Unicode, "application/json");

        //        HttpResponseMessage response = null;
        //        response = await client.PostAsync(uploadServiceBaseAddress, content);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            DisplayAlert("success", "success", "ok");
        //        }
        //        else
        //        {
        //            DisplayAlert("failed", "failed", "ok");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DisplayAlert("An error occurred: '{0}'", ex.Message, "ok");
        //    }
        //}

        //private async void UploadFile2_Clicked(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        //IFolder rootFolder = await FileSystem.Current.GetFolderFromPathAsync(_mediaFile.Path);
        //        //var file = await rootFolder.GetFileAsync("my_image_name.jpg");

        //        IFile file = await FileSystem.Current.GetFileFromPathAsync(_mediaFile.Path);
        //        Stream stream = await file.OpenAsync(FileAccess.Read);

        //        using (var streamReader = new StreamReader(stream))
        //        {
        //            using (var memstream = new MemoryStream())
        //            {
        //                streamReader.BaseStream.CopyTo(memstream);
        //                byte[] bytes = memstream.ToArray();
        //                String upImages = Convert.ToBase64String(bytes);

        //                //byte[] aaa = Encoding.UTF8.GetBytes("sssss");
        //                //String bbb = Convert.ToBase64String(aaa);
        //                var obj1 = new Models.Image
        //                {
        //                    //imageData = bytes,
        //                    //imageData = bbb,
        //                    imageData = upImages,
        //                    filename = file.Name,
        //                    senderID = Guid.NewGuid(),
        //                };

        //                var uploadServiceBaseAddress = "http://uploadmediatoserver.azurewebsites.net/api/Files/UploadMedia2";

        //                var client = new HttpClient();

        //                string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj1);
        //                var content = new StringContent(json, Encoding.UTF8, "application/json");

        //                //to do upload to blob storage

        //                ////1st approach
        //                //var result = client.PostAsync(uploadServiceBaseAddress, content).Result;

        //                //if (result.IsSuccessStatusCode)
        //                //{
        //                //    var tokenJson = await result.Content.ReadAsStringAsync();
        //                //}


        //                //2nd approach
        //                HttpResponseMessage response = null;
        //                response = await client.PostAsync(uploadServiceBaseAddress, content);
        //                if (response.IsSuccessStatusCode)
        //                {
        //                    DisplayAlert("success", "success", "ok");
        //                }
        //                else
        //                {
        //                    DisplayAlert("failed", "failed", "ok");
        //                }
        //            }
        //        }

        //        //IFile file = await FileSystem.Current.GetFileFromPathAsync(_mediaFile.Path);

        //        //using (Stream stream1 = await file.OpenAsync(FileAccess.ReadAndWrite))
        //        //{
        //        //    byte[] filebytearray = new byte[stream1.Length];
        //        //    stream1.Read(filebytearray, 0, (int)stream1.Length);
        //        //    String upImages = Convert.ToBase64String(filebytearray);
        //        //    //upImages = "/9j/4Q9lRXhpZgAASUkqAAgAAAAPABABAgAJAAAAwgAAAAABBAABAAAAwBQAAAICBAABAAAA2goAABsBBQABAAAAywAAAA8BAgAIAAAA0wAAABIBAwABAAAAAQAAACWIBAABAAAATAMAABoBBQABAAAA2wAAADEBAgAOAAAA4wAAAGmHBAABAAAABQEAAAEBBAABAAAArAsAADIBAgAUAAAA8QAAACgBAwABAAAAAgAAAAECBAABAAAAgwQAABMCAwABAAAAAQAAABkEAABTTS1OOTIwOABIAAAAAQAAAHNhbXN1bmcASAAAAAEAAABOOTIwOFhYVTNDUUc0ADIwMTc6MTI6MTggMTI6MDY6MDAAGwCaggUAAQAAAE8CAAAiiAMAAQAAAAIAAACdggUAAQAAAFcCAAACoAMAAQAAAKwLAAADpAMAAQAAAAAAAAAEkAIAFAAAAF8CAACGkgcAFQAAAHMCAAACpAMAAQAAAAAAAAADkAIAFAAAAIgCAAABoAMAAQAAAAEAAAAFpAQAAQAAABwAAAADoAMAAQAAAMAUAAB8kgcAYgAAAJwCAAADkgoAAQAAAP4CAAAFkgUAAQAAAAYDAAAAoAcABAAAADAxMDABkgoAAQAAAA4DAAAJkgMAAQAAAAAAAAACkgUAAQAAABYDAAAEkgoAAQAAAB4DAAAniAMAAQAAAOgDAAAKkgUAAQAAACYDAAAIkgQAAQAAAAAAAAAAkAcABAAAADAyMjAgpAIAHgAAAC4DAAAGpAMAAQAAAAAAAAAHkgMAAQAAAAIAAAAAAAAAAQAAAAcAAAATAAAACgAAADIwMTc6MTI6MTggMTI6MDY6MDAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMjAxNzoxMjoxOCAxMjowNjowMAAHAAEABwAEAAAAMDEwMAIABAABAAAAACABAAwABAABAAAAAAAAABAABQABAAAAWgAAAEAABAABAAAAAAAAAFAABAABAAAAAQAAAAABAwABAAAAAAAAAAAAAAAAAAAAAAAAAN/9//9kAAAAuQAAAGQAAAAcAQAAZAAAALkAAABkAAAAAAAAAAoAAACuAQAAZAAAAEIxNkxMSUgwOFZNIEIxNkxMS0QwMUdNCgAAAAAAAAkAAAABAAQAAAACAgAABwAFAAMAAAC+AwAABQABAAEAAAAAAAAAAgAFAAMAAADWAwAABgAFAAEAAADuAwAAHQACAAsAAAD2AwAABAAFAAMAAAABBAAAAQACAAIAAABOAAAAAwACAAIAAABFAAAAAAAAAAQAAAABAAAABQAAAAEAAAA6AAAAAQAAAAIAAAABAAAAMQAAAAEAAAAWAAAAAQAAADkAAAABAAAAMjAxNzoxMjoxOABlAAAAAQAAADQAAAABAAAAFwAAAAEAAAAHABIBAwABAAAAAQAAAAEBBAABAAAAEAEAAAABBAABAAAA4AEAABoBBQABAAAAcwQAAAMBAwABAAAABgAAABsBBQABAAAAewQAACgBAwABAAAAAgAAAAAAAABIAAAAAQAAAEgAAAABAAAAFhU0AVQ1WX/OvgBILhdBoe5tXZGXK8MjxQqDPl8jQWqgkPN1Lq7lWSPLl5sO3eh9F/OU+mwtcjTUx3mivNKY7TWbVBVKAzr3rbycrSoCkKdb5VoqFMgSqqiiJ5Aq5dmch02jjzbZ5b46kSfnX4bKjszAtHTcGLFfh003EzNEZnswWKg+g1af83G9MxEyJ5BQsdsiyC5MmLomJQ0UCtBDzky40egEeoVZuOj1NpsXTf2BEtUK1ezReiKTYTV6yqofHiJvx7FHJIHR/wBtu3wQQmocirBJkzalEX6zDN9pzl1Mf6lGcj9djaw3SfwtWtAr50LAXdkZ2Syzx4SAOkSdAWWhmiCZLLpXHAb5YoEguEJFEBIfU5meuWR/JinUwsq0diqB2cBmZgrFdNsn4Iojlh3/AGUgGZTzBaRYJEcgr5kRVyFZkMkqyowYkc4BbaM/BIUTFT9f5ooUVdCTSrIJ7BYK3K//AD+dRS7oD5BRkZEbzKNPc2mSQCv/AFsGVpr0ZqCMhiH6+rMRMieQQuxhupWaPyxKG2hwDI+bSyEWgYpAiY3pd0Wf8x/g2CxKzrjpJnVaqWpQJ9VdZq1g1E06y8mkDMFun5J02vHaLzFJpN5pXypJkcF7hHsGLF1SjvUMNAt5uxiZ9cNrTSnsf6ZFAKETWoD5O5lmD15CsQPsFcc9UfRLcfCBILhT9R5Yna/C3WdZqjaHiiGhCulV2v7DABYRSpq+TwizMw05M+P2wE0Z+in4oTRU9gzqaRqo4QS9Bj2oQGKA8qx4VpMJKXRlBXyLu7DqgRVbujRqtZDErfHANgELqzBpEKzgE/bREVJHYJIqrDJiSFxxJnofWasRR5snZVtykUSQiQ6vx/h0Lsr0IiS0RleXtOqmKCFsQtTeXrwSiaN0ib3kSak/Xbn27KiC/wCkrTImqtTTMNgF2/8AU5CvRaegNXQuFiCmgoLNNWYn7OHFUVOW3knh1Px+UISnKvxN0pfHo7lgUS0gyr8AbCssjMGf0durSEnZnUqLEBm4UI//AFejkxTdJhCXI1x5egZWdUdGVILJ5gs4GLVtX1WYDyC1ZhUUKQEZxRO2lT02OHkZsy85CSdaxJNCUaBf00vOq/KKyIKds6iPtp7oysUNzQuElyHlIqvCKFoqKEO7J9Y6LLVVNsmdX2fRxJFbzZlCErV2noaK+aoWKp6fbt8oedBhQERsEYMoUq1pASo5MldO+6qgLrYtMRAMLUQAVn6dZpScIopLHAs98cFnYvKpRq/THTUgrkFXmk98pkY03+J/Irn2yKAEKqSyUddkECOPV9bdugEXzSgSDLMfx+GQ30CpNhrIZ8nHhS65FupyLZB8BOuNy/p6p6JFVK8bZZpb/jt+VuGg4l0gUzjR5QcPB5wCVgDTkUD5Eaq13AoKTyQDRvMflQgs4qKdGV0GANIT4lKE2QMnqm6q4hRkJ9PRLIiA1ULHGWJqFtRTqCOJB5MuuVYuVaizI9GXTuBQgK6MS+3PYo6LzJ1VGtPJcQEl9F+73g7vM2btZPSelYL6pVm+hWhadmZaHkf1X7b+Px1FbbhZqjKYcqhV4PKqT81CdMzRI3RyFOpdFiuw6ou83fXqnCSWIQhjUeE6eeSSxQhaN608Q90pygcslCrDEPzP2dXbUQHAdWFS/wBeVgyBLRSfoiT1x5oYpObBqIvqwpwxp/x3K4+iKtJGknVhI9QrBEVlFh5SLr24RWZQtinDkq3PeQXp6TLU74nysqVcegAFQX/iGVRywXkAkPtgPBWINpBdIhsdwe9D/t5IwmKZ2634+qVoGeVJoB8kijLzZ5MxFLTlJnLF/wDDOJOSgZyyKylin8bVXmoJfgkAOarNaCVXVqtOrIoNDtaBugDJS4LFlUl5aX0B5Hozezo6I57o2+h5Mf5lKdb9VT7F1iNAUi/aKqOWatJdqtaPwWsXenaEEwV/QgKasLL5cLQBJYLQSDaMnqllPB5ixnqp8VoEctNzFF2FY/yceacoPEENRmYIvbVOjFNohMZkvCbsKM1CwkNzRhOSKhp/1pH4bR0xvoR8GZ6Zf6FtVs/9SrT6oiTbUgrImxeYoLj0/Ck3B52FlUoIo9IqxHmH4aoBn+sEKspY0pw86NK/kGjWEiZBa7EOkQIgQaH9pU1K0lMrO9VVRRlo7zjjpwkLqppMddLtnARgOdvvWxk6+QmCtXps0yhNXxmojk0CTJerVp0szTQ+5mRSn/wq5kizbwEkhCVBoRajoC7MAPVf57btosR5qiojfzZFWywUiEpqmUFbFlNyzxk8x0TsoFKHdUo3nJCjkNkV4BRHDbE+rKR9IwEEd2viG10y4Ji5HrCb79KSujLYWJ4kFVHbRtMggHl9EvMNXhT0R+D+xqboVm/lUsAyCSzaYqVf1JLNjM1GtQKBWgaQJbkEJrMBmJoiR2zUM1Uocgnh2pOZerTDhg3AUK83C2IJ1v3RpEFCtFHQpPrEWfc57oipxRTbcYq6mgLN3dfjaVptrCptRbAkyPpOYPk8aBR4uSrbXzV6TDAOoLdMN/kCYyMJ17FJkSZirRZAj0UlxMuSJIQq0f4CsJBmn0OrnSunL/dW0CvXDVdCOlA8XEyoUkIq5ASbtVh/IMnIJvLIPixVTNMhwFdZsfUzFipYs4csmQVWvmrVV6zOOE/BKIsCeqoDIA8hQzZQWZ9fkBIh0GRQHpse3O/FFUhgeo+335IQqe5BECr6NQ0or/yBIAZ254op85rvlwfufLgd0ZDL50mqvRVm6hWCoyMuhMs3KAhXdgqN810dC/t/MKEEEvyqqqPZ58j1gzdntWUqaOqq0pjwK+rqq+kp6FztMHJ+axjZB5ya5tOSJpFoHYL8URgKAoQJ3NX0wyFmj9su9F7WdvNVmyTLtMJJZboTR3KUT+fSMLNUqTAkp/o3VpRs6gFQKCCcPF7FmbwvwAxajMpMaGZBDh3RXUI9J25ehDL3QDy6Sn2ciePyhWasetm9nZDVFcUBR/4EEaFcHeG/kUdFK+a0RZoh+xZldglFKFHZNEqxWbFgVC9Hz9Ntaq2UoqLuePtxWcyZ0eYedFa8aKjhmAoQql52XfzVd8u6CLNSrAI/1Czr/LgVYuaVJcnplodM3i5sNldf4okqpRT6KxFQFWZD43bRebezs1FR6DoRYlNATXtevzGPvHbyC3AJLBQfrurEG0IqwZGmyqrUJcctSovMo1VvugLyZ4igfFfkEuVUmWVqyEnxuZbq6aImxVWSTOm/Uz6Zon6hv9Plw0lDEzKzWsjpBTx/rNqwR2HDcyATSgdKZ0H7Au3lyy6CYdUikl81NTjxLAuspIJ9yZPslpczZPoFZn1Fv9jsmYSJkcwl9sPAS0r8IKacUt5Rm4jX/QLYvKYQIrZH9f5tlJ6qv5pr4qtgzIEYrpqs6v5hFcvIkKUKK2gWp+xJPh/6/hMxdxNVLIRNHU/PnyhXnrj4nESZElp9vJf4r/uCWVWalAMo7sKPSpVJ1+s5C0UftPEIytIFugZymLj1KrjH3jt5BJ4RC5BJsB6bVfVkjqluUReRROtND+lKFmkkUJaunHTKLIJdD3o3fY5J+o9oUVVdGWhmyr91Ey6PUWFfeT+kmcBSlZvQghPjWmaW1R1gzTRgEC0CqPr6qxJ1QGgBHygQymzv5gKkuy1lUWPc1dq10eGGwFmJH4FGPDEKeWCnn/X4lZxRAwli/+AAEEpGSUYAAQEAAAEAAQAA/9sAQwABAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEB/9sAQwEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEB/8AAEQgUwAusAwEiAAIRAQMRAf/EABwAAAMBAQEBAQEAAAAAAAAAAAIDBAEABQYHCv/EADoQAAMAAwEAAgMAAgICAQEAEwECAwQREhMhIgAUIzEyJDMFQUI0Q1FSFWFEU2JxJYFUYwZyZHORg//EABsBAAMBAQEBAQAAAAAAAAAAAAABAgMEBQYH/8QAQhEAAQIEBAMHBAICAQMEAQIHAQARAiExQRJRYYFxofAiMpGxwdHhAxNC8VJicoKiI5LCBEOy4hTS8mMzk1Ozg8P/2gAMAwEAAhEDEQA/AP5GqM8iT6oiCNJ3Vb03SSgBaF/etVyt15kW0ooUp8+fwbGX8SNIwAqhXk0o6pJnVcZSCAiUDTRlcklx6SMD35j1jBkRZyRqOwT+jMhqxVw71E4iIbr4LRl8saCn8T+GfB+jPpnNoMXm1FcdsyhW0VVCwHbAMeWpkzLP5/X8tEIBcDmV92JEEwsM2i9ynkEABTN3ZRkk1p02nZFC5Vy9VX7ljLteVq6J2W1rGxkdG9LJIxmJ9hSsdGdNSVHb0WTBw84uxFaVV3XlERyVoqIyJlwyMOKmjfYlX8qkapyG4dmmnav7bJUtpaA1rDUyzuKsAvyymUxz6Ijt2AhWjkKqOf5nbOxVkgTKsxQmRPIrHmiUZpETckUSn9SztaiyMSZzx+K0DGyMk5ptqKcOnPRXZFZyUZHnLi3oNPzJtGkqU+tXebMqU483QTX+K9fNJZ1q8ijTpoCisA3JE1UXWk/hopyykIQHRWrXe1/OdtcsAqzkXQNJmYGlZq8gWTy7FnYshm7krpX5KrtCIEsDyKQwOGravqksyPFRVyzxCEPy25wRoiLJQBS06oiyYf7qSnotdjnRzVEm01iPW6zpEK5MF6WKTSrr3GGztCSfRmbhOT0wSWLFaypstY1mHETGhNUpYrZHISaqemPToxS8yPBvyehmk5kEUKVYM4QyFLK6unj0Un/Xkigc0HaEGRPP5ScfdO3mES5M8n1QK7qkmSj0CgFpVCzu6I39Xkvmr/8A2MxnTt04HTkl7UAZqK1kWnauoWYUTDVmAVRxeShWOqq9CgUtwdZFEmKASWpFZWnMPwSKqTzZGJYpMVDsXilBL28yin7YhRGj0lIUFrLPbBpzj5VoKlQoF6NkMWlIGbsVZHawdfMUfT74petKGqOiNp7IzP3IswRpFcd5qpVgKOVaQKn+YITnzRxBqT7Is1jPmeQlKyx4uwRac0NUSiqv0DkopqUdu5r/AANuWXaHafPjUZIVAWXEokk4aoJZvb6msx9Jxeit6JKLKfs2xBIDNykl4oLkMJHFXTYwcUmxXRYVNAFUNVl/zr4ASC4XQaHubV2RlyvDI8UKgz5fI0FqoJDzdS6u5Vkjy5ebDt3ofRfzlPpsLXI01Md5orzSmO01m1QVSgM69628nK0qApCnW+VaKhTIEqqooieQKuXZnIdNo4822eW+OpEn51+Gyo7MwLR03BixX4dNNxMzRGZ7MFioPoNWn/NxvTMRMieQULHbIsguTJi6JiUNFArQQ85MuNHoBHqFWbjo9TabF039gRLVCtXs0Xoik2E1esqqHx4ib8exRySB0f8Abbt8EEJqHIqwSZM2pRF+swzfac5dTH+pRnI/XY2sN0n8LVrQK+dCwF3ZGdkss8eEgDpEnQFloZogmSy6VxwG+WKBILhCRRASH1OZnrlkfyYp1MLKtHYqgdnAZmYKxXTbJ+CKI5Yd/wBlIBmU8wWkWCRHIK+ZEVchWZDJKsqMGJHOAW2jPwSFExU/X+aKFFXQk0qyCewWCtyv/wA/nUUu6A+QUZGRG8yjT3NpkkAr/wBbBlaa9GagjIYh+vqzETInkELsYbqVmj8sShtocAyPm0shFoGKQImN6XdFn/Mf4NgsSs646SZ1WqlqUCfVXWatYNRNOsvJpAzBbp+SdNrx2i8xSaTeaV8qSZHBe4R7BixdUo71DDQLebsYmfXDa00p7H+mRQChE1qA+TuZZg9eQrED7BXHPVH0S3HwgSC4U/UeWJ2vwt1nWao2h4ohoQrpVdr+wwAWEUqavk8IszMNOTPj9sBNGfop+KE0VPYM6mkaqOEEvQY9qEBigPKseFaTCSl0ZQV8i7uw6oEVW7o0arWQxK3xwDYBC6swaRCs4BP20RFSR2CSKqwyYkhccSZ6H1mrEUebJ2VbcpFEkIkOr8f4dC7K9CIktEZXl7TqpighbELU3l68EomjdIm95EmpP1259uyogv8ApK0yJqrU0zDYBdv/AFOQr0WnoDV0LhYgpoKCzTVmJ+zhxVFTlt5J4dT8flCEpyr8TdKXx6O5YFEtIMq/AGwrLIzBn9Hbq0hJ2Z1KixAZuFCP/wBXo5MU3SYQlyNceXoGVnVHRlSCyeYLOBi1bV9VmA8gtWYVFCkBGcUTtpU9Njh5GbMvOQknWsSTQlGgX9NLzqvyisiCnbOoj7ae6MrFDc0LhJch5SKrwihaKihDuyfWOiy1VTbJnV9n0cSRW82ZQhK1dp6GivmqFiqen27fKHnQYUBEbBGDKFKtaQEqOTJXTvuqoC62LTEQDC1EAFZ+nWaUnCKKSxwLPfHBZ2LyqUav0x01IK5BV5pPfKZGNN/ifyK59sigBCqkslHXZBAjj1fW3boBF80oEgyzH8fhkN9AqTYayGfJx4UuuRbqci2QfATrjcv6eqeiRVSvG2WaW/47flbhoOJdIFM40eUHDwecAlYA05FA+RGqtdwKCk8kA0bzH5UILOKinRldBgDSE+JShNkDJ6puquIUZCfT0SyIgNVCxxliahbUU6gjiQeTLrlWLlWosyPRl07gUICujEvtz2KOi8ydVRrTyXEBJfRfu94O7zNm7WT0npWC+qVZvoVoWnZmWh5H9V+2/j8dRW24WaoymHKoVeDyqk/NQnTM0SN0chTqXRYrsOqLvN316pwkliEIY1HhOnnkksUIWjetPEPdKcoHLJQqwxD8z9nV21EBwHVhUv8AXlYMgS0Un6Ik9ceaGKTmwaiL6sKcMaf8dyuPoirSRpJ1YSPUKwRFZRYeUi69uEVmULYpw5Ktz3kF6eky1O+J8rKlXHoABUF/4hlUcsF5AJD7YDwViDaQXSIbHcHvQ/7eSMJimdut+PqlaBnlSaAfJIoy82eTMRS05SZyxf8AwziTkoGcsispYp/G1V5qCX4JADmqzWglV1arTqyKDQ7WgboAyUuCxZVJeWl9AeR6M3s6OiOe6NvoeTH+ZSnW/VU+xdYjQFIv2iqjlmrSXarWj8FrF3p2hBMFf0ICmrCy+XC0ASWC0Eg2jJ6pZTweYsZ6qfFaBHLTcxRdhWP8nHmnKDxBDUZmCL21ToxTaITGZLwm7CjNQsJDc0YTkioaf9aR+G0dMb6EfBmemX+hbVbP/Uq0+qIk21IKyJsXmKC49PwpNwedhZVKCKPSKsR5h+GqAZ/rBCrKWNKcPOjSv5Bo1hImQWuxDpECIEGh/aVNStJTKzvVVUUZaO8446cJC6qaTHXS7ZwEYDnb71sZOvkJgrV6bNMoTV8ZqI5NAkyXq1adLM00PuZkUp/8KuZIs28BJIQlQaEWo6AuzAD1X+e27aLEeaoqI382RVssFIhKaplBWxZTcs8ZPMdE7KBSh3VKN5yQo5DZFeAURw2xPqykfSMBBHdr4htdMuCYuR6wm+/Skroy2FieJBVR20bTIIB5fRLzDV4U9Efg/sam6FZv5VLAMgks2mKlX9SSzYzNRrUCgVoGkCW5BCazAZiaIkds1DNVKHIJ4dqTmXq0w4YNwFCvNwtiCdb90aRBQrRR0KT6xFn3Oe6IqcUU23GKupoCzd3X42labawqbUWwJMj6TmD5PGgUeLkq2181ekwwDqC3TDf5AmMjCdexSZEmYq0WQI9FJcTLkiSEKtH+ArCQZp9Dq50rpy/3VtAr1w1XQjpQPFxMqFJCKuQEm7VYfyDJyCbyyD4sVUzTIcBXWbH1MxYqWLOHLJkFVr5q1VeszjhPwSiLAnqqAyAPIUM2UFmfX5ASIdBkUB6bHtzvxRVIYHqPt9+SEKnuQRAq+jUNKK/8gSAGdueKKfOa75cH7ny4HdGQy+dJqr0VZuoVgqMjLoTLNygIV3YKjfNdHQv7fzChBBL8qqqj2efI9YM3Z7VlKmjqqtKY8Cvq6qvpKehc7TByfmsY2QecmubTkiaRaB2C/FEYCgKECdzV9MMhZo/bLvRe1nbzVZsky7TCSWW6E0dylE/n0jCzVKkwJKf6N1aUbOoBUCggnDxexZm8L8AMWozKTGhmQQ4d0V1CPSduXoQy90A8ukp9nInj8oVmrHrZvZ2Q1RXFAUf+BBGhXB3hv5FHRSvmtEWaIfsWZXYJRShR2TRKsVmxYFQvR8/TbWqtlKKi7nj7cVnMmdHmHnRWvGio4ZgKEKpedl381XfLugizUqwCP9Qs6/y4FWLmlSXJ6ZaHTN4ubDZXX+KJKqUU+isRUBVmQ+N20Xm3s7NRUeg6EWJTQE17Xr8xj7x28gtwCSwUH67qxBtCKsGRpsqq1CXHLUqLzKNVb7oC8meIoHxX5BLlVJllashJ8bmW6umiJsVVkkzpv1M+maJ+ob/T5cNJQxMys1rI6QU8f6zasEdhw3MgE0oHSmdB+wLt5csugmHVIpJfNTU48SwLrKSCfcmT7JaXM2T6BWZ9Rb/Y7JmEiZHMJfbDwEtK/CCmnFLeUZuI1/0C2LymECK2R/X+bZSeqr+aa+KrYMyBGK6arOr+YRXLyJClCitoFqfsST4f+v4TMXcTVSyETR1Pz58oV564+JxEmRJafbyX+K/7gllVmpQDKO7Cj0qVSdfrOQtFH7TxCMrSBboGcpi49Sq4x947eQSeEQuQSbAem1X1ZI6pblEXkUTrTQ/pShZpJFCWrpx0yiyCXQ96N32OSfqPaFFVXRloZsq/dRMuj1FhX3k/pJnAUpWb0IIT41pmltUdYM00YBAtAqj6+qsSdUBoAR8oEMps7+YCpLstZVFj3NXatdHhhsBZiR+BRjwxCnlgp5/1+JWcUQMJYh8r1FtPBYIOlFICuiIE9r69KQtpvoxDslQVKAOtigYoo2h27IpRlG2UySTyRJmxeETzvKtVlUvFqCRoOlCqzkTnsdbJuqKcnqoctFnsPKaLTXjactEK6HiLOwIGUpJI/wDZRdWcqnJp8oycvVFYFqJlZVCtC8WFkf8AV6CRNMeKD+DfgsiAZFJA9dr3P0VDE8s78fMpK1FDFwhQKV5ndfYt6KNK4DH6lSqxD/dDt/UtVx5g1YSZmcAIO5MWZOUyURz6MiVlzdpXbcqgyJn48koHdGq4Wk6ZMXZSCiBmXZ0CofYEICyRHaLRv+lu3xnoqimTpZlni39Wkqic1av+zlGKigvDMUuOVeqLkbzApeaRTv0e1aTZYl0RSRNpuqhmWSF2A2pRFIMj+alMejTZfQVqpdhKSVJZaqq0Zw/zJZgNka2FqG/69jpB6QFqzqXDDuTWDSnJ3nzkTp0ANk8KwYExs02dNDZuaU5SbHTZFWOQg86MzgTeTKjk4/LjffevZm/t99/grhioQuDh3Y8rRWdOjHHYfr9fMzNi5FEU0cud08nM6fzA+5vADzLboXyV45AcUYB514nMuzzZmJnoXYcpRYn1LIwM3So5dMgUjjyWKpyHVaBmKtp0xevFVIZ3acnZbKN7EmSzI1RgOkZELJTYYq8FoOuDkMyliT2D6UZqeelASC4VGMkNLrdI8IvGrhyEkgktKDhZUmUVCxFKFG6LIy9ttDke/OpoMUMWrFgKQ82CqRIVHErBFR3UhG/qDV3Ahw2mrNivTD0vs7OjBpORUKSMdwBSkDFmUu5UUESZKkqUZGYsE/GM9Tv6mQP68Wm86Blm4RzZHCpT6CRWa0pVgz44IReQ9Y4s+QUy/Kl6esqo6xY11FAJ9SnD4TpurCVCyigJyGY84qk1ZFm5QMr/ABLwyhA6lrFLBZK6xvac6MVpZwrTcvtqzG1p5mU2RTVegqRyKTnuj1eZEHmtFcNSTACoGgPlx5N03z1/hfzrFFUSrQIhBkTjTczeoY9SZwGcs/IYbJkUEJsQKDdQxkkSB024hErUtwspLhGE581aRRAEszGjvNvFJO5Cq7oytRO2HI3NNlW1VBHEOBwATMK9qhS08kbeXAahqzFG+hdZje33pdU0k1fsiylMmRS6dPTsoxHG+pKoZlomkUiVykwfN2SMjHnC1XDWqmPWyEAUCTdgrUK/Vb2mjMERzlsq1Der75WzEwM9nr16JEsH6nJURSarOjcqiSSLPMBmx7Kg7kodvOhHRQhXIdqzCzfkhSVJR5DVa7EuC4xjLFT9nlpHlmchtlRMSiWUsmphWLrMlECyC0pQJN2CoSfGzOqp6ANUBbEl50BmOXgjSAA/CVvUtjs9SWlOnChlIaRBV1keEAm3ZRURCqrIsW7GsyHBjztyr8JYwZHu2ehN/RvlPPIsaKiMvNI73oaUBGmWUOArgs1Qhun92Hat0on1MI12pUjUjNFdZtQUVa+fez2CGmoUBgyOk3Q+g09A7oigQZ2DFzFm8uQX3zwzkM2jBxyoKUTt1l6bxbCZYWmpctlF2vrhG24Pn0AZyHy0whU1Kv8AekllzILEHJMmIThb12ohpKWqSWizeZlw/mGOTOhLEh5qFekOpt5E1ZEajecPA+nSp5EKHaaDHi82BKUqJ+QrUOaq6ZeQAxNnmA9EJXfZ/HJmJj5E/Lh5sylSk29f5ToSIz9K6cO9HT46q5Waa22lqKIzcl2aql1XI0x2zJ9yRtQ8po040oS4KzyOR+UQ4MeduVfhLFH+VLzf1N0a0m6Uke2E7egBms5vit7UfqYGj03RBdh2qBgDs6vemmtjmffIaio8eEXkuwSSeSz4m0+LUDSoqt9HO215jh6Nd+a0THV0DULq2IrlWOQq7Xvzb0QTB1MThP8A+P45i1Q7UqSSoHjzyEo79J3Qr8wqjBeT7JJQlHmOVZYTEYcMZ2l7hJZ07WnqamWiGmGWKpJqq0WWiF7orAp2emnIf5sGHnrPuJL0RJJkynTpunsrhlmiVZkeMielXp5KVRYgJ6HexyA80RgFLtwaLITl0DGgVUAJRqOqFlVnn/qwr/xSqBWkCrF6LNBJcSm2IYL5jmUqJ2bVn9gjIKuGQWM1XrbAcgZq8RMiS12Adc6wMxDu73+Q81klGjs6michulbjgsgcUnClqGvovGKbSk6hwIUQ1nWSyn5U+GBRkcCbMrDXHi7JYqfP42SHlWnFnxugcdeWUPV5xWa0C+iSjBk01StIGEAURostFrVKbqasbkNBmY8ltFDwHRXVwQ9Q4DVAlRnadD6rEsKMDTxU0Po6YEJLdrklei0WtqDa25sHDNVQ5QHlD6+8KS0XEkNACpJdtj8IqQoBDnViQqKysUl/SM2uzBpMWmzE/Yl0mpI2NsYSdhRFazvKT3M7CKRlIDqAVFM5sqlTTz6VaqsSjsra893/AKXkX3ejTK+fajI9GJDuqfI9JEdjyYfarFYh/ugZzicXHa9lpKEaBUczPMw0xFKza2Q+3ZTXuLF05XYNDxy7DUAGH+5/OioAWjhHZhTHPqoSJSgnOmO4alfFyoRleKGop3zSY/3fG6q2RR6P5Ms1Q+bNbUGaSxQFVT0R0IZv2003eyu+HkdWPQ9G89TlX0BVPRilURfOQdfp5+jSXbf/ACQKOqWMDhq2r6qTGGLGdpe4QvHhZKJ0Jj0o92NIhdsOvQURHaKIn+wrc9qKN/rupW9GUMi2aa3rUpkFV4IAagxmfa2aaieuVFQw068/OZk4gZIJxrBalZcN/wAeTHsqZAibUmAVFuWnETNOG2aD8TSob1THf3dUWaokohgrogY+lF5npQCA7KW8jzdirKaIBkVB+oRMzbQfCcpYKVRbxJqxd6yUOfv2rFmaYJkBsALYoI/7N1+KlknDLXZ52tErVz/fdYuPnycO3d1Xp3BZGSU6FBH79HHmbOrVqk3ZqCwZaVfyl0PJF4YURn7T4mhfkCYALutqDSsWnWhmFf8AYVCiSpNGpQRDN3aB8UU/Kjyj26bXpYIcuZ91P39OXyqsqlriNLv5qHMFKhmiru8LoWJspt6D5tUq7FmsHsNDck6dKArwjY1igAx51ESFIdtvcz7orHsaLB+jq/Opgq1ZzwivOQ9mFg0wILQhYIhVg7dmPwJlQztzb5P408irGVMb0m87XqsVQIrOdGCIuOTmMNovRbYcnp/nQIQC4HMoP1gZEM+nyVSchNoP7xQM5jLisO6zIFF2o4sKTQotnx7IGUL5p/uU+k+Grjud+dli7O/lujpNsWcVR+9r1PpQ02Uzobp58/ks3o1C2sle6q0rEMpISSzC1i9XRPMgMaGStuRCVXpttbuT1aTcK0aLWdprR5ZEKM3SoQ0UeQTuk/6OzPZGcei/jIBkVOOHPz9kfp9isiSp4d2dpA+lCrFFHqqK65LUEl2GV4hmZvf64KzUrRp+VGBE4srAgqh+XlN3MHdUaZUszM6MZo/20lVuwQXRK0lEfWx5i0eKcULAT9qs09zIuoC4YoCvrw7PlyJzk7KXWaskWpTyE6G2mEmBoWkq4/8APtXQ8S+TX8QhALgcyoMZFInB0bxlNYPIlZhRtmNwrO8yCDFeStHCOqBAxmVLsiNtE+OuZVo4ulUvJnWsxUGX/GI8zUIuQHnSh1Pc5lVd2odeYH4YozB5FWSaOaTFRNrtRpAMsz6MQ5KzYds+uIrRvlUViFZAI1J0pSU+aohJgEcKvpMcl1dUC/rTFgn83WTKzslJYorluXlmlyadmotETqpOFNKUUNkmjB5zZmLCCKpIOS6K39Vsrrs7TZjlWq++h9y1WQellR3Kt/6ULj/ZzIqF81KGx4GivecWnaioAeoWoFeswFUAPEJTJUuzi038fSagWFHgwKnC0ryd0W92AFdirSp+wrMW2Ub+cq0V6Voku0pMqcifYDIgGRRjYOWcUiaY83yQoyIGnNjw+OoD3LiZoHVxlWUfMjpngE2Zs9GKO+jy+sfR7UHk33AmKtRRVZoJjaqrooZlYdjfHolGK8AkLNTlKl1UvCc1KtOifuCo7lkU/wAx7VOgWC0coO7XLDy6N7pOPboD6pZVpjAbpRZKIVLFlQR4UT0GIn6ttd/KwgTAD2clSfqgyMXL4ROsYq52Hlyom2VR0C0VWSVsh5uWWKUVCsztlmrzUATbZWKtvlledRBbv5gT9+3m7D/QNIupVgzf15qlP91/GTJoDR1RxwQrlxQsC6mjiQNa2pAhw6TLlQ9SUC9MiQOmiFc9UTHZGGyCV1xRjOjR0XLpULSgcvQe82my/jIBkW0z2SaEseaZY5fwmR5xRZdFKFQzvKZ9aF9u1AiD0IRlVgtsgOuvsQctMKKRZHD+XkGYCSuX8/NuTKxozCkez45Hkr+s0pxBZhObCrcTQD0DKzWgHiyvPTMVZUPbNMUptnKdf1H4ao0uUoVNsYU2wFe2RiU9KgMyXQAVXgA+k6VyT8KPxYYcvNAav8m5Pz9k8mFCLVeZMwLSFAeyxJD7ZWoXxR6tKlS0+kJC+ckmGLUaKAomtQdSZQGL0YFm1pF1G3o6/wBDZf2aTP35P5s6cowk5ZlLi7UjParlSJ8x61UKkeCqEmJelakMevjpbx6aurIywn2gDTZugpE2CkqBUCapLioSgQg1FPrnFC06jy4+6oEguEHPorOh1Iic0L07LmdPVsWatp69V4aTqEizBNovn84xEFmP+Os7PZTO71pFl1IeVQCPW7MBNH+wSThZg/f8o6ZzyZaracqQFxMikzjSaONNO9pabOniSMcNqWl6VdY5d/ZDajCsxlMHRVs747NOwkxY0VkdjGXobVK2TtQoTcqhGXnPSQ9EPkpk9qeU06qrLapJahOzKzBlVVqtFTWoUBNezX6lC2hRkl9FuylAV5yKpISuyKJ1Zp2qAVWhd/Y+hGuDte1k1mMd+s+t47AowEaVSdWdmCrN6KyFOn6pPUk19hSvoUSDTl1JZTZp+9TxEMb4xQ0WtOlV1WjJTxYT1L1+wqEbkBq6/C1QY44Kr/XvU/MrWnO1mJMzMKJ5ntH+H2rP9fj5KiEK7eZ7W72ZkVgACHDt/le/vR3P3tQbl5xntuwNJtOU/T16me2gOtZClaOkDxsVoV6ozhlefswdhQ6xfsC05ZBHRJpkFP7uoZC5WZIx6KqlCihKBgDplZuRWtmlHbnjTrR3iyItJk1IC3pG1J0Z29OMhaMD7fsEQOvzXWKMHFkow+LAoJldVp5KqqQVsKmgE5nQYU6VdrvahhdVcfRhNnFnC0ny5YrZFZNlJglqoiTMGpssetAHxpznMjupD4ho+qBiXDK/nyELFRIdkXYtyQm06AlESASOpprB9hpozhAfEwXRuk9oJ+SrRaM68P0yh1Lkpss34K9KVBVoTp06PV9KKsvTR+qkoyLMo4Kzf+o7caXe1m4adaNV+pzV+FWn7IBo3lQrZKUD2dlWjMyEzspdueZtXMaUlSbUBo3CtoVqdMA3KEsysEZJvrzTSY3JfRKiyMRMieQUaouu2Mmr04HPJdDRPHUqJ50FAVdEDlpl3Xp2YJpzzYEZGOtQUDhBNlYkUXlFKkbFR/PzDF6/9dXIEU6xshHe6qe4CCVGpRLTYrpj0GagDliJHf00n/2/xillVaSaU4cI7mp8pUQ6IUuzBTpWSyhwVpJyAVZRtgsXClIhiCVhkKa+lSggDNX9azdi6LcsrUZ96WjBR8RZQfQ/lDqJlaN/paTBWuFX9eoUlJssvu6qtAzUb1H1sh59D0blCsSlVXTI+KoWlUslJl2NN3Z5lfR6FqEhXB6AUpufXW2kMlenmWYUZzMEmhYO2g1TyryK2Ukv4oH9WCjEgnKpQqi1gBzDgAulUaqMrkUVCuIoTU+VqzULBWdiyB1MTsWdT17dT15ElgwUDQCAjbAI4YKDRGQlSe2AOuaNPQEXejJOrGqr9A87o7KCARyxLuE81eiyWr1/oD+EcQ1LpIfsOokaSKrCJLbYpyzLyJ8qGII7Lr/3aHCVQd4b+RS1ezs5mArq5CaoEWQIaaixWQ5TrXkyhF9GCrROj0arl9jauJOlTQ8DVKqgTqTjp1qF5nzMXYVYN2vXzTLygzNTEtdoycZDRyVn+wtvnyZFyKdCfqCHJmJVrCnK88vOBFfCYo62NIsv2FFZf1g4SaFyBUypRePNzP0SYwE59GFpGwhL9TCE2XzEwwkWq4GQXFJSdVCb22hJuei7b9jJ1+Br5nMgqhwwkvm9WZwQdzo7O5YBmZ6GbOrsp+Z17IX/ACypx4i5QOJyu1HWk3ZVR2FScmNBQFwQzS/nTqVJIDMP/Ol6MGLlVaaAGfqCzNPnSUBWyerM8lGMZs7L5UdcgepVRYmJhWV8uOtlgm9PmZIhJcZQ1PYhp+nL1DcAOszDi7RY4839/Jps1OpKSKBWKmgnT1DwZU1KqqiOibHVRV5q0XV6ztUQi5/Xf8o1GkBy1X6x3dv6NIDychas4ZahGmVmxLMRjFO8qe1W2biKTaZI9I1t2KB8RQGAdMdQwZqFCzUcnljSvoBoM4kwJByoUS9KSTJ3MseoSzdiE0CFX9w6f0UTFG9KDorVAu+SPxDKzIQomoCutUT0MuDQsy0QhW8kHJfyBJDr95qGDc7o1gpV6yaZWiUeqPUGhs9OB2jUKMZIgV1Ni5CHksj2IM5qWk4HF5qqIH4+BE3ZZl7+ckLO4ZmBFQt1+ygTQeyRkjFuFaBMsdFIahZvN1xXLmWO1GNC9Emy+q1XtmLaGr4o6CBPQCInkEBY9TW1KTmAm7MfU1VJgaXxdzyfhiLzJA5I46/qpAdyqg8IrK7fshB04/rbuC/0QddYKycEMtKtNQelH82ASlKymENQ1J/7yPN5zM0dZIVo34JYYch4Jq2b16dBV0ahJcdOESgSkplahFkk2C+8+Jr/AGRXVu9qmJUeKpTygtKDIq2wsa0Falo6/qGboTYPRgJICruGOsZUTzYoBRYqjJOjK3DhaQkaQJVHdXeey1dy+x79eZoozRsi1FgmPeMqIilDKNViyDe+Np9qBNGgscOix+W/BAhALgcymM/bTrX+ipR1aLMgrNJdztjUbpXo2wJ0UtTlce+pt0dLGiS7yq60FlIkrT4eYdo3rQ8D7EutF9MXp6y/31wyOYkmSZOPWq5CNO3LRNZVQAASR2ONwR5z9QhYRlQQLW0LJrNJoAqO/C+VXrVDI9BC2SarPlgQ7CNH9ndmZkQFOhN2nLdvVTXqlfN3aReQM51x9S4CV5PavGS0K6d1/mpKFSFO+3lLjEHjITUaHKzr3apXhmbQAMuG6ARkx1La1Smiv56E23ZAizQjJ3jzBIWxmsnadAG/mVWc3X0oQTVeZ003Mh9GqYtSUObJsBNJNqS+6ii0Qsw6uwVaYYaiP040FcSMYY906BgeQVA/YX0FAEaqznVZRXiceiIDmQiDPfnUczk7SFFEX33+BdfIM2wjF1llcVFHL8yBcsH58mZkRgXWgsnLIn6o3mP9SqAbDsZj0cb/AKlyYuAxo9CA/U1m/IK9TbYoLBjrQJMrLiKoqqSoXVXarNUMzMPNRxVUJ1bvqjbCzHabtrRSI3IDV1+FOmEhoMc0dukh3tJ05t50QzT0Yv4hA71VS40HpyfQfhGF/wDxzI96tOTDVbkTXhS9GWkhNWqE+dUUNQupydQnyWHCcmk6rJbHTuKWo6eVQJhEowoT5llZVV2RGm1Nh9fBsROLVN4YmROGKzh7PZ6OGVg8plkaj6Q1ZEDKlBn6Pzv8MTyxPo7rQByBmkV9v6sPNHTHWeZ30q5MynfoqqGTpAxfXTUVm3SjdKqawUqQWSizGOcZbW5ARk3/AEYkGKhgjTolJuCxVVfZ5pQsBdmfzUB+ekIblprss+zyA8UaRdWEJhRtfQlkaFVqtH6pZJyIKc1H6vsVUMiFJkGTcMrFGaS+oXtNivA03MspHaf7ou7F2+h6lJIPYKp6abKT/QGg5CJMhWWSV0wYmm9qBUsS4CKTB0M0NRzSb9M9V6KysyoCldrZ3VFG9HVBW0mDXaavSTT/AJw/kJwdeYsoQLNQvVGXoTPrBuH38YUVYVrQ6BY1dZfyfzWc600WFD5qyr5D4h21l5Tr7VB3hv5FMO4d92aht+Omc1wBkyKiTo2TLmdLgsp8VLCbrpKqwLdpLbk+cGd5bUMtiP2ZSFlLWlWatQPM6vvz/Zb+j8IFM1nKNLaFNdjp57RpMiP3GnllWVnu7ejfwlpqwDlEIVSFnPtQi1YRbrpFKZ8GnWNpNZH3UE5AV69M7NJm9QmlE16Adnm1m9AE2T7OK2KdN3ez8ZosZuYgf0VrJQdK30NQspqjMvqxQIZusyKTEwjUcHfnTKgmnDBZvCgoypQP3VnRqIXW3W3HqkchP2VSihTX5CKqS9paJVB6OjCjZAc1zEojgO3YWYSYNRNWUk6xiV6G03ZulsjtueXtwF5RpJ5KsLBXTmnop18Maj+3/IFG4RDgjNMRtEwrPh16r0Qiu3h50x1yKOPI8NRDEuWWiAB6OEehx5haHbSsyHzDJjUnNby8/OpWbomIGZSbdqtI1Qf97SdVbTvsNtrr2FWDRaTlyoLZPBopIZQhfqHHKFiCdAKDjMGfUiwb8HFpWDtREj6GjgszNzZRQnxK/wDXoOrgyQBWkMj6IR9sSGLKpzOVTxl6pjKruKKlxOaGqEIhBA0vET2piVZiZyApTaYzBz0fzpJbr0FF5q+rB+xFC1fSDUYIDGjOGM31Yl1BdNBd1vRnW1VyXmSzjHMREL3QL1NVNf5psy62YkuqzDHvgoc2IpJawMarKbtpaI/MComXmq0LyYsgGpKl9T8246MkAyKqFxENRy6CJy7Kqz/WagJRpvVz/NBxM5LgBHkH042si4a/8zo6BP10LuLr0T125K48OLInkZlAqfTza1Um57OXsNsEcRkhKMto0cTeLkuISCf7dbEx/Q/V+yAUg4Gz3sdPzqWHfkrY902HDZFD0ilSp4K+jEiugwVfYtrsfiwgTAY55azLSqrj7p28wsp4VarylMqaGe0cJ7yd9JiBKBC7AkODRxO3r1qLTUPmvgEGkjNykWnObiXALiS6U0ZQ2zOrez8JEFmVVA2jFhstbtJeflXy/Yx5z0Bz9Ci0B0Fc7PTrT48z+GgH9L9Cx4SW4ArKc6b+1Znt/R2DciM0Mu0n2Ao/CdmdyztpKU597zWETMXdtK1XMpU66ZzYTKSx6FyV8PIaNbeQuzu6at5KlJtpZMR0uSNILGbTri1RwKhAr181NU9GDHqU4ofki1KNWSB9MNr/AGUohxXyasIoVQTRfWFpk6AYidHUJNqlVYOKQHs9F1z1+wEU1SwSCpkSmgKg+cyzoOmmopoFhJf1xzBCS9AQDE88LaO6xBImH1+UwOAY3M2uG/hZTNSbrhgHRn6yo6AEqqyM7+iBeXWWln0zUWjtPdA2QSyUE3nWiqI+gBkiohVKurdtSmvZGmxZwSz+Ff15cpI1Df7zCA6Yv0U+QRMqA06I4pN20iI2OgTYm7SckxM66CziqO6mAZNmbaQ29JqB3PmLfLmgSC4VCMvOekh6IJLruSTDetRNwN3M4rI/UO3qtuyQ4kFvQN5F3XS7AJXHrFx6oVpqk47MZ/0JFnqRN+mZKPo7bUYrNfhh+OLKzqxZKSBabxTxE62HsB5lVmxDF2RzOd3Cb1Ce+i1vFkZavX1mz0mJ0ZWtNbAGK9cH9iYB0WNAlomj0+iKtCMvOekh6J/c05/CVGumd6FlyCizxXxz5qOiyUINJhNkCZm7uriYZT2W+gGxlOdapjsvISzKiJLHKDkNEHnVnoUeiqdLTzbTB/zHhRWlX7IpSlpr5uA/pGmTFJqPRDkrBzVZ0Xy0Z46/+/xItkKHDBZrGVSUKo8xy6l1gFCF+N0awC+dp8fBFjpmNwQ1dfhMRzFtXoqKs0adNOa1CKbVmzogTgySCskzoq7Ko/6B94r7gHlxgbB0PmTpqWX+QKS0wmgQgTHmKLMpNZ+s2yokaWjM3RD1o7PeStNVAWa0nrHKmyEmXC2KhfQNKYcpqjVHqNvrBaTbqquQJlJDTzZ2dWiXAftca0qD0VkajzpXp25GoURBySzix2AryUwEQEpQUIHTK3ZGt0JpagBQTRzuk2FqTM3VFCkv0sN8TpT2EGo5PQAZWgC0r1VSXhitOUk5Cp0rNQ0HP42iQSxb08jpatS9GQSZ5qFpQK3Zn9mCqTMJkGn2+dflS+oeKF19LzdgHQnwaXKz8wAjPE9F8YzZCq07ateRywWIOSkkCZUaUmKCgjZxbIkonkEUlZjVmaVm/wAXmYnilGWgVWrQ9NFXnxCFqcvShDEqQLFiMhifCQrWje8zvyNKXZNiWmWSpM3o/k60DrOc2UNLoURiimZ8laNhajBQzngKpWnpf1PmyiMCfNxSiRhWBgS0mDNRVVqsqkspJ9P6EmN472QPy/uac/hS4ikCxOh46JJnQiel3FnC9RNiWZpbLK0lK+jkgtNgysHUGSgfLpWkopi7yKfeVZiqDmSsRMs4WjztbpWm8WClMgGr4/U0AYjNQ08gSVM7u0ORObAOqrH5H3alkkkvoK0XZLCfwjzkQpFLt6evgAzKrVceayDq7q1iEDT8yHHcyfhD+IhwY87cq/CBCQXxE6dFF0q1UOy6rUhKltLNZCpCMxl7qPIKJmhLOo15KtF3OlJvt1KihefS+npWK8rzkTd5OtUdzFaLSqlPoqx/m34xGG39XioLe2gq+3qWd3dIL/PuI5+s/quo1/2uw/CLfyuYrVXI4KeAcXgzByJ7ZTJWO1JRuyBLQIB/FB3hv5FMhwRmhZsRZvJsSnnJ5v8AFVSjnzP8EyAW7NJk/IAZg5VETlujFzKk2UR5abTVEs6gdKOjzM9s6qdEshBJLV+FUfjJzVa2E5saMyUQV87JJgK6pIkM/CxXgAzDBxXstoEKqnEmJBLyMqGnpGS0W0ygKl1VEi9KNHYnsIa0b/qG9lBgNi+jN6pJmlW8xIvjo1i4mS4SMomiXagXVKVfqk61CwZwB8si6M2GQzVPAQuGMAOxdz04RQ+woZXV/krU9R8koAeKYzZjVliRUoVQ9BuQ7fIpJAQsBwznmYKr+1P0n5/PnzVprIRRlQ09t2WpZkC6BkzBKuVfp0gtXVgXJLcABEgTKQhIIcONvdMsgZ6lnHSMUBgjLOvZJ9D3QErwrPFXfZdJagQCPyqdleSoHBebJFGUsQJswo3JRgHAEURZLwEVKEp/j8zsuWU88mlA4EwXKdBCzA04KHzoz9qCVLqC3J1rKqD2RaC86BY0TFmuPa+wP6IarpuD2xIcO8mmFPR/EIgSwPIrQAP3W1l6FcB/GfVYs3ly63BLEH0RebcqJc0NGrZfMmIRS5KfjeE/r1PC2kgH3WnNCh5NbOXT6HpVnajIHWgCXb7FUUZvR4PtfP8AY/i2qefdizs7985M50CJNVaoVp+rTj6DvloA5CNKTUMtbcsJEBxR2QETtisvZCojEzf7K3mNT9S2/otIA4iB09VW26s9gaFkVLJSMpndFnT/AHV2uELzd1cB3PZof7bHBBQMguVDLECwm7/rSX0KFUZgyBmUKxVGZFNFTUx8/kx2GQmbpM1LeK2pT2pKU/VG8zZijB1/XQcsZ0I1FVfsou1HSXnOtifUNRUAFCSDTpTEIzDass2UgINVbdQM1cUQhFnsOuSsvhzpj+nnQVq1nlGdZkB5MoNhZAAyuqFSgVis2mWrP4Lefyz/ANHBq/qbBexuFnFEdh6qUt5hSGr2xaQqAxK/HoUVZcwMseSPeZ8t7HquwtC6MdQZnKDlXUs3xVj2RJkPLU1pjaoFpR1pyvsg8wPa/wAsitNT0WWhL0trz5PdQd4b+RWRJJcpSAbZjcB2r6uKKtWpViytRuQ7E2FHk6jle/NiG0AHSZEZ5sZlBRqOGYtd51CqJIpKJVNf068gV/sNn/0kVpp6QNJovHD1elukfcikm5bt0eRmmgQJIuwNjbKUsl3BQqiPFTKoUjIstuu0mqgMcoHpV433ydjn52V0+nVtd/WiHJCM0Qf2AVDeSoQaqgU8lV9SSQR0gkZMHEPiik8keLBFdDVpxdjMsoTijctQIpEzQKDvy2u3+Tvf5grGSGK87DitKZO3YLWr0VVoA5O2UkMoLChLLx5fKxi01a6m8hV10dKLBxPfaOFCtNVfpZkvSijDq4TYBRezPqkIv5AHW/Oq1J0KoItN6UZgz174ZlUorA6cJpSzQ/12rw+fgEGau6UJH+6K8VGTxSjqsUJrNWf3UsyiJo5SczsgemvyLyJo6Vozk9lSq7QcsGDQRdBlZRRmo3n2wmA44P4wqrFGkxZbK5XlwVdZebvadUDOrCqATT+wRSOi4P1UWH8m9dr+CoREmQlwnTwqqwCKBwHU2koqlWSkporOtFRUDadZ/c+Qb+IvpG0dVJDyGP0mJAngWAnR6ZIanUpu6NyMmYmzUCpPI9kr9UXra5eHrVnTL+zGgfJB3j1yfqbh5BWo6/ZAJlqGdqdTku+5dk0yeFXGAVQ5LDpGkxTaICzGtUB9FSZVKBulZi2kI54R3aXmwl0VRLByhPDLLTCnVQI9JJKTaoI86KWchieETio2A/TTbe8lErVOthFFwxUtsl6M6rAgMAFKr0rs/fJ4d9HSr5KsKzq/oyivTvPUrzeYA3yoK1BDo7kMV0DxDf8ASqFCwZFIS4YZIx1agOIJrMVC2d9aUKHAWkS7un3GuWtEgRE7EUPGXqihucldJxdBcxVRUTWNeg9bUebJXy1/uGBnMUcl0DDqetOqvMpJC6nzmpWpyHIVkOOjttavErXlUos6gQ9l8Psq7TqgTI8wlHYZRr1I+QVFjR6CgeYZiO3oyj4pVMd/JmOKkBT2lZWsh9IIYhqKm6AFU0GoJM6qEAHnZQvUuPuiQJlRHG7hgdd6yArQ+abGtEYQU1eeRSkaY7TXRx6LSrTqGQFJyb4DqqgqFQ2PY0BWTiNEBb+jmyp2r+aOzyZTrzq4Lc7SVjOZZGoORtQVn0t1NPNkQt2Z+lOyohcOD7QKhGZFpTc2kpf6gFkPJHZ7G31YFtM8L44LynziR3rwoBuGlOqunyu/lCIEsDyKgAksEUJ0bpqrdnQyoWrbgDsjtZHk+aGR6ZmXjyrywHQ/DM0/0n8tRRFuXDezMGsGBIRk2GapCAIVSc//AMY/MdhUTlGfIBhWiUNW7a6IjzSi15VKCj16YnUKT0B8gg7+TMqy639QPlj5hPSc5srehDOUadJh+GpDIfXgN0qAMJcimo4JwLvy+TekkPKAmc5h8hFgVLCVe5qonPHXI5ZysXyVm22aYTLsiT9WoZujk7ItJwE/tks+QQVUhiiUNHCczND58Li0qzFHD+dCVd0osZoE7/ldmbeOqaCS7aRDHvT+Z5B5L8f1jURLt5qjRetAgYt/h1DIzM8mnOjtR6zUN5doJmMmQBc5Gfki+tE6szXeIVwdHzKks/mlO+V5jPZLNwhmAd9/hYre9olkvJYWdU1TdqbC6x2T4Kq8wiQQuU6QtN59t2M8XKh2zY5fHSNW7Y8rUhaE9uQHWYoJsGpR3M5wUVXoqDeFFZ6hWhOaRbx6EnmeQOTqirKA5Us4VXVFPrSvonApOK+JtXQ0pu4dbUIXpkJZ2CO7UYq8mZg0hsErJTThk/toH8IXoye81jOsWLzYLT/YGaWqjkos5FHAKKOEfydHoyPz2W8VR+K2lRxfqS1Na1vIqwiEEnVNjbIEb7Uk5d1HO0WktElU3b1KnIIm6nsENoSiAjMVY7UKrKVPNbqWToUEsCckFrKr3LClI+RdloKA5jTpNFZGZdUKleirMVKzo4dDQdC81mZtOJqTELWcVUj2ShokUjMTcVpRXexc0ymQSQuUff46rVJ+qmRVhKM5t7CVH0fF6NXyykflxR2ZulRqSY/b8HG0rAOyO6sKz0xHDVQLbCeoo5dp0syKzL5ToHtsfr7Io+5pz+FrOcnTGBUvZNCPwFLqVsky7aR1MFfvpVmUM5B2FFIBZ+QQQujeLxI47nEeytJK0aU2UCe53iUxlKopEm355HFKtRkVUcL0kjRXiUGi1VUA/wCxdmkNeiDONNBNktoYIlJVGWFmw4nO8wzikKJjuHCbVGZk5HSjgPRgxkuhGOKtr5GfXTofM7aruj82R6TdqSeYLKs1n5seSg+01kS+jLfAJjHqK4ekyEUJYssXyWNI15k2OjBnVZltT+4HPaMUdSH6u8ee1t0YqMeE5TVxZliSi+fSbRnbs0RuIJ646L3yeRr/AECzFF6GRWYRUYlWUznKNX0ql66OhykQ71saDyGxWBhDBTKrBrFlcsURawchrbtE9TNx96R5LUXthOjFZmx9Ph8hTG8XauPq3uoCIi+DCjM01cuFCBXBTKRnZ5sapZejtdtvuZuGKgkeayVVClW6o6k7b9hnCsimc35k1h5je9uhKJfzWg5pI/6MtNzB2UPnOjsKyqeUojCSPTpuEQDIppF1S7TWrqqsOQkshrNQqoPQo/1bHyU+5NS/LHR9Pjlhq93qlo2bqTyBWk7UkUV6CFtq3MAGcn4CDa8Ium3qPZ5yZqE8RWNOlM8itk38oQyjyRJfKqXKL9fNvjXUZLxWaKyui2dWx1o3uhXxb37Vuz6GnppI6Ahtk2pKEIBcDmUM8mfThNLUCJ6KB4tzGq0FaTVg3YKsqoiogKrHTzYK+Pz186ERNQ8eQx2qsiJ2aMxsHTbFC9p+jo3m93QO/s8Ok9azlWmZygrF6I0gJTYwbhZqVSQIVhNlVLKzXLmbTUN2R+FM0fGbRizptyKFkq4NJ/z69FYc8VgSVU0XxoiIGbbJYE5e6GebUvk8uvhQu50GIC+jXv5KTQMwHYnOahm8lmhJ083DIdVf/wCPPMjmdGVXRGBNwgkyOVdG2nDKtLlFIcvTbLr9ivnxXlI7xQl+1s4rSkk5SIaZiFxq7CBI0RvcdhqeVPUHSanAUKNs89yadGRaLRrpQSRj0x5gqVZkdRMBd1nTU6fgC4dRGzwuHrKmSXMH7E06Ea9eag/wXaolJswUlnDqz+Rg4Vk7YUNVeylDKs1ZMksytj2JI5UxZQpCBFV5qqtYlSsVlKwMwxLOjGTThGcMoaq/5nMBK6qhjy1JKKj07BBmvxMO7I/J5ZZmRvJ9OmPRf17BZyZlBar9sq1YVM0ogaaFA5UPohX11xQAIgWDakP4HZSsGrMejVHLkDobCTmzxqZq0+XyQaqeS2MpZlnN39G4JIia0CIyfrrkaX0DbRVKC03YH/bRCuCAZFm9z6/DX+1HnkKiziQRzpRP0KI4nSbMGZmd/Wv00TR/QeX5ODyWJX0YBVszLdFFCSfUK2wGkOSyAa4WSm38R+M+8p9mnM9aSYGBL00+U43vNHYlciawRyg/hNbRpNGqCvRNEEYLSXNRysmKW66izLJFmdqMWqs2D7Wi2AB3SC7ZQqFiiyLAPHx7dCoUI9FdWX00GRDMuC7yUpzXIBLUa8nszSOpqhCH0sfNdd7vLouqoqxdFFejSjK56Rm0vkxVQ0aRG2kdC3KP+EJYg9UVwd0b+ZWUogcOsmVWZWp1w1orVaJMxSJSkyk0KeICJ2J8vfR4Y07o46WIeaNlSmqIVWViy0Z2oQVVHZPBROp/XlIMD8fixNMlUZZ0NpIABJ6TR/PprFLICzeYXZ8RVXOxxYhmixr8oisSFULIznrr6MGshiRxRwOh6E0qjondYlwrXFGCCJ9HirFdovIoJ48WolkcKUdavtZiCvRGZkZXLBQKVDNVQWElqogvei6mS1TNJ9twwlqsmD+hVhMOtFLoskUiKqiTc/JRfrui6AUVmZN+jApygisaiSRo/ppfrWjFGZyWUBUpvf5Jdnn0RSmXSoTT2rREFVQMhWe0/Vmk4ifmFZWExqj9HWYZ5u2lUEgQgmjn/wAVyuKK76Tgg+iGNFejAef9WB6naB2sXARDMQC9AlTxZwxooogqxikxJevig9l29IgLVkZ4yYRc8o8sQ/ZzMs1+7M31qLVZyKtdzIOKXioQln0fNcYjgXnR2dNjpiAVS4X0U1gHWZ1RatJ0Pf8AstEVw1ARsRae1LV4HDOG2J9WUiIEsDyK0PURnKxYhgVkifIbzRNyZldy5VbozsqirVcDkKrb2bpjBqY4crCsyaIx5o9KOWP6yDVGVgiBQUl+wisSzBuTr9WcL2kPulGXU4UVwbuqBVJW5JRKMBr605op30xSXogJeiqJw3t1D0lWarKrEoyMwXj0ojzaqPtmC/CTSFmd0rZwSkZrfmfk6yd6PSUHdylaLVmorEhVBYK6/PTwVsGKWmw3yqqUYtQ8ibIOjMlSVf6k6UY+3f8A9IrNiFVgzaq9lcHhEf8AmGlCgZFVeVIaipKTmVVIo5ThQrOfq8chXY0VkfhygmAekZTRm6ZgIFLY9GCyR+SOXQThLEHjTUMrX9U5axlcagJhHKFAgKreqF2Tj5b9fQHzz9H+OZpOHZmaKxaaCfen1TIcHtEM/wD2VVgp+ABCO5r/AOynV3cyVY5MWWAs5VBou5crk3UsriJI2ytQ9DJRhQ6SZ5E6v/OZTti050YOj9Ceqmaeaqkem7Myk0SkaMVHf4deK1MXZcaeYkeuCcootVehqQqla2mwUgRqr1smQiTZVmpDBeFXgIKUGk2MmlJZAfBLWYBnRim1kQzOwf8Al5/0mRGgM5z1Uf5M7X50uQIh/n0TRGN9vgFh9pk1Kab/AFkFR/8ALFdFLn7gBJBU7YZH33QMrs1F9lZMejTZYs6lvW9jrQUFSG559BOExNOT28LW80xpuVk4aLmekLO00abEUSY0V1exDcWTb+dC9QqcfZiU2hoHVEFXmXiZ0fz+0vRRVlkZOqdOy8mh/p/63+KmqErBHcrfijGhLzVDyppsMjUVNNFCpPiQgLz7+87zd3rysy6BCpVnKMsaPMOQ/J3UyZS3od+tf+744amMEmEDX0VFnR1LtP2byQNYeYFzQ8vBkRydSLBP2eip0UAP4AQVc/U9PQiOS0u2aaoD0skYqqInm6kyT/BYsd/Cf1uUAkqts+0Zl2NJqiCTsxVixZzSaOEZmDUb6TXrpsWHbedvY0UOuwqCMFAceLhPOLhRVmdGAdAVX7d6YLEHJQYDcS2PJCygelCl186TyKdoAn6yFdVUhgVejj2mjFy1AoEzo/jf150dVmZgsmSu26Uoqmb/AKlp+gCPkdvNXPPUyv1TXzy8ETnQqKCxP2Z+AT0wlZF5c5RVGVpEUUuGcAfG2BpBVZX4KXugFJ8zmCiMkXQojUXoo7Mk59KGD1/muqMbghq6/CzwgRAiW+hs3qFAyus8VmKmS46rXwZTeK0kQwD+lejStg6BnHFGTTJv5qnIJxOlTTqU5JlSJXH0QZK1XmyCsaqaK6gkKpp9n38M4rQVk0qtqD0S6FFsUrySjIoZnAV2Fa6XvzINvp8JTHxJUocj0VDLKVKGrkfsULssBIJyZVtz8ffS+vXr8FYVgsQclochsizGs+IurdmJuH2rcVp8M1qdjwR1HBQaLdNrmBp5kSonooPxWU0hRhyCzTVj8XPRDKelpLRGj+KBpRJMY+LLBkpOp2p5dFx7swGmpNGVBOagzdqdGHSmhqvaNwVLtDoerNWjidADNu1IWkxzSky505tqg+fxgsQclUUbghm1fXgk9cujqiK/nVHQsNDxZHNbKrT5czmhRqJTsglUOmH4+1MjKbZyrvGuOuMy7KrKMW7RgSSD4tIqiIp7E23H4X8MlS9KvNPizm1KBZspCOA9YjfSmxKoViv8mPWQdjWoGZFNHcqySnkO7HlVi6sq0VUVolJzB77o5iByzL2BeMmQEzSfuFAmWBBPEKXozVl151UInbTJDPs9Ub0ZVSaSJcMJgoZMeZkjoQFm327n80cFJljyEd5/IFDQuA8HLfE6pSoaXmeru+Xpj9QnWQkpS9JgsvhHTWoSEMnFWahE5r6kOlOpuRjOGfpQ7zelKSOMGo0GIgjSyCzOKohPkiojjknI/p8NJHGxel6eiowkTI5hTLDHbnT30lSGYFXdxTko3G1GlIdSp8yPPyDW8TxKk1kV7RxcljBXUsUdeBFqbIIQPTgp/wDA65WnRK+pOakFp90eCizu9mhLxkuygZthXHJVMgUbgzahZt8zUxTplk+TybtRD326M2nqqT2OQGRpNLaEcICKeX1UHeG/kVBpVtTafXqo5ShOaWVXDsd2PpPdkYBeZq6uVqKqzTFtyZZH+IYD8cz7dVDIpkyvNSvJ2xHE2JVHCKihZ746Tz4RRr8KelegIUCZYE1mtS9BsThYsIc5G2UpUKf8kc2CgIRST1kZ1C8syIG2pVQi9wM+tVqhcMilAleTNlUsNyofFKXFpuJ4iKB6WRFfqqgkGRAZgWC8fQMlVBanywH1YOr7Gimj0hqur0DKsmXJ0qDZ5sFDRo4E6a/wSyzPss988AuCYx3t1wAUaY9QD0Kbkel59QfRA/LFJlF0uQP8kE5ik0UOnomUSzGiBKUo7KdIvS1iaBnPAIHDNM78xoTH03Im+lObpXM6q4HoJghShCkUQkijD7Be5FWJ12e5P/Mt1rq4sSdsoagVkhL/AAwEl6eRDs4DMCw9GVOGC8+QJDVLReJAhqsegQilGYUadOS3pvrhhRGBbWpgj5H4VbABUXRZTseWlSvc/wCs1HKMtNIyrdlX5eIBbr4Ff28M2Ztds1Kvum3Rz5q5SgnVXWSojqu3fXo3+Ufldnmsw7+m1YFabKaMUk9AdrNxFbSJWReVAe2RgCfVCmmmiFTMb1ppQMj+qk8mII1QbVTIFVIfTsFd+HLeXmwB3+IcUfQD5CTfnQket19ZIlJppVAmzcVDgUNVsSW18CE5xP6GvAq6hCzIWcWkrfLl6s0y7H/R0Z1FAVmRrSjBFKVTVLNusa+h3aZLCtt+RMwk1UFmM0TvsqOBtqurALVFala0P66vMtdUZl95kmjEOWkayIGkRudg/AUbqZde/ohJISZl4noIjhT2UPbG01VdPNz86+BMM86JtgHCrDHShCyVxNaQZFNnM2DLUKg3QhmICIzz5B/XYCec7hZszhp+jRawSe4zdg4blLAAUUfI5QzbtCKcDmzHtwqUZVVqFV2oCvHHWo5i4o6tZJ5Fue1DGaNNxrgfib1BPmoVayorwC8gvkaoKyqKBjTcxOjAgkFUon/a23CASAepJmCITBpk8wZdTTusc7s9GZlYs5alSr8ogTJMwqLQBVC62OXRVJIp8JdzVEuRjpQpLJoLKrLSNf2VeTF1edGYvSTXqSqZL3htWVdzJ9RwNqrMt+VWShCwZp3YfX5p6qp4Q26MztfglhuU/wBaxSlQJMQHbIAm2iNl6eaMGmvrTbdh5aX12dRCAXA5lBfRrs3NuT7J2Pd0YGSO3jJqBShK5KVVYOqJQ1RVBsUklJ2osjjlIFW6RT0d/wBhmVwNFxNBObIlsl3bIIk7pzItabhlsCyMaInK+ah0hiyGjpODFSJIoMrIx9AQraUzaTUUTGvZSW/gNNR+ptEqtQ0ZxADURLJc+IkW+EVVMeBssVd1YBfc7RhmIhmHG6lhFI387c0GWAnpL0cklVeSutpgUKVoiIjV1WjsWF5NRaMRI2T4QMYT5tKpx/8AZMpGVdpIhpkABSPUuz0W/UyVMnX3Hn81eTSyJkfz9HkHqiLbryRyIhHmRLHm/nJXP3nJWMyvoT+LsPoSgadeMuDI7MroiN6DasHmXOhj2mfSv7LVI89nqjWHj6FBgqTUMIuEmvc5ZapKOVfxT+beUrCCozvOS0D9CwaZo1G+XKVd17oB2obXdR+RA8hS0/QUXbqKvdnX/Vf2AGVpooCv0mhYq3mDmgEmkiEzEjJitFYohqCKMpLeuy4VFPoG53NtjSH5QdpZEdGrPZQtNTwSuMyNBl66H2LN6Fy3SzOumkIQC4HMpzATAW9WZjRKVaZRAhszKynjZS4p0gZmGkmsgiex6EBvNR/ZWUmZXf1ZBQr6VcjoNQHlgpHieVO/r+clhd9mdUNURlXpo+bnyMVqPRwIkusg5KMqrQTU9t+PHmWLIK/KjqDFSFbHq385ELMGTKZ0KejUmlZudFgCJp8Mln+W49ETpURAKUhRwZClygCBXqytRS3X07pMhe1UZaJkEze8v10YSFVRvHmMjS9Ucr5KFeTMQvQIp3Ty+klu2Vht2lVaPQdIyYjo/SAVFBW/EmDfYsGQOAB5nezcOOlIsYqqt5nlh0y6qVA9KVdGJu+yNkr8c/gliD4b8s25JmTjM7XRmTpKmpEppB1AZnWyT/wZXX6SBZ3/AGEjNEbzYhzyWY7XHyiwEWSDlTTEq5Lert7IiyZOxJDSFOHxegPhGNagcxoFqyXgsp/A8WpoFl/wVmkmLnZ2GAp/9j/EubUSspoHP9bOexPIDJSRU5NfYN5zAmyKobuc6OZv2NCoO8nfSqFN8VpMjdajidCAhsVmKqsyVZ6j6oyhVDTpFuRz07Y3mxDpsS5HnVi85swUMlRwyn230ZOXm72E1mfo/wCINMhVQSkar7q0jJT3NAiTolFYL2rqWnYExLsTpGIJCzSgJmj2ahlWRVQ4UUZgi5CgnSUTZYrAeq5F35JXv8RAMiqEUcJecs3bK6fa2wIN4jxVJsta80XkwmqSagSyEoyKGCmiqDc1r+vtUqoB41IsRIoLAo+pl61SDM7rRtl0tMAutQ4oj7XkQJkEiauyj1VaMvsVaylqFyVRw9XJohdV/u/QZidPQFkMjDIZrV9p0dugOECm8uyuxNiSsZbRh5FSQPxCEAuBzKv7oMosIi4T49WSj5xE5LVX6ZqNKQZnqtCaPQbkGCqpVlVT9GY0MgE/G0VkmquMxQSJ0s8STkFf8dDtnakyDPk0PYgtTAb1+PWoU2i9aIy3dgIKSaUpJObrQMwjjA16aWiBNy5/00ZXVaOzSXzJXYTIvtmkWYP/AMdfp9QFs2gPRGvZv/WqTEQJYHkV6TXKY0yivpvWrNyVLmcz0zKiOSWGmcgP6IGVkn5gulltMXyR7pzD2DLtKrCi9I5eaqV4b+bIVlT1QcOPsrbF9WjM5BvKbQKrYAkAsSxWujN6lA4JCljN3X/4/i7haWUTedcY+xSoXg1m6URWVmG0uOm24WbLT0qlEVH2KgHLBMmUDzmgWfcpRrJGqkk9HkZzZmAUUyOFerMuPbff1C9M81gtBFnG6FrBKFFWXo1KVNqNNVR6v6MwpOZmzpQm5YN+LTlFntnYiauyYyLtlR3ZnZFfyozzChZrSipJJooczYAXZQv8GuyhTtqefonb+W2Qufas2syU6PZamR5onDdCHIBGdQlaQ8BkAixdDs9vJXBf9iBUDzWiEGeiCy/rdNXkcG8Yi1VRqCiz7lViPFWSqrfss4BoBSxQ0UINw+SxGsG043TTjqTl2JLhUq0aadORMEHI9dg+aMpdORvvifRLu7cBkUAdTZj0/wAo66AP0NAR/dE3U6A/BJczTm1ujYoylCatRKGqURi7ozMgVqqo9X42Sys67lvRzIuqed1myq0UJQPN0QqRz8zeaKE8q7d9kh3XWnstBNz8FdNuvSUgg6lSZWe9tAfQ3KKek13Sml5x6AzGrackwqeWaTC8dJaTFHDgj4aKB+bhS820roAkFwhTTjy1LD1AQ1QgKzSWnmalKI7AGvX0nsEAhkGvT85FsELkUVGoHk8kYsQfhlcLVVbTTabz02thdfxI/KmUKXPaEqvQShUR7KlVpUIB6CsuVDLUERCg2OhMIM6T7EhRyoDi0S25CdNmbHUSjx7I03iXPfOipZ2YiZE8ghoj3ZG3XBaW7R6idXVmc5ALKGR5D4nOvILsepoilPrYyTmvyVom6zb0Nkk8p0VVCz84tNHWYZHCcZBnQtVulLh1/mOdGD+giCVi+jNEefaRJZ1YTebsAautK+bH0BlROTb5KOJdXYSXkfsRKAkFWpML+vZT109AQ4VCyDW9THz+JMCOGcVc61zs6teistT1KAqD6ck+ArYvp0SbglSCDTSeYn6ei0+vIIAu1pRfOLpIS0wZzQS+FVhRKPuKFugFT9fzSJavxHQ2DHuS8ttvu+zRSQ/1UcNMU0+tLpaBVJPXxgewUlca7EKhDgca5rpyOTucECNtFrImM9sH2OAAksE8URk/IJ3/ABgXRFyAyETolKP1IbKgCpKqgi/c3RkS3SZM3mqzTbEanmswj6qGWQdZ7o/Q8yoZDX+ppjg02ALfJoqlulURfVkR7MrUP9Ah4WoRtkKzBKCk0Y00WJdTMn+g/FcONKtSyMAEVzUiElaJLM4JFJA0UsJ/LpRqfPf4zCRMjmFfbcU9KGvTOyodmYoOIM6mky1U7CsJsjRYSyFR9HpaNFoy8xkEVoekFC3Mmkjqk53LTspJ69FegEweZIqrpBUbU+tKfQ66aFUZyCLIndCvozaWliFabUd25mpBij7DOFOQr66Xeq7AheMe4Zm1Fi0nbJNaulr6afExNlyESYIbzYvra7SoYnnhbR3TFEkldQhAD+IYBnZAFk7hOqglnSaBDrQnOukr/wDB0GSvk3gEPqrTWrbRDp2UA+XRJmjOJoVYgqwA5+ZSVqZUWWPjG3BDu6ktWZnsz2rHyDhWZ9qRD2UupYbU5UevmGbgs8W2RHaKC7tMkhJgOSFAf/Z9+vY52g7o38ylH3Tt5hVU8laChqW/0NphshzJWomOXmRoUQPNiXmUv0JKzqoO+lDxY4gBn1ebtUmc444f0ZaRq3MpEoza/ujGT+hq7SKlLqhbfpkOUecaLMBenJ9xwNnxw2Taqo54Jb4l6ffOrKRkrOk7O7Ldzy6Fm9Vs4V02i8GY5Y/Httb/AGb8zj7x28gsXadWm2aa6LJgCoT1QUPpx2/nzy7J1Nk/ZUegK8gRcz8G8wxGVDSgJmGZ9quq80x5MtHTIDFl6pDlum3jutP5JSX7DdA1FKY78QisWOQVYbV6kqEeZJK0D9hpwor9SpNk9eG4Pl9Uk69EOwsS6tT0PIRY1IXbMAE56iJoGXz/APQlIx/cDUu7U53SwqzeBM+VSuTjRq5D9qBtnXTNraFupl3CqmStsl9Irc1JBqFJZRnuVVpYSjZ0o1Cy41mnzDdGYsEElqpfmLlOiRiITDMrgMrz84tM14LaU3YdSWtH687cs0+kXTen1Wyumne7GiAyAVGVZ+i/9QUkr6kKoSwdQs59eSh9EUGEt3idD+0ZaSPR29+VBKrBOeXnotKzNp8ijKdvp0TudSyLtdiRzRHoxK0LFJUA02NxAdyQtXQR1/XXocIeuKT6YHHoJSoqM1HlMMt2ec9w6QHShplvGbOGp9O0au6fHL4yhJPUH0QeqrIsQ3Nwi+2O56dUJYM3Ephqq/Rl9XrUHeG/kVEcEw3HaYiD8OrrdUSRnt+on7pJR/yGVf49EPQyRB962krOQAvNAWaRMwPjKrSRBScivmBGYYFZByzhPKY2KFWeQomS3fowRE0QMYh6DhBH0pRVKyo/SiHp9g9X8QuiGYLJtMx2AySLtjzbic3L0Vgx7aiQP2bwdrn6sUCqZmbgJYOeNlIhIL9Ybc2nzSn6ebVoZOUImSwB1IFeKdUyFq8BR0pUM6MqC21dSvFvoFiMV2ZrydQFVp0abvUtSdKksxJx1U+Jllc5Hfwu+whxSaoaqSX6ktGQTK8dMUtFHCiaBlcUoF9DJjTjQLAW5YGjT5Ct4sxRkWyEnSLpmaVfQSZvISnI0/rTrai1hxSq2vCWrcJLomf/AMkV3eoCSAnGvvWzCMlt0yidGVxPosqaRS575RlC3Dik8dZsqnugcuLCizMVmQy6kwK2HBirt0Cof5XRagSu81A6PKg/RFgCH5Vyqu66UAqzKKLPnnZ3oZzt0nNyGalQgMsdZlQXji6oNhWZ/TsMnCtvW9kFatrP0W4jIDZUPv1kiF5MxDsoWapWhVewNFEKTVGB2AnwFcIOzpT86bFW0px/L7TazLfU4zCcuaZCGoDTefLVUKCVqC1B8bgYUDlWDk8uazon1krljAJQopDOB/vEtqjOF0WO31CLj7kgHaFUG37IAVQdFWM+GeQKMQ2whfexpkn+T6T9Qs2mDk/NUk2jRBT0acy3pFGBZkDTafKkURUBCmUmVj6ISt1KfMmQ1nsiB8anCJVMd+jWo+yMKuqh6+bELR1YL/CgNDofjQtpUiz4+OVFGdRR5+6KoUBgVdQccEdebrMOz1JSzTXzZHsBWLSikuop7yNqJKzJJyyPQPV3Ymb1oJsBSgCFQNJWMf45jLet29HSLKrMXolYlQFVcdCUE3Tc1VmjpYctE44ZH0lsWm6+f1b6SLksVYN+xJ+CvWx3ESxkP1H2fl2CyATeSfPkdLDVEHLqV/2Zjt/2ELKzE1+yKmOQW8lVp/FE27a5YUKKDQdO0lrQSRHZosVLIZJyAUHA73JCwahVV0WcAJLBKb3fd/eipaYjSzI1jFdqG7nItFywsHKpRECqaTNVm/8AmMDT+faB7rMHI8+QrBO5yDNx3RTQU7mAWIZiDkUKsaMr16KSQxTRJoR5pos5qm29ERzDqgWnIRx4sKickZ1ReTvcjxH3jO6M9XHLoZtRKMapKOjREeZADHtT7EP5p6HdYIsuY903iE57uyobILOZOylFUtNUuCVbuatZaA/RFSc2o1ZlERaac7J/E5GM0S6LTH/lQMQWek1xyk7G0pzBaoBHmakmbzb6NLTdqMYyl/MrNwvtSpciSqNoi1YkuylQwGmQORH4bQ5dDJyOQkpo7zo5XLoitRNF2qBJ2VVmqCn+yobnc1oOj+IwkTI5hPGcgkkScIL05mVo0ziY6pSrNNjEMWeQWVRShKD7FHWpo5h8cGRZvNBt3UDlCVX2DbTa0eip5LQSYnoobeS/9RH5SlGuq0ViJ0i8VXkAJYzR3sz+g3JQGix23adU6p39dX7SpR7FZETHM5r16DzXpVZ6KNEUo38p7rIfLj4WSWDs+i1hgwzEnk53ymhNCLNY0YBBI02CDS4BnSNPgqZ0cEo/KhyCR/jX5zJXYNPr3Wcy7+4dQGCiEmWqaeeiacBdybRU6ACi7NLSydrJNZjIg7FR8SDlY+s1JBICc8/by2G+NGlTypM+qcuXNGAoKFZszKrUInZgPqefmYU7bWghESWwka9BUSf5PpP1COjUKz0FaY5AYmzi7LMFaNNqKyUcBmLs8J+lKF6ONKZ0DCWlMdKGpOqnliSS51KQVqIvyJtNEBRIaKL21GsFYswok+ZrFbCoecVfY2Gab/Y1Sa0M1PB11Sex0xQVYz8wfEyZEU0Cl5sjjHcwKsQ7rMej9f6Hlv8Ab8ZAMisz3of9vJNVfc1Vkh/JWuQvkqouuiHUl5qWttf5qpLo/wDVmB0s0LM1kEiSEpTdCJWdeRNSziDqHbxYhgAxoqo9RsrNp17jKqJatEM1d6MGNJUd0buRCjmbTlShBLp9T8MfwD5tMAOk0eioJs48VyTIqKLQj6TcP6SoBos8iPhD+SYBaWsz6qjtq+V+Sp9OC7WBZzBKsk1mqSyWkCiOpPqjBQQZlSBNWYLQMeXVmyVSO2DkTas20EErFupB1YMoWjVQB1PQSeknot+QghVm5o9yDWLLQcFk67X6IgPoqzRVBDgDGrUr/Yn8FXLEkTvFFYI060RGnckGOqPQD6vVQvJJSbK7ykEAYEDEF6afKiIOCAzyZq4Xlz2VNcec2LdM8ks8G0x9OWk5WtZsWVwpn6qS1SrGZFn3zP0wyUnMPWrJKQE9AEyh8vKhVCtWmlAy6V1ZZiZUH5152NZ2YlzV6swrZbeYkqhm7VQjBUUKAYogYmp8215DfJTzKefHU4VUo/a1j15+Fnppx5LGYVZqnRR58q3zq04YWnU+XWa2h28wzDrShjPq0QjNw1TKw7ZtoALlSxDuGb6Dfb850Si2edMapGzNkNqKqTqaj03MqrLFCp82Vn9I8/dCs827cTmGpEFXkK8ToKAuzdTFLMHZKTRzRWrc0gul2YQ/05xkeSSrMsY7RJOVKZYRrN7rp4CdGVlT0ClZfHYtIO8N/Io7KEqepF6vr0DAAvP7uVKpw5rQl0VeaLVilGcdDap9THozNSZotSHmJu4rcsswoLSVE+5mjUDFAv8Aj5R3FEZmVQDJQhBah6dVQehANH/5LBURJDaGfZnJwrcg60O3ZS0WiRkKaMoYKk3R6EtItZaFTShVA6PKQQ9fgtSWBOSYXkgKFfQzPLQZPKeOOgjtZ1YaX7ll7dZlyGOKnPVAS/Y8+6AaXymyBT27y0basNGrT/mhJQMs6qk/duicFWVQVm5BDeaFwFYFpTauibt8kTYKBQs9SWBGpU+KoCKBVRRUFwCSWYOhfyKlpnzDFtCsoVk3+R+MByBmsSXLp57o00Z7OtLM7UKqchW56AnMUKVRkYsgVW+KRLkfXauQzheCt1QtFnJZ8hi2rI32MooqBzdQzFAKUKgT+aVHmsI83fk2IBLKFRkWqRZOk+33nV25eYS5REbhvwJFTIvRuRBaM0pSFGctVk5c/cUDmJRUdSVVFISYBb8owGxfRm9UlkovW5g6FiHqwiw0SF6BrLQRMaTq9aKpCL3SoYt+sOQICcKXVzO1JijKOrqBRpKHYMw6+jMVdEZAzo69nZM7v6o5yHJelMegdleTsy+j25cKJHTtkMGHTiy/vDjy/OsWVrSVqKwEapwKVbEj5zJE9Gh3jsEQnluaKaEJx3STCRMjmEwCSwSGfpUaZuVxJnhYjTqtbcgoxLP7ctUM0y2qA9O2100sUZldpOAiCHgCL8K7FW06jnvRp7vRfqdiddnh00CFwu6q0h95MXW5owJeR8iUBRlJDEOyAUTXbfg+Y1E6nGYDUFRRatTia7kEVSwnSqstSwDaDBt7H5JezPqqEET5ay91hVloZikbmcpXm4CNJVMpydarKvVURWYllRRY7mwXobEo84WRBN/RKXb7M9A9KMTVG82a8Br+SI9XUKzkIbDrZJSrz90SdpNtlLKs1RR9LSDJ1MkqsliSAhMWOy3xcqWTGE0lSS3oyFVb9hg+UDSSlx/JpMXX9dRK5iwltpdAuScUe2erKsBvESOtVDKJ9Fam29oU86Kg4pTrlaltIEIDMoRwtFbnbr/7ooqvOaCbAUIRHA6NaLt2ko9FduQe3/2mGDdOwI1hjkII+hefsXo0VZWiGHAakafcf8haT6VWXilJUKjgD823LKnNFpq9qMZKxpLcV/qpPxClQTXlFLyHjMZGOqMatdEAYPnTgOvJKFnKmqTVS5Bs7SFAgYNwt5M1Jso6Vgich/Uqu9/g+VAjdaLB0JVLK7IrW6QTsVUMrHz82b6OC7f+/wA4IQefMU+4UkOSMjHqkmCFkcj9ibOHLTpRHtbXuGDbWZg8vy9KI2psgoGB+jTDL6gCdR0WM+mMWH3b/wBIhwytULShanf2EglQi8BEjL7kMlC6OqBendykkP8A19M3w8VQeALzxqN41XXPmRTo+wR1AIREV+WZ6U79QhXHXS9sKCYBYJSwfqYZg1OikkXgGlmckLOjWozJUhPoT+Awny4nD9dv97IbnubyNUrkGbcurcyZiDTFmeSrunCo8RQAAmfR4JhnDkgXIqgLMr0UEqGtTpKq59rIyMyfb7UoXRGB0FWs7Nwgp8sRGZkUlnSYRqFOiNmcQ6t50DrWc/lDMK4eImZ0FCU7plKvQqg8hOIdXelDtTRygHnKqz2HfzCmooFY7Z2dFeLMG/YUFQZrH+dKFnVuQxbhAyzZ6qXJUIGUAuQc10fTZw0RMM2elG2VTqWxjMMzUUDhj5zOQJBf+N0aBj5eqotPQLNLLxLqL78pWqVyAmR69AVynCvCQK1rQl4czJ60OyRMf0E1sfP89qrFbsGGSAVm6I1TZqaCo7xp/hmk4BdAaA2pb5Ua68od0pUVD0TuiN6f+2P1VofZyDMNt16+r+ewN/KIBkV1QkARPMS9UvYLdv4K4Ykp2QCAUWnEqM6JpmKK3Rf7/VqfPHETbRfGew5UzZJ00syoZZkwvBGZNkFjMOfjon4/GOqzZfqGcLJqhe/N5FVIyft9fNutFQAdJJu15+2sQjMrLwd759i5AIBBNHzIF+ge+uWADBQ7a+Fghy5n3TxQfx5Be4jq2whMBUBWRgSRdGVPOg4UgVRWCNEEcqJO8xVe8KGhZSLPOc33WQaZlVC1TKn+BKaMzXmGUKrN5vY/GnLUPSg5qWWMnHM2/bSsSgCP9mm9UZEmLuldFUdFyPE8Md6ebqA2q19aJNu53qnSequSA9JeE/aX8OoqtBOXR75hjcPS9PRfNrzz+wGQ9gFkVneqSEaOF4qjDf8ALr4ai7CuXdUt/Q/iqOSoWkHqHnSmRORf2XcZlTcq1EOgVZeE6Wg4Fwqufz0XXmrzcijgqs6inMwX7ctSZeP0HDO9W2GorgPUIeZ4Kat4NcF3SMqpF3G16VOp8MqXbkBGUrUDIRp6Prv8tc4D0SxlJ6Pyl5hlRw6EGbhqtB2VSfFwRxKajqZ7T4pT/XWsaMs2yElzchaCbFwVorRi/Lq83vTlStW67L6gV1+BU1lNVq9hNCUQMUIDKoNZVopf0+WXsV2teqsZS9fsVW3NuqxoF+Bjb1RSxZWKUCzD1Umm6z5SJLgOvjo1B3hv5FNyHHiOuCPHGtIWnSgu/wDmulSQrjqss0PRG/Xcf5mFVHNP6KOULJTEeom6Rs0Vte//AOCLKgKIGVyfLJL+XnFlMmtfkMrvpqrjP/S/j52qgE0cSizEptmn6zblUUES5IrPzQjJ/X1LGM6MpagBsCGc3Sbzv5bjEhaRYKwdkagRArWyGpKvknOpoZtrknF+P+I9VpTtELsjUDlf150QPZRUMhcMAXq31ryE8ilUYkspIBtvstoLFiFBtNnKtR+RH5YMPsvFNwki5GMwc+XwSpNlcqlFdUVqzL0NuXU+s90lTzoalaATKeLilJtP9QljpLYCrQqKOKhuSNu7AKNdhZMDRvMMiuZtMNY8/GZEj230evNIFiDksVserSm6Ih6VYO4IL0oWDWaZNVIUTZqTZLoi/riR/g7LiqqGM3dE9hw5mgM6KhUuHIpZoSDEqujceoZ+p9/Tgg4XTySiUvUpTr9idJqzj0IA2dj0twG+k1/uvxsyUjRw9lRXdauqBmd0GMrut0LsTxRRSXQoPZHn1/Qj8haCNyA1dfhDIOZrWbycmqq6ybdi/jpQ4NfvjAVRA7PQ+7grlAkzJZOvNnXHrKrqCZyY1p/qRR/814Z/sZK/KMtJf8clVZgNV3JWmvoKRRwm52dd8KS2hPkBUB0pKUJQ/KA/goqkLK9f61R5UYN0ZsmQ5CHmh6JR+zpAn6lFZ9kLoAJLBWiYf5LPFlArN0Uu/kferIKuE9F9EDOEbhVlv181M+kgbABB0qvOijIdFmGmxZjoP8MpfxEQCFUOXfgcVKZ+dvNa+rVfhpGVWxqOaMXNrIDasShB5YLPnpD/ADP42+THxULkY1LUur24RU4I+ieyhQrsQUE2Y0USROUYZBH4zCRMjmEwHLBTBR/llqCJFC5P8IRjYrN7AD/6lKhpVLAO9x6OD2NLnT4ev09ACxmUAaY5Q5BoS0gq0CowciZVg4Hr2Qrz5r1pKKLGjRDVIV4Hhl9JI0iJFSJo0wzFXom4eWqauOSyESnkIQCp2zLaCq70WiqW5MghKmhR37hFrn0HKQ5AIzqE5cleAlVVH4Vh6S49UVwJqsyGIPXRQdTFp+qhz2fwBSxqOyaeizHyqrZOtowNWChg3STd1Adt2olKGhKyZCu87qK+lJegnWnJJoxWSuNuSGgtPkcv00zU2X1+XaWVEksGeVaz2liELQWjepS20EBV1opp6RIAA6uWPmwHIGaeIs374dWUlSBFVSaBBVp8hyzOUNnM7VLK7Uq9A4AclbE9KQVH5yM9Gpo6U99mfH7HJM3PLCgd4SdhRBwyjmfZ+V28TXy74Eh1zMP5rJLCNGcPbs1oqTb6lvWcrJGpVv8A1oR/ktMgBLPqYbl4p+uZrjCZcrVSSZLFF8wg0irBmGrNC09q162XN9RoiQeHhP32TkJWtFUha2SAIA2zSmzkzEnRgJ0b7owKJ52PO/nl/iu7O0x9pIFJ/pMz7L2WgSe2cqr1bksk4qvprpN+fCzz5pR5uXk1Go/TaW3oFYByZKlC6xmyO89Eu96NQrNSZDu0vULKu2QAY6K5qZuiIH0QQxlNeqMEYTPky/03LPLt/wC1N1UBaJpMau+RyVrgKzhQtpFj7UcsXYFkWSzsDyGUTKspMF3NuKDTaGY+f2GaUyxCzIVbD6tRHdhQVCFTR/ZqVpNnFz6oyr1r0DuvynMoZCZAKtNW/XLiBDzKBsuyBqV4MNML9WrsKBHLc80Wlu/QK6JSzpRRss5Wahogg/ZVob/6KPs1JhhxcAtzhaTvy92yeeaLai9FM5sBREM3I6VqFHGSgRgHvQcIk2VnFKsdXULwtKeVBILRDVYtSk1bY2dBYseJkdCZWriWlZQEOj+Mq/alWatF/bQ9IizQlkLgGm328VD1K8hihn066G+Y/epliVBWro6mi0BMjyt3ZxtOybS5mDro9Gv1E2TCAQLtPgc/aSlLMrFy5lqrTdOUdgyvIzkYEigRTbGZuar5/wA6SJp+MVCrBEAMFup12aXx8dpS+rsaBaVDOipXb+L1nT2HPyAS2OWu4CsEqqN6G3Kshmg4HXIs9Ah9QsX783iWdNbTyoBN/Hr4YATBEwpSZhz0Vt5L0GtUAFe6CW6IPyELJuFg71SJtLIZ5KqIFNGpXtnZ/wCfjaTK6jo/r0AmHbX1acgTlwKQ9Is95PpWQCBoarIqGeoMVYBp7K6x9FQ2zNVno25M7O/ChIOH4gjsyBldWbtGdKsGqtVsRwIqfvy7dV5qkwAwSjqXjdnVBd5AAFzSfwVdmPPn/YfgsYi9C44NtqqI0xeN9K00Zlm3bNOdXE2WjKH362ajq4Po6tb0dU4VgBWdHDraVKOlECeTIyik56m5Llyqn/LhqMVxmX9o/slVxCSzUAptVHS5CBuGOwgVQD/OCUCzm4ZVczY0JC/j1reYsfRS8xxF3CWFAxGlkHDUZiqy07t6BPNdBQB+CIO8N/IpAP8Anr0eRE5soUMxk6urrT/QaojBVbbUdrUq1gaDefZhNyoDMp+7gpARU9T83ailXVKLW7FuuaBZtXpuWUNGY7NF9VIY/wCWFJrqKoEX0DaRiwoVmzecy69/h0ZzRqUnJyJBAtG4xkr4PS2Mx9EBszISwR5qprSfs3HJFskKoam5zmwX/rlXmatcECfuCgVQ02VCmyxK2ND8r+UzEp0sUk1A7hxCv2CtQc1vOyydWWbz4aU1LgGg0CQy8FmwkJq5E6yVHtQpPttEcGhRSsFWc5oC/ogXUAyq7zUDrSUkZE1RpJ3KyD6pQu23nQoAys5DlLMjttQoTYhOirFuewVdKornoszyWSg0QUYqQR5t2rOtSCb03zNgT9VyUKdLKdGX0DrOrMK8OqnrJiq+QdOqiXpViE39pbdKiR5M6BqlXI8myp1ryWJkFDD7At8NUurdKT/g0ZCppd6qJJVnKqgvFS7FZc8uDMCzzIZ/RhssIfX0EwwqWE361oj5YgrR1BVXohLu1VaxYmhBVQceda2e3LAslLK0qlAFXd1NGWlJUUosysCUJQMyDIe/ARciLqstUUrJijhUZQ41zKgm0xXHQHpeixUWkFFXBMy4m6EhURtoUfTuD9RkiM6Ip0HqFE16KJSrpQO23VS3dQ6kLd1V6O8xwrqJkEhwGF7v0WOUl0t0YNJfPihI+WE3q5Aer/LUPsD6JKhoxJlL9krJVTZK6J6zk7DsFzREHkNFu5xViRcM381Bn/Q9M1eRwTGVR/igtLmDpUt1XhSXYqHZSjGZ111uI9FofQqgFG0XKlis/wBgVSgXHV5JNgoRnLMSGRipCgq6EJTR4FjHFWFsp80xEnKnmkw3ItJ70s/lQ3cqmRSu5ytSdAyAhaURCoUjTEtYUGg646lgjddLY8RYM1XojrWhT7mTupNNsG/6h+AoNodspYKasCaq0mR6IH3L5P1dPEKekRpp3EbXZIF9DNMhZqzOHZldxj8FknJkBTudF40qtNGqDBZn2P4KBUTbXJL3RSZ8ULbZqdlVAHmGKlgpVT8KJorbGm7/APX4KMmiQ7doitJ4LQ+bFle7UmH5yJtEGsJlACDBu019smHYKKJMFiSxazFnqpMj5trgSdWH2dWU1AVj8/jXZ3VhR4CnNDVCgVgrIy+85KD5S2TRHVUWaOtldeTsVzFBiP8ALPflX2QNSUsiVoFuGxmVlpEkoXTqmQF2Rw6A/ViheZnNav8AOsXzcym8qLOTgP51SNNVo1Qrg/0Ei9EacyomqG02q/A57xQykywZmCfecuij1+4LNSrKEPCpVZLzy+plq+ekLGZA8ZzCOxtZpI/ckpRFZrBqLNGowXX6iRJKF/miq52jQzbXJN4vyAw36BzXLHHezegIoWXl0op36cnzaZLojD+AUqLiikcx2pZBpdCXmHzyZSfFD1quPEzZzStdtZmkqOS/p/VfkAeXRT80PkmKzU9q1Ck8ShDVvtl6ehClZp5oz9E+fe6du0m5AP6LdXRSiVVt3n+zakyqByD9loqsrrSaDikweUf2+uZEj230evNSfuMW9PSfgjVseJWzsUXzl96K6nJmPUcznJmVQ425ghPK9SebeA0J/mI2WiO1UKxW8/6Vb79hP/RBdqBa12v1iDHpRtrHgf8ATD08XdmAIb9MFfRWZmnMHtlagcOTxSiRXtgcBIDc03LzTS0KAi3nMkxo7swyHUoRpZqSKqqAH8gM83bSqzLsXLG51vkh5Ulu0+JTKbQAOoyH7dUPbMhjP1PvRJOlSKfzVBvpH2kqRV/8ZVnPIIbhAbAr6bqru9aIFUGSKFDv5/XWLMpcFyqo9tOAKbkPozNy5CL3JrQUL9/2GNJ9AlApdxFjN6pSDZMpgok1Knjr5dnScxQqy7IN/wDNR/j8Zw2xPqyhj+MbnJ/G+TrGblDOqhDRLGKpWpdfMuzT9WNGZntFwjlKBaxRFm2+krkEVryRWNqLFNF2DvBvkQgXdUkz9jkFiQWVgz7KINHQ0+0lyOSg5mpmbqRRW82RQrIT8O4Zis5R27d";

        //        //    var obj1 = new Models.Image
        //        //    {
        //        //        imageData = upImages,
        //        //        filename = "aaa",
        //        //        senderID = Guid.NewGuid(),
        //        //    };

        //        //    var uploadServiceBaseAddress = "http://uploadmediatoserver.azurewebsites.net/api/Files/UploadMedia2";

        //        //    var client = new HttpClient();

        //        //    string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj1);
        //        //    var content = new StringContent(json, Encoding.Unicode, "application/json");

        //        //    var result = client.PostAsync(uploadServiceBaseAddress, content).Result;

        //        //    if (result.IsSuccessStatusCode)
        //        //    {
        //        //        var tokenJson = await result.Content.ReadAsStringAsync();
        //        //    }
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        DisplayAlert("An error occurred: '{0}'", ex.Message, "ok");
        //    }

        //}
    }
}