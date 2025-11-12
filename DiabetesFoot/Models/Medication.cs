using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiabetesFoot.Models
{
    /// <summary>
    /// 药物管理模型
    /// </summary>
    public class Medication
    {
        // 主键：药物ID（自增）
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MedicationId { get; set; }

        // 患者ID（外键，关联到 Patient 表）
        [Required(ErrorMessage = "患者ID不能为空")]
        public int PatientId { get; set; }

        // 药物名称（必填）
        [Required(ErrorMessage = "请输入药物名称")]
        [Display(Name = "药物名称")]
        [MaxLength(200, ErrorMessage = "药物名称不能超过200个字符")]
        public string MedicationName { get; set; }

        // 药物剂量（必填）
        [Required(ErrorMessage = "请输入药物剂量")]
        [Display(Name = "剂量")]
        [MaxLength(100, ErrorMessage = "剂量描述不能超过100个字符")]
        public string Dosage { get; set; }

        // 用药频率（必填）
        [Required(ErrorMessage = "请输入用药频率")]
        [Display(Name = "用药频率")]
        [MaxLength(100, ErrorMessage = "用药频率不能超过100个字符")]
        public string Frequency { get; set; }

        // 开始日期（必填）
        [Required(ErrorMessage = "请选择开始日期")]
        [Display(Name = "开始日期")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        // 结束日期（可选）
        [Display(Name = "结束日期")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        // 提醒时间（可选，格式：HH:mm）
        [Display(Name = "提醒时间")]
        [MaxLength(10, ErrorMessage = "提醒时间格式不正确")]
        public string ReminderTime { get; set; }

        // 备注（可选）
        [Display(Name = "备注")]
        [MaxLength(500, ErrorMessage = "备注不能超过500个字符")]
        public string Notes { get; set; }

        // 是否启用提醒（默认启用）
        [Display(Name = "启用提醒")]
        public bool IsReminderEnabled { get; set; } = true;

        // 创建时间
        [Display(Name = "创建时间")]
        public DateTime CreateDate { get; set; }

        // 导航属性：关联到患者
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
    }
}

