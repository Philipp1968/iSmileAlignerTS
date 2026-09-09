using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iSmileAlignerTS.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Diesen Browser merken?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "E-Mail")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Kennwort")]
        public string Password { get; set; }

        [Display(Name = "Merken?")]
        public bool RememberMe { get; set; }

        public bool DisableLogin { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "\"{0}\" muss mindestens {2} Zeichen lang sein.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Kennwort")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Kennwort bestätigen")]
        [Compare("Password", ErrorMessage = "Das Kennwort entspricht nicht dem Bestätigungskennwort.")]
        public string ConfirmPassword { get; set; }

        [MaxLength(20)]
        [UIHint("DropDown")]
        [Display(Name = "Titel")]
        public string Titel { get; set; }

        [MaxLength(20)]
        [UIHint("DropDown")]
        [Display(Name = "Anrede")]
        public string Salutation { get; set; }

        [MaxLength(50)]
        [MinLength(2)]
        [Display(Name = "Vorname")]
        public string Firstname { get; set; }

        [MaxLength(50)]
        [MinLength(2)]
        [Display(Name = "Nachname")]
        public string Lastname { get; set; }

        [Required]
        [Display(Name = "Geburtsdatum")]
        public string BirthDate { get; set; }
        
        [MaxLength(50)]
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
        public bool isPatient { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "\"{0}\" muss mindestens {2} Zeichen lang sein.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Kennwort")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Kennwort bestätigen")]
        [Compare("Password", ErrorMessage = "Das Kennwort stimmt nicht mit dem Bestätigungskennwort überein.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }
    }
}
