using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DiabetesFoot.Models
{
    public class DiabetesFootDbContext : IdentityDbContext<ApplicationUser>
    {
        public DiabetesFootDbContext() : base("DiabetesFootConnection")
        {
        }

        public DbSet<Patient> Patients { get; set; }
        //public DbSet<Doctor> Doctors { get; set; }
        public DbSet<BloodGlucoseRecord> BloodGlucoseRecords { get; set; }
        public DbSet<WoundRecord> WoundRecords { get; set; } // 伤口记录表
        public DbSet<Medication> Medications { get; set; } // 药物管理表

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 配置多对多关系
            //modelBuilder.Entity<Doctor>()
            //    .HasMany(d => d.Patients)
            //    .WithMany(p => p.Doctors)
            //    .Map(m =>
            //    {
            //        m.ToTable("DoctorPatientRelations");
            //        m.MapLeftKey("DoctorId");
            //        m.MapRightKey("PatientId");
            //    });
        }

        public static DiabetesFootDbContext Create()
        {
            return new DiabetesFootDbContext();
        }
    }
}