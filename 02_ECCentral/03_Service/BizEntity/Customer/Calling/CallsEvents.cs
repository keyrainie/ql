using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Customer
{
    //顾客来电
    public class CallsEvents : IIdentity
    {
        /// <summary>
        /// 来电系统编号
        /// </summary>
        public int? SysNo { get; set; }
        /// <summary>
        /// 顾客系统编号
        /// </summary>
        public int? CustomerSysNo { get; set; }
        /// <summary>
        /// 顾客编号
        /// </summary>
        public string CustomerID { get; set; }
        /// <summary>
        /// 顾客姓名
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public int? OrderSysNo { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string FromLinkSource { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public CallsEventsStatus? Status { get; set; }

        /// <summary>
        /// 关闭时间
        /// </summary>
        public DateTime? CloseDate { get; set; }
        /// <summary>
        /// 关闭人
        /// </summary>
        public int? CloseUserSysNo { get; set; }
        /// <summary>
        /// 引用外部单据类型
        /// </summary>

        public CallingReferenceType? ReferenceType { get; set; }

        /// <summary>
        /// 引用外部单据编号
        /// </summary>
        public int? ReferenceSysNo { get; set; }
        /// <summary>
        /// 来电事件被重新开启的次数
        /// </summary>
        public int? ReopenCount { get; set; }
        /// <summary>
        /// 来电处理日志
        /// </summary>
        public List<CallsEventsFollowUpLog> LogList { get; set; }
        /// <summary>
        /// 来电处理时间
        /// </summary>
        public int UsedHours { get; set; }
        /// <summary>
        /// 所属渠道，仅仅只是一个标签
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode { get; set; }
        /// <summary>
        /// 创建人系统编号
        /// </summary>
        public int? CreateUserSysNo { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 上次更新时间
        /// </summary>
        public DateTime? LastEditDate { get; set; }
        /// <summary>
        /// 上次更新人
        /// </summary>
        public int? LastEditUserSysNo { get; set; }

    }
}
