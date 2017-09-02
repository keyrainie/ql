using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ECCentral.BizEntity.Common;
using System.Xml.Serialization;

namespace ECCentral.BizEntity.Inventory
{
    /// <summary>
    /// 渠道仓库信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class StockInfo : IIdentity, ICompany, IWebChannel
    {
        #region IIdentity Members

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        #endregion IIdentity Members

        #region ICompany Members

        /// <summary>
        /// 所属公司
        /// </summary>
        [DataMember]
        public string CompanyCode
        {
            get;
            set;
        }

        #endregion ICompany Members

        #region IWebChannel Members
        /// <summary>
        /// 所属渠道
        /// </summary>
        [DataMember]
        public WebChannel WebChannel
        {
            get;
            set;
        }
        #endregion IWebChannel Members  

        /// <summary>
        /// 渠道仓库编号
        /// </summary>
        [DataMember]
        public string StockID
        {
            get;
            set;
        }

        /// <summary>
        /// 渠道仓库名称
        /// </summary>
        [DataMember]
        public string StockName
        {
            get;
            set;
        }

        /// <summary>
        /// 渠道仓库状态
        /// </summary>
        [DataMember]
        [XmlIgnore]
        public ValidStatus StockStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 渠道仓库所属仓库
        /// </summary>
        [DataMember]
        public WarehouseInfo WarehouseInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public UserInfo CreateUser { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [DataMember]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        [DataMember]
        public UserInfo EditUser { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        [DataMember]
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 发件省份
        /// </summary>
        [DataMember]
        public string Province { get; set; }
        /// <summary>
        /// 发件城市
        /// </summary>
        [DataMember]
        public string City { get; set; }

        /// <summary>
        /// 发件公司名称
        /// </summary>
        [DataMember]
        public string CompanyName { get; set; }

        /// <summary>
        /// 仓库类型（自贸仓库和海外仓库）
        /// </summary>
        [DataMember]
        public TradeType StockType { get; set; }

        [DataMember]
        public int MerchantSysNo { get; set; }
    }
}