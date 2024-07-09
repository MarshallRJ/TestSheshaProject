using FluentMigrator;
using Shesha.FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// <summary>
/// Adding the Members table
/// </summary>

[Migration(20240709112300)]
public class M20240709112300 : Migration
{
    /// <summary>
    /// Code to execute when executing the migrations
    /// </summary>
    public override void Up()
    {
        Alter.Table("Core_Persons")
            .AddColumn("Mem_MembershipNumber").AsString().Nullable()
            .AddColumn("Mem_ResidentialAddress").AsString().Nullable()
            .AddForeignKeyColumn("Mem_RegionId", "Core_Areas").Nullable()
            .AddForeignKeyColumn("Mem_BranchId", "Core_Areas").Nullable()
            .AddColumn("Mem_MembershipStartDate").AsDateTime().Nullable()
            .AddColumn("Mem_MembershipEndDate").AsDateTime().Nullable();
    }
    /// <summary>
    /// Code to execute when rolling back the migration
    /// </summary>
    public override void Down()
    {
        throw new NotImplementedException();
    }
}