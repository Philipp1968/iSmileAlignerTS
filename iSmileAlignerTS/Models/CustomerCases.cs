using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace iSmileAlignerTS.Models
{
    public class CustomerCases
    {
        public long Id { get; set; }

        [Index]
        public long CasesId { get; set; }

        [Index]
        [MaxLength(128)]
        public string UserId { get; set; }
    }


    public class CustomerCasesView
    {
        public List<SelectListItem> Users { get; set; }
        public List<SelectListItem> Cases { get; set; }
        public List<SelectListItem> Assigns { get; set; }

        public string UserId { get; set; }
        public string UserSearch { get; set; }

        public long CasesId { get; set; }
        public string CasesSearch { get; set; }

        public long AssignId { get; set; }

        public bool DoAktion { get; set; }
    }

}