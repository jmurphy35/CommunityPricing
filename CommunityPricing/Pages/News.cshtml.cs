﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace CommunityPricing.Pages.Shared
{
    [AllowAnonymous]
    public class NewsModel : PageModel
    {
       
        public void OnGet()
        {

        }
    }
}