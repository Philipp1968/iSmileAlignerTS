using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iSmileAlignerTS.Validation;

namespace iSmileAlignerTS.Models
{
    public enum WARESSHOP { all = 0, doctors = 1, patients = 2, dontshow = 3}

    public class Wares
    {
        public long Id { get; set; }
        [Index]
        public int WareNumber { get; set; }
        [StringLength(100)]
        public string Longtext { get; set; }
        public decimal PriceDoc { get; set; }               // doc Preis
        public decimal PricePat { get; set; }               // pat Preis
        [StringLength(10)]
        public string Currency { get; set; }
        public decimal Tax { get; set; }
        [StringLength(100)]
        public string AddInfoText { get; set; }
        public bool isRequestQuote { get; set; }
        [Index(IsUnique=false)]
        public WARESSHOP InShop { get; set; }                        
    }

    public class WaresView
    {
        public long Id { get; set; }
        [Index]
        public int WareNumber { get; set; }
        [StringLength(100)]
        public string Longtext { get; set; }
        public decimal PriceDoc { get; set; }               // doc Preis
        public decimal PricePat { get; set; }               // pat Preis
        [StringLength(10)]
        public string Currency { get; set; }
        public decimal Tax { get; set; }
        [StringLength(100)]
        public string AddInfoText { get; set; }
        public bool isRequestQuote { get; set; }
        [Index(IsUnique = false)]
        public WARESSHOP InShop { get; set; }

        [MaxFileSize(1 * 1024 * 1024, ErrorMessage = "Maximal 1MB")]
        [FileTypes("jpg,jpeg,png", ErrorMessage = "JPG, JPEG oder PNG")]
        public HttpPostedFileBase WareFotoBin { get; set; }
        public string WareFotoName { get; set; }
        public long WareFotoId { get; set; }
        public long WareThumbId { get; set; }
    }
}
