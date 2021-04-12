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
using CommunityPricing.Areas.Models.HelperModels;

namespace CommunityPricing.Pages.Admin.ProductPages
{
    public class AddVendorsToProductModel : ListHelper
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public AddVendorsToProductModel(CommunityPricing.Data.CommunityPricingContext context,
            IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager )
            : base(context, authorizationService, userManager)
        {
            _context = context;
        }

        public Guid ProductID { get; set; }
        public PaginatedList<Vendor> Vendor { get;set; }
        
        public string NameSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public int PageIndex { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id, string sortOrder, string currentFilter,
            string searchString, int? pageIndex)
        {
            Vendor vendorForAuth = new Vendor();
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, vendorForAuth, Operations.Read);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            ProductID = id;
            CurrentSort = sortOrder;
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            if(searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            CurrentFilter = searchString;

            IQueryable<Vendor> VendorIQ = from v in _context.Vendor
                                          select v;
            if (!String.IsNullOrEmpty(searchString))
            {
                VendorIQ = VendorIQ.Where(v => v.VendorName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    VendorIQ = VendorIQ.OrderByDescending(v => v.VendorName);
                    break;
                default:
                    VendorIQ = VendorIQ.OrderBy(v => v.VendorName);
                    break;
            }
            int pageSize = 12;
            Vendor = await PaginatedList<Vendor>.CreateAsync(VendorIQ, pageIndex ?? 1, pageSize);
            PageIndex = Vendor.PageIndex;
            MakeDesignatedVendorList(Vendor, id);

            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string[] sortedVendors, string[] selectedVendors, Guid productID,
            string CurrentSort, string CurrentFilter, int PageIndex, string AddVendor)
        {
            var productToUpdate = await _context.Product
                .FindAsync(productID);

            if (await TryUpdateModelAsync<Product>(
                productToUpdate,
                "productToUpdate"))
            {
                UpdateOfferings(productID, sortedVendors, selectedVendors);
                await _context.SaveChangesAsync();
            }
            else
            {
                MakeDesignatedVendorList(Vendor, productID);  
            }   

            if(AddVendor == "SaveAndAdd")
            {
                return RedirectToPage("./AddNewVendor",
                new { id = productID, currentSort = CurrentSort, currentFilter = CurrentFilter, pageIndex = PageIndex });
            }
            else
            {
                return RedirectToPage("./AddVendorsToProduct",
                new { id = productID, sortOrder = CurrentSort, currentFilter = CurrentFilter, pageIndex = PageIndex });
            }        
        }
    }
}
