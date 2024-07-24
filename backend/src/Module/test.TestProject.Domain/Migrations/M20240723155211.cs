using FluentMigrator;
using Shesha.FluentMigrator;
using System;

namespace Boxfusion.Membership.Common.Domain.Migrations
{
    [Migration(20240723155211)]
    public class M20240723155211 : Migration
    {
        /// <summary>
        /// Code to execute when executing the migrations
        /// </summary>
        public override void Up()
        {
            Create.Table("TP_Vehicles")
                .WithIdAsGuid()
                .WithFullAuditColumns()
                .WithColumn("RegistrationNumber").AsString().NotNullable().Unique()
                .WithColumn("Make").AsString().NotNullable()
                .WithColumn("Model").AsString().NotNullable()
                .WithColumn("Year").AsString().NotNullable();
        }
        /// <summary>
        /// Code to execute when rolling back the migration
        /// </summary>
        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}