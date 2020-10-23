using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityPricing.Pages.Shared
{
    public class ProductCategoryDisplayModel
    {
        public int Number { get; set; }

        [Display(Name="Please select from the following: ")]
        public string Name { get; set; }

    }
}
