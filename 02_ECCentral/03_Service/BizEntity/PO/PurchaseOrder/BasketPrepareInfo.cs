using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 预备采购篮信息(备货中心用)
    /// </summary>
    public class BasketPrepareInfo : IIdentity, ICompany
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }
        /// <summary>
        /// 采购篮商品列表
        /// </summary>
        public List<BasketItemsPrepareInfo> Items { get; set; }
    }
}
