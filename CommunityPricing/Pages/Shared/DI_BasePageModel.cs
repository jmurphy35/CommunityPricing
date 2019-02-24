using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CommunityPricing.Data;
using CommunityPricing.Areas.Data;

namespace CommunityPricing.Pages.Shared
{
    public class DI_BasePageModel : PageModel
    {
        protected CommunityPricingContext Context { get; }
        protected IAuthorizationService AuthorizationService { get; }
        protected UserManager<ApplicationUser> UserManager  { get; }

        public DI_BasePageModel(
            CommunityPricingContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager
            ) : base()
        {
            Context = context;
            AuthorizationService = authorizationService;
            UserManager = userManager;
        }
    }
}
