using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ECommerce.Entity;

namespace ECommerce.Entity.Store
{

    /// <summary>
    ///品牌报备
    /// </summary>
    [Serializable]
    [DataContract]
    public class StoreBrandFiling : EntityBase
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
        ///品牌编号
        /// </summary>
        [DataMember]
        public int? BrandSysNo
        {
            get;
            set;
        }

        /// <summary>
        ///商检号
        /// </summary>
        [DataMember]
        public string InspectionNo
        {
            get;
            set;
        }

        /// <summary>
        ///代理等级
        /// </summary>
        [DataMember]
        public string AgentLevel
        {
            get;
            set;
        }

        /// <summary>
        ///0:无效
        ///1:草稿
        ///2:审核通过
        /// </summary>
        [DataMember]
        public int? Staus
        {
            get;
            set;
        }




    }
}
