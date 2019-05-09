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

namespace CommunityPricing.Pages.PermittedMembers
{
    public class PMGateModel : DI_BasePageModel
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public PMGateModel(CommunityPricing.Data.CommunityPricingContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
        }

        public Vendor Vendor { get; set; }     
        public string Message { get; set; }
        public int? VendorId { get; set; }
        public bool authVendor { get; set; }


        public async Task<IActionResult> OnGet(int? id)
        {
            authVendor = false;
                   
            var userId = UserManager.GetUserId(User);
            
            List<Vendor> vendorToMatchList = new List<Vendor>();
            foreach (var vendor in _context.Vendor)
            {
                if (vendor.OwnerID == userId)
                    vendorToMatchList.Add(vendor);
            }
            
            if(id != null)
            {
                Vendor Vendor = await _context.Vendor.FirstOrDefaultAsync(v => v.VendorID == id);
                if (Vendor != null)
                {
                    var isAdminAuthorized = await AuthorizationService.AuthorizeAsync(User, Vendor, Operations.Read);

                    if(isAdminAuthorized.Succeeded)
                    {
                        VendorId = id;
                        authVendor = true;
                    }
                    else
                    {
                        if (vendorToMatchList != null)
                        {
                            if (vendorToMatchList.Any(v => v.VendorID == id))
                            {
                                VendorId = id;
                                authVendor = true;
                            }
                            else
                            {
                                Message = "The person logged in is not authorized to access this vendor." +
                                    " Re-enter your Vendor Id. ";
                                return Page();
                            }
                        }
                        else
                        {
                            Message = "No vendor exists that matches the person who is signed into this site.";
                            return Page();
                        }
                    }           
                }
                else
                {
                    Message = "No vendor id matches the value that you put in the box";
                    return Page();
                }
            }

            else
            {
                Message = "Enter your vendor number in the little box below.";
            }
            
            return Page();

            
        }      
    }
}
