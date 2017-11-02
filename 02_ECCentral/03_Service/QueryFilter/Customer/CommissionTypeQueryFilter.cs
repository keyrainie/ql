using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.QueryFilter.Common;
namespace ECCentral.QueryFilter.Customer
{
    public class CommissionTypeQueryFilter
    {

        public string SysNo { get; set; }

        public string CommissionTypeID { get; set; }

        public string CommissionTypeName { get; set; }

        public CurrencyStatus? IsOnlineShow { get; set; }

        public PagingInfo PagingInfo { get; set; }
        #region 扩展属性
        public string OrganizationID { get; set; }

        public string OrganizationName { get; set; }
        #endregion
    }
}
