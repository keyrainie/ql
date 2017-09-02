using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    ///店铺网页的Header信息。
    /// </summary>
    [Serializable]
    [DataContract]
    public class StorePageHeader
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
        ///
        /// </summary>
        [DataMember]
        public string HeaderContent
        {
            get;
            set;
        }

        /// <summary>
        ///创建者系统编号
        /// </summary>
        [DataMember]
        public int? InUserSysNo
        {
            get;
            set;
        }

        /// <summary>
        ///创建者显示名
        /// </summary>
        [DataMember]
        public string InUserName
        {
            get;
            set;
        }

        /// <summary>
        ///创建时间
        /// </summary>
        [DataMember]
        public DateTime? InDate
        {
            get;
            set;
        }

        /// <summary>
        ///最后修改人系统编号
        /// </summary>
        [DataMember]
        public int? EditUserSysNo
        {
            get;
            set;
        }

        /// <summary>
        ///最后修改人显示名
        /// </summary>
        [DataMember]
        public string EditUserName
        {
            get;
            set;
        }

        /// <summary>
        ///最后修改时间
        /// </summary>
        [DataMember]
        public DateTime? EditDate
        {
            get;
            set;
        }


    }
}