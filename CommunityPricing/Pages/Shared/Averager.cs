using System;
using System.Collections.Generic;
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
            Average = num / den;
            return Average;
        }
    }
}
