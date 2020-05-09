using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;


namespace CommunityPricing.Pages.Shared
{
    public class Averager
    {
        public static decimal Average { get; set; }
        
        public static decimal FindAverage(int den, List<decimal> numList)
        {
            decimal num = 0.0M;
            for (int i = 0; i < numList.Count; i++)
            {
                num = num + numList[i];
            }
            if (den > 0)
            {
                Average = num / den;
            }
            else { Average = 0; }
            return Average;
        }
    }

    public class PercentIncreaser
    {
        public decimal PercentIncrease { get; set; }

        public decimal FindPercentIncrease(decimal basis, decimal increment)
        {
            PercentIncrease = ((increment - basis) / basis)*100;
            return PercentIncrease;
        }
    }
    public class RegressionCalc
    {
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public decimal XY { get; set; }
        public decimal Xsquared { get; set; }
        public decimal Ysquared { get; set; }
        public decimal CalcRegressionPoint(List<decimal> xpoints, List<decimal> ypoints)
        {
            decimal basis = 0;
            double[] xdata = new double[] { 10, 20, 30 };
            double[] ydata = new double[] { 15, 20, 25 };

            return basis;
           
           
        }
    }
}
