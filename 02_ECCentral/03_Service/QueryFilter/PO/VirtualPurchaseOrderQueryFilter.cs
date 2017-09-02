using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.PO
{
    public class VirtualPurchaseOrderQueryFilter
    {
        public VirtualPurchaseOrderQueryFilter()
        {
            PageInfo = new PagingInfo();
        }

        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 创建时间 - 从
        /// </summary>
        public DateTime? CreateDateFrom { get; set; }
        /// <summary>
        /// 创建时间 - 到
        /// </summary>
        public DateTime? CreateDateTo { get; set; }

        /// <summary>
        ///  状态
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 虚库申请单代码
        /// </summary>
        public string VSPOSysNo { get; set; }

        /// <summary>
        ///  销售单代码
        /// </summary>
        public string SOSysNo { get; set; }

        /// <summary>
        /// 产品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 销售单状态
        /// </summary>
        public int? SOStatus { get; set; }

        /// <summary>
        /// 当前归属PM
        /// </summary>
        public int? PMLeaderSysNo { get; set; }

        /// <summary>
        /// PM处理人员
        /// </summary>
        public int? PMExecSysNo { get; set; }

        /// <summary>
        /// 采购单状态
        /// </summary>
        public int? POStatus { get; set; }

        /// <summary>
        /// 支付方式编号
        /// </summary>
        public int? PayTypeSysNo { get; set; }

        /// <summary>
        /// 移仓单编号
        /// </summary>
        public int? ShiftStatus { get; set; }

        /// <summary>
        /// 商品入库状态
        /// </summary>
        public int? InStockStatus { get; set; }

        /// <summary>
        /// 转换单状态
        /// </summary>
        public int? TransferStatus { get; set; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        public int? StockSysNo { get; set; }

        /// <summary>
        /// 单据类型
        /// </summary>
        public int? InStockOrderType { get; set; }

        /// <summary>
        /// 超时工作日
        /// </summary>
        public int? DelayDays { get; set; }

        /// <summary>
        /// 预计超时工作日
        /// </summary>
        public int? EstimateDelayDays { get; set; }

        /// <summary>
        /// 1级类别编号
        /// </summary>
        public int? C1SysNo { get; set; }

        /// <summary>
        /// 2级类别编号
        /// </summary>
        public int? C2SysNo { get; set; }

        /// <summary>
        /// 3级类别编号
        /// </summary>
        public int? C3SysNo { get; set; }

        /// <summary>
        /// 是否包含历史记录
        /// </summary>
        public bool? IsHasHistory { get; set; }

        public string CompanyCode { get; set; }
    }
}
