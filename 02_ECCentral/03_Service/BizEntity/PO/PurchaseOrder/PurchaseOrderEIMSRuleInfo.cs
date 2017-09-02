using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 采购单EIMS合同信息
    /// </summary>
    public class PurchaseOrderEIMSRuleInfo
    {
        #region [合同基本信息]

        public int? RuleNumber { get; set; }

        /// <summary>
        /// 合同编号
        /// </summary>
        public string AssignedCode { get; set; }

        /// <summary>
        /// 供应商编号
        /// </summary>
        public string VendorNumber { get; set; }


        /// <summary>
        /// 供应商编号
        /// </summary>
        public string VendorName { get; set; }
        /// <summary>
        /// PM
        /// </summary>
        public string PM { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string StockName { get; set; }

        /// <summary>
        /// 合同名称
        /// </summary>
        public string RuleName { get; set; }

        /// <summary>
        /// 合同描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 归入部门编号
        /// </summary>
        public string DepartmentNumber { get; set; }


        /// <summary>
        /// 费用类型
        /// </summary>
        public string EIMSType { get; set; }
        /// <summary>
        /// 费用类型编号
        /// </summary>
        public int? EIMSTypeNumber { get; set; }

        /// <summary>
        /// 费用类型
        /// </summary>
        public PurchaseOrderEIMSRuleType? enumEIMSType { get; set; }
        /// <summary>
        /// 结算类型
        /// </summary>
        public string ReceiveType { get; set; }

        /// <summary>
        /// 公司编码
        /// </summary>
        public string CompanyCode { get; set; }
        /// <summary>
        /// 结算周期
        /// </summary>
        public string BillingCycle { get; set; }

        #endregion

        public EIMSRuleRebateScheme RebateScheme { get; set; }

        public List<EIMSRuleRebateSchemeTransaction> RebateSchemeTransactions { get; set; }

    }

    /// <summary>
    /// 采购单EIMS合同返利信息
    /// </summary>
    public class EIMSRuleRebateScheme
    {
        /// <summary>
        /// 合同系统
        /// </summary>
        public int? TransactionNumber { get; set; }


        /// <summary>
        /// 固定金额
        /// </summary>
        public decimal? RebateAmount { get; set; }

        /// <summary>
        /// 返点比例
        /// </summary>
        public decimal? RebatePercentage { get; set; }

        /// <summary>
        /// 基于类型
        /// </summary>
        public string RebateBaseType { get; set; }

        /// <summary>
        /// 基于规则
        /// </summary>
        public string RebateSchemeType { get; set; }

        /// <summary>
        /// 合同开始时间
        /// </summary>
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// 合同开始时间
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
    /// <summary>
    /// 采购单EIMS合同明细信息
    /// </summary>
    public class EIMSRuleRebateSchemeTransaction
    {
        /// <summary>
        /// 返利比例
        /// </summary>
        public decimal? Percent { get; set; }


        /// <summary>
        /// 返利数量
        /// </summary>
        public decimal? RebatePerUnit { get; set; }


        /// <summary>
        /// 返利数量（或金额）下限
        /// </summary>
        public decimal? LowerLimitValue { get; set; }

        /// <summary>
        /// 返利数量（或金额）上限
        /// </summary>
        public decimal? UpperLimitValue { get; set; }

        /// <summary>
        /// 基于类型
        /// </summary>
        public string RebateBaseType { get; set; }
    }
}
