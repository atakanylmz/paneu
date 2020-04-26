using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PanEU.Models.EntityFramework;
using PanEU.ViewModels;

namespace PanEU.Controllers
{
    public class AppointmentsController : Controller
    {
        private PaneuDBEntities db = new PaneuDBEntities();

        // GET: Appointments
        public ActionResult Index()
        {
            string userName = HttpContext.User.Identity.Name;
            User user = db.User.Where(u => u.UserName == userName).FirstOrDefault();

            var appointment = db.Appointment.Include(a => a.Store).Include(a => a.User).Where(a=>a.UserId==user.Id);
            return View(appointment.ToList());
        }

        // GET: Appointments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointment.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // GET: Appointments/Create
        public ActionResult Create(int storeId)
        {
            //ViewBag.StoreId = storeId;
            string userName = HttpContext.User.Identity.Name;
            User user = db.User.Where(u => u.UserName == userName).FirstOrDefault();
            Store store = db.Store.Find(storeId);
            //  ViewBag.UserId = new SelectList(db.User, "Id", "Name");

            DateTime dateTime = DateTime.Today;
            int weekday = (int)dateTime.DayOfWeek;
            MakeAnAppointmentModel appointment = new MakeAnAppointmentModel { UserId=user.Id,StoreId=storeId,Store=store,DayInWeek=weekday };
            return View(appointment);
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,StoreId,UserId,AppointmentTime,DayInWeek")] MakeAnAppointmentModel appointmentModel)
        {
            User user = db.User.Find(appointmentModel.UserId);
            Store store = db.Store.Find(appointmentModel.StoreId);

            DateTime dateTime = DateTime.Today;
            int addingDay = 0;
            int weekday = (int)dateTime.DayOfWeek;
            if (weekday > appointmentModel.DayInWeek)
            {
                addingDay =7+ appointmentModel.DayInWeek -weekday;
            }
            else
            {
                addingDay = appointmentModel.DayInWeek  - weekday;
            }
            dateTime = dateTime.AddDays(addingDay);
            dateTime = dateTime.Add(appointmentModel.AppointmentTime);
            Appointment appointment = new Appointment
            {
                StoreId=store.Id,
                Store=store,
                UserId=user.Id,
                User=user,
                AppointmentDate=dateTime
            };
            
            if (ModelState.IsValid)
            {
                db.Appointment.Add(appointment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StoreId = new SelectList(db.Store, "Id", "Country", appointment.StoreId);
            ViewBag.UserId = new SelectList(db.User, "Id", "Name", appointment.UserId);
            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointment.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            ViewBag.StoreId = new SelectList(db.Store, "Id", "Country", appointment.StoreId);
            ViewBag.UserId = new SelectList(db.User, "Id", "Name", appointment.UserId);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StoreId,UserId,AppointmentDate")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appointment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StoreId = new SelectList(db.Store, "Id", "Country", appointment.StoreId);
            ViewBag.UserId = new SelectList(db.User, "Id", "Name", appointment.UserId);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointment.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Appointment appointment = db.Appointment.Find(id);
            db.Appointment.Remove(appointment);
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
    }
}
