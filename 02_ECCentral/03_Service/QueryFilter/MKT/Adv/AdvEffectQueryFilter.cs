using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    /// <summary>
    /// 广告效果监视
    /// </summary>
    public class AdvEffectQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 销售订单编号
        /// </summary>
        public string SOID { get; set; }

        /// <summary>
        /// 客户SysNo
        /// </summary>
        public int? CustomerSysNo { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// 订单最大金额
        /// </summary>
        public decimal? MinSOAmt { get; set; }

        /// <summary>
        /// 订单最小金额
        /// </summary>
        public decimal? MaxSOAmt { get; set; }

        /// <summary>
        /// cm_mmc关键字
        /// </summary>
        public string CMP { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public ECCentral.BizEntity.SO.SOStatus? SOStatus { get; set; }

        /// <summary>
        /// 包含产生RO退款单
        /// </summary>
        public NYNStatus? IsRefund { get; set; }

        /// <summary>
        /// 订单金额等级
        /// </summary>
        public int? Grade { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? InDateFrom { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? InDateTo { get; set; }

        /// <summary>
        /// 监视动作类型
        /// </summary>
        public string OperationType { get; set; }

        /// <summary>
        /// 就否通过手机验证
        /// </summary>
        public NYNStatus? IsPhone { get; set; }

        /// <summary>
        /// 就否通过邮箱验证
        /// </summary>
        public NYNStatus? IsEmailConfirmed { get; set; }

        /// <summary>
        /// 就否有效订单
        /// </summary>
        public NYNStatus? IsValidSO { get; set; }

        /// <summary>
        /// CompanyCode
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public int? ChannelID { get; set; }
    }
}
