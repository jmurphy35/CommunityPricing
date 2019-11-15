using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CommunityPricing.Areas.Models.HelperModels
{
    public class DisplayDetailHelper
    {
        public Guid ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string ProductWeight_Volume { get; set; }
        public string Wholesaler { get; set; }
        public string Category { get; set; }

        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public string VendorAddress2 { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "not avail.")]
        public decimal? PricePerUnit { get; set; }
        public decimal?
            Average { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MMM-dd}")]
        public DateTime asOfDate { get; set; }

    }
}
