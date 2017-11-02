using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Store.Vendor
{
    [Serializable]
    [DataContract]
    public class CertificationInfo
    {
        [DataMember]
        public int? SysNo { get; set; }

        [DataMember]
        public string Memo { get; set; }

        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public string Url { get; set; }

        /// <summary>
        /// 用户系统编号
        /// </summary>
        public int UserSysNo { get; set; }

        /// <summary>
        /// 登录名称
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string UserDisplayName { get; set; }

        /// <summary>
        /// 商家系统编号
        /// </summary>
        public int SellerSysNo { get; set; }

        /// <summary>
        /// 商家名称
        /// </summary>
        public string SellerName { get; set; }

        public string Status { get; set; }
    }

    [Serializable]
    [DataContract]
    public class AttachmentInfo 
    {
        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public string Url { get; set; }
    }
}
