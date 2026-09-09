using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSmileAlignerTS.Models
{
    public class CaseFiles
    {
        public long Id { get; set; }
        [Index]
        public long CaseId { get; set; }
        [Index]
        [MaxLength(200)]
        public string Filename { get; set; }
        [MaxLength(100)]
        public string ContentType { get; set; }
        public int ContentLength { get; set; }
        public byte[] Content { get; set; }
        public DateTime? FileDate { get; set; }             // Datum der Aufnahme, Sortierung
        public CaseFileType FileType { get; set; }
        public bool isDeleted { get; set; }
        public bool isThumbNail { get; set; }               // kleines Bild, oder großes Bild
        [Index]
        public long FileNum { get; set; }                    // für gleiche bilder normal & thumb, bzw dateien
    }
}