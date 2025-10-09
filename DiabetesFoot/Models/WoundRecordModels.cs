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
        [Key]
        [Column(Order = 1)]
        public int PatientId { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime RecordDate { get; set; }
        public int WoundId { get; set; }
        
        public string Position { get; set; }
        public decimal? Size { get; set; }
        public string PhotoPath { get; set; }
        public string Description { get; set; }
        
        public string HealingStatus { get; set; }

        // 导航属性
        public virtual Patient Patient { get; set; }
    }
}