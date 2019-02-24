using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CommunityPricing.Data;
using CommunityPricing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CommunityPricing.Areas.Data;
using CommunityPricing.Areas.Authorization;
using CommunityPricing.Pages.Shared;

namespace CommunityPricing.Pages.Admin.VendorPages
{
    public class DetailsModel : DI_BasePageModel
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public DetailsModel(CommunityPricing.Data.CommunityPricingContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
        }

        public Vendor Vendor { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Vendor = await _context.Vendor
                .Include(o => o.Offering)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.VendorID == id);

     //**********************Authorization***************************************************************

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Vendor, Operations.Detail);

            if(!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }  
     //*************************************************************************************************
            if (Vendor == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
