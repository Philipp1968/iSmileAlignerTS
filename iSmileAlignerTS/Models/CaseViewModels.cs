using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Web;
using iSmileAlignerTS.Validation;

namespace iSmileAlignerTS.Models
{
    public class CaseIndexViewModel
    {
        public string DoctorName { get; set; }
        public int CasesOpen { get; set; }
        public int CasesClosed { get; set; }
        public decimal CasesOpenSum { get; set; }
        public decimal CasesClosedSum { get; set; }
        public List<CaseSmallViewModel> CaseList { get; set; }
        public string CaseSearch { get; set; }
        public bool IsAdmin { get; set; }
        public bool isDoctor { get; set; }
    }

    public class MessageViewModel
    {
        public string FromUserEmail { get; set; }
        public string ToUserEmail { get; set; }
        public string DoctorName { get; set; }
        public string CaseNumber { get; set; }
        public string CaseNumberStr { get; set; }
        public string Message { get; set; }
        public string MessageDate { get; set; }
    }
    
    public class CaseSmallViewModel 
    {
        public long CaseId { get; set; }
        public int CaseNumber { get; set; }
        public string CaseNumberStr { get; set; }
        public string PatName { get; set; }
        public CaseStates Status { get; set; }
        public int BestelltWieAngegeben { get; set; }                   // 0 nix, 1 = Bestellt wie Angegeben, 2 = Bestellt mit Rücksprache
        public string BestelltWieAngegebenKommentar { get; set; }
        public string Doc { get; set; }
    }

    public class CaseFilesList
    {
        public long ImageId { get; set; }
        public long ThumbId { get; set; }
        public string Filename { get; set; }
        public CaseFileType FileType { get; set; }
        public DateTime FileDate { get; set; }
    }

    public class CaseViewModel
    {
        [Display(Name = "Fall ID")]
        public long CaseId { get; set; }
        [Display(Name = "Fallnummer")]
        public int CaseNumber { get; set; }                                     // Eine fortlaufende Nummer
        [Display(Name = "Arzt ID")]
        public string DoctorId { get; set; }                                    // welcher arzt / user
        [Display(Name = "Arzt")]
        public string DoctorName { get; set; }                                  // Arzt

        [Display(Name = "Anrede")]
        public string PatSalutation { get; set; }                               // Patientanrede
        [Display(Name = "Titel")]
        public string PatTitel { get; set; }                                    // ..
        [Display(Name = "Vorname")]
        public string PatFirstname { get; set; }
        [Display(Name = "Nachname")]
        public string PatLastName { get; set; }
        [Display(Name = "Geschlecht")]
        public string PatSex { get; set; }
        [Display(Name = "Geburtsdatum")]
        public String PatBirthDate { get; set; }
        [Display(Name = "Adresse")]
        public string PatAddress { get; set; }
        [Display(Name = "PLZ")]
        public string PatZIP { get; set; }
        [Display(Name = "Ort")]
        public string PatCity { get; set; }
        [Display(Name = "Land")]
        public string PatCountry { get; set; }
        [Display(Name = "Telephon")]
        public string PatPhone { get; set; }
        [Display(Name = "Mobil")]
        public string PatMobilPhone { get; set; }
        [Display(Name = "Pat.EMail")]
        public string PatEmail { get; set; }

        public bool Tooth11 { get; set; }                                       // Zahnschema
        public bool Tooth12 { get; set; }
        public bool Tooth13 { get; set; }
        public bool Tooth14 { get; set; }
        public bool Tooth15 { get; set; }
        public bool Tooth16 { get; set; }
        public bool Tooth17 { get; set; }
        public bool Tooth18 { get; set; }

        public bool Tooth21 { get; set; }
        public bool Tooth22 { get; set; }
        public bool Tooth23 { get; set; }
        public bool Tooth24 { get; set; }
        public bool Tooth25 { get; set; }
        public bool Tooth26 { get; set; }
        public bool Tooth27 { get; set; }
        public bool Tooth28 { get; set; }

        public bool Tooth31 { get; set; }
        public bool Tooth32 { get; set; }
        public bool Tooth33 { get; set; }
        public bool Tooth34 { get; set; }
        public bool Tooth35 { get; set; }
        public bool Tooth36 { get; set; }
        public bool Tooth37 { get; set; }
        public bool Tooth38 { get; set; }

