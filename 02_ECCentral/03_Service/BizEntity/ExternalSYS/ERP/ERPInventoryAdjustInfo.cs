using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.ExternalSYS
{
    [Serializable]
    [DataContract]
    public class ERPInventoryAdjustInfo
    {
        /// <summary>
        /// 单据类型
        /// </summary>
        [DataMember]
        public string OrderType { get; set; }
        /// <summary>
        /// 单据编号
        /// </summary>
        [DataMember]
        public int? OrderSysNo { get; set; }
        /// <summary>
        /// 调仓商品
        /// </summary>
        [DataMember]
        public List<ERPItemInventoryInfo> AdjustItemList { get; set; }
        /// <summary>
        /// 调仓备注
        /// </summary>
        [DataMember]
        public string Memo { get; set; }

        /// <summary>
        /// 调仓返回信息
        /// </summary>
        public string ReturnMsg { get; set; }
    }
}
