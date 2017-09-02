using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Store
{
    /// <summary>
    ///佣金规则
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommissionRule
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
        ///
        /// </summary>
        [DataMember]
        public decimal? OrderCommissionFee
        {
            get;
            set;
        }

        /// <summary>
        ///佣金规则
        /// </summary>
        [DataMember]
        public string SalesRule
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [DataMember]
        public decimal? DeliveryFee
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [DataMember]
        public decimal? RentFee
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [DataMember]
        public string Status
        {
            get;
            set;
        }

        /// <summary>
        ///品牌SysNo
        /// </summary>
        [DataMember]
        public int? BrandSysNo
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [DataMember]
        public int? C1SysNo
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [DataMember]
        public int? C2SysNo
        {
            get;
            set;
        }

        /// <summary>
        ///三级分类
        /// </summary>
        [DataMember]
        public int? C3SysNo
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
