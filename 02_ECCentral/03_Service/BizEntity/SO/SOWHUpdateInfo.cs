using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.SO
{
    /// <summary>
    /// 手动更改订单仓库信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class SOWHUpdateInfo : IIdentity
    {
        /// <summary>
        /// 信息编号
        /// </summary>
        [DataMember]
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMember]
        public int? SystemNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public string ProductId
        {
            get;
            set;
        }

        /// <summary>
        ///　商品系统编号
        /// </summary>
        [DataMember]
        public int? ProductSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string ProductName
        {
            get;
            set;
        }

        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMember]
        public int? SOSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 数量
        /// </summary>
        [DataMember]
        public int? Quantity
        {
            get;
            set;
        }

        /// <summary>
        /// 原仓库名称
        /// </summary>
        [DataMember]
        public string StockName
        {
            get;
            set;
        }

        /// <summary>
        /// 原仓库号
        /// </summary>
        [DataMember]
        public int? StockSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 目标仓库编号
        /// </summary>
        [DataMember]
        public int? TargetStockSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 公司编号
        /// </summary>
        [DataMember]
        public string CompanyCode
        {
            get;
            set;
        }
    }
}
