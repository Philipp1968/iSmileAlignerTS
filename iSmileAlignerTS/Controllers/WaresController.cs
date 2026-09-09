using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using iSmileAlignerTS.Models;
using ISmileAlignerTS.Controllers;
using System.Drawing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Data.Entity.Infrastructure;

namespace iSmileAlignerTS.Controllers
{
    [Authorize]
    public class WaresController : ISmileController
    {
        void CopyWares(Wares d, WaresView s)
        {
            d.Id= s.Id;
            d.WareNumber= s.WareNumber;
            d.Longtext= s.Longtext;
            d.PriceDoc= s.PriceDoc;
            d.PricePat = s.PricePat;
            d.Currency= s.Currency;
            d.Tax = s.Tax;
            d.AddInfoText = s.AddInfoText;
            d.isRequestQuote= s.isRequestQuote;
            d.InShop= s.InShop;
            
            //// Bild Infos überspringen
        }


        void CopyWares(WaresView d, Wares s)
        {
            d.Id = s.Id;
            d.WareNumber = s.WareNumber;
            d.Longtext = s.Longtext;
            d.PriceDoc = s.PriceDoc;
            d.PricePat = s.PricePat;
            d.Currency = s.Currency;
            d.Tax = s.Tax;
            d.AddInfoText = s.AddInfoText;
            d.isRequestQuote = s.isRequestQuote;
            d.InShop = s.InShop;
            
            //// bild nachher setzen
            //
            d.WareFotoBin = null;
            d.WareFotoName = null;
            d.WareFotoId = 0;
            d.WareThumbId = 0;
        }



        // GET: Wares
        public async Task<ActionResult> Index()
        {
            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (!user.isVerified || !user.isAdmin)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            return View(await db.Wares.ToListAsync());
        }

        // GET: Wares/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (!user.isVerified || !user.isAdmin)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wares wares = await db.Wares.FindAsync(id);
            if (wares == null)
            {
                return HttpNotFound();
            }
            return View(wares);
        }

        // GET: Wares/Create
        public ActionResult Create()
        {
            ApplicationUser user = UserManager.FindById(User.Identity.GetUserId());
            if (!user.isVerified || !user.isAdmin)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            return View();
        }

        // POST: Wares/Create
        // Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
        // finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,WareNumber,Longtext,PriceDoc,PricePat,Currency,Tax,AddInfoText,isRequestQuote,InShop")] WaresView wares)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (!user.isVerified || !user.isAdmin)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            if (ModelState.IsValid)
            {
                Wares w = new Wares();
                CopyWares(w, wares);
                db.Wares.Add(w);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(wares);
        }

        // GET: Wares/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (!user.isVerified || !user.isAdmin)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wares wares = await db.Wares.FindAsync(id);
            if (wares == null)
            {
                return HttpNotFound();
            }

            WaresView sessionWares = new WaresView();
            CopyWares(sessionWares, wares);

            //// Bildinfos holen
            //// und in sessionWares eintragen

            Session["Wares"] = wares;
            return View(sessionWares);
        }

        // POST: Wares/Edit/5
        // Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
        // finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(WaresView wares)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (!user.isVerified || !user.isAdmin)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            Wares aWare = (Wares)Session["Wares"];
            if (aWare == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                aWare.WareNumber = wares.WareNumber;
                aWare.Longtext = wares.Longtext;
                aWare.AddInfoText = wares.AddInfoText;
                aWare.PriceDoc = wares.PriceDoc;
                aWare.PricePat = wares.PricePat;
                aWare.Currency = (wares.Currency == null || wares.Currency == "") ? "EUR" : wares.Currency.ToUpper();
                aWare.isRequestQuote = wares.isRequestQuote;
                aWare.Tax = wares.Tax;
                aWare.InShop = wares.InShop;

                db.Entry(aWare).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(wares);
        }

        // GET: Wares/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (!user.isVerified || !user.isAdmin)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wares wares = await db.Wares.FindAsync(id);
            if (wares == null)
            {
                return HttpNotFound();
            }
            return View(wares);
        }

        // POST: Wares/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (!user.isVerified || !user.isAdmin)
            {
                return RedirectToAction("KeinZugriff", "Home");
            }

            Wares wares = await db.Wares.FindAsync(id);
            db.Wares.Remove(wares);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
