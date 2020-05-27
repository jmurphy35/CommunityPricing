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

namespace CommunityPricing.Pages.Admin.ProductPages
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

        public PaginatedList<Product> Product { get;set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter,
            string searchString, int? pageIndex)
        {
//********************SortFilterPage*************************************************************************



            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            CurrentSort = sortOrder;
            if(searchString != null )
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            CurrentFilter = searchString;

            IQueryable<Product> ProductIQ = from p in _context.Product
                            select p;
            if(!String.IsNullOrEmpty(searchString))
            {
                ProductIQ = ProductIQ.Where(p => p.ProductName.Contains(searchString));
            }
            
            switch (sortOrder)
            {
                case "name_desc" :
                    ProductIQ = ProductIQ.OrderByDescending(p => p.ProductName)
                        .ThenByDescending(p => p.Wholesaler).ThenByDescending(p => p.ProductDescr1);
                    break;
                default :
                    ProductIQ = ProductIQ.OrderBy(p => p.ProductName)
                        .ThenByDescending(p => p.Wholesaler).ThenByDescending(p => p.ProductDescr1);
                    break;  
            }

            int pageSize = 9;
            Product = await PaginatedList<Product>.CreateAsync(ProductIQ, pageIndex ?? 1, pageSize);
//************************************************************************************************************
            


//***********************Authorization***********************************************************************

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Product, Operations.Read);

            if(!isAuthorized.Succeeded)
            {
                new ChallengeResult();
            }
 //**********************************************************************************************************
 
        }
    }
}
