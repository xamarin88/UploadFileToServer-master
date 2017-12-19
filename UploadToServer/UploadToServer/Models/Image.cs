using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadToServer.Models
{
    public class Image
    {
        public int imageId { get; set; }
        //public byte[] imageData { get; set; }
        public string imageData { get; set; }
        public string filename { get; set; }
        public Guid senderID { get; set; }
    }
}
