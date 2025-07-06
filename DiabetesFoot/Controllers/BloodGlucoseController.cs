using DiabetesFoot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DiabetesFoot.Controllers
{
    public class BloodGlucoseController : Controller
    {
        private DiabetesFootDbContext db = new DiabetesFootDbContext();

        // GET: BloodGlucose/Record/5 (5是患者ID)
        public ActionResult Record(int id, DateTime? startDate, DateTime? endDate)
        {
            var patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }

            ViewBag.PatientName = patient.Name;
            ViewBag.PatientId = id;

            IQueryable<BloodGlucoseRecord> query = db.BloodGlucoseRecords
                .Where(r => r.PatientId == id)
                .OrderByDescending(r => r.RecordTime);

            // 添加时间筛选
            if (startDate.HasValue)
            {
                query = query.Where(r => r.RecordTime >= startDate);
            }
            if (endDate.HasValue)
            {
                query = query.Where(r => r.RecordTime <= endDate);
            }

            // 获取最近10条记录用于首页显示
            var recentRecords = query.Take(10).ToList();

            return View(recentRecords);
        }

        // GET: BloodGlucose/AllRecords/5
        public ActionResult AllRecords(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index", "Patient"); // 重定向到患者列表
            }

            var patient = db.Patients.Find(id.Value);
            if (patient == null)
            {
                return HttpNotFound();
            }

            ViewBag.PatientName = patient.Name;
            ViewBag.PatientId = id.Value;

            var records = db.BloodGlucoseRecords
                           .Where(r => r.PatientId == id.Value)
                           .OrderByDescending(r => r.RecordTime)
                           .ToList();

            return View(records);
        }

        // POST: BloodGlucose/Record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Record(BloodGlucoseRecord record)
        {
            if (ModelState.IsValid)
            {
                // 检查是否危急值
                record.IsCritical = (record.GlucoseValue < 3.9m || record.GlucoseValue > 11.1m);

                db.BloodGlucoseRecords.Add(record);
                db.SaveChanges();

                if (record.IsCritical)
                {
                    TempData["AlertMessage"] = $"警告：血糖值异常({record.GlucoseValue}mmol/L)！";
                    TempData["AlertType"] = "danger";
                }
                else
                {
                    TempData["AlertMessage"] = "血糖记录已保存";
                    TempData["AlertType"] = "success";
                }

                return RedirectToAction("Record", new { id = record.PatientId });
            }

            ViewBag.PatientId = record.PatientId;
            return View(db.BloodGlucoseRecords.Where(r => r.PatientId == record.PatientId).ToList());
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