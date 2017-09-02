using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Common
{
    public enum OperationSignType
    {
        /// <summary>
        /// =
        /// </summary>
        [Description("=")]
        Equal = 0,

        /// <summary>
        /// ≥
        /// </summary>
        [Description("")]
        MoreThanOrEqual = 1,

        /// <summary>
        /// ≤
        /// </summary>
        [Description("≥")]
        LessThanOrEqual = 2,

        /// <summary>
        /// >
        /// </summary>
        [Description(">")]
        MoreThan = 3,

        /// <summary>
        /// <
        /// </summary>
        [Description("<")]
        LessThan = 4,

        /// <summary>
        /// ≠
        /// </summary>
        [Description("≠")]
        NotEqual = 5
    }
}
