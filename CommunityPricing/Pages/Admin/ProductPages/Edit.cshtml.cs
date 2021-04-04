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

namespace CommunityPricing.Pages.Admin.ProductPages
{
    public class EditModel : DI_BasePageModel
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public EditModel(CommunityPricing.Data.CommunityPricingContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; }

        public SelectList ProductCategorySL { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product = await _context.Product.FindAsync(id);

            //***************************Authorization***********************************************************
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Product, Operations.Update);

            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            //***************************************************************************************************

            if (Product == null)
            {
                return NotFound();
            }
           ProductCategorySL = new SelectList(_context.ProductCategory, "ProductCategoryID", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id, string submitProduct)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var productToUpdate = await _context.Product.FindAsync(id);

            if(productToUpdate == null)
            {
                HandleDeletedProduct();
            }

            _context.Entry(productToUpdate).Property("RowVersion").OriginalValue = Product.RowVersion;

            if(await TryUpdateModelAsync<Product>(
                productToUpdate,
                "product",
                p => p.ProductName, p => p.ProductDescr1,
                p => p.ProductDescr2_Wt_Vol, p => p.Wholesaler, p => p.ProductCategoryID))
            {
                try
                {
                    //await _context.SaveChangesAsync();
                    if (submitProduct == "SaveAndEditVendors")
                    {
                        return RedirectToPage("./AddVendorsToProduct", id);
                    }
                    else
                    {
                        return RedirectToPage("./Index");
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Product)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if(databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save. " +
                                                    "The product was deleted by another user.");
                        return Page();
                    }

                    var dbValues = (Product)databaseEntry.ToObject();
                    await SetDbErrorMessage(dbValues, clientValues, _context);
                    Product.RowVersion = (byte[])dbValues.RowVersion;
                    ModelState.Remove("Product.RowVersion");
                }
                ProductCategorySL = new SelectList(Context.ProductCategory,
              "ProductCategoryID", "Name", productToUpdate.ProductCategoryID);
            }
            return Page();       
        }
        private IActionResult HandleDeletedProduct()
        {
            ModelState.AddModelError(string.Empty, "The Product that you are trying" +
                "to edit has been deleted by another user.");
            return Page();
        }

        private async Task SetDbErrorMessage(Product dbValues, Product clientValues,
            CommunityPricingContext _context)
        {
            if(dbValues.ProductName != clientValues.ProductName)
            {
                ModelState.AddModelError("Product.ProductName",
                    $"Current value: {dbValues.ProductName}");
            }
            if (dbValues.ProductDescr1 != clientValues.ProductDescr1)
            {
                ModelState.AddModelError("Product.ProductDescr1",
                    $"Current value: {dbValues.ProductDescr1}");
            }
            if (dbValues.ProductDescr2_Wt_Vol != clientValues.ProductDescr2_Wt_Vol)
            {
                ModelState.AddModelError("Product.ProductDescr2_Wt_Vol",
                    $"Current value: {dbValues.ProductDescr2_Wt_Vol}");
            }
            if (dbValues.Wholesaler != clientValues.Wholesaler)
            {
                ModelState.AddModelError("Product.Wholesaler",
                    $"Current value: {dbValues.Wholesaler}");
            }
            if (dbValues.ProductCategoryID != clientValues.ProductCategoryID)
            {
                ProductCategory dbProductCategory = await _context.ProductCategory.FindAsync(dbValues.ProductCategoryID);

                ModelState.AddModelError("Product.ProductCategoryID", $"Current value: {dbProductCategory.Name}");
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
