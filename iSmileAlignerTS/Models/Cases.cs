using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


// https://coding.abel.nu/2012/03/ef-migrations-command-reference/
//•Enable-Migrations: Enables Code First Migrations in a project.
//•Add-Migration: Scaffolds a migration script for any pending model changes.
//•Update-Database: Applies any pending migrations to the database.
//•Get-Migrations: Displays the migrations that have been applied to the target database.

namespace iSmileAlignerTS.Models
{
    public class Cases
    {
        public long Id { get; set; }
        [Index]
        public int CaseNumber { get; set; }                                     // Eine fortlaufende Nummer
        public string DoctorId { get; set; }                                    // welcher arzt / user
        public CaseStates CaseState { get; set; }                               // 1=Planung 2=Planung bez. 3=Fallpl.1 4=Fallpl. 2 5=Fallpl. 3 6=Herst.bez. 7=Herstellung 8=Versand 9=Fertig

        [MaxLength(20)]
        public string PatSalutation { get; set; }                               // Patientanrede
        [MaxLength(20)]
        public string PatTitel { get; set; }                                    // ..
        [Index]
        [MaxLength(200)]
        public string PatFirstname { get; set; }
        [Index]
        [MaxLength(200)]
        public string PatLastName { get; set; }
        [MaxLength(2)]                                                          // M, W
        public string PatSex { get; set; }
        public DateTime? PatBirthDate { get; set; }
        [MaxLength(200)]
        public string PatAddress { get; set; }
        [MaxLength(12)]
        public string PatZIP { get; set; }
        [MaxLength(200)]
        public string PatCity { get; set; }
        [MaxLength(10)]
        public string PatCountry { get; set; }
        [MaxLength(20)]
        public string PatPhone { get; set; }
        [MaxLength(20)]
        public string PatMobilPhone { get; set; }
        [MaxLength(100)]
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

        [MaxLength(10000)]
        public string TreatComment { get; set; }

        public CaseStripping Stripping { get; set; }
        public bool AddTreatClassII { get; set; }
        public bool AddTreatSuspender { get; set; }
        public bool AddTreatExtract { get; set; }
        public bool AddTreatRetainer { get; set; }
        public bool AddTreatRail { get; set; }
        public bool AddTreatExtrusion { get; set; }
        public bool AddTreatButtons { get; set; }
        public bool AddTreatPontic { get; set; }
        public bool AddTreatWire { get; set; }
        public bool AddTreatAttachment { get; set; }

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
        [MaxLength(500)]
        public string Zeitplan { get; set; }
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



        [MaxLength(1000)]
        public string PrivatKommentar { get; set; }

        public decimal Payment1 { get; set; }                   // Zahlung 1
        public DateTime? Payment1Date { get; set; }             // Zahlungdatum
        public decimal Payment2 { get; set; }                   // Zahlung 2
        public DateTime? Payment2Date { get; set; }             // Datum 2

        public bool Accepted { get; set; }
        public DateTime AcceptedDate { get; set; }

        [MaxLength(200)]
        [Index]
        public String CaseNumberStr { get; set; }              // Fall-Nummer für Thomas AT SCHNELL 2015101301

        public DateTime ErstellungsDatum { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public int BestelltWieAngegeben { get; set; }                   // 0 nix, 1 = Bestellt wie Angegeben, 2 = Bestellt mit Rücksprache
        public string BestelltWieAngegebenKommentar { get; set; }       // Kommentar
        public DateTime BestelltWieAngegebenDatum { get; set; }         // Wann das war
    }

    public class CaseBestelltViewModel
    {
        public int CaseId { get; set; }
        public int BestelltWieAngegeben { get; set; }
        public string BestelltWieAngegebenKommentar { get; set; }
        public DateTime BestelltWieAngegebenDatum { get; set; }
    }
}