        public bool Tooth41 { get; set; }
        public bool Tooth42 { get; set; }
        public bool Tooth43 { get; set; }
        public bool Tooth44 { get; set; }
        public bool Tooth45 { get; set; }
        public bool Tooth46 { get; set; }
        public bool Tooth47 { get; set; }
        public bool Tooth48 { get; set; }

        [Display(Name = "Behandlungskommentar")]
        [MaxLength(10000)]
        public string TreatComment { get; set; }

        public List<MessageViewModel> Messages { get; set; }

        [MaxLength(200)]
        public string Step01OK { get; set; }
        [MaxLength(200)]
        public string Step01UK { get; set; }
        [MaxLength(200)]
        public string Step02OK { get; set; }
        [MaxLength(200)]
        public string Step02UK { get; set; }
        [MaxLength(200)]
        public string Step03OK { get; set; }
        [MaxLength(200)]
        public string Step03UK { get; set; }
        [MaxLength(200)]
        public string Step04OK { get; set; }
        [MaxLength(200)]
        public string Step04UK { get; set; }
        [MaxLength(200)]
        public string Step05OK { get; set; }
        [MaxLength(200)]
        public string Step05UK { get; set; }
        [MaxLength(200)]
        public string Step06OK { get; set; }
        [MaxLength(200)]
        public string Step06UK { get; set; }
        [MaxLength(200)]
        public string Step07OK { get; set; }
        [MaxLength(200)]
        public string Step07UK { get; set; }
        [MaxLength(200)]
        public string Step08OK { get; set; }
        [MaxLength(200)]
        public string Step08UK { get; set; }
        [MaxLength(200)]
        public string Step09OK { get; set; }
        [MaxLength(200)]
        public string Step09UK { get; set; }
        [MaxLength(200)]
        public string Step10OK { get; set; }
        [MaxLength(200)]
        public string Step10UK { get; set; }
        [MaxLength(200)]
        public string Step11OK { get; set; }
        [MaxLength(200)]
        public string Step11UK { get; set; }
        [MaxLength(200)]
        public string Step12OK { get; set; }
        [MaxLength(200)]
        public string Step12UK { get; set; }

        [MaxLength(200)]
        public string Step01Kommentar { get; set; }
        [MaxLength(200)]
        public string Step02Kommentar { get; set; }
        [MaxLength(200)]
        public string Step03Kommentar { get; set; }
        [MaxLength(200)]
        public string Step04Kommentar { get; set; }
        [MaxLength(200)]
        public string Step05Kommentar { get; set; }
        [MaxLength(200)]
        public string Step06Kommentar { get; set; }
        [MaxLength(200)]
        public string Step07Kommentar { get; set; }
        [MaxLength(200)]
        public string Step08Kommentar { get; set; }
        [MaxLength(200)]
        public string Step09Kommentar { get; set; }
        [MaxLength(200)]
        public string Step10Kommentar { get; set; }
        [MaxLength(200)]
        public string Step11Kommentar { get; set; }
        [MaxLength(200)]
        public string Step12Kommentar { get; set; }

        [Display(Name = "Zeitplan")]
        [MaxLength(100)]
        public string Zeitplan { get; set; }

        [Display(Name = "Privatkommentar")]
        [MaxLength(1000)]
        public string PrivatKommentar { get; set; }

        [MaxFileSize(9 * 1024 * 1024, ErrorMessage = "Maximal 9MB")]
        [FileTypes("jpg,jpeg,png", ErrorMessage="JPG, JPEG oder PNG")]
        public HttpPostedFileBase PortraitFotoBin { get; set; }
        public string PortraitFotoName { get; set; }
        public long PortraitFotoId { get; set; }
        public long PortraitThumbId { get; set; }

        [MaxFileSize(9 * 1024 * 1024, ErrorMessage = "Maximal 9MB")]
        [FileTypes("jpg,jpeg,png", ErrorMessage = "JPG, JPEG oder PNG")]
        public HttpPostedFileBase PortraitSmilingFotoBin { get; set; }
        public string PortraitSmilingFotoName { get; set; }
        public long PortraitSmilingFotoId { get; set; }
        public long PortraitSmilingThumbId { get; set; }

        [MaxFileSize(9 * 1024 * 1024, ErrorMessage = "Maximal 9MB")]
        [FileTypes("jpg,jpeg,png", ErrorMessage = "JPG, JPEG oder PNG")]
        public HttpPostedFileBase ProfilFotoBin { get; set; }
        public string ProfilFotoName { get; set; }
        public long ProfilFotoId { get; set; }
        public long ProfilThumbId { get; set; }

