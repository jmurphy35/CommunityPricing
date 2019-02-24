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

namespace CommunityPricing.Pages.Admin.ProductCategoryPages
{
    public class DetailsModel : DI_BasePageModel
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public DetailsModel(CommunityPricing.Data.CommunityPricingContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base (context, authorizationService, userManager)
        {
            _context = context;
        }

        public ProductCategory ProductCategory { get; set; }
        public IList<Product> Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductCategory = await _context.ProductCategory
                .Include(o => o.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ProductCategoryID == id);

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, ProductCategory, Operations.Detail);
            if(!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            

            if (ProductCategory == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
