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
using ISmileAlignerTS.Controllers;


namespace iSmileAlignerTS.Controllers
{
    [Authorize]
    public class CaseController : ISmileController
    {
        public CaseController()
        {
        }

        public CaseController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }


        // eigene Methoden
        //
        public string Case2Emailbody(Cases aCase)
        {
            if (aCase == null) return "";
            string result = "";

            result= "intern ID:" + aCase.Id.ToString("#0") + Environment.NewLine
                + "intern Fallnummer:" + aCase.CaseNumber.ToString("#0") + Environment.NewLine
                + "Fall:" + s(aCase.CaseNumberStr) + Environment.NewLine
                + Environment.NewLine
                + "Vorname:" + s(aCase.PatFirstname) + Environment.NewLine
                + "Nachname:" + s(aCase.PatLastName) + Environment.NewLine
                + "Geschlecht:" + s(aCase.PatSex) + Environment.NewLine
                + "Geburtsdatum:" + ((aCase.PatBirthDate == null) ? "" : aCase.PatBirthDate.GetValueOrDefault().ToString(@"yyyy-MM-dd")) + Environment.NewLine
                + "Adresse:" + s(aCase.PatAddress) + Environment.NewLine
                + "PLZ:" + s(aCase.PatZIP) + Environment.NewLine
                + "Ort:" + s(aCase.PatCity) + Environment.NewLine
                + "Land:" + s(aCase.PatCountry) + Environment.NewLine
                + "Tel:" + s(aCase.PatPhone) + Environment.NewLine
                + "Mob:" + s(aCase.PatMobilPhone) + Environment.NewLine
                + "Email:" + s(aCase.PatEmail) + Environment.NewLine
                + Environment.NewLine
                + "OK:" + (aCase.Tooth18 ? "X" : "_")
                        + (aCase.Tooth17 ? "X" : "_")
                        + (aCase.Tooth16 ? "X" : "_")
                        + (aCase.Tooth15 ? "X" : "_")
                        + (aCase.Tooth14 ? "X" : "_")
                        + (aCase.Tooth13 ? "X" : "_")
                        + (aCase.Tooth12 ? "X" : "_")
                        + (aCase.Tooth11 ? "X" : "_")
                        + (aCase.Tooth21 ? "X" : "_")
                        + (aCase.Tooth22 ? "X" : "_")
                        + (aCase.Tooth23 ? "X" : "_")
                        + (aCase.Tooth24 ? "X" : "_")
                        + (aCase.Tooth25 ? "X" : "_")
                        + (aCase.Tooth26 ? "X" : "_")
                        + (aCase.Tooth27 ? "X" : "_")
                        + (aCase.Tooth28 ? "X" : "_") 
                        + Environment.NewLine
                + "UK:" + (aCase.Tooth48 ? "X" : "_")
                        + (aCase.Tooth47 ? "X" : "_")
                        + (aCase.Tooth46 ? "X" : "_")
                        + (aCase.Tooth45 ? "X" : "_")
                        + (aCase.Tooth44 ? "X" : "_")
                        + (aCase.Tooth43 ? "X" : "_")
                        + (aCase.Tooth42 ? "X" : "_")
                        + (aCase.Tooth41 ? "X" : "_")
                        + (aCase.Tooth31 ? "X" : "_")
                        + (aCase.Tooth32 ? "X" : "_")
                        + (aCase.Tooth33 ? "X" : "_")
                        + (aCase.Tooth34 ? "X" : "_")
                        + (aCase.Tooth35 ? "X" : "_")
                        + (aCase.Tooth36 ? "X" : "_")
                        + (aCase.Tooth37 ? "X" : "_")
                        + (aCase.Tooth38 ? "X" : "_") + Environment.NewLine
                + Environment.NewLine
                + "Kommentar:" + s(aCase.TreatComment) + Environment.NewLine
                + Environment.NewLine
                + "Stripping:" + aCase.Stripping.ToString() + Environment.NewLine
                + "Klasse2/3:" + (aCase.AddTreatClassII ? "Ja" : "Nein") + Environment.NewLine
                + "Suspender:" + (aCase.AddTreatSuspender ? "Ja" : "Nein") + Environment.NewLine
                + "Extraktion:" + (aCase.AddTreatExtract ? "Ja" : "Nein") + Environment.NewLine
                + "Retainer:" + (aCase.AddTreatRetainer ? "Ja" : "Nein") + Environment.NewLine
                + "Bleich:" + (aCase.AddTreatRail ? "Ja" : "Nein") + Environment.NewLine
                + "Extrusion:" + (aCase.AddTreatExtrusion ? "Ja" : "Nein") + Environment.NewLine
                + "Knöpfe:" + (aCase.AddTreatButtons ? "Ja" : "Nein") + Environment.NewLine
                + "Pontic:" + (aCase.AddTreatPontic ? "Ja" : "Nein") + Environment.NewLine
                + "Retainer:" + (aCase.AddTreatRetainer ? "Ja" : "Nein") + Environment.NewLine
                + Environment.NewLine
                + "Step 01 OK:" + s(aCase.Step01OK) + Environment.NewLine
                + "Step 01 UK:" + s(aCase.Step01UK) + Environment.NewLine
                + "Step 01 KM:" + s(aCase.Step01Kommentar) + Environment.NewLine
                + "Step 02 OK:" + s(aCase.Step02OK) + Environment.NewLine
                + "Step 02 UK:" + s(aCase.Step02UK) + Environment.NewLine
                + "Step 01 KM:" + s(aCase.Step02Kommentar) + Environment.NewLine
                + "Step 03 OK:" + s(aCase.Step03OK) + Environment.NewLine
                + "Step 03 UK:" + s(aCase.Step03UK) + Environment.NewLine
                + "Step 01 KM:" + s(aCase.Step03Kommentar) + Environment.NewLine
                + "Step 04 OK:" + s(aCase.Step04OK) + Environment.NewLine
                + "Step 04 UK:" + s(aCase.Step04UK) + Environment.NewLine
                + "Step 01 KM:" + s(aCase.Step04Kommentar) + Environment.NewLine
                + "Step 05 OK:" + s(aCase.Step05OK) + Environment.NewLine
                + "Step 05 UK:" + s(aCase.Step05UK) + Environment.NewLine
                + "Step 01 KM:" + s(aCase.Step05Kommentar) + Environment.NewLine
                + "Step 06 OK:" + s(aCase.Step06OK) + Environment.NewLine
                + "Step 06 UK:" + s(aCase.Step06UK) + Environment.NewLine
                + "Step 01 KM:" + s(aCase.Step06Kommentar) + Environment.NewLine
                + "Step 07 OK:" + s(aCase.Step07OK) + Environment.NewLine
                + "Step 07 UK:" + s(aCase.Step07UK) + Environment.NewLine
                + "Step 01 KM:" + s(aCase.Step07Kommentar) + Environment.NewLine
                + "Step 08 OK:" + s(aCase.Step08OK) + Environment.NewLine
                + "Step 08 UK:" + s(aCase.Step08UK) + Environment.NewLine
                + "Step 01 KM:" + s(aCase.Step08Kommentar) + Environment.NewLine
                + "Step 09 OK:" + s(aCase.Step09OK) + Environment.NewLine
                + "Step 09 UK:" + s(aCase.Step09UK) + Environment.NewLine
                + "Step 01 KM:" + s(aCase.Step09Kommentar) + Environment.NewLine
                + "Step 10 OK:" + s(aCase.Step10OK) + Environment.NewLine
                + "Step 10 UK:" + s(aCase.Step10UK) + Environment.NewLine
                + "Step 01 KM:" + s(aCase.Step10Kommentar) + Environment.NewLine
                + "Step 11 OK:" + s(aCase.Step11OK) + Environment.NewLine
                + "Step 11 UK:" + s(aCase.Step11UK) + Environment.NewLine
                + "Step 01 KM:" + s(aCase.Step11Kommentar) + Environment.NewLine
                + "Step 12 OK:" + s(aCase.Step12OK) + Environment.NewLine
                + "Step 12 UK:" + s(aCase.Step12UK) + Environment.NewLine
                + "Step 01 KM:" + s(aCase.Step12Kommentar) + Environment.NewLine
                + Environment.NewLine
                + "Zeitplan:" + s(aCase.Zeitplan) + Environment.NewLine
                + "Privatkommentar:" + s(aCase.PrivatKommentar) + Environment.NewLine
                + Environment.NewLine;
            return result;
        }


        // Fallnummer reformatieren
        // vor allem Blanks entfernen
        // ucase
        public string AdjustCaseNumberStr(string x)
        {
            string result = x.Trim();
            result = result.ToUpper();
            string[] words = result.Split();
            if (words.Count() >= 2)
            {
                result = String.Join(" ", words);
            }
            return result;
        }


        // Kopiere (SQL)FallDaten in View-Falldaten
        //
        private void CopyCase(CaseViewModel d, Cases s)
        {
            d.CaseId = s.Id;
            d.CaseNumber = s.CaseNumber;
            d.DoctorId = s.DoctorId;
            d.CaseState = s.CaseState;
            
            d.Accepted = s.Accepted;
            d.AcceptedDate = s.AcceptedDate;

            d.PatSalutation = s.PatSalutation;
            d.PatTitel = s.PatTitel;
            d.PatFirstname = s.PatFirstname;
            d.PatLastName = s.PatLastName;
            d.PatSex = (string.IsNullOrWhiteSpace(s.PatSex) || s.PatSex != "W") ? "M" : "W";
            d.PatBirthDate = (s.PatBirthDate == null) ? "" : s.PatBirthDate.GetValueOrDefault().ToString(@"d.M.yyyy");
            d.PatAddress = s.PatAddress;
            d.PatZIP = s.PatZIP;
            d.PatCity = s.PatCity;
            d.PatCountry = s.PatCountry;
            d.PatPhone = s.PatPhone;
            d.PatMobilPhone = s.PatMobilPhone;
            d.PatEmail = s.PatEmail;

            d.Tooth11 = s.Tooth11;
            d.Tooth12 = s.Tooth12;
            d.Tooth13 = s.Tooth13;
            d.Tooth14 = s.Tooth14;
            d.Tooth15 = s.Tooth15;
            d.Tooth16 = s.Tooth16;
            d.Tooth17 = s.Tooth17;
            d.Tooth18 = s.Tooth18;
            d.Tooth21 = s.Tooth21;
            d.Tooth22 = s.Tooth22;
            d.Tooth23 = s.Tooth23;
            d.Tooth24 = s.Tooth24;
            d.Tooth25 = s.Tooth25;
            d.Tooth26 = s.Tooth26;
            d.Tooth27 = s.Tooth27;
            d.Tooth28 = s.Tooth28;
            d.Tooth31 = s.Tooth31;
            d.Tooth32 = s.Tooth32;
            d.Tooth33 = s.Tooth33;
            d.Tooth34 = s.Tooth34;
            d.Tooth35 = s.Tooth35;
            d.Tooth36 = s.Tooth36;
            d.Tooth37 = s.Tooth37;
            d.Tooth38 = s.Tooth38;
            d.Tooth41 = s.Tooth41;
            d.Tooth42 = s.Tooth42;
            d.Tooth43 = s.Tooth43;
            d.Tooth44 = s.Tooth44;
            d.Tooth45 = s.Tooth45;
            d.Tooth46 = s.Tooth46;
            d.Tooth47 = s.Tooth47;
            d.Tooth48 = s.Tooth48;

            d.TreatComment = s.TreatComment;

            d.Stripping = s.Stripping;
            d.AddTreatButtons = s.AddTreatButtons;
            d.AddTreatClassII = s.AddTreatClassII;
            d.AddTreatExtract = s.AddTreatExtract;
            d.AddTreatExtrusion = s.AddTreatExtrusion;
            d.AddTreatPontic = s.AddTreatPontic;
            d.AddTreatRail = s.AddTreatRail;
            d.AddTreatRetainer = s.AddTreatRetainer;
            d.AddTreatSuspender = s.AddTreatSuspender;
            d.AddTreatWire = s.AddTreatWire;
            d.AddTreatAttachment = s.AddTreatAttachment;

            d.Step01OK = s.Step01OK;
            d.Step01UK = s.Step01UK;
            d.Step02OK = s.Step02OK;
            d.Step02UK = s.Step02UK;
            d.Step03OK = s.Step03OK;
            d.Step03UK = s.Step03UK;
            d.Step04OK = s.Step04OK;
            d.Step04UK = s.Step04UK;
            d.Step05OK = s.Step05OK;
            d.Step05UK = s.Step05UK;
            d.Step06OK = s.Step06OK;
            d.Step06UK = s.Step06UK;
            d.Step07OK = s.Step07OK;
            d.Step07UK = s.Step07UK;
            d.Step08OK = s.Step08OK;
            d.Step08UK = s.Step08UK;
            d.Step09OK = s.Step09OK;
            d.Step09UK = s.Step09UK;
            d.Step10OK = s.Step10OK;
            d.Step10UK = s.Step10UK;
            d.Step11OK = s.Step11OK;
            d.Step11UK = s.Step11UK;
            d.Step12OK = s.Step12OK;
            d.Step12UK = s.Step12UK;

            d.Step01Kommentar = s.Step01Kommentar;
            d.Step02Kommentar = s.Step02Kommentar;
            d.Step03Kommentar = s.Step03Kommentar;
            d.Step04Kommentar = s.Step04Kommentar;
            d.Step05Kommentar = s.Step05Kommentar;
            d.Step06Kommentar = s.Step06Kommentar;
            d.Step07Kommentar = s.Step07Kommentar;
            d.Step08Kommentar = s.Step08Kommentar;
            d.Step09Kommentar = s.Step09Kommentar;
            d.Step10Kommentar = s.Step10Kommentar;
            d.Step11Kommentar = s.Step11Kommentar;
            d.Step12Kommentar = s.Step12Kommentar;

            d.Zeitplan = s.Zeitplan;
            d.PrivatKommentar = s.PrivatKommentar;


            d.Payment1 = s.Payment1;
            if (s.Payment1Date == null)
            {
                d.Payment1Date = null;
            }
            else
            {
                d.Payment1Date = s.Payment1Date.Value.Date;
            }
            d.Payment2 = s.Payment2;
            if (s.Payment2Date == null)
            {
                d.Payment2Date = null;
            }
            else
            {
                d.Payment2Date = s.Payment2Date.Value.Date;
            }

            d.CaseNumberStr = s.CaseNumberStr;
            d.ErstellungsDatum = s.ErstellungsDatum;

            d.Messages = new List<MessageViewModel>();
            d.Files = new List<CaseFilesList>();
            d.Wares = new List<Wares>();
            d.Carts = new List<CaseCart>();

            d.CurrentMessage = "";

            d.DeleteWord = "";

            d.RowVersion = s.RowVersion;

            d.BestelltWieAngegeben = s.BestelltWieAngegeben;
            d.BestelltWieAngegebenDatum = s.BestelltWieAngegebenDatum;
            d.BestelltWieAngegebenKommentar = s.BestelltWieAngegebenKommentar;
        }


        // Kopiere View-Falldaten in (SQL)FallDaten
        //
        private void CopyCase(Cases d, CaseViewModel s)
        {
//            d.CaseNumber = s.CaseNumber;
//            d.DoctorId = s.DoctorId;
            d.CaseState = s.CaseState;

            d.Accepted = s.Accepted;
            d.AcceptedDate = s.AcceptedDate;

            d.PatSalutation = s.PatSalutation;
            d.PatTitel = s.PatTitel;
            d.PatFirstname = s.PatFirstname;
            d.PatLastName = s.PatLastName;
            d.PatSex = (s.PatSex == null || s.PatSex != "W") ? "M" : "W";

            DateTime adate = DateTime.Now.Date;
            if (GuessDateFromString(s.PatBirthDate, ref adate) == true) d.PatBirthDate = adate;

            d.PatAddress = s.PatAddress;
            d.PatZIP = s.PatZIP;
            d.PatCity = s.PatCity;
            d.PatCountry = s.PatCountry;
            d.PatPhone = s.PatPhone;
            d.PatMobilPhone = s.PatMobilPhone;
            d.PatEmail = s.PatEmail;

            d.Tooth11 = s.Tooth11;
            d.Tooth12 = s.Tooth12;
            d.Tooth13 = s.Tooth13;
            d.Tooth14 = s.Tooth14;
            d.Tooth15 = s.Tooth15;
            d.Tooth16 = s.Tooth16;
            d.Tooth17 = s.Tooth17;
            d.Tooth18 = s.Tooth18;
            d.Tooth21 = s.Tooth21;
            d.Tooth22 = s.Tooth22;
            d.Tooth23 = s.Tooth23;
            d.Tooth24 = s.Tooth24;
            d.Tooth25 = s.Tooth25;
            d.Tooth26 = s.Tooth26;
            d.Tooth27 = s.Tooth27;
            d.Tooth28 = s.Tooth28;
            d.Tooth31 = s.Tooth31;
            d.Tooth32 = s.Tooth32;
            d.Tooth33 = s.Tooth33;
            d.Tooth34 = s.Tooth34;
            d.Tooth35 = s.Tooth35;
            d.Tooth36 = s.Tooth36;
            d.Tooth37 = s.Tooth37;
            d.Tooth38 = s.Tooth38;
            d.Tooth41 = s.Tooth41;
            d.Tooth42 = s.Tooth42;
            d.Tooth43 = s.Tooth43;
            d.Tooth44 = s.Tooth44;
            d.Tooth45 = s.Tooth45;
            d.Tooth46 = s.Tooth46;
            d.Tooth47 = s.Tooth47;
            d.Tooth48 = s.Tooth48;

            d.TreatComment = s.TreatComment;

            d.Stripping = s.Stripping;
            d.AddTreatButtons = s.AddTreatButtons;
            d.AddTreatClassII = s.AddTreatClassII;
            d.AddTreatExtract = s.AddTreatExtract;
            d.AddTreatExtrusion = s.AddTreatExtrusion;
            d.AddTreatPontic = s.AddTreatPontic;
            d.AddTreatRail = s.AddTreatRail;
            d.AddTreatRetainer = s.AddTreatRetainer;
            d.AddTreatSuspender = s.AddTreatSuspender;
            d.AddTreatWire = s.AddTreatWire;
            d.AddTreatAttachment = s.AddTreatAttachment;

            d.Step01OK = s.Step01OK;
            d.Step01UK = s.Step01UK;
            d.Step02OK = s.Step02OK;
            d.Step02UK = s.Step02UK;
            d.Step03OK = s.Step03OK;
            d.Step03UK = s.Step03UK;
            d.Step04OK = s.Step04OK;
            d.Step04UK = s.Step04UK;
            d.Step05OK = s.Step05OK;
            d.Step05UK = s.Step05UK;
            d.Step06OK = s.Step06OK;
            d.Step06UK = s.Step06UK;
            d.Step07OK = s.Step07OK;
            d.Step07UK = s.Step07UK;
            d.Step08OK = s.Step08OK;
            d.Step08UK = s.Step08UK;
            d.Step09OK = s.Step09OK;
            d.Step09UK = s.Step09UK;
            d.Step10OK = s.Step10OK;
            d.Step10UK = s.Step10UK;
            d.Step11OK = s.Step11OK;
            d.Step11UK = s.Step11UK;
            d.Step12OK = s.Step12OK;
            d.Step12UK = s.Step12UK;

            d.Step01Kommentar = s.Step01Kommentar;
            d.Step02Kommentar = s.Step02Kommentar;
            d.Step03Kommentar = s.Step03Kommentar;
            d.Step04Kommentar = s.Step04Kommentar;
            d.Step05Kommentar = s.Step05Kommentar;
            d.Step06Kommentar = s.Step06Kommentar;
            d.Step07Kommentar = s.Step07Kommentar;
            d.Step08Kommentar = s.Step08Kommentar;
            d.Step09Kommentar = s.Step09Kommentar;
            d.Step10Kommentar = s.Step10Kommentar;
            d.Step11Kommentar = s.Step11Kommentar;
            d.Step12Kommentar = s.Step12Kommentar;

            d.Zeitplan = s.Zeitplan;
            d.PrivatKommentar = s.PrivatKommentar;

            d.Payment1 = s.Payment1;
            if (s.Payment1Date == null)
            {
                d.Payment1Date = null;
            }
            else
            {
                d.Payment1Date = s.Payment1Date.Value.Date;
            }
            d.Payment2 = s.Payment2;
            if (s.Payment2Date == null)
            {
                d.Payment2Date = null;
            }
            else
            {
                d.Payment2Date = s.Payment2Date.Value.Date;
            }

            d.CaseNumberStr = s.CaseNumberStr;
            d.ErstellungsDatum = s.ErstellungsDatum;

            d.BestelltWieAngegeben = s.BestelltWieAngegeben;
            d.BestelltWieAngegebenDatum = s.BestelltWieAngegebenDatum;
            d.BestelltWieAngegebenKommentar = s.BestelltWieAngegebenKommentar;
        }

        
        // GET: Case Index List
        [Authorize]
        public async Task<ActionResult> Index()
        {
            Session["caseView"] = null;

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (!user.isVerified)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            ViewBag.isAdmin = user.isAdmin;

            CaseIndexViewModel model = new CaseIndexViewModel
            {
                DoctorName = "" + user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname,
                IsAdmin = user.isAdmin,
                isDoctor = user.isDoctor,
                CasesOpen = 0,
                CasesClosed = 0,
                CasesOpenSum = 0,
                CasesClosedSum = 0,
                CaseList = new List<CaseSmallViewModel>()
            };

            IQueryable<Cases> records = null;
            if (user.isAdmin)
            {
                records = db.Cases.OrderByDescending(x => x.CaseNumber);
            }
            else if (user.isDoctor)
            {
                records = db.Cases.Where(k => k.DoctorId == user.Id).OrderByDescending(x => x.CaseNumber);
            }
            else
            {
                records = db.Cases.Where(k => k.DoctorId == user.Id).OrderByDescending(x => x.CaseNumber);
            }
            if (records != null && records.Count() >= 0)
            {
                foreach (Cases x in records)
                {
                    if (x.CaseState == CaseStates.FallAbgeschlossen)
                    {
                        model.CasesClosed += 1;
                        model.CasesClosedSum += x.Payment1;
                        model.CasesClosedSum += x.Payment2;
                    }
                    else
                    {
                        model.CasesOpen += 1;
                        model.CasesOpenSum += x.Payment1;
                        model.CasesOpenSum += x.Payment2;
                    }

                    CaseSmallViewModel y = new CaseSmallViewModel
                    {
                        CaseId = x.Id,
                        CaseNumber = x.CaseNumber,
                        CaseNumberStr = x.CaseNumberStr,
                        Status = x.CaseState,
                        PatName = s(x.PatFirstname) + " " + s(x.PatLastName),
                        BestelltWieAngegeben = x.BestelltWieAngegeben,
                        BestelltWieAngegebenKommentar = "",
                        //// Doc = !string.IsNullOrWhiteSpace(x.DoctorId) ? "" : null,
                    };
                    if (x.BestelltWieAngegeben > 0)
                    {
                        y.BestelltWieAngegebenKommentar = x.BestelltWieAngegebenDatum.ToString("yyyy-MM-dd HH:mm:ss") + " " + (x.BestelltWieAngegebenKommentar ?? "");
                    }
                    model.CaseList.Add(y);
                }
            }


            return View(model);
        }


