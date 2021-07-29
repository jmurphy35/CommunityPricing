using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CommunityPricing.Data;
using CommunityPricing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CommunityPricing.Areas.Data;
using CommunityPricing.Areas.Authorization;
using CommunityPricing.Pages.Shared;

namespace CommunityPricing.Pages.Admin.ProductPages
{
    public class CreateModel : DI_BasePageModel
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public CreateModel(CommunityPricing.Data.CommunityPricingContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            Message = Message;
            ViewData["ProductCategoryID"] = new SelectList(_context.ProductCategory,
                "ProductCategoryID", "Name");
            return Page();
        }

        [BindProperty]
        public Product Product { get; set; }
        public string Message { get; set; }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //***********************Authorization************************************************************

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Product, Operations.Create);

            if(!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            //************************************************************************************************

            var productToUpdate = new Product();

            if (await TryUpdateModelAsync<Product>(
                productToUpdate,
                "product",
                p => p.ProductName, p => p.ProductDescr1,
                p => p.ProductDescr2_Wt_Vol, p => p.Wholesaler, p => p.ProductCategoryID))
            {
                Guid guid = Guid.NewGuid();
                productToUpdate.ProductID = guid;
                _context.Product.Add(productToUpdate);
                
                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }
            return null;           
        }
    }
}