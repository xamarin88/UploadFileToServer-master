using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UploadToServer.Server.Models
{
    public class BlobData
    {
        public int imageId { get; set; }
        public string filePath { get; set; }
        public string fileExt { get; set; }
        public string senderNumber { get; set; }
        public decimal senderLoc { get; set; }
        public DateTime createdOn { get; set; }
    }
}