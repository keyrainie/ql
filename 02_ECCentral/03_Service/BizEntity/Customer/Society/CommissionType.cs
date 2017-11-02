
using ECCentral.BizEntity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECCentral.BizEntity.Customer.Society
{
    [Serializable]
    [DataContract]
    public class CommissionType : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 返佣金方式编号
        /// </summary>
        [DataMember]
        public string CommissionTypeID { get; set; }

        /// <summary>
        /// 返佣金方式名称
        /// </summary>
        [DataMember]
        public string CommissionTypeName { get; set; }
        /// <summary>
        /// 返佣金方式描述
        /// </summary>
        [DataMember]
        public string CommissionTypeDesc { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public bool? CommissionStatus { get; set; }
        /// <summary>
        /// 状态 ： 正常，停用
        /// </summary>
        [DataMember]
        public SYNStatus? CommissionStatusNeum
        {
            get
            {
                return (CommissionStatus.HasValue && CommissionStatus.Value) ? SYNStatus.Yes : SYNStatus.No;
            }
            set
            {
                CommissionStatus = value.HasValue ? (value.Equals(SYNStatus.Yes) ? true : false) : (bool?)null;
            }
        }
        /// <summary>
        /// 返佣金下限
        /// </summary>
        [DataMember]
        public decimal? LowerLimit { get; set; }
        /// <summary>
        /// 返佣金上限
        /// </summary>
        [DataMember]
        public decimal? UpperLimit { get; set; }
        /// <summary>
        /// 返佣率
        /// </summary>
        [DataMember]
        public decimal? CommissionRate { get; set; }
        /// <summary>
        /// 显示顺序
        /// </summary>
        [DataMember]
        public decimal? CommissionOrder { get; set; }

        #region 扩展属性
        [DataMember]
        public string OrganizationID { get; set; }
        public string OrganizationName { get; set; }
       
        #endregion

    }
}
