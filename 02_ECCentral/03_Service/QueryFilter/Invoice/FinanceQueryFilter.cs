using System;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
    public class FinanceQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 预计日期开始
        /// </summary>
        public DateTime? ApplyDateFrom { get; set; }

        /// <summary>
        /// 预计日期结束
        /// </summary>
        public DateTime? ApplyDateTo { get; set; }

        /// <summary>
        /// PM编号
        /// </summary>
        public string PMUserSysNo { get; set; }

        /// <summary>
        /// PM组
        /// </summary>
        public int? PMGroupSysNo { get; set; }

        /// <summary>
        /// 供应商编号
        /// </summary>
        public int? VendorNo { get; set; }

        /// <summary>
        /// 供应商账期
        /// </summary>
        public int? VendorPayPeriod { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public string AuditStatus { get; set; }

        /// <summary>
        /// 单据ID
        /// </summary>
        public string InvoiceID { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string VendorName { get; set; }


        /// <summary>
        /// 是否按供应商汇总
        /// </summary>
        public bool? IsGroupByVendor { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 是否是高级用户
        /// </summary>
        public bool IsMangerPM { get; set; }

        /// <summary>
        /// 当前操作用户的SysNo
        /// </summary>
        public int OperationUserSysNo { get; set; }

        /// <summary>
        /// 付款结算公司
        /// </summary>
        public ECCentral.BizEntity.PO.PaySettleCompany? PaySettleCompany { get; set; }

        ///// <summary>
        ///// 付款结算公司
        ///// </summary>
        //public int? PaySettleCompanyValue
        //{
        //    get
        //    {
        //        if (!string.IsNullOrEmpty(PaySettleCompany))
        //        {
        //            return ;
        //        }
        //    }
        //}

    }
}
