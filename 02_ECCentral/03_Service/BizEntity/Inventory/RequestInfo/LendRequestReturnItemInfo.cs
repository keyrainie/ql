using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.IM;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Inventory
{
    /// <summary>
    /// 借货单-借出商品的归还信息
    /// </summary>    
    [DataContract]
    [Serializable]
    public class LendRequestReturnItemInfo : IIdentity
    {
        public LendRequestReturnItemInfo()
        {
            this.BatchDetailsInfoList = new List<InventoryBatchDetailsInfo>();
        }

        [DataMember]
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 归还商品信息
        /// </summary>
        [DataMember]
        public ProductInfo ReturnProduct { get; set; }

        /// <summary>
        /// 归还数量
        /// </summary>
        [DataMember]
        public int ReturnQuantity { get; set; }

        /// <summary>
        /// 归还日期
        /// </summary>
        [DataMember]
        public DateTime ReturnDate { get; set; }

        /// <summary>
        /// 商品批次信息列表
        /// </summary>
        [DataMember]
        public List<InventoryBatchDetailsInfo> BatchDetailsInfoList { get; set; }

    }
}
