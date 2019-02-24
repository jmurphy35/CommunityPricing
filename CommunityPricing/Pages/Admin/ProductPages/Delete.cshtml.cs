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
    public class DeleteModel : DI_BasePageModel
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public DeleteModel(CommunityPricing.Data.CommunityPricingContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; }
        public string ErrorMessage { get; set; }


        public async Task<IActionResult> OnGetAsync(Guid? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product = await _context.Product
                .Include(p => p.ProductCategory)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ProductID == id);

            if (Product == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ErrorMessage = "Delete failed. Try again";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //*********************Authorization*************************************************************
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Product, Operations.Delete);

            if(!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            //***********************************************************************************************
            Product = await _context.Product
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (Product == null)
            {
                return HandleDeletedProduct();
            }
            try
            {
                _context.Product.Remove(Product);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException)
            {
                return RedirectToAction("./Delete",
                             new { id, saveChangesError = true });
            }
            
        }

        private IActionResult HandleDeletedProduct()
        {
            ModelState.AddModelError(string.Empty, "The Product that you are trying to update has been" +
                "deleted by another user.");
            return Page();
        }
    }
}
