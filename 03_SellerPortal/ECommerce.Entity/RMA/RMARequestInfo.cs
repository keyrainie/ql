using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.RMA
{
    public class RMARequestInfo : EntityBase
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 销售单号
        /// </summary>
        public int? SOSysNo { get; set; }

        /// <summary>
        /// 顾客编号
        /// </summary>
        public int? CustomerSysNo { get; set; }

        /// <summary>
        /// 客户帐号
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 收货地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 收货地址区域编号
        /// </summary>
        public int? AreaSysNo { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 收货时间
        /// </summary>
        public DateTime? RecvTime { get; set; }

        /// <summary>
        /// 收货用户信息
        /// </summary>
        public int? RecvUserSysNo { get; set; }

        /// <summary>
        /// 收货人用户名
        /// </summary>
        public string RecvUserName { get; set; }

        /// <summary>
        /// 申请单处理状态
        /// </summary>
        public RMARequestStatus? Status { get; set; }

        /// <summary>
        /// 客户发货时间
        /// </summary>
        public DateTime? CustomerSendTime { get; set; }

        /// <summary>
        /// 是否是物流拒收
        /// </summary>
        public bool IsRejectRMA { get; set; }

        /// <summary>
        /// 收货仓库编号
        /// </summary>
        public string ReceiveWarehouse { get; set; }

        /// <summary>
        /// 送货方式
        /// </summary>
        public string ShipViaCode { get; set; }

        /// <summary>
        /// 邮包编号
        /// </summary>
        public string TrackingNumber { get; set; }

        /// <summary>
        /// 申请单编号
        /// </summary>
        public string RequestID { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 单件列表
        /// </summary>
        public List<RMARegisterInfo> Registers { get; set; }

        /// <summary>
        /// 是否发短信
        /// </summary>       
        public bool? IsReceiveMsg { get; set; }

        /// <summary>
        /// 服务编码
        /// </summary>
        public string ServiceCode { get; set; }

        /// <summary>
        /// 审核人编号
        /// </summary>
        public int? AuditUserSysNo { get; set; }

        /// <summary>
        /// 审核人名称
        /// </summary>
        public string AuditUserName { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditTime { get; set; }
    }
}
