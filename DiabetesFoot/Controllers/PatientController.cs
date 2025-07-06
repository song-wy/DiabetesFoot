using DiabetesFoot.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DiabetesFoot.Controllers
{
    public class PatientController : Controller
    {
        private DiabetesFootDbContext db = new DiabetesFootDbContext();

        // GET: Patient
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var patients = db.Patients.Where(p => p.UserId == userId).ToList();
            return View(patients);
        }

        // GET: Patient/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Patient/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Patient patient)
        {
            if (ModelState.IsValid)
            {
                patient.UserId = User.Identity.GetUserId();
                patient.RegisterDate = DateTime.Now;
                db.Patients.Add(patient);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(patient);
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