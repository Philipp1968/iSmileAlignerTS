using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace iSmileAlignerTS.Models
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
        public bool CanCreateCase { get; set; }
        public bool CanVerifyUser { get; set; }
        public bool CanPurchase { get; set; }
        public bool NeedVerify { get; set; }
        public bool CanViewCases { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "\"{0}\" muss mindestens {2} Zeichen lang sein.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Neues Kennwort")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Neues Kennwort bestätigen")]
        [Compare("NewPassword", ErrorMessage = "Das neue Kennwort stimmt nicht mit dem Bestätigungskennwort überein.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Aktuelles Kennwort")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "\"{0}\" muss mindestens {2} Zeichen lang sein.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Neues Kennwort")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Neues Kennwort bestätigen")]
        [Compare("NewPassword", ErrorMessage = "Das neue Kennwort stimmt nicht mit dem Bestätigungskennwort überein.")]
        public string ConfirmPassword { get; set; }
    }

    public class SetUserPasswordViewModel
    {
        public string UserID { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string newPassword { get; set; }
        public int NumberOfCases { get; set; }
    }

    public class VerifyUserDataViewModel
    {
        [ScaffoldColumn(false)]
        public string UserID { get; set; }
        [MaxLength(20)]
        [Display(Name = "Titel")]
        public string Titel { get; set; }
        [MaxLength(20)]
        [Display(Name = "Anrede")]
        public string Salutation { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(2)]
        [Display(Name = "Vorname")]
        public string Firstname { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(2)]
        [Display(Name = "Nachname")]
        public string Lastname { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Geburtsdatum")]
        public DateTime BirthDate { get; set; }
        [MaxLength(20)]
        [Display(Name = "Fachgebiet")]
        public string MedicalField { get; set; }
        [MaxLength(50)]
        [Display(Name = "Ordination")]
        public string Praxis { get; set; }
        [MaxLength(50)]
        [Display(Name = "Adresse")]
        public string Address { get; set; }
        [MaxLength(12)]
        [Display(Name = "Postleitzahl")]
        public string ZIP { get; set; }
        [MaxLength(50)]
        [Display(Name = "Ort")]
        public string City { get; set; }
        [MaxLength(10)]
        [Display(Name = "Land")]
        public string Country { get; set; }
        [MaxLength(20)]
        [Display(Name = "Telephon")]
        public string Phone { get; set; }
        [MaxLength(20)]
        [Display(Name = "Mobilnummer")]
        public string MobilPhone { get; set; }
        [MaxLength(20)]
        [Display(Name = "Fax")]
        public string Fax { get; set; }
        [MaxLength(100)]
        [Display(Name = "Web")]
        public string WebURL { get; set; }
        [MaxLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Änderung Am")]
        public DateTime? NeedVerifyDate { get; set; }
        [MaxLength(6)]
        [Display(Name = "Kurzname")]
        public string DocShortName { get; set; }
    }

    public class ChangeUserDataViewModel
    {
        [ScaffoldColumn(false)]
        public string UserID { get; set; }
        [MaxLength(20)]
        [UIHint("DropDown")]
        [Display(Name = "Titel")]
        public string Titel { get; set; }
        [MaxLength(20)]
        [UIHint("DropDown")]
        [Display(Name = "Anrede")]
        public string Salutation { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(2)]
        [Display(Name = "Vorname")]
        public string Firstname { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(2)]
        [Display(Name = "Nachname")]
        public string Lastname { get; set; }
        [Required]
        [Display(Name = "Geburtsdatum")]
        public string BirthDate { get; set; }
        [MaxLength(20)]
        [UIHint("DropDown")]
        [Display(Name = "Fachgebiet")]
        public string MedicalField { get; set; }
        [MaxLength(50)]
        [Display(Name = "Ordination")]
        public string Praxis { get; set; }
        [MaxLength(50)]
        [Display(Name = "Adresse")]
        public string Address { get; set; }
        [MaxLength(12)]
        [Display(Name = "Postleitzahl")]
        public string ZIP { get; set; }
        [MaxLength(50)]
        [Display(Name = "Ort")]
        public string City { get; set; }
        [MaxLength(30)]
        [UIHint("DropDown")]
        [Display(Name = "Land")]
        public string CountryCode { get; set; }
        [MaxLength(20)]
        [Display(Name = "Telephon")]
        public string Phone { get; set; }
        [MaxLength(20)]
        [Display(Name = "Mobilnummer")]
        public string MobilPhone { get; set; }
        [MaxLength(20)]
        [Display(Name = "Fax")]
        public string Fax { get; set; }
        [MaxLength(100)]
        [Display(Name = "Web")]
        public string WebURL { get; set; }
        [Display(Name = "Partner-Arzt")]
        public bool isDoctor { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Telefonnummer")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Telefonnummer")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }
}