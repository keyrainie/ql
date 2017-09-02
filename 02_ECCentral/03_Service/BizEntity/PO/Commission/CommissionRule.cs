using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 佣金规则信息
    /// </summary>
    public class CommissionRule : IIdentity, ICompany
    {

        /// <summary>
        /// 佣金规则系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode { get; set; }
        /// <summary>
        /// 品牌系统编号
        /// </summary>
        public int BrandSysNo { get; set; }

        /// <summary>
        /// 生厂商系统编号
        /// </summary>
        public int ManufacturerSysNo { get; set; }

        /// <summary>
        /// 成本
        /// </summary>
        public decimal CostValue { get; set; }

        /// <summary>
        /// 规则类型  C：仓储费规则 ，P：配送费规则
        /// </summary>

        public string RuleType { get; set; }

        /// <summary>
        /// 是否为全局规则
        /// </summary>
        public string IsDefaultRule { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public string InUser { get; set; }

        /// <summary>
        /// 最后一次编辑用户
        /// </summary>
        public string EditUser { get; set; }

        /// <summary>
        /// 货币代码
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// 生产商名称
        /// </summary>
        public string ManufacturerName { get; set; }

        /// <summary>
        /// 品牌名称（中文）
        /// </summary>
        public string BrandNameCH { get; set; }

        /// <summary>
        /// 品牌名称（英文）
        /// </summary>
        public string BrandNameEN { get; set; }

        /// <summary>
        /// C1级别编号
        /// </summary>
        public int? C1SysNo { get; set; }

        /// <summary>
        /// C1级别名称
        /// </summary>
        public string C1Name { get; set; }

        /// <summary>
        /// C2级别编号
        /// </summary>
        public int? C2SysNo { get; set; }

        /// <summary>
        /// C2级别名称
        /// </summary>
        public string C2Name { get; set; }

        /// <summary>
        /// C3级别编号
        /// </summary>
        public int? C3SysNo { get; set; }

        /// <summary>
        /// C3级别名称
        /// </summary>
        public string C3Name { get; set; }

        /// <summary>
        /// 规则级别（详细程度）
        /// </summary>
        public string Level { get; set; }

    }
}
