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

namespace CommunityPricing.Pages.Admin.VendorPages
{
    public class EditModel : DI_BasePageModel
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public EditModel(CommunityPricing.Data.CommunityPricingContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base( context, authorizationService, userManager)
        {
            _context = context;
        }

        [BindProperty]
        public Vendor Vendor { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Vendor = await _context.Vendor.FindAsync(id);

            if (Vendor == null)
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

            //****************Authorization*****************************************************************
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Vendor, Operations.Update);

            if(!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            //**********************************************************************************************

            var vendorToUpdate = await _context.Vendor
                .FirstOrDefaultAsync(v => v.VendorID == id);

            if(vendorToUpdate == null)
            {
                return HandleDeletedVendor();      
            }

            _context.Entry(vendorToUpdate).Property("RowVersion").OriginalValue = Vendor.RowVersion;

            if(await TryUpdateModelAsync<Vendor>(
                vendorToUpdate,
                "vendor",
                v => v.VendorID, v => v.VendorName, v => v.OwnerID, v => v.VendorAddress1,
                v => v.VendorAddress2))
            {
                try
                {
                    //await _context.SaveChangesAsync();
                    return RedirectToPage("./Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Vendor)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save. " +
                            "The department was deleted by another user.");
                        return Page();
                    }
                    var dbValues = (Vendor)databaseEntry.ToObject();
                    setDbErrorMessage(dbValues, clientValues, _context);     
                    Vendor.RowVersion = (byte[])dbValues.RowVersion;
                    ModelState.Remove("Vendor.RowVersion");
                }
            }
            return Page();   
        }
        private IActionResult HandleDeletedVendor()
        {
            ModelState.AddModelError(string.Empty, "The Vendor that you are trying to update has been" +
                "deleted by another user.");
            return Page();
        }

        private void setDbErrorMessage(Vendor dbValues, Vendor clientValues, CommunityPricingContext _context)
        {
            if (dbValues.OwnerID != clientValues.OwnerID)
            {
                ModelState.AddModelError("Vendor.OwnerID",
                    $"Current value: {dbValues.OwnerID}");
            }
            if (dbValues.VendorName != clientValues.VendorName)
            {
                ModelState.AddModelError("Vendor.VendorName",
                    $"Current value: {dbValues.VendorName}");
            }
            if (dbValues.VendorAddress1 != clientValues.VendorAddress1)
            {
                ModelState.AddModelError("Vendor.VendorAddress1",
                    $"Current value: {dbValues.VendorAddress1}");
            }
            if (dbValues.VendorAddress2 != clientValues.VendorAddress2)
            {
                ModelState.AddModelError("Vendor.VendorAddress2",
                    $"Current value: {dbValues.VendorAddress2}");
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
