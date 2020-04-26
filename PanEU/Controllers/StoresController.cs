using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PanEU.Models.EntityFramework;

namespace PanEU.Controllers
{
    public class StoresController : Controller
    {
        public string[] cities = new string[]
        { "Adana", "Adıyaman", "Afyon", "Ağrı", "Amasya", "Ankara", "Antalya", "Artvin", "Aydın", "Balıkesir", "Bilecik",
            "Bingöl", "Bitlis", "Bolu", "Burdur", "Bursa", "Çanakkale", "Çankırı", "Çorum", "Denizli", "Diyarbakır", "Edirne",
            "Elazığ", "Erzincan", "Erzurum", "Eskişehir", "Gaziantep", "Giresun", "Gümüşhane", "Hakkari", "Hatay", "Isparta",
            "İçel", "İstanbul", "İzmir", "Kars", "Kastamonu", "Kayseri", "Kırklareli", "Kırşehir", "Kocaeli", "Konya", "Kütahya",
            "Malatya", "Manisa", "K.maraş", "Mardin", "Muğla", "Muş", "Nevşehir", "Niğde", "Ordu", "Rize", "Sakarya", "Samsun",
            "Siirt", "Sinop", "Sivas", "Tekirdağ", "Tokat", "Trabzon", "Tunceli", "Şanlıurfa", "Uşak", "Van", "Yozgat", "Zonguldak",
            "Aksaray", "Bayburt", "Karaman", "Kırıkkale", " Batman", "Şırnak", "Bartın", "Ardahan", "Iğdır", "Yalova", "Karabük", "Kilis", "Osmaniye", "Düzce" };
        private PaneuDBEntities db = new PaneuDBEntities();

        // GET: Stores
        [Authorize(Roles ="ADMIN,USER")]
        public ActionResult Index()
        {
            var store = db.Store.Where(s=>s.IsComfirmed==true);
            return View(store.ToList());
        }

        // GET: Stores/Details/5   
        [Authorize(Roles ="USER")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = db.Store.Include(a=>a.User).FirstOrDefault(a=>a.Id==id);

            if (store.User.Name != HttpContext.User.Identity.Name)
                return HttpNotFound();
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }

        // GET: Stores/Create
        [Authorize(Roles = "ADMIN,USER")]
        public ActionResult Create()
        {
            ViewBag.OwnerUserId = new SelectList(db.User, "Id", "Name");
            return View();
        }
        // POST: Stores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN,USER")]
        public ActionResult Create([Bind(Include = "Id,OwnerUserId,Country,City,Town,Name,Category,IsComfirmed,Monday,Tuesday," +
            "Wednesday,Thursday,Friday,Saturday,Sunday,StartTime,EndTime,MinuteIncrease,NumberOfPeople")] Store store)
        {
            var user = db.User.Where(u => u.UserName == HttpContext.User.Identity.Name).FirstOrDefault();
            store.OwnerUserId = user.Id;
            store.User = user;
            store.StartTime = TimeSpan.FromHours(9);
            store.EndTime = TimeSpan.FromHours(18);
            int plaka = Convert.ToInt16(store.City);
            if (plaka > 0)
                store.City = cities[plaka - 1];
            else
                store.City = null;
            if (ModelState.IsValid)
            {
                db.Store.Add(store);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OwnerUserId = new SelectList(db.User, "Id", "Name", store.OwnerUserId);
            return View(store);
        }

        // GET: Stores/Edit/5
        [Authorize(Roles = "USER")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = db.Store.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            ViewBag.OwnerUserId = new SelectList(db.User, "Id", "Name", store.OwnerUserId);
            return View(store);
        }

        // POST: Stores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "USER")]
        public ActionResult Edit([Bind(Include = "Id,OwnerUserId,Country,City,Town,Name,Category,IsComfirmed,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,StartTime,EndTime,MinuteIncrease,NumberOfPeople")] Store store)
        {
            Store _store = db.Store.Find(store.Id);
            _store.StartTime = store.StartTime;
            _store.EndTime = store.EndTime;
            _store.MinuteIncrease = store.MinuteIncrease;
            _store.NumberOfPeople = store.NumberOfPeople;
            _store.Monday = store.Monday;
            _store.Tuesday = store.Tuesday;
            _store.Wednesday = store.Wednesday;
            _store.Thursday = store.Thursday;
            _store.Friday = store.Friday;
            _store.Saturday = store.Saturday;
            _store.Sunday = store.Sunday;
            if (ModelState.IsValid)
            {
                
                db.Entry(_store).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MyStores");
            }
            ViewBag.OwnerUserId = new SelectList(db.User, "Id", "Name", store.OwnerUserId);
            return View(store);
        }

        // GET: Stores/Delete/5
        [Authorize(Roles = "USER")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = db.Store.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }

        // POST: Stores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "USER")]
        public ActionResult DeleteConfirmed(int id)
        {
            Store store = db.Store.Find(id);
            db.Store.Remove(store);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [Authorize(Roles = "ADMIN")]
        public ActionResult UnConfirmedStores()
        {
            var store = db.Store.Include(s => s.User).Where(s => s.IsComfirmed==false);
            return View(store.ToList());
        }

        [Authorize(Roles ="ADMIN,USER")]
        public ActionResult MyStores()
        {
            var user = db.User.Where(u => u.UserName == HttpContext.User.Identity.Name).FirstOrDefault();

            var store = db.Store.Where(s => s.OwnerUserId == user.Id);
            return View(store.ToList());
        }

        public bool ConfirmStore(int id)
        {
            Store store = db.Store.Find(id);
            store.IsComfirmed = !store.IsComfirmed;
            db.Entry(store).State = EntityState.Modified;
            db.SaveChanges();
            Store newstore = db.Store.Find(id);
            return newstore.IsComfirmed;
        }
    }
}
