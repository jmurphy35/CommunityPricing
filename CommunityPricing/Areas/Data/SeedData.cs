using CommunityPricing.Areas.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityPricing.Models;
using CommunityPricing.Areas.Data;

namespace CommunityPricing.Data
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            using (var context = new CommunityPricingContext(
                serviceProvider.GetRequiredService<DbContextOptions<CommunityPricingContext>>()))
            {
                // For sample purposes seed both with the same password.
                // Password is set with the following:
                // dotnet user-secrets set SeedUserPW <pw>
                // The admin user can do anything

                var adminID = await EnsureUser(serviceProvider, testUserPw, "jmurphy35@gmail.com");
                await EnsureRole(serviceProvider, adminID, Constants.AdministratorsRole);

                SeedDB(context, adminID);
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                                    string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new ApplicationUser { UserName = UserName };
                await userManager.CreateAsync(user, testUserPw);
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                                      string uid, string role)
        {
            IdentityResult IR = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByIdAsync(uid);

            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }

        public static void SeedDB(CommunityPricingContext context, string adminID)
        {

            //context.Database.EnsureCreated();

            if(context.Product.Any())
            {
                return;
            }
            Guid guid1 = Guid.NewGuid();
            Guid guid2 = Guid.NewGuid();
            Guid guid3 = Guid.NewGuid();
            

            var productCategory = new ProductCategory{ ProductCategoryID= 1000, Name = "Test Category"};
            context.ProductCategory.Add(productCategory);
            context.SaveChanges();

            var products = new Product[]
            {
                new Product{ProductID = guid1, ProductName = "Product A", ProductDescr1 = "Desc1",
                    ProductDescr2_Wt_Vol = "1lb", ProductCategoryID = 1000, Wholesaler = "Unknown", },
                new Product{ProductID = guid2, ProductName = "Product B", ProductDescr1 = "Desc1",
                    ProductDescr2_Wt_Vol = "1lb", ProductCategoryID = 1000, Wholesaler = "Unknown"},
                new Product{ProductID = guid3, ProductName = "Product C", ProductDescr1 = "Desc1",
                    ProductDescr2_Wt_Vol = "1lb", ProductCategoryID = 1000, Wholesaler = "Unknown"}
            };

            foreach (var product in products)
            {
                context.Product.Add(product);
            }
            context.SaveChanges();
            /*return; */  // DB has been seeded

        }
            
    }
}
