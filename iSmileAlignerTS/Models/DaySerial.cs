using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSmileAlignerTS.Models
{
    public class DaySerial
    {
        public long Id { get; set; }
        [Index(IsUnique=true)]
        public DateTime Day { get; set; }
        public int Number { get; set; }
    }
}