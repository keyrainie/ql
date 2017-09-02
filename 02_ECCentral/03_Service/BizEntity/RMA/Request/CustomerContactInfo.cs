using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.RMA
{
    /// <summary>
    /// RMA联系人信息
    /// </summary>
    public class CustomerContactInfo : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// RMA Request系统编号
        /// </summary>
        public int? RMARequestSysNo { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string ReceiveContact { get; set; }

        /// <summary>
        /// 收货地区编号
        /// </summary>
        public int? ReceiveAreaSysNo { get; set; }

        /// <summary>
        /// 收货人
        /// </summary>
        public string ReceiveName { get; set; }

        /// <summary>
        /// 收货电话
        /// </summary>
        public string ReceivePhone { get; set; }

        /// <summary>
        /// 收货手机
        /// </summary>
        public string ReceiveCellPhone { get; set; }

        /// <summary>
        /// 收货地址
        /// </summary>
        public string ReceiveAddress { get; set; }

        /// <summary>
        /// 收货邮编
        /// </summary>
        public string ReceiveZip { get; set; }

        /// <summary>
        /// 退款支付类型
        /// </summary>
        public int? RefundPayType { get; set; }

        /// <summary>
        /// 银行支行
        /// </summary>
        public string BranchBankName { get; set; }

        /// <summary>
        /// 银行卡号
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// 持卡人姓名
        /// </summary>
        public string CardOwnerName { get; set; }
    }
}