        [MaxFileSize(9 * 1024 * 1024, ErrorMessage = "Maximal 9MB")]
        [FileTypes("jpg,jpeg,png", ErrorMessage = "JPG, JPEG oder PNG")]
        public HttpPostedFileBase ProfilSmilingFotoBin { get; set; }
        public string ProfilSmilingFotoName { get; set; }
        public long ProfilSmilingFotoId { get; set; }
        public long ProfilSmilingThumbId { get; set; }

        [MaxFileSize(9 * 1024 * 1024, ErrorMessage = "Maximal 9MB")]
        [FileTypes("jpg,jpeg,png", ErrorMessage = "JPG, JPEG oder PNG")]
        public HttpPostedFileBase PanoFotoBin { get; set; }
        public string PanoFotoName { get; set; }
        public long PanoFotoId { get; set; }
        public long PanoThumbId { get; set; }

        [MaxFileSize(9 * 1024 * 1024, ErrorMessage = "Maximal 9MB")]
        [FileTypes("jpg,jpeg,png", ErrorMessage = "JPG, JPEG oder PNG")]
        public HttpPostedFileBase DistXFotoBin { get; set; }
        public string DistXFotoName { get; set; }
        public long DistXFotoId { get; set; }
        public long DistXThumbId { get; set; }

        [MaxFileSize(9 * 1024 * 1024, ErrorMessage = "Maximal 9MB")]
        [FileTypes("jpg,jpeg,png", ErrorMessage = "JPG, JPEG oder PNG")]
        public HttpPostedFileBase DataFotoBin { get; set; }
        public string DataFotoName { get; set; }
        public long DataFotoId { get; set; }
        public long DataThumbId { get; set; }

        [MaxFileSize(9 * 1024 * 1024, ErrorMessage = "Maximal 9MB")]
        [FileTypes("jpg,jpeg,png", ErrorMessage = "JPG, JPEG oder PNG")]
        public HttpPostedFileBase ModelBeforeFrontFotoBin { get; set; }
        public string ModelBeforeFrontFotoName { get; set; }
        public long ModelBeforeFrontFotoId { get; set; }
        public long ModelBeforeFrontThumbId { get; set; }

        [MaxFileSize(9 * 1024 * 1024, ErrorMessage = "Maximal 9MB")]
        [FileTypes("jpg,jpeg,png", ErrorMessage = "JPG, JPEG oder PNG")]
        public HttpPostedFileBase ModelAfterFrontFotoBin { get; set; }
        public string ModelAfterFrontFotoName { get; set; }
        public long ModelAfterFrontFotoId { get; set; }
        public long ModelAfterFrontThumbId { get; set; }

        [MaxFileSize(9 * 1024 * 1024, ErrorMessage = "Maximal 9MB")]
        [FileTypes("jpg,jpeg,png", ErrorMessage = "JPG, JPEG oder PNG")]
        public HttpPostedFileBase ModelBeforeRightFotoBin { get; set; }
        public string ModelBeforeRightFotoName { get; set; }
        public long ModelBeforeRightFotoId { get; set; }
        public long ModelBeforeRightThumbId { get; set; }

        [MaxFileSize(9 * 1024 * 1024, ErrorMessage = "Maximal 9MB")]
        [FileTypes("jpg,jpeg,png", ErrorMessage = "JPG, JPEG oder PNG")]
        public HttpPostedFileBase ModelAfterRightFotoBin { get; set; }
        public string ModelAfterRightFotoName { get; set; }
        public long ModelAfterRightFotoId { get; set; }
        public long ModelAfterRightThumbId { get; set; }

        [MaxFileSize(9 * 1024 * 1024, ErrorMessage = "Maximal 9MB")]
        [FileTypes("jpg,jpeg,png", ErrorMessage = "JPG, JPEG oder PNG")]
        public HttpPostedFileBase ModelMaxFotoBin { get; set; }
        public string ModelMaxFotoName { get; set; }
        public long ModelMaxFotoId { get; set; }
        public long ModelMaxThumbId { get; set; }

        [MaxFileSize(9 * 1024 * 1024, ErrorMessage = "Maximal 9MB")]
        [FileTypes("jpg,jpeg,png", ErrorMessage = "JPG, JPEG oder PNG")]
        public HttpPostedFileBase ModelMandFotoBin { get; set; }
        public string ModelMandFotoName { get; set; }
        public long ModelMandFotoId { get; set; }
        public long ModelMandThumbId { get; set; }

