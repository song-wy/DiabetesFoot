using DiabetesFoot.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DiabetesFoot.Controllers
{
    /// <summary>
    /// 药物管理控制器
    /// </summary>
    [Authorize] // 需要登录才能访问
    public class MedicationController : Controller
    {
        // 使用全局的 DiabetesFootDbContext
        private DiabetesFootDbContext db = new DiabetesFootDbContext();

        // GET: Medication - 药物列表（按患者分组显示）
        public ActionResult Index(int? patientId)
        {
            var userId = User.Identity.GetUserId();

            // 如果指定了患者ID，显示该患者的药物列表
            if (patientId.HasValue)
            {
                // 验证患者是否属于当前用户
                var patient = db.Patients.FirstOrDefault(p => p.PatientId == patientId.Value && p.UserId == userId);
                if (patient == null)
                {
                    return HttpNotFound("患者不存在或无权访问");
                }

                // 获取该患者的所有药物记录
                var medications = db.Medications
                    .Where(m => m.PatientId == patientId.Value)
                    .OrderByDescending(m => m.CreateDate)
                    .ToList();

                ViewBag.PatientId = patientId.Value;
                ViewBag.PatientName = patient.Name;
                return View(medications);
            }

            // 如果没有指定患者ID，显示当前用户所有患者的药物列表
            var patients = db.Patients.Where(p => p.UserId == userId).ToList();
            ViewBag.Patients = patients;
            
            // 获取所有患者的药物记录
            var allMedications = db.Medications
                .Where(m => patients.Select(p => p.PatientId).Contains(m.PatientId))
                .OrderByDescending(m => m.CreateDate)
                .ToList();

            return View(allMedications);
        }

        // GET: Medication/Create - 添加药物记录
        public ActionResult Create(int patientId)
        {
            // 验证患者是否属于当前用户
            var userId = User.Identity.GetUserId();
            var patient = db.Patients.FirstOrDefault(p => p.PatientId == patientId && p.UserId == userId);
            if (patient == null)
            {
                return HttpNotFound("患者不存在或无权访问");
            }

            ViewBag.PatientId = patientId;
            ViewBag.PatientName = patient.Name;
            
            // 创建新的药物记录，设置默认值
            var medication = new Medication
            {
                PatientId = patientId,
                StartDate = DateTime.Now,
                IsReminderEnabled = true,
                CreateDate = DateTime.Now
            };

            return View(medication);
        }

        // POST: Medication/Create - 保存药物记录
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Medication medication)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // 验证患者是否属于当前用户
                    var userId = User.Identity.GetUserId();
                    var patient = db.Patients.FirstOrDefault(p => p.PatientId == medication.PatientId && p.UserId == userId);
                    if (patient == null)
                    {
                        return HttpNotFound("患者不存在或无权访问");
                    }

                    // 设置创建时间
                    medication.CreateDate = DateTime.Now;

                    // 保存药物记录
                    db.Medications.Add(medication);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "药物记录添加成功！";
                    return RedirectToAction("Index", new { patientId = medication.PatientId });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "保存失败: " + ex.Message);
            }

            // 如果模型验证失败，返回视图并保留输入数据
            ViewBag.PatientId = medication.PatientId;
            var patientInfo = db.Patients.Find(medication.PatientId);
            if (patientInfo != null)
            {
                ViewBag.PatientName = patientInfo.Name;
            }
            return View(medication);
        }

        // GET: Medication/Details/5 - 查看药物记录详情
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            // 获取药物记录，并验证患者是否属于当前用户
            var userId = User.Identity.GetUserId();
            var medication = db.Medications
                .Include(m => m.Patient)
                .FirstOrDefault(m => m.MedicationId == id && m.Patient.UserId == userId);

            if (medication == null)
            {
                return HttpNotFound("药物记录不存在或无权访问");
            }

            return View(medication);
        }

        // GET: Medication/Edit/5 - 编辑药物记录
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            // 获取药物记录，并验证患者是否属于当前用户
            var userId = User.Identity.GetUserId();
            var medication = db.Medications
                .Include(m => m.Patient)
                .FirstOrDefault(m => m.MedicationId == id && m.Patient.UserId == userId);

            if (medication == null)
            {
                return HttpNotFound("药物记录不存在或无权访问");
            }

            return View(medication);
        }

        // POST: Medication/Edit/5 - 更新药物记录
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Medication medication)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // 验证患者是否属于当前用户
                    var userId = User.Identity.GetUserId();
                    var existingMedication = db.Medications
                        .Include(m => m.Patient)
                        .FirstOrDefault(m => m.MedicationId == medication.MedicationId && m.Patient.UserId == userId);

                    if (existingMedication == null)
                    {
                        return HttpNotFound("药物记录不存在或无权访问");
                    }

                    // 更新药物记录信息
                    existingMedication.MedicationName = medication.MedicationName;
                    existingMedication.Dosage = medication.Dosage;
                    existingMedication.Frequency = medication.Frequency;
                    existingMedication.StartDate = medication.StartDate;
                    existingMedication.EndDate = medication.EndDate;
                    existingMedication.ReminderTime = medication.ReminderTime;
                    existingMedication.Notes = medication.Notes;
                    existingMedication.IsReminderEnabled = medication.IsReminderEnabled;

                    // 保存更改
                    db.Entry(existingMedication).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "药物记录更新成功！";
                    return RedirectToAction("Index", new { patientId = existingMedication.PatientId });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "更新失败: " + ex.Message);
            }

            // 如果模型验证失败，返回视图并保留输入数据
            return View(medication);
        }

        // GET: Medication/Delete/5 - 删除药物记录
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            // 获取药物记录，并验证患者是否属于当前用户
            var userId = User.Identity.GetUserId();
            var medication = db.Medications
                .Include(m => m.Patient)
                .FirstOrDefault(m => m.MedicationId == id && m.Patient.UserId == userId);

            if (medication == null)
            {
                return HttpNotFound("药物记录不存在或无权访问");
            }

            return View(medication);
        }

        // POST: Medication/Delete/5 - 确认删除药物记录
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // 验证患者是否属于当前用户
            var userId = User.Identity.GetUserId();
            var medication = db.Medications
                .Include(m => m.Patient)
                .FirstOrDefault(m => m.MedicationId == id && m.Patient.UserId == userId);

            if (medication == null)
            {
                return HttpNotFound("药物记录不存在或无权访问");
            }

            int patientId = medication.PatientId;

            // 删除药物记录
            db.Medications.Remove(medication);
            db.SaveChanges();
            TempData["SuccessMessage"] = "药物记录已删除！";
            return RedirectToAction("Index", new { patientId = patientId });
        }

        // GET: Medication/Reminder - 药物提醒列表
        public ActionResult Reminder(int? patientId)
        {
            var userId = User.Identity.GetUserId();
            var today = DateTime.Now.Date;

            IQueryable<Medication> query = db.Medications
                .Include(m => m.Patient)
                .Where(m => m.Patient.UserId == userId && m.IsReminderEnabled == true);

            // 如果指定了患者ID，只显示该患者的提醒
            if (patientId.HasValue)
            {
                query = query.Where(m => m.PatientId == patientId.Value);
            }

            // 筛选出有效的药物（开始日期 <= 今天，且结束日期为空或 >= 今天）
            var medications = query
                .Where(m => m.StartDate <= today && (m.EndDate == null || m.EndDate >= today))
                .OrderBy(m => m.ReminderTime)
                .ToList();

            ViewBag.PatientId = patientId;
            return View(medications);
        }

        // 释放资源
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
