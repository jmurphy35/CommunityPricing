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


namespace CommunityPricing.Pages.Admin.VendorPages
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
            return Page();
        }

        [BindProperty]
        public Vendor Vendor { get; set; }
        public string Message { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
//**********************************Authorization***********************************************************
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Vendor, Operations.Create);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
//***********************************************************************************************************

            var vendorToUpdate = new Vendor();

            if(await TryUpdateModelAsync<Vendor>(
                vendorToUpdate,
                "vendor",
                v => v.VendorID, v => v.VendorName, v => v.OwnerID, v => v.VendorAddress1,
                v => v.VendorAddress2))
            {
                if (!_context.Vendor.Any(v => v.VendorID == vendorToUpdate.VendorID))
                {
                    try
                    {
                        //_context.Vendor.Add(vendorToUpdate);
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
                    Message = "The Vendor ID that you have chosen has already" +
                            " been taken. Please choose a different one.";
                    RedirectToPage("./Create", new { Message });
                }
            }

            return null;
        }
    }
}