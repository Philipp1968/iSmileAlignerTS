using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace iSmileAlignerTS.Models
{
    public enum PAYRELSKIND  { Invoice = 0, Delivery = 1, InvAndDel = 2}

    // Sie können Profildaten für den Benutzer durch Hinzufügen weiterer Eigenschaften zur ApplicationUser-Klasse hinzufügen. Weitere Informationen finden Sie unter "http://go.microsoft.com/fwlink/?LinkID=317594".
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Beachten Sie, dass der "authenticationType" mit dem in "CookieAuthenticationOptions.AuthenticationType" definierten Typ übereinstimmen muss.
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Benutzerdefinierte Benutzeransprüche hier hinzufügen
            return userIdentity;
        }

        [MaxLength(20)]
        public String Titel { get; set; }
        [MaxLength(20)]
        public String Salutation { get; set; }
        [MaxLength(50)]
        [MinLength(2)]
        public String Firstname { get; set; }
        [MaxLength(50)]
        [MinLength(2)]
        public String Lastname { get; set; }
        public DateTime BirthDate { get; set; }
        [MaxLength(20)]
        public String MedicalField { get; set; }
        [MaxLength(50)]
        public String Praxis { get; set; }
        [MaxLength(50)]
        public String Address { get; set; }
        [MaxLength(12)]
        public String ZIP { get; set; }
        [MaxLength(50)]
        public String City { get; set; }
        [MaxLength(50)]
        public String Country { get; set; }
        [MaxLength(20)]
        public String Phone { get; set; }
        [MaxLength(20)]
        public String MobilPhone { get; set; }
        [MaxLength(20)]
        public String Fax { get; set; }
        [MaxLength(100)]
        public String WebURL { get; set; }

        public Boolean isAdmin { get; set; }                                        // Admin, der sozusagen überall rein darf
        public Boolean isDoctor { get; set; }                                       // bei Registrierung wird auf Ja gesetzt, falls als Arzt mitmachen will
        public Boolean needVerify { get; set; }                                     // bei Neuanlage oder Datenänderung
        public DateTime? needVerifyDate { get; set; }                               // wann Neuanlage/Änderung war
        public Boolean isVerified { get; set; }                                     // von Admin freigeschaltet
        public DateTime? isVerfiedDate { get; set; }                                // wann das freigeschaltet wurde
        public Boolean isDeactivated { get; set; }                                  // deaktiviertes Konto
        public Boolean isCustomer { get; set; }                                     // einfacher TS Kunde

        [MaxLength(2)]
        public String CountryCode { get; set; }                                     // Wenn dieser Arzt etwas anlegt welcher Länder-Code
        [MaxLength(100)]
        public String DocShortName { get; set; }                                    // Kürzel vom Arzt, 5 Zeichen, Upper

        public bool hasFastInternet { get; set; }                                   // 3D Modell gleich anzeigen, oder mit Schalter aktivieren

        public PAYRELSKIND paysInvoices { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            this.Database.CommandTimeout = 300;
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }


        public DbSet<Settings> Settings { get; set; }
        public DbSet<Messages> Messages { get; set; }
        public DbSet<CaseFiles> CaseFiles { get; set; }
        public DbSet<Cases> Cases { get; set; }
        public DbSet<Payment> Paymet { get; set; }
        public DbSet<Wares> Wares { get; set; }
        public DbSet<CaseCart> CaseCart { get; set; }
        public DbSet<DaySerial> DaySerial { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Journal> Journal { get; set; }
        public DbSet<Laender> Laender { get; set; }
        public DbSet<CustomerCases> CustomerCases { get; set; }
    }
}