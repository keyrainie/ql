using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.IM;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Inventory
{
    /// <summary>
    /// 借货单-借出商品信息
    /// </summary>    
    [DataContract]
    [Serializable]
    public class LendRequestItemInfo : IIdentity
    {
        public LendRequestItemInfo()
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
        /// 借出商品信息
        /// </summary>
        [DataMember]
        public ProductInfo LendProduct { get; set; }

        /// <summary>
        /// 借出数量
        /// </summary>
        [DataMember]
        public int LendQuantity { get; set; }

        /// <summary>
        /// 已归还数量
        /// </summary>
        [DataMember]
        public int? ReturnQuantity { get; set; }

        /// <summary>
        /// 本次归还数量
        /// </summary>
        [DataMember]
        public int? ToReturnQuantity { get; set; }

        /// <summary>
        /// 期望归还日期
        /// </summary>
        [DataMember]
        public DateTime? ExpectReturnDate { get; set; }

        /// <summary>
        /// 借出成本(借出商品出库时更新)
        /// </summary>
        [DataMember]
        public decimal LendUnitCost { get; set; }

        /// <summary>
        /// 借出去税成本(借出商品出库时更新)
        /// </summary>
        [DataMember]
        public decimal LendUnitCostWithoutTax { get; set; }

        /// <summary>
        /// 添加商品至借货单时的借出成本
        /// </summary>
        [DataMember]
        public decimal LendUnitCostWhenCreate { get; set; }

        /// <summary>
        /// 商品批次信息列表
        /// </summary>
        [DataMember]
        public List<InventoryBatchDetailsInfo> BatchDetailsInfoList { get; set; }
    }
}
