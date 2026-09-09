using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using iSmileAlignerTS.Models;
using ISmileAlignerTS.Controllers;

namespace iSmileAlignerTS.Controllers
{
    [Authorize]
    public class AccountController : ISmileController
    {
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            LoginViewModel model = new LoginViewModel();
            shopKind = WhichShop(Request);
            if (shopKind == SHOPART.OldShop) return View("LoginDisabled1", model);
            // if (shopKind == SHOPART.DoctorShop) return View("LoginDisabled2", model);
            // if (shopKind == SHOPART.PatientShop) return View("LoginDisabled2", model);
            return View(model);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            shopKind = WhichShop(Request);

            // Anmeldefehler werden bezüglich einer Kontosperre nicht gezählt.
            // Wenn Sie aktivieren möchten, dass Kennwortfehler eine Sperre auslösen, ändern Sie in "shouldLockout: true".
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    if (returnUrl == null || returnUrl == "")
                    {
                        returnUrl = @"/Manage";
                    }

                    var user = await UserManager.FindByNameAsync(model.Email);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "Ungültiger Anmeldeversuch.");
                        return View(model);
                    }
                    if (!user.isAdmin)
                    {
                        MvcApplication.logMsg("login " + user.Email + " " + user.isDoctor.ToString() + " bei " + shopKind.ToString());

                        if (user.isDoctor && shopKind != SHOPART.DoctorShop && shopKind != SHOPART.DevelShop)
                        {
                            AuthenticationManager.SignOut();

                            ModelState.AddModelError("", "Für den Arzt-Bereich verwenden Sie bitte http://doctors.thinortho.com!");
                            return View(model);
                        }
                        if (user.isCustomer && shopKind != SHOPART.PatientShop && shopKind != SHOPART.DevelShop)
                        {
                            AuthenticationManager.SignOut();

                            ModelState.AddModelError("", "Für den Kunden-Bereich verwenden Sie bitte http://patients.thinortho.com!");
                            return View(model);
                        }
                    }
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Ungültiger Anmeldeversuch.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult LoginToManage()
        {
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Verlangen, dass sich der Benutzer bereits mit seinem Benutzernamen/Kennwort oder einer externen Anmeldung angemeldet hat.
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Der folgende Code schützt vor Brute-Force-Angriffen der zweistufigen Codes. 
            // Wenn ein Benutzer in einem angegebenen Zeitraum falsche Codes eingibt, wird das Benutzerkonto 
            // für einen bestimmten Zeitraum gesperrt. 
            // Sie können die Einstellungen für Kontosperren in "IdentityConfig" konfigurieren.
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Ungültiger Code.");
                    return View(model);
            }
        }


        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            RegisterViewModel model = new RegisterViewModel();
            shopKind = WhichShop(Request);
            if (shopKind == SHOPART.OldShop || shopKind == SHOPART.DevelShop) return View("RegisterDisabled", model);
            model.isPatient = (shopKind == SHOPART.PatientShop) ? true : false;
            PrepareViewBagLandISOList(null);
            Session["RegisterViewModel"] = model;
            if (shopKind == SHOPART.PatientShop) return View("RegisterCustomer", model);
            return View("Register", model);
        }


        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            DateTime aDate = DateTime.Now.Date;
            bool reView = false;

            RegisterViewModel altModel = (RegisterViewModel)Session["RegisterViewModel"];
            if (altModel == null) return RedirectToAction("Register");
            PrepareViewBagLandISOList(null);
            model.isPatient = altModel.isPatient;

            model.Address = s(model.Address);
            model.BirthDate = s(model.BirthDate);
            model.City = s(model.City);
            model.ConfirmPassword = s(model.ConfirmPassword);
            model.CountryCode = s(model.CountryCode);
            model.Email = s(model.Email);
            model.Fax = s(model.Fax);
            model.Firstname = s(model.Firstname);
            model.Lastname = s(model.Lastname);
            model.MedicalField = s(model.MedicalField);
            model.MobilPhone = s(model.MobilPhone);
            model.Password = s(model.Password);
            model.Phone = s(model.Phone);
            model.Praxis = s(model.Praxis);
            model.Salutation = s(model.Salutation);
            model.Titel = s(model.Titel);
            model.WebURL = s(model.WebURL);
            model.ZIP = s(model.ZIP);

            if (GuessDateFromString(model.BirthDate, ref aDate) == false)
            {
                ModelState.AddModelError("", "Das Geburtsdatum muß angegeben werden.");

                ViewBag.ValidationErrors = string.Join(",", ModelState.Values.Where(e => e.Errors.Count > 0).SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToArray());

                PrepareViewBagLandISOList(null);

                reView = true;
                return View(model);
            }


            if (!ModelState.IsValid || reView)
            {
                ViewBag.ValidationErrors = string.Join(",", ModelState.Values.Where(e => e.Errors.Count > 0).SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToArray());

                PrepareViewBagLandISOList(null);

                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Salutation = model.Salutation,
                Titel = model.Titel,
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                BirthDate = aDate,
                Address = model.Address,
                ZIP = model.ZIP,
                City = model.City,
                CountryCode = model.CountryCode,
                Phone = model.Phone,
                MobilPhone = model.MobilPhone,
                Fax = model.Fax,
                WebURL = model.WebURL,
                Praxis = model.Praxis,
                MedicalField = model.MedicalField,
                isDoctor = true
            };
            user.isAdmin = false;
            user.isDeactivated = false;
            user.isVerified = false;
            user.isVerfiedDate = null;
            user.isCustomer = false;
            user.needVerify = true;
            user.needVerifyDate = DateTime.Now;
            string shortName = model.Lastname.Trim().ToUpper();
            if (shortName.Length > 6)
            {
                shortName = shortName.Substring(0, 6);
            }
            user.DocShortName = shortName;
            IQueryable<Laender>land = db.Laender.Where(x => x.ISOCode == model.CountryCode);
            if (land != null && land.Count() == 1) user.Country = land.First().Land;
            land = null;
            user.paysInvoices = PAYRELSKIND.Invoice;
            
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                // Weitere Informationen zum Aktivieren der Kontobestätigung und Kennwortzurücksetzung finden Sie unter "http://go.microsoft.com/fwlink/?LinkID=320771".
                // E-Mail-Nachricht mit diesem Link senden
                // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                // await UserManager.SendEmailAsync(user.Id, "Konto bestätigen", "Bitte bestätigen Sie Ihr Konto. Klicken Sie dazu <a href=\"" + callbackUrl + "\">hier</a>");

                if (user.isDoctor)
                {
                    string message = "Ein neuer Arzt hat sich gemeldet." + Environment.NewLine +
                        "Name: " + model.Salutation + " " + model.Titel + " " + model.Firstname + " " + model.Lastname + Environment.NewLine +
                        "Adresse: " + model.Address + ", " + model.ZIP + " " + model.City + ", " + model.CountryCode + Environment.NewLine +
                        (model.isDoctor ? "Ordination: " + model.Praxis + ", " + model.MedicalField : "") + Environment.NewLine +
                        "Kontakt: Tel:" + model.Phone + " Fax:" + model.Fax + " Mobil:" + model.MobilPhone + Environment.NewLine;

                    /*
                    System.Linq.IQueryable<ApplicationUser> adminusers = db.Users.Where(x => x.isAdmin == true);
                    if (adminusers != null)
                    {
                        foreach (ApplicationUser x in adminusers)
                        {
                            await UserManager.SendEmailAsync(x.Id, "Neuer Arzt " + user.Firstname + " " + user.Lastname + " " + user.Email, message);
                        }
                    }
                    adminusers = null;
                    */
                    SendeEmail("info@thinortho.com", "Neuer Arzt " + user.Firstname + " " + user.Lastname + " " + user.Email, message);
                    SendeEmail("info@philippott.eu", "Neuer Arzt " + user.Firstname + " " + user.Lastname + " " + user.Email, message);
                }

                return RedirectToAction("Index", "Home");
            }
            
            AddErrors(result);

            ViewBag.ValidationErrors = string.Join(",", ModelState.Values.Where(e => e.Errors.Count > 0).SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToArray());

            PrepareViewBagLandISOList(null);

            return View(model);
        }


        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult RegisterCustomer()
        {
            RegisterViewModel model = new RegisterViewModel();
            shopKind = WhichShop(Request);
            if (shopKind == SHOPART.OldShop || shopKind == SHOPART.DevelShop) return View("RegisterDisabled", model);
            model.isPatient = (shopKind == SHOPART.PatientShop) ? true : false;
            PrepareViewBagLandISOList(null);
            Session["RegisterViewModel"] = model;
            return View(model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterCustomer(RegisterViewModel model)
        {
            DateTime aDate = DateTime.Now.Date;
            bool reView = false;

            RegisterViewModel altModel = (RegisterViewModel)Session["RegisterViewModel"];
            if (altModel == null) return RedirectToAction("RegisterCustomer");
            PrepareViewBagLandISOList(null);
            model.isPatient = altModel.isPatient;
            
            model.Address = s(model.Address);
            model.BirthDate = s(model.BirthDate);
            model.City = s(model.City);
            model.ConfirmPassword = s(model.ConfirmPassword);
            model.CountryCode = s(model.CountryCode);
            model.Email = s(model.Email);
            model.Fax = s(model.Fax);
            model.Firstname = s(model.Firstname);
            model.Lastname = s(model.Lastname);
            model.MedicalField = s(model.MedicalField);
            model.MobilPhone = s(model.MobilPhone);
            model.Password = s(model.Password);
            model.Phone = s(model.Phone);
            model.Praxis = s(model.Praxis);
            model.Salutation = s(model.Salutation);
            model.Titel = s(model.Titel);
            model.WebURL = s(model.WebURL);
            model.ZIP = s(model.ZIP);

            if (GuessDateFromString(model.BirthDate, ref aDate) == false)
            {
                ModelState.AddModelError("", "Das Geburtsdatum muß angegeben werden.");

                ViewBag.ValidationErrors = string.Join(",", ModelState.Values.Where(e => e.Errors.Count > 0).SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToArray());

                reView = true;
                return View(model);
            }

            if (!ModelState.IsValid || reView)
            {
                ViewBag.ValidationErrors = string.Join(",", ModelState.Values.Where(e => e.Errors.Count > 0).SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToArray());

                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.Firstname) && string.IsNullOrWhiteSpace(model.Lastname))
            {
                ModelState.AddModelError("", "Ein Name muß angegeben werden.");

                reView = true;
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Salutation = model.Salutation,
                Titel = model.Titel,
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                BirthDate = aDate,
                Address = model.Address,
                ZIP = model.ZIP,
                City = model.City,
                CountryCode = model.CountryCode,
                Phone = model.Phone,
                MobilPhone = model.MobilPhone,
                isDoctor = false,
                isCustomer= true
            };
            user.isAdmin = false;
            user.isDeactivated = false;
            user.isVerified = false;
            user.isVerfiedDate = null;
            user.isCustomer = true;
            user.needVerify = false;
            user.isVerified = true;
            user.needVerifyDate = DateTime.Now;
            user.DocShortName = "";
            user.paysInvoices = PAYRELSKIND.Invoice;

            IQueryable<Laender> land = db.Laender.Where(x => x.ISOCode == model.CountryCode);
            if (land != null && land.Count() == 1) user.Country = land.First().Land;
            land = null;

            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                // Weitere Informationen zum Aktivieren der Kontobestätigung und Kennwortzurücksetzung finden Sie unter "http://go.microsoft.com/fwlink/?LinkID=320771".
                // E-Mail-Nachricht mit diesem Link senden
                // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                // await UserManager.SendEmailAsync(user.Id, "Konto bestätigen", "Bitte bestätigen Sie Ihr Konto. Klicken Sie dazu <a href=\"" + callbackUrl + "\">hier</a>");

                if (user.isCustomer)
                {
                    string message = "Ein neuer Kunde hat sich gemeldet." + Environment.NewLine +
                        "Name: " + model.Salutation + " " + model.Titel + " " + model.Firstname + " " + model.Lastname + Environment.NewLine +
                        "Adresse: " + model.Address + ", " + model.ZIP + " " + model.City + ", " + model.CountryCode + Environment.NewLine +
                        "Kontakt: Tel:" + model.Phone + " Fax:" + model.Fax + " Mobil:" + model.MobilPhone + Environment.NewLine;

                    /*
                    System.Linq.IQueryable<ApplicationUser> adminusers = db.Users.Where(x => x.isAdmin == true);
                    if (adminusers != null)
                    {
                        foreach (ApplicationUser x in adminusers)
                        {
                            await UserManager.SendEmailAsync(x.Id, "Neuer Kunde " + user.Firstname + " " + user.Lastname + " " + user.Email, message);
                        }
                    }
                    adminusers = null;
                    */
                    SendeEmail("info@thinortho.com", "Neuer Kunde " + user.Firstname + " " + user.Lastname + " " + user.Email, message);
                    SendeEmail("info@philippott.eu", "Neuer Kunde " + user.Firstname + " " + user.Lastname + " " + user.Email, message);
                }

                return RedirectToAction("Index", "Home");
            }

            AddErrors(result);

            ViewBag.ValidationErrors = string.Join(",", ModelState.Values.Where(e => e.Errors.Count > 0).SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToArray());

            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Nicht anzeigen, dass der Benutzer nicht vorhanden ist oder nicht bestätigt wurde.
                    return View("ForgotPasswordConfirmation");
                }

                // Weitere Informationen zum Aktivieren der Kontobestätigung und Kennwortzurücksetzung finden Sie unter "http://go.microsoft.com/fwlink/?LinkID=320771".
                // E-Mail-Nachricht mit diesem Link senden
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Kennwort zurücksetzen", "Bitte setzen Sie Ihr Kennwort zurück. Klicken Sie dazu <a href=\"" + callbackUrl + "\">hier</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // Wurde dieser Punkt erreicht, ist ein Fehler aufgetreten; Formular erneut anzeigen.
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Nicht anzeigen, dass der Benutzer nicht vorhanden ist.
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Umleitung an den externen Anmeldeanbieter anfordern
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Token generieren und senden
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Benutzer mit diesem externen Anmeldeanbieter anmelden, wenn der Benutzer bereits eine Anmeldung besitzt
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // Benutzer auffordern, ein Konto zu erstellen, wenn er kein Konto besitzt
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Informationen zum Benutzer aus dem externen Anmeldeanbieter abrufen
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        #region Hilfsprogramme
        // Wird für XSRF-Schutz beim Hinzufügen externer Anmeldungen verwendet
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}