using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// 顾客收货地址信息
    /// </summary>
    public class ShippingAddressInfo : IIdentity
    {
        /// <summary>
        /// 收货地址系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 收货人系统编号
        /// </summary>
        public int? CustomerSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 收货人顾客ID
        /// </summary>
        public string CustomerID
        {
            get;
            set;
        }

        /// <summary>
        /// 收货人名称
        /// </summary>
        public string ReceiveName
        {
            get;
            set;
        }

        /// <summary>
        /// 联系人名称
        /// </summary>
        public string ReceiveContact
        {
            get;
            set;
        }

        /// <summary>
        /// 收货人电话
        /// </summary>
        public string ReceivePhone
        {
            get;
            set;
        }

        /// <summary>
        /// 收货人手机
        /// </summary>
        public string ReceiveCellPhone
        {
            get;
            set;
        }

        /// <summary>
        /// 收货人传真
        /// </summary>
        public string ReceiveFax
        {
            get;
            set;
        }

        /// <summary>
        /// 收货人邮编
        /// </summary>
        public string ReceiveZip
        {
            get;
            set;
        }

        /// <summary>
        /// 收货人地区
        /// </summary>
        public int? ReceiveAreaSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 收货人地址
        /// </summary>
        public string ReceiveAddress
        {
            get;
            set;
        }

        /// <summary>
        /// 收货地址简称
        /// </summary>
        public string AddressTitle
        {
            get;
            set;
        }

        /// <summary>
        /// 是否是默认收货地址
        /// </summary>
        public bool? IsDefault
        {
            get;
            set;
        }
    }
}