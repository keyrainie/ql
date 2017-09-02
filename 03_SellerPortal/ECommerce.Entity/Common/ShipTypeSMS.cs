using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Common
{
    /// <summary>
    /// 支付方式短信类型实体
    /// </summary>
    public class ShipTypeSMS
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode { get; set; }
        /// <summary>
        /// 配送方式
        /// </summary>
        public int? ShipTypeSysNo { get; set; }

        /// <summary>
        /// 短信类型
        /// </summary>
        public SMSType? SMSType { get; set; }

        /// <summary>
        /// 短信内容
        /// </summary>
        public string SMSContent { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ShipTypeSMSStatus? Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }


    }
}
