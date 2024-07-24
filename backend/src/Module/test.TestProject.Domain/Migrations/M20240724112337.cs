using FluentMigrator;
using Shesha.FluentMigrator;
using System;

namespace Boxfusion.Membership.Common.Domain.Migrations
{
    [Migration(20240724112337)]
    public class M20240724112337 : Migration
    {
        /// <summary>
        /// Code to execute when executing the migrations
        /// </summary>
        public override void Up()
        {
            Create.Table("TP_VehicleReservations")
                .WithIdAsGuid()
                .WithFullAuditColumns()
                .WithForeignKeyColumn("VehicleId", "TP_Vehicles").NotNullable()
                .WithForeignKeyColumn("ReserverId", "Core_Persons").NotNullable()
                .WithColumn("From").AsDateTime().NotNullable()
                .WithColumn("To").AsDateTime().NotNullable();
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