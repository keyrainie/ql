using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace ECommerce.Entity.Order
{
    public class PaymentInfo
    {
        #region [ Fields ]

        private int payTypeID;
        private string payTypeName;
        private int isPayWhenRecv;
        private int isNet;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// 支付方式
        /// </summary>
        [DataMember]
        public int PayTypeID
        {
            get { return this.payTypeID; }
            set { this.payTypeID = value; }
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        [DataMember]
        [XmlIgnore]
        public string PayTypeName
        {
            get { return this.payTypeName; }
            set { this.payTypeName = value; }
        }

        /// <summary>
        /// 是否是货到付款
        /// </summary>
        [DataMember]
        public int IsPayWhenRecv
        {
            get { return this.isPayWhenRecv; }
            set { this.isPayWhenRecv = value; }
        }

        /// <summary>
        /// 是否是在线支付
        /// </summary>
        [DataMember]
        public int IsNet
        {
            get { return this.isNet; }
            set { this.isNet = value; }
        }

        #endregion
    }
}