        // GET: Case Index List
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Index(string resetButton, string emailListeButton, CaseIndexViewModel model)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (!user.isVerified || !user.isDoctor)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            string search = (model.CaseSearch == null) ? "" : model.CaseSearch.Trim().ToLower();
            if (resetButton != null && resetButton != "") search = "";

            ViewBag.isAdmin = user.isAdmin;

            model.DoctorName = "" + user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname;
            model.IsAdmin = user.isAdmin;
            model.CasesOpen = 0;
            model.CasesClosed = 0;
            model.CasesOpenSum = 0;
            model.CasesClosedSum = 0;
            model.CaseList = new List<CaseSmallViewModel>();

            IQueryable<Cases> records = null;
            if (user.isAdmin)
            {
                records = db.Cases.OrderByDescending(x => x.CaseNumber);
            }
            else if (user.isDoctor)
            {
                records = db.Cases.Where(k => k.DoctorId == user.Id).OrderByDescending(x => x.CaseNumber);
            }

            if (records != null && search != "") records = records.Where(x => x.PatFirstname.Contains(search) ||
                x.PatLastName.Contains(search) ||
                x.CaseNumberStr.Contains(search));

            string emailBody = "";
            if (records != null && records.Count() >= 0)
            {
                foreach (Cases x in records)
                {
                    emailBody = emailBody + Case2Emailbody(x) + Environment.NewLine + Environment.NewLine + "------------------------------------------------------------------------------------------" + Environment.NewLine + Environment.NewLine;

                    if (x.CaseState == CaseStates.FallAbgeschlossen)
                    {
                        model.CasesClosed += 1;
                        model.CasesClosedSum += x.Payment1;
                        model.CasesClosedSum += x.Payment2;
                    }
                    else
                    {
                        model.CasesOpen += 1;
                        model.CasesOpenSum += x.Payment1;
                        model.CasesOpenSum += x.Payment2;
                    }

                    CaseSmallViewModel y = new CaseSmallViewModel();
                    y.CaseId = x.Id;
                    y.CaseNumber = x.CaseNumber;
                    y.CaseNumberStr = x.CaseNumberStr;
                    y.Status = x.CaseState;
                    y.PatName = s(x.PatFirstname) + " " + s(x.PatLastName);
                    y.BestelltWieAngegeben = x.BestelltWieAngegeben;
                    model.CaseList.Add(y);
                }
            }

            if (emailListeButton != null && emailListeButton != "" && emailBody != "")
            {
                System.Linq.IQueryable<ApplicationUser> adminusers = db.Users.Where(x => x.isAdmin == true);
                if (Environment.MachineName.ToLower() != "apps" && adminusers != null)
                {
                    foreach (ApplicationUser x in adminusers)
                    {
                        UserManager.SendEmail(x.Id, "Fall-Liste von " + user.UserName, "Von " + user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname + System.Environment.NewLine + System.Environment.NewLine + emailBody);
                    }
                }
                adminusers = null;
            }

            ModelState.Clear();
            model.CaseSearch = search;

