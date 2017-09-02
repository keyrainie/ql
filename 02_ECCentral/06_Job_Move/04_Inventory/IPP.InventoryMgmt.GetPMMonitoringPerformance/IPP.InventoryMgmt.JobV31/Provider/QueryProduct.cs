using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPP.InventoryMgmt.Taobao.JobV31.Provider
{
    /// <summary>
    /// 商品筛选条件
    /// </summary>
    public class QueryProduct
    {
        /// <summary>
        /// 仓库号
        /// </summary>
        public int[] WareHourseNumber { get; set; }

        /// <summary>
        /// 第三方标识
        /// </summary>
        public string PartnerType { get; set; }

        /// <summary>
        /// 企业标识
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 商品SysNo
        /// </summary>
        public int[] ProductSysNo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string[] ProductID { get; set; }

        /// <summary>
        /// 第三方商品的ID
        /// </summary>
        public string[] SysProductID { get; set; }


    }
}
