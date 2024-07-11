using Shesha.Domain.Attributes;
using Shesha.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Boxfusion.Membership.Common.Domain.Enums;
using Azure;
using Abp.Specifications;
using System.Linq.Expressions;
using Shesha.Specifications;
using Abp.Domain.Repositories;

namespace Boxfusion.Membership.Common.Domain.Domain
{

    [Entity(TypeShortAlias = "TP.CurrentAccount")]
    public class CurrentAccount : Account
    {
        [NotMapped]
        public bool IsCurrent
        {
            get
            {
                return true;
            }
        }
    }

        // <summary>
        /// A person within the application that is a Member
        /// </summary>
        [Entity(TypeShortAlias = "TP.Member")]
    public class Member : Person
    {
        /// <summary>
        /// The membership number for the Member
        /// </summary>
        public virtual string MembershipNumber { get; set; }
        /// <summary>
        /// The Members residential address
        /// </summary>
        public virtual string ResidentialAddress { get; set; }
        /// <summary>
        /// The region that the Member belongs to
        /// </summary>
        public virtual Area Region { get; set; }
        /// <summary>
        /// The branch that the Member belongs to
        /// </summary>
        public virtual Area Branch { get; set; }
        /// <summary>
        /// The date when the Members membership started
        /// </summary>
        public virtual DateTime MembershipStartDate { get; set; }
        /// <summary>
        /// The date when the Members membership ended
        /// </summary>
        public virtual DateTime MembershipEndDate { get; set; }
        /// <summary>
        /// Identification document for the Member
        /// </summary>
        [NotMapped]
        public virtual StoredFile IdDocument { get; set; }

        public virtual RefListMembershipStatuses? MembershipStatus { get; set; }

        /// <summary>
        /// Specifies the remaining capacity. Calculated column based on: Capacity less SUM of all Active appointments.
        /// </summary>
        [NotMapped]
        public bool IsCurrent
        {
            get
            {
                return MembershipEndDate > DateTime.Now;
            }
        }
    }

    public class IsMembershipCurrentSpecification : ShaSpecification<Member>
    {
        public override Expression<Func<Member, bool>> BuildExpression()
        {
            //return c => c.IsCurrent;
            return c=> c.MembershipEndDate > DateTime.Now;
        }
    }

    public class IsUserMemberSameOrganisationsUser : ShaSpecification<Member>
    {
        public override Expression<Func<Member, bool>> BuildExpression()
        {
            var personService = IocManager.Resolve<IRepository<Person,Guid>>();
            var currentPerson = personService.GetAll().FirstOrDefault(p => p.User != null && p.User.Id == AbpSession.UserId);

            

            return c => currentPerson.PrimaryOrganisation == c.PrimaryOrganisation;
        }
    }




}