            return View(model);
        }


        // GET: Case Index List
        [Authorize]
        public async Task<ActionResult> PatIndex()
        {
            Session["caseView"] = null;

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (!user.isVerified || !user.isCustomer)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            ViewBag.isAdmin = user.isAdmin;

            CaseIndexViewModel model = new CaseIndexViewModel();

            model.DoctorName = "";
            model.IsAdmin = user.isAdmin;
            model.isDoctor = user.isDoctor;
            model.CasesOpen = 0;
            model.CasesClosed = 0;
            model.CasesOpenSum = 0;
            model.CasesClosedSum = 0;
            model.CaseList = new List<CaseSmallViewModel>();

            IEnumerable<CustomerCases> records = null;
            records = db.CustomerCases.Where(k => k.UserId == user.Id);
            if (records != null && records.Count() >= 0)
            {
                foreach (CustomerCases x in records)
                {
                    Cases record = db.Cases.Find(x.CasesId);
                    if (record != null)
                    {
                        CaseSmallViewModel y = new CaseSmallViewModel();
                        y.CaseId = record.Id;
                        y.CaseNumber = record.CaseNumber;
                        y.CaseNumberStr = record.CaseNumberStr;
                        y.Status = record.CaseState;
                        y.PatName = s(record.PatFirstname) + " " + s(record.PatLastName);
                        y.BestelltWieAngegeben = record.BestelltWieAngegeben;
                        model.CaseList.Add(y);
                    }
                }
            }
            records = null;

            return View(model);
        }


        // GET: Case Message Index List
        [Authorize]
        public async Task<ActionResult> CheckUserMessages()
        {
            Session["caseView"] = null;

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null || !user.isAdmin)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            List<MessageViewModel> model = new List<MessageViewModel>();
            IEnumerable<Messages> records = db.Messages.Where(x => x.isReplied == false).OrderByDescending(x => x.MessageDate).ToList();
            if (records != null && records.Count() > 0)
            {
                foreach (Messages m in records)
                {
                    Cases c = db.Cases.Where(x => x.Id == m.CaseId).First<Cases>();
                    if (c != null)
                    {
                        var u = UserManager.FindById(c.DoctorId);
                        if (u != null)
                        {
                            MessageViewModel msgToShow = new MessageViewModel();
                            msgToShow.CaseNumber = c.CaseNumber.ToString("#0");
                            msgToShow.CaseNumberStr = c.CaseNumberStr;
                            msgToShow.DoctorName = u.Salutation + " " + u.Firstname + " " + u.Lastname;
                            msgToShow.MessageDate = m.MessageDate.ToString("yy-MM-dd HH:mm");
                            msgToShow.Message = m.Msg;
                            model.Add(msgToShow);
                        }
                    }
                }
            }
            records = null;

            return View(model);
        }


        // speichert bild ab - zweimal, einmal klein thumbnail, und einmal original
        //
        private async Task<int> UpdatePicture(HttpPostedFileBase webfile, CaseFileType kind, long CaseId)
        {
            int result = 0;
            bool addOrSave = false;
            int contentLengthThumb = 0;
            IEnumerable<CaseFiles> pics = null;
            CaseFiles pic = null;
            DateTime fdate = DateTime.Now;
            long newFileNum = 0;

            byte[] uploadedImageFile = null;
            using (var reader = new System.IO.BinaryReader(webfile.InputStream))
            {
                uploadedImageFile = reader.ReadBytes(webfile.ContentLength);
            }

            int contentLength = uploadedImageFile.Length;

            MvcApplication.logMsg("UpdatePicture " + webfile.FileName + " " + contentLength.ToString("#0"));

            switch (kind)
            {
                case CaseFileType.P3szFile:
                case CaseFileType.PDFFile:
                case CaseFileType.SltModel:
                case CaseFileType.SltModelOK:
                case CaseFileType.SltModelOKNow:
                case CaseFileType.SltModelUK:
                case CaseFileType.SltModelUKNow:
                case CaseFileType.SltModelOKUK:
                case CaseFileType.SltModelOKUKNow:
                    pics = db.CaseFiles.Where(x => x.CaseId == CaseId && x.FileType == kind && x.isThumbNail == false);
                    pic = null;
                    if (pics == null || pics.Count() == 0)
                    {
                        pic = new CaseFiles();
                        addOrSave = true;
                        MvcApplication.logMsg("updatepic10 add thumb false");
                    }
                    else
                    {
                        pic = pics.First<CaseFiles>();
                        MvcApplication.logMsg("updatepic10 load thumb false");
                    }
                    pic.CaseId = CaseId;
                    pic.FileType = kind;
                    pic.isDeleted = false;
                    pic.isThumbNail = false;
                    pic.Filename = System.IO.Path.GetFileName(webfile.FileName);
                    pic.ContentType = webfile.ContentType;
                    pic.ContentLength = contentLength;
                    pic.FileDate = fdate;
                    pic.Content = uploadedImageFile;
                    result = 0;
                    try
                    {
                        if (addOrSave)
                        {
                            newFileNum = NewNumber(db, "CaseFileNum", "10000", null);
                            pic.FileNum = newFileNum;
                            db.CaseFiles.Add(pic);
                        }
                        else
                        {
                            newFileNum = pic.FileNum;
                            if (newFileNum == 0)
                            {
                                newFileNum = NewNumber(db, "CaseFileNum", "10000", null);
                                pic.FileNum = newFileNum;
                            }
                        }
                        result = await db.SaveChangesAsync();
                        MvcApplication.logMsg("updatepic10 add/save " + result.ToString("#0"));
                    }
                    catch (Exception ex)
                    {
                        MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                        result = -1;
                    }
                    pic = null;
                    pics = null;
                    return result;
            }

            // Bild Größe anpassen, immer Thumbnail erzeugen
            //
            Image picture = Image.FromStream(new System.IO.MemoryStream(uploadedImageFile));
            int newwidth = picture.Width;
            int newheight = picture.Height;

            MvcApplication.logMsg("updatepic 1 " + CaseId.ToString("#0") + " " + kind.ToString() + " " + newwidth.ToString() + " x " + newheight.ToString() + " " + webfile.FileName + " " + webfile.ContentType + " " + webfile.ContentLength.ToString("#0"));

            switch (kind)
            {
                case CaseFileType.Profil:                                   // Profilbilder 4teln
                case CaseFileType.ProfilSmiling:
                case CaseFileType.Portrait:
                case CaseFileType.PortraitSmiling:
                    if (newwidth != 240)
                    {
                        double factor = 240.0 / (double)picture.Width;
                        newwidth = (int)Math.Round((double)picture.Width * factor, MidpointRounding.AwayFromZero);
                        newheight = (int)Math.Round((double)picture.Height * factor, MidpointRounding.AwayFromZero);
                    }
                    break;

                case CaseFileType.ModelBeforeFront:                         // Modelbilder halbe Seite
                case CaseFileType.ModelAfterFront:
                case CaseFileType.ModelBeforeOKUK:
                case CaseFileType.ModelAfterOKUK:
                    if (newwidth != 460)
                    {
                        double factor = 460.0 / (double)picture.Width;
                        newwidth = (int)Math.Round((double)picture.Width * factor, MidpointRounding.AwayFromZero);
                        newheight = (int)Math.Round((double)picture.Height * factor, MidpointRounding.AwayFromZero);
                    }
                    break;

                case CaseFileType.ModelBeforeRight:
                case CaseFileType.ModelAfterRight:
                    if (newwidth != 380)
                    {
                        double factor = 380 / (double)picture.Width;
                        newwidth = (int)Math.Round((double)picture.Width * factor, MidpointRounding.AwayFromZero);
                        newheight = (int)Math.Round((double)picture.Height * factor, MidpointRounding.AwayFromZero);
                    }
                    break;

                case CaseFileType.ModelMandView:                            // Mand./Max. klein
                case CaseFileType.ModelMaxView:
                    if (newwidth != 120)
                    {
                        double factor = 120.0 / (double)picture.Width;
                        newwidth = (int)Math.Round((double)picture.Width * factor, MidpointRounding.AwayFromZero);
                        newheight = (int)Math.Round((double)picture.Height * factor, MidpointRounding.AwayFromZero);
                    }
                    break;

                case CaseFileType.DataPicture:                              // excel bild
                    if (newwidth != 920)
                    {
                        double factor = 920.0 / (double)picture.Width;
                        newwidth = (int)Math.Round((double)picture.Width * factor, MidpointRounding.AwayFromZero);
                        newheight = (int)Math.Round((double)picture.Height * factor, MidpointRounding.AwayFromZero);
                    }
                    break;

                case CaseFileType.DistXRay:                                 // Röntgen 1
                case CaseFileType.PanoXRay:                                 // Röntgen 2
                    if (newwidth != 480)
                    {
                        double factor = 480.0 / (double)picture.Width;
                        newwidth = (int)Math.Round((double)picture.Width * factor, MidpointRounding.AwayFromZero);
                        newheight = (int)Math.Round((double)picture.Height * factor, MidpointRounding.AwayFromZero);
                    }
                    break;

                default:
                    if (newwidth != 120)                                    // Sonstige Thumbnails nicht größer
                    {
                        double factor = 120.0 / (double)picture.Width;
                        newwidth = (int)Math.Round((double)picture.Width * factor, MidpointRounding.AwayFromZero);
                        newheight = (int)Math.Round((double)picture.Height * factor, MidpointRounding.AwayFromZero);
                    }
                    break;
            }

            MvcApplication.logMsg("updatepic 2 " + CaseId.ToString("#0") + " " + kind.ToString() + " " + newwidth.ToString() + " x " + newheight.ToString());

            Bitmap smaller = new Bitmap(newwidth, newheight);
            Graphics g = Graphics.FromImage(smaller);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(picture, 0, 0, newwidth, newheight);
            System.IO.MemoryStream smallerStream = new System.IO.MemoryStream();
            smaller.Save(smallerStream, picture.RawFormat);
            contentLengthThumb = (int)smallerStream.Length;
            byte[] uploadedThumbFile = null;
            using (var reader = new System.IO.BinaryReader(smallerStream))
            {
                smallerStream.Seek(0, System.IO.SeekOrigin.Begin);
                uploadedThumbFile = reader.ReadBytes(contentLengthThumb);
            }
            g.Dispose();
            smaller.Dispose();
            smallerStream.Dispose();
            picture.Dispose();

            // original speichern
            //
            pics = db.CaseFiles.Where(x => x.CaseId == CaseId && x.FileType == kind && x.isThumbNail == false);
            pic = null;
            if (pics == null || pics.Count() == 0)
            {
                pic = new CaseFiles();
                addOrSave = true;
                MvcApplication.logMsg("updatepic add thumb false");
            }
            else
            {
                pic = pics.First<CaseFiles>();
                MvcApplication.logMsg("updatepic load thumb false");
            }
            pic.CaseId = CaseId;
            pic.FileType = kind;
            pic.isDeleted = false;
            pic.isThumbNail = false;
            pic.Filename = System.IO.Path.GetFileName(webfile.FileName);
            pic.ContentType = webfile.ContentType;
            pic.ContentLength = contentLength;
            pic.FileDate = fdate;
            pic.Content = uploadedImageFile;
            result = 0;
            try
            {
                if (addOrSave)
                {
                    newFileNum = NewNumber(db, "CaseFileNum", "10000", null);
                    pic.FileNum = newFileNum;
                    db.CaseFiles.Add(pic);
                }
                else
                {
                    newFileNum = pic.FileNum;
                    if (newFileNum == 0)
                    {
                        newFileNum = NewNumber(db, "CaseFileNum", "10000", null);
                        pic.FileNum = newFileNum;
                    }
                }
                result = await db.SaveChangesAsync();
                MvcApplication.logMsg("updatepic 3 add/save " + result.ToString("#0"));
            }
            catch (Exception ex)
            {
                MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                result = -1;
            }
            pic = null;
            pics = null;


            // thumbnail speichern
            //
            pics = db.CaseFiles.Where(x => x.CaseId == CaseId && x.FileType == kind && x.isThumbNail == true);
            pic = null;
            if (pics == null || pics.Count() == 0)
            {
                pic = new CaseFiles();
                addOrSave = true;
                MvcApplication.logMsg("updatepic add thumb true");
            }
            else
            {
                pic = pics.First<CaseFiles>();
                MvcApplication.logMsg("updatepic load thumb true");
            }
            pic.CaseId = CaseId;
            pic.FileType = kind;
            pic.isDeleted = false;
            pic.isThumbNail = true;
            pic.Filename = System.IO.Path.GetFileName(webfile.FileName);
            pic.ContentType = webfile.ContentType;
            pic.ContentLength = contentLengthThumb;
            pic.FileDate = fdate;
            pic.Content = uploadedThumbFile;
            pic.FileNum = newFileNum;
            result = 0;
            try
            {
                if (addOrSave)
                {
                    db.CaseFiles.Add(pic);
                }
                result = await db.SaveChangesAsync();
                MvcApplication.logMsg("updatepic 4 add/save " + result.ToString("#0"));
            }
            catch (Exception ex)
            {
                MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                result = -1;
            }
            pic = null;
            pics = null;

            return result;
        }


        // speichert bild ab - zweimal, einmal klein thumbnail, und einmal original
        //
        private int AddPicture(HttpPostedFileBase webfile, CaseFileType kind, long CaseId, out long newImgId, out long newThumbId, out string fname, out DateTime fdate)
        {
            int result = 0;
            int contentLengthThumb = 0;
            CaseFiles pic = null;
            fdate = DateTime.Now;
            newImgId = 0;
            newThumbId = 0;
            fname = "";
            long newFileNum = NewNumber(db, "CaseFileNum", "10000", null);

            byte[] uploadedImageFile = null;
            using (var reader = new System.IO.BinaryReader(webfile.InputStream))
            {
                uploadedImageFile = reader.ReadBytes(webfile.ContentLength);
            }
            int contentLength = uploadedImageFile.Length;

            // Bild Größe anpassen, immer Thumbnail erzeugen
            //
            Image picture = Image.FromStream(new System.IO.MemoryStream(uploadedImageFile));
            int newwidth = picture.Width;
            int newheight = picture.Height;

            MvcApplication.logMsg("AddPicture 1 " + CaseId.ToString("#0") + " " + kind.ToString() + " " + newwidth.ToString() + " x " + newheight.ToString() + " " + webfile.FileName + " " + webfile.ContentType + " " + webfile.ContentLength.ToString("#0"));

            switch (kind)
            {
                case CaseFileType.Profil:                                   // Profilbilder 4teln
                case CaseFileType.ProfilSmiling:
                case CaseFileType.Portrait:
                case CaseFileType.PortraitSmiling:
                    if (newwidth != 240)
                    {
                        double factor = 240.0 / (double)picture.Width;
                        newwidth = (int)Math.Round((double)picture.Width * factor, MidpointRounding.AwayFromZero);
                        newheight = (int)Math.Round((double)picture.Height * factor, MidpointRounding.AwayFromZero);
                    }
                    break;

                case CaseFileType.ModelBeforeFront:                         // Modelbilder halbe Seite
                case CaseFileType.ModelAfterFront:
                case CaseFileType.ModelBeforeOKUK:
                case CaseFileType.ModelAfterOKUK:
                    if (newwidth != 460)
                    {
                        double factor = 460.0 / (double)picture.Width;
                        newwidth = (int)Math.Round((double)picture.Width * factor, MidpointRounding.AwayFromZero);
                        newheight = (int)Math.Round((double)picture.Height * factor, MidpointRounding.AwayFromZero);
                    }
                    break;

                case CaseFileType.ModelBeforeRight:
                case CaseFileType.ModelAfterRight:
                    if (newwidth != 380)
                    {
                        double factor = 380 / (double)picture.Width;
                        newwidth = (int)Math.Round((double)picture.Width * factor, MidpointRounding.AwayFromZero);
                        newheight = (int)Math.Round((double)picture.Height * factor, MidpointRounding.AwayFromZero);
                    }
                    break;

                case CaseFileType.ModelMandView:                            // Mand./Max. klein
                case CaseFileType.ModelMaxView:
                    if (newwidth != 120)
                    {
                        double factor = 120.0 / (double)picture.Width;
                        newwidth = (int)Math.Round((double)picture.Width * factor, MidpointRounding.AwayFromZero);
                        newheight = (int)Math.Round((double)picture.Height * factor, MidpointRounding.AwayFromZero);
                    }
                    break;

                case CaseFileType.DataPicture:                              // excel bild
                    if (newwidth != 920)
                    {
                        double factor = 920.0 / (double)picture.Width;
                        newwidth = (int)Math.Round((double)picture.Width * factor, MidpointRounding.AwayFromZero);
                        newheight = (int)Math.Round((double)picture.Height * factor, MidpointRounding.AwayFromZero);
                    }
                    break;

                case CaseFileType.DistXRay:                                 // Röntgen 1
                case CaseFileType.PanoXRay:                                 // Röntgen 2
                    if (newwidth != 480)
                    {
                        double factor = 480.0 / (double)picture.Width;
                        newwidth = (int)Math.Round((double)picture.Width * factor, MidpointRounding.AwayFromZero);
                        newheight = (int)Math.Round((double)picture.Height * factor, MidpointRounding.AwayFromZero);
                    }
                    break;

                default:
                    if (newwidth != 120)                                    // Sonstige Thumbnails nicht größer
                    {
                        double factor = 120.0 / (double)picture.Width;
                        newwidth = (int)Math.Round((double)picture.Width * factor, MidpointRounding.AwayFromZero);
                        newheight = (int)Math.Round((double)picture.Height * factor, MidpointRounding.AwayFromZero);
                    }
                    break;
            }

            MvcApplication.logMsg("AddPicture 2 " + CaseId.ToString("#0") + " " + kind.ToString() + " " + newwidth.ToString() + " x " + newheight.ToString());

            fname = System.IO.Path.GetFileName(webfile.FileName);

            Bitmap smaller = new Bitmap(newwidth, newheight);
            Graphics g = Graphics.FromImage(smaller);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(picture, 0, 0, newwidth, newheight);
            System.IO.MemoryStream smallerStream = new System.IO.MemoryStream();
            smaller.Save(smallerStream, picture.RawFormat);
            contentLengthThumb = (int)smallerStream.Length;
            byte[] uploadedThumbFile = null;
            using (var reader = new System.IO.BinaryReader(smallerStream))
            {
                smallerStream.Seek(0, System.IO.SeekOrigin.Begin);
                uploadedThumbFile = reader.ReadBytes(contentLengthThumb);
            }
            g.Dispose();
            smaller.Dispose();
//            smallerStream.Close();
//            smallerStream.Dispose();
            smallerStream = null;
            picture.Dispose();

            // original speichern
            //
            pic = new CaseFiles();
            pic.CaseId = CaseId;
            pic.FileType = kind;
            pic.isDeleted = false;
            pic.isThumbNail = false;
            pic.Filename = fname;
            pic.ContentType = webfile.ContentType;
            pic.ContentLength = contentLength;
            pic.FileDate = fdate;
            pic.Content = uploadedImageFile;
            pic.FileNum = newFileNum;
            result = 0;
            try
            {
                db.CaseFiles.Add(pic);
                result = db.SaveChanges();
                MvcApplication.logMsg("updatepic3 add/save " + result.ToString("#0"));
                newImgId = pic.Id;
            }
            catch (Exception ex)
            {
                MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                result = -1;
            }
            pic = null;

            // thumbnail speichern
            //
            pic = new CaseFiles();
            pic.CaseId = CaseId;
            pic.FileType = kind;
            pic.isDeleted = false;
            pic.isThumbNail = true;
            pic.Filename = fname;
            pic.ContentType = webfile.ContentType;
            pic.ContentLength = contentLengthThumb;
            pic.FileDate = fdate;
            pic.Content = uploadedThumbFile;
            pic.FileNum = newFileNum;
            result = 0;
            try
            {
                db.CaseFiles.Add(pic);
                result = db.SaveChanges();
                MvcApplication.logMsg("updatepic4 add/save " + result.ToString("#0"));
                newThumbId = pic.Id;
            }
            catch (Exception ex)
            {
                MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                result = -1;
            }
            pic = null;

            return result;
        }


        // speichert bild ab - zweimal, einmal klein thumbnail, und einmal original
        //
        private int AddFile(HttpPostedFileBase webfile, CaseFileType kind, long CaseId, out long newImgId, out string fname, out DateTime fdate)
        {
            int result = 0;
            CaseFiles pic = null;
            fdate = DateTime.Now;
            newImgId = 0;
            fname = "";
            long newFileNum = NewNumber(db, "CaseFileNum", "10000", null);

            byte[] uploadedImageFile = null;
            using (var reader = new System.IO.BinaryReader(webfile.InputStream))
            {
                uploadedImageFile = reader.ReadBytes(webfile.ContentLength);
            }
            int contentLength = uploadedImageFile.Length;

            fname = System.IO.Path.GetFileName(webfile.FileName);

            // original speichern
            //
            pic = new CaseFiles();
            pic.CaseId = CaseId;
            pic.FileType = kind;
            pic.isDeleted = false;
            pic.isThumbNail = false;
            pic.Filename = fname;
            pic.ContentType = webfile.ContentType;
            pic.ContentLength = contentLength;
            pic.FileDate = fdate;
            pic.Content = uploadedImageFile;
            pic.FileNum = newFileNum;
            result = 0;
            try
            {
                db.CaseFiles.Add(pic);
                result = db.SaveChanges();
                MvcApplication.logMsg("updatepic3 add/save " + result.ToString("#0"));
                newImgId = pic.Id;
            }
            catch (Exception ex)
            {
                MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                result = -1;
            }
            pic = null;

            return result;
        }


        // Lädt Bild Infos
        // 
        private int LoadPicture(CaseFileType kind, long CaseId, out string PicName, out long PicId, out long ThumbId)
        {
            int result = 0;

            PicName = "";
            PicId = 0;
            ThumbId = 0;

            MvcApplication.logMsg("loadpic " + kind.ToString() + " " + CaseId.ToString());

            IEnumerable<CaseFiles> pics = db.CaseFiles.Where(x => x.CaseId == CaseId && x.FileType == kind).ToList();
            if (pics != null && pics.Count() >= 1)
            {
                foreach (CaseFiles pic in pics)
                {
                    MvcApplication.logMsg("loadpic cache " + pic.Id.ToString() + " " + pic.Filename + " " + (pic.isThumbNail ? "thumb" : "normal") + pic.ContentType + " " + pic.ContentLength.ToString("#0"));

                    // cache Bild in unserer Session
                    //
                    string SessionPicName = "bild_" + pic.Id.ToString("#0");
                    Session[SessionPicName] = pic;

                    PicName = pic.Filename;
                    if (pic.isThumbNail)
                    {
                        ThumbId = pic.Id;
                    }
                    else
                    {
                        PicId = pic.Id;
                    }
                    result += 1;
                }
            }
            pics = null;

            return result;
        }


        // GET: Picture
        // [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [Authorize]
        public async Task<ActionResult> OpenPicture(string id)
        {
            CaseFiles pic = null;
            string myId = (id == null) ? "" : id;

            MvcApplication.logMsg("openpic " + myId);

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user.isCustomer)
            {
                ; ; // prüfen ob er darf
            }
            else if (!user.isVerified || !user.isDoctor)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            if (myId == "")
            {
                MvcApplication.logMsg("id is null");
                return RedirectToAction("KeinZugriff", "Home");
            }

            int pos = myId.IndexOf(".");
            if (pos >= 0)
            {
                myId = myId.Substring(0, pos);
                MvcApplication.logMsg("openpic shrink id " + myId);
            }

            string SessionPicName = "bild_" + myId;
            pic = (CaseFiles)Session[SessionPicName];
            if (pic != null)
            {
                MvcApplication.logMsg("openpic in der session " + SessionPicName + " " + pic.Filename + " " + pic.ContentType + " " + pic.ContentLength.ToString("#0"));
                return File(pic.Content, pic.ContentType, pic.Filename);
            }

            long xid = Int64.Parse(myId);
            IQueryable<CaseFiles> casefiles = db.CaseFiles.Where(x => x.Id == xid);
            if (casefiles == null || casefiles.Count() != 1)
            {
                MvcApplication.logMsg("openpic pic not found " + id);
                return RedirectToAction("KeinZugriff", "Home");
            }

            pic = casefiles.First<CaseFiles>();

            IQueryable<Cases> cases = db.Cases.Where(x => x.Id == pic.CaseId);
            if (cases == null || cases.Count() != 1)
            {
                MvcApplication.logMsg("id case not found " + pic.CaseId.ToString());
                return RedirectToAction("KeinZugriff", "Home");
            }

            Cases acase = cases.First<Cases>();
            if (user == null || (!user.isAdmin && acase.DoctorId != user.Id))
            {
                MvcApplication.logMsg("id pic no access right");
                return RedirectToAction("KeinZugriff", "Home");
            }

            return File(pic.Content, pic.ContentType, pic.Filename);
        }


        // GET: Picture
        // [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [Authorize]
        public async Task<ActionResult> OpenPictureSaveAs(string id)
        {
            CaseFiles pic = null;
            string myId = (id == null) ? "" : id;

            MvcApplication.logMsg("openpic " + myId);

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user.isCustomer)
            {
                ; ; // prüfen ob er darf
            }
            else if (!user.isVerified || !user.isDoctor)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            if (myId == "")
            {
                MvcApplication.logMsg("id is null");
                return RedirectToAction("KeinZugriff", "Home");
            }

            int pos = myId.IndexOf(".");
            if (pos >= 0)
            {
                myId = myId.Substring(0, pos);
                MvcApplication.logMsg("openpic shrink id " + myId);
            }

            string SessionPicName = "bild_" + myId;
            pic = (CaseFiles)Session[SessionPicName];
            if (pic != null)
            {
                MvcApplication.logMsg("openpic in der session " + SessionPicName + " " + pic.Filename + " " + pic.ContentType + " " + pic.ContentLength.ToString("#0"));
                return File(pic.Content, "application/octet-stream", pic.Filename);
            }

            long xid = Int64.Parse(myId);
            IQueryable<CaseFiles> casefiles = db.CaseFiles.Where(x => x.Id == xid);
            if (casefiles == null || casefiles.Count() != 1)
            {
                MvcApplication.logMsg("openpic pic not found " + id);
                return RedirectToAction("KeinZugriff", "Home");
            }

            pic = casefiles.First<CaseFiles>();

            IQueryable<Cases> cases = db.Cases.Where(x => x.Id == pic.CaseId);
            if (cases == null || cases.Count() != 1)
            {
                MvcApplication.logMsg("id case not found " + pic.CaseId.ToString());
                return RedirectToAction("KeinZugriff", "Home");
            }

            Cases acase = cases.First<Cases>();
            if (user == null || (!user.isAdmin && acase.DoctorId != user.Id))
            {
                MvcApplication.logMsg("id pic no access right");
                return RedirectToAction("KeinZugriff", "Home");
            }

            return File(pic.Content, "application/octet-stream", pic.Filename);
        }


        // GET: Picture
        // [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [Authorize]
        public async Task<ActionResult> OpenModel(string id)
        {
            CaseFiles pic = null;
            string myId = id ?? "";

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user.isCustomer)
            {
                ; ; // prüfen ob er darf
            }
            else if (!user.isVerified || !user.isDoctor)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            if (myId == "")
            {
                MvcApplication.logMsg("id is null");
                return RedirectToAction("KeinZugriff", "Home");
            }

            long xid = Int64.Parse(myId);
            IQueryable<CaseFiles> casefiles = db.CaseFiles.Where(x => x.Id == xid);
            if (casefiles == null || casefiles.Count() != 1)
            {
                MvcApplication.logMsg("openpic pic not found " + id);
                return RedirectToAction("KeinZugriff", "Home");
            }

            pic = casefiles.First<CaseFiles>();

            IQueryable<Cases> cases = db.Cases.Where(x => x.Id == pic.CaseId);
            if (cases == null || cases.Count() != 1)
            {
                MvcApplication.logMsg("id case not found " + pic.CaseId.ToString());
                return RedirectToAction("KeinZugriff", "Home");
            }

            Cases acase = cases.First<Cases>();
            if (user == null || (!user.isCustomer && !user.isAdmin && acase.DoctorId != user.Id))
            {
                MvcApplication.logMsg("id pic no access right");
                return RedirectToAction("KeinZugriff", "Home");
            }

            string SessionPicName = "bild_" + pic.Id.ToString("#0");
            Session[SessionPicName] = pic;

            ViewBag.ModelIdString = xid.ToString("#0");

            return View();
        }


        // GET: Picture
        // [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [Authorize]
        public async Task<ActionResult> OpenModels(string id)
        {
            CaseFiles pic = null;
            string myId = id ?? "";

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user.isCustomer)
            {
                ; ; // prüfen ob er darf
            }
            else if (!user.isVerified || !user.isDoctor)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            if (myId == "")
            {
                MvcApplication.logMsg("id is null");
                return RedirectToAction("KeinZugriff", "Home");
            }

            long xid = Int64.Parse(myId);
            IQueryable<Cases> cases = db.Cases.Where(x => x.Id == xid);
            if (cases == null || cases.Count() != 1)
            {
                MvcApplication.logMsg("id case not found " + pic.CaseId.ToString());
                return RedirectToAction("KeinZugriff", "Home");
            }

            Cases acase = cases.First<Cases>();
            if (user == null || (!user.isAdmin && acase.DoctorId != user.Id))
            {
                MvcApplication.logMsg("id pic no access right");
                return RedirectToAction("KeinZugriff", "Home");
            }

            ///// MOdelle laden und anzeigen
            CaseViewModel model = new CaseViewModel();

            
            return View(model);
        }

        
        public async Task<int> AddCartAsync(CaseViewModel aCase, int warenumber, decimal amount)
        {
            int result = 0;

            IEnumerable<Wares> wares = null;
            Wares ware = null;
            CaseCart cart = null;

            int CartNumber = GetCartNumber(aCase.CaseNumber, null);

            if (CartNumber == 0)
            {
                CartNumber = (int)NewNumber(db, "CartNummer", "201000", null); 
            }

            MvcApplication.logMsg("addcart " + aCase.CaseNumber.ToString("#0") + " " + CartNumber.ToString("#0") + " " + warenumber.ToString("#0") + " " + amount.ToString("#0.00"));

            wares = db.Wares.Where(x => x.WareNumber == warenumber);
            if (wares != null && wares.Count() == 1)
            {
                ware = wares.First<Wares>();
                cart = new CaseCart();
                cart.CartNumber = CartNumber;                               // neue CartNummer holen
                cart.CaseNumber = aCase.CaseNumber;                         // mit dem Fall verbinden
                cart.PaymentNumber = 0;
                cart.isPayed = false;                                       // noch nicht bezahlt
                cart.CreateDate = DateTime.Now;                             // Jetzt
                cart.Amount = amount;                                       // Anzahl
                cart.WareNumber = ware.WareNumber;                          // text setzen
                cart.WareText = ware.Longtext;
                cart.UnitPrice = (shopKind == SHOPART.PatientShop) ? ware.PricePat : ware.PriceDoc;
                cart.Price = Math.Round(amount * cart.UnitPrice, 2);        // preis ausrechnen
                cart.Tax = ware.Tax;                                        // Steuer kopieren
                cart.Currency = ware.Currency;                              // Währung
                cart.UserId = aCase.DoctorId;                               // Arzt
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
            }
            ware = null;
            wares = null;
            cart = null;

            return result;
        }

        // Cart erzeugen
        // OK Zeile eintragen
        // UK Zeile eintragen
        // CaseID verknüpfen
        // 
        // OK UK prüfen, ob die im Cart sind
        // 1010 und 1011, falls nicht, hinzufügen
        // falls doch, und noch nicht bezahlt, entfernen falls nicht mehr da
        // Zusatzbehandlungen prüfen, falls nicht da, hinzufügen
        //
        public async Task<int> UpdateCartInfos(CaseViewModel aCase)
        {
            int result = 0;
            IEnumerable<CaseCart> carts = null;

            // prüfe OK, UK
            bool OK1010 = aCase.Tooth11 || aCase.Tooth12 || aCase.Tooth13 || aCase.Tooth14 || aCase.Tooth15 || aCase.Tooth16 || aCase.Tooth17 || aCase.Tooth18 ||
                aCase.Tooth21 || aCase.Tooth22 || aCase.Tooth23 || aCase.Tooth24 || aCase.Tooth25 || aCase.Tooth26 || aCase.Tooth27 || aCase.Tooth28;

            if (OK1010)
            {
                carts = db.CaseCart.Where(x => x.CaseNumber == aCase.CaseNumber && x.WareNumber == 200).OrderBy(y => y.CartNumber);
                if (carts == null || carts.Count() == 0)
                {
                    // OK Zahlung hinzufügen
                    await AddCartAsync(aCase, 200, 1.0m);
                }
                carts = null;
            }

            bool UK1011 = aCase.Tooth31 || aCase.Tooth32 || aCase.Tooth33 || aCase.Tooth34 || aCase.Tooth35 || aCase.Tooth36 || aCase.Tooth37 || aCase.Tooth38 ||
                aCase.Tooth41 || aCase.Tooth42 || aCase.Tooth43 || aCase.Tooth44 || aCase.Tooth45 || aCase.Tooth46 || aCase.Tooth47 || aCase.Tooth48;
            if (OK1010)
            {
                carts = db.CaseCart.Where(x => x.CaseNumber == aCase.CaseNumber && x.WareNumber == 201).OrderBy(y => y.CartNumber);
                if (carts == null || carts.Count() == 0)
                {
                    // UK Zahlung hinzufügen
                    await AddCartAsync(aCase, 201, 1.0m);
                }
                carts = null;
            }

            return result;
        }


        // GET: NewCase
        //
        [Authorize]
        public async Task<ActionResult> AddNewCase()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            // if (!user.isVerified || !user.isDoctor)
            if (!user.isVerified)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            if (Session.IsNewSession)
            {
                Session.Clear();
                return RedirectToAction("Index", "Case");
            }

            CaseViewModel modelCase = new CaseViewModel();
            modelCase.DoctorId = user.Id.ToString();
            modelCase.CaseId = 0;
            modelCase.CaseNumber = 0;
            modelCase.CaseState = CaseStates.Planung;

            modelCase.PatSalutation = "Hr.";
            modelCase.PatTitel = "";
            modelCase.PatFirstname = "";
            modelCase.PatLastName = "";
            modelCase.PatSex = "M";
            modelCase.PatAddress = "";
            modelCase.PatZIP = "";
            modelCase.PatCity = "";
            modelCase.PatCountry = "";
            modelCase.PatPhone = "";
            modelCase.PatMobilPhone = "";
            modelCase.PatEmail = "";

            if (!user.isDoctor)
            {
                modelCase.PatSalutation = user.Salutation;
                modelCase.PatFirstname = user.Firstname;
                modelCase.PatLastName = user.Lastname;
                modelCase.PatEmail = user.Email;
                modelCase.PatAddress = user.Address;
                modelCase.PatZIP = user.ZIP;
                modelCase.PatCity = user.City;
                modelCase.PatPhone = user.Phone;
                if (user.BirthDate != null) modelCase.PatBirthDate = user.BirthDate.ToString(@"yyyy-MM-dd");
            }

            modelCase.Tooth11 = false;
            modelCase.Tooth12 = false;
            modelCase.Tooth13 = false;
            modelCase.Tooth14 = false;
            modelCase.Tooth15 = false;
            modelCase.Tooth16 = false;
            modelCase.Tooth17 = false;
            modelCase.Tooth18 = false;

            modelCase.Tooth21 = false;
            modelCase.Tooth22 = false;
            modelCase.Tooth23 = false;
            modelCase.Tooth24 = false;
            modelCase.Tooth25 = false;
            modelCase.Tooth26 = false;
            modelCase.Tooth27 = false;
            modelCase.Tooth28 = false;

            modelCase.Tooth31 = false;
            modelCase.Tooth32 = false;
            modelCase.Tooth33 = false;
            modelCase.Tooth34 = false;
            modelCase.Tooth35 = false;
            modelCase.Tooth36 = false;
            modelCase.Tooth37 = false;
            modelCase.Tooth38 = false;

            modelCase.Tooth41 = false;
            modelCase.Tooth42 = false;
            modelCase.Tooth43 = false;
            modelCase.Tooth44 = false;
            modelCase.Tooth45 = false;
            modelCase.Tooth46 = false;
            modelCase.Tooth47 = false;
            modelCase.Tooth48 = false;

            modelCase.Stripping = CaseStripping.Ja;
            modelCase.AddTreatRail = false;
            modelCase.AddTreatButtons = false;
            modelCase.AddTreatClassII = false;
            modelCase.AddTreatExtract = false;
            modelCase.AddTreatExtrusion = false;
            modelCase.AddTreatPontic = false;
            modelCase.AddTreatRetainer = false;
            modelCase.AddTreatSuspender = false;
            modelCase.AddTreatWire = false;
            modelCase.AddTreatAttachment = false;

            modelCase.Payment1 = 0;
            modelCase.Payment1Date = null;
            modelCase.Payment2 = 0;
            modelCase.Payment2Date = null;
            modelCase.Accepted = false;

            // neues projekt leere Listen anlegen
            //
            modelCase.Files = new List<CaseFilesList>();
            modelCase.Messages = new List<MessageViewModel>();
            modelCase.Wares = new List<Wares>();
            modelCase.Carts = new List<CaseCart>();

            Session["caseView"] = modelCase;

            if (!user.isDoctor)
            {
                return View("AddNewCasePat", modelCase);
            }
            return View(modelCase);
        }


        // Post: AddNewCase
        //
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> AddNewCase(string submitButton, CaseViewModel model)
        {
            DateTime adate = DateTime.Now.Date;

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (!user.isVerified)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            if (Session.IsNewSession)
            {
                Session.Clear();
                return RedirectToAction("Index", "Case");
            }

            if (submitButton != null && submitButton.ToLower() == "abbruch")
            {
                Session["caseView"] = null;
                return RedirectToAction("Index", "Case");
            }

            CaseViewModel sCaseModel = (CaseViewModel)Session["caseView"];
            if (sCaseModel == null || sCaseModel.DoctorId != user.Id)
            {
                Session["caseView"] = null;
                return RedirectToAction("Index", "Case");
            }


            if (!ModelState.IsValid || model.Accepted == false)
            {
                ViewBag.ValidationErrors = string.Join(",", ModelState.Values.Where(e => e.Errors.Count > 0).SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToArray());
                return View(model);
            }

            Cases newCase = new Cases();
            newCase.BestelltWieAngegeben = 0;
            newCase.BestelltWieAngegebenDatum = new DateTime(1970, 1, 1);
            newCase.BestelltWieAngegebenKommentar = "";

            int result = 0;
            using (var tr = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    newCase.CaseNumber = (int)NewNumber(db, "FallNummer", "2015101000", tr);
                    newCase.DoctorId = user.Id;
                    newCase.CaseState = CaseStates.Planung;
                    newCase.AcceptedDate = DateTime.Now;

                    newCase.PatFirstname = model.PatFirstname;
                    newCase.PatLastName = model.PatLastName;
                    newCase.PatEmail = model.PatEmail;
                    if (GuessDateFromString(model.PatBirthDate, ref adate)) newCase.PatBirthDate = adate;
                    newCase.PatAddress = model.PatAddress;
                    newCase.PatZIP = model.PatZIP;
                    newCase.PatCity = model.PatCity;
                    newCase.PatSex = (model.PatSex == null || model.PatSex != "W") ? "M" : "W";
                    newCase.PatPhone = model.PatPhone;

                    newCase.Tooth11 = model.Tooth11;
                    newCase.Tooth12 = model.Tooth12;
                    newCase.Tooth13 = model.Tooth13;
                    newCase.Tooth14 = model.Tooth14;
                    newCase.Tooth15 = model.Tooth15;
                    newCase.Tooth16 = model.Tooth16;
                    newCase.Tooth17 = model.Tooth17;
                    newCase.Tooth18 = model.Tooth18;

                    newCase.Tooth21 = model.Tooth21;
                    newCase.Tooth22 = model.Tooth22;
                    newCase.Tooth23 = model.Tooth23;
                    newCase.Tooth24 = model.Tooth24;
                    newCase.Tooth25 = model.Tooth25;
                    newCase.Tooth26 = model.Tooth26;
                    newCase.Tooth27 = model.Tooth27;
                    newCase.Tooth28 = model.Tooth28;

                    newCase.Tooth31 = model.Tooth31;
                    newCase.Tooth32 = model.Tooth32;
                    newCase.Tooth33 = model.Tooth33;
                    newCase.Tooth34 = model.Tooth34;
                    newCase.Tooth35 = model.Tooth35;
                    newCase.Tooth36 = model.Tooth36;
                    newCase.Tooth37 = model.Tooth37;
                    newCase.Tooth38 = model.Tooth38;

                    newCase.Tooth41 = model.Tooth41;
                    newCase.Tooth42 = model.Tooth42;
                    newCase.Tooth43 = model.Tooth43;
                    newCase.Tooth44 = model.Tooth44;
                    newCase.Tooth45 = model.Tooth45;
                    newCase.Tooth46 = model.Tooth46;
                    newCase.Tooth47 = model.Tooth47;
                    newCase.Tooth48 = model.Tooth48;

                    newCase.Stripping = model.Stripping;
                    newCase.AddTreatClassII = model.AddTreatClassII;
                    newCase.AddTreatSuspender = model.AddTreatSuspender;
                    newCase.AddTreatExtract = model.AddTreatExtract;
                    newCase.AddTreatRetainer = model.AddTreatRetainer;
                    newCase.AddTreatRail = model.AddTreatRail;
                    newCase.AddTreatExtrusion = model.AddTreatExtrusion;
                    newCase.AddTreatButtons = model.AddTreatButtons;
                    newCase.AddTreatPontic = model.AddTreatPontic;
                    newCase.AddTreatWire = model.AddTreatWire;
                    newCase.AddTreatAttachment = model.AddTreatAttachment;

                    newCase.Payment1 = 0;
                    newCase.Payment2 = 0;
                    newCase.Payment1Date = null;
                    newCase.Payment2Date = null;

                    newCase.Accepted = model.Accepted;
                    newCase.AcceptedDate = DateTime.Now.Date;
                    newCase.ErstellungsDatum = DateTime.Now.Date;

                    string landstr = (user.CountryCode == null || user.CountryCode.Length != 2) ? "AT" : user.CountryCode;
                    string docname = (user.DocShortName == null || user.DocShortName.Length == 0) ? user.Lastname.ToUpper() : user.DocShortName;
                    if (docname.Length >= 7) docname = docname.Substring(0, 7);
                    int fallstr = NewDailyNumber(db, newCase.ErstellungsDatum, tr);
                    newCase.CaseNumberStr = landstr + " " + docname + " " + fallstr.ToString("0000000000");

                    db.Cases.Add(newCase);
                    result = await db.SaveChangesAsync();
                    tr.Commit();
                }
                catch (Exception ex)
                {
                    tr.Rollback();
                    MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                    result = -1;
                }
            }

            if (result <= 0)
            {
                ; // fehler anzeigen
                return RedirectToAction("KeinSpeichern", "Home");
            }

            model.CaseNumber = newCase.CaseNumber;
            model.CaseId = newCase.Id;

            if (model.CurrentMessage != null && model.CurrentMessage.Length > 0)
            {
                newCase.TreatComment = model.CurrentMessage;

                // nachricht schicken
                //
                Messages newMsg = new Messages();
                newMsg.CaseId = model.CaseId;
                newMsg.FromUserEmail = user.Email;
                newMsg.MessageDate = DateTime.Now;
                newMsg.Msg = model.CurrentMessage;
                newMsg.isReplied = false;
                result = 0;
                try
                {
                    db.Messages.Add(newMsg);
                    result = await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                    result = -1;
                }
            }

            // Bilder speichern
            // Portrait
            //
            if (model.PortraitFotoBin != null && model.PortraitFotoBin.ContentLength > 0)
            {
                await UpdatePicture(model.PortraitFotoBin, CaseFileType.Portrait, model.CaseId);
            }

            // Portrait lächelnd
            //
            if (model.PortraitSmilingFotoBin != null && model.PortraitSmilingFotoBin.ContentLength > 0)
            {
                await UpdatePicture(model.PortraitSmilingFotoBin, CaseFileType.PortraitSmiling, model.CaseId);
            }

            // Profil
            //
            if (model.ProfilFotoBin != null && model.ProfilFotoBin.ContentLength > 0)
            {
                await UpdatePicture(model.ProfilFotoBin, CaseFileType.Profil, model.CaseId);
            }

            // Profil lächeldn
            //
            if (model.ProfilSmilingFotoBin != null && model.ProfilSmilingFotoBin.ContentLength > 0)
            {
                await UpdatePicture(model.ProfilSmilingFotoBin, CaseFileType.ProfilSmiling, model.CaseId);
            }

            // Pano
            //
            if (model.PanoFotoBin != null && model.PanoFotoBin.ContentLength > 0)
            {
                await UpdatePicture(model.PanoFotoBin, CaseFileType.PanoXRay, model.CaseId);
            }

            // Dist Rö
            //
            if (model.DistXFotoBin != null && model.DistXFotoBin.ContentLength > 0)
            {
                await UpdatePicture(model.DistXFotoBin, CaseFileType.DistXRay, model.CaseId);
            }

            // Front Animation Gif
            //
            if (model.FrontAnimationGifBin != null && model.FrontAnimationGifBin.ContentLength > 0)
            {
                await UpdatePicture(model.FrontAnimationGifBin, CaseFileType.AnimationGifFront, model.CaseId);
            }
            // Right Animation Gif
            //
            if (model.SideAnimationGifBin != null && model.SideAnimationGifBin.ContentLength > 0)
            {
                await UpdatePicture(model.SideAnimationGifBin, CaseFileType.AnimationGifSide, model.CaseId);
            }
            // Left Animation Gif
            //
            if (model.OcclAnimationGifBin != null && model.OcclAnimationGifBin.ContentLength > 0)
            {
                await UpdatePicture(model.OcclAnimationGifBin, CaseFileType.AnimationGifOccl, model.CaseId);
            }

            
            /*
            System.Linq.IQueryable<ApplicationUser> adminusers = db.Users.Where(x => x.isAdmin == true);
            if (Environment.MachineName.ToLower() != "apps" && adminusers != null)
            {
                string body = Case2Emailbody(newCase);

                foreach (ApplicationUser x in adminusers)
                {
                    UserManager.SendEmail(x.Id, "Ein neuer Fall wurde angelegt " + model.CaseNumber.ToString("#0") + " von " + user.UserName, "Von " + user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname + System.Environment.NewLine + System.Environment.NewLine + body);
                }
            }
            adminusers = null;
            */
            string body = Case2Emailbody(newCase);
            SendeEmail("info@thinortho.com", "Ein neuer Fall wurde angelegt " + model.CaseNumber.ToString("#0") + " von " + user.UserName, "Von " + user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname + System.Environment.NewLine + System.Environment.NewLine + body);
            SendeEmail("info@philippott.eu", "Ein neuer Fall wurde angelegt " + model.CaseNumber.ToString("#0") + " von " + user.UserName, "Von " + user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname + System.Environment.NewLine + System.Environment.NewLine + body);


            // Ausrechnen ob und wieviel zu zahlen ist
            // Bezahl-Datensatz anlegen
            //

            // Cart erzeugen
            // OK Zeile eintragen
            // UK Zeile eintragen
            // CaseID verknüpfen
            // 
            // OK UK prüfen, ob die im Cart sind
            // 1010 und 1011, falls nicht, hinzufügen
            // falls doch, und noch nicht bezahlt, entfernen falls nicht mehr da
            // Zusatzbehandlungen prüfen, falls nicht da, hinzufügen
            //
            await UpdateCartInfos(model);


            return RedirectToAction("Index", "Case");
        }


        // GET: Case/DeleteCase
        //
        public async Task<ActionResult> DeleteCase(string id)
        {
            int result = 0;

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null || !user.isVerified || !user.isDoctor || !user.isAdmin)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            CaseViewModel sCaseModel = null;
            Session["caseView"] = sCaseModel;

            IQueryable<Cases> dbcases = null;
            Cases dbcase = null;
            try
            {
                int casenr = Int32.Parse(id);
                if (user.isAdmin)
                {
                    dbcases = db.Cases.Where(x => x.CaseNumber == casenr);
                }
                else
                {
                    dbcases = db.Cases.Where(x => x.DoctorId == user.Id && x.CaseNumber == casenr);
                }
                if (dbcases != null && dbcases.Count() == 1)
                {
                    dbcase = dbcases.First<Cases>();
                }
            }
            catch (Exception ex)
            {
                MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                result = -1;
            }

            if (result < 0 || dbcase == null)
            {
                dbcase = null;
                return RedirectToAction("Index", "Case");
            }

            sCaseModel = new CaseViewModel();

            CopyCase(sCaseModel, dbcase);

            dbcase = null;

            sCaseModel.DoctorName = user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname;

            Session["caseView"] = sCaseModel;

            ViewBag.isAdmin = user.isAdmin;

            return View(sCaseModel);
        }


        // POST: EditCase
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> DeleteCase(CaseViewModel model)
        {
            int result = 0;
            ///// System.Linq.IQueryable<ApplicationUser> adminusers = null;

            CaseViewModel sCaseModel = (CaseViewModel)Session["caseView"];

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null || !user.isVerified || !user.isDoctor || !user.isAdmin || sCaseModel == null || model.CaseId != sCaseModel.CaseId)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }
            ViewBag.isAdmin = user.isAdmin;

            if (Session.IsNewSession)
            {
                Session.Clear();
                return RedirectToAction("Index", "Case");
            }

            if (model.DeleteWord != null && model.DeleteWord == "LÖSCHEN")
            {
                MvcApplication.logMsg("lösche fall " + sCaseModel.CaseId.ToString() + " " + sCaseModel.CaseNumber.ToString());

                System.Data.Entity.DbContextTransaction tr = null;
                using (tr = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        IQueryable<Cases> lcases = null;
                        IQueryable<Messages> lmessages = null;
                        IQueryable<CaseFiles> lcasefiles = null;
                        IQueryable<CaseCart> lcasecarts = null;
                        IQueryable<Payment> lpayments = null;

                        lcases = db.Cases.Where(x => x.Id == sCaseModel.CaseId);
                        if (lcases != null && lcases.Count() >= 1)
                        {
                            MvcApplication.logMsg("lösche " + lcases.Count().ToString() + " fälle");
                            db.Cases.RemoveRange(lcases);
                        }
                        lcases = null;

                        lmessages = db.Messages.Where(x => x.CaseId == sCaseModel.CaseId);
                        if (lmessages != null && lmessages.Count() >= 1)
                        {
                            MvcApplication.logMsg("lösche " + lmessages.Count().ToString() + " nachrichten");
                            db.Messages.RemoveRange(lmessages);
                        }
                        lmessages = null;

                        lcasefiles = db.CaseFiles.Where(x => x.CaseId == sCaseModel.CaseId);
                        if (lcasefiles != null && lcasefiles.Count() >= 1)
                        {
                            MvcApplication.logMsg("lösche " + lcasefiles.Count().ToString() + " dateien");
                            db.CaseFiles.RemoveRange(lcasefiles);
                        }
                        lcasefiles = null;

                        lcasecarts = db.CaseCart.Where(x => x.CaseNumber == sCaseModel.CaseNumber);
                        if (lcasecarts != null && lcasecarts.Count() >= 1)
                        {
                            MvcApplication.logMsg("lösche " + lcasecarts.Count().ToString() + " carts");
                            db.CaseCart.RemoveRange(lcasecarts);
                        }
                        lcasecarts = null;

                        lpayments = db.Paymet.Where(x => x.CaseNumber == sCaseModel.CaseNumber);
                        if (lpayments != null && lpayments.Count() >= 1)
                        {
                            MvcApplication.logMsg("lösche " + lpayments.Count().ToString() + " payments");
                            db.Paymet.RemoveRange(lpayments);
                        }
                        lpayments = null;

                        result = await db.SaveChangesAsync();

                        MvcApplication.logMsg("gelöscht " + result.ToString() + " records");

                        tr.Commit();
                    }
                    catch (Exception ex)
                    {
                        tr.Rollback();
                        MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                        result = -1;
                    }
                }
                tr = null;
            }

            if (result >= 1)
            {
                /*
                adminusers = db.Users.Where(x => x.isAdmin == true);
                if (Environment.MachineName.ToLower() != "apps" && adminusers != null)
                {
                    foreach (ApplicationUser x in adminusers)
                    {
                        UserManager.SendEmail(x.Id, "Der Fall " + sCaseModel.CaseNumber.ToString("#0") + " wurde gelöscht von " + user.UserName, "Von " + user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname);
                    }
                }
                adminusers = null;
                */
                SendeEmail("info@thinortho.com", "Der Fall " + sCaseModel.CaseNumber.ToString("#0") + " wurde gelöscht von " + user.UserName, "Von " + user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname);
                SendeEmail("info@philippott.eu", "Der Fall " + sCaseModel.CaseNumber.ToString("#0") + " wurde gelöscht von " + user.UserName, "Von " + user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname);
            }

            return RedirectToAction("Index", "Case");
        }


        // GET: Case
        // [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [Authorize]
        public async Task<ActionResult> ViewCaseFiles(string id)
        {
            MvcApplication.logMsg("viewcasefiles: " + ((id == null) ? "null" : id));

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (!user.isVerified || !user.isDoctor)
            {
                MvcApplication.logMsg("wrong user");
                return RedirectToAction("KeinZugriff", "Home");
            }

            CaseViewModel model = (CaseViewModel)Session["caseView"];
            if (model == null || model.CaseId.ToString() != id)
            {
                MvcApplication.logMsg("wrong model");
                return RedirectToAction("KeinZugriff", "Home");
            }

            model.Files = new List<CaseFilesList>();
            CaseFilesList item = null;
            string lastCmp = "";
            string newCmp = "";
            IEnumerable<CaseFiles> pics = db.CaseFiles.Where(x => x.CaseId == model.CaseId).OrderBy(y => y.FileType).ThenBy(y => y.FileNum).ThenBy(z => z.FileDate).ThenBy(z => z.Filename).ThenBy(z => z.isThumbNail).ToList();
            if (pics != null && pics.Count() > 0)
            {
                MvcApplication.logMsg("view pics: " + pics.Count().ToString("#0"));

                for (int i = 0; i < pics.Count(); i += 1)
                {
                    CaseFiles pic = pics.ElementAt(i);

                    MvcApplication.logMsg("pics-x: " + i.ToString("#0") + " " + pic.FileNum.ToString("#0") + " " + pic.Id.ToString("#0") + " " + pic.Filename + " " + (pic.isThumbNail ? "(t)" : "(n)"));

                    newCmp = pic.CaseId.ToString("#0") + "." + pic.FileType.ToString() + "." + pic.FileNum.ToString("#0") + "." + pic.Filename;

                    MvcApplication.logMsg("cmp <" + newCmp + "> <" + lastCmp + ">");

                    if (newCmp != lastCmp)
                    {
                        lastCmp = newCmp;
                        if (item != null)
                        {
                            model.Files.Add(item);
                        }
                        item = null;
                    }
                    else
                    {
                        if (item.ThumbId != 0 && item.ImageId != 0)
                        {
                            model.Files.Add(item);
                            item = null;
                        }
                    }

                    string SessionPicName = "bild_" + pic.Id.ToString("#0");
                    Session[SessionPicName] = pic;

                    if (item == null)
                    {
                        item = new CaseFilesList();
                        item.ImageId = 0;
                        item.ThumbId = 0;
                    }

                    item.FileDate = pic.FileDate.GetValueOrDefault(DateTime.Now);
                    item.Filename = pic.Filename;
                    item.FileType = pic.FileType;

                    if (pic.isThumbNail)
                    {
                        item.ThumbId = pic.Id;
                    }
                    else
                    {
                        item.ImageId = pic.Id;
                    }
                    pic = null;
                }

                if (item != null && (item.ThumbId != 0 || item.ImageId != 0)) model.Files.Add(item);

                item = null;

                MvcApplication.logMsg("files items: " + model.Files.Count().ToString());
            }
            pics = null;

            MvcApplication.logMsg("here 1");

            ViewBag.isAdmin = user.isAdmin;
            ViewBag.isDoctor = user.isDoctor;

            MvcApplication.logMsg("here 2");

            return View(model);
        }


        // POST: Case
        // [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> ViewCaseFiles(string delButton, string addAnyFileButton, CaseViewModel modelx)
        {
            int result = 0;

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (!user.isVerified || !user.isDoctor)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            ViewBag.isAdmin = user.isAdmin;
            ViewBag.isDoctor = user.isDoctor;

            CaseViewModel model = (CaseViewModel)Session["caseView"];
            if (model == null)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            if (user.isAdmin && delButton != null && delButton != "")
            {
                delButton = delButton.Trim();

                long id1 = 0;
                long id2 = 0;
                int pos = delButton.IndexOf(",");
                if (pos >= 0)
                {
                    id1 = Int32.Parse(delButton.Substring(0, pos));
                    id2 = Int32.Parse(delButton.Substring(pos + 1));
                }
                else
                {
                    id1 = Int32.Parse(delButton);
                }

                try
                {
                    CaseFiles pic1 = null;
                    CaseFiles pic2 = null;
                    if (id1 != 0)
                    {
                        pic1 = db.CaseFiles.Where(x => x.Id == id1).First<CaseFiles>();
                        db.CaseFiles.Remove(pic1);
                        pic1 = null;
                        foreach (CaseFilesList x in model.Files)
                        {
                            if (x.ImageId == id1)
                            {
                                model.Files.Remove(x);
                                Session["caseView"] = model;
                                break;
                            }
                        }
                    }
                    if (id2 != 0)
                    {
                        pic2 = db.CaseFiles.Where(x => x.Id == id2).First<CaseFiles>();
                        db.CaseFiles.Remove(pic2);
                        pic2 = null;
                        foreach (CaseFilesList x in model.Files)
                        {
                            if (x.ThumbId == id2)
                            {
                                model.Files.Remove(x);
                                Session["caseView"] = model;
                                break;
                            }
                        }
                    }
                    result = await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    result = -1;
                    MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                }
            }

            if (addAnyFileButton != null && addAnyFileButton != "" && modelx.AddAnyFileBin != null)
            {
                // addAnyFileButton hinzufügen
                string fname = "";
                long thumbId = 0;
                long imgId = 0;
                DateTime fdate = DateTime.Now;

                if (modelx.AddAnyFileBin != null && modelx.AddAnyFileBin.ContentLength > 0)
                {
                    result = AddFile(modelx.AddAnyFileBin, CaseFileType.AnyFile, model.CaseId, out imgId, out fname, out fdate);
                    if (result >= 1)
                    {
                        // Update Cache/Liste (sessionCase)
                        CaseFilesList item = new CaseFilesList();

                        item.FileDate = fdate;
                        item.Filename = fname;
                        item.FileType = CaseFileType.AnyFile;
                        item.ThumbId = thumbId;
                        item.ImageId = imgId;

                        model.Files.Add(item);
                        Session["caseView"] = model;
                    }
                }
                modelx = model;
                ModelState.Clear();
                return View(modelx);
            }

            ModelState.Clear();
            return View(model);
        }


        // GET: Case
        // [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [Authorize]
        public async Task<ActionResult> Bestaetigung(string id)
        {
            int result = 0;

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (!user.isVerified)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            if (Session.IsNewSession)
            {
                Session.Clear();
                return RedirectToAction("Index", "Case");
            }

            // id laden
            //
            CaseViewModel sCaseModel = null;

            // bisherige infos löschen
            //
            Session["caseViewSmall"] = sCaseModel;

            IQueryable<Cases> dbcases = null;
            Cases dbcase = null;
            try
            {
                int casenr = Int32.Parse(id);
                if (user.isAdmin)
                {
                    dbcases = db.Cases.Where(x => x.CaseNumber == casenr);
                }
                else
                {
                    dbcases = db.Cases.Where(x => x.DoctorId == user.Id && x.CaseNumber == casenr);
                }
                if (dbcases != null && dbcases.Count() == 1)
                {
                    dbcase = dbcases.First<Cases>();
                }
            }
            catch (Exception ex)
            {
                MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                result = -1;
            }

            if (result < 0 || dbcase == null)
            {
                dbcase = null;
                return RedirectToAction("Index", "Case");
            }

            string body = Case2Emailbody(dbcase);
            SendeEmail("info@thinortho.com", "Ein Fall wurde geöffnet " + dbcase.CaseNumber.ToString("#0") + " von " + user.UserName, "Von " + user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname + System.Environment.NewLine + System.Environment.NewLine + body);
            SendeEmail("info@philippott.eu", "Ein Fall wurde geöffnet " + dbcase.CaseNumber.ToString("#0") + " von " + user.UserName, "Von " + user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname + System.Environment.NewLine + System.Environment.NewLine + body);

            sCaseModel = new CaseViewModel();

            CopyCase(sCaseModel, dbcase);

            if (sCaseModel.BestelltWieAngegeben == 0)
            {
                sCaseModel.BestelltWieAngegebenDatum = DateTime.Now.Date;
                sCaseModel.BestelltWieAngegeben = 1;
                sCaseModel.BestelltWieAngegebenKommentar = "";
            }

            ApplicationUser docUser = await UserManager.FindByIdAsync(dbcase.DoctorId);
            sCaseModel.DoctorName = docUser.Salutation + " " + docUser.Titel + " " + docUser.Firstname + " " + docUser.Lastname;
            docUser = null;

            //
            // LoadPicture(scaseModel, dbcase.Id.ToString, 
            // profil & portrait
            //
            long xid = 0;
            long tid = 0;
            string xname = null;
            LoadPicture(CaseFileType.Portrait, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.PortraitFotoId = xid;
            sCaseModel.PortraitThumbId = tid;
            sCaseModel.PortraitFotoName = xname;

            LoadPicture(CaseFileType.PortraitSmiling, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.PortraitSmilingFotoId = xid;
            sCaseModel.PortraitSmilingThumbId = tid;
            sCaseModel.PortraitSmilingFotoName = xname;

            LoadPicture(CaseFileType.Profil, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ProfilFotoId = xid;
            sCaseModel.ProfilThumbId = tid;
            sCaseModel.ProfilFotoName = xname;

            LoadPicture(CaseFileType.ProfilSmiling, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ProfilSmilingFotoId = xid;
            sCaseModel.ProfilSmilingThumbId = tid;
            sCaseModel.ProfilSmilingFotoName = xname;

            // nachrichten laden
            //
            sCaseModel.Messages = new List<MessageViewModel>();
            IEnumerable<Messages> msgs = db.Messages.Where(x => x.CaseId == sCaseModel.CaseId).OrderByDescending(x => x.MessageDate).ToList();
            if (msgs != null && msgs.Count() > 0)
            {
                foreach (Messages item in msgs)
                {
                    MessageViewModel m = new MessageViewModel();
                    m.MessageDate = item.MessageDate.ToString("yy-MM-dd HH:mm");
                    m.FromUserEmail = item.FromUserEmail;
                    m.ToUserEmail = item.ToUserEmail;
                    m.Message = item.Msg;
                    sCaseModel.Messages.Add(m);
                }
            }
            msgs = null;

            Session["caseViewSmall"] = sCaseModel;

            ViewBag.isAdmin = user.isAdmin;

            return View(sCaseModel);
        }


        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Bestaetigung(CaseViewModel model)
        {
            int result = 0;
            DateTime adate = DateTime.Now.Date;
            Cases dbcase = null;

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (!user.isVerified)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }
            ViewBag.isAdmin = user.isAdmin;

            if (Session.IsNewSession)
            {
                Session.Clear();
                return RedirectToAction("Index", "Case");
            }

            CaseViewModel sessionCase = (CaseViewModel)Session["caseViewSmall"];
            if (sessionCase == null)
            {
                Session["caseViewSmall"] = null;
                Session.Clear();
                ModelState.Clear();
                return RedirectToAction("Index", "Case");
            }

            if (model == null || !ModelState.IsValid)
            {
                Session["caseViewSmall"] = null;
                Session.Clear();
                ModelState.Clear();
                return RedirectToAction("Index", "Case");
            }

            if (model.CaseId != sessionCase.CaseId || model.CaseNumber != sessionCase.CaseNumber || model.CaseNumber != sessionCase.CaseNumber)
            {
                Session["caseViewSmall"] = null;
                Session.Clear();
                ModelState.Clear();
                return RedirectToAction("Index", "Case");
            }

            System.Data.Entity.DbContextTransaction tr = null;
            using (tr = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    dbcase = db.Cases.Find(sessionCase.CaseId);
                    if (dbcase != null)
                    {
                        dbcase.BestelltWieAngegeben = model.BestelltWieAngegeben;
                        dbcase.BestelltWieAngegebenDatum = DateTime.Now.Date;
                        dbcase.BestelltWieAngegebenKommentar = model.BestelltWieAngegebenKommentar;

                        // dbcase.RowVersion = sessionCase.RowVersion;
                        db.Entry(dbcase).OriginalValues["RowVersion"] = sessionCase.RowVersion;

                        MvcApplication.logMsg("case " + dbcase.Id.ToString("#0") + " rowversion = " + dbcase.RowVersion.ToArray().ToString());

                        result = await db.SaveChangesAsync();
                        tr.Commit();

                        MvcApplication.logMsg("case " + dbcase.Id.ToString("#0") + " updated rowversion = " + dbcase.RowVersion.ToArray().ToString());
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    tr.Rollback();

                    MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                    result = -1;

                    ViewBag.ErrorExceptionString = "Ihre Eingaben wurden zwischenzeitlich durch die Bearbeitung dieses Falls durch einen anderen Anwender verworfen! Bitte öffnen Sie den Fall erneut über die Übersicht, um die aktuellen Daten oder Bilder zu sehen und ändern zu können.";
                }
                catch (RetryLimitExceededException ex)
                {
                    tr.Rollback();

                    MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                    result = -1;

                    ViewBag.ErrorExceptionString = "Ihre Eingaben wurden zwischenzeitlich durch die Bearbeitung dieses Falls durch einen anderen Anwender verworfen! Bitte öffnen Sie den Fall erneut über die Übersicht, um die aktuellen Daten oder Bilder zu sehen und ändern zu können.";
                }
                catch (Exception ex)
                {
                    tr.Rollback();

                    MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                    result = -1;

                    ViewBag.ErrorExceptionString = "Ihre Eingaben wurden zwischenzeitlich durch die Bearbeitung dieses Falls durch einen anderen Anwender verworfen! Bitte öffnen Sie den Fall erneut über die Übersicht, um die aktuellen Daten oder Bilder zu sehen und ändern zu können.";
                }
            }
            tr = null;

            Session.Clear();

            return RedirectToAction("Index", "Case");
        }

        
        // GET: Case
        // [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [Authorize]
        public async Task<ActionResult> EditCase(string id)
        {
            int result = 0;
            ///// System.Linq.IQueryable<ApplicationUser> adminusers = null;

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (!user.isVerified)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            if (Session.IsNewSession)
            {
                Session.Clear();
                return RedirectToAction("Index", "Case");
            }

            // id laden
            //
            CaseViewModel sCaseModel = null;
            
            // bisherige infos löschen
            //
            Session["caseView"] = sCaseModel;

            IQueryable<Cases> dbcases = null;
            Cases dbcase = null;
            try
            {
                int casenr = Int32.Parse(id);
                if (user.isAdmin)
                {
                    dbcases = db.Cases.Where(x => x.CaseNumber == casenr);
                }
                else
                {
                    dbcases = db.Cases.Where(x => x.DoctorId == user.Id && x.CaseNumber == casenr);
                }
                if (dbcases != null && dbcases.Count() == 1)
                {
                    dbcase = dbcases.First<Cases>();
                }
            }
            catch (Exception ex)
            {
                MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                result = -1;
            }

            if (result < 0 || dbcase == null)
            {
                dbcase = null;
                return RedirectToAction("Index", "Case");
            }

            /*
            adminusers = db.Users.Where(x => x.isAdmin == true);
            if (Environment.MachineName.ToLower() != "apps" && adminusers != null)
            {
                string body = Case2Emailbody(dbcase);

                foreach (ApplicationUser x in adminusers)
                {
                    UserManager.SendEmail(x.Id, "Ein Fall wurde geöffnet " + dbcase.CaseNumber.ToString("#0") + " von " + user.UserName, "Von " + user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname + System.Environment.NewLine + System.Environment.NewLine + body);
                }
            }
            adminusers = null;
            */
            string body = Case2Emailbody(dbcase);
            SendeEmail("info@thinortho.com", "Ein Fall wurde geöffnet " + dbcase.CaseNumber.ToString("#0") + " von " + user.UserName, "Von " + user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname + System.Environment.NewLine + System.Environment.NewLine + body);
            SendeEmail("info@philippott.eu", "Ein Fall wurde geöffnet " + dbcase.CaseNumber.ToString("#0") + " von " + user.UserName, "Von " + user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname + System.Environment.NewLine + System.Environment.NewLine + body);


            sCaseModel = new CaseViewModel();

            CopyCase(sCaseModel, dbcase);

            ApplicationUser docUser = await UserManager.FindByIdAsync(dbcase.DoctorId);
            sCaseModel.DoctorName = docUser.Salutation + " " + docUser.Titel + " " + docUser.Firstname + " " + docUser.Lastname;
            docUser = null;

            ViewBag.isAdmin = user.isAdmin;
            ViewBag.isDoctor = user.isDoctor;
            ViewBag.isCustomer = user.isCustomer;

            //
            // LoadPicture(scaseModel, dbcase.Id.ToString, 
            // profil & portrait
            //
            long xid = 0;
            long tid = 0;
            string xname = null;
            LoadPicture(CaseFileType.Portrait, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.PortraitFotoId = xid;
            sCaseModel.PortraitThumbId = tid;
            sCaseModel.PortraitFotoName = xname;

            LoadPicture(CaseFileType.PortraitSmiling, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.PortraitSmilingFotoId = xid;
            sCaseModel.PortraitSmilingThumbId = tid;
            sCaseModel.PortraitSmilingFotoName = xname;

            LoadPicture(CaseFileType.Profil, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ProfilFotoId = xid;
            sCaseModel.ProfilThumbId = tid;
            sCaseModel.ProfilFotoName = xname;

            LoadPicture(CaseFileType.ProfilSmiling, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ProfilSmilingFotoId = xid;
            sCaseModel.ProfilSmilingThumbId = tid;
            sCaseModel.ProfilSmilingFotoName = xname;

            // Rö
            //
            LoadPicture(CaseFileType.PanoXRay, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.PanoFotoId = xid;
            sCaseModel.PanoThumbId = tid;
            sCaseModel.PanoFotoName = xname;

            LoadPicture(CaseFileType.DistXRay, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.DistXFotoId = xid;
            sCaseModel.DistXThumbId = tid;
            sCaseModel.DistXFotoName = xname;

            // Data
            //
            LoadPicture(CaseFileType.DataPicture, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.DataFotoId = xid;
            sCaseModel.DataThumbId = tid;
            sCaseModel.DataFotoName = xname;

            // Model before, after, right before, max, mand, right after, ok uk before, ok uk after
            //
            LoadPicture(CaseFileType.ModelBeforeFront, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelBeforeFrontFotoId = xid;
            sCaseModel.ModelBeforeFrontThumbId = tid;
            sCaseModel.ModelBeforeFrontFotoName = xname;

            LoadPicture(CaseFileType.ModelAfterFront, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelAfterFrontFotoId = xid;
            sCaseModel.ModelAfterFrontThumbId = tid;
            sCaseModel.ModelAfterFrontFotoName = xname;

            LoadPicture(CaseFileType.ModelBeforeRight, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelBeforeRightFotoId = xid;
            sCaseModel.ModelBeforeRightThumbId = tid;
            sCaseModel.ModelBeforeRightFotoName = xname;

            LoadPicture(CaseFileType.ModelMaxView, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelMaxFotoId = xid;
            sCaseModel.ModelMaxThumbId = tid;
            sCaseModel.ModelMaxFotoName = xname;

            LoadPicture(CaseFileType.ModelMandView, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelMandFotoId = xid;
            sCaseModel.ModelMandThumbId = tid;
            sCaseModel.ModelMandFotoName = xname;

            LoadPicture(CaseFileType.ModelAfterRight, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelAfterRightFotoId = xid;
            sCaseModel.ModelAfterRightThumbId = tid;
            sCaseModel.ModelAfterRightFotoName = xname;

            LoadPicture(CaseFileType.ModelBeforeOKUK, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelBeforeOKUKFotoId = xid;
            sCaseModel.ModelBeforeOKUKThumbId = tid;
            sCaseModel.ModelBeforeOKUKFotoName = xname;

            LoadPicture(CaseFileType.ModelAfterOKUK, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelAfterOKUKFotoId = xid;
            sCaseModel.ModelAfterOKUKThumbId = tid;
            sCaseModel.ModelAfterOKUKFotoName = xname;

            sCaseModel.ShowCanvasModels = false;


            LoadPicture(CaseFileType.SltModelOK, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelBeforeOKSltId = xid;
            sCaseModel.ModelBeforeOKSltName = xname;

            LoadPicture(CaseFileType.SltModelOKNow, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelAfterOKSltId = xid;
            sCaseModel.ModelAfterOKSltName = xname;

            LoadPicture(CaseFileType.SltModelOKUK, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelBeforeOKUKSltId = xid;
            sCaseModel.ModelBeforeOKUKSltName = xname;

            LoadPicture(CaseFileType.SltModelUK, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelBeforeUKSltId = xid;
            sCaseModel.ModelBeforeUKSltName = xname;

            LoadPicture(CaseFileType.SltModelUKNow, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelAfterUKSltId = xid;
            sCaseModel.ModelAfterUKSltName = xname;

            LoadPicture(CaseFileType.SltModelUKNow, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelAfterUKSltId = xid;
            sCaseModel.ModelAfterUKSltName = xname;

            LoadPicture(CaseFileType.SltModelOKUKNow, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelAfterOKUKSltId = xid;
            sCaseModel.ModelAfterOKUKSltName = xname;

            LoadPicture(CaseFileType.SltModelOKUKNow, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelAfterOKUKSltId = xid;
            sCaseModel.ModelAfterOKUKSltName = xname;


            LoadPicture(CaseFileType.PDFFile, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.PDFDocumentID = xid;
            sCaseModel.PDFDocumentFilename = xname;

            LoadPicture(CaseFileType.P3szFile, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ProjektID = xid;
            sCaseModel.ProjektFilename = xname;

            LoadPicture(CaseFileType.AnimationGifFront, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.FrontAnimationGifID = xid;
            sCaseModel.FrontAnimationGifFilename = xname;

            LoadPicture(CaseFileType.AnimationGifSide, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.SideAnimationGifID = xid;
            sCaseModel.SideAnimationGifFilename = xname;

            LoadPicture(CaseFileType.AnimationGifOccl, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.OcclAnimationGifID = xid;
            sCaseModel.OcclAnimationGifFilename = xname;

            // bilder laden
            // Dist, Pano, Any
            sCaseModel.Files = new List<CaseFilesList>();
            //
            //    IEnumerable<CaseFiles> pics  = db.CaseFiles.Where(x => x.CaseId == sCaseModel.CaseId && x.FileType == CaseFileType.AnyPhoto).OrderBy(y => y.FileNum).OrderBy(y => y.FileDate).ThenBy(z => z.isThumbNail);
            IEnumerable<CaseFiles> pics = db.CaseFiles.Where(x => x.CaseId == sCaseModel.CaseId &&
                (x.FileType == CaseFileType.AnyPhoto ||
                x.FileType == CaseFileType.AnyModelOK ||
                x.FileType == CaseFileType.AnyModelUK ||
                x.FileType == CaseFileType.AnyPDFFile ||
                x.FileType == CaseFileType.PDFRechung)
                ).OrderBy(y => y.FileType).ThenBy(y => y.FileNum).ThenBy(z => z.FileDate).ThenBy(z => z.Filename).ThenBy(z => z.isThumbNail).ToList();
            if (pics != null && pics.Count() > 0)
            {
                MvcApplication.logMsg("add pics: " + pics.Count().ToString("#0"));

                for (int i=0; i < pics.Count();)
                {
                    CaseFiles pic = pics.ElementAt(i);
                    bool anyPhoto = pic.FileType == CaseFileType.AnyPhoto;

                    MvcApplication.logMsg("pics-a: " + i.ToString("#0") + " " + pic.Id.ToString("#0") + " " + pic.Filename + " " + (pic.isThumbNail ? "(t)" : "(n)"));

                    CaseFilesList item = new CaseFilesList();

                    string SessionPicName = "bild_" + pic.Id.ToString("#0");
                    Session[SessionPicName] = pic;

                    item.FileDate = pic.FileDate.GetValueOrDefault(DateTime.Now);
                    item.Filename = pic.Filename;
                    item.FileType = pic.FileType;
                    if (pic.isThumbNail)
                    {
                        item.ThumbId = pic.Id;
                    }
                    else
                    {
                        item.ImageId = pic.Id;
                    }
                    pic = null;

                    if (anyPhoto == true)
                    {
                        pic = pics.ElementAt(i + 1);

                        MvcApplication.logMsg("pics-b: " + i.ToString("#0") + " " + pic.Id.ToString("#0") + " " + pic.Filename + " " + (pic.isThumbNail ? "(t)" : "(n)"));

                        SessionPicName = "bild_" + pic.Id.ToString("#0");
                        Session[SessionPicName] = pic;

                        if (pic.isThumbNail)
                        {
                            item.ThumbId = pic.Id;
                        }
                        else
                        {
                            item.ImageId = pic.Id;
                        }
                        i += 2;
                    }
                    else
                    {
                        i += 1;
                    }
                    sCaseModel.Files.Add(item);
                    pic = null;
                }
            }
            pics= null;

            // nachrichten laden
            //
            sCaseModel.Messages = new List<MessageViewModel>();
            IEnumerable<Messages> msgs = db.Messages.Where(x => x.CaseId == sCaseModel.CaseId).OrderByDescending(x => x.MessageDate).ToList();
            if (msgs != null && msgs.Count() > 0)
            {
                foreach (Messages item in msgs)
                {
                    MessageViewModel m = new MessageViewModel();
                    m.MessageDate = item.MessageDate.ToString("yy-MM-dd HH:mm");
                    m.FromUserEmail = item.FromUserEmail;
                    m.ToUserEmail = item.ToUserEmail;
                    m.Message = item.Msg;
                    sCaseModel.Messages.Add(m);
                }
            }
            msgs = null;

            
            // Artikel laden
            //
            WARESSHOP showShop = WARESSHOP.all;
            if (shopKind == SHOPART.DoctorShop) showShop = WARESSHOP.doctors;
            if (shopKind == SHOPART.PatientShop) showShop = WARESSHOP.patients;
            sCaseModel.isDoctorShop = false;
            if (shopKind == SHOPART.DoctorShop) sCaseModel.isDoctorShop = true; 
            sCaseModel.Wares = new List<Wares>();
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
                    sCaseModel.Wares.Add(w);
                }
            }
            wares = null;


            // Cart laden
            //
            sCaseModel.Carts = new List<CaseCart>();
            IEnumerable<CaseCart> carts = db.CaseCart.Where(x => x.CaseNumber == sCaseModel.CaseNumber).OrderBy(x => x.CartNumber).ThenBy(y => y.CreateDate);
            if (carts != null && carts.Count() > 0)
            {
                foreach (CaseCart c in carts)
                {
                    sCaseModel.Carts.Add(c);
                }
            }
            carts = null;

            Session["caseView"] = sCaseModel;

            return View(sCaseModel);
        }


        // POST: EditCase
        // [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> EditCase(string submitButton, string sendMsgButton, string addWareButton, string delWareButton, string addPicButton, string addAnyFileButton, string addStlButton, string addPDFButton, string canvasShowButton, CaseViewModel model)
        {
            int result = 0;
            ///// System.Linq.IQueryable<ApplicationUser> adminusers = null;
            bool reView = false;
            DateTime adate = DateTime.Now.Date;

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (!user.isVerified || !user.isDoctor)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }
            ViewBag.isAdmin = user.isAdmin;

            if (Session.IsNewSession)
            {
                Session.Clear();
                return RedirectToAction("Index", "Case");
            }

            if (submitButton != null && submitButton.ToLower() == "abbruch")
            {
                Session["caseView"] = null;
                return RedirectToAction("Index", "Case");
            }

            CaseViewModel sessionCase = (CaseViewModel)Session["caseView"];
            if (sessionCase == null)
            {
                Session["caseView"] = null;
                Session.Clear();
                return RedirectToAction("Index", "Case");
            }

            if (model.CaseId != sessionCase.CaseId || model.CaseNumber != sessionCase.CaseNumber || model.CaseNumber != sessionCase.CaseNumber)
            {
                Session["caseView"] = null;
                Session.Clear();
                return RedirectToAction("Index", "Case");
            }

            ViewBag.isAdmin = user.isAdmin;
            ViewBag.isDoctor = user.isDoctor;
            ViewBag.isCustomer = user.isCustomer;

            if (sessionCase.CaseState == CaseStates.Planung)
            {
                //if (model.PatFirstname == null || model.PatFirstname.Length < 2 || model.PatLastName == null || model.PatLastName.Length < 2)
                //{
                //    reView = true;
                //    ModelState.AddModelError("", "Der Name des Patienten muss angegeben werden.");
                //}

                //if (model.PatBirthDate != null && GuessDateFromString(model.PatBirthDate, ref adate) == false)
                //{
                //    reView = true;
                //    ModelState.AddModelError("", "Das Geburtsdatum des Patienten muß angegeben werden.");
                //}
            }

            if (!ModelState.IsValid || reView)
            {
                ViewBag.ValidationErrors = string.Join(",", ModelState.Values.Where(e => e.Errors.Count > 0).SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToArray());
                ModelState.Clear();
                model = sessionCase;
                return View(model);
            }

            // eingaben abgleichen aus dem viewmodel
            //
            if (user.isAdmin || sessionCase.CaseState == CaseStates.Planung)
            {
                sessionCase.PatFirstname = model.PatFirstname;
                sessionCase.PatLastName = model.PatLastName;
                sessionCase.PatBirthDate = model.PatBirthDate;
                sessionCase.PatAddress = model.PatAddress;
                sessionCase.PatZIP = model.PatZIP;
                sessionCase.PatCity = model.PatCity;
                sessionCase.PatSex = (model.PatSex == null || model.PatSex != "W") ? "M" : "W";
                if (GuessDateFromString(model.PatBirthDate, ref adate) == true) sessionCase.PatBirthDate = model.PatBirthDate;
                sessionCase.PatEmail = model.PatEmail;
                sessionCase.PatPhone = model.PatPhone;
            }

            if (user.isAdmin && sessionCase.CaseNumberStr != model.CaseNumberStr && model.CaseNumberStr != null && model.CaseNumberStr.Length > 3)
            {
                sessionCase.CaseNumberStr = AdjustCaseNumberStr(model.CaseNumberStr.Trim());
            }

            sessionCase.Tooth11 = model.Tooth11;
            sessionCase.Tooth12 = model.Tooth12;
            sessionCase.Tooth13 = model.Tooth13;
            sessionCase.Tooth14 = model.Tooth14;
            sessionCase.Tooth15 = model.Tooth15;
            sessionCase.Tooth16 = model.Tooth16;
            sessionCase.Tooth17 = model.Tooth17;
            sessionCase.Tooth18 = model.Tooth18;
            sessionCase.Tooth21 = model.Tooth21;
            sessionCase.Tooth22 = model.Tooth22;
            sessionCase.Tooth23 = model.Tooth23;
            sessionCase.Tooth24 = model.Tooth24;
            sessionCase.Tooth25 = model.Tooth25;
            sessionCase.Tooth26 = model.Tooth26;
            sessionCase.Tooth27 = model.Tooth27;
            sessionCase.Tooth28 = model.Tooth28;
            sessionCase.Tooth31 = model.Tooth31;
            sessionCase.Tooth32 = model.Tooth32;
            sessionCase.Tooth33 = model.Tooth33;
            sessionCase.Tooth34 = model.Tooth34;
            sessionCase.Tooth35 = model.Tooth35;
            sessionCase.Tooth36 = model.Tooth36;
            sessionCase.Tooth37 = model.Tooth37;
            sessionCase.Tooth38 = model.Tooth38;
            sessionCase.Tooth41 = model.Tooth41;
            sessionCase.Tooth42 = model.Tooth42;
            sessionCase.Tooth43 = model.Tooth43;
            sessionCase.Tooth44 = model.Tooth44;
            sessionCase.Tooth45 = model.Tooth45;
            sessionCase.Tooth46 = model.Tooth46;
            sessionCase.Tooth47 = model.Tooth47;
            sessionCase.Tooth48 = model.Tooth48;
            sessionCase.Stripping = model.Stripping;
            sessionCase.AddTreatButtons = model.AddTreatButtons;
            sessionCase.AddTreatClassII = model.AddTreatClassII;
            sessionCase.AddTreatExtract = model.AddTreatExtract;
            sessionCase.AddTreatExtrusion = model.AddTreatExtrusion;
            sessionCase.AddTreatPontic = model.AddTreatPontic;
            sessionCase.AddTreatRail = model.AddTreatRail;
            sessionCase.AddTreatRetainer = model.AddTreatRetainer;
            sessionCase.AddTreatSuspender = model.AddTreatSuspender;
            sessionCase.AddTreatWire = model.AddTreatWire;
            sessionCase.AddTreatAttachment = model.AddTreatAttachment;
            if (user.isAdmin)
            {
                sessionCase.TreatComment = model.TreatComment;
            }

            sessionCase.Step01OK = model.Step01OK;
            sessionCase.Step01UK = model.Step01UK;
            sessionCase.Step02OK = model.Step02OK;
            sessionCase.Step02UK = model.Step02UK;
            sessionCase.Step03OK = model.Step03OK;
            sessionCase.Step03UK = model.Step03UK;
            sessionCase.Step04OK = model.Step04OK;
            sessionCase.Step04UK = model.Step04UK;
            sessionCase.Step05OK = model.Step05OK;
            sessionCase.Step05UK = model.Step05UK;
            sessionCase.Step06OK = model.Step06OK;
            sessionCase.Step06UK = model.Step06UK;
            sessionCase.Step07OK = model.Step07OK;
            sessionCase.Step07UK = model.Step07UK;
            sessionCase.Step08OK = model.Step08OK;
            sessionCase.Step08UK = model.Step08UK;
            sessionCase.Step09OK = model.Step09OK;
            sessionCase.Step09UK = model.Step09UK;
            sessionCase.Step10OK = model.Step10OK;
            sessionCase.Step10UK = model.Step10UK;
            sessionCase.Step11OK = model.Step11OK;
            sessionCase.Step11UK = model.Step11UK;
            sessionCase.Step12OK = model.Step12OK;
            sessionCase.Step12UK = model.Step12UK;

            sessionCase.Step01Kommentar = model.Step01Kommentar;
            sessionCase.Step02Kommentar = model.Step02Kommentar;
            sessionCase.Step03Kommentar = model.Step03Kommentar;
            sessionCase.Step04Kommentar = model.Step04Kommentar;
            sessionCase.Step05Kommentar = model.Step05Kommentar;
            sessionCase.Step06Kommentar = model.Step06Kommentar;
            sessionCase.Step07Kommentar = model.Step07Kommentar;
            sessionCase.Step08Kommentar = model.Step08Kommentar;
            sessionCase.Step09Kommentar = model.Step09Kommentar;
            sessionCase.Step10Kommentar = model.Step10Kommentar;
            sessionCase.Step11Kommentar = model.Step11Kommentar;
            sessionCase.Step12Kommentar = model.Step12Kommentar;

            sessionCase.Zeitplan = model.Zeitplan;

            // nur infos die ein Admin setzen kann
            //
            if (user.isAdmin)
            {
                if (model.Payment1 != sessionCase.Payment1)
                {
                    sessionCase.Payment1 = model.Payment1;
                    sessionCase.Payment1Date = DateTime.Now;
                }
                if (model.Payment2 != sessionCase.Payment2)
                {
                    sessionCase.Payment2 = model.Payment2;
                    sessionCase.Payment2Date = DateTime.Now;
                }

                if (model.PrivatKommentar != sessionCase.PrivatKommentar)
                {
                    sessionCase.PrivatKommentar = model.PrivatKommentar;
                }

                sessionCase.CaseState = model.CaseState;
            }

            // Bilder?
            //
            if (model.PortraitFotoBin != null && model.PortraitFotoBin.ContentLength > 0 && model.PortraitFotoBin.FileName != null && model.PortraitFotoBin.FileName.Length > 0)
            {
                sessionCase.PortraitFotoBin = model.PortraitFotoBin;
            }
            if (model.PortraitSmilingFotoBin != null && model.PortraitSmilingFotoBin.ContentLength > 0 && model.PortraitSmilingFotoBin.FileName != null && model.PortraitSmilingFotoBin.FileName.Length > 0)
            {
                sessionCase.PortraitSmilingFotoBin = model.PortraitSmilingFotoBin;
            }
            if (model.ProfilFotoBin != null && model.ProfilFotoBin.ContentLength > 0 && model.ProfilFotoBin.FileName != null && model.ProfilFotoBin.FileName.Length > 0)
            {
                sessionCase.ProfilFotoBin = model.ProfilFotoBin;
            }
            if (model.ProfilSmilingFotoBin != null && model.ProfilSmilingFotoBin.ContentLength > 0 && model.ProfilSmilingFotoBin.FileName != null && model.ProfilSmilingFotoBin.FileName.Length > 0)
            {
                sessionCase.ProfilSmilingFotoBin = model.ProfilSmilingFotoBin;
            }
            if (model.PanoFotoBin != null && model.PanoFotoBin.ContentLength > 0 && model.PanoFotoBin.FileName != null && model.PanoFotoBin.FileName.Length > 0)
            {
                sessionCase.PanoFotoBin = model.PanoFotoBin;
            }
            if (model.DistXFotoBin != null && model.DistXFotoBin.ContentLength > 0 && model.DistXFotoBin.FileName != null && model.DistXFotoBin.FileName.Length > 0)
            {
                sessionCase.DistXFotoBin = model.DistXFotoBin;
            }
            if (model.ModelBeforeFrontFotoBin != null && model.ModelBeforeFrontFotoBin.ContentLength > 0 && model.ModelBeforeFrontFotoBin.FileName != null && model.ModelBeforeFrontFotoBin.FileName.Length > 0)
            {
                sessionCase.ModelBeforeFrontFotoBin = model.ModelBeforeFrontFotoBin;
            }
            if (model.ModelAfterFrontFotoBin != null && model.ModelAfterFrontFotoBin.ContentLength > 0 && model.ModelAfterFrontFotoBin.FileName != null && model.ModelAfterFrontFotoBin.FileName.Length > 0)
            {
                sessionCase.ModelAfterFrontFotoBin = model.ModelAfterFrontFotoBin;
            }
            if (model.ModelBeforeRightFotoBin != null && model.ModelBeforeRightFotoBin.ContentLength > 0 && model.ModelBeforeRightFotoBin.FileName != null && model.ModelBeforeRightFotoBin.FileName.Length > 0)
            {
                sessionCase.ModelBeforeRightFotoBin = model.ModelBeforeRightFotoBin;
            }
            if (model.ModelMaxFotoBin != null && model.ModelMaxFotoBin.ContentLength > 0 && model.ModelMaxFotoBin.FileName != null && model.ModelMaxFotoBin.FileName.Length > 0)
            {
                sessionCase.ModelMaxFotoBin = model.ModelMaxFotoBin;
            }
            if (model.ModelMandFotoBin != null && model.ModelMandFotoBin.ContentLength > 0 && model.ModelMandFotoBin.FileName != null && model.ModelMandFotoBin.FileName.Length > 0)
            {
                sessionCase.ModelMandFotoBin = model.ModelMandFotoBin;
            }
            if (model.ModelAfterRightFotoBin != null && model.ModelAfterRightFotoBin.ContentLength > 0 && model.ModelAfterRightFotoBin.FileName != null && model.ModelAfterRightFotoBin.FileName.Length > 0)
            {
                sessionCase.ModelAfterRightFotoBin = model.ModelAfterRightFotoBin;
            }
            if (model.ModelBeforeOKUKFotoBin != null && model.ModelBeforeOKUKFotoBin.ContentLength > 0 && model.ModelBeforeOKUKFotoBin.FileName != null && model.ModelBeforeOKUKFotoBin.FileName.Length > 0)
            {
                sessionCase.ModelBeforeOKUKFotoBin = model.ModelBeforeOKUKFotoBin;
            }
            if (model.ModelAfterOKUKFotoBin != null && model.ModelAfterOKUKFotoBin.ContentLength > 0 && model.ModelAfterOKUKFotoBin.FileName != null && model.ModelAfterOKUKFotoBin.FileName.Length > 0)
            {
                sessionCase.ModelAfterOKUKFotoBin = model.ModelAfterOKUKFotoBin;
            }
            if (model.ModelBeforeOKSltBin != null && model.ModelBeforeOKSltBin.ContentLength > 0 && model.ModelBeforeOKSltBin.FileName != null && model.ModelBeforeOKSltBin.FileName.Length > 0)
            {
                sessionCase.ModelBeforeOKSltBin = model.ModelBeforeOKSltBin;
            }
            if (model.ModelBeforeOKUKSltBin != null && model.ModelBeforeOKUKSltBin.ContentLength > 0 && model.ModelBeforeOKUKSltBin.FileName != null && model.ModelBeforeOKUKSltBin.FileName.Length > 0)
            {
                sessionCase.ModelBeforeOKUKSltBin = model.ModelBeforeOKUKSltBin;
            }
            if (model.ModelAfterOKSltBin != null && model.ModelAfterOKSltBin.ContentLength > 0 && model.ModelAfterOKSltBin.FileName != null && model.ModelAfterOKSltBin.FileName.Length > 0)
            {
                sessionCase.ModelAfterOKSltBin = model.ModelAfterOKSltBin;
            }
            if (model.ModelBeforeUKSltBin != null && model.ModelBeforeUKSltBin.ContentLength > 0 && model.ModelBeforeUKSltBin.FileName != null && model.ModelBeforeUKSltBin.FileName.Length > 0)
            {
                sessionCase.ModelBeforeUKSltBin = model.ModelBeforeUKSltBin;
            }
            if (model.ModelAfterUKSltBin != null && model.ModelAfterUKSltBin.ContentLength > 0 && model.ModelAfterUKSltBin.FileName != null && model.ModelAfterUKSltBin.FileName.Length > 0)
            {
                sessionCase.ModelAfterUKSltBin = model.ModelAfterUKSltBin;
            }
            if (model.ModelAfterOKUKSltBin != null && model.ModelAfterOKUKSltBin.ContentLength > 0 && model.ModelAfterOKUKSltBin.FileName != null && model.ModelAfterOKUKSltBin.FileName.Length > 0)
            {
                sessionCase.ModelAfterOKUKSltBin = model.ModelAfterOKUKSltBin;
            }
            if (model.AddPDFBin != null && model.AddPDFBin.ContentLength > 0 && model.AddPDFBin.FileName != null && model.AddPDFBin.FileName.Length > 0)
            {
                sessionCase.AddPDFBin = model.AddPDFBin;
            }
            if (model.AddProjektBin != null && model.AddProjektBin.ContentLength > 0 && model.AddProjektBin.FileName != null && model.AddProjektBin.FileName.Length > 0)
            {
                sessionCase.AddProjektBin = model.AddProjektBin;
            }
            if (model.FrontAnimationGifBin != null && model.FrontAnimationGifBin.ContentLength > 0 && !string.IsNullOrWhiteSpace(model.FrontAnimationGifFilename))
            {
                sessionCase.FrontAnimationGifBin = model.FrontAnimationGifBin;
            }
            if (model.SideAnimationGifBin != null && model.SideAnimationGifBin.ContentLength > 0 && !string.IsNullOrWhiteSpace(model.SideAnimationGifFilename))
            {
                sessionCase.SideAnimationGifBin = model.SideAnimationGifBin;
            }
            if (model.OcclAnimationGifBin != null && model.OcclAnimationGifBin.ContentLength > 0 && !string.IsNullOrWhiteSpace(model.OcclAnimationGifFilename))
            {
                sessionCase.OcclAnimationGifBin = model.OcclAnimationGifBin;
            }


            // sitzungsvar aktualisieren
            //
            Session["caseView"] = sessionCase;


            // sonst was?
            //
            if (delWareButton != null && delWareButton.Length > 0)
            {
                result = 0;
                long xid = long.Parse(delWareButton);
                CaseCart aCart = null;
                IQueryable<CaseCart> carts = db.CaseCart.Where(x => x.Id == xid);
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
                        MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                        result = -1;
                    }
                    if (result < 0)
                    {
                        ModelState.Clear();
                        Session.Clear();
                        return RedirectToAction("KeinSpeichern", "Home");
                    }
                    foreach (CaseCart y in sessionCase.Carts)
                    {
                        if (y.Id == xid)
                        {
                            sessionCase.Carts.Remove(y);
                            break;
                        }
                    }
                    model.Carts = sessionCase.Carts;
                    Session["caseView"] = sessionCase;
                }
                aCart = null;
                carts = null;

                model = sessionCase;
                ModelState.Clear();
                return View(model);
            }

            if (addWareButton != null && addWareButton.Length > 0)
            {
                if (model.AddWareItem != null && model.AddWareItem.Length > 0 && model.AddWareAmount != 0)
                {
                    await AddCartAsync(sessionCase, int.Parse(model.AddWareItem), model.AddWareAmount);
                    sessionCase.Carts = new List<CaseCart>();
                    IEnumerable<CaseCart> carts = db.CaseCart.Where(x => x.CaseNumber == sessionCase.CaseNumber).OrderBy(x => x.CartNumber).ThenBy(y => y.CreateDate);
                    if (carts != null && carts.Count() > 0)
                    {
                        foreach (CaseCart c in carts)
                        {
                            sessionCase.Carts.Add(c);
                        }
                    }
                    carts = null;
                }
                Session["caseView"] = sessionCase;
                model = sessionCase;
                ModelState.Clear();
                return View(model);
            }

            if (sendMsgButton != null && sendMsgButton != "")
            {
                // nachricht senden
                // viewmodel add msg
                //
                Messages newMsg = new Messages();
                newMsg.CaseId = sessionCase.CaseId;
                newMsg.FromUserEmail = user.Email;
                newMsg.MessageDate = DateTime.Now;
                newMsg.Msg = model.CurrentMessage;
                newMsg.isReplied = user.isAdmin;

                MessageViewModel mneu = new MessageViewModel();
                mneu.MessageDate = newMsg.MessageDate.ToString("yy-MM-dd HH:mm");
                mneu.FromUserEmail = newMsg.FromUserEmail;
                mneu.ToUserEmail = newMsg.ToUserEmail;
                mneu.Message = newMsg.Msg;
                sessionCase.Messages.Add(mneu);

                Session["caseView"] = sessionCase;

                result = 0;
                try
                {
                    db.Messages.Add(newMsg);
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

                /*
                adminusers = db.Users.Where(x => x.isAdmin == true);
                if (Environment.MachineName.ToLower() != "apps" && adminusers != null)
                {
                    foreach (ApplicationUser x in adminusers)
                    {
                        UserManager.SendEmail(x.Id, "Eine Nachricht wurde geschickt von " + user.UserName, "Von " + user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname + Environment.NewLine + Environment.NewLine + model.CurrentMessage);
                    }
                }
                adminusers = null;
                */
                SendeEmail("info@thinortho.com", "Eine Nachricht wurde geschickt von " + user.UserName, "Von " + user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname + Environment.NewLine + Environment.NewLine + model.CurrentMessage);
                SendeEmail("info@philippott.eu", "Eine Nachricht wurde geschickt von " + user.UserName, "Von " + user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname + Environment.NewLine + Environment.NewLine + model.CurrentMessage);

                if (user.isAdmin)
                {
                    result = 0;
                    try
                    {
                        IEnumerable<Messages> reps = db.Messages.Where(x => x.CaseId == sessionCase.CaseId && x.isReplied == false).ToList();
                        if (reps != null && reps.Count() > 0)
                        {
                            foreach (Messages m in reps)
                            {
                                m.isReplied = true;
                            }
                            result = await db.SaveChangesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                        result = -1;
                    }
                }

                model = sessionCase;
                ModelState.Clear();
                return View(model);

                // return RedirectToAction("EditCase", "Case", new { id = sessionCase.CaseNumber.ToString("#0") });
            }

            if (addPicButton != null && addPicButton != "")
            {
                // AddFotoBin hinzufügen
                string fname = "";
                long thumbId = 0;
                long imgId = 0;
                DateTime fdate = DateTime.Now;

                if (model.AddFotoBin != null && model.AddFotoBin.ContentLength > 0)
                {
                    result = AddPicture(model.AddFotoBin, CaseFileType.AnyPhoto, sessionCase.CaseId, out imgId, out thumbId, out fname, out fdate);
                    if (result >= 1)
                    {
                        // Update Cache/Liste (sessionCase)
                        CaseFilesList item = new CaseFilesList();

                        item.FileDate = fdate;
                        item.Filename = fname;
                        item.FileType = CaseFileType.AnyPhoto;
                        item.ThumbId = thumbId;
                        item.ImageId = imgId;

                        sessionCase.Files.Add(item);
                    }

                }
                model = sessionCase;
                ModelState.Clear();
                return View(model);
            }

            if (addAnyFileButton != null && addAnyFileButton != "")
            {
                // addAnyFileButton hinzufügen
                string fname = "";
                long thumbId = 0;
                long imgId = 0;
                DateTime fdate = DateTime.Now;

                if (model.AddAnyFileBin != null && model.AddAnyFileBin.ContentLength > 0)
                {
                    result = AddFile(model.AddAnyFileBin, CaseFileType.AnyFile, sessionCase.CaseId, out imgId, out fname, out fdate);
                    if (result >= 1)
                    {
                        // Update Cache/Liste (sessionCase)
                        CaseFilesList item = new CaseFilesList();

                        item.FileDate = fdate;
                        item.Filename = fname;
                        item.FileType = CaseFileType.AnyFile;
                        item.ThumbId = thumbId;
                        item.ImageId = imgId;

                        sessionCase.Files.Add(item);
                    }

                }
                model = sessionCase;
                ModelState.Clear();
                return View(model);
            }

            if (addStlButton != null && addStlButton != "")
            {
                // AddFotoBin hinzufügen
                string fname = "";
                long thumbId = 0;
                long imgId = 0;
                DateTime fdate = DateTime.Now;

                if (model.AddModelBin != null && model.AddModelBin.ContentLength > 0)
                {
                    result = AddFile(model.AddModelBin, model.AddModelType, sessionCase.CaseId, out imgId, out fname, out fdate);
                    if (result >= 1)
                    {
                        // Update Cache/Liste (sessionCase)
                        CaseFilesList item = new CaseFilesList();

                        item.FileDate = fdate;
                        item.Filename = fname;
                        item.FileType = model.AddModelType;
                        item.ThumbId = thumbId;
                        item.ImageId = imgId;

                        sessionCase.Files.Add(item);
                    }
                }
                model = sessionCase;
                ModelState.Clear();
                return View(model);
            }


            if (addPDFButton != null && addPDFButton != "")
            {
                // AddFotoBin hinzufügen
                string fname = "";
                long thumbId = 0;
                long imgId = 0;
                DateTime fdate = DateTime.Now;

                if (model.AddAnyPDFBin != null && model.AddAnyPDFBin.ContentLength > 0)
                {
                    result = AddFile(model.AddAnyPDFBin, CaseFileType.AnyPDFFile, sessionCase.CaseId, out imgId, out fname, out fdate);
                    if (result >= 1)
                    {
                        // Update Cache/Liste (sessionCase)
                        CaseFilesList item = new CaseFilesList();

                        item.FileDate = fdate;
                        item.Filename = fname;
                        item.FileType = CaseFileType.AnyPDFFile;
                        item.ThumbId = thumbId;
                        item.ImageId = imgId;

                        sessionCase.Files.Add(item);
                    }
                }
                model = sessionCase;
                ModelState.Clear();
                return View(model);
            }


            if (canvasShowButton != null && canvasShowButton != "")
            {
                sessionCase.ShowCanvasModels = true;
                Session["caseView"] = sessionCase; 
                model = sessionCase;
                ModelState.Clear();
                return View(model);
            }

            // basisdatensatz aktualisieren
            //
            Cases dbcase = null;
            result = 0;
            System.Data.Entity.DbContextTransaction tr = null;
            using (tr = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    dbcase = db.Cases.Find(sessionCase.CaseId);
                    if (dbcase != null)
                    {
                        //            d.CaseNumber = s.CaseNumber;
                        //            d.DoctorId = s.DoctorId;
                        CopyCase(dbcase, sessionCase);

                        if (dbcase.ErstellungsDatum.Year < 2015) dbcase.ErstellungsDatum = DateTime.Now.Date;

                        if (dbcase.CaseNumberStr == null || dbcase.CaseNumberStr.Length == 0)
                        {
                            string landstr = (user.CountryCode == null || user.CountryCode.Length != 2) ? "AT" : user.CountryCode;
                            string docname = (user.DocShortName == null || user.DocShortName.Length == 0) ? user.Lastname.ToUpper() : user.DocShortName;
                            if (docname.Length >= 7) docname = docname.Substring(0, 7);
                            int fallstr = NewDailyNumber(db, dbcase.ErstellungsDatum, tr);
                            dbcase.CaseNumberStr = landstr + " " + docname + " " + fallstr.ToString("0000000000");
                        }

                        // dbcase.RowVersion = sessionCase.RowVersion;
                        db.Entry(dbcase).OriginalValues["RowVersion"] = sessionCase.RowVersion;

                        MvcApplication.logMsg("case " + dbcase.Id.ToString("#0") + " rowversion = " + dbcase.RowVersion.ToArray().ToString());

                        result = await db.SaveChangesAsync();
                        tr.Commit();

                        MvcApplication.logMsg("case " + dbcase.Id.ToString("#0") + " updated rowversion = " + dbcase.RowVersion.ToArray().ToString());
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    tr.Rollback();

                    MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                    result = -1;

                    ViewBag.ErrorExceptionString = "Ihre Eingaben wurden zwischenzeitlich durch die Bearbeitung dieses Falls durch einen anderen Anwender verworfen! Bitte öffnen Sie den Fall erneut über die Übersicht, um die aktuellen Daten oder Bilder zu sehen und ändern zu können.";
                }
                catch (RetryLimitExceededException ex)
                {
                    tr.Rollback();

                    MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                    result = -1;

                    ViewBag.ErrorExceptionString = "Ihre Eingaben wurden zwischenzeitlich durch die Bearbeitung dieses Falls durch einen anderen Anwender verworfen! Bitte öffnen Sie den Fall erneut über die Übersicht, um die aktuellen Daten oder Bilder zu sehen und ändern zu können.";
                }
                catch (Exception ex)
                {
                    tr.Rollback();

                    MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                    result = -1;

                    ViewBag.ErrorExceptionString = "Ihre Eingaben wurden zwischenzeitlich durch die Bearbeitung dieses Falls durch einen anderen Anwender verworfen! Bitte öffnen Sie den Fall erneut über die Übersicht, um die aktuellen Daten oder Bilder zu sehen und ändern zu können.";
                }
            }
            tr = null;
            
            if (result < 0)
            {
                // fehler anzeigen

                dbcase = null;
                return RedirectToAction("KeinSpeichern", "Home");
            }

            // aktualisierte rowersion holen
            //
            dbcase = db.Cases.Find(sessionCase.CaseId);
            sessionCase.RowVersion = dbcase.RowVersion;
            sessionCase.CaseNumberStr = dbcase.CaseNumberStr;
            Session["caseView"] = sessionCase; 

            string body = Case2Emailbody(dbcase);
            dbcase = null;


            // update bilder
            //
            // Portrait
            //
            if (model.PortraitFotoBin != null && model.PortraitFotoBin.ContentLength > 0)
            {
                await UpdatePicture(model.PortraitFotoBin, CaseFileType.Portrait, model.CaseId);
            }

            // Portrait lächelnd
            //
            if (model.PortraitSmilingFotoBin != null && model.PortraitSmilingFotoBin.ContentLength > 0)
            {
                await UpdatePicture(model.PortraitSmilingFotoBin, CaseFileType.PortraitSmiling, model.CaseId);
            }

            // Profil
            //
            if (model.ProfilFotoBin != null && model.ProfilFotoBin.ContentLength > 0)
            {
                await UpdatePicture(model.ProfilFotoBin, CaseFileType.Profil, model.CaseId);
            }

            // Profil lächeldn
            //
            if (model.ProfilSmilingFotoBin != null && model.ProfilSmilingFotoBin.ContentLength > 0)
            {
                await UpdatePicture(model.ProfilSmilingFotoBin, CaseFileType.ProfilSmiling, model.CaseId);
            }

            // Pano
            //
            if (model.PanoFotoBin != null && model.PanoFotoBin.ContentLength > 0)
            {
                await UpdatePicture(model.PanoFotoBin, CaseFileType.PanoXRay, model.CaseId);
            }

            // Dist Rö
            //
            if (model.DistXFotoBin != null && model.DistXFotoBin.ContentLength > 0)
            {
                await UpdatePicture(model.DistXFotoBin, CaseFileType.DistXRay, model.CaseId);
            }

            // Datatable
            //
            if (model.DataFotoBin != null && model.DataFotoBin.ContentLength > 0)
            {
                await UpdatePicture(model.DataFotoBin, CaseFileType.DataPicture, model.CaseId);
            }

            // Model vorher
            //
            if (model.ModelBeforeFrontFotoBin != null && model.ModelBeforeFrontFotoBin.ContentLength > 0)
            {
                await UpdatePicture(model.ModelBeforeFrontFotoBin, CaseFileType.ModelBeforeFront, model.CaseId);
            }

            // Model nachher
            //
            if (model.ModelAfterFrontFotoBin != null && model.ModelAfterFrontFotoBin.ContentLength > 0)
            {
                await UpdatePicture(model.ModelAfterFrontFotoBin, CaseFileType.ModelAfterFront, model.CaseId);
            }

            // Model vorher rechts
            //
            if (model.ModelBeforeRightFotoBin != null && model.ModelBeforeRightFotoBin.ContentLength > 0)
            {
                await UpdatePicture(model.ModelBeforeRightFotoBin, CaseFileType.ModelBeforeRight, model.CaseId);
            }

            // Max Ansicht
            //
            if (model.ModelMaxFotoBin != null && model.ModelMaxFotoBin.ContentLength > 0)
            {
                await UpdatePicture(model.ModelMaxFotoBin, CaseFileType.ModelMaxView, model.CaseId);
            }

            // Mand Ansicht
            //
            if (model.ModelMandFotoBin != null && model.ModelMandFotoBin.ContentLength > 0)
            {
                await UpdatePicture(model.ModelMandFotoBin, CaseFileType.ModelMandView, model.CaseId);
            }

            // Model nachfer rechts
            //
            if (model.ModelAfterRightFotoBin != null && model.ModelAfterRightFotoBin.ContentLength > 0)
            {
                await UpdatePicture(model.ModelAfterRightFotoBin, CaseFileType.ModelAfterRight, model.CaseId);
            }

            // Model OKUK vorher
            //
            if (model.ModelBeforeOKUKFotoBin != null && model.ModelBeforeOKUKFotoBin.ContentLength > 0)
            {
                await UpdatePicture(model.ModelBeforeOKUKFotoBin, CaseFileType.ModelBeforeOKUK, model.CaseId);
            }

            // Model OK UK nachher
            //
            if (model.ModelAfterOKUKFotoBin != null && model.ModelAfterOKUKFotoBin.ContentLength > 0)
            {
                await UpdatePicture(model.ModelAfterOKUKFotoBin, CaseFileType.ModelAfterOKUK, model.CaseId);
            }

            // PDF Datei
            //
            if (model.AddPDFBin != null && model.AddPDFBin.ContentLength > 0)
            {
                await UpdatePicture(model.AddPDFBin, CaseFileType.PDFFile, model.CaseId);
            }

            // Projekt 3sz Datei
            //
            if (model.AddProjektBin != null && model.AddProjektBin.ContentLength > 0)
            {
                await UpdatePicture(model.AddProjektBin, CaseFileType.P3szFile, model.CaseId);
            }

            // SLT Modelle abbilden
            //
            if (model.ModelBeforeOKSltBin != null && model.ModelBeforeOKSltBin.ContentLength > 0)
            {
                await UpdatePicture(model.ModelBeforeOKSltBin, CaseFileType.SltModelOK, model.CaseId);
            }
            if (model.ModelBeforeOKUKSltBin != null && model.ModelBeforeOKUKSltBin.ContentLength > 0)
            {
                await UpdatePicture(model.ModelBeforeOKUKSltBin, CaseFileType.SltModelOKUK, model.CaseId);
            }
            if (model.ModelAfterOKSltBin != null && model.ModelAfterOKSltBin.ContentLength > 0)
            {
                await UpdatePicture(model.ModelAfterOKSltBin, CaseFileType.SltModelOKNow, model.CaseId);
            }
            if (model.ModelBeforeUKSltBin != null && model.ModelBeforeUKSltBin.ContentLength > 0)
            {
                await UpdatePicture(model.ModelBeforeUKSltBin, CaseFileType.SltModelUK, model.CaseId);
            }
            if (model.ModelAfterUKSltBin != null && model.ModelAfterUKSltBin.ContentLength > 0)
            {
                await UpdatePicture(model.ModelAfterUKSltBin, CaseFileType.SltModelUKNow, model.CaseId);
            }
            if (model.ModelAfterOKUKSltBin != null && model.ModelAfterOKUKSltBin.ContentLength > 0)
            {
                await UpdatePicture(model.ModelAfterOKUKSltBin, CaseFileType.SltModelOKUKNow, model.CaseId);
            }

            // Front Animation Gif
            //
            if (model.FrontAnimationGifBin != null && model.FrontAnimationGifBin.ContentLength > 0)
            {
                await UpdatePicture(model.FrontAnimationGifBin, CaseFileType.AnimationGifFront, model.CaseId);
            }
            // Right Animation Gif
            //
            if (model.SideAnimationGifBin != null && model.SideAnimationGifBin.ContentLength > 0)
            {
                await UpdatePicture(model.SideAnimationGifBin, CaseFileType.AnimationGifSide, model.CaseId);
            }
            // Left Animation Gif
            //
            if (model.OcclAnimationGifBin != null && model.OcclAnimationGifBin.ContentLength > 0)
            {
                await UpdatePicture(model.OcclAnimationGifBin, CaseFileType.AnimationGifOccl, model.CaseId);
            }




            //// Email-Nachricht schicken an:
            // theinvisibleorthodontics@outlook.com
            //
            /*
            adminusers = db.Users.Where(x => x.isAdmin == true);
            if (Environment.MachineName.ToLower() != "apps" && adminusers != null)
            {
                foreach (ApplicationUser x in adminusers)
                {
                    UserManager.SendEmail(x.Id, "Ein Fall wurde bearbeitet " + sessionCase.CaseNumber.ToString("#0") + " von " + user.UserName, "Von " + user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname + System.Environment.NewLine + System.Environment.NewLine + body);
                }
            }
            adminusers = null;
            */
            SendeEmail("info@thinortho.com", "Ein Fall wurde bearbeitet " + sessionCase.CaseNumber.ToString("#0") + " von " + user.UserName, "Von " + user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname + System.Environment.NewLine + System.Environment.NewLine + body);
            SendeEmail("info@philippott.eu", "Ein Fall wurde bearbeitet " + sessionCase.CaseNumber.ToString("#0") + " von " + user.UserName, "Von " + user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname + System.Environment.NewLine + System.Environment.NewLine + body);

            if (sessionCase.CaseState == CaseStates.Planung)
            {
                await UpdateCartInfos(sessionCase);
            }

            dbcase = null;

//            Session["caseView"] = null;
//            Session.Clear();
//            return RedirectToAction("Index", "Case");

            return RedirectToAction("EditCase", "Case", new { Id = sessionCase.CaseNumber.ToString("#0") });

//            model = sessionCase;
//            ModelState.Clear();
//            return View(model);
        }


        // GET: Case
        // [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [Authorize]
        public async Task<ActionResult> ViewCase(string id)
        {
            int result = 0;
            ///// System.Linq.IQueryable<ApplicationUser> adminusers = null;

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (!user.isVerified || !user.isCustomer)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            if (Session.IsNewSession)
            {
                Session.Clear();
                return RedirectToAction("PatIndex", "Case");
            }

            // id laden
            //
            CaseViewModel sCaseModel = null;

            // bisherige infos löschen
            //
            Session["caseView"] = sCaseModel;

            IQueryable<Cases> dbcases = null;
            Cases dbcase = null;
            try
            {
                int casenr = Int32.Parse(id);
                dbcases = db.Cases.Where(x => x.CaseNumber == casenr);
                if (dbcases != null && dbcases.Count() == 1)
                {
                    dbcase = dbcases.First<Cases>();
                    IEnumerable<CustomerCases> records = db.CustomerCases.Where(x => x.CasesId == dbcase.Id && x.UserId == user.Id);
                    if (records == null || records.Count() != 1) dbcase = null;
                    records = null;
                }
            }
            catch (Exception ex)
            {
                MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                result = -1;
            }

            if (result < 0 || dbcase == null)
            {
                dbcase = null;
                return RedirectToAction("PatIndex", "Case");
            }

            string body = Case2Emailbody(dbcase);
            SendeEmail("info@thinortho.com", "Ein Fall wurde geöffnet " + dbcase.CaseNumber.ToString("#0") + " von " + user.UserName, "Von " + user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname + System.Environment.NewLine + System.Environment.NewLine + body);
            SendeEmail("info@philippott.eu", "Ein Fall wurde geöffnet " + dbcase.CaseNumber.ToString("#0") + " von " + user.UserName, "Von " + user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname + System.Environment.NewLine + System.Environment.NewLine + body);

            sCaseModel = new CaseViewModel();

            CopyCase(sCaseModel, dbcase);

            ApplicationUser docUser = await UserManager.FindByIdAsync(dbcase.DoctorId);
            sCaseModel.DoctorName = docUser.Salutation + " " + docUser.Titel + " " + docUser.Firstname + " " + docUser.Lastname;
            docUser = null;

            //
            // LoadPicture(scaseModel, dbcase.Id.ToString, 
            // profil & portrait
            //
            long xid = 0;
            long tid = 0;
            string xname = null;
            LoadPicture(CaseFileType.Portrait, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.PortraitFotoId = xid;
            sCaseModel.PortraitThumbId = tid;
            sCaseModel.PortraitFotoName = xname;

            LoadPicture(CaseFileType.PortraitSmiling, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.PortraitSmilingFotoId = xid;
            sCaseModel.PortraitSmilingThumbId = tid;
            sCaseModel.PortraitSmilingFotoName = xname;

            LoadPicture(CaseFileType.Profil, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ProfilFotoId = xid;
            sCaseModel.ProfilThumbId = tid;
            sCaseModel.ProfilFotoName = xname;

            LoadPicture(CaseFileType.ProfilSmiling, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ProfilSmilingFotoId = xid;
            sCaseModel.ProfilSmilingThumbId = tid;
            sCaseModel.ProfilSmilingFotoName = xname;

            // Rö
            //
            LoadPicture(CaseFileType.PanoXRay, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.PanoFotoId = xid;
            sCaseModel.PanoThumbId = tid;
            sCaseModel.PanoFotoName = xname;

            LoadPicture(CaseFileType.DistXRay, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.DistXFotoId = xid;
            sCaseModel.DistXThumbId = tid;
            sCaseModel.DistXFotoName = xname;

            // Data
            //
            LoadPicture(CaseFileType.DataPicture, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.DataFotoId = xid;
            sCaseModel.DataThumbId = tid;
            sCaseModel.DataFotoName = xname;

            // Model before, after, right before, max, mand, right after, ok uk before, ok uk after
            //
            LoadPicture(CaseFileType.ModelBeforeFront, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelBeforeFrontFotoId = xid;
            sCaseModel.ModelBeforeFrontThumbId = tid;
            sCaseModel.ModelBeforeFrontFotoName = xname;

            LoadPicture(CaseFileType.ModelAfterFront, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelAfterFrontFotoId = xid;
            sCaseModel.ModelAfterFrontThumbId = tid;
            sCaseModel.ModelAfterFrontFotoName = xname;

            LoadPicture(CaseFileType.ModelBeforeRight, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelBeforeRightFotoId = xid;
            sCaseModel.ModelBeforeRightThumbId = tid;
            sCaseModel.ModelBeforeRightFotoName = xname;

            LoadPicture(CaseFileType.ModelMaxView, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelMaxFotoId = xid;
            sCaseModel.ModelMaxThumbId = tid;
            sCaseModel.ModelMaxFotoName = xname;

            LoadPicture(CaseFileType.ModelMandView, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelMandFotoId = xid;
            sCaseModel.ModelMandThumbId = tid;
            sCaseModel.ModelMandFotoName = xname;

            LoadPicture(CaseFileType.ModelAfterRight, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelAfterRightFotoId = xid;
            sCaseModel.ModelAfterRightThumbId = tid;
            sCaseModel.ModelAfterRightFotoName = xname;

            LoadPicture(CaseFileType.ModelBeforeOKUK, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelBeforeOKUKFotoId = xid;
            sCaseModel.ModelBeforeOKUKThumbId = tid;
            sCaseModel.ModelBeforeOKUKFotoName = xname;

            LoadPicture(CaseFileType.ModelAfterOKUK, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelAfterOKUKFotoId = xid;
            sCaseModel.ModelAfterOKUKThumbId = tid;
            sCaseModel.ModelAfterOKUKFotoName = xname;

            sCaseModel.ShowCanvasModels = false;

            LoadPicture(CaseFileType.SltModelOK, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelBeforeOKSltId = xid;
            sCaseModel.ModelBeforeOKSltName = xname;

            LoadPicture(CaseFileType.SltModelOKNow, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelAfterOKSltId = xid;
            sCaseModel.ModelAfterOKSltName = xname;

            LoadPicture(CaseFileType.SltModelOKUK, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelBeforeOKUKSltId = xid;
            sCaseModel.ModelBeforeOKUKSltName = xname;

            LoadPicture(CaseFileType.SltModelUK, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelBeforeUKSltId = xid;
            sCaseModel.ModelBeforeUKSltName = xname;

            LoadPicture(CaseFileType.SltModelUKNow, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelAfterUKSltId = xid;
            sCaseModel.ModelAfterUKSltName = xname;

            LoadPicture(CaseFileType.SltModelOKUKNow, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ModelAfterOKUKSltId = xid;
            sCaseModel.ModelAfterOKUKSltName = xname;

            LoadPicture(CaseFileType.PDFFile, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.PDFDocumentID = xid;
            sCaseModel.PDFDocumentFilename = xname;

            LoadPicture(CaseFileType.P3szFile, sCaseModel.CaseId, out xname, out xid, out tid);
            sCaseModel.ProjektID = xid;
            sCaseModel.ProjektFilename = xname;

            // bilder laden
            // Dist, Pano, Any
            sCaseModel.Files = new List<CaseFilesList>();
            //
            //    IEnumerable<CaseFiles> pics  = db.CaseFiles.Where(x => x.CaseId == sCaseModel.CaseId && x.FileType == CaseFileType.AnyPhoto).OrderBy(y => y.FileNum).OrderBy(y => y.FileDate).ThenBy(z => z.isThumbNail);
            IEnumerable<CaseFiles> pics = db.CaseFiles.Where(x => x.CaseId == sCaseModel.CaseId &&
                (x.FileType == CaseFileType.AnyPhoto ||
                x.FileType == CaseFileType.AnyModelOK ||
                x.FileType == CaseFileType.AnyModelUK ||
                x.FileType == CaseFileType.AnyPDFFile ||
                x.FileType == CaseFileType.PDFRechung)
                ).OrderBy(y => y.FileType).ThenBy(y => y.FileNum).ThenBy(z => z.FileDate).ThenBy(z => z.Filename).ThenBy(z => z.isThumbNail).ToList();
            if (pics != null && pics.Count() > 0)
            {
                MvcApplication.logMsg("add pics: " + pics.Count().ToString("#0"));

                for (int i = 0; i < pics.Count(); )
                {
                    CaseFiles pic = pics.ElementAt(i);
                    bool anyPhoto = pic.FileType == CaseFileType.AnyPhoto;

                    MvcApplication.logMsg("pics-a: " + i.ToString("#0") + " " + pic.Id.ToString("#0") + " " + pic.Filename + " " + (pic.isThumbNail ? "(t)" : "(n)"));

                    CaseFilesList item = new CaseFilesList();

                    string SessionPicName = "bild_" + pic.Id.ToString("#0");
                    Session[SessionPicName] = pic;

                    item.FileDate = pic.FileDate.GetValueOrDefault(DateTime.Now);
                    item.Filename = pic.Filename;
                    item.FileType = pic.FileType;
                    if (pic.isThumbNail)
                    {
                        item.ThumbId = pic.Id;
                    }
                    else
                    {
                        item.ImageId = pic.Id;
                    }
                    pic = null;

                    if (anyPhoto == true)
                    {
                        pic = pics.ElementAt(i + 1);

                        MvcApplication.logMsg("pics-b: " + i.ToString("#0") + " " + pic.Id.ToString("#0") + " " + pic.Filename + " " + (pic.isThumbNail ? "(t)" : "(n)"));

                        SessionPicName = "bild_" + pic.Id.ToString("#0");
                        Session[SessionPicName] = pic;

                        if (pic.isThumbNail)
                        {
                            item.ThumbId = pic.Id;
                        }
                        else
                        {
                            item.ImageId = pic.Id;
                        }
                        i += 2;
                    }
                    else
                    {
                        i += 1;
                    }
                    sCaseModel.Files.Add(item);
                    pic = null;
                }
            }
            pics = null;

            // nachrichten laden
            //
            sCaseModel.Messages = new List<MessageViewModel>();
            IEnumerable<Messages> msgs = db.Messages.Where(x => x.CaseId == sCaseModel.CaseId).OrderByDescending(x => x.MessageDate).ToList();
            if (msgs != null && msgs.Count() > 0)
            {
                foreach (Messages item in msgs)
                {
                    MessageViewModel m = new MessageViewModel();
                    m.MessageDate = item.MessageDate.ToString("yy-MM-dd HH:mm");
                    m.FromUserEmail = item.FromUserEmail;
                    m.ToUserEmail = item.ToUserEmail;
                    m.Message = item.Msg;
                    sCaseModel.Messages.Add(m);
                }
            }
            msgs = null;


            // Artikel laden
            //
            WARESSHOP showShop = WARESSHOP.all;
            if (shopKind == SHOPART.DoctorShop) showShop = WARESSHOP.doctors;
            if (shopKind == SHOPART.PatientShop) showShop = WARESSHOP.patients;
            sCaseModel.isDoctorShop = false;
            if (shopKind == SHOPART.DoctorShop) sCaseModel.isDoctorShop = true;
            sCaseModel.Wares = new List<Wares>();
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
                    sCaseModel.Wares.Add(w);
                }
            }
            wares = null;


            // Cart laden
            //
            sCaseModel.Carts = new List<CaseCart>();
            IEnumerable<CaseCart> carts = db.CaseCart.Where(x => x.CaseNumber == sCaseModel.CaseNumber).OrderBy(x => x.CartNumber).ThenBy(y => y.CreateDate);
            if (carts != null && carts.Count() > 0)
            {
                foreach (CaseCart c in carts)
                {
                    sCaseModel.Carts.Add(c);
                }
            }
            carts = null;

            Session["caseView"] = sCaseModel;

            ViewBag.isAdmin = user.isAdmin;

            return View(sCaseModel);
        }
    
    }

}