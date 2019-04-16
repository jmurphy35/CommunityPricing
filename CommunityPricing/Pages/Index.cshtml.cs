using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using CommunityPricing.Pages.Shared;
using CommunityPricing.Data;


namespace CommunityPricing.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public IndexModel(CommunityPricing.Data.CommunityPricingContext context)
        {
            _context = context;
        }
        
        public void OnGet()
        {

        }
    }
}
