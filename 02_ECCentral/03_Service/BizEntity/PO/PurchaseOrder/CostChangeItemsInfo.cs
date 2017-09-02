using ECCentral.BizEntity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 成本变价单明细信息
    /// </summary>
    public class CostChangeItemsInfo : ICompany
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? ItemSysNo { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductSysNo { get; set; }
        /// <summary>
        /// 原采购单编号
        /// </summary>
        public int POSysNo { get; set; }
        /// <summary>
        /// 旧成本价
        /// </summary>
        public decimal OldPrice { get; set; }
        /// <summary>
        /// 新成本价
        /// </summary>
        public decimal NewPrice { get; set; }
        /// <summary>
        /// 变价数量
        /// </summary>
        public int ChangeCount { get; set; }
        /// <summary>
        /// 公司编码
        /// </summary>
        public string CompanyCode { get; set; }
        /// <summary>
        /// 公司编码
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 公司编码
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 结存数量（未锁定）
        /// </summary>
        public int AvaliableQty { get; set; }
        /// <summary>
        /// 明细状态
        /// </summary>
        public ItemActionStatus ItemActionStatus { get; set; }
        
    }
}
