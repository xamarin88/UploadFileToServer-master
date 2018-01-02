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
        public string adminArea { get; set; }
        public string countryCode { get; set; }
        public string countryName { get; set; }
        public string featureName { get; set; }
        public string locality { get; set; }
        public string postalCode { get; set; }
        public string subAdminArea { get; set; }
        public string subLocality { get; set; }
        public string subThoroughFare { get; set; }
        public string thoroughFare { get; set; }
        public DateTime createdOn { get; set; }
    }
}