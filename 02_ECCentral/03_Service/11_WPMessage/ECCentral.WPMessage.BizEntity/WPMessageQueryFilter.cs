using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ECCentral.BizEntity;
using System.ComponentModel;
using ECCentral.WPMessage.BizEntity;

namespace ECCentral.WPMessage.QueryFilter
{
    public class WPMessageQueryFilter
    {
        public int UserSysNo
        {
            get;
            set;
        }

        public int? CategorySysNo
        {
            get;
            set;
        }
        public DateTime? BeginCreateTime
        {
            get;
            set;
        }
        public DateTime? EndCreateTime
        {
            get;
            set;
        }

        public WPMessageStatus? WPMessageStatus
        {
            get;
            set;
        }

        public int PageIndex
        {
            get;
            set;
        }
        public int PageSize
        {
            get;
            set;
        }

        public string SortField
        {
            get;
            set;
        }
    }
}
