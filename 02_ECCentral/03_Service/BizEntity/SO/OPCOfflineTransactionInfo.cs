using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.SO
{
    /// <summary>
    /// 订单处理中心信息。
    /// </summary>
    public class OPCOfflineInfo : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo  //TransactionNumber
        { get; set; }
        /// <summary>
        /// 订单系统编号
        /// </summary>
        public int SOSysNo //SONumber
        { get; set; }
        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public int CustomerSysNo //CustomerNumber
        { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public WMSAction ActionType { get; set; }
        /// <summary>
        /// 是否需要回调处理
        /// </summary>
        public bool NeedResponse { get; set; }

        /// <summary>
        /// 处理状态
        /// </summary>
        public OPCStatus Status { get; set; }

        /// <summary>
        /// 添加用户编号
        /// </summary>
        public int InUserSysNo { get; set; }

        /// <summary>
        /// 添加用户
        /// </summary>
        public string InUser { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime? Indate { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime? SentDate { get; set; }

        /// <summary>
        /// 关闭时间
        /// </summary>
        public DateTime? CloseDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 来源系统名称
        /// </summary>
        public string FromSystem { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 事务信息
        /// </summary>
        public List<OPCOfflineTransactionInfo> Transactions;

        /// <summary>
        /// 回调类型
        /// </summary>
        public OPCCallBackType CallBackService { get; set; }

        /// <summary>
        /// 订单XML序列化后的信息
        /// </summary>
        public string Body { get; set; }
    }
    /// <summary>
    /// 订单处理事务信息
    /// </summary>
    public class OPCOfflineTransactionInfo : IIdentity
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
        /// 仓库编号
        /// </summary>
        public int StockSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 仓库名称（用于数据读取）
        /// </summary>
        public string StockName
        {
            get;
            set;
        }
        /// <summary>
        /// 处理状态
        /// </summary>
        public OPCTransStatus Status { get; set; }

        /// <summary>
        /// 操作标识
        /// </summary>
        public string OperationFlag { get; set; }

        /// <summary>
        /// 抢购标识
        /// </summary>
        public int FailedFlag { get; set; }
        /// <summary>
        /// 添加用户编号
        /// </summary>
        public int InUserSysNo { get; set; }

        /// <summary>
        /// 添加用户
        /// </summary>
        public string InUser { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime? InDate { get; set; }
        /// <summary>
        /// 回调时间
        /// </summary>
        public DateTime? ResponseDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 订单处理系统编号
        /// </summary>
        public int MasterID { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode { get; set; }
    }

    /// <summary>
    /// 订单处理消息实体
    /// </summary>
    public class OPCOfflineMessageInfo : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 请求信息
        /// </summary>
        public string RequestMessage { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public int SOSysNo //SONumber 
        {
            get;
            set;
        }
        /// <summary>
        /// 回调信息
        /// </summary>
        public string ResponseMessage { get; set; }
        /// <summary>
        /// 回调时间
        /// </summary>
        public DateTime? ResponseDate { get; set; }
        /// <summary>
        /// 订单处理事务编号
        /// </summary>
        public int OPCTransactionSysNo //OPCTransactionID
        {
            get;
            set;
        }
        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode { get; set; }
    }

}
