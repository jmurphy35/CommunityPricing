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
using CommunityPricing.Areas.Models.HelperModels;
using CommunityPricing.Pages.Shared;
using Microsoft.AspNetCore.Identity;
using CommunityPricing.Areas.Data;
using CommunityPricing.Areas.Authorization;

namespace CommunityPricing.Pages.GeneralPublic
{
    [AllowAnonymous]
    public class StatisticsModel : ListHelper
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public StatisticsModel(CommunityPricing.Data.CommunityPricingContext context,
        IAuthorizationService authorizationService,
        UserManager<ApplicationUser> userManager)
        : base(context, authorizationService, userManager)
        {
            _context = context;
        }

        public PaginatedList<CalculatedInflation> CalculatedInflation { get; set; }
        public List<CalculatedInflation> calculatedInflations { get; set; }


        public async Task OnGetAsync(int? pageIndex)
        {
            //This page will have pagination, but not sort, and not filter

            IQueryable<ProductCategory> productCategoryIQ = from pc in _context.ProductCategory
                                          .Include(p => p.Product)
                                          .ThenInclude(o => o.Offering)
                                          .ThenInclude(ao => ao.ArchivedOffering)
                                                            select pc;

            calculatedInflations = new List<CalculatedInflation>();

            foreach (var pc in productCategoryIQ)
            {
                List<ArchivedOffering> archivesInCategory = pc.Product.SelectMany(o => o.Offering.SelectMany(ao => ao
               .ArchivedOffering.Where(aoVal => aoVal.Price.HasValue).Where(aoDate => aoDate.Date != null))).ToList();

                DateTime oldestDate = archivesInCategory.Min(aoDate => aoDate.Date);
                int length = DateTime.Now.Year - oldestDate.Year + 1;
                if (archivesInCategory.Count != 0)
                {
                    for (int i = 0; i < length; i++)
                    {
                        CalculatedInflation calculatedInflation = GroupByYear(archivesInCategory, oldestDate, pc.Name, i);
                        calculatedInflations.Add(calculatedInflation);
                    }                    
                }          
            }
            calculatedInflations = calculatedInflations.OrderBy(c => c.InflationName)
                .ThenByDescending(date => date.FiscalYear).ToList();

            int pageSize = 12;

            CalculatedInflation = PaginatedList<CalculatedInflation>.CreateNonAsync(calculatedInflations, pageIndex ?? 1, pageSize);


            //***********************Authorization***********************************************************************

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Product, Operations.Read);

            if (!isAuthorized.Succeeded)
            {
                new ChallengeResult();
            }
            //**********************************************************************************************************

        }
    }
}
