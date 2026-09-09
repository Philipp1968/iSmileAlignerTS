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
using ISmileAlignerTS.Controllers;

namespace iSmileAlignerTS.Controllers
{
    public class HomeController : ISmileController
    {
        public HomeController()
        {
        }

        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ActionResult Index()
        {
            string home = Request.Url.Host ?? ("(" + System.Environment.MachineName + ")");

            ViewBag.HostDirName = home;

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Kontakt";

            return View();
        }

        public ActionResult Impressum()
        {
            ViewBag.Message = "Impressum";

            return View();
        }

        public ActionResult AGB()
        {
            ViewBag.Message = "AGB";

            return View();
        }

        public ActionResult Idee()
        {
            ViewBag.Message = "Idee";

            return View();
        }

        public ActionResult Therapie()
        {
            ViewBag.Message = "Idee";

            return View();
        }

        public ActionResult Vorteile()
        {
            ViewBag.Message = "Vorteile";

            return View();
        }

        public ActionResult Fallbeispiele()
        {
            ViewBag.Message = "Fallbeispiele";

            return View();
        }

        public ActionResult Englisch()
        {
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie == null)
            {
                cookie = new HttpCookie("_culture");
            }
            cookie.Value = "en-US";
            cookie.Expires = DateTime.Now.AddYears(1);
            Response.Cookies.Add(cookie);
            return RedirectToAction("Index");
        }

        public ActionResult Deutsch()
        {
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie == null)
            {
                cookie = new HttpCookie("_culture");
            }
            cookie.Value = "de-AT";
            cookie.Expires = DateTime.Now.AddYears(1);
            Response.Cookies.Add(cookie);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> KeinZugriff()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            ViewBag.Message = "Funktionsaufruf nicht gestattet";

            Session["caseView"] = null;

            if (ViewBag.ErrorExceptionString == null || ViewBag.ErrorExceptionString == "")
            {
                ViewBag.ErrorExceptionString = (user != null && user.isAdmin) ? MvcApplication.ErrorString() : "";
            }
            ViewBag.LogString = (user != null && user.isAdmin) ? MvcApplication.LogString() : "";
            MvcApplication.ErrorStringReset();

            return View();
        }

        public async Task<ActionResult> KeinSpeichern()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            ViewBag.Message = "Daten konnten leider nicht gespeichert werden.";

            Session["caseView"] = null;

            if (ViewBag.ErrorExceptionString == null || ViewBag.ErrorExceptionString == "")
            {
                ViewBag.ErrorExceptionString = (user != null && user.isAdmin) ? MvcApplication.ErrorString() : "";
            }
            ViewBag.LogString = (user != null && user.isAdmin) ? MvcApplication.LogString() : "";
            MvcApplication.ErrorStringReset();

            return View();
        }
    }
}
