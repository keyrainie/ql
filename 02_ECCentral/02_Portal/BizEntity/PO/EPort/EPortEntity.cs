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

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 电子口岸信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class EPortEntity 
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int SysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 名称
        /// </summary>
        [DataMember]
        public string ePortName
        {
            get;
            set;
        }
        /// <summary>
        /// 免税限额
        /// </summary>
        [DataMember]
        public int TaxFreeLimit
        {
            get;
            set;
        }
        /// <summary>
        /// 发货方式
        /// </summary>
        [DataMember]
        public EPortShippingTypeENUM ShippingType
        {
            get;
            set;
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        [DataMember]
        public string PayType
        {
            get;
            set;
        }
        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public EPortStatusENUM Status
        {
            get;
            set;
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public DateTime Indate
        {
            get;
            set;
        }
        /// <summary>
        /// 创建人SysNo
        /// </summary>
        [DataMember]
        public int? InUser
        {
            get;
            set;
        }
        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public string CreateUser
        {
            get;
            set;
        }
        /// <summary>
        /// 最后的编辑时间
        /// </summary>
        [DataMember]
        public DateTime LastEditdate
        {
            get;
            set;
        }

        /// <summary>
        /// 最后的编辑时间
        /// </summary>
        [DataMember]
        public int? LastEditUser
        {
            get;
            set;
        }


        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Memo
        {
            get;
            set;
        }
    }
}
