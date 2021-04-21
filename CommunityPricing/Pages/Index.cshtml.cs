using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using CommunityPricing.Pages.Shared;
using CommunityPricing.Data;
using CommunityPricing.Pages.GeneralPublic;
using Microsoft.AspNetCore.Identity;
using CommunityPricing.Areas.Data;
using CommunityPricing.Models;

namespace CommunityPricing.Pages
{
    [AllowAnonymous]
    public class IndexModel : ListHelper
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public IndexModel(CommunityPricing.Data.CommunityPricingContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
        }
        public List<ProductCategoryDisplayModel> ProductCategoryDMListFood { get; set; }
        public List<ProductCategoryDisplayModel> ProductCategoryDMListNonFood { get; set; }
        public List<ProductCategory> ProductCategory { get; set; }

        public async Task OnGetAsync()
        {
            int startRange = 1;
            int endRange = 5000;

            ProductCategoryDMListFood = new List<ProductCategoryDisplayModel>();
            ProductCategory = await MakeRevisedCategoryList(startRange, endRange);
            foreach (var pc in ProductCategory)
            {
                ProductCategoryDisplayModel pcDm = new ProductCategoryDisplayModel();
                pcDm.Number = pc.ProductCategoryID;
                pcDm.Name = pc.Name;
                ProductCategoryDMListFood.Add(pcDm);
            }
            ProductCategoryDMListFood = ProductCategoryDMListFood.OrderBy(pc => pc.Name).ToList();


            ProductCategoryDMListNonFood = new List<ProductCategoryDisplayModel>();
            IQueryable<ProductCategory> RemainingCategories = from pc in _context.ProductCategory
                                                              where pc.ProductCategoryID <= endRange
                                                              select pc;
            ProductCategory = RemainingCategories.ToList();
            foreach (var item in ProductCategory)
            {
                ProductCategoryDisplayModel pcDm = new ProductCategoryDisplayModel();
                pcDm.Number = item.ProductCategoryID;
                pcDm.Name = item.Name;
                ProductCategoryDMListNonFood.Add(pcDm);
            }
            ProductCategoryDMListNonFood = ProductCategoryDMListNonFood.OrderBy(pc => pc.Name).ToList();
        }
    }
}
