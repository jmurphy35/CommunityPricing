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
using Microsoft.EntityFrameworkCore;

namespace CommunityPricing.Pages.Admin.ProductPages
{
    public class AddNewVendorModel : ListHelper
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public AddNewVendorModel(CommunityPricing.Data.CommunityPricingContext context,
            IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
        }

        public IActionResult OnGet(Guid id, string currentSort, string currentFilter, int pageIndex, string message)
        {
            ProductId = id;
            CurrentSort = currentSort;
            CurrentFilter = currentFilter;
            PageIndex = pageIndex;
            Message = message;

            return Page();
        }

        [BindProperty]
        public Vendor Vendor { get; set; }

        public Guid ProductId { get; set; }
        public string Message { get; set; }

        public string CurrentSort { get; set; }
        public string CurrentFilter { get; set; }
        public int PageIndex { get; set; }

        public async Task<IActionResult> OnPostAsync(Guid ProductId, string CurrentSort, string CurrentFilter,
            int PageIndex, string includeVendor)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Vendor, Operations.Create);
            if(!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            var vendorToAdd = new Vendor();
            if(await TryUpdateModelAsync<Vendor>(
                vendorToAdd,
                "vendor",
                v => v.VendorID, v => v.VendorName, v => v.VendorAddress1, v => v.VendorAddress2,
                v => v.OwnerID))
            {
                if (!_context.Vendor.Any(v => v.VendorID == vendorToAdd.VendorID))
                {
                    Message = null;
                    try
                    {
                        _context.Vendor.Add(vendorToAdd);

                        if (includeVendor == "selected")
                        {
                            var productToUpdate = await _context.Product
                                .Include(p => p.Offering)
                                .FirstOrDefaultAsync(p => p.ProductID == ProductId);

                            productToUpdate.Offering.Add(new Offering()
                            {
                                ProductID = this.ProductId,
                                VendorID = Vendor.VendorID
                            });      
                        }
                        else { }
                        //await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        RedirectToPage("./AddNewVendor", new {
                            id = ProductId,
                            currentSort = CurrentSort,
                            currentFilter = CurrentFilter,
                            pageIndex = PageIndex,
                            message = ex.Message
                        });
                    }
                }
                else
                {
                    Message = "The Product Category ID that you have chosen has already" +
                            " been taken. Please choose a different one.";
                    return RedirectToPage("./AddNewVendor", new {
                        id = ProductId,
                        sortOrder = CurrentSort,
                        currentFilter = CurrentFilter,
                        pageIndex = PageIndex,
                        message = Message });
                }     
            }
            
            return RedirectToPage("./AddVendorsToProduct", new
            {
                id = ProductId,
                sortOrder = CurrentSort,
                currentFilter = CurrentFilter,
                pageIndex = PageIndex
            });
        }
    }
}