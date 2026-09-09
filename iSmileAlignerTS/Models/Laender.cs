using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSmileAlignerTS.Models
{
    public enum LaenderZoneEnum
    {
        AT = 1,
        EU = 2,
        Dritt = 3
    }

    public class Laender
    {
        public long Id { get; set; }

        [Display(Name = "ISO")]
        [Index]
        [MinLength(2)]
        [MaxLength(5)]
        public string ISOCode { get; set; }

        [Display(Name = "PLZ-Präfix")]
        [Index]
        [MinLength(2)]
        [MaxLength(5)]
        public string PLZCode { get; set; }

        [MaxLength(100)]
        public string Land { get; set; }

        [Display(Name = "Abrechnungszone")]
        [Required]
        public LaenderZoneEnum Zone { get; set; }

        [ScaffoldColumn(false)]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
