namespace DiabetesFoot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddKeyToWoundRecord : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WoundRecords",
                c => new
                    {
                        PatientId = c.Int(nullable: false),
                        RecordDate = c.DateTime(nullable: false),
                        WoundId = c.Int(nullable: false),
                        Position = c.String(),
                        Size = c.Decimal(precision: 18, scale: 2),
                        PhotoPath = c.String(),
                        Description = c.String(),
                        HealingStatus = c.String(),
                    })
                .PrimaryKey(t => new { t.PatientId, t.RecordDate })
                .ForeignKey("dbo.Patients", t => t.PatientId, cascadeDelete: true)
                .Index(t => t.PatientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WoundRecords", "PatientId", "dbo.Patients");
            DropIndex("dbo.WoundRecords", new[] { "PatientId" });
            DropTable("dbo.WoundRecords");
        }
    }
}
