using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommunityPricing.Pages.Admin
{
    public class AdminGateModel : PageModel
    {
        public string Message { get; set; }
        public void OnGet()
        {
            string message = "This is the administrator's gateway page.";
            Message = message;
        }
    }
}