using ECCentral.BizEntity.Common;
using System;
using System.Net;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ECCentral.BizEntity.Customer.Society
{
    [Serializable]
    [DataContract]
    public class CommissionType : IIdentity
    {
        [DataMember]
        public int? SysNo { get; set; }

        [DataMember]
        public string OrganizationName { get; set; }

        [DataMember]
        public int? OrganizationID { get; set; }
        /////
        [DataMember]
        public string CommissionTypeID { get; set; }

        [DataMember]
        public string CommissionTypeName { get; set; }
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

        [DataMember]
        public decimal? LowerLimit { get; set; }
        [DataMember]
        public decimal? UpperLimit { get; set; }
        [DataMember]
        public decimal? CommissionRate { get; set; }

    }
}
