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

        public PaginatedList<ProductCategory> ProductCategory { get;set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter,
            string searchString, int? pageIndex)
        {
            //*********************PageFilterSort**************************************************************

            CurrentSort = sortOrder;
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            if(searchString != null)
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
                case "name_desc" :
                    ProductCategoryIQ = ProductCategoryIQ.OrderByDescending(pc => pc.Name);
                    break;

                default:
                    ProductCategoryIQ = ProductCategoryIQ.OrderBy(pc => pc.Name);
                    break;
            }

            int pageSize = 9;

            ProductCategory = await PaginatedList<ProductCategory>.CreateAsync(ProductCategoryIQ,
                pageIndex ?? 1, pageSize);
            //*************************************************************************************************

            //*********************Authorization******************************************************************

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, ProductCategory, Operations.Read);

            if(!isAuthorized.Succeeded)
            {
                new ChallengeResult();
                //return new ChallengeResult(); when i change the Task to Task<IActionResult>
            }

            //**************************************************************************************************
        }
    }
}
