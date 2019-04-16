using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CommunityPricing.Data;
using CommunityPricing.Models;
using CommunityPricing.Areas.Models.HelperModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CommunityPricing.Areas.Data;
using CommunityPricing.Areas.Authorization;
using CommunityPricing.Pages.Shared;

namespace CommunityPricing.Pages.GeneralPublic
{
    [AllowAnonymous]
    public class ProductDetailModel : DI_BasePageModel
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public ProductDetailModel(CommunityPricing.Data.CommunityPricingContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
        }
        public string PriceSort { get; set; }
        public string AverageSort { get; set; }
        public string CurrentSort { get; set; }
        public Product Product { get; set; }
        public Vendor Vendor { get; set; }
        public List<Offering> Offerings { get; set; }
        public ArchiveOffering archOff { get; set; }
        public List<DisplayDetailHelper> DisplayDetailHelper{ get; set; }
        public string notAvailable { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? productId, string sortOrder)
        {
            CurrentSort = sortOrder;
            PriceSort = String.IsNullOrEmpty(sortOrder) ? "price_desc" : "";
            AverageSort = sortOrder == "Average" ? "average_desc" : "Average";

            IQueryable<Offering> offeringIQ = from o in _context.Offering.Where(o => o.ProductID == productId).Include(v => v.Vendor)
                                              select o;

            Offerings = await offeringIQ.AsNoTracking().ToListAsync();
            DisplayDetailHelper = MapToHelper(Offerings);

            switch (sortOrder)
            {
                case "price_desc":
                    DisplayDetailHelper = DisplayDetailHelper.OrderByDescending(d => d.PricePerUnit).ToList();
                    break;

                case "Average":
                    DisplayDetailHelper = DisplayDetailHelper.OrderBy(d => d.Average).ToList();
                    break;

                case "average_desc":
                    DisplayDetailHelper = DisplayDetailHelper.OrderByDescending(d => d.Average).ToList();
                    break;

                default:
                    DisplayDetailHelper = DisplayDetailHelper.OrderBy(d => d.PricePerUnit).ToList();
                    break;
            }
            

            if (productId == null)
            {
                return NotFound();
            }

            Product = await _context.Product
                .Include(p => p.ProductCategory)
                .Include(o => o.Offering)
                .ThenInclude(v => v.Vendor)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ProductID == productId);

            archOff = new ArchiveOffering(_context);



            if (Product == null)
            {
                return NotFound();
            }
            return Page();
        }


        private List<DisplayDetailHelper> MapToHelper(List<Offering> offeringIQ)
        {
            DisplayDetailHelper = new List<DisplayDetailHelper>();
            foreach (var offering in offeringIQ)
            {
                DisplayDetailHelper listItem = new DisplayDetailHelper();
                listItem.VendorName = offering.Vendor.VendorName;
                listItem.VendorAddress = offering.Vendor.VendorAddress1;
                listItem.VendorAddress2 = offering.Vendor.VendorAddress2;
                listItem.PricePerUnit = offering.ProductPricePerWeight;

                ArchiveOffering arcOf = new ArchiveOffering(_context);
                List<decimal> num = arcOf.ArchivedPrices(offering.OfferingID);
                var average = Averager.FindAverage(num.Count, num);
                listItem.Average = average;
                listItem.asOfDate = offering.AsOfDate;
                 
                DisplayDetailHelper.Add(listItem);
            }
            return DisplayDetailHelper;
        }
    }
}
