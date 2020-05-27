using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommunityPricing.Models
{
    public class Product
    {
        //Did not use [DatabaseGenerated(DatabaseGeneratedOption.None)]. So db will pick the primary key instead of app choosing it.
        [Display(Name = "Product ID Number")]
        public Guid ProductID { get; set; }

        [Display(Name = "Product Name")]
        [StringLength(50, MinimumLength = 1)]
        public string ProductName { get; set; }

        //I want to allow letters and numbers only in Product Description.
        [Display(Name = "Product Type")]
        [StringLength(50, ErrorMessage = "Product Description cannot be longer than 50 characters.")]
        [Column("DescrOne")]
        public string ProductDescr1 { get; set; }

        [Display(Name = "Weight(or Volume)")]
        [StringLength(20, ErrorMessage = "Product Description cannot be longer than 20 characters.")]
        [Column("Weight_Volume")]
        public string ProductDescr2_Wt_Vol { get; set; }

        [DisplayFormat(NullDisplayText = "No wholesaler identified")]
        [StringLength(50, ErrorMessage = "Wholesaler cannot be longer than 50 characters.")]
        public string Wholesaler { get; set; }

        [Display(Name = "Product Category ID")]
        public int ProductCategoryID { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        //ICollection<T> or List<T>, or HashSet<T>; EF makes a hashset when using ICollection<T>
        public ProductCategory ProductCategory { get; set; }

        [Display(Name = "Offered By")]
        public ICollection<Offering> Offering { get; set; }
    }
}
