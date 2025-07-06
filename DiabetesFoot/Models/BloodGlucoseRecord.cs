using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DiabetesFoot.Models
{
    public class BloodGlucoseRecord
    {
        [Key]
        public int RecordId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        [Range(1, 30)]
        [Display(Name = "血糖值(mmol/L)")]
        public decimal GlucoseValue { get; set; }

        [Required]
        [Display(Name = "记录时间")]
        public DateTime RecordTime { get; set; }

        [Required]
        [Display(Name = "测量类型")]
        public string MeasureType { get; set; }

        [Display(Name = "备注")]
        public string Notes { get; set; }

        [Display(Name = "是否危急值")]
        public bool IsCritical { get; set; }

        // 导航属性
        public virtual Patient Patient { get; set; }
    }
}