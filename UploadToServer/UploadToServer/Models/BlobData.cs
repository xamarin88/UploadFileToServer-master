using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadToServer.Models
{
    public class BlobData
    {
        public int imageId { get; set; }
        public string filePath { get; set; }
        public string fileExt { get; set; }
        public string senderNumber { get; set; }
        public decimal senderLat { get; set; }
        public decimal senderLong { get; set; }
        public DateTime createdOn { get; set; }
    }
}