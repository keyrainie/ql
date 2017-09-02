using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.RMA;

namespace ECCentral.QueryFilter.Customer
{
    public class RefundAdjustQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        public string SysNo { get; set; }
        /// <summary>
        /// 退款单号
        /// </summary>
        public int? RefundSysNo { get; set; }

        /// <summary>
        /// 申请单号
        /// </summary>
        public string RequestID { get; set; }

        /// <summary>
        /// 创建日期 开始
        /// </summary>
        public DateTime? CreateDateFrom { get; set; }

        /// <summary>
        /// 创建日期 结束
        /// </summary>
        public DateTime? CreateDateTo { get; set; }

        /// <summary>
        /// 完成日期 开始
        /// </summary>
        public DateTime? RefundDateFrom { get; set; }

        /// <summary>
        /// 完成日期 结束
        /// </summary>
        public DateTime? RefundDateTo { get; set; }

        /// <summary>
        /// 顾客ID
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string SoSysNo { get; set; }

        /// <summary>
        /// 退款方式
        /// </summary>
        public RefundPayType? RefundPayType { get; set; }

        /// <summary>
        /// 处理状态
        /// </summary>
        public RefundAdjustStatus? RefundAdjustStatus { get; set; }

        /// <summary>
        /// 补偿类型
        /// </summary>
        public RefundAdjustType? AdjustType { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 供应商编号
        /// </summary>
        public string VendorID { get; set; }

        /// <summary>
        /// 公司编码
        /// </summary>
        public string CompanyCode { get; set; }
    }
}
