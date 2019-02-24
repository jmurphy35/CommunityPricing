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

namespace CommunityPricing.Pages.PermittedMembers
{
    public class VendorEditModel : DI_BasePageModel
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public VendorEditModel(CommunityPricing.Data.CommunityPricingContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
        }

        [BindProperty]
        public Vendor Vendor { get; set; }

        bool isVendorAuthorized = false;

        public async Task<IActionResult> OnGetAsync(int id)
        {            
            Vendor = await _context.Vendor.FindAsync(id);

            if (Vendor == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var vendorToUpdate = await _context.Vendor.FindAsync(id);

            if(await TryUpdateModelAsync<Vendor>(
                vendorToUpdate,
                "vendor",
                v => v.VendorName, v => v.VendorAddress1, v => v.VendorAddress2))
            {
                var isAdminAuthorized = await AuthorizationService.AuthorizeAsync(User,
                    Vendor, Operations.Update);
                
                isVendorAuthorized = VendorAuth(vendorToUpdate, isVendorAuthorized);
                if ((!isAdminAuthorized.Succeeded) && !(isVendorAuthorized == true))
                {
                    return new ChallengeResult();
                }
                await _context.SaveChangesAsync();
                return RedirectToPage("./PMGate");
            }
            else
            {
                return Page();
            } 
        }

        private bool VendorAuth(Vendor vendorToUpdate, bool isVendorAuthorized)
        {    
            var userId = UserManager.GetUserId(User);

            if (vendorToUpdate.OwnerID == userId)
            {
                isVendorAuthorized = true;   
            }
            else
            {
                isVendorAuthorized = false; 
            }
            return isVendorAuthorized;
        }
    }
}
