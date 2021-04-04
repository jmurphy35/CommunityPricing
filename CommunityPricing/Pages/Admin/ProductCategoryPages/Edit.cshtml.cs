using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class EditModel : DI_BasePageModel
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public EditModel(CommunityPricing.Data.CommunityPricingContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base (context, authorizationService, userManager)
        {
            _context = context;
        }

        [BindProperty]
        public ProductCategory ProductCategory { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductCategory = await _context.ProductCategory.FindAsync(id);

            if (ProductCategory == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //**********************Authorization*******************************************************************
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User,
                ProductCategory, Operations.Update);

            if(!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            //******************************************************************************************************

            var productCategoryToUpdate = await _context.ProductCategory
                .FirstOrDefaultAsync(pc => pc.ProductCategoryID == id);

            if(productCategoryToUpdate == null)
            {
                HandleDeletedProductCategory();
            }

            _context.Entry(productCategoryToUpdate).Property("RowVersion")
                .OriginalValue = ProductCategory.RowVersion;

            if(await TryUpdateModelAsync<ProductCategory>(
                productCategoryToUpdate,
                "productCategory",
                pc => pc.ProductCategoryID, pc => pc.Name))
            {
                try
                {
                    //await _context.SaveChangesAsync();
                    return RedirectToPage("./Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntries = ex.Entries.Single();
                    var clientValues = (ProductCategory)exceptionEntries.Entity;
                    var databaseEntry = exceptionEntries.GetDatabaseValues();
                    if(databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save. " +
                            "The department was deleted by another user.");
                        return Page();
                    }
                    var dbValues = (ProductCategory)databaseEntry.ToObject();
                    SetDbErrorMessage(dbValues, clientValues, _context);
                    ProductCategory.RowVersion = (byte[])dbValues.RowVersion;
                    ModelState.Remove("ProductCategory.RowVersion");
                }            
            }
            return Page();
        }  
        private IActionResult HandleDeletedProductCategory()
        {
            ModelState.AddModelError(string.Empty, "The Product Category that you" +
                " are trying to edit has been deleted by another user.");
            return Page();
        }
        private void SetDbErrorMessage(ProductCategory dbValues, ProductCategory clientValues,
            CommunityPricingContext _context)
        {
            if(dbValues.Name != clientValues.Name)
            {
                ModelState.AddModelError("ProductCategory.Name",
                    $"current value: {dbValues.Name}");
            }

            ModelState.AddModelError(string.Empty,
                "The record you attempted to edit "
              + "was modified by another user after you. The "
              + "edit operation was canceled and the current values in the database "
              + "have been displayed. If you still want to edit this record, click "
              + "the Save button again.");

        }
    }
}
