using DiabetesFoot.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DiabetesFoot.Controllers
{
    [Authorize] // 必须登录才能访问
    public class BloodSugarController : Controller
    {
        // 数据库上下文（复用项目已有配置）
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        // 1. 血糖记录列表页
        public async Task<ActionResult> Index()
        {
            // 获取当前登录用户ID（EF 6.5.1专属写法）
            var userId = User.Identity.GetUserId();
            // 查询当前用户的血糖记录（按时间倒序）
            var bloodSugars = await _context.BloodSugars
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.RecordTime)
                .ToListAsync();
            return View(bloodSugars);
        }

        // 2. 跳转到新增血糖记录页
        public ActionResult Create()
        {
            // 默认显示当前时间
            ViewBag.CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            return View();
        }

        // 3. 提交新增血糖记录（表单提交触发）
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BloodSugar bloodSugar)
        {
            if (ModelState.IsValid)
            {
                // 绑定当前登录用户ID
                bloodSugar.UserId = User.Identity.GetUserId();
                _context.BloodSugars.Add(bloodSugar);
                await _context.SaveChangesAsync();
                // 提示成功并跳转
                TempData["SuccessMessage"] = "血糖记录添加成功！";
                return RedirectToAction("Index");
            }
            // 数据不合法，返回表单页重新填写
            return View(bloodSugar);
        }

        // 4. 血糖趋势分析页（7天/30天）
        public async Task<ActionResult> Trend(string timeRange = "7day")
        {
            var userId = User.Identity.GetUserId();
            var endDate = DateTime.Now;
            var startDate = timeRange == "30day" ? endDate.AddDays(-30) : endDate.AddDays(-7);

            // 查询时间范围内的血糖数据（按日期分组取平均值）
            var trendData = await _context.BloodSugars
                .Where(b => b.UserId == userId && b.RecordTime >= startDate && b.RecordTime <= endDate)
                .GroupBy(b => b.RecordTime.Date)
                .Select(g => new
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    AvgValue = g.Average(b => b.BloodSugarValue)
                })
                .OrderBy(d => d.Date)
                .ToListAsync();

            // 把数据传给前端（JSON序列化）
            ViewBag.TrendData = Newtonsoft.Json.JsonConvert.SerializeObject(trendData);
            ViewBag.SelectedRange = timeRange;
            return View();
        }
    }
}