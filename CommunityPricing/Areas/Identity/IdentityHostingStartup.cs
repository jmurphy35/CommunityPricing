using System;
using CommunityPricing.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CommunityPricing.Areas.Data;


[assembly: HostingStartup(typeof(CommunityPricing.Areas.Identity.IdentityHostingStartup))]
namespace CommunityPricing.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices(configureServices: (context, services) => {

                //But can add an ApplicationUser class to the Data folder, then ApplicationUser
                //Inherits from Identity User, then change below IdentityUser to ApplicationUser
                services.AddIdentity<ApplicationUser, IdentityRole>(config => {

                    config.SignIn.RequireConfirmedEmail = true;
                    config.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<CommunityPricingContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders();
            });
            
        }
    }
}