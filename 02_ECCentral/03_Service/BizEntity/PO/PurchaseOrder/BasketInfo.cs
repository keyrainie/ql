using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 采购篮
    /// </summary>
    public class BasketInfo : IIdentity, ICompany
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 采购篮商品
        /// </summary>
        public List<BasketItemsInfo> Items { get; set; }
    }

}
