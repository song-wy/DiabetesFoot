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
                TempData["SuccessMessage"] = "患者信息添加成功！";
                return RedirectToAction("Index");
            }
            return View(patient);
        }

        // GET: Patient/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            // 获取患者信息，并验证是否属于当前用户
            var userId = User.Identity.GetUserId();
            var patient = db.Patients.FirstOrDefault(p => p.PatientId == id && p.UserId == userId);
            
            if (patient == null)
            {
                return HttpNotFound("患者不存在或无权访问");
            }

            // 加载相关的血糖记录和伤口记录
            var bloodGlucoseRecords = db.BloodGlucoseRecords
                .Where(b => b.PatientId == id)
                .OrderByDescending(b => b.RecordTime)
                .Take(10)
                .ToList();

            var woundRecords = db.WoundRecords
                .Where(w => w.PatientId == id)
                .OrderByDescending(w => w.RecordDate)
                .Take(10)
                .ToList();

            ViewBag.BloodGlucoseRecords = bloodGlucoseRecords;
            ViewBag.WoundRecords = woundRecords;

            return View(patient);
        }

        // GET: Patient/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            // 获取患者信息，并验证是否属于当前用户
            var userId = User.Identity.GetUserId();
            var patient = db.Patients.FirstOrDefault(p => p.PatientId == id && p.UserId == userId);
            
            if (patient == null)
            {
                return HttpNotFound("患者不存在或无权访问");
            }

            return View(patient);
        }

        // POST: Patient/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Patient patient)
        {
            if (ModelState.IsValid)
            {
                // 验证患者是否属于当前用户
                var userId = User.Identity.GetUserId();
                var existingPatient = db.Patients.FirstOrDefault(p => p.PatientId == patient.PatientId && p.UserId == userId);
                
                if (existingPatient == null)
                {
                    return HttpNotFound("患者不存在或无权访问");
                }

                // 更新患者信息（保留 UserId 和 RegisterDate）
                existingPatient.Name = patient.Name;
                existingPatient.Gender = patient.Gender;
                existingPatient.BirthDate = patient.BirthDate;
                existingPatient.Phone = patient.Phone;
                existingPatient.EmergencyContact = patient.EmergencyContact;
                existingPatient.EmergencyPhone = patient.EmergencyPhone;
                existingPatient.DiabetesType = patient.DiabetesType;
                existingPatient.WagnerGrade = patient.WagnerGrade;

                db.Entry(existingPatient).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "患者信息更新成功！";
                return RedirectToAction("Index");
            }
            return View(patient);
        }

        // GET: Patient/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            // 获取患者信息，并验证是否属于当前用户
            var userId = User.Identity.GetUserId();
            var patient = db.Patients.FirstOrDefault(p => p.PatientId == id && p.UserId == userId);
            
            if (patient == null)
            {
                return HttpNotFound("患者不存在或无权访问");
            }

            return View(patient);
        }

        // POST: Patient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // 验证患者是否属于当前用户
            var userId = User.Identity.GetUserId();
            var patient = db.Patients.FirstOrDefault(p => p.PatientId == id && p.UserId == userId);
            
            if (patient == null)
            {
                return HttpNotFound("患者不存在或无权访问");
            }

            // 删除相关的血糖记录
            var bloodGlucoseRecords = db.BloodGlucoseRecords.Where(b => b.PatientId == id).ToList();
            db.BloodGlucoseRecords.RemoveRange(bloodGlucoseRecords);

            // 删除相关的伤口记录和照片
            var woundRecords = db.WoundRecords.Where(w => w.PatientId == id).ToList();
            foreach (var wound in woundRecords)
            {
                // 删除伤口照片文件
                if (!string.IsNullOrEmpty(wound.PhotoPath))
                {
                    string photoPath = System.Web.Hosting.HostingEnvironment.MapPath(wound.PhotoPath);
                    if (System.IO.File.Exists(photoPath))
                    {
                        System.IO.File.Delete(photoPath);
                    }
                }
            }
            db.WoundRecords.RemoveRange(woundRecords);

            // 删除相关的药物记录
            var medications = db.Medications.Where(m => m.PatientId == id).ToList();
            db.Medications.RemoveRange(medications);

            // 删除患者
            db.Patients.Remove(patient);
            db.SaveChanges();
            TempData["SuccessMessage"] = "患者信息已删除！";
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