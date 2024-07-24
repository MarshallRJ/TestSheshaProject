using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Repositories;
using Shesha.Domain.Attributes;
using Shesha.Specifications;
using System;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Abp.Auditing;
using FluentValidation;
namespace Boxfusion.Membership.Common.Domain.Domain
{

    public class CreateVehcileValidator : AbstractValidator<Vehicle>
    {
        public CreateVehcileValidator()
        {
            RuleFor(x => x.RegistrationNumber).NotNull().Length(3, 12);
            RuleFor(x => x.Make).NotNull().Length(2, 30);
            RuleFor(x => x.Model).NotNull().Length(2, 30);
            RuleFor(x => x.Year).ExclusiveBetween(1900, 2100);
        }
    }

    [Entity(TypeShortAlias = "TP.Vehicle")]
    public class Vehicle : FullAuditedEntity<Guid>
    {
        public virtual string Make { get; set; }
        public virtual string Model { get; set; }

        public virtual string RegistrationNumber { get; set; }

        public virtual int Year { get; set; }
    }

    
}