using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newegg.Oversea.Framework.Entity;

namespace ECCentral.Job.Inventory.ProductRing.Model
{
    public class ProductBatchInfo
    {
        /// <summary>
        /// 商品系统编号
        /// </summary>
        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMapping("ProductID", DbType.String)]
        public string ProductID { get; set; }

        /// <summary>
        /// 欧亚批号
        /// </summary>
        [DataMapping("BatchNumber", DbType.String)]
        public string BatchNumber { get; set; }

        /// <summary>
        /// 最终批次状态
        /// </summary>
        [DataMapping("NewStatus", DbType.String)]
        public string NewStatus { get; set; }

        /// <summary>
        ///公司编码
        /// </summary>
        [DataMapping("CompanyCode", DbType.String)]
        public string CompanyCode { get; set; }
    }
}
