using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CommunityPricing.Models
{
    public class Vendor
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VendorID { get; set; }
        public string OwnerID { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress1 { get; set; }
        public string VendorAddress2 { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public ICollection<Offering> Offering { get; set; }
    }
}