        [MaxFileSize(9 * 1024 * 1024, ErrorMessage = "Maximal 9MB")]
        [FileTypes("jpg,jpeg,png", ErrorMessage = "JPG, JPEG oder PNG")]
        public HttpPostedFileBase ModelBeforeOKUKFotoBin { get; set; }
        public string ModelBeforeOKUKFotoName { get; set; }
        public long ModelBeforeOKUKFotoId { get; set; }
        public long ModelBeforeOKUKThumbId { get; set; }

        [MaxFileSize(9 * 1024 * 1024, ErrorMessage = "Maximal 9MB")]
        [FileTypes("jpg,jpeg,png", ErrorMessage = "JPG, JPEG oder PNG")]
        public HttpPostedFileBase ModelAfterOKUKFotoBin { get; set; }
        public string ModelAfterOKUKFotoName { get; set; }
        public long ModelAfterOKUKFotoId { get; set; }
        public long ModelAfterOKUKThumbId { get; set; }


        [MaxFileSize(20 * 1024 * 1024, ErrorMessage = "Maximal 20MB")]
        [FileTypes("stl", ErrorMessage = "STL Modell")]
        public HttpPostedFileBase ModelBeforeOKSltBin { get; set; }
        public string ModelBeforeOKSltName { get; set; }
        public long ModelBeforeOKSltId { get; set; }

        [MaxFileSize(20 * 1024 * 1024, ErrorMessage = "Maximal 20MB")]
        [FileTypes("stl", ErrorMessage = "STL Modell")]
        public HttpPostedFileBase ModelBeforeOKUKSltBin { get; set; }
        public string ModelBeforeOKUKSltName { get; set; }
        public long ModelBeforeOKUKSltId { get; set; }

        [MaxFileSize(20 * 1024 * 1024, ErrorMessage = "Maximal 20MB")]
        [FileTypes("stl", ErrorMessage = "STL Modell")]
        public HttpPostedFileBase ModelBeforeUKSltBin { get; set; }
        public string ModelBeforeUKSltName { get; set; }
        public long ModelBeforeUKSltId { get; set; }

        [MaxFileSize(20 * 1024 * 1024, ErrorMessage = "Maximal 20MB")]
        [FileTypes("stl", ErrorMessage = "STL Modell")]
        public HttpPostedFileBase ModelAfterOKSltBin { get; set; }
        public string ModelAfterOKSltName { get; set; }
        public long ModelAfterOKSltId { get; set; }

        [MaxFileSize(20 * 1024 * 1024, ErrorMessage = "Maximal 20MB")]
        [FileTypes("stl", ErrorMessage = "STL Modell")]
        public HttpPostedFileBase ModelAfterUKSltBin { get; set; }
        public string ModelAfterUKSltName { get; set; }
        public long ModelAfterUKSltId { get; set; }

        [MaxFileSize(20 * 1024 * 1024, ErrorMessage = "Maximal 20MB")]
        [FileTypes("stl", ErrorMessage = "STL Modell")]
        public HttpPostedFileBase ModelAfterOKUKSltBin { get; set; }
        public string ModelAfterOKUKSltName { get; set; }
        public long ModelAfterOKUKSltId { get; set; }

        [MaxFileSize(9 * 1024 * 1024, ErrorMessage = "Maximal 9MB")]
        [FileTypes("jpg,jpeg,png", ErrorMessage = "JPG, JPEG oder PNG")]
        public HttpPostedFileBase AddFotoBin { get; set; }

        [MaxFileSize(20 * 1024 * 1024, ErrorMessage = "Maximal 19MB")]
        [FileTypes("*", ErrorMessage = "Jede Datei")]
        public HttpPostedFileBase AddAnyFileBin { get; set; }

        [MaxFileSize(100 * 1024 * 1024, ErrorMessage = "Maximal 100MB")]
        [FileTypes("pdf", ErrorMessage = "PDF")]
        public HttpPostedFileBase AddAnyPDFBin { get; set; }

        [MaxFileSize(20 * 1024 * 1024, ErrorMessage = "Maximal 20MB")]
        [FileTypes("stl", ErrorMessage = "STL Modell")]
        public HttpPostedFileBase AddModelBin { get; set; }
        public CaseFileType AddModelType { get; set; }

        [MaxFileSize(100 * 1024 * 1024, ErrorMessage = "Maximal 100MB")]
        [FileTypes("pdf", ErrorMessage = "PDF")]
        public HttpPostedFileBase AddPDFBin { get; set; }
        public long PDFDocumentID { get; set; }
        public string PDFDocumentFilename { get; set; }

