using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSmileAlignerTS.Models
{
    public class Journal
    {
        public long Id { get; set; }
        [Index]
        public int InvoiceNr { get; set; }
        public int LineNr { get; set; }
        [Index]
        public int WareNumber { get; set; }
        [StringLength(100)]
        public string Longtext { get; set; }
        public Decimal Amount { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Price { get; set; }
        [StringLength(10)]
        public string Currency { get; set; }
        public decimal Tax { get; set; }
        [StringLength(100)]
        public string AddInfoText { get; set; }
    }
}