using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.IM;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 成本变价单基本信息
    /// </summary>
    public class CostChangeBasicInfo
    {
        /// <summary>
        /// 成本变价单编号
        /// </summary>
        public int SysNo { get; set; }
        /// <summary>
        /// 变价差异
        /// </summary>
        public decimal TotalDiffAmt { get; set; }
        /// <summary>
        /// 变价原因
        /// </summary>
        public string Memo { get; set; }
        /// <summary>
        /// 审核或作废原因
        /// </summary>
        public string AuditMemo { get; set; }
        /// <summary>
        /// 供应商编号
        /// </summary>
        public int VendorSysNo { get; set; }
        /// <summary>
        /// 产品经理编号
        /// </summary>
        public int PMSysNo { get; set; }
        /// <summary>
        /// 单据状态
        /// </summary>
        public CostChangeStatus Status { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        public int? AuditUser { get; set; }
        /// <summary>
        /// 审核人名称
        /// </summary>
        public string AuditUserStr { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditDate { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public int? InUser { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        public string InUserStr { get; set; }
        /// <summary>
        /// 生成日期
        /// </summary>
        public DateTime? Indate { get; set; }
        /// <summary>
        /// 编辑人
        /// </summary>
        public int? EditUser { get; set; }
        /// <summary>
        /// 编辑时间
        /// </summary>
        public DateTime? EditDate { get; set; }
        /// <summary>
        /// 公司编码
        /// </summary>
        public string CompanyCode { get; set; }

    }
}
