using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.Inventory
{
    /// <summary>
    /// 仓库信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class WarehouseInfo : IIdentity, ICompany
    {
        public WarehouseInfo()
        {
            OwnerInfo = new WarehouseOwnerInfo();
        }
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

        /// <summary>
        /// 仓库编号
        /// </summary>
        [DataMember]
        public string WarehouseID { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        [DataMember]
        public string WarehouseName { get; set; }
        
        /// <summary>
        /// 仓库地址
        /// </summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        [DataMember]
        public string Contact { get; set; }

        /// <summary>
        /// 联系人Email
        /// </summary>
        [DataMember]
        public string ContactEmail { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [DataMember]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 仓库所在地区
        /// </summary>
        [DataMember]
        public int WarehouseArea { get; set; }

        /// <summary>
        /// 收货地址
        /// </summary>
        [DataMember]
        public string ReceiveAddress { get; set; }

        /// <summary>
        /// 收货联系人
        /// </summary>
        [DataMember]
        public string ReceiveContact { get; set; }

        /// <summary>
        /// 收货联系电话
        /// </summary>
        [DataMember]
        public string ReceiveContactPhoneNumber { get; set; }

        /// <summary>
        /// 移仓分仓系数
        /// </summary>
        [DataMember]
        public decimal TransferRate { get; set; }

        /// <summary>
        /// 仓库类型
        /// </summary>
        [DataMember]
        public WarehouseType WarehouseType { get; set; }

        /// <summary>
        /// 仓库类型
        /// </summary>
        [DataMember]
        [XmlIgnore]
        public ValidStatus WarehouseStatus { get; set; }

        /// <summary>
        /// 仓库所有者
        /// </summary>
        [DataMember]
        public WarehouseOwnerInfo OwnerInfo { get; set; }

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
        /// 地区代码
        /// </summary>
        [DataMember]
        public String CountryCode { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        [DataMember]
        public String Zip { get; set; }

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

        /// <summary>
        /// 海关关区代码
        /// </summary>
        [DataMember]
        public CustomsCodeMode CustomsCode { get; set; }
    }
}
