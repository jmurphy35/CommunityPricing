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
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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

        public List<Product> Product { get; set; }

        public Offering Offering { get; set; }
        public List<Offering> Offerings { get; set; }
        public List<Vendor> Vendors { get; set; }
        public List<ArchivedOffering> ArchivedOffering { get; set; }
        public List<ProductCategory> ProductCategory { get; set; }
        public List<DesignatedVendor> DesignatedVendorList { get; set; }
        public List<CalculatedInflation> calculatedInflations { get; set; }
        //public List<CalculatedInflation> calcInfl { get; set; }

        public class XYPOINT
        {
            public double X { get; set; }
            public double Y { get; set; }
        }

        public static SortedDictionary<int, int> myDictionaryX { get; set; }
        public static SortedDictionary<int, double> myDictionaryY { get; set; }
        

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
                    if (productOfferings.Select(o => o.VendorID).Contains(int.Parse(id)))
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
        public void GroupByYear(List<ArchivedOffering> aos, DateTime oldestYear, string pcName)
        {
            int length = DateTime.Now.Year - oldestYear.Year + 1;

            for (int i = 0; i < length; i++)
            {
                List<XYPOINT> xypoints = new List<XYPOINT>();

                foreach (var ao in aos)
                {
                    if (ao.Date != null && ao.Price != null && ao.Date.Year == DateTime.Now.Year - i)
                    {
                        double aoPrice = (double)ao.Price;

                        XYPOINT xyPoint = new XYPOINT();
                        xyPoint.X = ao.Date.DayOfYear;
                        xyPoint.Y = aoPrice;
                        xypoints.Add(xyPoint);      
                    }
                }
                xypoints = xypoints.OrderBy(x => x.X).ToList();
                CalculatedInflation calculatedInflation = InflationHelper(xypoints, i, pcName);
                calculatedInflations.Add(calculatedInflation);
            }
            calculatedInflations = calculatedInflations.OrderBy(c => c.InflationName)
                .ThenByDescending(date => date.FiscalYear).ToList();
        }

       
        public CalculatedInflation InflationHelper(List<XYPOINT> xypoints, int i, string pcName)
        {
            //NumberFormatInfo nfi = new CultureInfo("en-us", false).NumberFormat;

            CalculatedInflation calculatedInflation = new CalculatedInflation();
            

            calculatedInflation.InflationID = 1000;
            calculatedInflation.InflationName = pcName;
            calculatedInflation.FiscalYear = DateTime.Now.Year - i;

            if (xypoints.Count >= 2)
            {
                calculatedInflation.InflationRate = String.Format("{0:0.00%}", InflationCalculator(xypoints));
                
            }
            else
            {
                calculatedInflation.InflationRate = "not avail.";
            }
            return calculatedInflation;
        }
        
        public double InflationCalculator(List<XYPOINT> xypoints)
        {
            
            double inflationRate = 0;
            double m = 0; double b = 0;

            double Xavg = xypoints.Average(x => x.X);
            double Yavg = xypoints.Average(y => y.Y);

            double numeratorOfSlope = 0;
            double denominatorOfSlope = 0;


            for (int i = 0; i < xypoints.Count; i++)
                {
               numeratorOfSlope += (xypoints[i].X - Xavg) * (xypoints[i].Y - Yavg);
               denominatorOfSlope += (xypoints[i].X - Xavg) * (xypoints[i].X - Xavg);
             }

            m = numeratorOfSlope / denominatorOfSlope;
            b = Yavg - (m * Xavg);

            double startprice = b;

            double endprice = 365 * m + b;

            inflationRate = Math.Round((endprice - startprice) / startprice, 5);
            
            return inflationRate;
        }     

        public void GroupByOffering(List<ArchivedOffering> archivedOfferings)
        {
            List<Guid> offeringIds = new List<Guid>();

            foreach (var archive in archivedOfferings)
            {
                offeringIds.Add(archive.OfferingID);
            }
            offeringIds = offeringIds.Distinct().ToList();

            List<double> InflationRatesOfOfferings = new List<double>();
            foreach (var id in offeringIds)
            {
                List<XYPOINT> xypoints = new List<XYPOINT>();

                foreach (var archive in archivedOfferings)
                {
                    if (archive.OfferingID == id)
                    {
                        double aoPrice = (double)archive.Price;

                        XYPOINT xyPoint = new XYPOINT();
                        xyPoint.X = archive.Date.DayOfYear;
                        xyPoint.Y = aoPrice;
                        xypoints.Add(xyPoint);
                    }
                }


                if(xypoints.Count >= 2)
                {
                    xypoints = xypoints.OrderBy(x => x.X).ToList();
                    double inflationRateOfAnOffering = InflationCalculator(xypoints);
                    InflationRatesOfOfferings.Add(inflationRateOfAnOffering);
                }     
            }
            double annualAverageInflation = InflationRatesOfOfferings.Average(x => x);
        }



        public List<string> ListOfPublishedInflations()
        {
            List<string> publishedInflations = new List<string>();
            string produce2020 = "4.65";
            publishedInflations.Add(produce2020);
            string produce2019 = "4.55";
            publishedInflations.Add(produce2019);
            return publishedInflations;
        }
    } 
}

