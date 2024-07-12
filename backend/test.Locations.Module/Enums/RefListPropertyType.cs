using Shesha.Domain.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Locations.Module.Enums
{
    /// <summary>
    /// Statuses for a Members Membership
    /// </summary>
    [ReferenceList("Loc", "PropertyTypes")]
    public enum RefListPropertyType : long
    {
       
        [Description("Residential")]
        Residential = 1,
        /// <summary>
        /// Membership status is active
        /// </summary>
        [Description("Commercial")]
        Commercial = 2,
        /// <summary>
        /// Membership status is cancelled
        /// </summary>
        [Description("Industrial")]
        Industrial = 3
    }
}
