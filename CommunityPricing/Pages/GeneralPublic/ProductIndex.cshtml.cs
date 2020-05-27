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
    public class ProductIndexModel : DI_BasePageModel
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public ProductIndexModel(CommunityPricing.Data.CommunityPricingContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
        }

        public PaginatedList<Product> Product { get;set; }

        public string NameSort { get; set; }
        public string WholesalerSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public async Task OnGetAsync(int productCategoryId, string sortOrder, string currentFilter,
            string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            WholesalerSort = sortOrder == "Wholesaler" ? "wholesaler_desc" : "Wholesaler";
            if(searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            CurrentFilter = searchString;

            IQueryable<Product> ProductIQ = from p in _context.Product
                                            .Where(pr => pr.ProductCategoryID == productCategoryId)
                                            .Include(pc => pc.ProductCategory)
                                            select p;
            if(!String.IsNullOrEmpty(searchString))
            {
                ProductIQ = ProductIQ.Where(p => p.ProductName.Contains(searchString));
            }
            
            switch (sortOrder)
            {
                case "name_desc" :
                    ProductIQ = ProductIQ.OrderByDescending(p => p.ProductName)
                        .ThenBy(p => p.ProductDescr1);
                    break;

                case "Wholesaler" :
                    ProductIQ = ProductIQ.OrderBy(p => p.Wholesaler)
                        .ThenBy(p => p.ProductName).ThenBy(p => p.ProductDescr1);
                    break;

                case "wholesaler_desc" :
                    ProductIQ = ProductIQ.OrderByDescending(p => p.Wholesaler)
                        .ThenBy(p => p.ProductName).ThenBy(p => p.ProductDescr1);
                    break;

                default:
                    ProductIQ = ProductIQ.OrderBy(p => p.ProductName)
                        .ThenBy(p => p.ProductDescr1);
                    break;
            }
            int pageSize = 7;
            Product = await PaginatedList<Product>.CreateAsync(ProductIQ.AsNoTracking(), pageIndex ?? 1,
                pageSize); 
        }
    }
}
