using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.SO
{

    /// <summary>
    /// 配送信息
    /// </summary>
    public class DeliveryInfo : IIdentity
    {

        public int? SysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 订单系统编号
        /// </summary>
        public int? OrderSysNo //OrderSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 配送原因
        /// </summary>
        public DeliveryType? Type //OrderType
        {
            get;
            set;
        }

        /// <summary>
        /// 配送员编号
        /// </summary>
        public int? DeliveryUserSysNo//DeliveryManUserSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 配送地址
        /// </summary>
        public string DeliveryAddress
        {
            get;
            set;
        }
        /// <summary>
        /// 配送地区编号
        /// </summary>
        public int? AreaSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 配送日期
        /// </summary>
        public DateTime? DeliveryDate
        {
            get;
            set;
        }
        /// <summary>
        /// 配送时间段：1表示上午，2表示下午
        /// </summary>
        public int? DeliveryTimeRange
        {
            get;
            set;
        }
        /// <summary>
        /// 配送状态
        /// </summary>
        public DeliveryStatus? Status
        {
            get;
            set;
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo 
        {
            get;
            set;
        }
        
        /// <summary>
        /// 注解
        /// </summary>
        public string Note
        {
            get;
            set;
        }
    }
}
