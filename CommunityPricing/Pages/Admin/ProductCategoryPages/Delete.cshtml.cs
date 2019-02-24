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
    public class DeleteModel : DI_BasePageModel
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public DeleteModel(CommunityPricing.Data.CommunityPricingContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base (context, authorizationService, userManager)
        {
            _context = context;
        }

        [BindProperty]
        public ProductCategory ProductCategory { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductCategory = await _context.ProductCategory
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ProductCategoryID == id);

            if (ProductCategory == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ErrorMessage = "Delete failed. Try again";
            }


            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //********************Authorization***********************************************************************
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, ProductCategory, Operations.Delete);

            if(!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            //********************************************************************************************************

            ProductCategory = await _context.ProductCategory
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ProductCategoryID == id);

            if(ProductCategory == null)
            {
                HandleDeletedProductCategory();
            }

            try
            {
                _context.ProductCategory.Remove(ProductCategory);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException)
            {
                return RedirectToAction("./Delete",
                    new { id, saveChangesError = true });
            }
        }

        private IActionResult HandleDeletedProductCategory()
        {
            ModelState.AddModelError(string.Empty, "The ProductCategory that you are trying to update has been" +
                "deleted by another user.");
            return Page();
        }

    }
}
