using FluentMigrator;
using Shesha.FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//M20240711144400

namespace test.TestProject.Domain.Migrations
{
    [Migration(20240711144400)]
    public class M20240711144400 : Migration
    {
        /// <summary>
        /// 
        /// </summary>
        public override void Up()
        {
            Execute.Sql(@"
		CREATE OR ALTER VIEW [dbo].[vw_TP_FlattenedMembershipPayments]
			AS
				SELECT 
					 Core_Persons.TP_MembershipNumber MembershipNumber
                    , Core_Persons.FullName
                    , Core_Persons.TP_ResidentialAddress ResidentialAddress
                    , TP_MembershipPayments.Id
                    , TP_MembershipPayments.MemberId
                    , TP_MembershipPayments.PaymentDate
                    , TP_MembershipPayments.Amount
                    , TP_MembershipPayments.CreationTime
				FROM 
                  TP_MembershipPayments 
                INNER JOIN
                  Core_Persons ON TP_MembershipPayments.MemberId = Core_Persons.Id
                GO
            ");
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Down()
        {
            throw new NotImplementedException();
        }

    }


}
