﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityPricing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CommunityPricing.Areas.Data;
using CommunityPricing.Areas.Authorization;
using CommunityPricing.Pages.Shared;
using Microsoft.EntityFrameworkCore;
using CommunityPricing.Data;


using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace CommunityPricing.Pages.Shared
{
    public class ArchiveOffering
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public ArchiveOffering(CommunityPricing.Data.CommunityPricingContext context)
        {
            _context = context;
        }

        public static List<ArchivedOffering> ArchivedOfferings { get; set; }

        public static async Task Archive(CommunityPricingContext _context)
        {
            ArchivedOfferings = _context.ArchivedOffering.ToList();
            List<Offering> offerings = await _context.Offering.ToListAsync();

            foreach (var offering in offerings)
            {
            //If I already have that guid in my archives, then get out
            //...maybe send a message up the ranks too.

            if (!(ArchivedOfferings.Select(aO => aO.OfferingID).Contains(offering.OfferingID)
                    && ArchivedOfferings.Select(aO => aO.Date).Contains(offering.AsOfDate)))
                {
                    ArchivedOffering archivedOffering = new ArchivedOffering();
                    archivedOffering.ArchivedOfferingID = Guid.NewGuid();
                    if (!ArchivedOfferings.Select(a => a.ArchivedOfferingID)
                    .Contains(archivedOffering.ArchivedOfferingID))
                    {
                        archivedOffering.OfferingID = offering.OfferingID;
                        archivedOffering.Date = offering.AsOfDate;
                        archivedOffering.Price = offering.ProductPricePerWeight;

                        _context.ArchivedOffering.Add(archivedOffering);
                    }
                    else
                    {

                    }
                }
                else
                {
                    var archivedOfferingToUpdate = await _context.ArchivedOffering
                        .FirstOrDefaultAsync(aO => aO.OfferingID == offering.OfferingID
                        && aO.Date == offering.AsOfDate);

                    if(archivedOfferingToUpdate.Price != offering.ProductPricePerWeight)
                    {
                        _context.Entry(archivedOfferingToUpdate).Property("Price")
                            .CurrentValue = offering.ProductPricePerWeight;
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
