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

        public List<ProductCategoryDisplayModel> ProductCategoryDMList { get; set; }
        public void OnGet()
        {
            IQueryable<ProductCategory> ProductCategoryIQ = from pc in _context.ProductCategory
                                                            select pc;
            ProductCategoryDMList = new List<ProductCategoryDisplayModel>();

            foreach (var pc in ProductCategoryIQ)
            {
                ProductCategoryDisplayModel pcDm = new ProductCategoryDisplayModel();
                pcDm.Number = pc.ProductCategoryID;
                pcDm.Name = pc.Name;
                ProductCategoryDMList.Add(pcDm);
            }
        }
    }
}
