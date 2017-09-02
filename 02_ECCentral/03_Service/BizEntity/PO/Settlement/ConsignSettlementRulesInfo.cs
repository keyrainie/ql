using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 代销商品规则信息
    /// </summary>
    public class ConsignSettlementRulesInfo : ICompany
    {

        public int? RuleSysNo { get; set; }
        /// <summary>
        /// 规则代码
        /// </summary>
        public string SettleRulesCode { get; set; }

        /// <summary>
        ///  规则名称
        /// </summary>
        public string SettleRulesName { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 供应商系统编号
        /// </summary>
        public int? VendorSysNo { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// 剩余结算数量
        /// </summary>
        public int? RemainQty { get; set; }

        /// <summary>
        /// 原始结算价格
        /// </summary>
        public decimal? OldSettlePrice { get; set; }

        /// <summary>
        /// 现在结算价格
        /// </summary>
        public decimal? NewSettlePrice { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 结算数量
        /// </summary>
        public int? SettleRulesQuantity { get; set; }

        /// <summary>
        /// 已结算数量
        /// </summary>
        public int? SettledQuantity { get; set; }

        /// <summary>
        /// 货币编号
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// 规则状态
        /// </summary>
        public ConsignSettleRuleStatus? Status { get; set; }

        /// <summary>
        /// 结算状态
        /// </summary>
        public char? StatusString { get; set; }

        /// <summary>
        /// 更新人系统编号
        /// </summary>
        public int? EditUserSysNo { get; set; }

        /// <summary>
        /// 更新人名称
        /// </summary>
        public string EditUser { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }
        /// <summary>
        /// 记录本次结算单提交的结算数量
        /// </summary>
        public int? SubmitSettleQuantity { get; set; }
        /// <summary>
        /// 是否需要更新状态，取消结算业务中，用于存储临时状态，不保存数据库 2012-10-29 by Jack
        /// </summary>
        public bool IsNeedUpdateStatus { get; set; }


        /// <summary>
        /// 是否剩余数量超过了设定值
        /// </summary>
        public bool IsOverQuantity
        {
            get
            {
                bool result = false;
                if (SettleRulesQuantity.HasValue)
                {
                    result = SettleRulesQuantity.Value - (SettledQuantity ?? 0) - (SubmitSettleQuantity ?? 0) < 0;
                }
                return result;
            }
        }
    }
}
