using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UploadToServer.Server.Controllers;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using UploadToServer.Server.Models;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web.Http;
using System.Net.Http;
using System.Web.Http.Hosting;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Web;

namespace UploadToServer.Server.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            UploadsController controller = new UploadsController();
            
            controller.Request = new HttpRequestMessage()
            {
                Properties = { { HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
            };

           byte[] toBytes = File.ReadAllBytes("D:\\myImage_6.jpg");
            String s = Convert.ToBase64String(toBytes);
            
            //byte[] toBytes = Encoding.ASCII.GetBytes("aaa");

            var obj1 = new Image
            {
                imageData = s,
                filename = "aaa",
                senderID = Guid.NewGuid(),
            };

            var result = await controller.PostMedia(obj1);
        }

        [TestMethod]
        public async Task TestMethod2()
        {
            UploadsController controller = new UploadsController();

            controller.Request = new HttpRequestMessage()
            {
                Properties = { { HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
            };

            byte[] toBytes = File.ReadAllBytes("D:\\myImage_18.jpg");
            String s = Convert.ToBase64String(toBytes);

            //byte[] toBytes = Encoding.ASCII.GetBytes("aaa");

            var obj1 = new Image
            {
                imageData = s,
                filename = "aaa",
                senderID = Guid.NewGuid(),
            };

            var result = await controller.PostMedia2(obj1);
        }

        [TestMethod]
        public async Task TestMethod3()
        {
            UploadsController controller = new UploadsController();

            controller.Request = new HttpRequestMessage()
            {
                Properties = { { HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
            };

            //byte[] toBytes = File.ReadAllBytes("D:\\images.jpg");
            byte[] toBytes = File.ReadAllBytes("D:\\myImage_18.jpg");
            String s = Convert.ToBase64String(toBytes);

            //byte[] aaa = Encoding.UTF8.GetBytes("sssss");
            //String bbb = Convert.ToBase64String(aaa);

            var obj1 = new Image
            {
                imageData = s,
                //imageData = bbb,
                filename = "test2",
                senderID = Guid.NewGuid(),
            };


            //1st approach
            var client = new HttpClient();

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj1);
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var uploadServiceBaseAddress = "http://uploadmediatoserver.azurewebsites.net/api/Files/UploadMedia2";

            HttpResponseMessage response = null;
            response = await client.PostAsync(uploadServiceBaseAddress, content);

            ////2nd approach
            //var client = new HttpClient();
            //var uploadServiceBaseAddress = "http://uploadmediatoserver.azurewebsites.net/api/Files/UploadMedia2";
            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj1, Formatting.Indented);

            //var buffer = System.Text.Encoding.UTF8.GetBytes(json);

            //var byteContent = new ByteArrayContent(buffer);

            //byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            //var httpResponce = client.PostAsync(uploadServiceBaseAddress, byteContent).Result;
        }

        [TestMethod]
        public void TestMethod4()
        {
            //var quotedString = "\"hello\"";
            //var unQuotedString = quotedString.TrimStart('"').TrimEnd('"');

            //// If the characters are the same, then you only need one call to Trim('"'):
            //unQuotedString = quotedString.Trim('"');

            //Console.WriteLine(quotedString);
            //Console.WriteLine(unQuotedString);

            String value = "http:\\asdasdad.asdasdjk.asdasdasdjklj\\image.jpg";
            //Char delimiter = '.';
            //String[] substrings = value.Split(delimiter);
            //foreach (var substring in substrings)
            //    Console.WriteLine(substring);

            string lastPart = value.Split('.').Last();
            Console.WriteLine(lastPart);
        }
        //[TestMethod]
        //public async Task get()
        //{
        //    UploadsController controller = new UploadsController();

        //    controller.Request = new HttpRequestMessage()
        //    {
        //        Properties = { { HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
        //    };

        //    var result = await controller.GetStoredProcedure("dbo.MediaDetailsIns");
        //}
    }
}
