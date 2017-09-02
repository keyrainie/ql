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
    /// 渠道仓库所有者信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class WarehouseOwnerInfo : IIdentity, ICompany
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

        /// <summary>
        /// 所有者编号
        /// </summary>
        [DataMember]
        public string OwnerID { get; set; }

        /// <summary>
        /// 所有者名称
        /// </summary>
        [DataMember]
        public string OwnerName { get; set; }

        /// <summary>
        /// 所有者类型
        /// </summary>
        [DataMember]
        public WarehouseOwnerType OwnerType { get; set; }

        /// <summary>
        /// 所有者状态
        /// </summary>
        [DataMember]
        [XmlIgnore]
        public ValidStatus OwnerStatus { get; set; }

        /// <summary>
        /// 所有者备注
        /// </summary>
        [DataMember]
        public string OwnerMemo { get; set; }

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

    }
}
