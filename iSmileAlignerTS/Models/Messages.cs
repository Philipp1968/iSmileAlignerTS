using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSmileAlignerTS.Models
{
    public class Messages
    {
        public long Id { get; set; }
        [MaxLength(100)]
        public string FromUserEmail { get; set; }
        [MaxLength(100)]
        public string ToUserEmail { get; set; }
        [Index]
        public long CaseId { get; set; }
        [MaxLength(1000)]
        public string Msg { get; set; }
        public DateTime MessageDate { get; set; }
        public bool isReplied { get; set; }
    }
}