using System;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.QueryFilter.Invoice
{
    public class OldChangeNewQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 创建时间 开始
        /// </summary>
        public DateTime? CreateDateFrom { get; set; }

        /// <summary>
        /// 创建时间 结束
        /// </summary>
        public DateTime? CreateDateTo { get; set; }

        /// <summary>
        /// 完成时间 开始
        /// </summary>
        public DateTime? CompleteDateFrom { get; set; }

        /// <summary>
        /// 创建时间  结束
        /// </summary>
        public DateTime? CompleteDateTo { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public int? OrderNo { get; set; }

        /// <summary>
        /// 订单编号列表
        /// </summary>
        public string OrderNoList { get; set; }

        /// <summary>
        /// 申请ID
        /// </summary>
        public string ApplyID { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public string CustomerNo { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        ///状态 
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 商品类别
        /// </summary>
        public string ProductType { get; set; }

        /// <summary>
        /// 凭证号
        /// </summary>
        public string CertificateNo { get; set; }

        /// <summary>
        /// 退款凭证
        /// </summary>
        public string RefundCertificate { get; set; }

        public int? SysNo { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }
    }
}
