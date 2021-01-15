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
                desVend.VendorAddress = vendor.VendorAddress1 + ", " + vendor.VendorAddress2;
               
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
        //-------------------------------------------------------------------------------------------------------------
        public CalculatedInflation MakeCalculatedInflation(List<ArchivedOffering> allPcArchives, string pcName, int i)
        {
            CalculatedInflation calculatedInflation;

            List<ArchivedOffering> AnnualArchives = MakeAnnualArchives(allPcArchives, i);
            List<Guid> distinctIDs = FindDistinctOfferingIDs(AnnualArchives);
            
            List<double> inflationsForOfferings = new List<double>();
            foreach (var id in distinctIDs)
            {
                List<XYPOINT> xYPOINTs; 
                double inflationForOffering;
                double rationalizedInflation;
                List<ArchivedOffering> archivesPerOffering = GroupByOffering(id, AnnualArchives);
                if (archivesPerOffering.Count >= 2)
                {
                    xYPOINTs = MakeXYPOINTS(archivesPerOffering);
                    inflationForOffering = InflationCalculator(xYPOINTs);
                    rationalizedInflation = RationalizeInlfationForYear(inflationForOffering, xYPOINTs);
                    rationalizedInflation = Math.Round(rationalizedInflation, 5);
                    inflationsForOfferings.Add(rationalizedInflation);
                }
            }
            if (inflationsForOfferings.Count >= 1)
            {
                double annualPCInflation = ReturnAverage(inflationsForOfferings);
                string infl = String.Format("{0:0.00%}", annualPCInflation);
                calculatedInflation = InflationHelper(infl, i, pcName);
            }
            else
            {
                calculatedInflation = InflationHelper("not avail.", i, pcName);
            }
            return calculatedInflation;
        }

        public List<ArchivedOffering> MakeAnnualArchives(List<ArchivedOffering> allPcArchives, int i)
        {
            List<ArchivedOffering> AnnualArchives = new List<ArchivedOffering>();
            foreach (var ao in allPcArchives)
            {
                if (ao.Date != null && ao.Price != null && ao.Date.Year == DateTime.Now.Year - i)
                {
                    AnnualArchives.Add(ao);
                }
            }
            return AnnualArchives;
        }
        public List<Guid> FindDistinctOfferingIDs(List<ArchivedOffering> archivesCurrentYear)
        {     
            List<Guid> distinctIDs = archivesCurrentYear.Select(a => a.OfferingID).Distinct().ToList(); 
            return distinctIDs;
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

        public double RationalizeInlfationForYear(double inflationRate, List<XYPOINT> xypoints)
        {
            double rationalizedInflation = new double();
            double span = xypoints.Max(x => x.X) - xypoints.Min(x => x.X);
            rationalizedInflation = (inflationRate * 365) /span;

            return rationalizedInflation;
        }

        public double ReturnAverage(List<double> mydoubles)
        {
            double doubles = mydoubles.Average();
            return doubles;
        }

        public List<XYPOINT> MakeXYPOINTS(List<ArchivedOffering> archivedOfferings)
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
            
            return xypoints;
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

            inflationRate = (endprice - startprice) / startprice;

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

