using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using iSmileAlignerTS.Models;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Text;
using Newtonsoft.Json;
using System.Globalization;
using System.Data.Entity;
using System.Text.RegularExpressions;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.ReportSource;
using CrystalDecisions.Shared;
using System.Web.Hosting;
using System.Net.Mail;
using System.Net.Mime;
using ISmileAlignerTS.Controllers;

namespace iSmileAlignerTS.Controllers
{
    [Authorize]
    public class PaymentController : ISmileController
    {
        public PaymentController()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public PaymentController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }


        /// PUMA Zugänge
        /// https://payunity.com/frontend/payment.prc
        /// LIVE
        /// sender 8a8394c44ba1d2ca014bc13b942c6712
        /// user 8a8394c44ba1d2ca014bc13b942d6714
        /// password 4fsmBpHG
        /// channel 8a8394c44ba1d2ca014bc13c1ccf6718
        /// TEST
        /// sender 8a8294174ae82ada014af1b9c6da2c73
        /// user 8a8294174ae82ada014af1b9c6da2c77
        /// password 2ka86ena
        /// channel 8a8294174ae82ada014af1bd19af2c7d

        public string AddURLEncode(string parameter, string value)
        {
            string result = "&";
            result += parameter;
            result += "=";
            result += Url.Encode(value);
            return result;
        }


