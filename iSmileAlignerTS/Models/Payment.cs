using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSmileAlignerTS.Models
{
    public enum PaymentStates
    {
        Active = 1,
        Payed,
        Declinded,
        Refund,
        Reversal
    }
    public enum PaymentWPFState
    {
        A_CL_WS_ShoppingCart = 1,
        B_CL_WS_OrderClick,
        C_WS_MS_InitWPFSession,
        D_MS_WS_ReturnURLToWPFSession,
        E_WS_MS_RedirectUserBrowserToWPF,
        F_CL_MS_UserClicksPay,
        G_MS_WS_CallResponsePage,
        H_WS_CL_ReturnUrlToNextStep
    }

    public class Payment
    {
        public long Id { get; set; }
        public int PaymentNumber { get; set; }                                  // fortlaufende Nummer
        public bool IsCasePayment { get; set; }

        [Index]
        public int CaseNumber { get; set; }                                     // Eine fortlaufende Nummer
        [Index]
        public int CaseCart { get; set; }                                       // Welcher Einkaufswagen das war

        public CaseStates OldState{ get; set; }                                 // Zahlungszustand vom Fall
        public CaseStates NewOKState { get; set; }                              // Zahlungszustand vom Fall nach erfolgreicher Zahlung

        [Index]
        [StringLength(200)]
        public string UserId { get; set; }                                      // login das die Zahlungsdaten auslöst/bekommt

        public string BillLine { get; set; }                                    // Descriptor bzw. Merchantid
        public Decimal Total { get; set; }

        public PaymentStates Status { get; set; }
        public PaymentWPFState WPFStatus { get; set; }
        public DateTime? StartedTime { get; set; }
        public DateTime? EventTime { get; set; }
        public DateTime? FinishedTime { get; set; }

        [Index]
        [StringLength(100)]
        public string FlexId { get; set; }                                      // Zahlungsreferenz bei payunity.flex

        [StringLength(20)]
        public string ResultCode { get; set; }

        public bool isPayed { get; set; }
        public int ResultCode1 { get; set; }
        public int ResultCode2 { get; set; }
        public int ResultCode3 { get; set; }

        [StringLength(200)]
        public string ResultCodeMsg { get; set; }

        public bool PrepareCheckout { get; set; }
        public bool RetrieveCheckout { get; set; }
        public bool RefundCheckout { get; set; }

        [Index(IsUnique=false)]
        [StringLength(50)]
        public string shopperMerchantDescriptor { get; set; }
        [Index(IsUnique = false)]
        [StringLength(50)]
        public string shopperMerchantInvId { get; set; }
        [Index(IsUnique = false)]
        [StringLength(50)]
        public string InvoiceDeliveryId { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}