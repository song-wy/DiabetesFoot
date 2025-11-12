using DiabetesFoot.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DiabetesFoot.Controllers
{
    public class WoundRecordController : Controller
    {
        // 使用全局的 DiabetesFootDbContext（已在 Models/DiabetesFootDbContext.cs 中定义）
        private DiabetesFootDbContext db = new DiabetesFootDbContext();

        // GET: 伤口记录列表
        public ActionResult Index(int patientId)
        {
            // 获取患者信息
            var patient = db.Patients.Find(patientId);
            if (patient == null)
            {
                return HttpNotFound("患者不存在");
            }

            // 获取该患者的所有伤口记录
            var records = db.WoundRecords
                          .Where(w => w.PatientId == patientId)
                          .OrderByDescending(w => w.RecordDate)
                          .ToList();

            // 准备愈合进度数据用于图表显示
            var healingProgress = records
                .OrderBy(r => r.RecordDate)
                .Select(r => new {
                    Date = r.RecordDate.ToString("yyyy-MM-dd"),
                    Size = r.Size ?? 0
                })
                .ToList();

            // 传递数据到视图
            ViewBag.PatientId = patientId;
            ViewBag.PatientName = patient.Name;
            ViewBag.HealingProgress = new
            {
                Labels = healingProgress.Select(h => h.Date).ToArray(),
                Sizes = healingProgress.Select(h => h.Size).ToArray()
            };

            return View(records);
        }

        // GET: 添加伤口记录
        public ActionResult Create(int patientId)
        {
            ViewBag.PatientId = patientId;
            ViewBag.WoundTypes = GetWoundTypes(); // 添加伤口类型下拉选项
            ViewBag.SeverityLevels = GetSeverityLevels(); // 添加严重程度下拉选项
            return View(new WoundRecord { PatientId = patientId });
        }

        // POST: 保存伤口记录
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WoundRecord record, HttpPostedFileBase woundPhoto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // 处理照片上传
                    if (woundPhoto != null)
                    {
                        if (woundPhoto.ContentLength > 5 * 1024 * 1024) // 限制5MB
                        {
                            ModelState.AddModelError("woundPhoto", "照片大小不能超过5MB");
                        }
                        else
                        {
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(woundPhoto.FileName);
                            string path = Path.Combine(Server.MapPath("~/Uploads/WoundPhotos"), fileName);

                            // 确保目录存在
                            Directory.CreateDirectory(Path.GetDirectoryName(path));

                            woundPhoto.SaveAs(path);
                            record.PhotoPath = "/Uploads/WoundPhotos/" + fileName;
                        }
                    }

                    // 设置默认值
                    record.RecordDate = DateTime.Now;

                    if (ModelState.IsValid)
                    {
                        db.WoundRecords.Add(record);
                        db.SaveChanges();
                        TempData["SuccessMessage"] = "伤口记录添加成功!";
                        return RedirectToAction("Index", new { patientId = record.PatientId });
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "保存失败: " + ex.Message);
            }

            // 如果模型验证失败，返回视图并保留输入数据
            ViewBag.WoundTypes = GetWoundTypes();
            ViewBag.SeverityLevels = GetSeverityLevels();
            return View(record);
        }

        // GET: 查看伤口记录详情
        public ActionResult Details(int id)
        {
            var record = db.WoundRecords.Find(id);
            if (record == null)
            {
                return HttpNotFound();
            }
            return View(record);
        }

        // GET: 编辑伤口记录
        public ActionResult Edit(int id)
        {
            var record = db.WoundRecords.Find(id);
            if (record == null)
            {
                return HttpNotFound();
            }
            ViewBag.WoundTypes = GetWoundTypes();
            ViewBag.SeverityLevels = GetSeverityLevels();
            return View(record);
        }

        // POST: 更新伤口记录
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(WoundRecord record, HttpPostedFileBase woundPhoto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // 从数据库获取原有记录
                    var existingRecord = db.WoundRecords.Find(record.WoundId);
                    if (existingRecord == null)
                    {
                        return HttpNotFound("伤口记录不存在");
                    }

                    // 处理照片更新
                    if (woundPhoto != null && woundPhoto.ContentLength > 0)
                    {
                        // 验证照片大小（限制5MB）
                        if (woundPhoto.ContentLength > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("woundPhoto", "照片大小不能超过5MB");
                            ViewBag.WoundTypes = GetWoundTypes();
                            ViewBag.SeverityLevels = GetSeverityLevels();
                            return View(record);
                        }

                        // 删除旧照片
                        if (!string.IsNullOrEmpty(existingRecord.PhotoPath))
                        {
                            string oldPath = Server.MapPath(existingRecord.PhotoPath);
                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }

                        // 保存新照片
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(woundPhoto.FileName);
                        string path = Path.Combine(Server.MapPath("~/Uploads/WoundPhotos"), fileName);
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                        woundPhoto.SaveAs(path);
                        existingRecord.PhotoPath = "/Uploads/WoundPhotos/" + fileName;
                    }

                    // 更新其他字段
                    existingRecord.RecordDate = record.RecordDate;
                    existingRecord.Position = record.Position;
                    existingRecord.Size = record.Size;
                    existingRecord.Description = record.Description;
                    existingRecord.HealingStatus = record.HealingStatus;

                    // 保存更改
                    db.Entry(existingRecord).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "伤口记录更新成功!";
                    return RedirectToAction("Index", new { patientId = existingRecord.PatientId });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "更新失败: " + ex.Message);
            }

            // 如果模型验证失败，返回视图并保留输入数据
            ViewBag.WoundTypes = GetWoundTypes();
            ViewBag.SeverityLevels = GetSeverityLevels();
            return View(record);
        }

        // GET: 删除伤口记录
        public ActionResult Delete(int id)
        {
            var record = db.WoundRecords.Find(id);
            if (record == null)
            {
                return HttpNotFound();
            }
            return View(record);
        }

        // POST: 确认删除伤口记录
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var record = db.WoundRecords.Find(id);
            if (record != null)
            {
                // 删除关联的照片文件
                if (!string.IsNullOrEmpty(record.PhotoPath))
                {
                    string path = Server.MapPath(record.PhotoPath);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                db.WoundRecords.Remove(record);
                db.SaveChanges();
                TempData["SuccessMessage"] = "伤口记录已删除!";
                return RedirectToAction("Index", new { patientId = record.PatientId });
            }
            return HttpNotFound();
        }

        // 伤口愈合进度跟踪
        public ActionResult HealingProgress(int patientId)
        {
            var records = db.WoundRecords
                          .Where(w => w.PatientId == patientId)
                          .OrderBy(w => w.RecordDate)
                          .ToList();

            if (!records.Any())
            {
                return Json(new { message = "没有找到伤口记录" }, JsonRequestBehavior.AllowGet);
            }

            var progressData = records.Select(r => new {
                Date = r.RecordDate.ToString("yyyy-MM-dd"),
                Size = r.Size,
                Status = r.HealingStatus,
                //Severity = r.SeverityLevel,
                //Location = r.Location
            });

            return Json(progressData, JsonRequestBehavior.AllowGet);
        }

        // 获取伤口类型列表
        private List<SelectListItem> GetWoundTypes()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Ulcer", Text = "溃疡" },
                new SelectListItem { Value = "Abrasion", Text = "擦伤" },
                new SelectListItem { Value = "Laceration", Text = "撕裂伤" },
                new SelectListItem { Value = "Puncture", Text = "穿刺伤" },
                new SelectListItem { Value = "Burn", Text = "烧伤" },
                new SelectListItem { Value = "Other", Text = "其他" }
            };
        }

        // 获取严重程度列表
        private List<SelectListItem> GetSeverityLevels()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Mild", Text = "轻度" },
                new SelectListItem { Value = "Moderate", Text = "中度" },
                new SelectListItem { Value = "Severe", Text = "重度" },
                new SelectListItem { Value = "Critical", Text = "危急" }
            };
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