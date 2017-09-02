using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.MKT.Promotion
{
    /// <summary>
    /// 支付方式信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class PayTypeInfo
    {
        /// <summary>
        /// 支付方式系统编号
        /// </summary>
        [DataMember]
        public int PayTypeSysNo { get; set; }

        /// <summary>
        /// 支付方式名称
        /// </summary>
        [DataMember]
        public string PayTypeName { get; set; }

        /// <summary>
        /// 是否选中，默认选中
        /// </summary>
        public bool _isChecked = false;

        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; }
        }
    }
}
