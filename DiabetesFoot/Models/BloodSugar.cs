using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace DiabetesFoot.Models
{
    // 血糖记录模型（EF 6.5.1适配）
    public class BloodSugar
    {
        // 主键（自动增长）
        public int Id { get; set; }

        // 关联当前登录用户（与ApplicationUser的Id对应）
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        // 记录时间（必填）
        [Required(ErrorMessage = "请选择记录时间")]
        [Display(Name = "记录时间")]
        public DateTime RecordTime { get; set; }

        // 血糖值（必填，范围3.9-20.0 mmol/L）
        [Required(ErrorMessage = "请输入血糖值")]
        [Range(3.9, 20.0, ErrorMessage = "血糖值范围应为3.9-20.0 mmol/L")]
        [Display(Name = "血糖值(mmol/L)")]
        public double BloodSugarValue { get; set; }

        // 记录类型（空腹/餐后2小时/随机）
        [Required(ErrorMessage = "请选择记录类型")]
        [Display(Name = "记录类型")]
        public string RecordType { get; set; } // 可选值："空腹"、"餐后2小时"、"随机"

        // 备注（可选）
        [Display(Name = "备注")]
        [MaxLength(500, ErrorMessage = "备注不能超过500字")]
        public string Remark { get; set; } // Removed nullable reference type syntax for compatibility with C# 7.3
    }

    // 数据库上下文扩展（确保能识别BloodSugar模型）
    // 注意：ApplicationDbContext 已在 IdentityModels.cs 中定义为 IdentityDbContext<ApplicationUser>
    // 这里扩展它，添加 BloodSugar 表
    public partial class ApplicationDbContext
    {
        public DbSet<BloodSugar> BloodSugars { get; set; }
    }
}