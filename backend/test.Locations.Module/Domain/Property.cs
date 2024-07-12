using Shesha.Domain.Attributes;
using Shesha.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Azure;
using Abp.Specifications;
using System.Linq.Expressions;
using Shesha.Specifications;
using Abp.Domain.Repositories;
using test.Locations.Module.Enums;

namespace test.Locations.Module.Domain
{
    // <summary>
    /// A person within the application that is a Member
    /// </summary>
    [Entity(TypeShortAlias = "Loc.Property")]
    public class Property : Site
    {
        public virtual RefListPropertyType? PropertyType { get; set; }
    }
}
