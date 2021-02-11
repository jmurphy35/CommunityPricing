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

namespace CommunityPricing.Pages.Admin.OfferingPages
{
    public class OfferingModel : VendorSelectList
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public OfferingModel(CommunityPricing.Data.CommunityPricingContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
        }
        public string NameSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public int PageIndex { get; set; }
        public string Message { get; set; }
        public PaginatedList<Offering> Offering { get; set; }

        public async Task<IActionResult> OnGetAsync(string message, string sortOrder, string currentFilter,
             string searchString, int? pageIndex)
        {
            Message = message;
            CurrentSort = sortOrder;
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            CurrentFilter = searchString;

            MakeVendorSelectList(_context);

            IQueryable<Offering> offeringIQ = from o in _context.Offering
                                              .Include(p => p.Product)
                                              .Include(v => v.Vendor)
                                              select o;
 
            if(searchString == "all" || searchString == null)
            {

            }
            else
            {
                offeringIQ = offeringIQ.Where(o => o.Vendor.VendorID.ToString() == searchString);
            }


            switch (sortOrder)
            {
                case "name_desc":
                    offeringIQ = offeringIQ.OrderByDescending(o => o.Product.ProductName)
                        .ThenByDescending(o => o.Product.Wholesaler);
                    break;
                default:
                    offeringIQ = offeringIQ.OrderBy(o => o.Product.ProductName)
                        .ThenByDescending(o => o.Product.Wholesaler);
                    break;
            }

            int pageSize = 9;

            Offering = await PaginatedList<Offering>.CreateAsync(offeringIQ, pageIndex ?? 1, pageSize);
            PageIndex = Offering.PageIndex;




            Offering offering = new Offering();
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, offering, Operations.Read);

            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string CurrentSort, string CurrentFilter,
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
                    "offering",
                    o => o.ProductPricePerWeight, o => o.AsOfDate))
                {

                }

            }
            try
            {
                await _context.SaveChangesAsync();
                await ArchiveOffering.Archive(_context);

                Message = null;
                return RedirectToPage("./Offering", new
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
                setDbErrorMessage(dbValues, clientValues, _context);

                ModelState.Remove("RowVersion");
                string message = "The record you attempted to edit "
                                + "was modified by another user after you. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. TO EDIT THIS RECORD, RE-ENTER YOUR VALUES."
                                + "Otherwise the other user would not be aware that you had overwritten those entries";

                return RedirectToPage("./Offering", new
                {
                    message,
                    sortOrder = CurrentSort,
                    currentFilter = CurrentFilter,
                    pageIndex = PageIndex
                });
            }

        }

        private void setDbErrorMessage(Offering dbValues, Offering clientValues, CommunityPricingContext _context)
        {
            ModelState.AddModelError(string.Empty,
       "The record you attempted to edit "
     + "was modified by another user after you. The "
     + "edit operation was canceled and the current values in the database "
     + "have been displayed. If you still want to edit this record, click "
     + "the Save button again.");

        }

        private async Task ArchiveHelper(CommunityPricingContext _context)
        {
            List<Offering> Offerings = await _context.Offering.ToListAsync();
            List<ArchivedOffering> ArchivedOfferings = await _context.ArchivedOffering.ToListAsync();

        }
    }
}
