using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.ExternalSYS
{
    [Serializable]
    [DataContract]
    public class ERPItemInventoryInfo
    {
        /// <summary>
        /// ERP商品编码
        /// </summary>
        [DataMember]
        public int? ERPProductId { get; set; }

        /// <summary>
        /// 系统商品编码
        /// </summary>        
        [DataMember]
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 网站销售数量
        /// </summary>
        [DataMember]
        public int? B2CSalesQuantity { get; set; }

        /// <summary>
        /// 总部库存
        /// </summary>
        [DataMember]
        public int? HQQuantity { get; set; }

        /// <summary>
        /// 门店库存
        /// </summary>
        [DataMember]
        public int? DeptQuantity { get; set; }

        /// <summary>
        /// 单据类型
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// 单据编号
        /// </summary>
        public int? OrderSysNo { get; set; }

        /// <summary>
        /// 调仓备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 调仓返回信息
        /// </summary>
        public string ReturnMsg { get; set; }
    }
}
