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
    public class ProductCategoryIndexModel : ListHelper
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public ProductCategoryIndexModel(CommunityPricing.Data.CommunityPricingContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
        }

        public PaginatedList<ProductCategory> ProductCategory { get; set; }

        public string NameSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }


        public async Task OnGetAsync(string sortOrder,
            string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            CurrentFilter = searchString;

            IQueryable<ProductCategory> ProductCategoryIQ = from pc in _context.ProductCategory
                                                            select pc;
            
            if (!String.IsNullOrEmpty(searchString))
            {
                ProductCategoryIQ = ProductCategoryIQ.Where(pc => pc.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    ProductCategoryIQ = ProductCategoryIQ.OrderByDescending(pc => pc.Name);
                    break;

                default:
                    ProductCategoryIQ = ProductCategoryIQ.OrderBy(pc => pc.Name);
                    break;
            }
           
            int endRange = 7000;


            IQueryable<ProductCategory> PrimeListIQ = from pc in _context.ProductCategory
                                                        where pc.ProductCategoryID >= endRange
                                                        select pc;
            IQueryable<ProductCategory> ProductCategoryRemainderIQ = from pc in _context.ProductCategory
                                                                     where pc.ProductCategoryID < endRange
                                                                     select pc;
            
            int pageSize = 20;

            ProductCategory = await PaginatedList<ProductCategory>.CreateFromManyAsync(PrimeListIQ,
                ProductCategoryRemainderIQ, pageIndex ?? 1, pageSize);
           
        }
        

    }
}
