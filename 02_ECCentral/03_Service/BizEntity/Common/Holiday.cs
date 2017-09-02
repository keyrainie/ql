using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Common
{
    /// <summary>
    /// 节假日
    /// </summary>
    [Serializable]
    [DataContract]
    public class Holiday : IIdentity, ICompany
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 创建人编码
        /// </summary>
        [DataMember]
        public int? CreateUserSysNo { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [DataMember]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 创建人姓名
        /// </summary>
        [DataMember]
        public string CreateUserName { get; set; }

        /// <summary>
        /// 配送类型名称
        /// </summary>
        [DataMember]
        public string ShipTypeName { get; set; }

        /// <summary>
        /// 节假日日期
        /// </summary>
        [DataMember]
        public DateTime HolidayDate { get; set; }

        /// <summary>
        /// 服务类型
        /// </summary>
        [DataMember]
        public BlockedServiceType? BlockedService { get; set; }

        /// <summary>
        /// 服务类型编码
        /// </summary>
        [DataMember]
        public int? ShipTypeSysNo { get; set; }

        public int DeliveryType
        {
            get
            {
                if (BlockedService == BlockedServiceType.TwoTimeToOneTime)
                    return 1;
                else
                    return 0;
            }
        }

        /// <summary>
        /// 公司编码
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }
    }
}
