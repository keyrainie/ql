using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace ECCentral.BizEntity.Common
{
    public class CommissionType 
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
        public bool? IsNet { get; set; }
        /// <summary>
        /// 状态 ： 正常，停用
        /// </summary>
        [DataMember]
        public SYNStatus? CommissionStatus
        {
            get
            {
                return (IsNet.HasValue && IsNet.Value) ? SYNStatus.Yes : SYNStatus.No;
            }
            set
            {
                IsNet = value.HasValue ? (value.Equals(SYNStatus.Yes) ? true : false) : (bool?)null;
            }
        }
        /// <summary>
        /// 返佣金下限
        /// </summary>
        [DataMember]
        public decimal? Lower { get; set; }
        /// <summary>
        /// 返佣金上限
        /// </summary>
        [DataMember]
        public decimal? Upper { get; set; }
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
    }
}
