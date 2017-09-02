using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Common
{
    /// <summary>
    /// 地区信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class AreaInfo : IIdentity, ICompany
    {
        /// <summary>
        /// 省级编号
        /// </summary>
        [DataMember]
        public int? ProvinceSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 市级编号
        /// </summary>
        [DataMember]
        public int? CitySysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 县级系统编号
        /// </summary>
        [DataMember]
        public int? DistrictSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 省份名称
        /// </summary>
        [DataMember]
        public string ProvinceName
        {
            get;
            set;
        }

        /// <summary>
        /// 市名称
        /// </summary>
        [DataMember]
        public string CityName
        {
            get;
            set;
        }

        /// <summary>
        /// 县（地区）名称
        /// </summary>
        [DataMember]
        public string DistrictName
        {
            get;
            set;
        }
        /// <summary>
        /// 显示的区域名称
        /// </summary>
        [DataMember]
        public string AreaName
        {
            get;
            set;
        }
        /// <summary>
        /// 显示的全名
        /// </summary>
        [DataMember]
        public string FullName
        {
            get;
            set;
        }
        /// <summary>
        /// 显示优先级
        /// </summary>
        [DataMember]
        public string OrderNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 是否本地
        /// </summary>
        [DataMember]
        public int? IsLocal
        {
            get;
            set;
        }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public int? Status
        {
            get;
            set;
        }

        /// <summary>
        /// 城市等级
        /// </summary>
        [DataMember]
        public string Rank
        {
            get;
            set;
        }

        /// <summary>
        /// 免运费的最小订单金额
        /// </summary>
        [DataMember]
        public decimal? SOAmountLimit
        {
            get;
            set;
        }

        /// <summary>
        /// 免运费的最大订单重量（整单）
        /// </summary>
        [DataMember]
        public int? WeightLimit
        {
            get;
            set;
        }

        #region IIdentity Members

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo
        {
            get;
            set;
        }

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
    }
}