using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace iSmileAlignerTS.Models
{
    public class CaseCart
    {
        public long Id { get; set; }
        [Index]
        public int CartNumber { get; set; }
        [Index]
        public int CaseNumber { get; set; }
        public int PaymentNumber { get; set; }
        public bool isPayed { get; set; }
        [Index]
        public DateTime CreateDate { get; set; }
        public Decimal Amount { get; set; }
        [Index]
        public long WareNumber { get; set; }
        public string WareText { get; set; }
        public Decimal UnitPrice { get; set; }
        public Decimal Price { get; set; }
        public string Currency { get; set; }
        public Decimal Tax { get; set; }
        [Index]
        [StringLength(200)]
        public string UserId { get; set; }
        [StringLength(100)]
        public string AddInfoText { get; set; }
        public bool isRequestQuote { get; set; }
    }

    public class CaseCartView
    {
        public long CaseNumber { get; set; }
        public long CartNumber { get; set; }
        public List<CaseCart> CartList { get; set; }
        public List<Wares> Wares { get; set; }
        public decimal Total { get; set; }
        public decimal TotalNet { get; set; }
        public decimal Total10 { get; set; }
        public decimal Total20 { get; set; }
        public String AddWareItem { get; set; }
        public decimal AddWareAmount { get; set; }

        [Display(Name = "Titel")]
        [MaxLength(20)]
        public String CustomerTitel { get; set; }
        [Display(Name = "Anrede")]
        [MaxLength(20)]
        public String CustomerSalutation { get; set; }
        [Display(Name = "Vorname")]
        [StringLength(50)]
        public string CustomerGivenName { get; set; }
        [Display(Name = "Nachname")]
        [StringLength(50)]
        public string CustomerSurName { get; set; }
        [StringLength(100)]
        public string CustomerEmail { get; set; }
        [Display(Name = "Adresse 1. Zeile")]
        [StringLength(50)]
        public string BillingStreet1 { get; set; }
        [Display(Name = "Adresse 2. Zeile")]
        [StringLength(50)]
        public string BillingStreet2 { get; set; }
        [Display(Name = "Stadt")]
        [StringLength(30)]
        public string BillingCity { get; set; }
        [Display(Name = "Postleitzahl")]
        [StringLength(10)]
        public string BillingPostcode { get; set; }
        [StringLength(10)]
        public string BillingCountry { get; set; }

        public bool AcceptCheckout { get; set; }

        public string shopperMerchantInvId { get; set; }
        public string shopperMerchantDescriptor { get; set; }
        public string shopperMerchantTransId { get; set; }

        public Dictionary<string, dynamic> checkOut { get; set; }
        public string shopperResultUrl { get; set; }
        public string checkOutID { get; set; }
        public string checkOutResultCode { get; set; }
        public string checkOutResultDescription { get; set; }


        public bool testMode { get; set; }

        public bool isDoctorShop { get; set; }
    }
}
