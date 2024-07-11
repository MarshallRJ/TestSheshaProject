using Abp.Domain.Entities;
using NHibernate.Mapping;
using Shesha.Domain.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace Boxfusion.Membership.Common.Domain.Domain
{
    [Table("vw_TP_FlattenedMembershipPayments")]
    [ImMutable]    // Indicates that the ORM should not attempt to update the database
    public class FlattenedMembershipPayment : Entity<Guid>
    {
        public virtual double Amount { get; set; }
        /// <summary>
        /// The date when the payment was made
        /// </summary>
        public virtual DateTime PaymentDate { get; set; }

        /// <summary>
        /// The membership number for the Member
        /// </summary>
        public virtual string MembershipNumber { get; set; }

        public virtual string FullName { get; set; }
        public virtual string ResidentialAddress { get; set; }

        public virtual DateTime CreationTime { get; set; }
    }
}