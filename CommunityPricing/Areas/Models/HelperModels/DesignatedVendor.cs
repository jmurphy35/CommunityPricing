using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityPricing.Areas.Models.HelperModels
{
    public class DesignatedVendor
    {
        public int VendorID { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public bool Designated { get; set; }
    }
}
