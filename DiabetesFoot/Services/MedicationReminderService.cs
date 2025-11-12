using DiabetesFoot.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiabetesFoot.Services
{
    /// <summary>
    /// 药物提醒服务
    /// 提供药物提醒相关的业务逻辑
    /// </summary>
    public class MedicationReminderService
    {
        private DiabetesFootDbContext db;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public MedicationReminderService(DiabetesFootDbContext context)
        {
            db = context;
        }

        /// <summary>
        /// 获取即将到来的提醒（今天需要服用的药物）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>需要提醒的药物列表</returns>
        public List<Medication> GetUpcomingReminders(string userId)
        {
            var today = DateTime.Now.Date;
            var currentTime = DateTime.Now.TimeOfDay;

            // 获取当前用户的所有患者
            var patients = db.Patients.Where(p => p.UserId == userId).ToList();
            var patientIds = patients.Select(p => p.PatientId).ToList();

            // 获取今天需要提醒的药物
            var medications = db.Medications
                .Where(m => patientIds.Contains(m.PatientId) &&
                           m.IsReminderEnabled == true &&
                           m.StartDate <= today &&
                           (m.EndDate == null || m.EndDate >= today))
                .ToList();

            // 如果设置了提醒时间，筛选出当前时间之后的提醒
            var upcomingReminders = medications
                .Where(m => string.IsNullOrEmpty(m.ReminderTime) || 
                           TimeSpan.TryParse(m.ReminderTime, out TimeSpan reminderTime) &&
                           reminderTime >= currentTime)
                .OrderBy(m => m.ReminderTime)
                .ToList();

            return upcomingReminders;
        }

        /// <summary>
        /// 获取今天的提醒列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>今天的提醒列表</returns>
        public List<Medication> GetTodayReminders(string userId)
        {
            var today = DateTime.Now.Date;

            // 获取当前用户的所有患者
            var patients = db.Patients.Where(p => p.UserId == userId).ToList();
            var patientIds = patients.Select(p => p.PatientId).ToList();

            // 获取今天需要提醒的药物
            var medications = db.Medications
                .Where(m => patientIds.Contains(m.PatientId) &&
                           m.IsReminderEnabled == true &&
                           m.StartDate <= today &&
                           (m.EndDate == null || m.EndDate >= today))
                .OrderBy(m => m.ReminderTime)
                .ToList();

            return medications;
        }

        /// <summary>
        /// 检查药物是否过期
        /// </summary>
        /// <param name="medication">药物记录</param>
        /// <returns>是否过期</returns>
        public bool IsMedicationExpired(Medication medication)
        {
            if (medication.EndDate.HasValue)
            {
                return medication.EndDate.Value < DateTime.Now.Date;
            }
            return false;
        }

        /// <summary>
        /// 检查药物是否有效（在有效期内）
        /// </summary>
        /// <param name="medication">药物记录</param>
        /// <returns>是否有效</returns>
        public bool IsMedicationActive(Medication medication)
        {
            var today = DateTime.Now.Date;
            return medication.StartDate <= today &&
                   (medication.EndDate == null || medication.EndDate >= today);
        }

        /// <summary>
        /// 获取需要提醒的药物数量
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>需要提醒的药物数量</returns>
        public int GetReminderCount(string userId)
        {
            var today = DateTime.Now.Date;

            // 获取当前用户的所有患者
            var patients = db.Patients.Where(p => p.UserId == userId).ToList();
            var patientIds = patients.Select(p => p.PatientId).ToList();

            // 统计需要提醒的药物数量
            var count = db.Medications
                .Count(m => patientIds.Contains(m.PatientId) &&
                           m.IsReminderEnabled == true &&
                           m.StartDate <= today &&
                           (m.EndDate == null || m.EndDate >= today));

            return count;
        }

        /// <summary>
        /// 获取即将过期的药物（7天内过期）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>即将过期的药物列表</returns>
        public List<Medication> GetExpiringMedications(string userId, int days = 7)
        {
            var today = DateTime.Now.Date;
            var expirationDate = today.AddDays(days);

            // 获取当前用户的所有患者
            var patients = db.Patients.Where(p => p.UserId == userId).ToList();
            var patientIds = patients.Select(p => p.PatientId).ToList();

            // 获取即将过期的药物
            var medications = db.Medications
                .Where(m => patientIds.Contains(m.PatientId) &&
                           m.EndDate.HasValue &&
                           m.EndDate >= today &&
                           m.EndDate <= expirationDate)
                .OrderBy(m => m.EndDate)
                .ToList();

            return medications;
        }
    }
}
