using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSmileAlignerTS.Models
{
    public class Invoice
    {
        public long Id { get; set; }
        
        [Index(IsUnique=true)]
        public int InvoiceNr { get; set; }
        
        [Index]
        public DateTime ReDate { get; set; }

        [Index]
        public long PaymentId { get; set; }
        [Index]
        public long PaymentNumber { get; set; }

        [Index]
        public int CaseNumber { get; set; }                                     // Eine fortlaufende Nummer
        [MaxLength(200)]
        [Index]
        public String CaseNumberStr { get; set; }                               // Fall-Nummer für Thomas AT SCHNELL 2015101301

        [MaxLength(20)]
        public String CustomerTitel { get; set; }
        [MaxLength(20)]
        public String CustomerSalutation { get; set; }
        [StringLength(50)]
        public string CustomerGivenName { get; set; }
        [StringLength(50)]
        public string CustomerSurName { get; set; }
        [StringLength(100)]
        public string CustomerEmail { get; set; }
        [StringLength(50)]
        public string BillingStreet1 { get; set; }
        [StringLength(50)]
        public string BillingStreet2 { get; set; }
        [StringLength(30)]
        public string BillingCity { get; set; }
        [StringLength(10)]
        public string BillingPostcode { get; set; }
        [StringLength(10)]
        public string BillingCountry { get; set; }

        public Decimal TotalNet { get; set; }
        public Decimal Total { get; set; }
        public Decimal Tax0 { get; set; }
        public Decimal Tax10 { get; set; }
        public Decimal Tax20 { get; set; }

        [StringLength(10)]
        public string Currency { get; set; }

        [Index(IsUnique=false)]
        public bool IsJustDelivery { get; set; }
    }
}
