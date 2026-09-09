using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSmileAlignerTS.Models
{
    public class Settings
    {
        public long Id { get; set; }
        [Index]
        [MaxLength(30)]
        public string Name { get; set; }
        [MaxLength(1000)]
        public string Value { get; set; }
    }
}