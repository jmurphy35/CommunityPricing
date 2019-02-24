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

namespace CommunityPricing.Pages.GeneralPublic
{
    [AllowAnonymous]
    public class ProductDetailModel : DI_BasePageModel
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public ProductDetailModel(CommunityPricing.Data.CommunityPricingContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
        }
        public string PriceSort { get; set; }
        public string CurrentSort { get; set; }
        public Product Product { get; set; }
        public Vendor Vendor { get; set; }
        public List<Offering> Offerings { get; set; }


        public async Task<IActionResult> OnGetAsync(Guid? productId, string sortOrder)
        {
            PriceSort = String.IsNullOrEmpty(sortOrder) ? "price_desc" : "";
            IQueryable<Offering> offeringIQ = from o in _context.Offering.Where(o => o.ProductID == productId).Include(v => v.Vendor)
                                              select o;
                                             
                                            
            switch (sortOrder)
            {
                case "price_desc":
                    offeringIQ = offeringIQ.OrderByDescending(o => o.ProductPricePerWeight);
                    break;
                default:
                    offeringIQ = offeringIQ.OrderBy(o => o.ProductPricePerWeight);
                    break;
            }
            Offerings = await offeringIQ.AsNoTracking().ToListAsync();

            if (productId == null)
            {
                return NotFound();
            }

            Product = await _context.Product
                .Include(p => p.ProductCategory)
                .Include(o => o.Offering)
                .ThenInclude(v => v.Vendor)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ProductID == productId);

            

            if (Product == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
