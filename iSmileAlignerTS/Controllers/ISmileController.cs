using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using iSmileAlignerTS.Models;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Text;
using Newtonsoft.Json;
using System.Globalization;
using System.Data.Entity;
using System.Text.RegularExpressions;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.ReportSource;
using CrystalDecisions.Shared;
using System.Web.Hosting;
using System.Net.Mail;
using System.Net.Mime;
using iSmileAlignerTS;
using System.Threading;

namespace ISmileAlignerTS.Controllers
{
    public enum SHOPART { OldShop = 1, DoctorShop = 2, PatientShop = 3, DevelShop = 10 };

    public class ISmileController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager = null;
        private ApplicationDbContext _db = null;
        private SHOPART _shopKind = SHOPART.DevelShop;
        private string CurrentLanguage { get; set; }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                if (_userManager == null)
                {
                    _userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                }

                return _userManager;
            }
            set
            {
                _userManager = value;
            }
        }

        public static string s(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            return s.Trim();
        }

        public static string s(string s, int l)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            if (s.Length < l) return s;
            return s.Substring(0, l).Trim();
        }

        public ApplicationDbContext db
        {
            get
            {
                if (_db == null)
                {
                    _db = new ApplicationDbContext();
                }
                return _db;
            }

            set
            {
                _db = value;
            }
        }

        public SHOPART shopKind
        {
            get
            {
                return _shopKind;
            }

            set
            {
                _shopKind = value;
            }
        }


        // Controller auflösen
        //
        protected override void Dispose(bool disposing)
        {
            if (disposing && _signInManager != null)
            {
                _signInManager.Dispose();
                _signInManager = null;
            }

            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            if (disposing && _db != null)
            {
                _db.Dispose();
                _db = null;
            }

            base.Dispose(disposing);
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string cultureName = null;

            // Attempt to read the culture cookie from Request
            HttpCookie cultureCookie = Request.Cookies["_culture"];
            if (cultureCookie != null && !string.IsNullOrWhiteSpace(cultureCookie.Value))
            {
                cultureName = cultureCookie.Value;
            }
            else
            {
                if (Request.UserLanguages != null && Request.UserLanguages.Length > 0)
                {
                    int i;
                    cultureName= "en-US";
                    for(i=0; i<Request.UserLanguages.Length; i+= 1)
                    {
                        string m = Request.UserLanguages[i].ToLower();
                        if (!string.IsNullOrWhiteSpace(m))
                        {
                            int j = m.IndexOf(';');
                            if (j > 0) m = m.Substring(0, j);
                            if (m.Length >= 2 && m.Substring(0,2) == "de")
                            {
                                cultureName = m;
                                break;
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(cultureName))
            {
                cultureName = "de-AT";
            }

            int pos = cultureName.IndexOf("-");
            if ((pos > 0 && cultureName.Substring(0, pos) == "de") || cultureName == "de")
            {
                cultureName = "de-AT";
            }
            else
            {
                cultureName = "en-US";
            }

            // Modify current thread's cultures         
            //
            CultureInfo ci = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);
            Thread.CurrentThread.CurrentUICulture = ci;
            
            MvcApplication.ExpandViewEnginesSearchPath(cultureName);

            return base.BeginExecuteCore(callback, state);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string cultureName = null;

            // Attempt to read the culture cookie from Request
            HttpCookie cultureCookie = Request.Cookies["_culture"];
            if (cultureCookie != null && !string.IsNullOrWhiteSpace(cultureCookie.Value))
            {
                cultureName = cultureCookie.Value;
            }
            else
            {
                if (Request.UserLanguages != null && Request.UserLanguages.Length > 0)
                {
                    int i;
                    cultureName = "en-US";
                    for (i = 0; i < Request.UserLanguages.Length; i += 1)
                    {
                        string m = Request.UserLanguages[i].ToLower();
                        if (!string.IsNullOrWhiteSpace(m))
                        {
                            int j = m.IndexOf(';');
                            if (j > 0) m = m.Substring(0, j);
                            if (m.Length >= 2 && m.Substring(0, 2) == "de")
                            {
                                cultureName = m;
                                break;
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(cultureName))
            {
                cultureName = "de-AT";
            }

            int pos = cultureName.IndexOf("-");
            if ((pos > 0 && cultureName.Substring(0, pos) == "de") || cultureName == "de")
            {
                cultureName = "de-AT";
            }
            else
            {
                cultureName = "en-US";
            }

            // Modify current thread's cultures         
            //
            CultureInfo ci = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);
            Thread.CurrentThread.CurrentUICulture = ci;

            MvcApplication.ExpandViewEnginesSearchPath(cultureName);

            base.OnActionExecuting(filterContext);
        }


        // ViewBag Hilfsfunktionen
        //
        public void PrepareViewBagLandISOList(string def)
        {
            string sel = (def == null) ? "" : def;

            ViewBag.LandISOList = db.Laender.OrderBy(x => x.Zone)
                .ThenBy(x => x.Land)
                .ToList()
                .Select(c => new SelectListItem
                {
                    Text = c.Land,
                    Value = c.ISOCode,
                    Selected = c.ISOCode == sel
                })
                .ToList();
        }


        // Welcher Shop bin ich?
        //
        public SHOPART WhichShop(HttpRequestBase r)
        {
            string host = (r.Url.Host ?? "unknown").ToLower();
            string h = host;
            SHOPART result = SHOPART.OldShop;
            int pos = host.IndexOf(".");
            if (pos > 0)
            {
                host = host.Substring(0, pos);
            }
            if (host == "doctors") result = SHOPART.DoctorShop;
            if (host == "patients") result = SHOPART.PatientShop;
            if (host == "localhost") result = SHOPART.DevelShop;
            MvcApplication.logMsg("shop " + h + "=" + result.ToString());
            return result;
        }


        // CaseCarts bestimmen die noch nicht bezahlt wurden
        //
        public IEnumerable<CaseCart> GetUnpayedCart(int cartNum)
        {
            IEnumerable<CaseCart> carts= db.CaseCart.Where(x => x.CartNumber == cartNum && x.isPayed == false).OrderBy(y => y.CreateDate);
            return carts;
        }


        // GetCart --> CartNumber, bestimme erste/niedrigste
        //
        public int GetCartNumber(int caseNum, String uid)
        {
            IEnumerable<CaseCart> carts = null;
            int result = 0;

            if (caseNum == 0 && uid != null)
            {
                carts = db.CaseCart.Where(x => x.CaseNumber == 0 && x.UserId == uid).OrderBy(x => x.CartNumber);
            }
            else if (caseNum != 0 && uid == null)
            {
                carts = db.CaseCart.Where(x => x.CaseNumber == caseNum).OrderBy(x => x.CartNumber);
            }
            else if (caseNum != 0 && uid != null)
            {
                carts = db.CaseCart.Where(x => x.UserId == uid && x.CaseNumber == caseNum).OrderBy(x => x.CartNumber);
            }
            if (carts != null && carts.Count() > 0)
            {
                result = carts.First().CartNumber;
            }
            carts = null;
            return result;
        }


        // Schicke Email
        //
        public static bool SendeEmail(string to, string subject, string body)
        {
            bool didsend = false;
            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("info@thinortho.com");
                msg.To.Add(new MailAddress(to));
                msg.Subject = subject;
                msg.Body = body ?? " ";
                msg.IsBodyHtml = false;
                msg.BodyEncoding = Encoding.UTF8;
                System.Net.Mime.ContentType content = new ContentType("text/plain");
                msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body ?? " ", Encoding.UTF8, MediaTypeNames.Text.Plain));
                SmtpClient smtp = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                try
                {
                    smtp.Credentials = new System.Net.NetworkCredential("azure_fbb8568967aaf7a9cbea09e721ce16e1@azure.com", "045a8079ad05435191efec6d32d12aff");
                    smtp.Send(msg);
                    didsend = true;
                }
                catch (Exception ex)
                {
                    MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + (to ?? "<to>") + " " + (subject ?? "<sub>") + " " + ObjectDumper.Dump(ex));
                }
                msg.Dispose();
                msg = null;
                smtp.Dispose();
                smtp = null;
            }
            catch (Exception ex)
            {
                MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + (to ?? "<to>") + " " + (subject ?? "<sub>") + " " + ObjectDumper.Dump(ex));
                didsend = false;
            }
            return didsend;
        }


        // Datum t.m.j, j/m/t, j-m-t
        // 
        public static bool GuessDateFromString(String str, ref DateTime val)
        {
            bool result = false;
            string copy = (str == null || str.Length == 0) ? " " : str.Trim();
            int tag = -1;
            int monat = -1;
            int jahr = -1;
            int jahrnow = DateTime.Now.Year;

            Regex r = new Regex(@"^(\d+)\.(\d+)\.(\d+)$");
            Match m = r.Match(copy);
            if (m.Success && m.Groups.Count == 4)
            {
                tag = Int32.Parse(m.Groups[1].Value);
                monat = Int32.Parse(m.Groups[2].Value);
                jahr = Int32.Parse(m.Groups[3].Value);
                if (jahr < 100)
                {
                    if (jahr <= (jahrnow % 100))
                    {
                        jahr = jahr + (jahrnow - (jahrnow % 100));
                    }
                    else
                    {
                        jahr = jahr + (jahrnow - (jahrnow % 100) - 100);
                    }
                }
                if (tag >= 1 && tag <= 31 && monat >= 1 && monat <= 12 && jahr >= 1900 && jahr <= DateTime.Now.Date.Year)
                {
                    try
                    {
                        DateTime x = new DateTime(jahr, monat, tag);
                        val = x;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MvcApplication.logMsg("Exception " + ex.Message + " bei parse " + copy);
                        MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                        return false;
                    }
                }
            }

            r = new Regex(@"^(\d+)-(\d+)-(\d+)$");
            m = r.Match(copy);
            if (m.Success && m.Groups.Count == 4)
            {
                tag = Int32.Parse(m.Groups[3].Value);
                monat = Int32.Parse(m.Groups[2].Value);
                jahr = Int32.Parse(m.Groups[1].Value);
                if (jahr < 100)
                {
                    if (jahr <= (jahrnow % 100))
                    {
                        jahr = jahr + (jahrnow - (jahrnow % 100));
                    }
                    else
                    {
                        jahr = jahr + (jahrnow - (jahrnow % 100) - 100);
                    }
                }
                if (tag >= 1 && tag <= 31 && monat >= 1 && monat <= 12 && jahr >= 1900 && jahr <= DateTime.Now.Date.Year)
                {
                    try
                    {
                        DateTime x = new DateTime(jahr, monat, tag);
                        val = x;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MvcApplication.logMsg("Exception " + ex.Message + " bei parse " + copy);
                        MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                        return false;
                    }
                }
            }

            r = new Regex(@"^(\d+)/(\d+)/(\d+)$");
            m = r.Match(copy);
            if (m.Success && m.Groups.Count == 4)
            {
                tag = Int32.Parse(m.Groups[3].Value);
                monat = Int32.Parse(m.Groups[2].Value);
                jahr = Int32.Parse(m.Groups[1].Value);
                if (jahr < 100)
                {
                    if (jahr <= (jahrnow % 100))
                    {
                        jahr = jahr + (jahrnow - (jahrnow % 100));
                    }
                    else
                    {
                        jahr = jahr + (jahrnow - (jahrnow % 100) - 100);
                    }
                }
                if (tag >= 1 && tag <= 31 && monat >= 1 && monat <= 12 && jahr >= 1900 && jahr <= DateTime.Now.Date.Year)
                {
                    try
                    {
                        DateTime x = new DateTime(jahr, monat, tag);
                        val = x;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MvcApplication.logMsg("Exception " + ex.Message + " bei parse " + copy);
                        MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                        return false;
                    }
                }
            }

            r = null;
            m = null;

            return result;
        }


        public static string AnsiString(string str)
        {
            string buffer = (str == null) ? "" : str.Trim();
            /*            for (int i = 0; i < buffer.Length; i+= 1)
                        {
                            char c = buffer[i];
                            if (c == ' ' || 
                                c == '.' || 
                                (c >= 'a' && c <= 'z') ||
                                (c >= 'A' && c <= 'Z') ||
                                (c >= '0' && c <= '9')
                                )
                            {
                                result += c;
                            }
                        }
                        return result;
             */
            return buffer;
        }


        public static string DumpToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj).ToString();
        }


        // Pro Tag eine neue Fallnummer anlegen
        //
        public static int NewDailyNumber(ApplicationDbContext db, DateTime tag, System.Data.Entity.DbContextTransaction tr)
        {
            int lfnr = 1;
            bool finish = false;

            if (tr != null)
            {
                for (finish = false; !finish; )
                {
                    try
                    {
                        IQueryable<DaySerial> e = db.DaySerial.Where(x => x.Day == tag);
                        DaySerial v = new DaySerial()
                        {
                            Day = tag,
                            Number = lfnr
                        };
                        if (e != null && e.Count() == 1)
                        {
                            v = e.First<DaySerial>();
                            lfnr = v.Number + 1;
                            v.Number = lfnr;
                        }
                        else
                        {
                            db.DaySerial.Add(v);
                        }
                        var result = db.SaveChanges();
                        finish = true;
                    }
                    catch (Exception ex)
                    {
                        MvcApplication.logMsg("Exception Dayserial+1 " + ex.Message);
                        MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                    }
                }
            }
            else
            {
                for (finish = false; !finish; )
                {
                    using (tr = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                    {
                        try
                        {
                            IQueryable<DaySerial> e = db.DaySerial.Where(x => x.Day == tag);
                            DaySerial v = new DaySerial()
                            {
                                Day = tag,
                                Number = lfnr
                            };
                            if (e != null && e.Count() == 1)
                            {
                                v = e.First<DaySerial>();
                                lfnr = v.Number + 1;
                                v.Number = lfnr;
                            }
                            else
                            {
                                db.DaySerial.Add(v);
                            }
                            var result = db.SaveChanges();
                            tr.Commit();
                            finish = true;
                        }
                        catch (Exception ex)
                        {
                            tr.Rollback();
                            MvcApplication.logMsg("Exception Dayserial+1 " + ex.Message);
                            MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                        }
                    }
                    tr = null;
                }
            }

            lfnr = lfnr + tag.Year * 1000000 + tag.Month * 10000 + tag.Day * 100;
            return lfnr;
        }

        
        // Neue Laufnummer erzeugen in Settings
        //
        public static long NewNumber(ApplicationDbContext db, string wert, string defwert, System.Data.Entity.DbContextTransaction tr)
        {
            long counter = Int64.Parse(defwert ?? "0");
            bool finish = false;

            if (tr != null)
            {
                for (finish = false; !finish; )
                {
                    try
                    {
                        var e = db.Settings.Where(x => x.Name == wert);
                        Settings s = new Settings { Name = wert, Value = defwert };
                        if (e != null && e.Count() == 1)
                        {
                            Settings record = e.First<Settings>();
                            counter = 1 + Int32.Parse(record.Value);
                            record.Value = counter.ToString("#0");
                        }
                        else
                        {
                            db.Settings.Add(s);
                        }
                        var result = db.SaveChanges();
                        finish = true;
                    }
                    catch (Exception ex)
                    {
                        MvcApplication.logMsg("Exception " + ex.Message);
                        MvcApplication.logMsg("Exception " + ex.HResult.ToString("X") + " " + ex.Message + " " + ObjectDumper.Dump(ex));
                    }
                }
            }
            else
            {
                for (finish = false; !finish; )
                {
                    using (tr = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                    {
                        try
                        {
                            var e = db.Settings.Where(x => x.Name == wert);
                            Settings s = new Settings { Name = wert, Value = defwert };
                            if (e != null && e.Count() == 1)
                            {
                                Settings record = e.First<Settings>();
                                counter = 1 + Int32.Parse(record.Value);
                                record.Value = counter.ToString("#0");
                            }
                            else
                            {
                                db.Settings.Add(s);
                            }
                            var result = db.SaveChanges();
                            tr.Commit();
                            finish = true;
                        }
                        catch (Exception)
                        {
                            tr.Rollback();
                        }
                    }
                    tr = null;
                }
            }
            return counter;
        }

    }
}