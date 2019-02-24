using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityPricing.Areas.Models;


namespace CommunityPricing.Models
{
    public class Offering
    {
        //Using ID instead of classnameID makes inheritance easier to implement.
        public Guid OfferingID { get; set; }

        //If i did this: public int? propname, then the ? would enter a nullable field into the db for me.
        public Guid ProductID { get; set; }
        public int VendorID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime AsOfDate { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "n/a")]
        public decimal? ProductPricePerWeight { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public Product Product { get; set; }
        public Vendor Vendor { get; set; }
        public ICollection<ArchivedOffering> ArchivedOffering { get; set; }
    }
}
