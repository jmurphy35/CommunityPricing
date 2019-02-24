using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CommunityPricing.Models;

namespace CommunityPricing.Models
{
    public class ArchivedOffering
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid ArchivedOfferingID { get; set; }

        public Guid OfferingID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "n/a")]
        public decimal? Price { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public Offering Offering { get; set; }
    }
}
