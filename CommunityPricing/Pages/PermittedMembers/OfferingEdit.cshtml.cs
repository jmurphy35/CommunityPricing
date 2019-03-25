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
    public class OfferingEditModel : DI_BasePageModel
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public OfferingEditModel(CommunityPricing.Data.CommunityPricingContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
        }

        public PaginatedList<Offering> Offering { get; set; }
        public int VendorId { get; set; }

        public string ProductTypeSort { get; set; }
        public string WholesalerSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public int PageIndex { get; set; }
        public string Message { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, string message, string sortOrder,
            string currentFilter, string searchString, int? pageIndex)
        {
            //*************************************Authorization******************************************

            Vendor Vendor = await _context.Vendor.FirstOrDefaultAsync(v => v.VendorID == id);
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Vendor, Operations.Read);
            if (!isAuthorized.Succeeded)
            {
                var userID = UserManager.GetUserId(User);
                if (userID != null)
                {
                    if (userID != Vendor.OwnerID)
                    {
                        return new ChallengeResult();
                    }
                }
                else
                {
                    return new ChallengeResult();
                }
            }
            //********************************************************************************************

            Message = message;
            VendorId = id;
            CurrentSort = sortOrder;
            ProductTypeSort = String.IsNullOrEmpty(sortOrder) ? "prodtype_desc" : "";
            WholesalerSort = sortOrder == "Wholesaler" ? "wholesaler_desc" : "Wholesaler";

            if(searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            CurrentFilter = searchString;

            IQueryable<Offering> offeringIQ = from o in _context.Offering
                                              .Include(p => p.Product)
                                              .Include(v => v.Vendor)
                                              .Where(o => o.VendorID == id)
                                              select o;

            
            if (!String.IsNullOrEmpty(searchString))
            {
                offeringIQ = offeringIQ.Where(o => o.Product.ProductName.Contains(searchString));
            }
            
            switch (sortOrder)
            {
                case "prodtype_desc" :
                    offeringIQ = offeringIQ.OrderByDescending(o => o.Product.ProductDescr1)
                        .ThenBy(o => o.Product.Wholesaler);
                    break;
                case "Wholesaler" :
                    offeringIQ = offeringIQ.OrderBy(o => o.Product.Wholesaler)
                        .ThenBy(o => o.Product.ProductDescr1);
                    break;
                case "wholesaler_desc" :
                    offeringIQ = offeringIQ.OrderByDescending(o => o.Product.Wholesaler)
                        .ThenBy(o => o.Product.ProductDescr1);
                    break;
                default:
                    offeringIQ = offeringIQ.OrderBy(o => o.Product.ProductDescr1)
                        .ThenBy(o => o.Product.Wholesaler);
                    break;
            }

            int pageSize = 7;
            Offering = await PaginatedList<Offering>.CreateAsync(offeringIQ, pageIndex ?? 1, pageSize);
            PageIndex = Offering.PageIndex;
            return Page();
        }

        public async Task<IActionResult> OnPost(int VendorId, string CurrentSort, string CurrentFilter,
            int PageIndex, IEnumerable<Offering> Offering)
        {
            foreach (var offering in Offering)
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }
                var offeringToUpdate = await _context.Offering
                    .FirstOrDefaultAsync(o => o.OfferingID == offering.OfferingID);

                _context.Entry(offeringToUpdate).Property("RowVersion").OriginalValue = offering.RowVersion;
                

                offeringToUpdate.ProductPricePerWeight = offering.ProductPricePerWeight;
                offeringToUpdate.AsOfDate = offering.AsOfDate;

                if (await TryUpdateModelAsync<Offering>(
                    offeringToUpdate,
                    "offeringToUpdate",
                    o => o.ProductPricePerWeight, o => o.AsOfDate))
                {

                }

            }

            try
            {
                
                await _context.SaveChangesAsync();
                await ArchiveOffering.Archive(_context);
                Message = null;
                return RedirectToPage("./OfferingEdit", new
                {
                    sortOrder = CurrentSort,
                    currentFilter = CurrentFilter,
                    pageIndex = PageIndex
                });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionEntry = ex.Entries.Single();
                var clientValues = (Offering)exceptionEntry.Entity;
                var databaseEntry = exceptionEntry.GetDatabaseValues();

                var dbValues = (Offering)databaseEntry.ToObject();

                ModelState.Remove("RowVersion");
                string message = "The record you attempted to edit "
                                + "was modified by another user after you. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. TO EDIT THIS RECORD, RE-ENTER YOUR VALUES."
                                + "Otherwise the other user would not be aware that you had overwritten those entries";

                return RedirectToPage("./OfferingEdit", new
                {
                    id = VendorId,
                    message,
                    sortOrder = CurrentSort,
                    currentFilter = CurrentFilter,
                    pageIndex = PageIndex
                });
            }
            
        }
    }
}