        public Dictionary<string, dynamic> PrepareCheckout(CaseCartView model)
        {
            Dictionary<string, dynamic> responseData;
            string data = "";
            string url = "";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            if (model.testMode)
            {
                data = "authentication.userId=8a8294174ae82ada014af1b9c6da2c77" +
                    "&authentication.password=2ka86ena" +
                    "&authentication.entityId=8a8294174ae82ada014af1bd19af2c7d";
                url = "https://test.oppwa.com/v1/checkouts";
            }
            else
            {
                data = "authentication.userId=8a8394c44ba1d2ca014bc13b942d6714" +
                    "&authentication.password=4fsmBpHG" +
                    "&authentication.entityId=8a8394c44ba1d2ca014bc13c1ccf6718";
                url = "https://oppwa.com/v1/checkouts";
            }

            data += AddURLEncode("customer.givenName", model.CustomerGivenName);
            data += AddURLEncode("customer.surname", model.CustomerSurName);
            data += AddURLEncode("customer.email", model.CustomerEmail);

            data += AddURLEncode("descriptor", model.shopperMerchantDescriptor);
            data += AddURLEncode("merchantInvoiceId", model.shopperMerchantInvId);
            data += AddURLEncode("merchantTransactionId", model.shopperMerchantTransId);

            string t = model.Total.ToString("#0.00", new CultureInfo("en-US"));
            data += AddURLEncode("amount", t);

            data += AddURLEncode("currency", "EUR");
            data += AddURLEncode("paymentType", "DB");

            byte[] buffer = Encoding.ASCII.GetBytes(data);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            Stream PostData = request.GetRequestStream();
            PostData.Write(buffer, 0, buffer.Length);
            PostData.Close();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                var s = new JavaScriptSerializer();
                responseData = s.Deserialize<Dictionary<string, dynamic>>(reader.ReadToEnd());
                reader.Close();
//                dataStream.Close();
//                dataStream.Dispose();
//                dataStream = null;
            }
            return responseData;
        }


        public Dictionary<string, dynamic> RetrieveCheckout(CaseCartView model, string resourcePath)
        {
            // /v1/checkouts/BEF950E6CA0C67C11FED2C5DABA4FC4C.sbg-vm-tx01/payment
            Dictionary<string, dynamic> responseData;

            if (String.IsNullOrWhiteSpace(resourcePath))
            {
                responseData = new Dictionary<string, dynamic>();
                return responseData;
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string data ="";
            string url= "";

            if (model.testMode)
            {
                data = "authentication.userId=8a8294174ae82ada014af1b9c6da2c77" +
                    "&authentication.password=2ka86ena" +
                    "&authentication.entityId=8a8294174ae82ada014af1bd19af2c7d";
                url = "https://test.oppwa.com";
            }
            else
            {
                data = "authentication.userId=8a8394c44ba1d2ca014bc13b942d6714" +
                    "&authentication.password=4fsmBpHG" +
                    "&authentication.entityId=8a8394c44ba1d2ca014bc13c1ccf6718";
                url = "https://oppwa.com";
            }
            url += resourcePath ?? "";
            url += "?" + data;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                var s = new JavaScriptSerializer();
                responseData = s.Deserialize<Dictionary<string, dynamic>>(reader.ReadToEnd());
                reader.Close();
//                dataStream.Close();
//                dataStream.Dispose();
//                dataStream = null;
            }
            return responseData;
        }




        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        // View Methoden


        // GET
        [Authorize]
        public async Task<ActionResult> PayCaseCart(string id)
        {
            int caseNum = 0;
            int cartNum = 0;
            IEnumerable<CaseCart> carts = null;

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (!user.isVerified)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            shopKind = WhichShop(Request);

            MvcApplication.logMsg("shopkind = " + shopKind.ToString());

            CaseCartView model = new CaseCartView();
            model.Total = 0;
            model.TotalNet = 0;
            model.Total10 = 0;
            model.Total20 = 0;
            model.CaseNumber = caseNum;
            // TESTMODE true = ohne echte Abrechnung
            // false = kein Test, Daten werden abgerechnet!!!
            //
            model.testMode = false;
            if (user.Email == "info@philippott.eu") model.testMode = true;

            model.isDoctorShop = false;
            if (shopKind == SHOPART.DoctorShop) model.isDoctorShop = true;
            if (shopKind == SHOPART.DevelShop) model.testMode = true;

            MvcApplication.logMsg("doctorshop=" + model.isDoctorShop.ToString());

            if (id == null || id == "")
            {
                // Nicht fall gefunden
                //
                Session["caseNumber"] = caseNum;

                // Aha, Kunde, direkt dem Login sozusagen verrechnen
                //
                model.CustomerSalutation = user.Salutation;
                model.CustomerTitel = user.Titel;
                model.CustomerGivenName = user.Firstname;
                model.CustomerSurName = user.Lastname;
                model.CustomerEmail = user.Email;
                model.BillingStreet1 = user.Address;
                model.BillingStreet2 = "";
                model.BillingPostcode = user.ZIP;
                model.BillingCity = user.City;
                model.BillingCountry = user.CountryCode;

                cartNum = GetCartNumber(0, user.Id);
                Session["cartNum"] = cartNum;

                model.CartNumber = cartNum;

                // Wagen laden
                //
                model.CartList = new List<CaseCart>();
                carts = GetUnpayedCart(cartNum);
                if (carts != null && carts.Count() > 0)
                {
                    foreach (CaseCart c in carts)
                    {
                        model.CartList.Add(c);
                        Decimal inkl = Math.Round(c.Price * (1m + c.Tax / 100m), 2);
                        model.Total += inkl;
                        model.TotalNet += c.Price;
                        if (c.Tax == 20)
                        {
                            model.Total20 += inkl - c.Price;
                        }
                        else if (c.Tax == 10)
                        {
                            model.Total10 += inkl - c.Price;
                        }
                    }
                }
            }
            else
            {
                if (Int32.TryParse(id, out caseNum) == false || caseNum <= 0)
                {
                    Session.Clear();
                    return RedirectToAction("Index", "Home");
                }

                Session["caseNumber"] = caseNum;
                model.CaseNumber = caseNum;

                //// dazu gibt es mindestens eine caseNumber, die kleinste, wo was ispayed=false ist

                //// alle die zusammenfassen

                //// und anzeigen

                //// zeilen wegnehmen

                //// zeilen hinzufügen (ispayed=false)

                //// bezahlen button beginnt prozedere

                // Kundedaten eintragen
                // isdamin -> fall-daten eintragen
                if (user.isAdmin)
                {
                    IEnumerable<Cases> aCaseList = db.Cases.Where(x => x.CaseNumber == caseNum);
                    if (aCaseList != null && aCaseList.Count() == 1)
                    {
                        Cases aCase = aCaseList.First();
                        var aUser = await UserManager.FindByIdAsync(aCase.DoctorId);
                        if (aUser != null)
                        {
                            model.CustomerSalutation = aUser.Salutation;
                            model.CustomerTitel = aUser.Titel;
                            model.CustomerGivenName = aUser.Firstname;
                            model.CustomerSurName = aUser.Lastname;
                            model.CustomerEmail = aUser.Email;
                            model.BillingStreet1 = aUser.Address;
                            model.BillingStreet2 = "";
                            model.BillingPostcode = aUser.ZIP;
                            model.BillingCity = aUser.City;
                            model.BillingCountry = aUser.CountryCode;
                        }
                        aUser = null;
                    }
                    aCaseList = null;
                }
                else
                {
                    model.CustomerSalutation = user.Salutation;
                    model.CustomerTitel = user.Titel;
                    model.CustomerGivenName = user.Firstname;
                    model.CustomerSurName = user.Lastname;
                    model.CustomerEmail = user.Email;
                    model.BillingStreet1 = user.Address;
                    model.BillingStreet2 = "";
                    model.BillingPostcode = user.ZIP;
                    model.BillingCity = user.City;
                    model.BillingCountry = user.CountryCode;
                }

                // Wagen laden
                //
                cartNum = GetCartNumber(caseNum, null);

                model.CartNumber = cartNum;

                model.CartList = new List<CaseCart>();
                carts = GetUnpayedCart(cartNum);
                if (carts != null && carts.Count() > 0)
                {
                    foreach (CaseCart c in carts)
                    {
                        model.CartList.Add(c);
                        Decimal inkl = Math.Round(c.Price * (1m + c.Tax / 100m), 2);
                        model.Total += inkl;
                        model.TotalNet += c.Price;
                        if (c.Tax == 20)
                        {
                            model.Total20 += inkl - c.Price;
                        }
                        else if (c.Tax == 10)
                        {
                            model.Total10 += inkl - c.Price;
                        }
                    }
                }
            }
            carts = null;

            // Artikel laden
            //
            WARESSHOP showShop = WARESSHOP.all;
            if (shopKind == SHOPART.DoctorShop) showShop = WARESSHOP.doctors;
            if (shopKind == SHOPART.PatientShop) showShop = WARESSHOP.patients;
            model.Wares = new List<Wares>();
            IEnumerable<Wares> wares = null;
            if (user.isAdmin)
            {
                wares = db.Wares.OrderBy(x => x.WareNumber).ToList();
            }
            else
            {
                wares = db.Wares.Where(x => x.InShop == showShop || x.InShop == WARESSHOP.all).OrderBy(x => x.WareNumber).ToList();
            }
            if (wares != null && wares.Count() > 0)
            {
                foreach (Wares w in wares)
                {
                    model.Wares.Add(w);
                }
            }
            wares = null;

            model.AddWareAmount = 1;

            Session["cartList"] = model;
            Session["cartNum"] = cartNum;
            Session["caseNumber"] = caseNum;

            return View(model);
        }


        // POST
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PayCaseCart(string addWareButton, string delWareButton, string payLSButton, CaseCartView vmodel)
        {
            int result = 0;
            int caseNum = 0;
            int cartNum = 0;
            ///// System.Linq.IQueryable<ApplicationUser> adminusers = null;
            CaseCartView model = null;
            IEnumerable<CaseCart> carts = null;
            int renr = 0;
            DateTime reDatum= DateTime.Now.Date;
            bool isDeliveryReport = String.IsNullOrWhiteSpace(payLSButton) ? false : true;

            shopKind = WhichShop(Request);

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (!user.isVerified)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            if (Session.IsNewSession)
            {
                Session.Clear();
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                MvcApplication.logMsg("ModelState.Invalid " + String.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)));
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "Eingabefehler gefunden.");
                CaseCartView old = (CaseCartView)Session["cartList"];
                old.AcceptCheckout = vmodel.AcceptCheckout;
                old.CustomerSalutation = vmodel.CustomerSalutation;
                old.CustomerTitel = vmodel.CustomerTitel;
                old.CustomerGivenName = vmodel.CustomerGivenName;
                old.CustomerSurName = vmodel.CustomerSurName;
                // old.CustomerEmail = vmodel.CustomerEmail ?? old.CustomerEmail;
                old.BillingStreet1 = vmodel.BillingStreet1;
                old.BillingStreet2 = vmodel.BillingStreet2;
                old.BillingPostcode = vmodel.BillingPostcode;
                old.BillingCity = vmodel.BillingCity;
                return View(old);
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            caseNum = (int)Session["caseNumber"];
            cartNum = (int)Session["cartNum"];
            model = (CaseCartView)Session["cartList"];
            
            model.AcceptCheckout = vmodel.AcceptCheckout;
            model.CustomerSalutation = AnsiString(vmodel.CustomerSalutation);
            model.CustomerTitel = AnsiString(vmodel.CustomerTitel);
            model.CustomerGivenName = AnsiString(vmodel.CustomerGivenName);
            model.CustomerSurName = AnsiString(vmodel.CustomerSurName);
            // model.CustomerEmail = vmodel.CustomerEmail ?? model.CustomerEmail;
            model.BillingStreet1 = AnsiString(vmodel.BillingStreet1);
            model.BillingStreet2 = AnsiString(vmodel.BillingStreet2);
            model.BillingPostcode = AnsiString(vmodel.BillingPostcode);
            model.BillingCity = AnsiString(vmodel.BillingCity);
            model.AddWareItem = vmodel.AddWareItem;
            model.AddWareAmount = vmodel.AddWareAmount;

            Session["cartList"] = model;

            if (addWareButton != null && addWareButton != "")
            {
                decimal amount = model.AddWareAmount;

                //// hinzufügen von material
                //
                Wares w = null;
                long wid = 0;
                if (long.TryParse(model.AddWareItem, out wid) && wid > 0) w = db.Wares.Where(x => x.WareNumber == wid).First();
                CaseCart cart = null;

                if (w != null && amount > 0)
                {
                    //// zu einer Fall-Nummer oder zu einer userid hinzufügen?

                    if (cartNum == 0)
                    {
                        cartNum = (int)NewNumber(db, "CartNummer", "201000", null);
                        model.CartNumber = cartNum;
                        Session["cartNum"] = cartNum;
                        Session["cartList"] = model;
                    }

                    //// hinzufügen
                    //
                    cart = new CaseCart();
                    cart.CartNumber = (int)cartNum;                             // neue CartNummer holen
                    cart.CaseNumber = (int)caseNum;                             // mit dem Fall verbinden
                    cart.UserId = user.Id;                                      // und mit dem User verbinden
                    cart.PaymentNumber = 0;
                    cart.isPayed = false;                                       // noch nicht bezahlt
                    cart.CreateDate = DateTime.Now;                             // Jetzt
                    cart.Amount = amount;                                       // Anzahl
                    cart.WareNumber = w.WareNumber;                             // text setzen
                    cart.WareText = w.Longtext;
                    cart.UnitPrice = (shopKind == SHOPART.PatientShop) ? w.PricePat : w.PriceDoc;
                    cart.Price = Math.Round(amount * cart.UnitPrice, 2);        // preis ausrechnen
                    cart.Tax = w.Tax;                                           // Steuer kopieren
                    cart.Currency = w.Currency ?? "EUR";                        // Währung
                    try
                    {
                        db.CaseCart.Add(cart);
                        result = await db.SaveChangesAsync();

                        MvcApplication.logMsg("addcart done " + result.ToString());
                    }
                    catch (Exception ex)
                    {
                        MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                        result = -1;
                    }
                    cart = null;

                    model.CartList.Clear();
                    model.Total = 0;
                    model.TotalNet = 0;
                    model.Total10 = 0;
                    model.Total20 = 0;
                    carts = GetUnpayedCart(cartNum);
                    if (carts != null && carts.Count() > 0)
                    {
                        foreach (CaseCart c in carts)
                        {
                            model.CartList.Add(c);
                            Decimal inkl = Math.Round(c.Price * (1m + c.Tax / 100m), 2);
                            model.Total += inkl;
                            model.TotalNet += c.Price;
                            if (c.Tax == 20)
                            {
                                model.Total20 += inkl - c.Price;
                            }
                            else if (c.Tax == 10)
                            {
                                model.Total10 += inkl - c.Price;
                            }
                        }
                    }
                    carts = null;

                    model.AddWareAmount = 1;

                    Session["cartList"] = model;
                }
                w = null;
                ModelState.Clear();
                return View(model);
            }

            if (delWareButton != null && delWareButton != "")
            {
                //// wegnehmen von material
                //
                long xid = long.Parse(delWareButton);
                CaseCart aCart = null;
                carts = db.CaseCart.Where(x => x.Id == xid);
                if (carts != null && carts.Count() == 1)
                {
                    aCart = carts.First<CaseCart>();
                    try
                    {
                        db.CaseCart.Remove(aCart);
                        result = await db.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        MvcApplication.logMsg("Exception " + ex + " (" + ((ex.InnerException != null) ? ex.InnerException.ToString() : "") + ")");
                        result = -1;
                    }
                    if (result < 0)
                    {
                        ModelState.Clear();
                        Session.Clear();
                        return RedirectToAction("KeinSpeichern", "Home");
                    }

                    model.CartList.Clear();
                    model.Total = 0;
                    model.TotalNet = 0;
                    model.Total10 = 0;
                    model.Total20 = 0;
                    carts = GetUnpayedCart(cartNum);
                    if (carts != null && carts.Count() > 0)
                    {
                        foreach (CaseCart c in carts)
                        {
                            model.CartList.Add(c);
                            Decimal inkl = Math.Round(c.Price * (1m + c.Tax / 100m), 2);
                            model.Total += inkl;
                            model.TotalNet += c.Price;
                            if (c.Tax == 20)
                            {
                                model.Total20 += inkl - c.Price;
                            }
                            else if (c.Tax == 10)
                            {
                                model.Total10 += inkl - c.Price;
                            }
                        }
                    }
                    carts = null;
                    model.AddWareAmount = 1;
                    Session["cartList"] = model;
                }
                aCart = null;
                carts = null;

                ModelState.Clear();
                return View(model);
            }

            if (!model.AcceptCheckout)
            {
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "Sie müssen die AGB und den Bezahlvorgang akzeptieren!");
                return View(model);
            }

            int jahr= DateTime.Now.Date.Year;
            string reStr = "";
            if (isDeliveryReport)
            {
                string delVal = jahr.ToString("#0") + "1001";
                reStr = "TransaktionsNummerLS" + jahr.ToString("#0");
                renr = (int)NewNumber(db, reStr, delVal, null);
            }
            else
            {
                string reVal = jahr.ToString("#0") + "1001";
                reStr = "TransaktionsNummer" + jahr.ToString("#0");
                renr = (int)NewNumber(db, reStr, reVal, null);
            }

            // OK wollen grundsätzlich zahlen
            // aber nicht falls isDeliveryReport -> dann nur LS erzeugen, keine Zahlung starten!
            //
            if (isDeliveryReport)
            {
                Session["Payment"] = null;
                Session["IsDeliveryReport"] = isDeliveryReport;
                return RedirectToAction("PayCaseCartDone");
            }


            //// Shopper URL etc erzeugen
            //
            model.shopperMerchantDescriptor = "iSmileWebS" + renr.ToString("#0");
            model.shopperMerchantInvId = (isDeliveryReport ? "LS" : "TX") + renr.ToString("#0");

            // neuen Zahldatensatz anlegen

            // 
            Payment newPayment = new Payment();
            newPayment.UserId = user.Id;
            newPayment.CaseNumber = (int)caseNum;
            newPayment.CaseCart = (int)cartNum;
            newPayment.FlexId = model.checkOutID;
            newPayment.StartedTime = DateTime.Now;
            newPayment.Total = model.Total;
            newPayment.IsCasePayment = true;
            newPayment.Status = PaymentStates.Active;
            newPayment.PrepareCheckout = false;
            newPayment.PaymentNumber = renr;
            newPayment.shopperMerchantInvId = model.shopperMerchantInvId;
            newPayment.shopperMerchantDescriptor = model.shopperMerchantDescriptor;
            newPayment.InvoiceDeliveryId = reStr;
            result = 0;
            try
            {
                db.Paymet.Add(newPayment);
                result = await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                result = -1;
            }
            if (result <= 0)
            {
                Session.Clear();
                return RedirectToAction("KeinSpeichern", "Home");
            }

            model.shopperMerchantTransId = newPayment.Id.ToString("#0");

            // Prep Checkout abschicken
            //
            Dictionary<string, dynamic> checkOut = PrepareCheckout(model);
            if (checkOut != null)
            {
                model.checkOutID = checkOut["id"].ToString();
                model.shopperResultUrl = Request.Url.Scheme + "://" + Request.Url.Host + ":" + Request.Url.Port.ToString() + "/Payment/PayCaseCartDone";
                newPayment.FlexId = model.checkOutID;
            }

            if (checkOut == null || model.checkOutID == null || model.checkOutID == "")
            {
                try
                {
                    db.Paymet.Remove(newPayment);
                    result = await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                    result = -1;
                }
                newPayment = null;

                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "Leider hat die Anmeldung zum Bezahlen beim Zahldienstleister nicht funktioniert!");
                return View(model);
            }
            else
            {
                newPayment.PrepareCheckout = true;
                try
                {
                    db.Entry(newPayment).State = EntityState.Modified;
                    result = await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                    result = -1;
                }
            }

            // --> thinortho@outlook.com
            //
            /*
            adminusers = db.Users.Where(x => x.isAdmin == true);
            if (Environment.MachineName.ToLower() != "apps" && adminusers != null)
            {
                string dict = DumpToJson(checkOut);
                foreach (ApplicationUser x in adminusers)
                {
                    UserManager.SendEmail(x.Id, "Eine Fall-Bezahlung wurde gestartet zu " + caseNum.ToString(), "Checkout:" + dict);
                }
            }
            adminusers = null;
            */
            string dict = DumpToJson(checkOut);
            SendeEmail("info@thinortho.com", "Eine Fall-Bezahlung wurde gestartet zu " + caseNum.ToString(), "Checkout:" + dict);
            SendeEmail("info@philippott.eu", "Eine Fall-Bezahlung wurde gestartet zu " + caseNum.ToString(), "Checkout:" + dict);

            Session["Payment"] = newPayment;
            Session["IsDeliveryReport"] = isDeliveryReport;

            newPayment = null;

            // OK tatsächlich Bezahlen eingeleitet
            // payunit.flex
            //
            model = (CaseCartView)Session["cartList"];
            ModelState.Clear();
            return View(model);
        }


        // GET
        [Authorize]
        public async Task<ActionResult> PayCaseCartDone(string resourcePath)
        {
            int result = 0;
            int caseNum = 0;
            int cartNum = 0;
            ///// System.Linq.IQueryable<ApplicationUser> adminusers = null;
            CaseCartView model = null;
            Cases aCase = null;
            IEnumerable<Cases> caseList = null;
            int renr = 0;
            bool isDeliveryReport = false;
            bool isPayed = false;

            ViewBag.Title = "Warenkorb bezahlt";

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (!user.isVerified)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            shopKind = WhichShop(Request);

            if (Session.IsNewSession)
            {
                Session.Clear();
                return RedirectToAction("Index", "Home");
            }

            if (String.IsNullOrWhiteSpace(resourcePath)) resourcePath= "";
            MvcApplication.logMsg("resourcePath=" + resourcePath);

            caseNum = (int)Session["caseNumber"];
            cartNum = (int)Session["cartNum"];
            model = (CaseCartView)Session["cartList"];
            Payment newPayment = (Payment)Session["Payment"];
            isDeliveryReport = (bool)Session["IsDeliveryReport"];

            /*
            {
              "id":"8a82944a522ffd220152331d4a2c7181",
              "paymentType":"PA",
              "paymentBrand":"VISA",
              "amount":"92.00",
              "currency":"EUR",
              "descriptor":"8091.7548.3042 OPP_Channel",
              "result":{
                "code":"000.100.110",
                "description":"Request successfully processed in 'Merchant in Integrator Test Mode'"
              },
              "card":{
                "bin":"420000",
                "last4Digits":"0000",
                "holder":"test mustermann",
                "expiryMonth":"01",
                "expiryYear":"2016"
              },
              "threeDSecure":{
                "eci":"07"
              },
              "risk":{
                "score":"0"
              },
              "buildNumber":"92e783057a826be3947d550955f00dcb0fd8fa4d@2016-01-07 12:17:14 +0000",
              "timestamp":"2016-01-11 23:55:03+0000",
              "ndc":"85C799A41BFECBEA50135475E1D64208.sbg-vm-tx01"
            }
             */
            Dictionary<string, dynamic> checkOutResult = RetrieveCheckout(model, resourcePath);
            /*
            adminusers = db.Users.Where(x => x.isAdmin == true);
            if (Environment.MachineName.ToLower() != "apps" && adminusers != null)
            {
                string dict = DumpToJson(checkOutResult);
                foreach (ApplicationUser x in adminusers)
                {
                    UserManager.SendEmail(x.Id, "Eine Fall-Bezahlung wurde durchgeführt zu " + caseNum.ToString(), "Resultat:" + dict);
                }
            }
            adminusers = null;
            */
            string dict = DumpToJson(checkOutResult);
            SendeEmail("info@thinortho.com", "Eine Fall-Bezahlung wurde durchgeführt zu " + caseNum.ToString(), "Resultat:" + dict);
            SendeEmail("info@philippott.eu", "Eine Fall-Bezahlung wurde durchgeführt zu " + caseNum.ToString(), "Resultat:" + dict);

            if (!isDeliveryReport)
            {
                model.checkOutResultCode = checkOutResult["result"]["code"].ToString();
                model.checkOutResultDescription = checkOutResult["result"]["description"].ToString();

                newPayment.EventTime = DateTime.Now;
                newPayment.ResultCode = model.checkOutResultCode;
                newPayment.ResultCodeMsg = model.checkOutResultDescription;
                newPayment.RetrieveCheckout = true;

                Regex rgx1 = new Regex(@"^(000\.000\.|000\.100\.1|000\.[36])");
                Regex rgx2 = new Regex(@"^(000\.400\.0|000\.400\.100)");
                if (rgx1.IsMatch(model.checkOutResultCode))
                {
                    // Transaction OK
                    // 
                    // CART bezahlt
                    //
                    foreach (CaseCart c in model.CartList)
                    {
                        c.isPayed = true;
                        db.Entry(c).State = EntityState.Modified;
                    }

                    model.checkOutResultCode = "";
                    model.checkOutResultDescription = "Die Bezahlung wurde durchgeführt.";

                    newPayment.isPayed = true;
                    isPayed = true;

                    ViewBag.Title = "Warenkorb bezahlt";
                }
                else if (rgx2.IsMatch(model.checkOutResultCode))
                {
                    // Transaction needs manual info

                    model.checkOutResultCode = "";
                    model.checkOutResultDescription = "Die Bezahlung wurde abgelehnt.";

                    ViewBag.Title = "Bezahlung fehlgeschlagen!";
                }
                else
                {
                    model.checkOutResultCode = "";
                    model.checkOutResultDescription = "Die Bezahlung wurde abgelehnt.";

                    ViewBag.Title = "Bezahlung fehlgeschlagen!";
                }
                rgx1 = null;
                rgx2 = null;

                try
                {
                    db.Entry(newPayment).State = EntityState.Modified;
                    result = await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                    result = -1;
                }
            }
            else
            {
                model.checkOutResultCode = "000.100.110";
                model.checkOutResultDescription = "Lieferschein angelegt.";
                result = 1;

                ViewBag.Title = "Lieferschein angelegt und übermittelt!";
            }

            //// Invoice anlegen
            // 1 x Invoice()
            // n x Detail
            //
            int jahr = DateTime.Now.Date.Year;
            if (isDeliveryReport)
            {
                string reStr = "LieferscheinNummer" + jahr.ToString("#0");
                string reVal = jahr.ToString("#0") + "0001";
                renr = (int)NewNumber(db, reStr, reVal, null);
            }
            else
            {
                string reStr = "RechnungNummer" + jahr.ToString("#0");
                string reVal = jahr.ToString("#0") + "1001";
                renr = (int)NewNumber(db, reStr, reVal, null);
            }

            Invoice inv = new Invoice();
            inv.ReDate = DateTime.Now.Date;
            inv.InvoiceNr = renr;
            if (!isDeliveryReport)
            {
                inv.PaymentId = newPayment.Id;
                inv.PaymentNumber = newPayment.PaymentNumber;
            }
            inv.CaseNumber = 0;
            inv.CaseNumberStr = "";
            if (caseNum != 0 && isPayed)
            {
                caseList= db.Cases.Where(x => x.CaseNumber == caseNum);
                if (caseList != null && caseList.Count() == 1)
                {
                    aCase = caseList.First();
                    inv.CaseNumber = (int)caseNum;
                    inv.CaseNumberStr = aCase.CaseNumberStr;
                    if (aCase.CaseState == CaseStates.Planung || aCase.CaseState == CaseStates.Herstellung)
                    {
                        aCase.CaseState = (aCase.CaseState == CaseStates.Planung) ? CaseStates.PlanungBezahlt : CaseStates.HerstellungBezahlt;
                        db.Entry(aCase).State = EntityState.Modified;
                    }
                }
                caseList = null;
            }

            inv.CustomerSalutation = model.CustomerSalutation;
            inv.CustomerTitel = model.CustomerTitel;
            inv.CustomerGivenName = model.CustomerGivenName;
            inv.CustomerSurName = model.CustomerSurName;
            inv.CustomerEmail = model.CustomerEmail;
            
            inv.BillingStreet1 = model.BillingStreet1;
            inv.BillingStreet2 = model.BillingStreet2;
            inv.BillingPostcode = model.BillingPostcode;
            inv.BillingCity = model.BillingCity;
            inv.BillingCountry = model.BillingCountry;
            
            inv.IsJustDelivery = isDeliveryReport;

            inv.Total = 0;
            inv.TotalNet = 0;
            inv.Tax0 = 0;
            inv.Tax10 = 0;
            inv.Tax20 = 0;
            int linenr = 1;
            foreach (CaseCart c in model.CartList)
            {
                Journal jnl = new Journal();
                jnl.InvoiceNr = renr;
                jnl.LineNr = linenr;
                jnl.Price = c.Price;
                jnl.UnitPrice = c.UnitPrice;
                jnl.Amount = c.Amount;
                jnl.WareNumber = (int)c.WareNumber;
                jnl.Longtext = c.WareText;
                jnl.Currency = c.Currency ?? "EUR";
                jnl.Tax = c.Tax;
                inv.Currency = jnl.Currency;
                Decimal inkl = Math.Round(c.Price * (1m + c.Tax / 100m), 2);
                inv.Total += inkl;
                inv.TotalNet += c.Price;
                if (c.Tax == 20)
                {
                    inv.Tax20 += inkl - c.Price;
                }
                else if (c.Tax == 10)
                {
                    inv.Tax10 += inkl - c.Price;
                }
                linenr += 1;
                db.Journal.Add(jnl);
            }
            db.Invoices.Add(inv);
            try
            {
                result = await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                result = -1;
            }

            if ((result >= model.CartList.Count() + 1) && (isPayed || isDeliveryReport))
            {
                // Rechnung wurde erstellt
                //

                //// Create Crystal Report
                //// send via email
                //
                List<Invoice> table1 = db.Invoices.Where(x => x.InvoiceNr == renr).ToList();
                List<Journal> table2 = db.Journal.Where(x => x.InvoiceNr == renr).ToList();

                ReportClass report = new ReportClass();
                report.FileName = HostingEnvironment.ApplicationPhysicalPath + (isDeliveryReport ? @"bin\Reports\DeliveryReport1.rpt" : @"bin\Reports\InvoiceReport1.rpt");
                report.Load();
                // report.SetDataSource(db);
                report.Database.Tables[0].SetDataSource(table1);
                report.Database.Tables[1].SetDataSource(table2);
                report.RecordSelectionFormula = "{iSmileAlignerTS_Models_Invoice.InvoiceNr} = " + renr.ToString("#0");
                report.SummaryInfo.ReportTitle = (isDeliveryReport ? "iSmile Lieferschein " + renr.ToString("#0") : "iSmile Rechnung " + renr.ToString("#0")) + ".pdf";
                using (MailMessage mm = new MailMessage())
                {
                    mm.From = new MailAddress("rechnung@thinortho.com");
                    mm.To.Add(new MailAddress(model.CustomerEmail));
                    mm.Subject = report.SummaryInfo.ReportTitle + " für Ihren Einkauf im ts-dent Webshop.";
                    string body = "Sehr geehrter Kunde!" + Environment.NewLine + Environment.NewLine +
                        (isDeliveryReport ? "Anbei erhalten Sie den Lieferschein über Ihren Einkauf! Die Rechnung wird separat zugeschickt!" : "Anbei erhalten Sie die Rechnung über Ihren Einkauf!") + Environment.NewLine + Environment.NewLine +
                        "Vielen Dank," + Environment.NewLine + Environment.NewLine +
                        "mit freundlichen Grüßen," + Environment.NewLine + Environment.NewLine +
                        "ts-dent Thomas Schneider" + Environment.NewLine + Environment.NewLine;
                    mm.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Plain));
                    Attachment att = new Attachment(report.ExportToStream(ExportFormatType.PortableDocFormat), MediaTypeNames.Application.Pdf);
                    att.Name = report.SummaryInfo.ReportTitle;
                    mm.Attachments.Add(att);
                    SmtpClient smtp = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                    try
                    {
                        smtp.Credentials = new System.Net.NetworkCredential("azure_fbb8568967aaf7a9cbea09e721ce16e1@azure.com", "045a8079ad05435191efec6d32d12aff");
                        smtp.Send(mm);
                    }
                    catch (Exception ex)
                    {
                        MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                    }
                    smtp.Dispose();
                    smtp = null;

                    mm.To.Clear();
                    mm.To.Add(new MailAddress("info@philippott.eu"));
                    mm.To.Add(new MailAddress("rechnung@thinortho.com"));
                    mm.Subject = "Webshop " + report.SummaryInfo.ReportTitle + " für Kunde " + model.CustomerEmail;
                    smtp = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                    try
                    {
                        smtp.Credentials = new System.Net.NetworkCredential("azure_fbb8568967aaf7a9cbea09e721ce16e1@azure.com", "045a8079ad05435191efec6d32d12aff");
                        smtp.Send(mm);
                    }
                    catch (Exception ex)
                    {
                        MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                    }
                    mm.Dispose();
                    smtp.Dispose();
                    smtp = null;
                }
                report.Dispose();
                report = null;

                table1 = null;
                table2 = null;
            }

            return View(model);
        }
    }
}
