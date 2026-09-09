using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using iSmileAlignerTS.Models;
using System.Collections.Generic;
using ISmileAlignerTS.Controllers;

namespace iSmileAlignerTS.Controllers
{
    [Authorize]
    public class ManageController : ISmileController
    {
        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Ihr Kennwort wurde geändert."
                : message == ManageMessageId.SetPasswordSuccess ? "Ihr Kennwort wurde festgelegt."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Ihr Anbieter für zweistufige Authentifizierung wurde festgelegt."
                : message == ManageMessageId.Error ? "Fehler"
                : message == ManageMessageId.AddPhoneSuccess ? "Ihre Telefonnummer wurde hinzugefügt."
                : message == ManageMessageId.RemovePhoneSuccess ? "Ihre Telefonnummer wurde entfernt."
                : "";

            var userId = User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(userId);
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };
            model.NeedVerify = user.needVerify;
            // model.CanCreateCase = user.isDoctor && user.isVerified;
            model.CanCreateCase = user.isVerified;
            model.CanVerifyUser = user.isAdmin;
            model.CanPurchase = user.isVerified;
            model.CanViewCases = user.isCustomer;
            user = null;

            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Token generieren und senden
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Ihr Sicherheitscode lautet " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Eine SMS über den SMS-Anbieter senden, um die Telefonnummer zu überprüfen.
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // Wurde dieser Punkt erreicht, ist ein Fehler aufgetreten. Formular erneut anzeigen.
            ModelState.AddModelError("", "Fehler beim Überprüfen des Telefons.");
            return View(model);
        }

        //
        // GET: /Manage/RemovePhoneNumber
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }


        private static int CompareUserPasswordViews(SetUserPasswordViewModel a, SetUserPasswordViewModel b)
        {
            string x = s(a.Firstname) + "." + s(a.Lastname) + "." + a.Email;
            string y = s(b.Firstname) + "." + s(b.Lastname) + "." + b.Email;

            return x.CompareTo(y);
        }


        // GET: /Manage/SetUserPassword
        //
        [Authorize]
        public ActionResult SetUserPassword()
        {
            var meUser = UserManager.FindById(User.Identity.GetUserId());
            if (!meUser.isAdmin) return RedirectToAction("KeinZugriff", "Home");

            IQueryable<ApplicationUser> users = UserManager.Users.OrderBy(x => x.Email);
            List<SetUserPasswordViewModel> model = new List<SetUserPasswordViewModel>();
            foreach (ApplicationUser x in users)
            {
                SetUserPasswordViewModel y = new SetUserPasswordViewModel();

                y.UserID = x.Id;
                y.Firstname = x.Firstname;
                y.Lastname = x.Lastname;
                if (x.isAdmin)
                {
                    y.Email = x.Email + " (Admin)";
                }
                else if (x.isDoctor)
                {
                    y.Email = x.Email + " (Dr.)";
                }
                else if (x.isCustomer)
                {
                    y.Email = x.Email + " (Kunde)";
                }

                model.Add(y);
            }

            model.Sort(CompareUserPasswordViews);

            return View(model);
        }


        // GET: /Manage/SetUserPassword
        //
        [Authorize]
        public ActionResult DeleteUser()
        {
            var meUser = UserManager.FindById(User.Identity.GetUserId());
            if (!meUser.isAdmin) return RedirectToAction("KeinZugriff", "Home");

            IQueryable<ApplicationUser> users = UserManager.Users.OrderBy(x => x.Email);
            List<SetUserPasswordViewModel> model = new List<SetUserPasswordViewModel>();
            foreach (ApplicationUser x in users)
            {
                SetUserPasswordViewModel y = new SetUserPasswordViewModel();

                y.UserID = x.Id;
                y.Firstname = x.Firstname;
                y.Lastname = x.Lastname;
                if (x.isAdmin)
                {
                    y.Email = x.Email + " (Admin)";
                }
                else if (x.isDoctor)
                {
                    y.Email = x.Email + " (Dr.)";
                }
                else if (x.isCustomer)
                {
                    y.Email = x.Email + " (Kunde)";
                }

                model.Add(y);
            }

            model.Sort(CompareUserPasswordViews);

            return View(model);
        }

        //
        // POST: /Manage/VerifyUserData
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteUser(string delUserButton)
        {
            string newPasswordString = "";

            ApplicationUser meUser = UserManager.FindById(User.Identity.GetUserId());
            if (!meUser.isAdmin) return RedirectToAction("KeinZugriff", "Home");

            string subject = null;
            string message = null;

            if (delUserButton != null && delUserButton.Length > 0)
            {
                ApplicationUser otherUser = UserManager.FindById(delUserButton);
                if (otherUser != null)
                {
                    IdentityResult r = null;


                
                }
            }

            return RedirectToAction("DeleteUser", "Manage");
        }

        //
        // POST: /Manage/VerifyUserData
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult SetUserPassword(string setPasswordButton)
        {
            string newPasswordString = "";

            ApplicationUser meUser = UserManager.FindById(User.Identity.GetUserId());
            if (!meUser.isAdmin) return RedirectToAction("KeinZugriff", "Home");

            string subject = null;
            string message = null;

            if (setPasswordButton != null && setPasswordButton.Length > 0)
            {
                string pwkey= "pw" + setPasswordButton;

                newPasswordString = Request.Form[pwkey];
                if (newPasswordString != null) newPasswordString = newPasswordString.Trim();

                ApplicationUser otherUser = UserManager.FindById(setPasswordButton);
                if (otherUser != null)
                {
                    IdentityResult r = null;

                    subject = "Kennwort ändern fehlgeschlagen";
                    message = "Konto:" + otherUser.Email + Environment.NewLine;

                    r = UserManager.RemovePassword(otherUser.Id);
                    if (r.Succeeded)
                    {
                        r = UserManager.AddPassword(otherUser.Id, newPasswordString);
                        if (r.Succeeded)
                        {
                            subject = "Kennwort von " + otherUser.Email + " erfolgreich geändert";
                            message = "Konto:" + otherUser.Email + " " + newPasswordString + Environment.NewLine;
                        }
                        else
                        {
                            subject = "Kennwort von " + otherUser.Email + " konnte nicht neu erstellt werden";
                            message = "Konto:" + otherUser.Email + " " + newPasswordString + Environment.NewLine;
                        }
                    }
                    else
                    {
                        subject = "Kennwort von " + otherUser.Email + " konnte nicht gelöscht werden";
                        message = "Konto:" + otherUser.Email + " " + newPasswordString + Environment.NewLine;
                    }
                    UserManager.SendEmail(meUser.Id, subject, message);
                    UserManager.SendEmail(otherUser.Id, subject, message);
                }
            }

            return RedirectToAction("SetUserPassword", "Manage");
        }


        private void LoadUsers(CustomerCasesView model)
        {
            model.Users.Clear();

            var users = UserManager.Users.Where(x => x.isCustomer == true).OrderBy(x => x.Firstname).ThenBy(x => x.Lastname).ToList();
            foreach(var x in users)
            {
                SelectListItem z = new SelectListItem();
                z.Selected = x.Id == model.UserId;
                z.Text = s(x.Titel) + " " + s(x.Firstname) + " " + s(x.Lastname) + ", " + x.Email;
                z.Value = x.Id;
                if (String.IsNullOrWhiteSpace(model.UserSearch) || z.Text.Contains(model.UserSearch)) model.Users.Add(z);
            }
        }

        private void LoadCases(CustomerCasesView model)
        {
            model.Cases.Clear();

            var cases = db.Cases.OrderBy(x => x.CaseNumberStr).ToList();
            foreach (var x in cases)
            {
                SelectListItem z = new SelectListItem();
                z.Selected= x.Id == model.CasesId;
                z.Text = x.CaseNumberStr + " (" + s(x.PatFirstname) + " " + s(x.PatLastName) + ")";
                z.Value = x.Id.ToString("#0");
                if (String.IsNullOrWhiteSpace(model.CasesSearch) || z.Text.Contains(model.CasesSearch)) model.Cases.Add(z);
            }
        }

        private static int CompareSelectListItems(SelectListItem a, SelectListItem b)
        {
            return a.Text.CompareTo(b.Text);
        }

        private void LoadAssigns(CustomerCasesView model)
        {
            model.Assigns.Clear();

            var assigns = db.CustomerCases.OrderBy(x => x.UserId).ToList();
            foreach (var x in assigns)
            {
                Cases c = db.Cases.Find(x.CasesId);
                ApplicationUser u = UserManager.FindById(x.UserId);

                SelectListItem z = new SelectListItem();
                z.Selected = false;
                z.Text = s(u.Firstname) + " " + s(u.Lastname) + " : " + c.CaseNumberStr;
                z.Value = x.Id.ToString("#0");
                model.Assigns.Add(z);
                u = null;
                c = null;
            }

            if (model.Assigns.Count() > 0)
            {
                model.Assigns.Sort(CompareSelectListItems);
            }
        }


        [Authorize]
        public ActionResult SetUserCase()
        {
            var meUser = UserManager.FindById(User.Identity.GetUserId());
            if (!meUser.isAdmin) return RedirectToAction("KeinZugriff", "Home");

            CustomerCasesView model = new CustomerCasesView();

            model.Users = new List<SelectListItem>();
            model.Cases = new List<SelectListItem>();
            model.Assigns = new List<SelectListItem>();

            model.DoAktion = false;
            model.CasesId = 0;
            model.UserId = "";
            model.UserSearch = model.CasesSearch = "";

            LoadUsers(model);
            LoadCases(model);
            LoadAssigns(model);

            Session["setusercaseview"] = model;

            return View(model);
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult SetUserCase(string userSearchButton, string userResetButton, string caseSearchButton, string caseResetButton, string assignCaseButton, string cutCaseButton, CustomerCasesView model)
        {
            var meUser = UserManager.FindById(User.Identity.GetUserId());
            if (!meUser.isAdmin) return RedirectToAction("KeinZugriff", "Home");

            CustomerCasesView smodel = (CustomerCasesView)Session["setusercaseview"];
            if (smodel == null) return RedirectToAction("KeinZugriff", "Home");

            smodel.UserId = model.UserId;
            smodel.UserSearch = model.UserSearch;
            smodel.CasesId = model.CasesId;
            smodel.CasesSearch = model.CasesSearch;
            smodel.AssignId = model.AssignId;

            Session["setusercaseview"] = smodel;

            if (!ModelState.IsValid)
            {
                ModelState.Clear();
                return View(smodel);
            }

            ModelState.Clear();

            if (userResetButton != null || userSearchButton != null)
            {
                if (userResetButton != null)
                {
                    smodel.UserSearch = "";
                    smodel.UserId = "";
                }

                LoadUsers(smodel);

                Session["setusercaseview"] = smodel;

                return View(smodel);
            }

            if (caseResetButton != null || caseSearchButton != null)
            {
                if (caseResetButton != null)
                {
                    smodel.CasesSearch = "";
                    smodel.CasesId = 0;
                }

                LoadCases(smodel);

                Session["setusercaseview"] = smodel;

                return View(smodel);
            }

            if (!model.DoAktion)
            {
                ModelState.AddModelError(String.Empty, "Der Aktionsschalter muß angewählt sein!");
                return View(smodel);
            }

            if (cutCaseButton != null)
            {
                CustomerCases x = db.CustomerCases.Find(smodel.AssignId);
                if (x != null)
                {
                    db.CustomerCases.Remove(x);
                    db.SaveChanges();
                }

                smodel.AssignId = 0;
                LoadAssigns(smodel);

                Session["setusercaseview"] = smodel;

                return View(smodel);
            }

            if (smodel.CasesId != 0 && !String.IsNullOrEmpty(smodel.UserId))
            {
                CustomerCases y = new CustomerCases();
                y.CasesId = smodel.CasesId;
                y.UserId = smodel.UserId;
                IEnumerable<CustomerCases> s = db.CustomerCases.Where(x => x.UserId == y.UserId && x.CasesId == y.CasesId);
                if (s == null | (s != null && s.Count() == 0))
                {
                    db.CustomerCases.Add(y);
                    db.SaveChanges();
                }
                s = null;

                LoadAssigns(smodel);

                Session["setusercaseview"] = smodel;

                return View(smodel);
            }

            return View(smodel);
        }

        
        //
        // GET: /Manage/VerifyUserData
        public ActionResult VerifyUserData()
        {
            var meUser = UserManager.FindById(User.Identity.GetUserId());
            if (!meUser.isAdmin) return RedirectToAction("KeinZugriff", "Home");

            IQueryable<ApplicationUser> users = UserManager.Users.Where(x => x.needVerify == true).OrderBy(x => x.needVerifyDate);
            List<VerifyUserDataViewModel> model = new List<VerifyUserDataViewModel>();
            foreach (ApplicationUser x in users)
            {
                VerifyUserDataViewModel y = new VerifyUserDataViewModel();

                y.UserID = x.Id;
                y.Salutation = x.Salutation;
                y.Titel = x.Titel;
                y.Firstname = x.Firstname;
                y.Lastname = x.Lastname;
                y.BirthDate = x.BirthDate;
                y.Address = x.Address;
                y.ZIP = x.ZIP;
                y.City = x.City;
                y.Country = x.Country;
                y.Phone = x.Phone;
                y.Fax = x.Fax;
                y.MobilPhone = x.MobilPhone;
                y.WebURL = x.WebURL;
                y.NeedVerifyDate = x.needVerifyDate;
                y.Email = x.Email;
                y.DocShortName = x.DocShortName;

                model.Add(y);
            }
            return View(model);
        }

        //
        // POST: /Manage/VerifyUserData
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyUserData(string btnOK, string btnDEL)
        {
            ApplicationUser meUser = UserManager.FindById(User.Identity.GetUserId());
            if (!meUser.isAdmin) return RedirectToAction("KeinZugriff", "Home");

            string subject = null;
            string message = null;

            if (btnOK != null)
            {
                meUser = UserManager.FindById(btnOK);
                if (meUser != null)
                {
                    meUser.isVerified = true;
                    meUser.isVerfiedDate = DateTime.Now;
                    meUser.needVerify = false;
                    meUser.isDoctor = true;
                    var result = UserManager.Update(meUser);
                    if (result.Succeeded)
                    {
                        subject = "" + meUser.Firstname + " " + meUser.Lastname + " wurde freigeschaltet.";
                        message = "Name: " + meUser.Salutation + " " + meUser.Titel + " " + meUser.Firstname + " " + meUser.Lastname + Environment.NewLine +
                            "Adresse: " + meUser.Address + ", " + meUser.ZIP + " " + meUser.City + ", " + meUser.Country + Environment.NewLine +
                            (meUser.isDoctor ? "Ordination: " + meUser.Praxis + ", " + meUser.MedicalField : "") + Environment.NewLine +
                            "Kontakt: Tel:" + meUser.Phone + " Fax:" + meUser.Fax + " Mobil:" + meUser.MobilPhone + Environment.NewLine;
                    }
                    else
                    {
                        subject = "Ein Fehler ist aufgetreten beim Freischalten von " + meUser.Firstname + " " + meUser.Lastname;
                        message = "Name: " + meUser.Salutation + " " + meUser.Titel + " " + meUser.Firstname + " " + meUser.Lastname + Environment.NewLine +
                            "Adresse: " + meUser.Address + ", " + meUser.ZIP + " " + meUser.City + ", " + meUser.Country + Environment.NewLine +
                            (meUser.isDoctor ? "Ordination: " + meUser.Praxis + ", " + meUser.MedicalField : "") + Environment.NewLine +
                            "Kontakt: Tel:" + meUser.Phone + " Fax:" + meUser.Fax + " Mobil:" + meUser.MobilPhone + Environment.NewLine;
                    }
                    UserManager.SendEmail(meUser.Id, subject, message);
                }
            }

            if (btnDEL != null)
            {
                var xUser = UserManager.FindById(btnDEL);
                if (xUser != null && xUser.Id != meUser.Id)
                {
                    var result = UserManager.Delete(xUser);
                    if (result.Succeeded)
                    {
                        subject = "" + meUser.Firstname + " " + meUser.Lastname + " wurde gelöscht!";
                        message = "Name: " + meUser.Salutation + " " + meUser.Titel + " " + meUser.Firstname + " " + meUser.Lastname + Environment.NewLine +
                            "Adresse: " + meUser.Address + ", " + meUser.ZIP + " " + meUser.City + ", " + meUser.Country + Environment.NewLine +
                            (meUser.isDoctor ? "Ordination: " + meUser.Praxis + ", " + meUser.MedicalField : "") + Environment.NewLine +
                            "Kontakt: Tel:" + meUser.Phone + " Fax:" + meUser.Fax + " Mobil:" + meUser.MobilPhone + Environment.NewLine;
                    }
                    else
                    {
                        subject = "Ein Fehler ist aufgetreten beim Löschen von " + meUser.Firstname + " " + meUser.Lastname;
                        message = "Name: " + meUser.Salutation + " " + meUser.Titel + " " + meUser.Firstname + " " + meUser.Lastname + Environment.NewLine +
                            "Adresse: " + meUser.Address + ", " + meUser.ZIP + " " + meUser.City + ", " + meUser.Country + Environment.NewLine +
                            (meUser.isDoctor ? "Ordination: " + meUser.Praxis + ", " + meUser.MedicalField : "") + Environment.NewLine +
                            "Kontakt: Tel:" + meUser.Phone + " Fax:" + meUser.Fax + " Mobil:" + meUser.MobilPhone + Environment.NewLine;
                    }
                }
            }

            if (message != null && subject != null)
            {
                System.Linq.IQueryable<ApplicationUser> adminusers = db.Users.Where(x => x.isAdmin == true);
                if (adminusers != null)
                {
                    foreach (ApplicationUser x in adminusers)
                    {
                        UserManager.SendEmail(x.Id, subject, message);
                    }
                }
                adminusers = null;
            }

            IQueryable<ApplicationUser> users = UserManager.Users.Where(x => x.needVerify == true).OrderBy(x => x.needVerifyDate);
            List<VerifyUserDataViewModel> model = new List<VerifyUserDataViewModel>();
            foreach (ApplicationUser x in users)
            {
                VerifyUserDataViewModel y = new VerifyUserDataViewModel();

                y.UserID = x.Id;
                y.Salutation = x.Salutation;
                y.Titel = x.Titel;
                y.Firstname = x.Firstname;
                y.Lastname = x.Lastname;
                y.BirthDate = x.BirthDate;
                y.Address = x.Address;
                y.ZIP = x.ZIP;
                y.City = x.City;
                y.Country = x.Country;
                y.Phone = x.Phone;
                y.Fax = x.Fax;
                y.MobilPhone = x.MobilPhone;
                y.WebURL = x.WebURL;
                y.NeedVerifyDate = x.needVerifyDate;
                y.Email = x.Email;

                model.Add(y);
            }
            return View(model);
        }

        //
        // GET: /Manage/ChangeUserData
        public async Task<ActionResult> ChangeUserData()
        {
            ChangeUserDataViewModel model = new ChangeUserDataViewModel();
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                model.UserID = user.Id;
                model.Salutation = user.Salutation;
                model.Titel = user.Titel;
                model.Firstname = user.Firstname;
                model.Lastname = user.Lastname;
                model.BirthDate = user.BirthDate.Date.Day.ToString("#0") + "." + user.BirthDate.Date.Month.ToString("#0") + "." + user.BirthDate.Date.Year.ToString("#0");
                model.Address = user.Address;
                model.ZIP = user.ZIP;
                model.City = user.City;
                model.CountryCode = user.CountryCode;
                model.Phone = user.Phone;
                model.Fax = user.Fax;
                model.MobilPhone = user.MobilPhone;
                model.WebURL = user.WebURL;
                model.Praxis = user.Praxis;
                model.MedicalField = user.MedicalField;
                model.isDoctor = user.isDoctor;
            }

            shopKind = WhichShop(Request);

            PrepareViewBagLandISOList(model.CountryCode);

            if (shopKind == SHOPART.PatientShop) return View("ChangeUserDataCustomer", model);

            return View(model);
        }


        //
        // POST: /Manage/ChangeUserData
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeUserData(ChangeUserDataViewModel model)
        {
            bool reView = false;
            DateTime aDate = DateTime.Now.Date;
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (GuessDateFromString(model.BirthDate, ref aDate) == false)
            {
                ModelState.AddModelError("", "Das Geburtsdatum muß angegeben werden.");
                reView = true;
            }
            if (!ModelState.IsValid || reView)
            {
                ViewBag.ValidationErrors = string.Join(",", ModelState.Values.Where(e => e.Errors.Count > 0).SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToArray());
                PrepareViewBagLandISOList(model.CountryCode);
                return View(model);
            }

            if (user != null)
            {
                string message = "Ein Partner hat seine Daten geändert." + Environment.NewLine +
                    "Alte Daten:" + Environment.NewLine +
                    "Name: " + user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname + Environment.NewLine +
                    "Adresse: " + user.Address + ", " + user.ZIP + " " + user.City + ", " + user.Country + Environment.NewLine +
                    (user.isDoctor ? "Ordination: " + user.Praxis + ", " + user.MedicalField : "") + Environment.NewLine +
                    "Kontakt: Tel:" + user.Phone + " Fax:" + user.Fax + " Mobil:" + user.MobilPhone + Environment.NewLine;
                message = message + "Neue Daten:" + Environment.NewLine +
                    "Name: " + model.Salutation + " " + model.Titel + " " + model.Firstname + " " + model.Lastname + Environment.NewLine +
                    "Adresse: " + model.Address + ", " + model.ZIP + " " + model.City + ", " + model.CountryCode + Environment.NewLine +
                    (model.isDoctor ? "Ordination: " + model.Praxis + ", " + model.MedicalField : "") + Environment.NewLine +
                    "Kontakt: Tel:" + model.Phone + " Fax:" + model.Fax + " Mobil:" + model.MobilPhone + Environment.NewLine;

                user.Salutation = model.Salutation;
                user.Titel = model.Titel;
                user.Firstname = model.Firstname;
                user.Lastname = model.Lastname;
                user.BirthDate = aDate;
                user.Address = model.Address;
                user.ZIP = model.ZIP;
                user.City = model.City;
                user.CountryCode = model.CountryCode;
                user.Phone = model.Phone;
                user.MobilPhone = model.MobilPhone;
                user.Fax = model.Fax;
                user.WebURL = model.WebURL;
                user.Praxis = model.Praxis;
                user.MedicalField = model.MedicalField;
                user.isDoctor = true;
                user.isCustomer = false;
                user.needVerify = true;
                user.needVerifyDate = DateTime.Now;
                user.isVerified = false;
                user.isVerfiedDate = null;

                IQueryable<Laender> land = db.Laender.Where(x => x.ISOCode == model.CountryCode);
                if (land != null && land.Count() == 1) user.Country = land.First().Land;
                land = null;

                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }

                    System.Linq.IQueryable<ApplicationUser> adminusers = db.Users.Where(x => x.isAdmin == true);
                    if (adminusers != null)
                    {
                        foreach (ApplicationUser x in adminusers)
                        {
                            await UserManager.SendEmailAsync(x.Id, "Datenänderung bei " + user.Firstname + " " + user.Lastname + " " + user.Email, message);
                        }
                    }
                    adminusers = null;
                }
            }

            return RedirectToAction("Index");
        }


        //
        // POST: /Manage/ChangeUserDataCusomter
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeUserDataCustomer(ChangeUserDataViewModel model)
        {
            bool reView = false;
            DateTime aDate = DateTime.Now.Date;
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (GuessDateFromString(model.BirthDate, ref aDate) == false)
            {
                ModelState.AddModelError("", "Das Geburtsdatum muß angegeben werden.");
                reView = true;
            }
            if (!ModelState.IsValid || reView)
            {
                ViewBag.ValidationErrors = string.Join(",", ModelState.Values.Where(e => e.Errors.Count > 0).SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToArray());
                PrepareViewBagLandISOList(model.CountryCode);
                return View(model);
            }

            if (user != null)
            {
                string message = "Ein Partner hat seine Daten geändert." + Environment.NewLine +
                    "Alte Daten:" + Environment.NewLine +
                    "Name: " + user.Salutation + " " + user.Titel + " " + user.Firstname + " " + user.Lastname + Environment.NewLine +
                    "Adresse: " + user.Address + ", " + user.ZIP + " " + user.City + ", " + user.Country + Environment.NewLine +
                    (user.isDoctor ? "Ordination: " + user.Praxis + ", " + user.MedicalField : "") + Environment.NewLine +
                    "Kontakt: Tel:" + user.Phone + " Fax:" + user.Fax + " Mobil:" + user.MobilPhone + Environment.NewLine;
                message = message + "Neue Daten:" + Environment.NewLine +
                    "Name: " + model.Salutation + " " + model.Titel + " " + model.Firstname + " " + model.Lastname + Environment.NewLine +
                    "Adresse: " + model.Address + ", " + model.ZIP + " " + model.City + ", " + model.CountryCode + Environment.NewLine +
                    (model.isDoctor ? "Ordination: " + model.Praxis + ", " + model.MedicalField : "") + Environment.NewLine +
                    "Kontakt: Tel:" + model.Phone + " Fax:" + model.Fax + " Mobil:" + model.MobilPhone + Environment.NewLine;

                user.Salutation = model.Salutation;
                user.Titel = model.Titel;
                user.Firstname = model.Firstname;
                user.Lastname = model.Lastname;
                user.BirthDate = aDate;
                user.Address = model.Address;
                user.ZIP = model.ZIP;
                user.City = model.City;
                user.CountryCode = model.CountryCode;
                user.Phone = model.Phone;
                user.MobilPhone = model.MobilPhone;
                user.isDoctor = false;
                user.isCustomer = true;
                user.needVerify = false;
                user.needVerifyDate = DateTime.Now;
                user.isVerified = true;
                user.isVerfiedDate = DateTime.Now;

                IQueryable<Laender> land = db.Laender.Where(x => x.ISOCode == model.CountryCode);
                if (land != null && land.Count() == 1) user.Country = land.First().Land;
                land = null;

                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }

                    System.Linq.IQueryable<ApplicationUser> adminusers = db.Users.Where(x => x.isAdmin == true);
                    if (adminusers != null)
                    {
                        foreach (ApplicationUser x in adminusers)
                        {
                            await UserManager.SendEmailAsync(x.Id, "Datenänderung bei " + user.Firstname + " " + user.Lastname + " " + user.Email, message);
                        }
                    }
                    adminusers = null;
                }
            }

            return RedirectToAction("Index");
        }


        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // Wurde dieser Punkt erreicht, ist ein Fehler aufgetreten. Formular erneut anzeigen.
            return View(model);
        }
      
        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "Die externe Anmeldung wurde entfernt."
                : message == ManageMessageId.Error ? "Fehler"
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Umleitung an den externen Anmeldeanbieter anfordern, um eine Anmeldung für den aktuellen Benutzer zu verknüpfen.
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        #region Hilfsprogramme
        // Wird für XSRF-Schutz beim Hinzufügen externer Anmeldungen verwendet.
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

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion
    }
}