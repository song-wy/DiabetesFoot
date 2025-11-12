using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DiabetesFoot.Models
{
    public class WoundRecord
    {
        // 主键：伤口记录ID（自增）
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WoundId { get; set; }

        // 患者ID（外键，关联到 Patient 表）
        [Required]
        public int PatientId { get; set; }

        // 记录日期
        [Required]
        [Display(Name = "记录日期")]
        [DataType(DataType.DateTime)]
        public DateTime RecordDate { get; set; }
        
        // 伤口位置
        [Display(Name = "伤口位置")]
        [MaxLength(200)]
        public string Position { get; set; }

        // 伤口大小（平方厘米）
        [Display(Name = "伤口大小(cm²)")]
        [Range(0, 1000, ErrorMessage = "伤口大小必须在0-1000之间")]
        public decimal? Size { get; set; }

        // 照片路径
        [Display(Name = "照片路径")]
        public string PhotoPath { get; set; }

        // 伤口描述
        [Display(Name = "伤口描述")]
        [MaxLength(1000)]
        public string Description { get; set; }
        
        // 愈合状态
        [Display(Name = "愈合状态")]
        [MaxLength(50)]
        public string HealingStatus { get; set; }

        // 导航属性：关联到患者
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
    }
}