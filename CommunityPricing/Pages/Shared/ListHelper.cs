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

        
        //public Offering Offering { get; set; }
        //public List<Vendor> Vendors { get; set; }
        public List<ArchivedOffering> ArchivedOffering { get; set; }
        public List<ProductCategory> ProductCategory { get; set; }
        public List<DesignatedVendor> DesignatedVendorList { get; set; }
        

        public class XYPOINT
        {
            public double X { get; set; }
            public double Y { get; set; }
        }

        //public static SortedDictionary<int, int> myDictionaryX { get; set; }
        //public static SortedDictionary<int, double> myDictionaryY { get; set; }


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

        public List<double> GetPricesFromArchives(Guid offeringId)
        {
            List<double> archivedPrices = new List<double>();
            var offering = _context.ArchivedOffering.Where(a => a.OfferingID == offeringId);

            foreach (var ao in offering)
            {
                if (ao.Price.HasValue)
                {
                    archivedPrices.Add((double)ao.Price);
                }
            }
            return archivedPrices;
        }

        public CalculatedInflation GroupByYear(List<ArchivedOffering> allPcArchives, DateTime oldestYear, string pcName, int i)
        {   
                List<ArchivedOffering> AnnualOfferings = new List<ArchivedOffering>();

                CalculatedInflation calculatedInflation = new CalculatedInflation();
                foreach (var ao in allPcArchives)
                {
                    if (ao.Date != null && ao.Price != null && ao.Date.Year == DateTime.Now.Year - i)
                    {
                        AnnualOfferings.Add(ao);
                    }
                }
                List<Guid> oIds = AnnualOfferings.Select(o => o.OfferingID).Distinct().ToList();
                List<double> OfferingInflations = new List<double>();

                foreach (var id in oIds)
                {
                    List<ArchivedOffering> ArchivesPerOffering = new List<ArchivedOffering>();
                    ArchivesPerOffering = GroupByOffering(id, AnnualOfferings);
                    if (ArchivesPerOffering.Count >= 2)
                    {
                        double inflationForAnOffering = MakeXYPOINTS(ArchivesPerOffering);
                        OfferingInflations.Add(inflationForAnOffering);
                    }
                }
                if (OfferingInflations.Count >= 1)
                {
                    double averageInflation = ReturnAverage(OfferingInflations);
                    string infl = String.Format("{0:0.00%}", averageInflation);
                    calculatedInflation = InflationHelper(infl, i, pcName);
                }
                else
                {
                    calculatedInflation = InflationHelper("Not Avail", i, pcName);
                }
                return calculatedInflation; 
        }

        public List<ArchivedOffering> GroupByOffering(Guid id, List<ArchivedOffering> archivedOfferings)
        {
            List<ArchivedOffering> ArchivedOfferings = new List<ArchivedOffering>();
            foreach (var archive in archivedOfferings)
            {
                if (archive.OfferingID == id)
                {
                    ArchivedOfferings.Add(archive);
                }
            }
            return ArchivedOfferings;
        }

        public double ReturnAverage(List<double> mydoubles)
        {
            double doubles = mydoubles.Average();
            doubles = Math.Round(doubles, 5);
            return doubles;
        }

        

        public double MakeXYPOINTS(List<ArchivedOffering> archivedOfferings)
        {
            List<XYPOINT> xypoints = new List<XYPOINT>();

            foreach (var archive in archivedOfferings)
            {
                double aoPrice = (double)archive.Price;

                XYPOINT xyPoint = new XYPOINT();
                xyPoint.X = archive.Date.DayOfYear;
                xyPoint.Y = aoPrice;
                xypoints.Add(xyPoint);
            }
            xypoints = xypoints.OrderBy(x => x.X).ToList();
            double inflationRate = InflationCalculator(xypoints);

            return inflationRate;
        }

        public CalculatedInflation InflationHelper(string inflation, int i, string pcName)
        {
            //NumberFormatInfo nfi = new CultureInfo("en-us", false).NumberFormat;

            CalculatedInflation calculatedInflation = new CalculatedInflation();

            calculatedInflation.InflationID = 1000;
            calculatedInflation.InflationName = pcName;
            calculatedInflation.FiscalYear = DateTime.Now.Year - i;

            calculatedInflation.InflationRate = inflation;

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
           
            
            double startprice = xypoints[FindIndexOfMin(xypoints)].X * m + b;

            double endprice = xypoints[FindIndexOfMax(xypoints)].X * m + b;

            inflationRate = Math.Round((endprice - startprice) / startprice, 5);

            return inflationRate;
        }

        public int FindIndexOfMin(List<XYPOINT> xyPoints)
        {
            int indexOfMin = 0;
            XYPOINT min = xyPoints[0];
            for (int i = 0; i < xyPoints.Count; i++)
            {
                if(xyPoints[i].X < min.X)
                {
                    min = xyPoints[i];
                    indexOfMin = i;
                }
            }
            return indexOfMin;
        }

        public int FindIndexOfMax(List<XYPOINT> xyPoints)
        {
            int indexOfMax = 0;
            XYPOINT max = xyPoints[0];
            for (int i = 0; i < xyPoints.Count; i++)
            {
                if (xyPoints[i].X > max.X)
                {
                    max = xyPoints[i];
                    indexOfMax = i;
                }
            }
            return indexOfMax;
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

