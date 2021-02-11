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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CommunityPricing.Pages.Shared
{
    public class VendorSelectList : DI_BasePageModel
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public VendorSelectList(CommunityPricing.Data.CommunityPricingContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
        }
        public SelectList VendorSL { get; set; }

        public SelectList MakeVendorSelectList(CommunityPricingContext _context, object selectedVendor = null)
        {
            var vendors = from v in _context.Vendor
                         orderby v.VendorName
                         select v;

            List<Vendor> modifiedVendors = new List<Vendor>();
            foreach (var vendor in vendors)
            {
                vendor.VendorName = vendor.VendorName + " - " + vendor.VendorAddress1 + ", " + vendor.VendorAddress2;
                modifiedVendors.Add(vendor);
            }


            VendorSL = new SelectList(modifiedVendors, "VendorID", "VendorName", selectedVendor);
                       
            return VendorSL;
        }
    }
}
