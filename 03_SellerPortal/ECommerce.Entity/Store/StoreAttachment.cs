using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ECommerce.Entity;

namespace ECommerce.Entity.Store
{
    /// <summary>
    ///企业(商家)附件
    /// </summary>
    [Serializable]
    [DataContract]
    public class StoreAttachment:EntityBase
    {

        /// <summary>
        ///系统编号
        /// </summary>
        [DataMember]
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        ///商家(卖家)系统编号
        /// </summary>
        [DataMember]
        public int? SellerSysNo
        {
            get;
            set;
        }

        /// <summary>
        ///附件地址
        /// </summary>
        [DataMember]
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        ///附件名称
        /// </summary>
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        ///1:可用,0:不可用
        /// </summary>
        [DataMember]
        public int? Status
        {
            get;
            set;
        }

        /// <summary>
        ///备注
        /// </summary>
        [DataMember]
        public string Remark
        {
            get;
            set;
        }
    }
}
