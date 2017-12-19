using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UploadToServer.Server.Models
{
    public class Image
    {
        public int imageId { get; set; }
        public string imageData { get; set; }
        public string filename { get; set; }
        public Guid senderID { get; set; }
    }
}