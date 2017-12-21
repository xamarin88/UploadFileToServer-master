using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using UploadToServer.Server.Models;
using System.Data.SqlClient;
using System.Data;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

namespace UploadToServer.Server.Controllers
{
    public class UploadsController : ApiController
    {
        [Route("api/Files/Upload")]
        public async Task<string> Post()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;

                if (httpRequest.Files.Count > 0)
                {
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];

                        var fileName = postedFile.FileName.Split('\\').LastOrDefault().Split('/').LastOrDefault();

                        var filePath = HttpContext.Current.Server.MapPath("~/Uploads/" + fileName);

                        postedFile.SaveAs(filePath);

                        return "/Uploads/" + fileName;
                    }
                }
            }
            catch (Exception exception)
            {
                return exception.Message;
            }

            return "no files";
        }

        [Route("api/Files/Upload2")]
        public async Task<string> Post2()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;

                if (httpRequest.Files.Count > 0)
                {
                    //string accountname = "uploadmediatoserverstore";

                    //string accesskey = "F2rmquz19Rm5fzWLWSuEOgfu+UPvjoWxMdPlsuWhL6+MVfpaSv5TxlRukV/uIMZGkS8Wuw7IljhtMoQ6J6ozqw==";

                    //string connectionString = "DefaultEndpointsProtocol=https;AccountName=uploadmediatoserverstore;AccountKey=F2rmquz19Rm5fzWLWSuEOgfu+UPvjoWxMdPlsuWhL6+MVfpaSv5TxlRukV/uIMZGkS8Wuw7IljhtMoQ6J6ozqw==;EndpointSuffix=core.windows.net";

                    //string localPath = "";


                    //var accountName = "uploadmediatoserverstore";
                    //var accountKey = "F2rmquz19Rm5fzWLWSuEOgfu+UPvjoWxMdPlsuWhL6+MVfpaSv5TxlRukV/uIMZGkS8Wuw7IljhtMoQ6J6ozqw==";
                    //var storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);

                    //CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    //CloudBlobContainer container = blobClient.GetContainerReference("images");

                    //container.CreateIfNotExists();
                    

                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=uploadmediatoserverstore;AccountKey=F2rmquz19Rm5fzWLWSuEOgfu+UPvjoWxMdPlsuWhL6+MVfpaSv5TxlRukV/uIMZGkS8Wuw7IljhtMoQ6J6ozqw==;EndpointSuffix=core.windows.net");

                    // Create the blob client.
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                    // Retrieve a reference to a container.
                    CloudBlobContainer container = blobClient.GetContainerReference("images");

                    container.CreateIfNotExists();

                    container.SetPermissions(
                        new Microsoft.WindowsAzure.Storage.Blob.BlobContainerPermissions
                        { PublicAccess = Microsoft.WindowsAzure.Storage.Blob.BlobContainerPublicAccessType.Blob });

                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];

                        var fileName = postedFile.FileName.Split('\\').LastOrDefault().Split('/').LastOrDefault();
                        var stream = postedFile.InputStream;

                        CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

                        await blockBlob.UploadFromStreamAsync(stream);

                        stream.Dispose();
                        return blockBlob?.Uri.ToString();
                    }



                    



                    //foreach (string file in httpRequest.Files)
                    //{
                    //    HttpPostedFile image = file;

                    //    using (file.InputStream)
                    //    {
                    //        blockBlob.UploadFromStream(image.InputStream);
                    //        byte[] fileData = null;
                    //        using (var binaryReader = new BinaryReader(image.InputStream))
                    //        {
                    //            fileData = binaryReader.ReadBytes(image.ContentLength);
                    //        }
                    //        string base64ImageRepresentation = Convert.ToBase64String(fileData);

                    //        // Clear and send the response back to the browser.
                    //        string json = "";
                    //        Hashtable resp = new Hashtable();
                    //        resp.Add("link", "data:image/" + System.IO.Path.GetExtension(image.FileName).Replace(@".", "") + ";base64," + base64ImageRepresentation);
                    //        resp.Add("imgID", "BLOB/" + fileName);
                    //        json = JsonConvert.SerializeObject(resp);
                    //        Response.Clear();
                    //        Response.ContentType = "application/json; charset=utf-8";
                    //        Response.Write(json);
                    //        Response.End();
                    //    }
                    //}



                    //foreach (string file in httpRequest.Files)
                    //{
                    //    var postedFile = httpRequest.Files[file];

                    //    var fileName = postedFile.FileName.Split('\\').LastOrDefault().Split('/').LastOrDefault();

                    //    CloudBlockBlob blob = container.GetBlockBlobReference(fileName);
                    //    blob.UploadFromStream()
                    //    //var filePath = HttpContext.Current.Server.MapPath("~/Uploads/" + fileName);

                    //    //postedFile.SaveAs(filePath);

                    //    //return "/Uploads/" + fileName;
                    //}
                }
            }
            catch (Exception exception)
            {
                return exception.Message;
            }

            return "no files";
        }

        [Route("api/Files/UploadMedia")]
        public async Task<string> PostMedia(Image image)
        {
           try
            {
                SqlConnection conn = null;
                SqlCommand command = null;

                var connectionString = string.Empty;
                var json = string.Empty;
                connectionString = "Server=tcp:uploadtoserver.database.windows.net,1433;Initial Catalog=UploadFileDB;Persist Security Info=False;User ID=uploadtoserveradmin;Password=Password123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=90;";

                conn = new SqlConnection(connectionString);
                command = new SqlCommand("MediaDetailsIns", conn);
                command.CommandTimeout = 0;

                command.CommandType = CommandType.StoredProcedure;
                conn.Open();

                //command.Parameters.Add("@imageData", SqlDbType.VarBinary, image.imageData.Length);
                command.Parameters.Add("@imageData", SqlDbType.NVarChar, -1);
                command.Parameters.Add("@filename", SqlDbType.VarChar, 999);
                command.Parameters.Add("@senderID", SqlDbType.UniqueIdentifier);

                command.Parameters["@imageData"].Value = image.imageData;
                command.Parameters["@filename"].Value = image.filename;
                command.Parameters["@senderID"].Value = image.senderID;

                command.ExecuteNonQuery();

                conn.Close();
                conn.Dispose();

                HttpResponseMessage response = Request.CreateResponse<Image>(HttpStatusCode.Created, image);
                return response.StatusCode.ToString();
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
            return "upload failed";
        }

        [Route("api/Files/UploadMedia2")]
        public async Task<string> PostMedia2(Image image)
        {
            try
            {
                SqlConnection conn = null;
                SqlCommand command = null;

                byte[] encodedDataAsBytes = System.Convert.FromBase64String(image.imageData);
                //string returnValue = System.Text.Encoding.UTF8.GetString(encodedDataAsBytes);

                var connectionString = string.Empty;
                var json = string.Empty;
                connectionString = "Server=tcp:uploadtoserver.database.windows.net,1433;Initial Catalog=UploadFileDB;Persist Security Info=False;User ID=uploadtoserveradmin;Password=Password123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=90;";

                conn = new SqlConnection(connectionString);
                command = new SqlCommand("MediaDetailsIns2", conn);
                command.CommandTimeout = 0;

                command.CommandType = CommandType.StoredProcedure;
                conn.Open();

                command.Parameters.Add("@imageData", SqlDbType.VarBinary, -1);
                command.Parameters.Add("@filename", SqlDbType.VarChar, 999);
                command.Parameters.Add("@senderID", SqlDbType.UniqueIdentifier);

                command.Parameters["@imageData"].Value = encodedDataAsBytes; // image.imageData;
                command.Parameters["@filename"].Value = image.filename;
                command.Parameters["@senderID"].Value = image.senderID;

                command.ExecuteNonQuery();

                conn.Close();
                conn.Dispose();

                HttpResponseMessage response = Request.CreateResponse<Image>(HttpStatusCode.Created, image);
                return response.StatusCode.ToString();
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
            return "upload failed";
        }

        //[Route("api/Files/GetStoredProcedure")]
        //public async Task<string> GetStoredProcedure(string spname)
        //{
        //    SqlConnection myConnection = new SqlConnection();
        //    SqlCommand sqlCmd = new SqlCommand();
        //    var connectionString = string.Empty;
        //    connectionString = "Server=tcp:uploadtoserver.database.windows.net,1433;Initial Catalog=UploadFileDB;Persist Security Info=False;User ID=uploadtoserveradmin;Password=Password123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        //    //myConnection = new SqlConnection(connectionString);
        //    //sqlCmd.CommandText = "select * from sys.procedures where name, @name";
        //    //sqlCmd.Parameters.AddWithValue("name", spname);
        //    //sqlCmd.CommandType = CommandType.Text;

        //    myConnection = new SqlConnection(connectionString);
        //    sqlCmd = new SqlCommand("select * from sys.procedures where name = 'MediaDetailsIns'", myConnection);

        //    sqlCmd.CommandType = CommandType.Text;
        //    myConnection.Open();

        //    DataTable dt = new DataTable();
        //    using (SqlDataAdapter a = new SqlDataAdapter(sqlCmd))
        //    {
        //        a.Fill(dt);
        //    }

        //    return "aaa";
        //}
    }
}