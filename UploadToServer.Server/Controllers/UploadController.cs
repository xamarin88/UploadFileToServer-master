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
using System.Configuration;

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
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["BlobStorageConnectionString"]);

                    // Create the blob client.
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                    // Retrieve a reference to a container.
                    CloudBlobContainer container = blobClient.GetContainerReference(ConfigurationManager.AppSettings["ImagesContainer"]);

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
                connectionString = ConfigurationManager.AppSettings["ConnectionString"];

                conn = new SqlConnection(connectionString);
                command = new SqlCommand("MediaDetailsIns", conn);
                command.CommandTimeout = 0;

                command.CommandType = CommandType.StoredProcedure;
                conn.Open();

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
                
                var connectionString = string.Empty;
                var json = string.Empty;
                connectionString = ConfigurationManager.AppSettings["ConnectionString"];

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

        [Route("api/Files/UpdateBlobData")]
        public async Task<string> PostBlobData(BlobData blobData)
        {
            try
            {
                SqlConnection conn = null;
                SqlCommand command = null;
                
                var connectionString = string.Empty;
                var json = string.Empty;
                connectionString = ConfigurationManager.AppSettings["ConnectionString"];
                //connectionString = "Server=tcp:uploadtoserver.database.windows.net,1433;Initial Catalog=UploadFileDB;Persist Security Info=False;User ID=uploadtoserveradmin;Password=Password123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=90;";

                conn = new SqlConnection(connectionString);
                command = new SqlCommand("BlobStorageIns", conn);
                command.CommandTimeout = 0;

                command.CommandType = CommandType.StoredProcedure;
                conn.Open();

                command.Parameters.Add("@filePath", SqlDbType.VarChar, 255);
                command.Parameters.Add("@fileExt", SqlDbType.VarChar, 10);
                command.Parameters.Add("@senderNumber", SqlDbType.VarChar, 30);
                command.Parameters.Add("@senderLat", SqlDbType.Decimal);
                command.Parameters.Add("@senderLong", SqlDbType.Decimal);
                command.Parameters.Add("@createdOn", SqlDbType.DateTime);

                command.Parameters["@filePath"].Value = blobData.filePath;
                command.Parameters["@fileExt"].Value = blobData.fileExt;
                command.Parameters["@senderNumber"].Value = blobData.senderNumber;
                command.Parameters["@senderLat"].Value = blobData.senderLat;
                command.Parameters["@senderLong"].Value = blobData.senderLong;
                command.Parameters["@createdOn"].Value = DateTime.UtcNow;

                command.ExecuteNonQuery();

                conn.Close();
                conn.Dispose();

                HttpResponseMessage response = Request.CreateResponse<BlobData>(HttpStatusCode.Created, blobData);
                return response.StatusCode.ToString();
            }
            catch (Exception exception)
            {
                return exception.Message;
            }

            return "upload failed";
        }
    }
}