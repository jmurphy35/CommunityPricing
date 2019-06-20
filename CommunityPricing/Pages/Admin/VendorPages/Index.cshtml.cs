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
    public class IndexModel : DI_BasePageModel
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public IndexModel(CommunityPricing.Data.CommunityPricingContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
        }

        public string NameSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }


        public PaginatedList<Vendor> Vendor { get;set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, 
            string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            CurrentFilter = searchString;
            if(searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            CurrentFilter = searchString;

            IQueryable<Vendor> VendorIQ = from v in _context.Vendor
                                          select v;

            if(!String.IsNullOrEmpty(searchString))
            {
                VendorIQ = VendorIQ.Where(v => v.VendorName.Contains(searchString));
            }
            
            switch (sortOrder)
            {
                case "name_desc":
                    VendorIQ = VendorIQ.OrderByDescending(v => v.VendorName);
                    break;

                default:
                    VendorIQ = VendorIQ.OrderBy(v => v.VendorName);
                    break;
            }

            int pageSize = 9;
            Vendor = await PaginatedList<Vendor>.CreateAsync(
                VendorIQ.AsNoTracking(), pageIndex ?? 1, pageSize);

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Vendor, Operations.Read);

            if (!isAuthorized.Succeeded)
            {
                new ChallengeResult();
                //return new ChallengeResult();
            }

            
            //Find a way to redirectPage to the login page. So it ends up in the same place as a ChallengeResult
        }
    }
}
