using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.QueryFilter.Inventory
{
    public class WarehouseQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        public string SysNo { get; set; }

        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }
        /// <summary>
        /// 仓库所有者编号
        /// </summary>
        public int? OwnerSysNo { get; set; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        public string WarehouseID { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WarehouseName { get; set; }

        /// <summary>
        /// 仓库所在地区
        /// </summary>
        public int? WarehouseArea { get; set; }

        /// <summary>
        /// 仓库类型
        /// </summary>
        public WarehouseType? WarehouseType { get; set; }

        /// <summary>
        /// 仓库类型
        /// </summary>
        public ValidStatus? WarehouseStatus { get; set; }

    }
}
