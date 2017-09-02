using ECommerce.Entity.Common;
using ECommerce.Entity.ControlPannel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 商品销售区域信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductSalesAreaInfo
    {
        /// <summary>
        /// 仓库信息
        /// </summary>
        [DataMember]
        public StockInfo Stock { get; set; }

        /// <summary>
        /// 销售区域信息
        /// </summary>
        [DataMember]
        public AreaInfo Province { get; set; }

        /// <summary>
        /// 公司编码
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }

        /// <summary>
        /// 语言编码
        /// </summary>
        [DataMember]
        public string LanguageCode { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public UserInfo OperationUser { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }
    }
}
