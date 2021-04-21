using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CommunityPricing.Models;
using CommunityPricing.Areas.Data;

namespace CommunityPricing.Data
{
    public class CommunityPricingContext : IdentityDbContext<ApplicationUser>
    {
        public CommunityPricingContext(DbContextOptions<CommunityPricingContext> options)
            : base(options)
        {
        }
        public DbSet<CommunityPricing.Models.ProductCategory> ProductCategory { get; set; }
        public DbSet<CommunityPricing.Models.Product> Product { get; set; }
        public DbSet<CommunityPricing.Models.Vendor> Vendor { get; set; }
        public DbSet<CommunityPricing.Models.Offering> Offering { get; set; }
        public DbSet<CommunityPricing.Models.ArchivedOffering> ArchivedOffering { get; set; }

        

    }
}