        [MaxFileSize(500 * 1024 * 1024, ErrorMessage = "Maximal 100MB")]
        [FileTypes("3sz", ErrorMessage = "3sz")]
        public HttpPostedFileBase AddProjektBin { get; set; }
        public long ProjektID { get; set; }
        public string ProjektFilename { get; set; }

        [MaxFileSize(10 * 1024 * 1024, ErrorMessage = "Maximal 10MB")]
        [FileTypes("gif", ErrorMessage = "gif")]
        public HttpPostedFileBase FrontAnimationGifBin { get; set; }
        public long FrontAnimationGifID { get; set; }
        public string FrontAnimationGifFilename { get; set; }

        [MaxFileSize(10 * 1024 * 1024, ErrorMessage = "Maximal 10MB")]
        [FileTypes("gif", ErrorMessage = "gif")]
        public HttpPostedFileBase SideAnimationGifBin { get; set; }
        public long SideAnimationGifID { get; set; }
        public string SideAnimationGifFilename { get; set; }

        [MaxFileSize(10 * 1024 * 1024, ErrorMessage = "Maximal 10MB")]
        [FileTypes("gif", ErrorMessage = "gif")]
        public HttpPostedFileBase OcclAnimationGifBin { get; set; }
        public long OcclAnimationGifID { get; set; }
        public string OcclAnimationGifFilename { get; set; }

        
        // liste sonstiger Bilder
        //
        public List<CaseFilesList> Files { get; set; }

        [Display(Name = "Nachricht")]
        [MaxLength(1000)]
        public string CurrentMessage { get; set; }

        [Display(Name = "Stripping")]
        public CaseStripping Stripping { get; set; }
        [Display(Name = "Klasse II/III Behandlung")]
        public bool AddTreatClassII { get; set; }
        [Display(Name = "Suspender")]
        public bool AddTreatSuspender { get; set; }
        [Display(Name = "Extraktionsfall")]
        public bool AddTreatExtract { get; set; }
        [Display(Name = "Retainer Hart")]
        public bool AddTreatRetainer { get; set; }
        [Display(Name = "Bleich Schiene")]
        public bool AddTreatRail { get; set; }
        [Display(Name = "Extrusionsfall")]
        public bool AddTreatExtrusion { get; set; }
        [Display(Name = "Knöpfe u. Elastics")]
        public bool AddTreatButtons { get; set; }
        [Display(Name = "Pontic / Zahn Lückenfüller")]
        public bool AddTreatPontic { get; set; }
        [Display(Name = "Retainer Draht")]
        public bool AddTreatWire { get; set; }
        [Display(Name = "Attachments")]
        public bool AddTreatAttachment { get; set; }

        [Display(Name = "Fallstatus")]
        public CaseStates CaseState { get; set; }               // 1=Planung 2=Planung bez. 3=Fallpl.1 4=Fallpl. 2 5=Fallpl. 3 6=Herst.bez. 7=Herstellung 8=Versand 9=Fertig
        [Range(-10000.0,10000.0)]
        public decimal Payment1 { get; set; }                   // Zahlung 1
        public DateTime? Payment1Date { get; set; }             // Zahlungdatum
        [Range(-10000.0, 10000.0)]
        public decimal Payment2 { get; set; }                   // Zahlung 2
        public DateTime? Payment2Date { get; set; }             // Datum 2

        [Display(Name = "Akzeptieren")]
        public bool Accepted { get; set; }
        public DateTime AcceptedDate { get; set; }

        public List<Wares> Wares { get; set; }
        public List<CaseCart> Carts { get; set; }
        public String AddWareItem { get; set; }
        public decimal AddWareAmount { get; set; }
        public bool isDoctorShop { get; set; }

        public String CaseNumberStr { get; set; }              // Fall-Nummer für Thomas AT SCHNELL 2015101301
        public DateTime ErstellungsDatum { get; set; }

        public String DeleteWord { get; set; }

        public bool ShowCanvasModels { get; set; }

        public byte[] RowVersion { get; set; }

        public int BestelltWieAngegeben { get; set; }                   // 0 nix, 1 = Bestellt wie Angegeben, 2 = Bestellt mit Rücksprache
        public string BestelltWieAngegebenKommentar { get; set; }       // Kommentar
        public DateTime BestelltWieAngegebenDatum { get; set; }         // Wann das war
    }
}
