using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommunityPricing.Models
{
    public class ProductCategory
    {
        [Display(Name = "Category Number")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductCategoryID { get; set; }
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Category Name")]
        public string Name { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public ICollection<Product> Product { get; set; }
    }
}
