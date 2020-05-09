using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommunityPricing.Areas.Models.HelperModels
{
    

    public class CalculatedInflation
    {
        public int InflationID { get; set; }

        [Display(Name = "Category")]
        public string InflationName { get; set; }

        [Display(Name = "Inflation Rate")]
        public string InflationRate { get; set; }

        [Display(Name = "Published Inflation")]
        public string PublishedInflation { get; set; }

        [Display(Name = "For Year")]
        public int FiscalYear { get; set; }
        public DateTime SinceInception { get; set; }
    }

    public class ReferencedInflation
    {
        public int InflationID { get; set; }
        public string InflationName { get; set; }
        public string InflationRate { get; set; }

    
        public int FiscalYear { get; set; }
        public DateTime SinceInception { get; set; }
    }
}
