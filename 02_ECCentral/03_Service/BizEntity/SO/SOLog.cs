using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.SO
{
    /// <summary>
    /// 订单日志
    /// </summary>
    public class SOLogInfo : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 订单编号
        /// </summary>
        public int? SOSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 操作IP
        /// </summary>
        public string IP
        {
            get;
            set;
        }

        /// <summary>
        /// 操作类型
        /// </summary>
        public BizLogType? OperationType
        {
            get;
            set;
        }

        /// <summary>
        /// 操作名称
        /// </summary>
        public string OperationName
        {
            get;
            set;
        }

        /// <summary>
        /// 操作描述
        /// </summary>
        public string Note
        {
            get;
            set;
        }
        /// <summary>
        /// 记录日志的用户
        /// </summary>
        public int? UserSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 记录日志时间
        /// </summary>
        public DateTime? LogTime
        {
            get;
            set;
        }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }
    }
    /*
    /// <summary>
    /// 订单打开日志
    /// </summary>
    public class SOOpenLogInfo : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 公司编号 
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        /// <summary>
        /// 订单编号
        /// </summary>
        public int SOSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Status
        {
            get;
            set;
        }

        /// <summary>
        /// 记录日志的用户
        /// </summary>
        public int? UserSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 记录日志时间
        /// </summary>
        public DateTime? OpenTime
        {
            get;
            set;
        }

        /// <summary>
        /// 记录日志时间
        /// </summary>
        public DateTime? CloseTime
        {
            get;
            set;
        }
    }
    */
    /// <summary>
    /// 发票修改日志
    /// </summary>
    public class SOInvoiceChangeLogInfo : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }
        /// <summary>
        /// 订单编号
        /// </summary>
        public int SOSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Note
        {
            get;
            set;
        }

        /// <summary>
        /// 发票修改类型
        /// </summary>
        public InvoiceChangeType ChangeType
        {
            get;
            set;
        }

        /// <summary>
        /// 仓库号
        /// </summary>
        public int StockSysNo// WarehouseNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 记录日志的用户
        /// </summary>
        public int? UserSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 记录日志时间
        /// </summary>
        public DateTime? ChangeTime
        {
            get;
            set;
        }
    }
    /// <summary>
    /// 订单修改日志
    /// </summary>
    public class SOChangeLog : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo //TranNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 订单编号
        /// </summary>
        public int? SOSysNo //SoNumber
        {
            get;
            set;
        }
    /// <summary>
    /// 客户编号
    /// </summary>
        public int? CustomerSysNo //CustomerNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ChangeTime //ChangeOrderTime
        {
            get;
            set;
        }
        /// <summary>
        /// 修改类型
        /// </summary>
        public int? ChangeType
        {
            get;
            set;
        }
        /// <summary>
        /// 唯一编号
        /// </summary>
        public Guid Guid //rowguid
        {
            get;
            set;
        }
        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

    }
    /// <summary>
    /// 物流跟踪信息
    /// </summary>
    public class SOLogisticsInfo
    {
        /// <summary>
        /// 顺丰状态码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 接受时间
        /// </summary>
        public string AcceptTime { get; set; }
        /// <summary>
        /// 接受地点
        /// </summary>
        public string AcceptAddress { get; set; }
        /// <summary>
        /// 圆通接受人
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 圆通状态值
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 物流类型
        /// </summary>
        public ExpressType Type { get; set; }
    }
}
