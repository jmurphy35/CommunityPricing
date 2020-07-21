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

namespace CommunityPricing.Pages.Admin
{
    public class AdminArchiveModel : DI_BasePageModel
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public AdminArchiveModel(CommunityPricing.Data.CommunityPricingContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
        }
        public string ProductNameSort { get; set; }
        public string VendorNameSort { get; set; }
        public string CurrentSort { get; set; }

        public IList<ArchivedOffering> ArchivedOffering { get;set; }

        public async Task<IActionResult> OnGetAsync(string sortOrder)
        {
            Offering offering = new Offering();
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, offering, Operations.Read);
            if(!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            ProductNameSort = string.IsNullOrEmpty(sortOrder) ? "prodnamesort_desc" : "";
            VendorNameSort = sortOrder == "Vendornamesort" ? "vendornamesort_desc" : "Vendornamesort";

            IQueryable<ArchivedOffering> archivedOfferingIQ = from aO in _context.ArchivedOffering
                                                              .Include(o => o.Offering)
                                                              .ThenInclude(v => v.Vendor)
                                                              .Include(o => o.Offering)
                                                              .ThenInclude(p => p.Product)
                                                              select aO;

            switch (sortOrder)
            {
                case "prodnamesort_desc":
                    archivedOfferingIQ = archivedOfferingIQ.OrderByDescending(aO => aO.Offering.Product.ProductName)
                        .ThenBy(aO => aO.Offering.Product.Wholesaler)
                        .ThenBy(aO => aO.Offering.Product.ProductDescr1)
                        .ThenBy(aO => aO.Date);
                    break;

                case "Vendornamesort":
                    archivedOfferingIQ = archivedOfferingIQ.OrderBy(aO => aO.Offering.Vendor.VendorName)
                        .ThenBy(aO => aO.Date)
                        .ThenBy(aO => aO.Offering.Product.ProductName)
                        .ThenBy(aO => aO.Offering.Product.ProductDescr1);
                    break;

                case "vendornamesort_desc":
                    archivedOfferingIQ = archivedOfferingIQ.OrderByDescending(aO => aO.Offering.Vendor.VendorName)
                        .ThenBy(aO => aO.Date)
                        .ThenBy(aO => aO.Offering.Product.ProductName)
                        .ThenBy(aO => aO.Offering.Product.ProductDescr1);
                    break;
                    
                default:
                    archivedOfferingIQ = archivedOfferingIQ.OrderBy(aO => aO.Offering.Product.ProductName)
                        .ThenBy(aO => aO.Offering.Product.Wholesaler)
                        .ThenBy(aO => aO.Offering.Product.ProductDescr1)
                        .ThenBy(aO => aO.Date);
                    break;
            }

            ArchivedOffering = archivedOfferingIQ.ToList();
            return Page();
        }
    }
}
