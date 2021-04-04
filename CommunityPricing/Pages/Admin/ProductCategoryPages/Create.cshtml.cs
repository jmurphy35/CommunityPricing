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

namespace CommunityPricing.Pages.Admin.ProductCategoryPages
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

        public IActionResult OnGet(string message)
        {
            Message = message;
            return Page();
        }

        [BindProperty]
        public ProductCategory ProductCategory { get; set; }
        public string Message { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //************Authorization****************************************************
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User,
                ProductCategory, Operations.Create);
            
            if(!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            //*****************************************************************************

            var productCategoryToUpdate = new ProductCategory();

            if (await TryUpdateModelAsync<ProductCategory>(
                productCategoryToUpdate,
                "productcategory",
                pc => pc.ProductCategoryID, pc => pc.Name))
            {
                if(!_context.ProductCategory.Any(pc => pc.ProductCategoryID == productCategoryToUpdate.ProductCategoryID))
                {
                    try
                    {
                        //_context.ProductCategory.Add(productCategoryToUpdate);
                        //await _context.SaveChangesAsync();
                        Message = null;
                        return RedirectToPage("./Index");
                    }
                    catch (Exception ex)
                    {
                        RedirectToPage("./Create", new { ex.Message });       
                    }
                }
                else
                {
                    Message = "The Product Category ID that you have chosen has already" +
                            " been taken. Please choose a different one.";
                    RedirectToPage("./Create", new { Message });
                }
               
            }

            return null;
        }
    }
}