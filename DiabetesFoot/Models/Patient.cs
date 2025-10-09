using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DiabetesFoot.Models
{
    public class Patient
    {
        [Required]
        public int PatientId { get; set; }
        public string UserId { get; set; } // 关联ASP.NET用户

        [Required]
        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Display(Name = "性别")]
        public string Gender { get; set; }

        [Display(Name = "出生日期")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Required]
        [Phone]
        [Display(Name = "手机号")]
        public string Phone { get; set; }

        [Display(Name = "紧急联系人")]
        public string EmergencyContact { get; set; }

        [Phone]
        [Display(Name = "紧急联系电话")]
        public string EmergencyPhone { get; set; }

        [Display(Name = "糖尿病类型")]
        public int? DiabetesType { get; set; } // 1=1型, 2=2型

        [Display(Name = "Wagner分级")]
        public int? WagnerGrade { get; set; }

        [Display(Name = "注册日期")]
        public DateTime RegisterDate { get; set; }

        // 导航属性
        public virtual ICollection<BloodGlucoseRecord> BloodGlucoseRecords { get; set; }
        public virtual ICollection<WoundRecord> WoundRecords { get; set; }
        //public virtual ICollection<Medication> Medications { get; set; }
        //public virtual ICollection<DoctorPatientRelation> Doctors { get; set; }
    }
}