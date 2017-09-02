using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.QueryFilter.Inventory
{
    public class WarehouseOwnerQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public int? SysNo
        {
            get;
            set;
        }

        public string OwnerID
        {
            get;
            set;
        }

        public string OwnerName
        {
            get;
            set;
        }

        public ValidStatus? OwnerStatus
        {
            get;
            set;
        }

        public WarehouseOwnerType? OwnerType
        {
            get;
            set;
        }

        public string CompanyCode
        {
            get;
            set;
        }
    }
}
