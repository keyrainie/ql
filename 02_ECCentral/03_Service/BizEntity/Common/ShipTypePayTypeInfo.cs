using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Common
{
    /// <summary>
    /// 配送方式和支付方式 否定关系
    /// </summary>
    [Serializable]
    [DataContract]
    public class ShipTypePayTypeInfo : IIdentity,ICompany
    {
        /// <summary>
        /// 系统编号
        /// </summary>
         [DataMember]
        public int? SysNo { get; set; }

         /// <summary>
         /// 支付方式系统编号
         /// </summary>
         [DataMember]
        public int PayTypeSysNo { get; set; }

         /// <summary>
         /// 配送方式系统编号
         /// </summary>
         [DataMember]
        public int ShipTypeSysNo { get; set; }

        /// <summary>
        /// 支付方式名称
        /// </summary>
         [DataMember]
        public string PayTypeName { get; set; }

        /// <summary>
        /// 配送方式名称
        /// </summary>
         [DataMember]
        public string ShipTypeName { get; set; }

        [DataMember]
         public string CompanyCode { get; set; }
    }
}
