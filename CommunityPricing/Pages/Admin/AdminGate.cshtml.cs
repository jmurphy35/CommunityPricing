using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CommunityPricing.Areas.Data;
using CommunityPricing.Areas.Authorization;
using CommunityPricing.Pages.Shared;
using CommunityPricing.Models;

namespace CommunityPricing.Pages.Admin
{
    public class AdminGateModel : DI_BasePageModel
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;
        public AdminGateModel(CommunityPricing.Data.CommunityPricingContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
        }

        public Vendor Vendor { get; set; }
        public string Message { get; set; }
        public async Task OnGetAsync()
        {
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Vendor, Operations.Read);

            if (!isAuthorized.Succeeded)
            {
                new ChallengeResult();
                //return new ChallengeResult();
            }

            string message = "This is the administrator's gateway page.";
            Message = message;
        }
    }
}