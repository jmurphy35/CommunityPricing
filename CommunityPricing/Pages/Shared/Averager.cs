using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityPricing.Pages.Shared
{
    public class Averager
    {
        public decimal Average { get; set; }
        
        public decimal FindAverage(int den, decimal[] array)
        {
            decimal numerator = 0.0M;
            var arrayList = array.ToList();
            for (int i = 0; i < arrayList.Count; i++)
            {
                numerator = numerator + arrayList[i];
            }
            Average = numerator / den;
            return Average;
        }
    }
}
