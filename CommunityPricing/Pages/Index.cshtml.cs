using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using CommunityPricing.Pages.Shared;


namespace CommunityPricing.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        public decimal Average { get; set; }
        public void OnGet()
        {
          
        }
    }
}
