using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CommunityPricing.Areas.Data;
using CommunityPricing.Areas.Authorization;
using CommunityPricing.Pages.Shared;
using CommunityPricing.Models;
using CommunityPricing.Areas.Models.HelperModels;
using Microsoft.AspNetCore.Mvc;

namespace CommunityPricing.Pages.Shared
{
    public class ListHelper : DI_BasePageModel
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public ListHelper(CommunityPricing.Data.CommunityPricingContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
        }
        
        public Offering Offering { get; set; }
        public List<Offering> Offerings { get; set; }
        public List<Vendor> Vendors { get; set; }
        public List<DesignatedVendor> DesignatedVendorList { get; set; }

        public List<DesignatedVendor> MakeDesignatedVendorList(List<Vendor> vendorList, Guid productId)
        {
            IQueryable<int> offId = from o in _context.Offering.Where(o => o.ProductID == productId)
                                    select o.VendorID;
            HashSet<int> OfferingsHS = offId.ToHashSet();

            DesignatedVendorList = new List<DesignatedVendor>();
            foreach (var vendor in vendorList)
            {
                DesignatedVendor desVend = new DesignatedVendor();
                desVend.VendorID = vendor.VendorID;
                desVend.VendorName = vendor.VendorName;
                if (OfferingsHS.Contains(vendor.VendorID))
                {
                    desVend.Designated = true;
                }
                DesignatedVendorList.Add(desVend);
            }
            return DesignatedVendorList;
        }

        public void UpdateOfferings(Guid productId, string[] sortedVendors, string[] selectedVendors)
        {
            HashSet<string> sortedVendorsHS = new HashSet<string>(sortedVendors);
            HashSet<string> selectedVendorHS = new HashSet<string>(selectedVendors);

            var productOfferings = from o in _context.Offering
                                   .Where(o => o.ProductID == productId)
                                   select o;

            foreach (var id in sortedVendors)
            {
                if (selectedVendorHS.Contains(id))
                {
                    if (!productOfferings.Select(o => o.VendorID).Contains(int.Parse(id)))
                    {
                        _context.Offering.Add(new Offering() { ProductID = productId, VendorID = int.Parse(id), AsOfDate = DateTime.Today });
                    }             
                }
                else
                {
                    if(productOfferings.Select(o => o.VendorID).Contains(int.Parse(id)))
                    {
                        try
                        {
                            Offering offeringToRemove = productOfferings.FirstOrDefault(o => o.VendorID == int.Parse(id));
                            _context.Remove(offeringToRemove);
                        }
                        catch (Exception)
                        {
                            throw;
                        }       
                    }
                }
            }
        }
    }
}

